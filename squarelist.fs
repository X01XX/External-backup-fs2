\ Functions for square lists.

\ Check TOS for square-list.
: is-square-list? ( tos -- bool )
    tos is-list?            \ tos bool
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
    is-square?              \ bool
;

\ Deallocate a square list.
: square-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-square-list? )

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
    assert( tos is-square-list? )

    [ ' .square ] literal swap .list
;

: .square-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-square-list? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    list-get-links      \ u lnk

    begin
        ?dup
    while
        dup link-get-data .square

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

\ Print square list states.
: .square-list-states ( sqr-lst -- )
    \ Check arg.
    assert( tos is-square-list? )

    [ ' .square-state ] literal swap .list
;

\ Return true if anf square in a list is between twe given squares.
: square-list-any-between? ( sqr2 sqr1 btw-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square? )
    assert( 3os is-square? )

    foreach                     \ sqr2 sqr1 btw-lnk sqrx
        #3 pick swap            \ sqr2 sqr1 btw-lnk sqr2 sqrx
        #3 pick swap            \ sqr2 sqr1 btw-lnk sqr2 sqr1 sqrx

        square-between?         \ sqr2 sqr1 btw-lnk bool
        if
            2drop drop
            true
            exit
        then
    next-item
                            \ sqr2 sqr1
    2drop
    false
;

\ Given a square (3os) a secand square (nos) and a list of squares,
\ return a list of squares where the nos square is between the 3os an square-list
\ squares.
: square-list-between-any ( sqr2 btw1 sqr-lst0 -- sqr-lst )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square? )
    assert( 3os is-square? )

    \ Init return list.
    list-new                    \ sqr2 btw1 sqr-lst0 ret-lst
    swap                        \ sqr2 btw1 ret-lst sqr-lst0

    foreach                     \ sqr2 btw1 ret-lst lnk sqrx
        #4 pick swap            \ sqr2 btw1 ret-lst lnk sqr2 sqrx

        #4 pick                 \ sqr2 btw1 ret-lst lnk sqr2 sqrx btwx
        square-between?         \ sqr2 btw1 ret-lst lnk bool
        if
            dup link-get-data   \ sqr2 btw1 ret-lst lnk sqrx
            #2 pick             \ sqr2 btw1 ret-lst lnk sqrx ret-lst
            list-push-struct    \ sqr2 btw1 ret-lst lnk
        then
    next-item
                            \ sqr2 sqr1 ret-lst
    nip nip
;

\ Return true if any square in a list has a pn value equal to a given pn value.
: square-list-any-pn-eq? ( pn1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-pn? )

    foreach                 \ pn1 sqr-lnk sqrx
        #2 pick swap        \ pn1 sqr-lnk pn1 sqrx
        square-get-pn       \ pn1 sqr-lnk pn1 sqr-pn
        = if
            2drop
            true
            exit
        then
    next-item
                            \ pn1
    drop
    false
;

\ Return a list of squares in a list that have a given pn value.
: square-list-pn-eq-squares ( pn1 sqr-lst0 -- sqr-lst t | f )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-pn? )

    \ Init return list.
    list-new -rot               \ ret-lst pn1 sqr-lst0

    foreach                     \ ret-lst pn1 sqr-lnk sqrx
        square-get-pn           \ ret-lst pn1 sqr-lnk sqr-pn
        #2 pick                 \ ret-lst pn1 sqr-lnk sqr-pn pn1
        = if
            link-get-data       \ ret-lst pn1 sqr-lnk sqrx
            #3 pick             \ ret-lst pn1 sqr-lnk sqrx ret-lst
            list-push-struct    \ ret-lst pn1 sqr-lnk
        then
    next-item
                                \ ret-lst pn1
    drop                        \ ret-lst

    check-list-return
;

\ Return the max pn value, from a non-empty list.
\ Order is 1, 2 then 0.
: square-list-max-pn-value ( sqr-lst0 -- mpn t | f)
    \ Check arg.
    assert( tos is-square-list? )

    \ Check list length.
    dup                                 \ sqr-lst0 bool
    list-is-empty?
    if drop false exit then

    \ Get base pn.
    0 over square-list-any-pn-eq?       \ sqr-lst0 bool
    if
        drop 0                          \ mpn
    else
        #2 over square-list-any-pn-eq?  \ sqr-lst0 bool
        if
            drop #2                     \ mpn
        else
            drop 1                      \ mpn
        then
    then
    true
