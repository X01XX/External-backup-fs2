\ Functions for square lists.

\ Check if tos is an empty list, or has a square instance as its first item.
: assert-tos-is-square-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-square
        drop
    then
;

\ Check if nos is an empty list, or has a square instance as its first item.
: assert-nos-is-square-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-square
        drop
    then
;

\ Deallocate a square list.
: square-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-square-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate square instances in the list.
        [ ' square-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a square-list
: .square-list ( list0 -- )
    \ Check arg.
    assert-tos-is-square-list

    [ ' .square ] literal swap .list
;

\ Print square list states.
: .square-list-states ( sqr-lst -- )
    \ Check arg.
    assert-tos-is-square-list

    [ ' .square-state ] literal swap .list
;

\ Return true if anf square in a list is between twe given squares.
: square-list-any-between? ( sqr2 sqr1 btw-lst0 -- bool )
    \ Check args.
    assert-tos-is-square-list
    assert-nos-is-square
    assert-3os-is-square

    list-get-links          \ sqr2 sqr1 btw-lnk

    begin
        ?dup
    while
        #2 pick #2 pick #2 pick \ sqr2 sqr1 btw-lnk sqr2 sqr1 btw-lnk
        link-get-data           \ sqr2 sqr1 btw-lnk sqr2 sqr1 sqrx
        square-between?         \ sqr2 sqr1 btw-lnk bool
        if
            2drop drop
            true
            exit
        then

        link-get-next
    repeat
                            \ sqr2 sqr1
    2drop
    false
;

\ Given a square (3os) a secand square (nos) and a list of squares,
\ return a list of squares where the nos square is between the 3os an square-list
\ squares.
: square-list-between-any ( sqr2 btw1 sqr-lst0 -- sqr-lst )
    \ Check args.
    assert-tos-is-square-list
    assert-nos-is-square
    assert-3os-is-square

    \ Init return list.
    list-new                \ sqr2 btw1 sqr-lst0 ret-lst
    swap list-get-links     \ sqr2 btw1 ret-lst lnk

    begin
        ?dup
    while
        #3 pick                 \ sqr2 btw1 ret-lst lnk sqr2
        over link-get-data      \ sqr2 btw1 ret-lst lnk sqr2 sqrx
        #4 pick                 \ sqr2 btw1 ret-lst lnk sqr2 btwx
        square-between?         \ sqr2 btw1 ret-lst lnk bool
        if
            dup link-get-data   \ sqr2 btw1 ret-lst lnk sqrx
            #2 pick             \ sqr2 btw1 ret-lst lnk sqrx ret-lst
            list-push-struct    \ sqr2 btw1 ret-lst lnk
        then

        link-get-next
    repeat
                            \ sqr2 sqr1 ret-lst
    nip nip
;

\ Return true if any square in a list has a pn value equal to a given pn value.
: square-list-any-pn-eq? ( pn1 sqr-lst0 -- bool )
    \ Check args.
    assert-tos-is-square-list
    over 0< abort" Invalid pn value"
    over 2 > abort" Invalid pn value"

    \ Prep for loop.
    list-get-links          \ pn1 sqr-lnk

    begin
        ?dup
    while
        over                \ pn1 sqr-lnk pn1
        over link-get-data  \ pn1 sqr-lnk pn1 sqrx
        square-get-pn       \ pn1 sqr-lnk pn1 sqr-pn
        = if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                            \ pn1
    drop
    false
;


\ Return the distance of two squares.
: square-pair-get-distance ( sqr-pr0 -- u )
    \ Check arg.
    assert-tos-is-square-list

    dup list-get-first-item square-get-state    \ sqr-pr0 sta1
    swap list-get-second-item square-get-state  \ sta1 sta2
    states-distance
;

\ Return the sum of the number of samples of two squares. 
: square-pair-get-num-samples ( sqr-pr0 -- u )
    \ Check arg.
    assert-tos-is-square-list

    dup list-get-first-item square-get-num-samples      \ sqr-pr0 ns1
    swap list-get-second-item square-get-num-samples    \ ns1 sn2
    +
;

\ Return a pair of incompatible squares, if any, from a list of squares.
\ If there is more than one pair, return the closest pair.
\ If more than one pair is of equal closeness, return the pair with the most samples.
\ If there is more than one pair with equal closeness, and number samples, return an
\ arbitrary pick.
\
\ Given a list of possible regions, from ~A + ~B calculations,
\ squares in each region can be gathered.
\ If there is no incompatible pair, a group can be made.
\ else the incompatible pair needs to be resolved, to improve the possible regions.
\
\ The incompatible pair selection is intended to minimize the effort of resolving
\ a pair. That is, getting samples so the pair are pnc, then finding samples between
\ them, if they are not adjacent.
: square-list-find-incompatible-pair ( sqr-lst0 -- sqr-lst t | f )
    \ Check arg.
    assert-tos-is-square-list

    \ Check list length.
    dup list-get-length                 \ sqr-lst0 len
    2 < if
        \ No pairs to check.
        drop
        false
        \ cr ." square-list-find-incompatible-pair: exit 1" cr
        exit
    then

    \ Get base pn.
    0 over square-list-any-pn-eq?       \ sqr-lst0 bool
    if
        0 swap                          \ pn sqr-lst0
    else
        2 over square-list-any-pn-eq?   \ sqr-lst0 bool
        if
            2 swap                      \ pn sqr-lst0
        else
            1 swap                      \ pn sqr-lst0
        then
    then

    \ Init the incompatible pair list.
    list-new swap                       \ pn inc-lst sqr-lst0

    \ Check every possible pair.
    \ For each pair, at least one must have the base pn, if so compare them.
    list-get-links                      \ pn inc-lst sqr-lnk

    begin
        ?dup
    while
        \ Check if loop 1 square pn is equal to the base pn.
        dup link-get-data               \ pn inc-lst sqr-lnx sqr1
        square-get-pn                   \ pn inc-lst sqr-lnx pn1
        #3 pick                         \ pn inc-lst sqr-lnx pn1 pn
        =                               \ pn inc-lst sqr-lnx bool1

        \ Check next squares.
        over link-get-next              \ pn inc-lst sqr-lnx bool1 nxt-lnk
        begin
            ?dup
        while
            \ Check if loop 2 square pn is equal to the base pn.
            dup link-get-data           \ pn inc-lst sqr-lnx bool1 nxt-lnk sqr2
            square-get-pn               \ pn inc-lst sqr-lnx bool1 nxt-lnk pn2
            #5 pick                     \ pn inc-lst sqr-lnx bool1 nxt-lnk pn2 pn
            =                           \ pn inc-lst sqr-lnx bool1 nxt-lnk bool2

            \ Check that at least one square of the pair has a pn equal to the base pn.
            #2 pick                     \ pn inc-lst sqr-lnx bool1 nxt-lnk bool2 bool1
            or                          \ pn inc-lst sqr-lnx bool1 nxt-lnk bool12
            if
                \ Check the pair.
                #2 pick link-get-data   \ pn inc-lst sqr-lnx bool1 nxt-lnk sqr1
                over link-get-data      \ pn inc-lst sqr-lnx bool1 nxt-lnk sqr1 sqr2
                squares-compare         \ pn inc-lst sqr-lnx bool1 nxt-lnk char
                [char] I =
                if
                    \ Init pair list.
                    list-new                \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst

                    \ Add loop1 square.
                    #3 pick                 \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst sqr-lnk
                    link-get-data           \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst sqr1
                    over list-push-struct   \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst

                    \ Add loop2 square.
                    over                    \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst nxt-lnk
                    link-get-data           \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst sqr2
                    over list-push-struct   \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst

                    \ Save pair.
                    #4 pick                 \ pn inc-lst sqr-lnx bool1 nxt-lnk pr-lst inc-lst
                    list-push-struct        \ pn inc-lst sqr-lnx bool1 nxt-lnk
                then
            then

            link-get-next
        repeat
                                        \ pn inc-lst sqr-lnx bool1
        drop                            \ pn inc-lst sqr-lnx
        link-get-next
    repeat
                                        \ pn inc-lst
    nip                                 \ inc-lst

    \ Check for an empty list.
    dup list-is-empty?
    if
        list-deallocate
        false
        \ cr ." square-list-find-incompatible-pair: exit 1.5" cr
        exit
    then

    \ Check for one pair.
    dup list-get-length
    1 =
    if
        dup list-pop-struct             \ inc-lst, sqr-pr t | f
        if
            swap list-deallocate        \ sqr-pr
            true
            \ cr ." square-list-find-incompatible-pair: exit 2" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair, get min distance pairs.

    \ Init min distance.
    9999                            \ inc-lst min-dis
    over list-get-links             \ inc-lst min-dis inc-lnk

    begin
        ?dup
    while
        dup link-get-data           \ inc-lst min-dis inc-lnk sqr-prx
        square-pair-get-distance    \ inc-lst min-dis inc-lnk u
        rot                         \ inc-lst inc-lnk u min-dis
        min                         \ inc-lst inc-lnk min
        swap                        \ inc-lst min-dis inc-lnk

        link-get-next
    repeat
                                    \ inc-lst min-dis
    \ cr ." min distance: " dup dec. cr

    \ Gather square pairs with min distance.

    \ Init new inc-lst.
    list-new                        \ inc-lst min-dis inc-lst2

    #2 pick list-get-links          \ inc-lst min-dis inc-lst2 inc-lnk
    begin
        ?dup
    while
        dup link-get-data           \ inc-lst min-dis inc-lst2 inc-lnk sqr-prx
        square-pair-get-distance    \ inc-lst min-dis inc-lst2 inc-lnk dis
        #3 pick                     \ inc-lst min-dis inc-lst2 inc-lnk dis min-dis
        =
        if
            dup link-get-data       \ inc-lst min-dis inc-lst2 inc-lnk sqr-prx
            #2 pick                 \ inc-lst min-dis inc-lst2 inc-lnk sqr-prx inc-lst2
            list-push-struct-list   \ inc-lst min-dis inc-lst2 inc-lnk
        then

        link-get-next
    repeat

    \ Clean up.
                                        \ inc-lst min-dis inc-lst2
    nip                                 \ inc-lst inc-lst2

    \ Deallocate previous list with square pairs.
    swap                                \ inc-lst2 inc-lst
    [ ' square-deallocate ] literal     \ inc-lst2 inc-lst xt
    swap                                \ inc-lst2 xt inc-lst
    list-deallocate-recursive-struct    \ inc-lst2

    \ Check for one pair in new list.
    dup list-get-length
    1 =
    if
        dup list-pop-struct             \ inc-lst2, sqr-pr t | f
        if
            swap list-deallocate        \ sqr-pr
            true
            \ cr ." square-list-find-incompatible-pair: exit 3" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair, get max number samples.

    \ Init max num samples.
    0                               \ inc-lst max-ns
    over list-get-links             \ inc-lst max-ns inc-lnk

    begin
        ?dup
    while
        dup link-get-data           \ inc-lst max-ns inc-lnk sqr-prx
        square-pair-get-num-samples \ inc-lst max-ns inc-lnk u
        rot                         \ inc-lst inc-lnk u max-ns
        max                         \ inc-lst inc-lnk max
        swap                        \ inc-lst max-ns inc-lnk

        link-get-next
    repeat
                                    \ inc-lst max-ns

    \ cr ." max samples: " dup dec. cr
    \ Gather square pairs with max number samples.

    \ Init new inc-lst.
    list-new                        \ inc-lst max-ns inc-lst2

    #2 pick list-get-links          \ inc-lst max-ns inc-lst2 inc-lnk
    begin
        ?dup
    while
        dup link-get-data           \ inc-lst max-ns inc-lst2 inc-lnk sqr-prx
        square-pair-get-num-samples \ inc-lst max-ns inc-lst2 inc-lnk dis
        #3 pick                     \ inc-lst max-ns inc-lst2 inc-lnk dis max-ns
        =
        if
            dup link-get-data       \ inc-lst max-ns inc-lst2 inc-lnk sqr-prx
            #2 pick                 \ inc-lst max-ns inc-lst2 inc-lnk sqr-prx inc-lst2
            list-push-struct-list   \ inc-lst max-ns inc-lst2 inc-lnk
        then

        link-get-next
    repeat
                                        \ inc-lst max-ns inc-lst2

    \ Clean up.
    nip                                 \ inc-lst inc-lst2

    \ Deallocate previous list with square pairs.
    swap                                \ inc-lst2 inc-lst
    [ ' square-deallocate ] literal     \ inc-lst2 inc-lst xt
    swap                                \ inc-lst2 xt inc-lst
    list-deallocate-recursive-struct    \ inc-lst2

    \ Check for one pair in new list.
    dup list-get-length
    1 =
    if
        dup list-pop-struct             \ inc-lst sqr-pr
        if
            swap list-deallocate        \ sqr-pr
            true
            \ cr ." square-list-find-incompatible-pair: exit 4" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair.
    dup list-pop-struct                 \ inc-lst, sqr-pr t | f
    if
        swap                            \ sqr-pr inc-lst

        \ Deallocate list with one, or more, square pairs.
        [ ' square-deallocate ] literal     \ sqr-pr inc-lst xt
        swap                                \ sqr-pr xt inc-lst
        list-deallocate-recursive-struct    \ sqr-pr

        true
        \ cr ." square-list-find-incompatible-pair: exit 5" cr
        exit
    else
        ." pop failed?" abort
    then
;

\ Find a square in a list, by state, if any.
: square-list-find ( sta1 list0 -- sqr t | f )                                                                                                
    \ Check args.
    assert-tos-is-list
    assert-nos-is-state

    [ ' square-state-eq ] literal -rot list-find
;
