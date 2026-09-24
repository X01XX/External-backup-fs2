\ Functions for actionstep lists.

\ Check TOS for actionstep-list.
: is-actionstep-list? ( tos -- bool )
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
    is-actionstep?            \ bool
;

\ Deallocate a actionstep list.
: actionstep-list-deallocate ( actstp-lst0 -- )
    \ Check arg.
    assert( tos is-actionstep-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ actstp-lst0 uc
    #2 < if
        \ Deallocate actionstep instances in the list.
        [ ' actionstep-deallocate ] literal over      \ actstp-lst0 xt actstp-lst0
        list-apply                                  \ actstp-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a actionstep-list
: .actionstep-list ( actstp-lst0 -- )
    \ Check arg.
    assert( tos is-actionstep-list? )

    [ ' .actionstep ] literal swap .list
;
