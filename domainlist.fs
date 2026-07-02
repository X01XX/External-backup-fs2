\ Functions for domain lists.

\ Check if tos is an empty list, or has a domain instance as its first item.
: assert-tos-is-domain-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-domain
        drop
    then
;

\ Deallocate a domain list.
: domain-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-domain-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate domain instances in the list.
        [ ' domain-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: domain-list-push-end ( domx dom-lst -- )
    \ Check args.
    assert-tos-is-domain-list
    assert-nos-is-domain

    list-push-end-struct        \
;

\ Find a domain in a list, by instance id, if any.
: domain-list-find ( id1 list0 -- dom t | f )
    \ Check args.
    assert-tos-is-domain-list

    [ ' domain-id-eq ] literal -rot list-find
;


