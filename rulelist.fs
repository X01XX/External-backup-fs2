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

\ Return a rule-list from a string.
: rule-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-allocated-rule ] literal over    \ lst xt lst
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

\ Return a rule-list from a string.
: rule-list-from-string-a ( c-addr u -- reg-lst )
    rule-list-from-string  \ lst t | f
    invert abort" Invalid rule-list?"
;

\ Return true if two rule lists are equal.
: rule-lists-eq? ( reg-lst1 reg-lst0 -- bool )
    \ Check args.
    assert-tos-is-rule-list
    assert-nos-is-rule-list

    \ Check lengths.
    over list-get-length            \ reg-lst1 reg-lst0 len1
    over list-get-length            \ reg-lst1 reg-lst0 len1 len0
    <>                              \ reg-lst1 reg-lst0 bool
    if
        2drop
        false
        exit
    then

    \ Check elements.
    list-get-links                  \ reg-lst1 lnk
    begin
        ?dup
    while
        [ ' rules-eq? ] literal     \ reg-lst1 lnk xt
        over link-get-data          \ reg-lst1 lnk xt regx
        #3 pick                     \ reg-lst1 lnk xt regx reg-lst1
        list-member                 \ reg-lst1 lnk bool
        if
        else
            2drop
            false
            exit
        then

        link-get-next
    repeat
    drop
    true
;