;

\ Return a list of base-pn squares from a non-empty list.
\ That is, any 0-pn squares, else any 2-pn squares, else any 1-pn squares.
: square-list-max-pn-squares ( sqr-lst1 -- sqr-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )

    dup square-list-max-pn-value    \ sqr-lst1, mpn t | f
    ifnot drop false exit then

    swap                            \ mpn sqr-lst1
    square-list-pn-eq-squares       \ sqr-lst t | f
;

\ Of the squares in a list with the highest pn ( order is 1, 2, 0),
\ return a list of adjacent, incompatible, square pairs,
\ as regions of square pair states, if any.
: square-list-find-adj-incompatible-pairs ( sqr-lst0 -- reg-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )
\    cr ." square-list-find-adj-incompatible-pairs: start: " .stack cr

    \ Check list length.
    dup list-get-length                     \ sqr-lst0 len
    #2 < if
        \ No pairs to check.
        drop
        false
        \ cr ." square-list-find-adj-incompatible-pairs: exit 1" cr
        exit
    then

    \ Get base pn.
    dup square-list-max-pn-value            \ sqr-lst, mpn t | f
    invert abort" unexpected"
    swap                                    \ mpn sqr-lst0

    \ Init the incompatible pair list.
    list-new
    \ cr ." list-new 3: " dup hex. cr
    swap                                    \ mpn inc-lst sqr-lst0

    \ Check every possible pair.
    \ For each pair, at least one must have the base pn, if so compare them.
    foreach                                 \ mpn inc-lst sqr-lnk sqr1
        \ Check if loop 1 square pn is equal to the base pn.
        square-get-pn                       \ mpn inc-lst sqr-lnk pn1
        #3 pick                             \ mpn inc-lst sqr-lnk pn1 mpn
        =                                   \ mpn inc-lst sqr-lnk bool1

        \ Check next squares.
        over link-get-next                  \ mpn inc-lst sqr-lnk bool1 nxt-lnk
        begin
            ?dup
        while
            \ Check if loop 2 square pn is equal to the base pn.
            dup link-get-data               \ mpn inc-lst sqr-lnk bool1 nxt-lnk sqr2
            square-get-pn                   \ mpn inc-lst sqr-lnk bool1 nxt-lnk pn2
            #5 pick                         \ mpn inc-lst sqr-lnk bool1 nxt-lnk pn2 mpn
            =                               \ mpn inc-lst sqr-lnk bool1 nxt-lnk bool2

            \ Check that at least one square of the pair has a pn equal to the base pn.
            #2 pick                         \ mpn inc-lst sqr-lnk bool1 nxt-lnk bool2 bool1
            or                              \ mpn inc-lst sqr-lnk bool1 nxt-lnk bool12

            if
                \ Check the pair.
                #2 pick link-get-data       \ mpn inc-lst sqr-lnk bool1 nxt-lnk sqr1
                over link-get-data          \ mpn inc-lst sqr-lnk bool1 nxt-lnk sqr1 sqr2
                squares-compare             \ mpn inc-lst sqr-lnk bool1 nxt-lnk char
                [char] I =
                if
                    \ Get loop1 square.
                    #2 pick                 \ mpn inc-lst sqr-lnk bool1 nxt-lnk sqr-lnk
                    link-get-data           \ mpn inc-lst sqr-lnk bool1 nxt-lnk sqr1
                    square-get-state        \ mpn inc-lst sqr-lnk bool1 nxt-lnk sta1

                    \ Get loop2 square.
                    over                    \ mpn inc-lst sqr-lnk bool1 nxt-lnk sta1 nxt-lnk
                    link-get-data           \ mpn inc-lst sqr-lnk bool1 nxt-lnk sta1 sqr2
                    square-get-state        \ mpn inc-lst sqr-lnk bool1 nxt-lnk sta1 sta2

                    2dup states-adjacent?   \ mpn inc-lst sqr-lnk bool1 nxt-lnk sta1 sta2 bool
                    if
                        \ Save pair.
                        region-new          \ mpn inc-lst sqr-lnk bool1 nxt-lnk regx'
                        #4 pick             \ mpn inc-lst sqr-lnk bool1 nxt-lnk regx' inc-lst
                        region-list-push    \ mpn inc-lst sqr-lnk bool1 nxt-lnk
                    else
                        2drop
                    then
                then
            then
        next-item
                                            \ mpn inc-lst sqr-lnk bool1
        drop                                \ mpn inc-lst sqr-lnk
    next-item
                                            \ mpn inc-lst
    nip                                     \ inc-lst

    check-list-return

    \ cr ." square-list-find-adj-incompatible-pairs: end: " .stack cr
