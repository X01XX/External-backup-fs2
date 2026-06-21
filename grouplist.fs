\ Functions for group lists.                                                                                                                      

\ Check if tos is an empty list, or has a group instance as its first item.
: assert-tos-is-group-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if  
        dup list-get-links link-get-data
        assert-tos-is-group
        drop
    then
;

\ Check if nos is an empty list, or has a group instance as its first item.
: assert-nos-is-group-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if  
        over list-get-links link-get-data
        assert-tos-is-group
        drop
    then
;

\ Deallocate a group list.
: group-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-group-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate group instances in the list.
        [ ' group-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;
