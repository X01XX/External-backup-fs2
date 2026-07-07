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

#19317 constant region-struct-id
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
: is-allocated-region? ( addr -- flag )
    dup region-mma mma-is-item? \ addr bool
    if
        struct-get-id
        region-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for region.
: is-region? ( tos -- t )
    dup is-allocated-region?
    if drop true exit then

    s" not an allocated region"
    .abort-xt execute
;

\ Start accessors.

\ Return the state-0 field from a region instance.
: region-get-state-0 ( reg0 -- sta0 )
    \ Check arg.
    assert( tos is-region? )

    region-state-0-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the state-1 field from a region instance.
: region-get-state-1 ( reg0 -- sta1 )
    \ Check arg.
    assert( tos is-region? )

    \ Get second state.
    region-state-1-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the state-0 field from a region instance, use only in this file.
: _region-set-state-0 ( sta1 reg0 -- )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-state? )

    region-state-0-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Set the state-1 field from a region instance, use only in this file.
: _region-set-state-1 ( sta1 reg0 -- )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-state? )

    region-state-1-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a region from two states on the stack.
\ The states may be the same.
: region-new ( sta1 sta0 -- reg )
    \ Check args.
    assert( tos is-state? )
    assert( nos is-state? )
    assert( 2dup states-same-num-bits? )

    \ Allocate space.
    region-struct-id region-mma \ sta1 sta0 id mma
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
: region-get-num-bits ( reg0 -- nb )
    \ Check arg.
    assert( tos is-region? )

    region-get-state-0          \ sta
    state-get-num-bits       \ nb
;

\ Print a region.
: .region ( reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    \ Print prefix.
    [char] r emit

    \ Setup for bit-position loop.
    dup region-get-state-1          \ reg0 sta1
    swap                            \ sta1 reg0
    dup region-get-state-0          \ sta1 reg0 sta0
    swap                            \ sta1 sta0 reg0
    region-get-num-bits             \ sta1 sta0 nb
    -1 swap                         \ sta1 sta0 -1 nb
    1-                              \ sta1 sta0 -1 nb-

    do
        \ Process each trit.        \ sta1 sta0
        \ Get state bit.
        i                           \ sta1 sta0 i
        #2 pick                     \ sta1 sta0 i sta1
        state-bit                   \ sta1 sta0 b1

        \ Get state bit.
        i                           \ sta1 sta0 b1 i
        #2 pick                     \ sta1 sta0 b1 i sta0
        state-bit                   \ sta1 sta0 b1 b0

        \ Put char on stack.
        if                          \ sta1 sta0 b1
            if                      \ sta1 sta0
                [char] 1 emit
            else
                [char] X emit
            then
        else                        \ sta1 sta0 b1
            if                      \ sta1 sta0
                [char] x emit
            else
                [char] 0 emit
            then
        then

    1 -loop
    2drop
;

\ Deallocate a region.
: region-deallocate ( reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" region-deallocate: Invalid use count"

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

\ Return false if a string is not a representation of a region.
\
\ Otherwise, generate a region from the string.
\ Valid chars are 0, 1, X, x, and underscore as separator.
\ All bit positions must be specified.
\ Like s" r01Xx" region-from-string
: region-from-string ( c-addr u --  reg t | f)

    \ Check length GT 1.
    dup #2 <
    if
        2drop
        false
        exit
    then

    \ Check for prefix char.
    over c@ [char] r <>
    if
        2drop
        false
        exit
    then

    \ Inc address.
    swap 1+ swap

    \ Dec len.
    1-

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
                        swap 1 lshift
                        \ Update num0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] x of
                        \ Set bit positions to 0/1.
                        \ Update num1
                        swap 1 lshift 1+
                        \ Update num0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] _ of
                    endof
            \ Unrecognized character, return false.

            \ Drop stack items.
            2drop
            2drop
            drop

            \ Set return bool.
            false

            \ Cancel do loop.
            unloop

            \ Return.
            exit
        endcase
    loop

    \ Create state 1.       \ c-addr cnt num1 num0
    swap                    \ c-addr cnt num0 num1
    #2 pick                 \ c-addr cnt num0 num1 cnt
    state-new               \ c-addr cnt num0 sta1

    \ Create state 0.
    -rot                    \ c-addr sta1 cnt num0
    swap                    \ c-addr sta1 num0 cnt
    state-new               \ c-addr sta1 sta0

    \ Make new region, return.
                            \ c-addr sta1 sta0
    region-new              \ c-addr reg
    nip                     \ reg
    true
