
\ Check TOS for statecorr-list.
: is-statecorr-list? ( tos -- bool )
    dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-statecorr?           \ bool
;

\ Deallocate a statecorr list.
: statecorr-list-deallocate ( stac-lst0 -- )
    \ Check arg.
    assert( tos is-statecorr-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ stac-lst0 uc
    #2 < if
        \ Deallocate statecorr instances in the list.
        [ ' statecorr-deallocate ] literal over     \ stac-lst0 xt stac-lst0
        list-apply                                  \ stac-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a statecorr-list
: .statecorr-list ( stac-lst0 -- )
    \ Check arg.
    assert( tos is-statecorr-list? )

    [ ' .statecorr ] literal swap .list
;

\ Print a statecorr list one line at a time, aligned with a given prefix.
: .statecorr-list-prefix ( c-addr u stac-lst0 -- )
    \ Check arg.
    assert( tos is-statecorr-list? )
    cr
    rot                 \ u stac-lst0 c-addr
    #2 pick             \ u stac-lst0 c-addr u
    type                \ u stac-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk grpx
        .statecorr

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