;

\ Return squares in a given region.
: square-list-in-region ( reg1 sqr-lst0 -- sqr-lst t | f )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-region? )
    \ cr ." square-list-in-region: start" cr

    [ ' square-in-region? ] literal -rot            \ xt reg1 sqr-lst0
    list-find-all-struct                            \ ret-list

    check-list-return
    \ cr ." square-list-in-region: end" cr
;

\ Find a square in a list, by state, if any.
: square-list-find ( sta1 list0 -- sqr t | f )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-state? )

    [ ' square-state-eq ] literal -rot list-find
;

\ Return true if a pair of states, in a region, make a non-adjacent, dissimilar, pair,
\ with no incompatible squares between them.
: square-list-nadj-pair-ok? ( reg1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-region? )
    \ cr ." square-list-nadj-pair-ok?: start: " over .region space dup .square-list cr

    2dup square-list-in-region      \ reg1 sqr-lst0, sqrs-in' t | f
    invert abort" no squares found?"

    \ Check if any squares between.
    dup list-get-length #2 =        \ reg1 sqr-lst0 sqrs-in' bool
    if
        square-list-deallocate      \ reg1 sqr-lst0
        2drop
        true
        exit
    then

    \ Get square for region state 0.
    rot                             \ sqr-lst0 sqrs-in' reg1
    dup region-get-state-0          \ sqr-lst0 sqrs-in' reg1 sta-0
    #2 pick                         \ sqr-lst0 sqrs-in' reg1 sta-0 sqrs-in'
    square-list-find                \ sqr-lst0 sqrs-in' reg1 sqr-0 t | f
    invert abort" square not found?"

    \ Get square for region state 1.
    swap region-get-state-1         \ sqr-lst0 sqrs-in' sqr-0 sta-1
    #2 pick                         \ sqr-lst0 sqrs-in' sqr-0 sta-1 sqrs-in'
    square-list-find                \ sqr-lst0 sqrs-in' sqr-0, sqr-1 t | f
    invert abort" square not found?"

    \ Check if any square, ne sqr-0, ne sqr-1, is incompatible with sqr-0 or sqr-1.
    #2 pick                         \ sqr-lst0 sqrs-in' sqr-0 sqr-1 sqrs-in'

    foreach                         \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx
        \ Check if sqrx is equal to sqr-0 or sqr-1.
        #3 pick over                \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx sqr-0 sqrx
        =                           \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool0
        #3 pick #2 pick             \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool0 sqr-1 sqrx
        =                           \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool0 bool1
        or                          \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool
        if
            drop                    \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk
        else
            \ sqrx could be incompatible with sqr-0, sqr-1, or both.

            \ Check if sqrx if incompatible with sqr-0.
            #3 pick over            \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx sqr-0 sqrx
            squares-compare         \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx char
            [char] I =              \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool
            if
                2drop 2drop             \ sqr-lst0 sqrs-in'
                square-list-deallocate  \ sqr-lst0
                drop
                false
                exit
            then

            \ Check if sqrx if incompatible with sqr-1.
            #2 pick over            \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx sqr-1 sqrx
            squares-compare         \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx char
            [char] I =              \ sqr-lst0 sqrs-in' sqr-0 sqr-1 in-lnk sqrx bool
            if
                2drop 2drop             \ sqr-lst0 sqrs-in'
                square-list-deallocate  \ sqr-lst0
                drop
                false
                exit
            then
        then
    next-item
                                    \ sqr-lst0 sqrs-in' sqr-0 sqr-1
    2drop                           \ sqr-lst0 sqrs-in'
    square-list-deallocate          \ sqr-lst0
    drop
    true
;

\ Of the squares in a list with the highest pn ( order is 1, 2, 0),
\ return a list of non-adjacent, incompatible, non-superset, square pairs,
\ as regions of square pair states, if any.
: square-list-find-nadj-incompatible-pairs ( sqr-lst0 -- reg-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )
    \ cr ." square-list-find-nadj-incompatible-pairs: start: " .stack cr

    \ Check list length.
    dup list-get-length                         \ sqr-lst0 len
    #2 < if
        \ No pairs to check.
        drop
        false
        \ cr ." square-list-find-nadj-incompatible-pairs: exit 1" cr
        exit
    then

    \ Get base pn.
    dup square-list-max-pn-value                \ sqr-lst0, mpn t | f
    invert abort" unexpected"
    swap                                        \ mpn sqr-lst0

    \ Init the incompatible pair list.
    list-new
    swap dup                                    \ mpn inc-lst sqr-lst0 sqr-lst0

    \ Check every possible pair.
    \ For each pair, at least one must have the base pn, if so compare them.
    foreach                                     \ mpn inc-lst sqr-lst0 sqr-lnk sqr1
        \ Check if loop 1 square pn is equal to the base pn.
        square-get-pn                           \ mpn inc-lst sqr-lst0 sqr-lnk pn1
        #3 pick                                 \ mpn inc-lst sqr-lst0 sqr-lnk pn1 mpn
        =                                       \ mpn inc-lst sqr-lst0 sqr-lnk bool1

        \ Check next squares.
        over link-get-next                      \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk
        begin
            ?dup
        while
            \ Check if loop 2 square pn is equal to the base pn.
            dup link-get-data                   \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sqr2
            square-get-pn                       \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk pn2
            #6 pick                             \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk pn2 mpn
            =                                   \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk bool2

            \ Check that at least one square of the pair has a pn equal to the base pn.
            #2 pick                             \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk bool2 bool1
            or                                  \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk bool12
            if
                \ Check the pair.
                #2 pick link-get-data           \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sqr1
                over link-get-data              \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sqr1 sqr2
                squares-compare                 \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk char
                [char] I =
                if
                    \ Get loop1 square.
                    #2 pick                     \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sqr-lnk
                    link-get-data               \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sqr1
                    square-get-state            \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sta1

                    \ Get loop2 square.
                    over                        \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sta1 nxt-lnk
                    link-get-data               \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sta1 sqr2
                    square-get-state            \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sta1 sta2

                    2dup states-adjacent?       \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk sta1 sta2 bool
                    if
                        2drop
                    else
                        \ Non-adjacent, incompatible pair found.

                        \ Find squares in-between, if any.
                        region-new dup          \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' regx'
                        #5 pick                 \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' regx' sqr-lst0
                        square-list-nadj-pair-ok?
                                                \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' bool

                        if
                            \ Save pair.
                            dup                     \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' regx'
                            #6 pick                 \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' regx' inc-lst
                            region-list-push-nosups \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk regx' bool
                            if
                                drop
                            else
                                region-deallocate
                            then
                        else
                            region-deallocate       \ mpn inc-lst sqr-lst0 sqr-lnk bool1 nxt-lnk
                        then
                    then
                then
            then

            link-get-next
        repeat
                                                \ mpn inc-lst sqr-lst0 sqr-lnk bool1
        drop                                    \ mpn inc-lst sqr-lst0 sqr-lnk
    next-item
                                                \ mpn inc-lst sqr-lst0
    drop nip                                    \ inc-lst

    check-list-return
    \ cr ." square-list-find-nadj-incompatible-pairs: end: " .stack cr
;

\ Return a region built from squares of the highest pn value, in a list.
: square-list-region ( sqr-lst0 -- reg t | f )
    \ Check arg.
    assert( tos is-square-list? )

    \ Get highest pn value
    dup square-list-max-pn-value            \ sqr-lst0, mpn t | f
    ifnot drop false exit then

    swap                                    \ mpn sqr-lst0

    \ Set return region to null.
    0 swap                                  \ mpn reg sqr-lst0

    foreach                                 \ mpn reg sqr-lnk sqrx
        \ Check if square pn is equal to the max pn of the list.
        dup square-get-pn                   \ mpn reg sqr-lnk sqrx s-pn
        #4 pick                             \ mpn reg sqr-lnk sqrx s-pn mpn
        = if
            square-get-state                \ mpn reg sqr-lnk stax
            rot                             \ mpn sqr-lnk stax reg
            dup 0=
            if
                                            \ mpn sqr-lnk stax 0
                drop                        \ mpn sqr-lnk stax
                dup                         \ mpn sqr-lnk stax stax
                region-new                  \ mpn sqr-lnk reg
                swap                        \ mpn reg sqr-lnk
            else
                                            \ mpn sqr-lnk stax reg
                2dup                        \ mpn sqr-lnk stax reg stax reg
                region-superset-of-state?   \ mpn sqr-lnk stax reg flag
                if
                    nip swap                \ mpn reg sqr-lnk
                else
                    \ Add state to expand return region.
                    tuck                    \ mpn sqr-lnk reg stax reg
                    region-union-state      \ mpn sqr-lnk reg reg2
                    swap region-deallocate  \ mpn sqr-lnk reg2
                    swap                    \ mpn reg2 sqr-lnk
                then
            then
        else
            drop
        then
    next-item
                                            \ mpn reg
    nip                                     \ reg
    true
;

\ Return the first square matching a given pn value.
: square-list-first-pn-eq ( pn sqr-lst0 -- sqr t | f )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-pn? )

    foreach                 \ pn sqr-lnk sqrx
        square-get-pn       \ pn sqr-lnk s-pn
        #2 pick             \ pn sqr-lnk s-pn pn
        = if
            link-get-data   \ pn sqrx
            nip             \ sqrx
            true
            exit
        then
    next-item
                            \ pn
    drop
    false
