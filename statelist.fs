\ Functions for state lists.

\ Check if tos is an empty list, or has a state instance as its first item.
: assert-tos-is-state-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-state
        drop
    then
;

\ Check if nos is an empty list, or has a state instance as its first item.
: assert-nos-is-state-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-state
        drop
    then
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

\ Print a state-list
: .state-list ( list0 -- )
    \ Check arg.
    assert-tos-is-state-list

    [ ' .state ] literal swap .list
;

\ Push a state to a state-list.
: state-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-struct
;

\ Push a state to the end of a state-list.
: state-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-end-struct
;

\ Return a state-list from a string.
: state-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-allocated-state ] literal over   \ lst xt lst
        list-apply-all-true?                    \ lst bool
        if
            true
        else
            structinfo-list-deallocate-struct-list-xt execute
            false
        then
    else
        false
    then
;
