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
\ "region" in a K-Map ( extended to allow up to one cell number of bits ).
\
\ In the action-incompatible-pairs list, regions are used as a two-state store, the states being not-equal.
\
\ Order of the states does not matter, although it can be seen in a printed region.
\ XxXx is state-0: 1010 and state-1: 0101.

#19317 constant region-id
    #3 constant region-struct-number-cells

\ Struct fields
0                           constant region-header-disp   \ 16-bits [0] struct id [1] use count.
region-header-disp  cell+   constant region-state-0-disp  \ First state.
region-state-0-disp cell+   constant region-state-1-disp  \ Second state.

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
    assert-nos-is-state

    region-state-0-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Set the state-1 field from a region instance, use only in this file.
: _region-set-state-1 ( sta1 reg0 -- )
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-state

    region-state-1-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a region from two states on the stack.
\ The states may be the same.
: region-new ( sta1 sta0 -- reg )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup state-same-num-bits? false? abort" states use a different number of bits?"

    \ Allocate space.
    region-id region-mma        \ sta1 sta0 id mma
    struct-allocate             \ sta1 sta0 reg

    \ Prepare to store states.
    -rot                        \ reg sta1 sta0
    #2 pick                     \ reg sta1 sta0 reg
    tuck                        \ reg sta1 reg sta0 reg

    \ Store states
    _region-set-state-0         \ reg sta1 reg
    _region-set-state-1         \ reg
;

\ Return the number of bits used for a region.
: region-get-number-bits ( reg0 -- nb )
    \ Check arg.
    assert-tos-is-region

    region-get-state-0          \ sta
    state-get-number-bits       \ nb
;

\ Store a character representation to a given address,
\ return the number characters stored.
\ Caller must set, or accumulate, the number bits prefix
\ of the string.
: region-str ( addr1 reg0 -- nc )
    \ Check arg.
    assert-tos-is-region

    \ Store string length.
    dup region-get-number-bits      \ addr1 reg0 nc
    -rot                            \ nc addr1 reg0

    \ Setup for bit-position loop. 
    dup region-get-state-1          \ nc addr1 reg0 sta1
    -rot                            \ nc sta1 addr1 reg0
    dup region-get-state-0          \ nc sta1 addr1 reg0 sta0
    -rot                            \ nc sta1 sta0 addr1 reg0
    region-get-number-bits          \ nc sta1 sta0 addr1 nb
    -1 swap                         \ nc sta1 sta0 addr1 -1 nb
    1-                              \ nc sta1 sta0 addr1 -1 nb-

    do
        \ Process each trit.        \ nc sta1 sta0 addr1
        \ Get state bit.
        i                           \ nc sta1 sta0 addr1 i
        #3 pick                     \ nc sta1 sta0 addr1 i sta1
        state-bit                   \ nc sta1 sta0 addr1 b1

        \ Get state bit.
        i                           \ nc sta1 sta0 addr1 b1 i
        #3 pick                     \ nc sta1 sta0 addr1 b1 i sta0
        state-bit                   \ nc sta1 sta0 addr1 b1 b0

        \ Put char on stack.
        if                          \ nc sta1 sta0 addr1 b1
            if                      \ nc sta1 sta0 addr1
                [char] 1
            else
                [char] X
            then
        else                        \ nc sta1 sta0 addr1 b1
            if                      \ nc sta1 sta0 addr1
                [char] x
            else
                [char] 0
            then
        then

        \ Store char to pad.        \ nc sta1 sta0 addr1 chr
        over                        \ nc sta1 sta0 addr1 chr addr1
        c!                          \ nc sta1 sta0 addr1
                                                                                                                                                             
        \ Point to next addr1 char.
        1+                          \ nc sta1 sta0 addr1

    1 -loop
    2drop drop                      \ nc
;

\ Print a region.
: .region ( reg0 -- )
    \ Check arg.
    assert-tos-is-region

    \ Calc string target address.
    pad 1+ swap         \ addr reg0

    \ Put mask string into pad.
    region-str          \ nc

    \ Store string length.
    pad c!              \   

    \ Move pad string to stack.
    pad string@         \ c-addr u

    \ Output string.
    type
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
        dup region-get-state-0 state-deallocate
        dup region-get-state-1 state-deallocate

        \ Deallocate instance.
        region-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Get a region from a string.
\ Valid chars are 0, 1, X, x, and underscore as separator.
\ All bit positions must be specified.
: region-from-string ( c-addr u --  reg t | f)
    \ Init character counter.
    0 swap              \ c-addr cnt u

    \ Init state 1, state 0, and do initial value.
    0 swap              \ c-addr cnt num1 u
    0 swap              \ c-addr cnt num1 num0 u
    0                   \ c-addr cnt num1 num0 u 0

    \ For each character...
    do                  \ c-addr cnt num1 num0
        \ Get a character.
        #3 pick         \ c-addr cnt num1 num0 c-addr
        i +             \ c-addr cnt num1 num0 c-addr+
        c@              \ c-addr cnt num1 num0 chr

        \ Process character.
        case
            [char] 0 of
                        \ Leave bit positions as 0/0.
                        \ Update num1
                        swap 1 lshift
                        \ Update num0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] 1 of
                        \ Set bit positions to 1/1.
                        \ Update num1
                        swap 1 lshift 1+
                        \ Update num0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] X of
                        \ Set bit positions to 1/0.
                        \ Update num1
                        swap 1 lshift 1+
                        \ Update num0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] x of
                        \ Set bit positions to 0/1.
                        \ Update num1
                        swap 1 lshift
                        \ Update num0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            \ Ignore unrecognized characters.
        endcase
    loop

    \ Create states.        \ c-addr cnt num1 num0
    swap                    \ c-addr cnt num0 num1
    #2 pick                 \ c-addr cnt num0 num1 cnt
    state-new               \ c-addr cnt num0 sta1 
    -rot                    \ c-addr sta1 cnt num0
    swap                    \ c-addr sta1 num0 cnt
    state-new               \ c-addr sta1 sta0

    \ Make new region, return.
                            \ c-addr sta1 sta0
    region-new              \ c-addr reg
    nip                     \ reg
    true
;
