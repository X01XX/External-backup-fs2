\ Functions for sesneed lists.

\ Check TOS for sesneed-list.
: is-sesneed-list? ( tos -- t )
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
    is-sesneed?             \ bool
;

\ Deallocate a sesneed list.
: sesneed-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-sesneed-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate sesneed instances in the list.
        [ ' sesneed-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;
