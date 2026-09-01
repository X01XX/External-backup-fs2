
\ Check TOS for pathstep-list.
: is-pathstep-list? ( tos -- bool )
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
    is-pathstep?            \ bool
;

\ Print a pathstep-list
: .pathstep-list ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathstep-list? )

    [ ' .pathstep ] literal swap .list
;

: pathstep-list-deallocate ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathstep-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ pthstp-lst0 uc
    #2 < if
        \ Deallocate pathstep instances in the list.
        [ ' pathstep-deallocate ] literal over      \ pthstp-lst0 xt pthstp-lst0
        list-apply                                  \ pthstp-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;
