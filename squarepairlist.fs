\ Deallocate a list of square pairs.
: square-pair-list-deallocate ( sqr-pr-lst0 -- )
    \ Check arg.
    assert-tos-is-list

    dup struct-get-use-count                      \ sqr-pr-lst0 uc
    #2 <
    if
        \ cr ." square-pair-list-deallocate: start: " .stack-gbl cr
        [ ' square-list-deallocate ] literal    \ sqr-pr-lst0 xt
        over                                    \ sqr-pr-lst0 xt sqr-pr-lst0
        list-apply                              \ sqr-pr-lst
        list-deallocate
    else
        struct-dec-use-count
    then

    \ cr ." square-pair-list-deallocate: end: " .stack-gbl cr
;

' square-pair-list-deallocate to square-pair-list-deallocate-xt

\ Return a pair of incompatible squares, if any, from a list of square pairs.
\ If there is more than one pair, return the closest pair.
\ If more than one pair is of equal closeness, return the pair with the most samples.
\ If there is more than one pair with equal closeness, and number samples, return an
\ arbitrary pick.
\
\ The square pair returned is still in the given square-pair list,
\ and may need to be removed if you want te deallocate the given square-pair-list.
: square-pair-list-choose-pair ( pr-lst0 -- sqr-pr t | f )
    \ Check arg.
    assert-tos-is-list
    \ cr ." square-pair-list-choose-pair: start: " .stack-gbl cr

    \ Check for an empty list.
    dup list-is-empty?                      \ pr-lst0 bool
    if
        drop
        false
        \ cr ." square-pair-list-choose-pair: exit 1" cr
        exit
    then

    \ Check for one pair.
    dup list-get-length                     \ pr-lst0 len
    1 =
    if
        dup list-get-first-item             \ pr-lst0, sqr-pr
        nip                                 \ sqr-pr
        \ cr ." square-pair-list-choose-pair: exit 2: " dup .list-raw
        true
        \ cr ." square-pair-list-choose-pair: exit 2" cr
        exit
    then

    \ More than one pair, get min distance pairs.

    \ Init min distance.
    #9999                                   \ pr-lst0 min-dis
    over list-get-links                     \ pr-lst0 min-dis pr-lnk

    begin
        ?dup
    while
        dup link-get-data                   \ pr-lst0 min-dis pr-lnk sqr-prx
        square-pair-get-distance            \ pr-lst0 min-dis pr-lnk u
        rot                                 \ pr-lst0 pr-lnk u min-dis
        min                                 \ pr-lst0 pr-lnk min
        swap                                \ pr-lst0 min-dis pr-lnk

        link-get-next
    repeat
                                            \ pr-lst0 min-dis
    \ cr ." min distance: " dup dec. cr

    \ Gather square pairs with min distance.

    \ Init new pair list.
    list-new                                \ pr-lst0 min-dis pr-lst2
    \ cr ." list-new 1: " dup hex. cr

    #2 pick list-get-links                  \ pr-lst0 min-dis pr-lst2 pr-lnk
    begin
        ?dup
    while
        dup link-get-data                   \ pr-lst0 min-dis pr-lst2 pr-lnk sqr-prx
        square-pair-get-distance            \ pr-lst0 min-dis pr-lst2 pr-lnk dis
        #3 pick                             \ pr-lst0 min-dis pr-lst2 pr-lnk dis min-dis
        =
        if
            dup link-get-data               \ pr-lst0 min-dis pr-lst2 pr-lnk sqr-prx
            #2 pick                         \ pr-lst0 min-dis pr-lst2 pr-lnk sqr-prx pr-lst2
            list-push-struct                \ pr-lst0 min-dis pr-lst2 pr-lnk
        then

        link-get-next
    repeat

    \ Clean up.
                                            \ pr-lst0 min-dis pr-lst2
    nip                                     \ pr-lst0 pr-lst2

    \ Replace previous pair list.
    nip                                     \ pr-lst2
    \ cr ." pr-lst2: " dup .list-raw cr
    \ Check for one pair in new list.
    dup list-get-length
    1 =
    if
        dup list-pop-struct                 \ pr-lst2, sqr-pr t | f
        if
            \ cr ." square-pair-list-choose-pair: exit 3.1: " .stack-gbl cr
            swap list-deallocate            \ sqr-pr
            \ cr ." square-pair-list-choose-pair: exit 3.2: " .stack-gbl cr
            \ cr ." square-pair-list-choose-pair: exit 3: " dup .list-raw
            true
            \ cr ." square-pair-list-choose-pair: exit 3" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair, get max number samples.

    \ Init max num samples.
    0                                       \ pr-lst2 max-ns
    over list-get-links                     \ pr-lst2 max-ns pr-lnk

    begin
        ?dup
    while
        dup link-get-data                   \ pr-lst2 max-ns pr-lnk sqr-prx
        square-pair-get-num-samples         \ pr-lst2 max-ns pr-lnk u
        rot                                 \ pr-lst2 pr-lnk u max-ns
        max                                 \ pr-lst2 pr-lnk max
        swap                                \ pr-lst2 max-ns pr-lnk

        link-get-next
    repeat
                                            \ pr-lst2 max-ns

    \ cr ." max samples: " dup dec. cr
    \ Gather square pairs with max number samples.

    \ Init new pair list.
    list-new                                \ pr-lst2 max-ns pr-lst3
    \ cr ." list-new 2: " dup hex. cr

    #2 pick list-get-links                  \ pr-lst2 max-ns pr-lst3 pr-lnk
    begin
        ?dup
    while
        dup link-get-data                   \ pr-lst2 max-ns pr-lst3 pr-lnk sqr-prx
        square-pair-get-num-samples         \ pr-lst2 max-ns pr-lst3 pr-lnk dis
        #3 pick                             \ pr-lst2 max-ns pr-lst3 pr-lnk dis max-ns
        =
        if
            dup link-get-data               \ pr-lst2 max-ns pr-lst3 pr-lnk sqr-prx
            #2 pick                         \ pr-lst2 max-ns pr-lst3 pr-lnk sqr-prx pr-lst3
            list-push-struct                \ pr-lst2 max-ns pr-lst3 pr-lnk
        then

        link-get-next
    repeat
                                            \ pr-lst2 max-ns pr-lst3

    \ Clean up.
    nip                                     \ pr-lst2 pr-lst3

    \ Deallocate previous list with square pairs.
    swap                                    \ pr-lst3 pr-lst2
    square-pair-list-deallocate             \ pr-lst3

    \ Check for one pair in new list.
    dup list-get-length
    1 =
    if
        dup list-pop-struct                 \ pr-lst3 sqr-pr
        if
            swap list-deallocate            \ sqr-pr
            \ cr ." square-pair-list-choose-pair: exit 4: " dup .list-raw
            true
            \ cr ." square-pair-list-choose-pair: exit 4" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair.
    dup list-pop-struct                     \ pr-lst3, sqr-pr t | f
    if
        swap                                \ sqr-pr pr-lst3

        \ Deallocate list with one, or more, square pairs.
        square-pair-list-deallocate         \ sqr-pr
        \ cr ." square-pair-list-choose-pair: exit 5: " dup .list-raw
        true
        \ cr ." square-pair-list-choose-pair: exit 5" cr
        exit
    else
        ." pop failed?" abort
    then
;

' square-pair-list-choose-pair to square-pair-list-choose-pair-xt
