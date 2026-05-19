\ Functions for mask lists.

\ Check if tos is an empty list, or has a mask instance as its first item.
: assert-tos-is-mask-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-mask
        drop
    then
;

\ Check if nos is an empty list, or has a mask instance as its first item.
: assert-nos-is-mask-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-mask
        drop
    then
;

\ Check if 3os is a list, if non-empty, with the first item being a mask.
: assert-3os-is-mask-list ( 3os nos tos -- 3os nos tos )
    assert-3os-is-list
    #2 pick list-is-not-empty?
    if
        #2 pick list-get-links link-get-data
        assert-tos-is-mask
        drop
    then
;

\ Check if 4os is a list, if non-empty, with the first item being a mask.
: assert-4os-is-mask-list ( 4os 3os nos tos -- 4os 3os nos tos )
    assert-4os-is-list
    #3 pick list-is-not-empty?
    if
        #3 pick list-get-links link-get-data
        assert-tos-is-mask
        drop
    then
;

\ Deallocate a mask list.
: mask-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-mask-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate mask instances in the list.
        [ ' mask-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a mask-list
: .mask-list ( list0 -- )
    \ Check arg.
    assert-tos-is-mask-list

    [ ' .mask ] literal swap .list
;

\ Push a mask to a mask-list.
: mask-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-mask-list
    assert-nos-is-mask

    list-push-struct
;

\ Push a mask to the end of a mask-list.
: mask-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-mask-list
    assert-nos-is-mask

    list-push-end-struct
;

\ Return a mask-list from a string.
: mask-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        dup list-get-links      \ lst lnk
        begin
            ?dup
        while
            dup link-get-data   \ lst lnk dat
            is-allocated-mask \ lst lnk bool
            if
            else
                drop            \ lst
                structinfo-list-deallocate-struct-list-xt execute
                false
                exit
            then

            link-get-next       \ lst lnk
        repeat
                                \ lst
        true
    else
        false
    then
;
