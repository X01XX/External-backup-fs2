\ The value struct, storing a value number.
#61717 constant value-id
    #2 constant value-struct-number-cells

\ Float struct fields.
0                         constant value-header-disp   \ 16 bits, [0] id, [1] use count [2] Number bits.
value-header-disp cell+   constant value-number-disp

0 value value-mma \ Storage for the value mma instance addr.

1 cells 8 * value max-bits

\ Init value mma.
: value-mma-init ( num-items -- ) \ sets value-mma.
    dup 1 <
    if
        ." value-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Value store."
    value-struct-number-cells swap mma-new to value-mma
;

\ Check instance type.
: is-allocated-value ( tos -- flag )
    dup value-mma mma-is-item  \ addr bool
    if
        get-first-word          \ w t | f
        if
            value-id =         \ bool
        else
            false               \ f
        then
    else
        drop
        false                   \ f
    then
;

\ Check TOS for value, unconventional, leaves stack unchanged.
: assert-tos-is-value ( tos -- tos )
    dup is-allocated-value
    if exit then

    s" TOS is not an allocated value"
    .abort-xt execute
;

\ Check NOS for value, unconventional, leaves stack unchanged.
: assert-nos-is-value ( nos tos -- nos tos )
    over is-allocated-value
    if exit then

    s" NOS is not an allocated value"
    .abort-xt execute
;

\ Start accessors.

\ Get the number of bits.
: value-get-number-bits ( val0 -- nb )
    \ Check arg.
    assert-tos-is-value

    2w@
;

\ Set the number of bits.
: _value-set-number-bits ( nb val0 -- )
    2w!
;

\ Get value number.
: _value-get-number ( val0 -- lst0 )
    value-number-disp + @
;

\ Set value number.
: _value-set-number ( lst1 val0 -- )
    value-number-disp + !
;

\ Return the msb for a given number of bits.
\ e.g. 8 for 4 bits.
: msb-from-num-bits ( nb0 -- msb )
    1 swap
    1-
    lshift
;

\ Return the msb for a value.
: value-calc-msb ( val0 -- msb )
    \ Check arg.
    assert-tos-is-value

    value-get-number-bits   \ nb
    msb-from-num-bits       \ msb
;

\ Return the maximum number for a given number of bits.
\ e.g. 15 for 4 bits.
: max-num-from-num-bits ( nb0 -- num )
    msb-from-num-bits
    1-
    1 lshift
    1+
;

\ Return the maximum number for a value.
: value-calc-max-num ( val0 -- msb )
    \ Check arg.
    assert-tos-is-value

    value-get-number-bits   \ nb
    max-num-from-num-bits   \ msb
;

\ Return the maximum number for a given number of bits.
\ e.g. 15 for 4 bits.
\ Return a new value struct instance address, with given data list and number bits.
: value-new ( num1 nb0 -- val )
    \ Check args.

    \ Check number bits.
    dup 1 < abort" Number bits < 1?"
    dup max-bits > abort" Number bits too large"

    \ Check number.
    2dup                    \ num1 nb0 num1 nb0
    max-num-from-num-bits   \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a value instance.
    value-mma mma-allocate   \ lst0 nb val

    \ Set struct id.
    value-id over            \ lst0 nb val id val
    struct-set-id            \ lst0 nb val

    \ Set use count
    0 over                   \ lst0 nb val 0 val
    struct-set-use-count     \ lst0 nb val

    \ Set number bits.
    tuck                    \ lst0 val nb val
    _value-set-number-bits  \ lst0 val

    tuck                     \ val lst0 val
    _value-set-number        \ val
;

\ Print a value struct instance.
: .value ( val -- )
   \ Check arg.
    assert-tos-is-value

    \ Setup for bit-position loop.
    dup _value-get-number       \ val0 num
    swap                        \ num val0
    value-calc-msb              \ num ms-bit

    \ Process each bit.
    begin
      ?dup
    while
      \ Apply msb to value, to get an isolated bit.
      2dup
      and                   \ val0 ms-bit bit

      if  
        ." 1"
      else
        ." 0"
      then

      1 rshift              \ val0 ms-bit
    repeat
    drop                    \   
;

\ Return true if two values are equal.
: value-eq ( val1 val0 -- flag )
    _value-get-number   \ val1 lst0
    swap                \ lst0 val1
    _value-get-number   \ lst0 lst1

    =
;

\ Deallocate a value.
: value-deallocate ( val -- )
    \ Check arg.
    assert-tos-is-value

    dup struct-get-use-count    \ val count

    dup 0< abort" invalid use count"

    #2 <
    if
        value-mma mma-deallocate \ Deallocate instance.
    else
        struct-dec-use-count
    then
;

\ Return true if two values have the same number bits.
: value-same-num-bits? ( val1 val0 -- flag )
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    value-get-number-bits   \ val1 nb0
    swap                    \ nb0 val1
    value-get-number-bits   \ nb0 nb1
    =
;

\ Return a value inverted.
: value-invert ( val0 -- val )
    \ Check arg.
    assert-tos-is-value

    dup value-calc-max-num      \ val0 max
    over                        \ val0 max val0
    _value-get-number           \ val0 max num
    xor                         \ val0 invert
    swap                        \ invert val0 
    value-get-number-bits       \ invert nb
    value-new                   \ val
;

\ Return the Boolean and of two values.
: value-and ( val1 val0 -- val )
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value
    2dup value-same-num-bits? false? abort" values do not have the same number bits?"

    over _value-get-number  \ val1 val0 num1
    swap _value-get-number  \ val1 num1 num0
    and                     \ val1 num
    swap                    \ num val1
    value-get-number-bits   \ num nb
    value-new               \ val
;

