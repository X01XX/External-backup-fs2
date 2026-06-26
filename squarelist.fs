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

\ Deallocate a list of square pairs.
: square-pair-list-deallocate ( sqr-pr-lst0 -- )
    \ Check arg.
    assert-tos-is-list
    \ cr ." square-pair-list-deallocate: start: " .stack-gbl cr
    [ ' square-deallocate ] literal     \ sqr-pr-lst0 xt
    swap                                \ xt sqr-pr-lst0
    list-deallocate-recursive-struct    \
    \ cr ." square-pair-list-deallocate: end: " .stack-gbl cr
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

\ Return the base pn value, from a non-empty list.
: square-list-base-pn ( sqr-lst0 -- pn )
    \ Check arg.
    assert-tos-is-square-list

    \ Check list length.
    dup                                 \ sqr-lst0 len
    list-is-empty? abort" list is empty?"

    \ Get base pn.
    0 over square-list-any-pn-eq?       \ sqr-lst0 bool
    if
        drop 0                          \ pn
    else
        2 over square-list-any-pn-eq?   \ sqr-lst0 bool
        if
            drop 2                      \ pn
        else
            drop 1                      \ pn
        then
    then
;

\ Return a pair of incompatible squares, if any, from a list of square pairs.
\ If there is more than one pair, return the closest pair.
\ If more than one pair is of equal closeness, return the pair with the most samples.
\ If there is more than one pair with equal closeness, and number samples, return an
\ arbitrary pick.
: square-list-choose-square-pair ( pr-lst0 -- sqr-pr t | f )
    \ Check arg.
    assert-tos-is-list
    \ cr ." square-list-choose-square-pair: start: " .stack-gbl cr

    \ Check for an empty list.
    dup list-is-empty?                      \ pr-lst0 bool
    if
        drop
        false
        \ cr ." square-list-choose-square-pair: exit 1" cr
        exit
    then

    \ Check for one pair.
    dup list-get-length                     \ pr-lst0 len
    1 =
    if
        dup list-pop-struct                 \ pr-lst0, sqr-pr t | f
        if
            nip                             \ sqr-pr
            true
            \ cr ." square-list-choose-square-pair: exit 2" cr
            exit
        else
            ." pop failed?" abort
        then
    then

    \ More than one pair, get min distance pairs.

    \ Init min distance.
    9999                                    \ pr-lst0 min-dis
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
            list-push-struct-list           \ pr-lst0 min-dis pr-lst2 pr-lnk
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
            swap list-deallocate            \ sqr-pr
            true
            \ cr ." square-list-choose-square-pair: exit 3" cr
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
            list-push-struct-list           \ pr-lst2 max-ns pr-lst3 pr-lnk
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
            true
            \ cr ." square-list-choose-square-pair: exit 4" cr
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

        true
        \ cr ." square-list-choose-square-pair: exit 5" cr
        exit
    else
        ." pop failed?" abort
    then
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
    \ cr ." square-list-find-incompatible-pair: start: " .stack-gbl cr

    \ Check list length.
    dup list-get-length                 \ s/qr-lst0 len
    #2 < if
        \ No pairs to check.
        drop
        false
        \ cr ." square-list-find-incompatible-pair: exit 1" cr
        exit
    then

    \ Get base pn.
    dup square-list-base-pn swap        \ pn sqr-lst0

    \ Init the incompatible pair list.
    list-new
    \ cr ." list-new 3: " dup hex. cr
    swap                       \ pn inc-lst sqr-lst0

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
                    \ cr ." list-new 4: " dup hex. cr

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
        drop                                \ pn inc-lst sqr-lnx
        link-get-next
    repeat
                                            \ pn inc-lst
    nip                                     \ inc-lst

    dup                                     \ inc-lst inc-lst
    square-list-choose-square-pair          \ inc-lst, sqr-lst t | f
    if
        dup struct-inc-use-count
        swap                                \ sqr-pr inc-lst
        \ cr ." at 3: " .stack-gbl cr cr
        square-pair-list-deallocate         \ sqr-pr
        \ cr ." at 4: " .stack-gbl cr cr
        true
    else
        list-deallocate
        false
    then
;

\ Find a square in a list, by state, if any.
: square-list-find ( sta1 list0 -- sqr t | f )
    \ Check args.
    assert-tos-is-square-list
    assert-nos-is-state

    [ ' square-state-eq ] literal -rot list-find
;

\ Return a region built from squares of the highest pn value, in a list.
: square-list-region ( sqr-lst0 -- reg t | f )
    \ Check arg.
    assert-tos-is-square-list
    dup list-is-empty?
    if
        drop
        false
        exit
    then

    \ Get highest pn value
    dup square-list-base-pn swap    \ pn sqr-lst

    \ Prep for loop.
    list-get-links                  \ pn link

    \ Set ruturn region to null.
    0 swap                          \ pn reg link

    \ Scan square list.
    begin
        ?dup
    while
        \ Check if square pn is equal to the max pn of the list.
        dup link-get-data               \ pn reg link sqr
        dup square-get-pn               \ pn reg link sqr s-pn
        #4 pick                         \ pn reg link sqr s-pn max-pn
        = if
            square-get-state            \ pn reg link sta
            rot                         \ pn link sta reg
            dup 0=
            if
                                        \ pn link sta 0
                drop                        \ pn link sta
                dup                         \ pn link sta sta
                region-new                  \ pn link reg
                swap                        \ pn reg link
            else
                                            \ pn link sta reg
                2dup                        \ pn link sta reg sta reg
                region-superset-of-state?   \ pn link sta reg flag
                if
                    nip swap                \ pn reg link
                else
                    \ Add state to expand return region.
                    tuck                    \ pn link reg sta reg
                    region-union-state      \ pn link reg reg2
                    swap region-deallocate  \ pn link reg2
                    swap                    \ pn reg2 link
                then
            then
        else
            drop
        then

        link-get-next           \ pn reg link
    repeat
                                \ pn reg
    nip                         \ reg
    true
;

\ Return the first square matching a given pn value.
: square-list-first-pn-eq ( pn sqr-lst0 -- sqr t | f )
    \ Check arg.
    assert-tos-is-square-list

    \ Prep for loop.
    list-get-links          \ pn sqr-lnk

    begin
        ?dup
    while
        dup link-get-data   \ pn sqr-lnk sqrx
        square-get-pn       \ pn sqr-lnk s-pn
        #2 pick             \ pn sqr-lnk s-pn pn
        = if
            link-get-data   \ pn sqrx
            nip             \ sqrx
            true
            exit
        then
        link-get-next
    repeat
                            \ pn
    drop
    false
;

\ Return rules for a square-list.
: square-list-get-rules ( sqr-lst0 -- rul-lst t | f )
    \ Check arg.
    assert-tos-is-square-list
    \ cr ." square-list-get-rules: start: " .stack-gbl cr
    \ cr dup .square-list cr

    \ Check for empty list.
    dup list-is-empty?
    if
        drop
        false
        \ cr ." square-list-get-rules: exit 1: " .stack-gbl cr
    then

    dup square-list-base-pn         \ sqr-lst0 max-pn

    \ Check for 0/U
    dup 0=
    if
        2drop                       \
        list-new                    \ rul-str
        true
        \ cr ." square-list-get-rules: exit 2: " .stack-gbl cr
        exit
    then

    swap                            \ max-pn sqr-lst0

    \ Init return rule list.
    2dup                                \ max-pn sqr-lst0 max-pn sqr-lst0
    square-list-first-pn-eq             \ max-pn sqr-lst0, sqr0 t | f
    invert abort" square-list-get-rules: first pn eq failed?"
    square-get-rules                    \ max-pn sqr-lst0 ret-ruls

    \ Adjust for one deallocate, below.
    list-copy-struct                    \ max-pn sqr-lst0 ret-ruls
    -rot                                \ ret-lst max-pn sqr-lst0

    \ Prep for loop
    list-get-links                      \ ret-lst max-pn link
    begin
        ?dup
    while
        \ Check if the current square pn is equal to the max-pn.
        dup link-get-data               \ ret-lst max-pn link sqr
        square-get-pn                   \ ret-lst max-pn link sqr-pn
        #2 pick                         \ ret-lst max-pn link sqr-pn max-pn
        =                               \ ret-lst max-pn link flag

        if                              \ ret-lst max-pn link
            \ Update the return rule-list.
            rot                         \ max-pn link ret-lst

            over link-get-data          \ max-pn link ret-lst sqr
            square-get-rules            \ max-pn link ret-lst sqr-ruls
            over                        \ max-pn link ret-lst sqr-ruls ret-lst
            \ cr ." about to union: " over .rule-list space dup .rule-list cr
            rule-list-union             \ max-pn link ret-lst, new-rules t | f
            if                          \ max-pn link ret-lst new-rules
                swap                    \ max-pn link new-rules ret-lst
                rule-list-deallocate    \ max-pn link new-rules
                -rot                    \ ret-lst max-pn link
            else                        \ max-pn link ret-lst
                rule-list-deallocate    \ max-pn link
                2drop
                false
                \ cr ." square-list-get-rules: exit 3: " .stack-gbl cr
                exit
            then
        then

        link-get-next
    repeat
                                \ ret-lst max-pn
    drop                        \ ret-lst
    true
    \ cr ." square-list-get-rules: exit 4: " .stack-gbl cr
;

\ Return squares in a given region.
: square-list-in-region ( reg1 sqr-lst0 -- sqr-lst )                                                                           
    \ Check args.
    assert-tos-is-square-list
    assert-nos-is-region

    [ ' square-in-region? ] literal -rot            \ xt reg1 sqr-lst0
    list-find-all-struct                            \ ret-list
;

\ Return true if base-pn elements of a square-list are compatible with a given square.
: square-list-square-compatible? ( sqr1 sqr-lst0 -- bool )
    \ Check args.
    assert-tos-is-square-list
    assert-nos-is-square

    \ Check for empty list.
    dup list-is-empty?
    if
        2drop
        true
        exit
    then

    \ Find base pn.
    dup square-list-base-pn     \ sqr1 sqr-lst0 bpn
    -rot                        \ bpn sqr1 sqr-lst0

    \ Prep for loop.
    list-get-links              \ bpn sqr1 sqr-lnk

    begin
        ?dup
    while
        dup link-get-data       \ bpn sqr1 sqr-lnk sqrx
        square-get-pn           \ bpn sqr1 sqr-lnk spn
        #3 pick                 \ bpn sqr1 sqr-lnk spn bpn
        =
        if
            dup link-get-data   \ bpn sqr1 sqr-lnk sqrx
            #2 pick             \ bpn sqr1 sqr-lnk sqrx sqr1
            squares-compare     \ bpn sqr1 sqr-lnk char
            [char] I =          \ bpn sqr1 sqr-lnk bool
            if
                2drop drop
                false
                exit
            then
        then

        link-get-next
    repeat
                                \ bpn sqr1
    2drop
    true
;

\ Return a region built from squares of the highest pn value, in a list.
: square-list-pnc-squares ( sqr-lst0 -- sqr-lst t | f )
    \ Check arg.
    assert-tos-is-square-list

    dup list-is-empty?
    if
        drop
        false
        exit
    then

    \ Init return list.
    list-new swap                   \ ret-lst sqr-lst

    \ Prep for loop.
    list-get-links                  \ ret-lst link

    \ Scan square list.
    begin
        ?dup
    while
        \ Check if square is pnc.
        dup link-get-data           \ ret-lst link sqr
        square-get-pnc              \ ret-lst link s-pnc
        if
            dup link-get-data       \ ret-lst link sqr
            #2 pick                 \ ret-lst link sqr ret-lst
            list-push-struct        \ ret-lst link
        then

        link-get-next               \ ret-lst link
    repeat
                                    \ ret-lst
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
;
