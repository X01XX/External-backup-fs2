
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

\ Print a path-list
: .path-list ( pth-lst0 -- )
    \ Check arg.
    assert( tos is-path-list? )

    [ ' .path ] literal swap .list
;

\ Print a path list one line at a time, aligned with a given prefix.
: .path-list-prefix ( c-addr u pth-lst0 -- )
    \ Check arg.
    assert( tos is-path-list? )
    cr
    rot                 \ u pth-lst0 c-addr
    #2 pick             \ u pth-lst0 c-addr u
    type                \ u pth-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk pthx
        .path

        link-get-next
        dup 0<> if
            cr over spaces
        then
    repeat
                        \ u
    drop
    cr
;

