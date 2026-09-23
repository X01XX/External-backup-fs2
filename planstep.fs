#37171 constant planstep-struct-id
    #4 constant planstep-struct-number-cells

\ Struct fields
0                               constant planstep-header-disp   \ 16-bits [0] struct id [1] use count [2] Action instance ID ( 8 bits ) Domain instance ID ( 8 bits ).
planstep-header-disp    cell+   constant planstep-from-disp     \ From state.
planstep-from-disp      cell+   constant planstep-to-disp       \ To state.
planstep-to-disp        cell+   constant planstep-alt-to-disp   \ Possible alternate result, may be zero.

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
: _planstep-set-act-inst-id ( id plnstp0 -- )
    4c!
;

: planstep-get-from ( plnstp0 -- sta )
    \ Check arg.
    assert( tos is-planstep? )

    planstep-from-disp +
    @
;

: _planstep-set-from ( sta1 plnstp0 -- )
    \ Check args.
    assert( tos is-planstep? )
    assert( nos is-state? )

    planstep-from-disp +
    !struct
;

: planstep-get-to ( plnstp0 -- sta )
    \ Check arg.
    assert( tos is-planstep? )

    planstep-to-disp +
    @
;

: _planstep-set-to ( sta1 plnstp0 -- )
    \ Check args.
    assert( tos is-planstep? )
    assert( nos is-state? )

    planstep-to-disp +
    !struct
;

: planstep-get-alt-to ( plnstp0 -- sta | 0 )
    \ Check arg.
    assert( tos is-planstep? )

    planstep-alt-to-disp +
    @
;

: _planstep-set-alt-to ( rul1 plnstp0 -- )
    \ Check args.
    assert( tos is-planstep? )
    assert( nos ?dup if is-state? else true then )

    planstep-alt-to-disp +
    over 0=
    if
        !
    else
        !struct
    then
;

\ End accessors.

: planstep-new ( alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 -- plnstp )
    \ Check args.
    assert( 3os is-state? )
    assert( 4os is-state? )
    assert( 5os ?dup if is-state? else true then )
    \ cr ." planstep-new: more arg checks todo " .stack cr

    \ Allocate instance.
    planstep-struct-id planstep-mma \ alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 id mma
    struct-allocate                 \ alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 plnstp

    \ Store fields.
    tuck _planstep-set-dom-inst-id  \ alt-sta4 to-sta3 from-sta2 act-id1 plnstp
    tuck _planstep-set-act-inst-id  \ alt-sta4 to-sta3 from-sta2 plnstp
    tuck _planstep-set-from         \ alt-sta4 to-sta3 plnstp
    tuck _planstep-set-to           \ alt-sta4 plnstp
    tuck _planstep-set-alt-to       \ plnstp
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
        dup planstep-get-alt-to
        ?dup
        if
            state-deallocate
        then

        \ Deallocate instance.
        planstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .planstep ( plnstp -- )
    \ Check arg.
    assert( tos is-planstep? )

    ." ( plnstp from: "
    dup planstep-get-from .state
    space ." to: " dup planstep-get-to .state
    planstep-get-alt-to
    ?dup
    if
        space ." alt-to: " .state
    then
    ." )"
;
