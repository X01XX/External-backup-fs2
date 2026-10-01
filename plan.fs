\ A struct for a plan to change the current state to a different state.

#37379 constant plan-struct-id
    #2 constant plan-struct-number-cells

\ Struct fields
0                       constant plan-header-disp   \ 16-bits [0] struct id [1] use count [2] Value ( 8 bits, sign bit and 0-127 ) Cost ( 8 bits, sign bit and 0-127 ).
plan-header-disp  cell+ constant plan-list-disp     \ A list of plansteps that are equal in successive to-from fields.

0 value plan-mma \ Storage for plan mma instance.

\ Init plan mma, return the addr of allocated memory.
: plan-mma-init ( num-items -- ) \ sets plan-mma.
    dup 1 <
    abort" plan-mma-init: Invalid number of items."

    cr ." Initializing Plan store."
    plan-struct-number-cells swap mma-new to plan-mma
;

\ Check if tos is an allocated plan.
: is-plan? ( tos -- flag )
    dup plan-mma mma-is-item? \ addr bool
    if
        struct-get-id
        plan-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the planstep list field from a plan instance.
: plan-get-steps ( pln0 -- plnstp-lst )
    \ Check arg.
    assert( tos is-plan? )

    plan-list-disp +    \ Add offset.
    @                   \ Fetch the field.
;

\ Set the planstep list field from a plan instance, use only in this file.
: _plan-set-steps ( plnstp-lst1 pln0 -- )
    \ Check arg.
    assert( tos is-plan? )
    assert( nos is-planstep-list? )

    plan-list-disp +    \ Add offset.
    !struct             \ Set the field.
;

\ Return the plan value.
: plan-get-value ( pln0 -- u )
    \ Check arg.
    assert( tos is-plan? )

    4c@                 \ Fetch the field.
    dup #128 and        \ val msb
    if
        #127 and -1 *
    then
;

\ Set the plan value.
: plan-set-value ( u1 pln0 -- )
    \ Check arg.
    assert( tos is-plan? )
    assert( nos #129 < )
    assert( nos #-129 > )

    over 0<
    if
        swap
        abs #128 or
        swap
    then

    4c!
;

\ Return the plan cost.
: plan-get-cost ( pln0 -- u )
    \ Check arg.
    assert( tos is-plan? )

    5c@                 \ Fetch the field.
    dup #128 and        \ val msb
    if
        #127 and -1 *
    then
;

\ Set the action domain id.
: plan-set-cost ( u1 pln0 -- )
    \ Check arg.
    assert( tos is-plan? )
    assert( nos #129 < )
    assert( nos #-129 > )

    over 0<
    if
        swap
        abs #128 or
        swap
    then

    5c!
;

\ End accessors.

\ Create a plan from a planstep list.
: plan-new ( plnstp-lst0 -- pln )
    \ Check args.
    assert( tos is-planstep-list? )

    \ Allocate instance.
    plan-struct-id plan-mma     \ plnstp-lst0 id mma
    struct-allocate             \ plnstp-lst0 pln

    \ Store fields.
    0 over plan-set-value       \ plnstp-lst0 pln
    0 over plan-set-cost        \ plnstp-lst0 pln
    tuck _plan-set-steps        \ pln

    \ cr ." plan-new: " dup hex. cr
;

\ Print a plan.
: .plan ( pln0 -- )
    \ Check arg.
    assert( tos is-plan? )

    ." ( plan Value: "
    dup plan-get-value dec.
    ." Cost: "
    dup plan-get-cost dec.
    s"   "
    rot plan-get-steps .planstep-list-prefix
    ." )"
;

\ Deallocate a plan.
: plan-deallocate ( pln0 -- )
    \ Check arg.
    assert( tos is-plan? )

    dup struct-get-use-count      \ pln0 count
    dup 0< abort" plan-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate steps.
        dup plan-get-steps planstep-list-deallocate

        \ Deallocate instance.
        plan-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Add a planstep to a plan.
: plan-add-step ( plnstp1 pln0 -- )
    \ Check args.
    assert( tos is-plan? )
    assert( nos is-planstep? )

    \ Check that the new step links to the last step.
    dup plan-get-steps          \ plnstp1 pln0 plnstp-lst
    list-is-not-empty?
    if
        dup plan-get-steps      \ plnstp1 pln0 plnstp-lst
        list-get-last-item      \ plnstp1 pln0 last-stp
        planstep-get-to         \ plnstp1 pln0 to-stac
        #2 pick                 \ plnstp1 pln0 to-stac plnstp1
        planstep-get-from       \ plnstp1 pln0 to-stac frm-stp
        statecorrs-eq?          \ plnstp1 pln0 bool
        invert abort" planstep not in sync?"
    then

    \ Add planstep.
    plan-get-steps              \ plnstp1 plnstp-lst
    list-push-end-struct
;

\ Append the steps of the nos plan onto the end of the tos plan.
: plan-append-steps ( plnstp-lst1 pln0 -- )
    \ Check args.
    assert( tos is-plan? )
    assert( nos is-planstep-list? )

    swap                \ pln0 stp-lst1

    foreach             \ pln0 stp-lnk stpx
        #2 pick         \ pln0 stp-lnk stpx pln0
        plan-add-step   \ pln0 stp-lnk
    next-item
                        \ pln0
    drop
;

\ Return the first state of a non-empty plan.
: plan-get-first-state ( pln0 -- stac )
    \ Check arg.
    assert( tos is-plan? )

    plan-get-steps          \ stp-lst
    list-get-first-item     \ stp
    planstep-get-from       \ stac
;

\ Return a statecorr path from a plan.
: plan-statecorr-path ( pln0 -- stac-lst )
    \ Check arg.
    assert( tos is-plan? )

    \ Init return list.
    list-new swap               \ ret-lst pln0

    \ Init return list.
    dup plan-get-first-state    \ ret-lst pln0 stacx
    #2 pick list-push-struct    \ ret-lst pln0

    plan-get-steps              \ ret-lst stp-lst

    foreach                     \ ret-lst stp-lnk stpx
        planstep-get-to         \ ret-lst stp-lnk stac
        #2 pick                 \ ret-lst stp-lnk stac ret-lst
        list-push-end-struct    \ ret-lst stp-lnk
    next-item
;
