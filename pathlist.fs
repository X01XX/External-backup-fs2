
\ Check TOS for path-list.
: is-path-list? ( tos -- bool )
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
    is-path?                \ bool
;

\ Deallocate a path list.
: path-list-deallocate ( pth-lst0 -- )
    \ Check arg.
    assert( tos is-path-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ pth-lst0 uc
    #2 < if
        \ Deallocate path instances in the list.
        [ ' path-deallocate ] literal over     \ pth-lst0 xt pth-lst0
        list-apply                                  \ pth-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;


