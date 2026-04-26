\ Functions for state lists.

\ To deallocate a state list, use list-deallocate.

\ Check if tos is a list, if non-empty, with the first item being a state.
: assert-tos-is-state-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-state
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a state.
: assert-nos-is-state-list ( tos -- tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-state
        drop
    then
;

\ Print a state-list
: .state-list ( list0 -- )
    \ Check arg.
    assert-tos-is-state-list

    [ ' .state ] literal swap .list
;

\ Deallocate a state list.
: state-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-state-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if 
        \ Deallocate state instances in the list.
        [ ' state-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \    
    else
        struct-dec-use-count
    then 
;

\ Push a state onto a list.
: state-list-push ( sta1 list0 --  )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-struct
;

\ Push a state onto the end of a list.
: state-list-push-end ( sta1 list0 --  )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-end-struct
;