;

\ Return true if any incompatible square pair is found.
\ At least one of the pair must be a base-pn square.
\ Base-pn: 0 if any pn-0 squares, else 2 if any pn-2 squares, else 1.
: square-list-all-compatible? ( sqr-lst0 -- bool )
    \ Check arg.
    assert( tos is-square-list? )
    \ cr ." square-list-all-compatible?: start: " .stack cr

    \ Check list length.
    dup list-get-length                 \ sqr-lst0 len
    #2 < if
        \ No pairs to check.
        drop
        true
        \ cr ." square-list-all-compatible?: exit 1 " .stack cr
        exit
    then

    \ Get base pn.
    dup square-list-max-pn-value        \ sqr-lst0, mpn t | f
    invert abort" unexpected"
    swap                                \ mpn sqr-lst0

    \ Check every possible pair, when at least one has the base pn, compare them.
    foreach                             \ mpn sqr-lnk sqr1
        \ Check if loop 1 square pn is equal to the base pn.
        square-get-pn                   \ mpn sqr-lnk pn1
        #2 pick                         \ mpn sqr-lnk pn1 mpn
        =                               \ mpn sqr-lnk bool1

        \ Check next squares.
        over link-get-next              \ mpn sqr-lnk bool1 nxt-lnk
        begin
            ?dup
        while
            \ Check if loop 2 square pn is equal to the base pn.
            dup link-get-data           \ mpn sqr-lnk bool1 nxt-lnk sqr2
            square-get-pn               \ mpn sqr-lnk bool1 nxt-lnk pn2
            #4 pick                     \ mpn sqr-lnk bool1 nxt-lnk pn2 mpn
            =                           \ mpn sqr-lnk bool1 nxt-lnk bool2

            \ Check that at least one square of the pair has a pn equal to the base pn.
            #2 pick                     \ mpn sqr-lnk bool1 nxt-lnk bool2 bool1
            or                          \ mpn sqr-lnk bool1 nxt-lnk bool12
            if
                \ Check the pair.
                #2 pick link-get-data   \ mpn sqr-lnk bool1 nxt-lnk sqr1
                over link-get-data      \ mpn sqr-lnk bool1 nxt-lnk sqr1 sqr2
                \ cr ." comparing: " over .square space ." and: " dup .square cr
                squares-compare         \ mpn sqr-lnk bool1 nxt-lnk char
                [char] I =
                if
                   2drop 2drop
                   false
                   \ cr ." square-list-all-compatible?: exit 2 " .stack cr
                   exit
                then
            then

            link-get-next
        repeat
                                        \ mpn sqr-lnk bool1
        drop                            \ mpn sqr-lnk
    next-item
                                        \ mpn
    drop
    true
    \ cr ." square-list-all-compatible?: exit 3 " .stack cr
