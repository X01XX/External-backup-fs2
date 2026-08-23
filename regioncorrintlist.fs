
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

' regioncorrint-list-deallocate to regioncorrint-list-deallocate-xt

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

' .regioncorrint-list to .regioncorrint-list-xt

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

' .regioncorrint-list-prefix to .regioncorrint-list-prefix-xt

\ Return true if a regioncorr int contains a regioncorr in its regioncorr list.
: regioncorrint-contains-regioncorr? ( regc2 regci0 -- bool )
    \ Check args.
    assert( tos is-regioncorrint? )
    assert( nos is-regioncorr? )

    regioncorrint-get-list          \ regc2 regc-lst

    [ ' = ] literal -rot            \ xt regc2 regc-lst
    list-member?
;

\ Return a list or regioncorrints that contain a regioncorr.
: regioncorrint-list-regioncorr-in ( regc2 regci-lst0 -- regci-lst )
    \ Check args.
    assert( tos is-regioncorrint-list? )
    assert( nos is-regioncorr? )
    \ Init return list.
    list-new -rot                           \ ret-lst regc2 regci-lst0
    foreach                                 \ ret-lst regc2 regci-lnk regcix
        #2 pick over                        \ ret-lst regc2 regci-lnk regcix regc2 regcix
        regioncorrint-contains-regioncorr?  \ ret-lst regc2 regci-lnk regcix bool
        if
            #3 pick                         \ ret-lst regc2 regci-lnk regcix ret-lst
            list-push-struct                \ ret-lst regc2 regci-lnk
        else
            drop
        then
    next
                                            \ ret-lst regc2
    drop
;

' regioncorrint-list-regioncorr-in to regioncorrint-list-regioncorr-in-xt

