\ Functions for planstep lists.

\ Check TOS for planstep-list.
: is-planstep-list? ( tos -- bool )
    tos is-list?            \ tos bool
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
    is-planstep?            \ bool
;

\ Deallocate a planstep list.
: planstep-list-deallocate ( plnstp-lst0 -- )
    \ Check arg.
    assert( tos is-planstep-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate square instances in the list.
        [ ' planstep-deallocate ] literal over      \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a planstep-list
: .planstep-list ( plnstp-lst0 -- )
    \ Check arg.
    assert( tos is-planstep-list? )

    [ ' .planstep ] literal swap .list
;

: .planstep-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-planstep-list? )
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

    foreach             \ u lnk grpx
        .planstep

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;
