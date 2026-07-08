\ Functions for action lists.

\ Check TOS for action-list.
: is-action-list? ( tos -- t )
    assert( tos is-list? )
    
    dup list-is-empty?
    if
        drop
        true
    else
        list-get-links link-get-data
        assert( is-action? )
        true
    then
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

: action-list-push-end ( actx act-lst -- )
    \ Check args.
    assert( tos is-action-list? )
    assert( nos is-action? )

    list-push-end-struct
;

\ Find a action in a list, by instance id, if any.
: action-list-find ( id1 list0 -- dom t | f )
    \ Check arg.
    assert( tos is-action-list? )

    [ ' action-id-eq ] literal -rot list-find
;

