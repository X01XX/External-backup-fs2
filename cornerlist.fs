\ Functions for corner lists.

\ Check TOS for corner-list.
: is-corner-list? ( tos -- bool )
    tos is-list?        \ tos bool
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
    is-corner?          \ bool
;

\ Deallocate a corner list.
: corner-list-deallocate ( crn-lst0 -- )
    \ Check arg.
    assert( tos is-corner-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate corner instances in the list.
        [ ' corner-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a corner list.
: .corner-list ( crn-lst0 -- )
    \ Check arg.
    assert( tos is-corner-list? )

    foreach                 \ grp-lnk
        dup link-get-data   \ grp-lnk grpx
        cr #8 spaces .corner
    next
;

: .corner-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-corner-list? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk
        dup link-get-data .corner

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;