;

\ Return rules for a square-list.
: square-list-calc-rules ( sqr-lst0 -- rul-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )
    \ cr ." square-list-get-rules: start: " .stack cr
    \ cr dup .square-list cr

    \ Check for empty list.
    dup list-is-empty?
    if
        drop
        false
        \ cr ." square-list-calc-rules: exit 1: " .stack cr
        exit
    then

    dup square-list-all-compatible?     \ sqr-lst0 bool
    if
    else
        drop
        false
        \ cr ." square-list-calc-rules: exit 2: " .stack cr
        exit
    then

    dup square-list-max-pn-value        \ sqr-lst0, mpn t | f
    invert abort" unexpected"

    \ Check for pn 0 (Unpredictable).
    dup 0=
    if
        2drop                           \
        list-new                        \ rul-lst
        true
        \ cr ." square-list-calc-rules: exit 3: " .stack cr
        exit
    then

    swap                                \ mpn sqr-lst0

    \ Init return rule list, with a base-pn square's rules.
    2dup                                \ mpn sqr-lst0 mpn sqr-lst0
    square-list-first-pn-eq             \ mpn sqr-lst0, sqr0 t | f
    invert abort" square-list-calc-rules: first pn eq failed?"
    square-get-rules                    \ mpn sqr-lst0 rul-lst

    \ Adjust for one deallocate, below.
    list-copy-struct                    \ mpn sqr-lst0 rul-lst
    -rot                                \ rul-lst mpn sqr-lst0

    foreach                             \ rul-lst mpn  sqr-lnk sqr
        \ Check if the current square pn is equal to the max-pn.
        square-get-pn                   \ rul-lst mpn  sqr-lnk sqr-pn
        #2 pick                         \ rul-lst mpn  sqr-lnk sqr-pn mpn
        =                               \ rul-lst mpn  sqr-lnk flag

        if                              \ rul-lst mpn  sqr-lnk
            \ Update the return rule-list.
            rot                         \ mpn  sqr-lnk rul-lst

            over link-get-data          \ mpn  sqr-lnk rul-lst sqr
            square-get-rules            \ mpn  sqr-lnk rul-lst sqr-ruls
            over                        \ mpn  sqr-lnk rul-lst sqr-ruls rul-lst
            \ cr ." about to union: " over .rule-list space dup .rule-list space .stack cr
            rule-list-union             \ mpn  sqr-lnk rul-lst, new-rules t | f
            \ cr ." after to union: " .stack cr
            if                          \ mpn  sqr-lnk rul-lst new-rules
                swap                    \ mpn  sqr-lnk new-rules rul-lst
                rule-list-deallocate    \ mpn  sqr-lnk new-rules
                -rot                    \ rul-lst mpn  sqr-lnk
            else                        \ mpn  sqr-lnk rul-lst
                rule-list-deallocate    \ mpn  sqr-lnk
                2drop
                false
                \ cr ." square-list-calc-rules: exit 4: " .stack cr
                exit
            then
        then
    next-item
                                        \ rul-lst mpn
    drop                                \ rul-lst
    true
    \ cr ." square-list-calc-rules: end: " .stack cr
