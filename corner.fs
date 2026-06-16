\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-id
    #5 constant corner-struct-number-cells

\ Struct fields
0                                       constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp              cell+   constant corner-anchor-state-disp       \ The anchor state.
corner-anchor-state-disp        cell+   constant corner-dissimilar-states-disp  \ Dissimilar, closest, state list.
corner-dissimilar-states-disp   cell+   constant corner-similar-states-disp     \ Similar, closest, state list.
corner-similar-states-disp      cell+   constant corner-regions-disp            \ Regions the anchor is in.

0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check instance type.
: is-allocated-corner? ( addr -- bool )
    dup corner-mma mma-is-item  \ addr bool
    if
        struct-get-id
        corner-id =             \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for corner, unconventional, leaves stack unchanged.
: assert-tos-is-corner ( tos -- tos )
    dup is-allocated-corner?
    false? if
        s" TOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Check NOS for corner, unconventional, leaves stack unchanged.
: assert-nos-is-corner ( nos tos -- nos tos )
    over is-allocated-corner?
    false? if
        s" NOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the anchor-state field from a corner instance.
: corner-get-anchor-state ( crn0 -- sta )
    \ Check arg.
    assert-tos-is-corner

    corner-anchor-state-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Return the dissimilar-states list field from a corner instance.
: corner-get-dissimilar-states ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-dissimilar-states-disp + \ Add offset.
    @                               \ Fetch the field.
;

\ Return the similar-states list field from a corner instance.
: corner-get-similar-states ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-similar-states-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ Return the regions list field from a corner instance.
: corner-get-regions ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-regions-disp +           \ Add offset.
    @                               \ Fetch the field.
;

\ Set the anchor-state field from a corner instance, use only in this file.
: _corner-set-anchor-state ( sta1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-state

    corner-anchor-state-disp +  \ Add offset.
    !struct                     \ Set the field.
;

\ Set the dissimilar-states list field from a corner instance, use only in this file.
: _corner-set-dissimilar-states ( sta-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-state-list

    corner-dissimilar-states-disp +     \ Add offset.
    !struct                             \ Set the field.
;

\ Set the similar-states list field from a corner instance, use only in this file.
: _corner-set-similar-states ( sta-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-state-list

    corner-similar-states-disp +        \ Add offset.
    !struct                             \ Set the field.
;

\ Set the regions list field from a corner instance, use only in this file.
: _corner-set-regions ( reg-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-region-list

    corner-regions-disp +               \ Add offset.
    !struct                             \ Set the field.
;

\ End accessors.

\ Return a corner's number bits.
: corner-get-number-bits ( crn0 -- nb )
    \ Check args.
    assert-tos-is-corner

    corner-get-anchor-state \ sta
    state-get-num-bits
;

\ Create a corner, given an anchor state.
: corner-new ( sta0 -- crn )
    \ Check args.
    assert-tos-is-state

    \ Allocate space.
    corner-id corner-mma                \ sta1 id mma
    struct-allocate                     \ sta1 crn

    \ Store anchor state.
    tuck _corner-set-anchor-state       \ crn

    \ Store dissimilar state list.
    list-new                            \ crn lst
    over _corner-set-dissimilar-states  \ crn

    \ Store similar state list.
    list-new                            \ crn lst
    over _corner-set-similar-states     \ crn

    \ Store regions list.
    dup corner-get-number-bits          \ crn nb
    region-list-max-x                   \ crn reg-lst
    over _corner-set-regions            \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    ." ("
    dup corner-get-anchor-state     \ crn0 sta
    .state                          \ crn0

    ."  dissimilar states:  "
    dup corner-get-dissimilar-states    \ crn0 ext-sta-lst
    .state-list                         \ crn0

    ."  similar states:  "
    dup corner-get-dissimilar-states    \ crn0 ext-sta-lst
    .state-list                         \ crn0

    ." )"
;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup corner-get-anchor-state state-deallocate
        dup corner-get-similar-states state-list-deallocate
        dup corner-get-dissimilar-states state-list-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

