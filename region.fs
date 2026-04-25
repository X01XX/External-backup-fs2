\ Implement a region struct and functions.
\
\ The region is a series of trits, representing a span of 2^N power squares,
\ in a K-Map of a number of bits, less the number of bits in a unsigned forth cell.
\
\ The bit limitation is for one Domain, but there can be a number of domains.
\
\ A number of trits equal to the number of bits in a whole forth cell may be possible,
\ but the sign bit may be a problem.
\
\ The region can do this with any two states/numbers. The states may be the same for a single-state
\ "region" in a K-Map.
\
\ In the action-incompatible-pairs list, regions are used as a two-state store, the states being not-equal.
\
\ Order of the states does not matter, although it can be seen in a printed region.
\ XxXx is state-0: 1010 and state-1: 0101.

#19317 constant region-id
    #3 constant region-struct-number-cells

\ Struct fields
0                           constant region-header-disp   \ 16-bits [0] struct id [1] use count.
region-header-disp  cell+   constant region-state-0-disp  \ First state value.
region-state-0-disp cell+   constant region-state-1-disp  \ Second state value.

0 value region-mma \ Storage for region mma instance.

\ Init region mma, return the addr of allocated memory.
: region-mma-init ( num-items -- ) \ sets region-mma.
    dup 1 <
    abort" region-mma-init: Invalid number of items."

    cr ." Initializing Region store."
    region-struct-number-cells swap mma-new to region-mma
;

\ Check instance type.
: is-allocated-region ( addr -- flag )
    dup region-mma mma-is-item  \ addr bool
    if
        get-first-word          \ w t | f
        if
            region-id =         \ bool
        else
            false               \ f
        then
    else
        drop
        false                   \ f
    then
;

\ Check TOS for region, unconventional, leaves stack unchanged.
: assert-tos-is-region ( tos -- tos )
    dup is-allocated-region
    if exit then

    s" TOS is not an allocated region"
    .abort-xt execute
;

\ Check NOS for region, unconventional, leaves stack unchanged.
: assert-nos-is-region ( nos tos -- nos tos )
    over is-allocated-region
    if exit then

    s" NOS is not an allocated region"
    .abort-xt execute
;

\ Check 3OS for region, unconventional, leaves stack unchanged.
: assert-3os-is-region ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-region
    if exit then

    s" 3OS is not an allocated region"
    .abort-xt execute
;

\ Check 4OS for region, unconventional, leaves stack unchanged.
: assert-4os-is-region ( 4os 3os nos tos -- 4os 3os nos tos )
    #3 pick is-allocated-region
    if exit then

    s" 4OS is not an allocated region"
    .abort-xt execute
;

\ Check 5OS for region, unconventional, leaves stack unchanged.
: assert-5os-is-region ( 5os 4os 3os nos tos -- 5os 4os 3os nos tos )
    #4 pick is-allocated-region
    if exit then

    s" 5OS is not an allocated region"
    .abort-xt execute
;

\ Start accessors.

\ Return the state-0 field from a region instance.
: region-get-state-0 ( reg0 -- sta0 )
    \ Check arg.
    assert-tos-is-region

    region-state-0-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the state-1 field from a region instance.
: region-get-state-1 ( reg0 -- sta1 )
    \ Check arg.
    assert-tos-is-region

    \ Get second state.
    region-state-1-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the state-0 field from a region instance, use only in this file.
: _region-set-state-0 ( sta1 reg0 -- )
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-value

    region-state-0-disp +   \ Add offset.
    !                       \ Set the field.
;

\ Set the state-1 field from a region instance, use only in this file.
: _region-set-state-1 ( sta1 reg0 -- )
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-value

    region-state-1-disp +   \ Add offset.
    !                       \ Set the field.
;

\ End accessors.

\ Create a region from two numbers on the stack.
\ The numbers may be the same.
: region-new ( u1 u0 -- reg )
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value
    2dup value-same-num-bits? false? abort" values use a different number of bits?"

    \ Allocate space.
    region-id region-mma        \ u1 u0 id mma
    struct-allocate             \ u1 u0 reg

    \ Prepare to store states.
    -rot                        \ reg u1 u0
    #2 pick                     \ reg u1 u0 reg
    tuck                        \ reg u1 reg u0 reg

    \ Store states
    _region-set-state-0         \ reg u1 reg
    _region-set-state-1         \ reg
;

\ Print a region.
: .region ( reg0 -- )
    \ Check arg.
    assert-tos-is-region

    \ Setup for trit-position loop.
    dup  region-get-state-1         \ reg0 sta1
    _value-get-number               \ reg1 num1
    swap region-get-state-0         \ num1 sta0
    dup                             \ num1 sta0 sta0
    _value-get-number               \ num1 sta0 num0
    swap                            \ num1 num0 sta0
    value-calc-msb                  \ num1 num0 ms-bit

    \ Process each trit.
    begin
      dup
    while
        \ Apply msb to state 1.
        #2 pick                     \ num1 num0 ms-bit num1
        over                        \ num1 num0 ms-bit num1 ms-bit
        and                         \ num1 num0 ms-bit num1-bit

        \ Apply msb to state 0.
        #2 pick                     \ num1 num0 ms-bit num1-bit num0
        #2 pick                     \ num1 num0 ms-bit num1-bit num0 ms-bit
        and                         \ num1 num0 ms-bit num1-bit num0-bit


        if                          \ num1 num0 ms-bit num1-bit
            if                      \ num1 num0 ms-bit
                ." 1"
            else
                ." X"
            then
        else                        \ num1 num0 ms-bit num1-bit
            if                      \ num1 num0 ms-bit
                ." x"
            else
                ." 0"
            then
        then

        1 rshift                    \ sta1 sta0 ms-bit\2
    repeat
                                    \ st2 st1 ms-bit
    3drop
;

\ Deallocate a region.
: region-deallocate ( reg0 -- )
    \ Check arg.
    assert-tos-is-region

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate states.
        dup region-get-state-0 value-deallocate
        dup region-get-state-1 value-deallocate

        \ Deallocate instance.
        region-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

