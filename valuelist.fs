\ Functions for value lists.

\ To deallocate a value list, use list-deallocate.

\ Check if tos is a list, if non-empty, with the first item being a value.
: assert-tos-is-value-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-value
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a value.
: assert-nos-is-value-list ( tos -- tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-value
        drop
    then
;

\ Print a value-list
: .value-list ( list0 -- )
    \ Check arg.
    assert-tos-is-value-list

    [ ' .value ] literal swap .list
;

\ Deallocate a value list.
: value-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-value-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if 
        \ Deallocate value instances in the list.
        [ ' value-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0
    then 

    \ Deallocate the list.
    list-deallocate                                 \    
;

\ Push a value onto a list.
: value-list-push ( val1 list0 --  )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value

    list-push-struct
;

\ Push a value onto the end of a list.
: value-list-push-end ( val1 list0 --  )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value

    list-push-end-struct
;

