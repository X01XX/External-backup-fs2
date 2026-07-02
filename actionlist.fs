\ Functions for action lists.

\ Check if tos is an empty list, or has an action instance as its first item.
: assert-tos-is-action-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-action
        drop
    then
;

\ Check if nos is an empty list, or has an action instance as its first item.
: assert-nos-is-action-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-action
        drop
    then
;

: .action-list ( actlst0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .action ] literal swap list-apply
;

\ Deallocate an action list.
: action-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-action-list

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
    assert-tos-is-action-list
    assert-nos-is-action

    list-push-end-struct
;

\ Find a action in a list, by instance id, if any.
: action-list-find ( id1 list0 -- dom t | f )
    \ Check arg.
    assert-tos-is-action-list

    [ ' action-id-eq ] literal -rot list-find
;

