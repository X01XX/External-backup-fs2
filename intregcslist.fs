
\ Check TOS for intregcs-list.
: is-intregcs-list? ( tos -- bool )
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
    is-intregcs?            \ bool
;

: intregcs-list-deallocate ( regci-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regc-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' intregcs-deallocate ] literal over \ regc-lst0 xt regc-lst0
        list-apply                                  \ regc-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

' intregcs-list-deallocate to intregcs-list-deallocate-xt

\ Print a intregcs list.
: .intregcs-list ( regci-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )
    ." ("
    foreach                 \ regci-lnk regcix
        .intregcs
        link-get-next
        dup 0> if space then
    repeat
    ." )"
;

' .intregcs-list to .intregcs-list-xt

: .intregcs-list-prefix ( c-addr u regci-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )
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
        .intregcs

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

' .intregcs-list-prefix to .intregcs-list-prefix-xt

\ Return true if a regioncorr int contains a regioncorr in its regioncorr list.
: intregcs-contains-regioncorr? ( regc2 regci0 -- bool )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-regioncorr? )

    intregcs-get-list           \ regc2 regc-lst

    [ ' = ] literal -rot        \ xt regc2 regc-lst
    list-member?
;

\ Return a list or intregcss that contain a regioncorr.
: intregcs-list-regioncorr-in ( regc2 regci-lst0 -- regci-lst )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-regioncorr? )
    \ Init return list.
    list-new -rot                           \ ret-lst regc2 regci-lst0
    foreach                                 \ ret-lst regc2 regci-lnk regcix
        #2 pick over                        \ ret-lst regc2 regci-lnk regcix regc2 regcix
        intregcs-contains-regioncorr?       \ ret-lst regc2 regci-lnk regcix bool
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

' intregcs-list-regioncorr-in to intregcs-list-regioncorr-in-xt

\ Return true if any intregcs in a list shares a regioncorr
\ with a givet regiotcorrint.
: intregcs-list-share-regioncorr-with? ( regci1 regci-lst0 -- bool )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-intregcs? )

    foreach                                 \ regci1 regci-lnk0 regci0
        #2 pick                             \ regci1 regci-lnk0 regci0 regci1
        intregcss-share-regioncorr?         \ regci1 regci-lnk0 bool
        if
            2drop
            true
            exit
        then
    next
    drop
    false
;

