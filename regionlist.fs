\ Functions for region lists.

\ Check if tos is an empty list, or has a region instance as its first item.
: assert-tos-is-region-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if nos is an empty list, or has a region instance as its first item.
: assert-nos-is-region-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if 3os is a list, if non-empty, with the first item being a region.
: assert-3os-is-region-list ( 3os nos tos -- 3os nos tos )
    assert-3os-is-list
    #2 pick list-is-not-empty?
    if
        #2 pick list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if 4os is a list, if non-empty, with the first item being a region.
: assert-4os-is-region-list ( 4os 3os nos tos -- 4os 3os nos tos )
    assert-4os-is-list
    #3 pick list-is-not-empty?
    if
        #3 pick list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Deallocate a region list.
: region-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-region-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' region-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a region-list
: .region-list ( list0 -- )
    \ Check arg.
    assert-tos-is-region-list

    [ ' .region ] literal swap .list
;

\ Push a region to a region-list.
: region-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-struct
;

\ Push a region to the end of a region-list.
: region-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-end-struct
;