;

\ Return a region from a string, or abort.
: region-from-string-a ( c-addr u -- reg )
    region-from-string    \ reg t | f
    invert abort" region-from-string failed."
;

\ Return a region's x mask.
: region-calc-x-mask ( reg0 -- x-msk' )
    \ Check arg.
    assert( tos is-region? )

    \ Get states.
    dup  region-get-state-0     \ reg0 sta-0
    swap region-get-state-1     \ sta-0 sta-1

    \ Calc x mask.
    state-xor-to-mask           \ x-msk'
;

\ Return a region's zeros mask.
: region-calc-0-mask ( reg0 -- 0-msk' )
    \ Check arg.
    assert( tos is-region? )

    \ Get states.
    dup  region-get-state-0     \ reg0 sta-0
    swap region-get-state-1     \ sta-0 sta-1

    \ Invert states.
    state-invert-to-mask        \ sta-0 msk-1'
    swap                        \ msk-1' sta-0
    state-invert-to-mask        \ msk-1' msk-0'

    \ Calc zeros mask.
    2dup                        \ msk-1' msk-0' msk-1' msk-0'
    mask-and                    \ msk-1' msk-0' 0-msk'

    \ Clean up.
    swap mask-deallocate        \ msk-1' 0-msk'
    swap mask-deallocate        \ 0-msk'
;

\ Return a region's ones mask.
: region-calc-1-mask ( reg0 -- 1-msk' )
    \ Check arg.
    assert( tos is-region? )

    \ Get states.
    dup  region-get-state-0     \ reg0 sta-0
    swap region-get-state-1     \ sta-0 sta-1

    \ Calc ones mask.
    state-and-state-to-mask     \ 1-msk'
;

\ Return the two states that make a region.
: region-get-states ( reg0 -- sta1 sta0 )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    dup region-get-state-1  \ reg0 sta1
    swap                    \ sta1 reg0
    region-get-state-0      \ sta1 sta0
;

\ Return true if a region uses a given state.
: region-uses-state? ( sta1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )

    region-get-states           \ sta1 reg-sta1 reg-sta0
    #2 pick                     \ sta1 reg-sta1 reg-sta0 sta1
    =                           \ sta1 reg-sta1 flag
    if                          \ sta1 reg-sta1
        2drop
        true
        exit
    then

                                \ sta1 reg-sta1
    =                           \ flag
;

\ Return a new region with some X positions set to zero.
\ Change 1-0 or 0-1 to 0-0.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-0 ( to-0-msk reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-mask? )
    over mask-get-num-bits over region-get-num-bits <> abort" region-x-to-0: num bits ne?"

    region-get-states       \ to-0-msk sta1 sta0
    rot mask-invert         \ sta1 sta0 keep-msk'
    tuck swap               \ sta1 keep-msk' keep-msk' sta0
    state-and-mask          \ sta1 keep-msk' sta0-new
    -rot                    \ sta0-new sta1 keep-msk'
    tuck swap               \ sta0-new keep-msk' keep-msk' sta1
    state-and-mask          \ sta0-new keep-msk' sta1-new
    swap mask-deallocate    \ sta0-new sta1-new
    swap                    \ sta1-new sta0-new
    region-new              \ reg
;

\ Return a new region with some X positions set to one.
\ Change 1-0 or 0-1 to 1-1.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-1 ( to-1-msk reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-mask? )
    over mask-get-num-bits over region-get-num-bits <> abort" region-x-to-1: num bits ne?"

    region-get-states       \ to-1-msk sta1 sta0
    #2 pick swap            \ to-1-msk sta1 to-1-msk sta0
    state-or-mask           \ to-1-msk sta1 sta0-new
    -rot                    \ sta0-new to-1-msk sta1
    state-or-mask           \ sta0-new sta1-new
    swap                    \ sta1-new sta0-new
    region-new              \ reg
;

\ Return a regions edge mask,
\ trits that are 0, or 1.
: region-edge-mask ( reg0 -- msk )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    region-calc-x-mask      \ x-msk'
    dup mask-invert         \ x-msk' msk-edg
    swap mask-deallocate    \ msk-edg
;

\ Return true if two regions have a different number of bits.
: regions-dif-num-bits? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    region-get-num-bits  \ reg1 nb0
    swap                 \ nb0 reg1
    region-get-num-bits  \ nb0 nb1
    <>
;

\ Return true if two regions have the same number of bits.
: regions-same-num-bits? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    region-get-num-bits  \ reg1 nb0
    swap                 \ nb0 reg1
    region-get-num-bits  \ nb0 nb1
    =
;

\ Return true if two regions intersect, no corresponding
\ trits are 0 and 1.
: region-intersects? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )
    \ cr ." region-intersects: reg1: " over .region space ." reg0: " dup .region cr

    \ Get different bits mask of any pair states from reg1 and reg0.
    over region-get-state-0     \ reg1 reg0 reg1-sta0
    over region-get-state-0     \ reg1 reg0 reg1-sta0 reg0-sta0
    state-xor-to-mask           \ reg1 reg0 dif'
    -rot                        \ dif' reg1 reg2

    \ Get reg0 edge bits.
    region-edge-mask            \ dif' reg1 reg0-edg'

    \ Get reg1 edge bits.
    swap region-edge-mask       \ dif'-msk reg0-edg' reg1-edg'

    \ Get mask of same edge bits in both regions.
    2dup                        \ dif'-msk reg0-edg' reg1-edg' reg0-edg' reg1-edg'
    mask-and                    \ dif' reg1-edg' reg0-edg' edge-msk'
    swap mask-deallocate        \ dif' reg1-edg' edge-msk'
    swap mask-deallocate        \ dif' edge-msk'

    \ Get different edge bit mask.
    2dup                        \ dif' edge-msk' dif' edge-msk'
    mask-and                    \ dif' edge-msk' edge-dif-msk'
    swap mask-deallocate        \ dif' edge-dif-msk'
    swap mask-deallocate        \ edge-dif-msk'

    \ Return result
    dup mask-is-zero?           \ edge-dif-msk' bool
    swap mask-deallocate        \ bool
;

\ Return the highest state in a region.
: region-high-state ( reg0 -- n )
    \ Check arg.
    assert( tos is-region? )

    dup  region-get-state-0    \ reg0 sta0
    swap region-get-state-1    \ sta0 sta1
    state-or                   \ sta-high
;

\ Return the lowest state in a region.
: region-low-state ( reg0 -- n )
    \ Check arg.
    assert( tos is-region? )

    dup  region-get-state-0    \ reg0 sta0
    swap region-get-state-1    \ sta0 sta1
    state-and                  \ sta-low
;

\ Return the region high state and low state.
: region-high-low ( reg0 -- high low )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    dup region-high-state   \ reg0 high
    swap region-low-state   \ high low
;

\ Return the intersection of two regions, or false if they do not intersect.
\ Since this must check for intersection first, there may be no need to check
\ for intersection before calling this.
: region-intersection ( reg1 reg0 -- reg t | f )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )

    \ Check that the two regions intersect.
    2dup region-intersects?     \ reg1 reg0 bool
    if
        \ Get high and low state of reg0
        region-high-low         \ reg1 reg0high' reg0low'

        \ Get high and low state of reg1
        rot                     \ reg0high' reg0low' reg1
        region-high-low         \ reg0high' reg0low' reg1high' reg1low'

        \ Group high/low states.
        rot                     \ reg0high' reg1high' reg1low' reg0low'

        \ Calc lowest state.
        2dup                    \ reg0high' reg1high' reg1low' reg0low' reg1low' reg0low'
        state-or                \ reg0high' reg1high' reg1low' reg0low' low2'
        swap state-deallocate   \ reg0high' reg1high' reg1low' low2'
        swap state-deallocate   \ reg0high' reg1high' low2'

        \ Calc highest state.
        -rot                    \ reg-low2' reg0high' reg1high'
        2dup                    \ reg-low2' reg0high' reg1high' reg0high' reg1high'
        state-and               \ reg-low2' reg0high' reg1high' high2'
        swap state-deallocate   \ reg-low2' reg0high' high2'
        swap state-deallocate   \ reg-low2' high2'

        \ Make new region, return.
        region-new
        true
    else                        \ reg1 reg0
        2drop
        false
    then
;

\ Return true if two regions are equal.
: regions-eq? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )

    \ Check address.
    2dup =                  \ reg1 reg0 bool
    if
        2drop
        true
        exit
    then

    over region-high-state  \ reg1 reg0 reg1-h'
    over region-high-state  \ reg1 reg0 reg1-h' reg0-h'
    2dup                    \ reg1 reg0 reg1-h' reg0-h' reg1-h' reg0-h'
    states-eq?              \ reg1 reg0 reg1-h' reg0-h' bool
    swap state-deallocate   \ reg1 reg0 reg1-h' bool
    swap state-deallocate   \ reg1 reg0 bool
    if
    else
        2drop
        false
        exit
    then

    region-low-state        \ reg1 reg0-low'
    swap region-low-state   \ reg0-low' reg1-low'
    2dup                    \ reg0-low' reg1-low' reg0-low' reg1-low'
    states-eq?              \ reg0-low' reg1-low' bool
    swap state-deallocate   \ reg0-low' bool
    swap state-deallocate   \ bool