\ Return a list of regioncorrs, in each intregcs, no duplicates.
: intregcs-list-regioncorr-list ( regci-lst0 -- regc-lst )
    \ Check arg.
    assert( tos is-intregcs-list? )

    \ Init working/return list.
    list-new swap                   \ wrk-lst' regci-lst0

    foreach                         \ wrk-lst' regci-lnk0 regci
        \ Get union of work list and current regci.
        [ ' regioncorrs-eq? ] literal swap  \ wrk-lst' regci-lnk0 xt regci
        intregcs-get-list      \ wrk-lst' regci-lnk0 xt regc-lst
        #3 pick                     \ wrk-lst' regci-lnk0 xt regc-lst wrk-lst'
        list-union-struct           \ wrk-lst' regci-lnk0 wrk-lst2'

        \ Deallocate old list.
        rot                         \ regci-lnk0 wrk-lst2' wrk-lst'
        regioncorr-list-deallocate  \ regci-lnk0 wrk-lst2'
        swap                        \ wrk-lst2' regci-lnk0
    next
                                    \ ret-lst
;

\ Return true if a intregcs is not already in a list, and has at
\ least one regioncorr in its list that is matched in a
\ intregcs in the list, and at least one that is not matched.
\
\ So adding the intregcs to the list will extend the reach of the
\ connected intregcss.
: intregcs-list-add-item? ( regci1 regci-lst0 -- bool )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-intregcs? )

    \ Check if its already in the list.
    [ ' = ] literal                         \ regci1 regci-lst0 xt
    #2 pick #2 pick                         \ regci1 regci-lst0 xt regci1 regci-lst0
    list-member?                            \ regci1 regci-lst0 bool
    if
        2drop
        false
        exit
    then

    \ Get intersection of regioncorr lists in both args.
    intregcs-list-regioncorr-list           \ regci1 regc-lst0'
    swap intregcs-get-list                  \ regc-lst0' regc-lst1
    [ ' regioncorrs-eq? ] literal           \ regc-lst0' regc-lst1 xt
    #2 pick #2 pick                         \ regc-lst0' regc-lst1 xt regc-lst0' regc-lst1
    list-intersection-struct                \ regc-lst0' regc-lst1 regc-int'

    \ Check if any intersection.
    dup list-is-empty?
    if
        list-deallocate
        drop
        regioncorr-list-deallocate
        false
        exit
    then

    \ Check if number of common regioncorrs are less then the number
    \ in regci1.
    dup list-get-length                 \ regc-lst0' regc-lst1 regc-int' len
    swap regioncorr-list-deallocate     \ regc-lst0' regc-lst1 len
    swap list-get-length                \ regc-lst0' len len
    <>                                  \ regc-lst0' bool
    swap regioncorr-list-deallocate     \ bool
;

\ Return a list of regioncorr lists, where each list allows
\ maximum mapping of paths from intregcs to intregcs.
: intregcs-list-find-connections ( regci-lst0 -- list of regioncorr-lists )

    \ Init return list-of-lists.
    list-new swap                   \ lol regci-lst0

    \ Init aggregate list of all regioncorrs included in lol.
    list-new swap                   \ lol agg regci-lst0

    begin
        \ Find the max number of regioncorrs any intregcs that
        \ are not in agg.

        \ Init maximum counter.
        0                               \ lol agg regci-lst0 max
        over                            \ lol agg regci-lst0 max regci-lst0
        foreach                         \ lol agg regci-lst0 max regci-lnk0 regci0
            intregcs-get-list           \ lol agg regci-lst0 max regci-lnk0 regc-lst
            #4 pick swap                \ lol agg regci-lst0 max regci-lnk0 agg regc-lst
            regioncorr-list-num-not-in  \ lol agg regci-lst0 max regci-lnk0 num
            rot max swap                \ lol agg regci-lst0 max regci-lnk0
        next

        \ Check max number.             \ lol agg regci-lst0 max
        dup 0=
        if
            \ No more to intregcs-lists are needed for lol.
            2drop                       \ lol
            exit
        then

        \ Get first regioncorr with the maximum number of regioncorrs not in agg.
        over                            \ lol agg regci-lst0 max regci-lst0
        list-get-links                  \ lol agg regci-lst0 max regci-lnk0
        begin
            #3 pick                     \ lol agg regci-lst0 max regci-lnk0 agg
            over link-get-data          \ lol agg regci-lst0 max regci-lnk0 agg regci
            intregcs-get-list           \ lol agg regci-lst0 max regci-lnk0 agg regc-lst
            regioncorr-list-num-not-in  \ lol agg regci-lst0 max regci-lnk0 num
            #2 pick =                   \ lol agg regci-lst0 max regci-lnk0 bool
            if
                link-get-data           \ lol agg regci-lst0 max regci
                true                    \ lol agg regci-lst0 max regci true
            else
                link-get-next           \ lol agg regci-lst0 max regci-lnk0
                false                   \ lol agg regci-lst0 max regci-lnk0 false
            then
        until
                                        \ lol agg regci-lst0 max regci
        nip                             \ lol agg regci-lst0 regci

        \ Init cycle intregcs list.
        list-new                        \ lol agg regci-lst0 regci cyc
        tuck list-push-struct           \ lol agg regci-lst0 cyc

        cr ." todo" cr



        \ Add cyc to lol.
        #3 pick                         \ lol agg regci-lst0 cyc lol
        list-push-end-struct            \ lol agg regci-lst0
    again
;
