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

