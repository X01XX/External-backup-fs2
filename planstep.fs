#37171 constant planstep-struct-id
    #3 constant planstep-struct-number-cells

\ Struct fields
0                               constant planstep-header-disp   \ 16-bits [0] struct id [1] use count [2] Action instance ID ( 8 bits ) Domain instance ID ( 8 bits ).
planstep-header-disp    cell+   constant planstep-from-disp     \ From state.
planstep-from-disp      cell+   constant planstep-to-disp       \ To state.
planstep-to-disp        cell+   constant planstep-rule-disp     \ Rule used.

0 value planstep-mma \ Storage for planstep mma instance.

\ Init planstep mma, return the addr of allocated memory.
: planstep-mma-init ( num-items -- ) \ sets planstep-mma.
    dup 1 <
    abort" planstep-mma-init: Invalid number of items."

    cr ." Initializing PlanStep store."
    planstep-struct-number-cells swap mma-new to planstep-mma
;

\ Check if tos is an allocated planstep.
: is-planstep? ( tos -- flag )
    dup planstep-mma mma-is-item? \ addr bool
    if
        struct-get-id
        planstep-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the planstep domain id.
: planstep-get-dom-inst-id ( plnstp0 -- id )
    \ Check arg.
    assert( tos is-planstep? )

    5c@                 \ Fetch the field.
;

\ Set the planstep domain id.
: _planstep-set-dom-inst-id ( id1 plnstp0 -- )
    5c!
;

\ Get the planstep id.
: planstep-get-act-inst-id ( plnstp0 -- id )
    \ Check arg.
    assert( tos is-planstep? )

    4c@
;

\ Set the planstep id.
: planstep-set-act-inst-id ( id plnstp0 -- )
    4c!
;

: planstep-get-from ( plnstp0 -- sta )
    \ Check arg.
    assert( is-planstep? )

    planstep-from-disp +
    @
;

: _planstep-set-from ( sta1 plnstp0 -- )
    \ Check args.
    assert( is-planstep? )
    assert( is-state? )

    planstep-from-disp +
    !struct
;

: planstep-get-to ( plnstp0 -- sta )
    \ Check arg.
    assert( is-planstep? )

    planstep-to-disp +
    @
;

: _planstep-set-to ( sta1 plnstp0 -- )
    \ Check args.
    assert( is-planstep? )
    assert( is-state? )

    planstep-to-disp +
    !struct
;

: planstep-get-rule ( plnstp0 -- rul )
    \ Check arg.
    assert( is-planstep? )

    planstep-rule-disp +
    @
;

: _planstep-set-rule ( rul1 plnstp0 -- )
    \ Check args.
    assert( is-planstep? )
    assert( is-rule? )

    planstep-rule-disp +
    !struct
;

\ End accessors.

: planstep-new ( rul2 to-sta1 from-sta0 -- plnstp )
    \ Check args.
    assert( tos is-state? )
    assert( nos is-state? )
    assert( 3os is-rule? )
    cr ." planstep-new: more arg checks todo" cr

    \ Allocate instance.
    planstep-struct-id planstep-mma \ rul2 to-sta1 from-sta0 id mma
    struct-allocate                 \ rul2 to-sta1 from-sta0 plnstp

    \ Store fields.
    tuck _planstep-set-from         \ rul2 to-sta1 plnstp
    tuck _planstep-set-to           \ rul2 plnstp
    tuck _planstep-set-rule         \ plnstp
;

\ Deallocate a planstep.
: planstep-deallocate ( plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )

    dup struct-get-use-count      \ plnstp0 count
    dup 0< abort" planstep-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate states.
        dup planstep-get-from state-deallocate
        dup planstep-get-to state-deallocate
        dup planstep-get-rule rule-deallocate

        \ Deallocate instance.
        planstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .planstep ( plnstp -- )
    \ Check arg.
    assert( is-planstep? )

    ." ( plnstp from: "
    dup planstep-get-from .state
    space ." to: " dup planstep-get-to .state
    space ." rule: " planstep-get-rule .rule
    ." )"
;
