\ Functions for domain lists.

\ Check TOS for domain-list.
: is-domain-list? ( tos -- bool )
   dup is-list?        \ tos bool                                                     
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?  \ tos bool
    if  
        drop
        true
        exit
    then

    list-get-links      \ link
    link-get-data       \ data
    is-domain?          \ bool
;

\ Deallocate a domain list.
: domain-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-domain-list? )

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
    assert( tos is-domain-list? )
    assert( nos is-domain? )

    list-push-end-struct        \
;

\ Find a domain in a list, by instance id, if any.
: domain-list-find ( id1 list0 -- dom t | f )
    \ Check args.
    assert( tos is-domain-list? )

    [ ' domain-id-eq ] literal -rot list-find
;


