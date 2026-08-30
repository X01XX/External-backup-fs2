
\ Check TOS for regcints-list.
: is-regcints-list? ( tos -- bool )
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
    is-regcints?            \ bool
;

: regcints-list-deallocate ( regcis-lst0 -- )
    \ Check arg.
    assert( tos is-regcints-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regcis-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' regcints-deallocate ] literal over      \ regcis-lst0 xt regc-lst0
        list-apply                                  \ regcis-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;
