
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

: intregcs-list-deallocate ( iregcs-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ iregcs-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' intregcs-deallocate ] literal over      \ iregcs-lst0 xt iregcs-lst0
        list-apply                                  \ iregcs-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

' intregcs-list-deallocate to intregcs-list-deallocate-xt

\ Print a intregcs list.
: .intregcs-list ( iregcs-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )
    ." ("
    foreach                 \ iregcs-lnk iregcsx
        .intregcs
        link-get-next
        dup 0> if space then
    repeat
    ." )"
;

' .intregcs-list to .intregcs-list-xt

: .intregcs-list-prefix ( c-addr u iregcs-lst0 -- )
    \ Check arg.
    assert( tos is-intregcs-list? )
    cr
    rot                 \ u iregcs-lst0 c-addr
    #2 pick             \ u iregcs-lst0 c-addr u
    type                \ u iregcs-lst0

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
: intregcs-contains-regioncorr? ( regc2 iregcs0 -- bool )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-regioncorr? )

    intregcs-get-regioncorrs           \ regc2 regc-lst

    [ ' = ] literal -rot        \ xt regc2 regc-lst
    list-member?
;

\ Return a list or intregcss that contain a regioncorr.
: intregcs-list-regioncorr-in ( regc2 iregcs-lst0 -- iregcs-lst )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-regioncorr? )
    \ Init return list.
    list-new -rot                           \ ret-lst regc2 iregcs-lst0
    foreach                                 \ ret-lst regc2 iregcs-lnk iregcsx
        #2 pick over                        \ ret-lst regc2 iregcs-lnk iregcsx regc2 iregcsx
        intregcs-contains-regioncorr?       \ ret-lst regc2 iregcs-lnk iregcsx bool
        if
            #3 pick                         \ ret-lst regc2 iregcs-lnk iregcsx ret-lst
            list-push-struct                \ ret-lst regc2 iregcs-lnk
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
: intregcs-list-share-regioncorr-with? ( iregcs1 iregcs-lst0 -- bool )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-intregcs? )

    foreach                                 \ iregcs1 iregcs-lnk0 iregcs0
        #2 pick                             \ iregcs1 iregcs-lnk0 iregcs0 iregcs1
        intregcss-share-regioncorr?         \ iregcs1 iregcs-lnk0 bool
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
: intregcs-list-regioncorr-list ( iregcs-lst0 -- regc-lst )
    \ Check arg.
    assert( tos is-intregcs-list? )

    \ Init working/return list.
    list-new swap                   \ wrk-lst' iregcs-lst0

    foreach                         \ wrk-lst' iregcs-lnk0 iregcs
        \ Get union of work list and current iregcs.
        [ ' regioncorrs-eq? ] literal swap  \ wrk-lst' iregcs-lnk0 xt iregcs
        intregcs-get-regioncorrs    \ wrk-lst' iregcs-lnk0 xt regc-lst
        #3 pick                     \ wrk-lst' iregcs-lnk0 xt regc-lst wrk-lst'
        list-union-struct           \ wrk-lst' iregcs-lnk0 wrk-lst2'

        \ Deallocate old list.
        rot                         \ iregcs-lnk0 wrk-lst2' wrk-lst'
        regioncorr-list-deallocate  \ iregcs-lnk0 wrk-lst2'
        swap                        \ wrk-lst2' iregcs-lnk0
    next
                                    \ ret-lst
;

\ Return true if a intregcs is not already in a list, and has at
\ least one regioncorr in its list that is matched in a
\ intregcs in the list, and at least one that is not matched.
\
\ So adding the intregcs to the list will extend the reach of the
\ connected intregcss.
: intregcs-list-add-item? ( iregcs1 iregcs-lst0 -- bool )
    \ Check args.
    assert( tos is-intregcs-list? )
    assert( nos is-intregcs? )

    \ Check if its already in the list.
    [ ' = ] literal                         \ iregcs1 iregcs-lst0 xt
    #2 pick #2 pick                         \ iregcs1 iregcs-lst0 xt iregcs1 iregcs-lst0
    list-member?                            \ iregcs1 iregcs-lst0 bool
    if
        2drop
        false
        exit
    then

    \ Get intersection of regioncorr lists in both args.
    intregcs-list-regioncorr-list           \ iregcs1 regc-lst0'
    swap intregcs-get-regioncorrs           \ regc-lst0' regc-lst1
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
    \ in iregcs1.
    dup list-get-length                 \ regc-lst0' regc-lst1 regc-int' len
    swap regioncorr-list-deallocate     \ regc-lst0' regc-lst1 len
    swap list-get-length                \ regc-lst0' len len
    <>                                  \ regc-lst0' bool
    swap regioncorr-list-deallocate     \ bool
;

\ Return a list of regioncorr lists, where each list allows
\ maximum mapping of paths from intregcs to intregcs.
: intregcs-list-find-connections ( iregcs-lst0 -- list of regioncorr-lists )

    \ Init return list-of-lists.
    list-new swap                   \ lol iregcs-lst0

    \ Init aggregate list of all regioncorrs included in lol.
    list-new swap                   \ lol agg iregcs-lst0

    begin
        \ Find the max number of regioncorrs any intregcs that
        \ are not in agg.

        \ Init maximum counter.
        0                               \ lol agg iregcs-lst0 max
        over                            \ lol agg iregcs-lst0 max iregcs-lst0
        foreach                         \ lol agg iregcs-lst0 max iregcs-lnk0 iregcs0
            intregcs-get-regioncorrs    \ lol agg iregcs-lst0 max iregcs-lnk0 regc-lst
            #4 pick swap                \ lol agg iregcs-lst0 max iregcs-lnk0 agg regc-lst
            regioncorr-list-num-not-in  \ lol agg iregcs-lst0 max iregcs-lnk0 num
            rot max swap                \ lol agg iregcs-lst0 max iregcs-lnk0
        next

        \ Check max number.             \ lol agg iregcs-lst0 max
        dup 0=
        if
            \ No more to intregcs-lists are needed for lol.
            2drop                       \ lol
            exit
        then

        \ Get first regioncorr with the maximum number of regioncorrs not in agg.
        over                            \ lol agg iregcs-lst0 max iregcs-lst0
        list-get-links                  \ lol agg iregcs-lst0 max iregcs-lnk0
        begin
            #3 pick                     \ lol agg iregcs-lst0 max iregcs-lnk0 agg
            over link-get-data          \ lol agg iregcs-lst0 max iregcs-lnk0 agg iregcs
            intregcs-get-regioncorrs    \ lol agg iregcs-lst0 max iregcs-lnk0 agg regc-lst
            regioncorr-list-num-not-in  \ lol agg iregcs-lst0 max iregcs-lnk0 num
            #2 pick =                   \ lol agg iregcs-lst0 max iregcs-lnk0 bool
            if
                link-get-data           \ lol agg iregcs-lst0 max iregcs
                true                    \ lol agg iregcs-lst0 max iregcs true
            else
                link-get-next           \ lol agg iregcs-lst0 max iregcs-lnk0
                false                   \ lol agg iregcs-lst0 max iregcs-lnk0 false
            then
        until
                                        \ lol agg iregcs-lst0 max iregcs
        nip                             \ lol agg iregcs-lst0 iregcs

        \ Init cycle intregcs list.
        list-new                        \ lol agg iregcs-lst0 iregcs cyc
        tuck list-push-struct           \ lol agg iregcs-lst0 cyc

        cr ." todo" cr



        \ Add cyc to lol.
        #3 pick                         \ lol agg iregcs-lst0 cyc lol
        list-push-end-struct            \ lol agg iregcs-lst0
    again
;

\ Generate a intregcs list from a complement list and an intersections list.
: intregcs-list-generate ( cmp-lst1 int-lst0 -- intregcs-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    \ Init list.
    list-new                                                            \ cmp-lst1 int-lst0 iregcs-lst

    \ For each intersection.
    swap                                                                \ cmp-lst1 iregcs-lst int-lst0
    foreach                                                             \ cmp-lst1 iregcs-lst int-lnk0 intx
        #3 pick                                                         \ cmp-lst1 iregcs-lst int-lnk0 intx cmp-lst1
        regioncorr-list-supersets-of                                    \ cmp-lst1 iregcs-lst int-lnk0 sup-lst
        dup #2 pick link-get-data                                       \ cmp-lst1 iregcs-lst int-lnk0 sup-lst sup-lst intx
        intregcs-new                                                    \ cmp-lst1 iregcs-lst int-lnk0 sup-lst, iregcs t | f
        if
            nip                                                         \ cmp-lst1 iregcs-lst int-lnk0 iregcs
            #2 pick                                                     \ cmp-lst1 iregcs-lst int-lnk0 iregcs iregcs-lst
            list-push-end-struct                                        \ cmp-lst1 iregcs-lst int-lnk0
        else
            regioncorr-list-deallocate                                  \ cmp-lst1 iregcs-lst int-lnk0
        then
    next
                                                                        \ cmp-lst1 iregcs-lst
    nip
;
