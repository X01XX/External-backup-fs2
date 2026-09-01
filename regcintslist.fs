
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

\ Print a regcints list.
: .regcints-list ( regcis-lst0 -- )
    \ Check arg.
    assert( tos is-regcints-list? )
    ." ("
    foreach                 \ regcis-lnk regcisx
        .regcints
        link-get-next
        dup 0> if space then
    repeat
    ." )"
;

: .regcints-list-prefix ( c-addr u regcis-lst0 -- )
    \ Check arg.
    assert( tos is-regcints-list? )
    cr
    rot                 \ u regcis-lst0 c-addr
    #2 pick             \ u regcis-lst0 c-addr u
    type                \ u regcis-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk grpx
        .regcints

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

: regcints-list-deallocate ( regcis-lst0 -- )
    \ Check arg.
    assert( tos is-regcints-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regcis-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' regcints-deallocate ] literal over      \ regcis-lst0 xt regcis-lst0
        list-apply                                  \ regcis-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Generate a regcints list from a complement list and an intersections list.
: regcints-list-generate ( cmp-lst1 int-lst0 -- regcints-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    \ Init regcints list.
    list-new                        \ cmp-lst1 int-lst0 regcis-lst
    rot                             \ int-lst0 regcis-lst cmp-lst1

    foreach                         \ int-lst0 regcis-lst cmp-lnk1 cmpx
        #3 pick                     \ int-lst0 regcis-lst cmp-lnk1 cmpx int-lst0
        regioncorr-list-subsets-of  \ int-lst0 regcis-lst cmp-lnk1 regc-lst
        over link-get-data          \ int-lst0 regcis-lst cmp-lnk1 regc-lst cmpx
        regcints-new                \ int-lst0 regcis-lst cmp-lnk1, regci t | f
        invert abort" regcints-new failed?"
        \ cr ." recgints: " dup .regcints cr
        #2 pick                     \ int-lst0 regcis-lst cmp-lnk1 regci regcis-lst
        list-push-end-struct        \ int-lst0 regcis-lst cmp-lnk1
    next
                                    \ int-lst0 regcis-lst
    nip
;

\ Return a list of regcints with a regioncorr that is superset of a given regioncorr.
: regcints-list-superset-of ( regc1 regcis-lst0 -- regcis-lst )
    \ Check args.
    assert( tos is-regcints-list? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot               \ ret-lst regc1 regcis-lst0

    foreach                     \ ret-lst regc1 regcis-lnk0 regcis
        regcints-get-regioncorr \ ret-lst regc1 regcis-lnk0 regc0
        #2 pick                 \ ret-lst regc1 regcis-lnk0 regc0 regc1
        swap                    \ ret-lst regc1 regcis-lnk0 regc1 regc0
        regioncorr-superset?    \ ret-lst regc1 regcis-lnk0 bool
        if
            dup link-get-data   \ ret-lst regc1 regcis-lnk0 regcis
            #3 pick             \ ret-lst regc1 regcis-lnk0 regcis ret-lst
            list-push-struct    \ ret-lst regc1 regcis-lnk0
        then
    next
                                \ ret-lst regc1
    drop
;

\ Return a list of regcints with a regioncorr that intersects of a given regioncorr.
: regcints-list-intersections ( regc1 regcis-lst0 -- regcis-lst )
    \ Check args.
    assert( tos is-regcints-list? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot               \ ret-lst regc1 regcis-lst0

    foreach                     \ ret-lst regc1 regcis-lnk0 regcis
        regcints-get-regioncorr \ ret-lst regc1 regcis-lnk0 regc0
        #2 pick                 \ ret-lst regc1 regcis-lnk0 regc0 regc1
        swap                    \ ret-lst regc1 regcis-lnk0 regc1 regc0
        regioncorrs-intersect?  \ ret-lst regc1 regcis-lnk0 bool
        if
            dup link-get-data   \ ret-lst regc1 regcis-lnk0 regcis
            #3 pick             \ ret-lst regc1 regcis-lnk0 regcis ret-lst
            list-push-struct    \ ret-lst regc1 regcis-lnk0
        then
    next
                                \ ret-lst regc1
    drop
;

\ Return a list of regcints that are in both lists.
: regcints-list-set-intersection ( regcis-lst1 regcis-lst0 -- regcis-lst )
    \ Check args.
    assert( tos is-regcints-list? )
    assert( nos is-regcints-list? )

    [ ' = ] literal         \ regcis-lst1 regcis-lst0 xt
    -rot list-intersection-struct
;
