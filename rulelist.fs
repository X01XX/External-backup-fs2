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

    [ ' rules-eq? ] literal -rot    \ xt reg-lst1 reg-lst0
    struct-lists-eq?                \ bool
;

\ Return true if a region can form a union with at
\ least one region in a region list pair.
: rule-list-union-superset? ( rul1 rul-lst0 -- bool )
    \ Check args.
    assert-tos-is-rule-list
    assert-nos-is-rule
    dup list-get-length #2 < abort" rule list too short?"

    \ Check union with first list rule.
    dup list-get-first-item     \ rul1 rul-lst0 rul0a
    #2 pick                     \ rul1 rul-lst0 rul0a rul1
    rule-union                  \ rul1 rul-lst0, rul t | f
    if
        rule-deallocate
        2drop
        true
        exit
    then

    \ Check union with second list rule.
    list-get-second-item        \ rul1 rul0b
    rule-union                  \ rul t | f
    if
        rule-deallocate
        true
    else
        false
    then
;

\ Return rule-list pair union.
\ Try two different orders of union, return true if one order works, and the other does not.
\ Kind of like XOR.
\ With two successful unions, at least one bit will be unpredictable,
\ the four possible values of one bit position will be 0->0, 0->1, 1->1, 1->0,
\ leading to X->1/X->0 in one union, X->X/X->x in the other union.
: rule-list-union ( rul-lst1 rul-lst0 -- rul-lst t | f )
    \ Check args.
    assert-tos-is-rule-list
    assert-nos-is-rule-list
    dup  list-get-length #2 <> abort" rule list not pair?"
    over list-get-length #2 <> abort" rule list not pair?"

    \ Check order one.
    list-new -rot                   \ ret-lst1 rul-lst1 rul-lst0
    over list-get-first-item        \ ret-lst1 rul-lst1 rul-lst0 rul1a
    over list-get-first-item        \ ret-lst1 rul-lst1 rul-lst0 rul1a rul0a
    rule-union                      \ ret-lst1 rul-lst1 rul-lst0, rul-u t | f
    if
        #3 pick                     \ ret-lst1 rul-lst1 rul-lst0 rul-u ret-lst1
        list-push-struct            \ ret-lst1 rul-lst1 rul-lst0
        \ Check rul0b union rul1b.
        over list-get-second-item   \ ret-lst1 rul-lst1 rul-lst0 rul1b
        over list-get-second-item   \ ret-lst1 rul-lst1 rul-lst0 rul1b rul0b
        rule-union                  \ ret-lst1 rul-lst1 rul-lst0, rul-u t | f
        if
            #3 pick                 \ ret-lst1 rul-lst1 rul-lst0 rul-u ret-lst1
            list-push-struct        \ ret-lst1 rul-lst1 rul-lst0
        \ else leave ret-lst1 with one rule.
        then
    \ else leave ret-lst1 empty.
    then

    \ Check order two.
    list-new -rot                   \ ret-lst1 ret-lst2 rul-lst1 rul-lst0
    over list-get-first-item        \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul1a
    over list-get-second-item       \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul1a rul0b
    rule-union                      \ ret-lst1 ret-lst2 rul-lst1 rul-lst0, rul-u t | f
    if
        #3 pick                     \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul-u ret-lst2
        list-push-struct            \ ret-lst1 ret-lst2 rul-lst1 rul-lst0
        \ Check rul0b union rul1b.
        over list-get-second-item   \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul1b
        over list-get-first-item    \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul1b rul0a
        rule-union                  \ ret-lst1 ret-lst2 rul-lst1 rul-lst0, rul-u t | f
        if
            #3 pick                 \ ret-lst1 ret-lst2 rul-lst1 rul-lst0 rul-u ret-lst2
            list-push-struct        \ ret-lst1 ret-lst2 rul-lst1 rul-lst0
        \ else leave ret-lst2 with one rule.
        then
    \ else leave ret-lst2 empty.
    then

    \ Check the four posibilities of return list length (successful union) EQ or NE 2.
    2drop                               \ ret-lst1 ret-lst2
    dup list-get-length                 \ ret-lst1 ret-lst2 len2
    #2 =
    if
        over list-get-length            \ ret-lst1 ret-lst2 len1
        #2 =
        if
            \ len2 == 2, len1 == 2, invalid union.
            rule-list-deallocate
            rule-list-deallocate
            false
        else
            \ len2 == 2, len1 != 2, valid union.
            swap rule-list-deallocate   \ ret-lst2
            true
        then
    else
        over list-get-length            \ ret-lst1 ret-lst2 len1
        #2 =
        if
            \ len2 != 2, len1 == 2, valid union.
            rule-list-deallocate        \ ret-lst1
            true
        else
            \ len2 != 2, len1 != 2, invalid union.
            rule-list-deallocate
            rule-list-deallocate
            false
        then
    then
;
