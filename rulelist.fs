\ Functions for rule lists.

\ Check if tos is an empty list, or has a rule instance as its first item.
: assert-tos-is-rule-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-rule
        drop
    then
;

\ Check if nos is an empty list, or has a rule instance as its first item.
: assert-nos-is-rule-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-rule
        drop
    then
;

\ Deallocate a rule list.
: rule-list-deallocate ( lst0 -- )
    \ Check args.
    assert-tos-is-rule-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rule instances in the list.
        [ ' rule-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Deallocate a rule list.
: rule-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-rule-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate square instances in the list.
        [ ' rule-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a rule-list
: .rule-list ( list0 -- )
    \ Check arg.
    assert-tos-is-rule-list

    [ ' .rule ] literal swap .list
;
