\ Functions for sample lists.

\ Check if tos is an empty list, or has a sample instance as its first item.
: assert-tos-is-sample-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-sample
        drop
    then
;

\ Check if nos is an empty list, or has a sample instance as its first item.
: assert-nos-is-sample-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-sample
        drop
    then
;

\ Deallocate a sample list.
: sample-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-sample-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate square instances in the list.
        [ ' sample-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a sample-list
: .sample-list ( list0 -- )
    \ Check arg.
    assert-tos-is-sample-list

    [ ' .sample ] literal swap .list
;
