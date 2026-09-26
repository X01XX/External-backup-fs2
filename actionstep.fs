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
    assert( nos ?dup if is-state? else true then )

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
    assert( 5os ?dup if is-state? else true then )
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
: actionstep-insert-to-state ( stac1 actstp -- stac )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos is-statecorr?-xt execute )

    \ Init return list.
    list-new -rot                   \ sta-lst stac1 actstp

    \ Prep for loop.

    \ Get domain intance id, as index, and to-state.
    dup actionstep-get-dom-inst-id    \ sta-lst stac1 actstp dom-id
    swap actionstep-get-to            \ sta-lst stac1 dom-id to-sta
    rot                             \ sta-lst dom-id to-sta stac1
    statecorr-get-list              \ sta-lst dom-id to-sta stac-lst
    dup list-get-length             \ sta-lst dom-id to-sta stac-lst len
    swap list-get-links swap        \ sta-lst dom-id to-sta stac-lst len
    0                               \ sta-lst dom-id to-sta stac-lnk len 0

    do                              \ sta-lst dom-id to-sta stac-lnk
        i                           \ sta-lst dom-id to-sta stac-lnk i
        #3 pick                     \ sta-lst dom-id to-sta stac-lnk i dom-id
        =                           \ sta-lst dom-id to-sta stac-lnk bool

        if
            \ Push actionstep to state.
            over                    \ sta-lst dom-id to-sta stac-lnk to-sta
            #4 pick                 \ sta-lst dom-id to-sta stac-lnk to-sta sta-lst
            list-push-end-struct    \ sta-lst dom-id to-sta stac-lnk
        else
            \ Push stac1 state.
            dup link-get-data       \ sta-lst dom-id to-sta stac-lnk stax
            #4 pick                 \ sta-lst dom-id to-sta stac-lnk stax sta-lst
            list-push-end-struct    \ sta-lst dom-id to-sta stac-lnk
        then

        link-get-next
    loop
                                    \ sta-lst dom-id to-sta stac-lnk
    \ Clean up.
    2drop drop                      \ sta-lst

    \ Return.
    statecorr-new
;