;

\ Return true if max-pn elements of a square-list are compatible with a given square.
: square-list-square-compatible? ( sqr1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square? )

    \ Find base pn.
    dup square-list-max-pn-value    \ sqr1 sqr-lst0, mpn t | f
    invert abort" square-list is empty?"

    -rot                            \ mpn sqr1 sqr-lst0

    foreach                         \ mpn sqr1 sqr-lnk sqrx
        square-get-pn               \ mpn sqr1 sqr-lnk spn
        #3 pick                     \ mpn sqr1 sqr-lnk spn mpn
        =
        if
            dup link-get-data       \ mpn sqr1 sqr-lnk sqrx
            #2 pick                 \ mpn sqr1 sqr-lnk sqrx sqr1
            squares-compare         \ mpn sqr1 sqr-lnk char
            [char] I =              \ mpn sqr1 sqr-lnk bool
            if
                2drop drop
                false
                exit
            then
        then
    next-item
                                \ mpn sqr1
    2drop
    true
;

\ Return a list of squares that have pnc set on.
: square-list-pnc-squares ( sqr-lst0 -- sqr-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )

    dup list-is-empty?
    if
        drop
        false
        exit
    then

    \ Init return list.
    list-new swap                   \ ret-lst sqr-lst

    foreach                         \ ret-lst  sqr-lnk sqr
        \ Check if square is pnc.
        square-get-pnc              \ ret-lst  sqr-lnk s-pnc
        if
            dup link-get-data       \ ret-lst  sqr-lnk sqr
            #2 pick                 \ ret-lst  sqr-lnk sqr ret-lst
            list-push-struct        \ ret-lst  sqr-lnk
        then
    next-item
                                    \ ret-lst
    check-list-return
