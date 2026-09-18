\ Functions for planstep lists.

\ Check TOS for planstep-list.
: is-planstep-list? ( tos -- bool )
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
    is-planstep?            \ bool
;

\ Deallocate a planstep list.
: planstep-list-deallocate ( plnstp-lst0 -- )
    \ Check arg.
    assert( tos is-planstep-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ plnstp-lst0 uc
    #2 < if
        \ Deallocate planstep instances in the list.
        [ ' planstep-deallocate ] literal over      \ plnstp-lst0 xt plnstp-lst0
        list-apply                                  \ plnstp-lst0

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
