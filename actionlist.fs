\ Functions for action lists.

\ Check TOS for action-list.
: is-action-list? ( tos -- bool )
    dup is-list?        \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?  \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links      \ link
    link-get-data       \ data
    is-action?          \ bool
;

: .action-list ( actlst0 -- )
    \ Check args.
    assert( tos is-list? )

    [ ' .action ] literal swap list-apply
;

\ Deallocate an action list.
: action-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-action-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate action instances in the list.
        [ ' action-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: action-id-eq? ( id1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )

    action-get-inst-id  \ id1 id0
    =
;

\ Find a action in a list, by instance id, if any.
: action-list-find ( id1 list0 -- dom t | f )
    \ Check arg.
    assert( tos is-action-list? )

    [ ' action-id-eq? ] literal -rot list-find
;

\ Push a action, set the action instance id to its position in the list.
\ So only push to the end of the list.
: action-list-push-end ( domx dom-lst -- )
    \ Check args.
    assert( tos is-action-list? )
    assert( nos is-action? )

    \ Set action instance, list position, id.
    dup list-get-length         \ domx dom-lst len
    #2 pick                     \ domx dom-lst len domx
    action-set-inst-id          \ domx dom-lst

    list-push-end-struct
;
