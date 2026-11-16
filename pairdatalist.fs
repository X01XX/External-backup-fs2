\ Functions for pairdata lists.

\ Check TOS for pairdata-list.
: is-pairdata-list? ( tos -- bool )
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
    is-pairdata?            \ bool
;

\ Deallocate a pairdata list.
: pairdata-list-deallocate ( prdt-lst0 -- )
    \ Check arg.
    assert( tos is-pairdata-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ reg-lst0 uc
    #2 < if
        \ Deallocate pairdata instances in the list.
        [ ' pairdata-deallocate ] literal over        \ reg-lst0 xt reg-lst0
        list-apply                                  \ reg-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Return the possible regions implied by one, or more, pairdata instances.
: pairdata-list-possible-regions ( prdt-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-pairdata-list? )
    assert( tos list-is-not-empty? )

    \ Init return list.
    list-new swap                               \ ret-lst' prdt-lst0
    dup list-get-first-item                     \ ret-lst' prdt-lst0 prdt-x
    pairdata-get-regions list-get-first-item    \ ret-lst' prdt-lst0 reg-x
    region-get-num-bits region-max-x            \ ret-lst' prdt-lst0 max-reg
    #2 pick list-push-struct                    \ ret-lst' prdt-lst0

    \ Process each pairdata instance.
    foreach                                     \ ret-lst' prdt-lnk prdt-x
        \ Intersect this pairdata's regions.
        pairdata-get-regions                    \ ret-lst' prdt-lnk reg-lst-x
        #2 pick                                 \ ret-lst' prdt-lnk reg-lst-x ret-lst'
        region-list-intersections-nosubs        \ ret-lst' prdt-lnk ret-lst-nxt'

        \ Clean up.
        rot region-list-deallocate              \ prdt-lnk ret-lst-nxt'

        swap                                    \ ret-lst-nxt' prdt-lnk
    next-item
                                                \ ret-lst'
;

\ Return the defining regions implied by one, or more, pairdata instances.
: pairdata-list-defining-regions ( prdt-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-pairdata-list? )
    assert( tos list-is-not-empty? )

    pairdata-list-possible-regions      \ pos-regs'
    dup region-list-defining-regions    \ pos-regs' def-regs'
    swap region-list-deallocate         \ def-regs'
;

\ Return a copy of a list except for one item,
\ specified by an index.
: pairdata-list-copy-except-one  ( inx1 prdt-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-pairdata-list? )
    assert( tos list-is-not-empty? )
    assert( nos 0>= )
    assert( 2dup list-get-length < )
    assert( dup list-get-length 1 > )

    \ Init return list.
    list-new -rot           \ ret-lst inx1 prdt-lst0

    \ Prep for loop.
    dup list-get-length     \ ret-lst inx1 prdt-lst0 len
    swap list-get-links     \ ret-lst inx1 len prdt-lnk
    swap 0                  \ ret-lst inx1 prdt-lnk len 0

    do
        i #2 pick           \ ret-lst inx1 prdt-lnk i inx1
        =                   \ ret-lst inx1 prdt-lnk bool
        ifnot
            dup link-get-data       \ ret-lst inx1 prdt-lnk prdt
            #3 pick                 \ ret-lst inx1 prdt-lnk prdt ret-lst
            list-push-end-struct    \ ret-lst inx1 prdt-lnk
        then

        link-get-next
    loop
                            \ ret-lst inx1 prdt-lnk
    2drop
;

\ Return a pairdata list with one less, unneeded, instance.
: pairdata-list-remove-one-unneeded-instance ( prdt-lst0 -- prdt-lst t | f )
    \ Check arg.
    assert( tos is-pairdata-list? )
    assert( tos list-is-not-empty? )

    \ Check list length.
    dup list-get-length                 \ prdt-lst0 len
    #2 <                                \ prdt-lst0 bool
    if
        drop
        false
        exit
    then

    \ Get base defining regions.
    dup
    pairdata-list-defining-regions      \ prdt-lst0 def-regs'
    swap                                \ def-regs' prdt-lst0

    \ Prep for loop.
    dup list-get-length 0               \ def-regs' prdt-lst0 limit start

    do                                  \ def-regs' prdt-lst0
        i over                          \ def-regs' prdt-lst0 inx prdt-lst0
        pairdata-list-copy-except-one   \ def-regs' prdt-lst0 prdt-lst'
        dup
        pairdata-list-defining-regions  \ def-regs' prdt-lst0 prdt-lst' def-regs2'
        dup #4 pick                     \ def-regs' prdt-lst0 prdt-lst' def-regs2' def-regs2' def-regs'
        region-lists-eq?                \ def-regs' prdt-lst0 prdt-lst' def-regs2' bool
        if
            region-list-deallocate      \ def-regs' prdt-lst0 prdt-lst'
            nip                         \ def-regs' prdt-lst'
            swap region-list-deallocate \ prdt-lst'
            true
            unloop
            exit
        then

        region-list-deallocate          \ def-regs' prdt-lst0 prdt-lst'
        pairdata-list-deallocate        \ def-regs' prdt-lst0
    loop
                                        \ def-regs' prdt-lst0
    drop region-list-deallocate
    false
;