;

\ Return a list of squares that have pnc set on.
: square-list-non-pnc-squares ( sqr-lst0 -- sqr-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )

    dup list-is-empty?
    if
        drop
        false
        exit
    then

    \ Init return list.
    list-new swap                   \ ret-lst sqr-lst

    foreach                         \ ret-lst  sqr-lnk sqr
        \ Check if square is pnc.
        square-get-pnc              \ ret-lst  sqr-lnk s-pnc
        ifnot
            dup link-get-data       \ ret-lst  sqr-lnk sqr
            #2 pick                 \ ret-lst  sqr-lnk sqr ret-lst
            list-push-struct        \ ret-lst  sqr-lnk
        then
    next-item
                                    \ ret-lst
    check-list-return
;

\ Return a list of squares with the maximum number of samples.
: square-list-max-samples ( sqr-lst0 -- sqr-lst t | f )
    \ Check arg.
    assert( tos is-square-list? )

    dup list-is-empty?
    if
        drop
        false
        exit
    then

    \ Find the maximum number of samples.
    0                                   \ sqr-lst0 max
    over                                \ sqr-lst0 max sqr-lst0

    foreach                             \ sqr-lst0 max sqr-lnk sqrx
        square-get-num-samples          \ sqr-lst0 max sqr-lnk num
        rot                             \ sqr-lst0 sqr-lnk num max
        max                             \ sqr-lst0 sqr-lnk max
        swap                            \ sqr-lst0 max sqr-lnk
    next-item
                                        \ sqr-lst0 max
    \ Init return list.
    list-new -rot                       \ ret-lst sqr-lst0 max

    swap                                \ ret-lst max sqr-lst0

    \ Collect squares with max samples.
    foreach                             \ ret-lst max sqr-lnk sqrx
        dup square-get-num-samples      \ ret-lst max sqr-lnk sqrx num
        #3 pick                         \ ret-lst max sqr-lnk sqrx num max
        =                               \ ret-lst max sqr-lnk sqrx bool
        if
            \ Save square.
            #3 pick                     \ ret-lst max sqr-lnk sqrx ret-lst
            list-push-struct            \ ret-lst max sqr-lnk
        else
            drop                        \ ret-lst max sqr-lnk
        then
    next-item
                                        \ ret-lst max
    drop                                \ ret-lst

    check-list-return
;


