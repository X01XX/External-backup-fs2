\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-struct-id
    #4 constant corner-struct-number-cells

\ Struct fields
0                                   constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count, [2] Rate ( 8 bits ).
                                                                            \ Rate will be the number of adjacent states that are only in one region.
corner-header-disp          cell+   constant corner-anchor-state-disp       \ The anchor square state.
corner-anchor-state-disp    cell+   constant corner-region-disp             \ Region the anchor is in, according to ~A + ~B calculation.
corner-region-disp          cell+   constant corner-adjacent-states-disp    \ All adjacent to anchor, external, states.


\ Needs: Meta, resolve needs for cornerns by some criteria. Dissimilar squares only in one region, ...
\        pnc anchor.
\        pnc dissimilar squares.


0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check if tos is an allocated corner.
: is-corner? ( addr -- bool )
    dup corner-mma mma-is-item? \ addr bool
    if
        struct-get-id
        corner-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the anchor-state field from a corner instance.
: corner-get-anchor-state ( crn0 -- sqr )
    \ Check arg.
    assert( tos is-corner? )

    corner-anchor-state-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the anchor-state field from a corner instance, use only in this file.
: _corner-set-anchor-state ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state? )

    corner-anchor-state-disp +      \ Add offset.
    !struct                         \ Set the field.
;

\ Return the adjacent-states list field from a corner instance.
: corner-get-adjacent-states ( crn0 -- sta-lst )
    \ Check arg.
    assert( tos is-corner? )

    corner-adjacent-states-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the adjacent-states list field from a corner instance, use only in this file.
: _corner-set-adjacent-states ( sta-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state-list? )

    corner-adjacent-states-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the region field from a corner instance.
: corner-get-region ( crn0 -- reg )
    \ Check arg.
    assert( tos is-corner? )

    corner-region-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the region field from a corner instance, use only in this file.
: _corner-set-region ( reg1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region? )

    corner-region-disp +       \ Add offset.
    !struct                             \ Set the field.
;

\ Get the rate value.
: corner-get-rate ( crn0 -- rt )
    \ Check arg.
    assert( tos is-corner? )

    4c@
;

\ Set the rate value.
: corner-set-rate ( rt crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos 0 >= )
    assert( nos [ 1 cells #8 * ] literal < )

    4c!
;

\ End accessors.

\ Return a corner's number bits.
: corner-get-num-bits ( crn0 -- nb )
    \ Check args.
    assert( tos is-corner? )

    corner-get-anchor-state        \ sta
    state-get-num-bits
;

\ Create a corner, given a regios and an anchor state.
: corner-new ( sta1 reg0 -- crn )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    assert( over state-get-num-bits over region-get-num-bits = )

    2dup region-superset-of-state?      \ sta1 reg0 bool
    ifnot cr ." corner-new: region not superset state?" abort then

    \ Init adjacent state list.
    list-new                            \ sta1 reg0 sta-lst

    \ Fill adjacent states list.
    over region-edge-mask               \ sta1 reg0 sta-lst edg-msk
    dup mask-split                      \ sta1 reg0 sta-lst edg-msk' msk-lst'
    swap mask-deallocate                \ sta1 reg0 sta-lst msk-lst'
    dup                                 \ sta1 reg0 sta-lst msk-lst' msk-lst'
    foreach                             \ sta1 reg0 sta-lst msk-lst' msk-lnk
        \ Calc one anchor-adjacent, external to region, state.
        dup link-get-data               \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx
        #5 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx sta1
        state-xor-mask                  \ sta1 reg0 sta-lst msk-lst' msk-lnk sta'

        \ Store the state.
        #3 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk sta' sta-lst
        list-push-struct                \ sta1 reg0 sta-lst msk-lst' msk-lnk
    next
    mask-list-deallocate                \ sta1 reg0 sta-lst

    \ Allocate space.
    corner-struct-id corner-mma         \ sta1 reg0 sta-lst id mma
    struct-allocate                     \ sta1 reg0 sta-lst crn

    \ Store adjacent states list.
    tuck _corner-set-adjacent-states

    \ Store region.
    tuck _corner-set-region             \ sta1 crn

    \ Store anchor state.
    tuck _corner-set-anchor-state       \ crn

    \ Set rate.
    0 over corner-set-rate              \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    ." ("
    dup corner-get-region               \ crn0 reg
    .region                             \ crn0

    space ." anchor: "
    dup corner-get-anchor-state         \ crn0 sta
    .state                              \ crn0

    space ." AE: "
    dup corner-get-adjacent-states      \ crn0 ext-sta-lst
    .state-list                         \ crn0

    ." )"
                                        \ crn0
    drop
;

\ Validate a corner.
\ Recalc if needed.
: corner-is-valid? ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    abort" TODO"
;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup corner-get-anchor-state state-deallocate
        dup corner-get-adjacent-states state-list-deallocate
        dup corner-get-region region-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check the anchor, after a change.
: corner-check-anchor ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    true abort" TODO"
;

\ Return true if a corner anchor is equal to a given state.
: corner-anchor-eq-state? ( sta1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state? )

    corner-get-anchor-state \ sta1 crn-sta
    states-eq?
;
