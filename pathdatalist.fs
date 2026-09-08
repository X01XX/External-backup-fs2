\ Check TOS for pathdata-list.
: is-pathdata-list? ( tos -- bool )
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
    is-pathdata?            \ bool
;

\ Print a pathdata-list
: .pathdata-list ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathdata-list? )

    [ ' .pathdata ] literal swap .list
;

: .pathdata-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-pathdata-list? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    list-get-links      \ u lnk

    begin
        ?dup
    while
        dup link-get-data .pathdata

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

: pathdata-list-deallocate ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathdata-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ pthstp-lst0 uc
    #2 < if
        \ Deallocate pathdata instances in the list.
        [ ' pathdata-deallocate ] literal over      \ pthstp-lst0 xt pthstp-lst0
        list-apply                                  \ pthstp-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;