;

\ Return true if a TOS region is a superset of the NOS region.
: region-superset? ( reg1 reg-sup -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )

    2dup region-intersects?         \ reg1 reg-sup flag
    if
        \ Regions intersect.
        over region-intersection    \ reg1 reg-int flag
        0= abort" region-superset-of: reg-sup and reg1 should intersect"
                                    \ reg1 reg-int
        tuck regions-eq?            \ reg-int flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return true if a TOS region is a superset of the NOS state.
: region-superset-of-state? ( sta1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    over state-get-num-bits
    over region-get-num-bits
    <> abort" region-superset-of-state?: num bits ne?"

    \ cr ." region-superset-of-state?: " over .state space dup .region cr
    region-get-states           \ sta1 reg-sta1 reg-sta0

    \ Get sta1 dif reg-sta0
    rot                         \ reg-sta1 reg-sta0 sta1
    tuck                        \ reg-sta1 sta1 reg-sta0 sta1
    state-xor-to-mask           \ reg-sta1 sta1 dif0'
    -rot                        \ dif0' reg-sta1 sta1

    \ Get dif sta1 reg-sta1
    state-xor-to-mask           \ dif0' dif1'

    \ Get sta1 dif both region states.
    2dup                        \ dif0' dif1' dif0' dif1'
    mask-and                    \ dif0' dif1' both-dif'
    swap mask-deallocate        \ dif0' both-dif'
    swap mask-deallocate        \ both-dif'

    \ Check if dif is zero.
    dup                         \ both-dif' both-dif'
    mask-is-zero?               \ both-dif' bool
    swap mask-deallocate        \ bool
;

\ Return true if a TOS region is a subset of the NOS region.
: region-subset? ( reg1 reg-sub -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )

    2dup region-intersects?         \ reg1 reg-sub flag
    if
        \ Regions intersect.
        tuck                        \ reg-sub reg1 reg-sub
        region-intersection         \ reg-sub reg-int flag
        0= abort" region-subset?: region-subset-of: reg-sub and reg1 should intersect"
                                    \ reg-sub reg-int'
        tuck regions-eq?            \ reg-int' flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return a region with all X bit positions for a given number of bits.
: region-max-x ( nb -- reg )
    dup all-bits                        \ nb u
    over state-new                      \ nb sta1
    swap                                \ sta1 nb
    0 swap                              \ sta1 0 nb
    state-new                           \ sta1 sta0
    region-new                          \ reg-max
;

\ Return the union of a region and a state.
: region-union-state ( sta1 reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    over state-get-num-bits over region-get-num-bits <> abort" region-union-state: num bits ne?"

    dup region-high-state           \ sta1 reg0 sta-h
    swap region-low-state           \ sta1 sta-h' sta-l'
    rot                             \ sta-h' sta-l' sta1
    tuck                            \ sta-h' sta1 sta-l' sta1

    \ Get new low state.
    over state-and                  \ sta-h' sta1 sta-l' low2
    swap state-deallocate           \ sta-h' sta1 low2

    \ Get new high state.
    -rot                            \ low2 sta-h' sta1
    over state-or                   \ low2 sta-h' high2
    swap state-deallocate           \ low2 high2

    region-new
;
