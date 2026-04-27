\ The state struct, storing a state number.
#61717 constant state-id
    #2 constant state-struct-number-cells

\ State struct fields.
0                         constant state-header-disp   \ 16 bits, [0] id, [1] use count [2] Number bits ( 8 bits ). 
state-header-disp cell+   constant state-number-disp

0 value state-mma \ Storage for the state mma instance addr.

\ Init state mma.
: state-mma-init ( num-items -- ) \ sets state-mma.
    dup 1 <
    if
        ." state-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing State store."
    state-struct-number-cells swap mma-new to state-mma
;

\ Check instance type.
: is-allocated-state ( tos -- flag )
    dup state-mma mma-is-item  \ addr bool
    if
        get-first-word          \ w t | f
        if
            state-id =          \ bool
        else
            false               \ f
        then
    else
        drop
        false                   \ f
    then
;

\ Check TOS for state, unconventional, leaves stack unchanged.
: assert-tos-is-state ( tos -- tos )
    dup is-allocated-state
    if exit then

    s" TOS is not an allocated state"
    .abort-xt execute
;

\ Check NOS for state, unconventional, leaves stack unchanged.
: assert-nos-is-state ( nos tos -- nos tos )
    over is-allocated-state
    if exit then

    s" NOS is not an allocated state"
    .abort-xt execute
;

\ Start accessors.

\ Get the number of bits.
: state-get-number-bits ( sta0 -- nb )
    \ Check arg.
    assert-tos-is-state

    4c@
;

\ Set the number of bits.
: _state-set-number-bits ( nb sta0 -- )
    4c!
;

\ Get state number.
: _state-get-number ( sta0 -- lst0 )
    state-number-disp + @
;

\ Set state number.
: _state-set-number ( lst1 sta0 -- )
    state-number-disp + !
;

\ Return a new state struct instance address, with given data list and number bits.
: state-new ( num1 nb0 -- val )
    \ Check args.

    \ Check number bits.
    dup 1 < abort" Number bits < 1?"
    dup cell-bits > abort" Number bits too large"

    \ Check number.
    2dup                    \ num1 nb0 num1 nb0
    _max-num-from-num-bits  \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a state instance.
    state-id state-mma      \ num1 nb0 id mma
    struct-allocate         \ num1 nb0 val

    \ Set number bits.
    tuck                    \ num1 val nb0 val
    _state-set-number-bits  \ num1 val

    \ Store number given.
    tuck                     \ val num1 val
    _state-set-number        \ val
;

\ Print a state struct instance.
: .state ( val -- )
   \ Check arg.
    assert-tos-is-state

    \ Setup for bit-position loop.
    dup _state-get-number       \ sta0 num
    swap                        \ num sta0
    state-get-number-bits       \ num nb
    _msb-from-num-bits          \ num ms-bit

    \ Process each bit.
    begin
      ?dup
    while
      \ Apply msb to state, to get an isolated bit.
      2dup
      and                   \ sta0 ms-bit bit

      if  
        ." 1"
      else
        ." 0"
      then

      1 rshift              \ sta0 ms-bit
    repeat
    drop                    \   
;

\ Return true if two states are equal.
: state-eq ( sta1 sta0 -- flag )
    _state-get-number   \ sta1 lst0
    swap                \ lst0 sta1
    _state-get-number   \ lst0 lst1

    =
;

\ Deallocate a state.
: state-deallocate ( val -- )
    \ Check arg.
    assert-tos-is-state

    dup struct-get-use-count    \ val count

    dup 0< abort" invalid use count"

    #2 <
    if
        state-mma mma-deallocate \ Deallocate instance.
    else
        struct-dec-use-count
    then
;

\ Return true if two states have the same number bits.
: state-same-num-bits? ( sta1 sta0 -- flag )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state

    state-get-number-bits   \ sta1 nb0
    swap                    \ nb0 sta1
    state-get-number-bits   \ nb0 nb1
    =
;

\ Return true if a state and a mask have the same number bits.
: state-same-num-bits-as-mask? ( msk1 sta0 -- flag )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask

    state-get-number-bits   \ msk1 nb0
    swap                    \ nb0 Rmsksta1
    mask-get-number-bits    \ nb0 nb1
    =
;

\ Return a state inverted, as a mask.
: state-invert ( sta0 -- mask )
    \ Check arg.
    assert-tos-is-state

    dup                         \ sta0 sta0
    state-get-number-bits       \ sta0 nb
    _max-num-from-num-bits      \ sta0 max   

    over                        \ sta0 max sta0
    _state-get-number           \ sta0 max num

    xor                         \ sta0 invert

    swap                        \ invert sta0 
    state-get-number-bits       \ invert nb
    mask-new                    \ msk
;

\ Return the Boolean and of two states, as a mask.
: state-and ( sta1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup state-same-num-bits? false? abort" states do not have the same number bits?"

    over _state-get-number  \ sta1 sta0 num1
    swap _state-get-number  \ sta1 num1 num0
    and                     \ sta1 num
    swap                    \ num sta1
    state-get-number-bits   \ num nb
    mask-new                \ msk
;

\ Return the Boolean and of two states, as a mask.
: state-and-mask ( msk1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask
    2dup state-same-num-bits-as-mask? false? abort" state and mask do not have the same number bits?"

    over _mask-get-number   \ msk1 sta0 num1
    swap _state-get-number  \ msk1 num1 num0
    and                     \ msk1 num
    swap                    \ num msk1
    mask-get-number-bits    \ num nb
    mask-new                \ msk
;

\ Return the state of a given bit number.
: state-bit ( u1 sta0 -- bit )
    \ Check arg.
    assert-tos-is-state

    over                \ u1 sta0 u1
    0< abort" Invalid bit number?"
    2dup                \ u1 sta0 u1 sta0
    state-get-number-bits
    > abort" Invalid bit number?"

    _state-get-number   \ u1 num
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

