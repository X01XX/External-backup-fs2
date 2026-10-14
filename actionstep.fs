#61379 constant actionstep-struct-id
    #4 constant actionstep-struct-number-cells

\ Struct fields
0                                 constant actionstep-header-disp   \ 16-bits [0] struct id [1] use count [2] Action instance ID ( 8 bits ) Domain instance ID ( 8 bits ).
actionstep-header-disp    cell+   constant actionstep-from-disp     \ From state.
actionstep-from-disp      cell+   constant actionstep-to-disp       \ To state.
actionstep-to-disp        cell+   constant actionstep-alt-to-disp   \ Possible alternate result, may be zero.

0 value actionstep-mma \ Storage for actionstep mma instance.

\ Init actionstep mma, return the addr of allocated memory.
: actionstep-mma-init ( num-items -- ) \ sets actionstep-mma.
    dup 1 <
    abort" actionstep-mma-init: Invalid number of items."

    cr ." Initializing ActionStep store."
    actionstep-struct-number-cells swap mma-new to actionstep-mma
;

\ Check if tos is an allocated actionstep.
: is-actionstep? ( tos -- flag )
    dup actionstep-mma mma-is-item? \ addr bool
    if
        struct-get-id
        actionstep-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the actionstep domain id.
: actionstep-get-dom-inst-id ( actstp0 -- id )
    \ Check arg.
    assert( tos is-actionstep? )

    5c@                 \ Fetch the field.
;

\ Set the actionstep domain id.
: _actionstep-set-dom-inst-id ( id1 actstp0 -- )
    5c!
;

\ Get the actionstep id.
: actionstep-get-act-inst-id ( actstp0 -- id )
    \ Check arg.
    assert( tos is-actionstep? )

    4c@
;

\ Set the actionstep id.
: _actionstep-set-act-inst-id ( id actstp0 -- )
    4c!
;

: actionstep-get-from ( actstp0 -- sta )
    \ Check arg.
    assert( tos is-actionstep? )

    actionstep-from-disp +
    @
;

: _actionstep-set-from ( sta1 actstp0 -- )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos is-state? )

    actionstep-from-disp +
    !struct
;

: actionstep-get-to ( actstp0 -- sta )
    \ Check arg.
    assert( tos is-actionstep? )

    actionstep-to-disp +
    @
;

: _actionstep-set-to ( sta1 actstp0 -- )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos is-state? )

    actionstep-to-disp +
    !struct
;

: actionstep-get-alt-to ( actstp0 -- sta | 0 )
    \ Check arg.
    assert( tos is-actionstep? )

    actionstep-alt-to-disp +
    @
;

: _actionstep-set-alt-to ( rul1 actstp0 -- )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos 0= if true else nos is-state? then )

    actionstep-alt-to-disp +
    over 0=
    if
        !
    else
        !struct
    then
;

\ End accessors.

: actionstep-new ( alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 -- actstp )
    \ Check args.
    assert( 3os is-state? )
    assert( 4os is-state? )
    assert( 5os 0= if true else 5os is-state? then )
    \ cr ." actionstep-new: more arg checks todo " .stack cr

    \ Allocate instance.
    actionstep-struct-id actionstep-mma \ alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 id mma
    struct-allocate                 \ alt-sta4 to-sta3 from-sta2 act-id1 dom-id0 actstp

    \ Store fields.
    tuck _actionstep-set-dom-inst-id  \ alt-sta4 to-sta3 from-sta2 act-id1 actstp
    tuck _actionstep-set-act-inst-id  \ alt-sta4 to-sta3 from-sta2 actstp
    tuck _actionstep-set-from         \ alt-sta4 to-sta3 actstp
    tuck _actionstep-set-to           \ alt-sta4 actstp
    tuck _actionstep-set-alt-to       \ actstp
;

\ Deallocate a actionstep.
: actionstep-deallocate ( actstp0 -- )
    \ Check arg.
    assert( tos is-actionstep? )

    dup struct-get-use-count      \ actstp0 count
    dup 0< abort" actionstep-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate states.
        dup actionstep-get-from state-deallocate
        dup actionstep-get-to state-deallocate
        dup actionstep-get-alt-to
        ?dup
        if
            state-deallocate
        then

        \ Deallocate instance.
        actionstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .actionstep ( actstp -- )
    \ Check arg.
    assert( tos is-actionstep? )

    ." ( actstp Dom: "
    dup actionstep-get-dom-inst-id dec.
    space ." Act: "
    dup actionstep-get-act-inst-id dec.
    space ." from: "
    dup actionstep-get-from .state
    space ." to: " dup actionstep-get-to .state
    actionstep-get-alt-to
    ?dup
    if
        space ." alt-to: " .state
    then
    ." )"
;

\ Return a copy of a statecorr, with the actionstep to-state inserted.
: actionstep-insert-to-state ( stac1 actstp0 -- stac )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos is-statecorr?-xt execute )

    \ Get domain intance id, as index, and to-state.
    dup actionstep-get-to           \ stac1 actstp0 sta-to
    -rot                            \ sta-to stac1 actstp0
    actionstep-get-dom-inst-id      \ sta-to stac1 dom-id
    swap                            \ sta-to dom-id stac1

    statecorr-insert-state          \ stac
;
