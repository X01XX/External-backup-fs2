
\ Check TOS for regioncorrint-list.
: is-regioncorrint-list? ( tos -- bool )
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
    is-regioncorrint?       \ bool
;

: regioncorrint-list-deallocate ( regci-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorrint-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regc-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' regioncorrint-deallocate ] literal over \ regc-lst0 xt regc-lst0
        list-apply                                  \ regc-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: regioncorr-list-supersets-of ( regc1 regc-lst0 -- regc-lst )
    \ Check args.
    \ cr ." regioncorr-list-in: start: " .stack-gbl cr
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot                   \ ret-lst sta1 reg-lst0

    foreach                         \ ret-lst sta1 reg-lnk0 regx
        #2 pick swap                \ ret-lst sta1 reg-lnk0 sta1 regx
        regioncorr-superset?        \ ret-lst sta1 reg-lnk0 bool
        if
            dup link-get-data       \ ret-lst sta1 reg-lnk0 regx
            #3 pick                 \ ret-lst sta1 reg-lnk0 regx ret-lst
            list-push-struct        \ ret-lst sta1 reg-lnk0
        then
    next
                                    \ ret-lst sta1
    drop
;

\ Print a regioncorrint list.
: .regioncorrint-list ( regci-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorrint-list? )
    ." ("
    foreach                 \ regci-lnk regcix
        .regioncorrint
        link-get-next
        dup 0> if space then
    repeat
    ." )"
;

: .regioncorrint-list-prefix ( c-addr u regci-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorrint-list? )
    cr
    rot                 \ u regci-lst0 c-addr
    #2 pick             \ u regci-lst0 c-addr u
    type                \ u regci-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk grpx
        .regioncorrint

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;