\ Return true if a square is a member of a square-list.
\ Comparing the square states might be more correct,
\ but address equality should work.
: square-list-member? ( sqr1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square? )

    [ ' = ] literal -rot    \ xt sqr1 sqr-lst0
    list-member?
;

\ Return a list of states that have corresponding squares
\ in the square list.
: square-list-states-in ( sta-lst1 sqr-lst0 -- sta-lst )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-state-list? )

    \ Init return list.
    list-new -rot               \ ret-lst sta-lst1 sqr-lst0

    \ Foreach state in the state list ...
    swap                        \ ret-lst sqr-lst0 sta-lst1
    foreach                     \ ret-lst sqr-lst0 sta-lnk sta
        #2 pick                 \ ret-lst sqr-lst0 sta-lnk sta sqr-lst0
        square-list-find        \ ret-lst sqr-lst0 sta-lnk, sqr t | f
        if
            square-get-state    \ ret-lst sqr-lst0 sta-lnk sta
            #3 pick             \ ret-lst sqr-lst0 sta-lnk sta ret-lst
            list-push-struct    \ ret-lst sqr-lst0 sta-lnk
        then
    next-item
                                \ ret-lst sqr-lst0
    drop
;

\ Remove squares matching a given list of states.
\ Return the number of squares removed.
: square-list-remove-matching-squares ( sta-lst1 sqr-lst0 -- num )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-state-list? )

    \ Init counter.
    0                                   \ sta-lst1 sqr-lst0 cnt
    rot                                 \ sqr-lst0 cnt sta-lst1

    foreach                             \ sqr-lst0 cnt sta-lnk stax
        [ ' square-state-eq? ] literal  \ sqr-lst0 cnt sta-lnk stax xt
        swap                            \ sqr-lst0 cnt sta-lnk xt stax
        #4 pick                         \ sqr-lst0 cnt sta-lnk xt stax sqr-lst0
        list-remove                     \ sqr-lst0 cnt sta-lnk, sqr t | f
        if
            square-deallocate           \ sqr-lst0 cnt sta-lnk
            \ Inc counter.
            swap 1+ swap
        then
    next-item
                                        \ sqr-lst0 cnt
    nip
;

\ Return a list of states used to define squares in a square-list.
: square-list-states ( sqr-lst0 -- sta-lst )
    \ Check arg.
    assert( tos is-square-list? )

    \ Init return list.
    list-new swap                   \ sta-lst sqr-lst0

    foreach                         \ sta-lst sqr-lnk sqrx
        \ Check square state.
        [ ' states-eq? ] literal    \ sta-lst sqr-lnk sqrx xt
        over square-get-state       \ sta-lst sqr-lnk sqrx xt sta0
        #4 pick                     \ sta-lst sqr-lnk sqrx xt sta0 sta-lst
        list-member?                \ sta-lst sqr-lnk sqrx bool
        ifnot
            dup square-get-state    \ sta-lst sqr-lnk sqrx sta0
            #3 pick                 \ sta-lst sqr-lnk sqrx sta0 sta-lst
            list-push-struct        \ sta-lst sqr-lnk sqrx
        then
        drop
    next-item
;

\ Return true if a square list contains a square that is
\ incompatible to a given square.
: square-list-any-incompatible? ( sqr1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square? )

    foreach                 \ sqr1 sqr-lnk0 sqr0x
        #2 pick             \ sqr1 sqr-lnk0 sqr0x sqr1
        squares-compare     \ sqr1 sqr-lnk0 char
        [char] I =          \ sqr1 sqr-lnk0 bool
        if
            2drop
            true
            exit
        then
    next-item
                            \ sqr1
    drop
    false
;

\ Return true if there are any incompatible square pairs
\ between two square lists.
: square-lists-any-incompatible-pairs? ( sqr-lst1 sqr-lst0 -- bool )
    \ Check args.
    assert( tos is-square-list? )
    assert( nos is-square-list? )

    foreach                             \ sqr-lst1 sqr-lnk0 sqr0x
        #2 pick                         \ sqr-lst1 sqr-lnk0 sqr0x sqr-lst1
        square-list-any-incompatible?   \ sqr-lst1 sqr-lnk0 bool
        if
            2drop
            true
            exit
        then
    next-item
                                        \ sqr-lst1
    drop
    false
;

