\ The mask struct, storing a mask number.
#61719 constant mask-id
    #2 constant mask-struct-number-cells

\ mask struct fields.
0                       constant mask-header-disp   \ 16 bits, [0] id, [1] use count [2] Number bits ( 8 bits ).
mask-header-disp cell+  constant mask-number-disp

0 value mask-mma    \ Storage for the mask mma instance addr.

1 cells 8 * value cell-bits

\ Init mask mma.
: mask-mma-init ( num-items -- )    \ sets mask-mma.
    dup 1 <
    if
        ." mask-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Mask store."
    mask-struct-number-cells swap mma-new to mask-mma
;

\ Check instance type.
: is-allocated-mask ( tos -- bool )
    dup mask-mma mma-is-item    \ addr bool
    if
        get-first-word          \ w t | f
        if
            mask-id =           \ bool
        else
            false               \ f
        then
    else
        drop
        false                   \ f
    then
;

\ Check TOS for mask, unconventional, leaves stack unchanged.
: assert-tos-is-mask ( tos -- tos )
    dup is-allocated-mask
    if exit then

    s" TOS is not an allocated mask"
    .abort-xt execute
;

\ Check NOS for mask, unconventional, leaves stack unchanged.
: assert-nos-is-mask ( nos tos -- nos tos )
    over is-allocated-mask
    if exit then

    s" NOS is not an allocated mask"
    .abort-xt execute
;

\ Start accessors.

\ Get the number of bits.
: mask-get-number-bits ( msk0 -- nb )
    \ Check arg.
    assert-tos-is-mask

    4c@
;

\ Set the number of bits.
: _mask-set-number-bits ( nb msk0 -- )
    4c!
;

\ Get mask number.
: _mask-get-number ( msk0 -- lst0 )
    mask-number-disp + @
;

\ Set mask number.
: _mask-set-number ( lst1 msk0 -- )
    mask-number-disp + !
;

\ Return the msb for a given number of bits.
\ e.g. 8 for 4 bits.
: _msb-from-num-bits ( nb0 -- msb )
    1 swap
    1-
    lshift
 ;

\ Return the maximum number for a given number of bits.
\ e.g. 15 for 4 bits.
: _max-num-from-num-bits ( nb0 -- num )
    _msb-from-num-bits
    1-
    1 lshift
    1+
;

\ Return a new mask struct instance address, with given data list and number bits.
: mask-new ( num1 nb0 -- msk )
    \ Check args.

    \ Check number bits.
    dup 1 < abort" Number bits < 1?"
    dup cell-bits > abort" Number bits too large"

    \ Check number.
    2dup                    \ num1 nb0 num1 nb0
    _max-num-from-num-bits  \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a mask instance.
    mask-id mask-mma        \ num1 nb0 id mma
    struct-allocate         \ num1 nb0 msk

    \ Set number bits.
    tuck                    \ num1 msk nb0 msk
    _mask-set-number-bits   \ num1 msk

    \ Store number given.
    tuck                    \ msk num1 msk
    _mask-set-number        \ msk
;

\ Store a string representation of a mask to pad.
: mask-str ( msk0 -- uc-addr )
    \ Check arg.
    assert-tos-is-mask

    \ Save pad pointer.
    pad swap                    \ pad msk0

    \ Store string length.
    dup mask-get-number-bits    \ pad msk0 nb
    #2 pick                     \ pad msk0 nb pad
    c!                          \ pad msk0

    \ Point to next pad char.
    swap 1+ swap                \ pad msk0
   
    \ Setup for bit-position loop.
    dup _mask-get-number        \ pad msk0 num
    swap                        \ pad num msk0
    mask-get-number-bits        \ pad num nb
    tuck                        \ pad nb num nb
    _msb-from-num-bits          \ pad nb num ms-bit
    rot                         \ pad num ms-bit nb
    0

    do
        \ Apply msb to mask, to get an isolated bit.
                                \ pad num ms-bit
        2dup and                \ pad num ms-bit bit

        if  
            [char] 1            \ pad num ms-bit chr
        else
            [char] 0            \ pad num ms-bit chr
        then

        \ Store char to pad.
        #3 pick                 \ pad num ms-bit chr pad
        c!                      \ pad num ms-bit
      
        \ Point to next pad char.
        rot 1+ -rot             \ pad num ms-bit

        \ Adjust msb mask.
        1 rshift                \ pad num ms-bit
    loop
    2drop drop pad              \ pad
;

\ Print a mask.
: .mask ( msk -- )
    \ Check arg.
    assert-tos-is-mask

    \ Put mask string into pad.
    mask-str        \ uc-addr

    \ Move pad string to stack.
    string@         \ c-adr u

    \ Output string.
    type
;

\ Return true if two masks are equal.
: mask-eq ( msk1 msk0 -- flag )
    _mask-get-number    \ msk1 lst0
    swap                \ lst0 msk1
    _mask-get-number    \ lst0 lst1

    =
;

\ Deallocate a mask.
: mask-deallocate ( msk -- )
    \ Check arg.
    assert-tos-is-mask

    dup struct-get-use-count    \ msk count

    dup 0< abort" invalid use count"

    #2 <
    if
        mask-mma mma-deallocate \ Deallocate instance.
    else
        struct-dec-use-count
    then
;

\ Return true if two masks have the same number bits.
: mask-same-num-bits? ( msk1 msk0 -- flag )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask

    mask-get-number-bits    \ msk1 nb0
    swap                    \ nb0 msk1
    mask-get-number-bits    \ nb0 nb1
    =
;

\ Return a mask inverted.
: mask-invert ( msk0 -- msk )
    \ Check arg.
    assert-tos-is-mask

    dup                         \ msk0 msk0
    mask-get-number-bits        \ msk0 nb
    _max-num-from-num-bits      \ msk0 max   

    over                        \ msk0 max msk0
    _mask-get-number            \ msk0 max num

    xor                         \ msk0 invert

    swap                        \ invert msk0 
    mask-get-number-bits        \ invert nb
    mask-new                    \ msk
;

\ Return the Boolean and of two masks.
: mask-and ( msk1 msk0 -- msk )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask
    2dup mask-same-num-bits? false? abort" masks do not have the same number bits?"

    over _mask-get-number  \ msk1 msk0 num1
    swap _mask-get-number  \ msk1 num1 num0
    and                    \ msk1 num
    swap                   \ num msk1
    mask-get-number-bits   \ num nb
    mask-new               \ msk
;

\ Return the mask of a given bit number.
: mask-bit ( u1 msk0 -- bit )
    \ Check arg.
    assert-tos-is-mask

    over                \ u1 msk0 u1
    0< abort" Invalid bit number?"
    2dup                \ u1 msk0 u1 msk0
    mask-get-number-bits
    > abort" Invalid bit number?"

    _mask-get-number    \ u1 num
    swap                \ num u1
    1 swap              \ num 1 u1
    lshift              \ num msk
    and                 \ bit ( could be 0, 1, 2, 4, 8 etc. )
    if
        1
    else
        0
    then
;

