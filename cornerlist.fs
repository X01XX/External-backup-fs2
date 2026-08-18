\ Functions for corner lists.

\ Check TOS for corner-list.
: is-corner-list? ( tos -- bool )
    \ cr ." is-corner-list?: start: " .stack-gbl cr

    tos is-list?        \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?  \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links      \ link
    link-get-data       \ data
    is-corner?          \ bool
;

\ Return true if tos is a corner list of lists.
: is-corner-lol? ( tos --  bool )
    \ cr ." is-corner-lol?: start: " .stack-gbl cr
    dup is-list?
    if
        dup list-is-empty?
        if
            drop
            true
        else
            list-get-links link-get-data
            is-corner-list?
        then
    else
        drop
        false
    then
;

\ Deallocate a corner list.
: corner-list-deallocate ( crn-lst0 -- )
    \ Check arg.
    assert( tos is-corner-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate corner instances in the list.
        [ ' corner-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a corner list.
: .corner-list ( crn-lst0 -- )
    \ Check arg.
    assert( tos is-corner-list? )
    ." ("
    foreach                 \ crn-lnk crnx
        .corner

        link-get-next       \ crn-lnk
        dup 0<> if space then
    repeat
    ." )"
;

: .corner-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-corner-list? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk crnx
        .corner

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
;

: .corner-clusters-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-corner-lol? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk crn-lst
        .corner-list

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
;

\ Return true if a corner's region matches a given region.
: corner-region-eq? ( reg1 crn0 -- bool )
    \ Check arg.
    assert( tos is-corner? )
    assert( nos is-region? )

    corner-get-region       \ reg1 crn-reg
    regions-eq?
;

\ Remove all corners that match a given region.
\ If use count becomes zero, deallocate it.
: corner-list-remove-all-region-match ( reg1 crn-lst0 -- )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-region? )
    \ cr ." corner-list-remove-all-region-match: start: " .stack-gbl cr

    begin
        [ ' corner-region-eq? ] literal     \ reg1 crn-lst0 xt
        #2 pick #2 pick                     \ reg1 crn-lst0 xt reg1 crn-lst0
        list-remove-struct                  \ reg1 crn-lst0, crnx t | f

        if
            dup struct-get-use-count        \ reg1 crn-lst0 crnx uc
            0= if
                corner-deallocate           \ reg1 crn-lst0
            else
                drop                        \ reg1 crn-lst0
            then
            false
        else
            true
        then
    until

    2drop
    \ cr ." corner-list-remove-all-region-match: end: " .stack-gbl cr
;

\ Find a corner in a list, by state, if any.
: corner-list-find ( sta1 crn-lst0 -- crn t | f )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-state? )

    [ ' corner-anchor-eq-state? ] literal -rot list-find
;

' corner-list-find to corner-list-find-xt

\ Find a corner in a list, by region, if any.
: corner-list-find-region ( reg1 crn-lst0 -- crn t | f )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-region? )

    [ ' corner-region-eq? ] literal -rot list-find
;

' corner-list-find-region to corner-list-find-region-xt

\ Deallocate a list of corner-list lol.
: corner-lol-deallocate ( crn-lol0 -- )
    \ Check arg.
    assert( tos is-corner-lol? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ crn-lol0 uc
    #2 < if
        \ Deallocate corner-list instances in the list.
        [ ' corner-list-deallocate ] literal over   \ crn-lol0 xt crn-lol0
        list-apply                                  \ crn-lol0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Return true if any corner in a corner list uses a state.
: corner-list-uses-state? ( sta1 crn-lst0 -- bool )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-state? )

    foreach                 \ sta1 crn-lnk crnx
        #2 pick swap        \ sta1 crn-lnk sta1 crnx
        corner-uses-state?  \ sta1 crn-lnk bool
        if
            2drop
            true
            exit
        then
    next

    drop
    false
;

\ Return true if each corner state is used in at least one
\ corner in a corner-list.
: corner-list-all-corner-states-in? ( crn1 crn-lst0 -- bool )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-corner? )

    \ Check anchor state.
    over corner-get-anchor-state        \ crn1 crn-lst0 anc-sta
    over corner-list-uses-state?        \ crn1 crn-lst0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Check each adjacent state.
    swap corner-get-adjacent-states     \ crn-lst0 sta-lst
    foreach                             \ crn-lst0 sta-lnk stax
        #2 pick                         \ crn-lst0 sta-lnk stax crn-lst0
        corner-list-uses-state?         \ crn-lst0 sta-lnk bool
        ifnot
            2drop
            false
            exit
        then
    next
                                        \ crn-lst0
    drop
    true
;

\ Return true if any corner state is used in at least one
\ corner in a corner-list.
: corner-list-any-corner-states-in? ( crn1 crn-lst0 -- bool )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-corner? )

    \ Check anchor state.
    over corner-get-anchor-state        \ crn1 crn-lst0 anc-sta
    over corner-list-uses-state?        \ crn1 crn-lst0 bool
    if
        2drop
        true
        exit
    then

    \ Check each adjacent state.
    swap corner-get-adjacent-states     \ crn-lst0 sta-lst
    foreach                             \ crn-lst0 sta-lnk stax
        #2 pick                         \ crn-lst0 sta-lnk stax crn-lst0
        corner-list-uses-state?         \ crn-lst0 sta-lnk bool
        if
            2drop
            true
            exit
        then
    next
                                        \ crn-lst0
    drop
    false
;

\ Remove a corner from a list.
\ If use count becomes zero, deallocate it.
: corner-list-remove ( crn1 crn-lst0 -- )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-corner? )

    [ ' = ] literal -rot            \ xt crn1 crn-lst0
    list-remove-struct              \ crn t | f
    if
        dup struct-get-use-count    \ crn uc
        0=
        if
            corner-deallocate
        else
            drop
        then
    then
;

\ Remove corners with a given region.
: corner-list-remove-by-region ( reg1 crn-lst0 -- )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-region? )

    \ Init delete list.
    list-new                    \ reg1 crn-lst0 del-lst'

    \ Collect corners to delete.
    over                        \ reg1 crn-lst0 del-lst' crn-lst0
    foreach                     \ reg1 crn-lst0 del-lst' crn-lnk0 crnx
        corner-get-region       \ reg1 crn-lst0 del-lst' crn-lnk0 regx
        #4 pick                 \ reg1 crn-lst0 del-lst' crn-lnk0 regx reg1
        regions-eq?             \ reg1 crn-lst0 del-lst' crn-lnk0 bool
        if
            dup link-get-data   \ reg1 crn-lst0 del-lst' crn-lnk0 crnx
            #2 pick             \ reg1 crn-lst0 del-lst' crn-lnk0 crnx del-lst'
            list-push-struct    \ reg1 crn-lst0 del-lst' crn-lnk0
        then
    next

    \ Delete corners.
    dup                         \ reg1 crn-lst0 del-lst' del-lst'
    foreach                     \ reg1 crn-lst0 del-lst' del-lnk crnx
        #3 pick                 \ reg1 crn-lst0 del-lst' del-lnk crnx crn-lst0
        corner-list-remove      \ reg1 crn-lst0 del-lst' del-lnk
    next

    corner-list-deallocate      \ reg1 crn-lst0
    2drop
;

\ Calc and set rate for of corners in a corner list.
: corner-list-calc-set-rate ( pos-lst1 crn-lst0 -- )
    \ Check args.
    assert( tos is-corner-list? )
    assert( nos is-region-list? )

    foreach                     \ pos-lst1 crn-lnk crnx
        #2 pick swap            \ pos-lst1 crn-lnk pos-lst1 crnx
        corner-calc-set-rate    \ pos-lst1 crn-lnk
    next
    drop
;

: .corner-cluster-list ( clstr-lst0 -- )
    \ Check arg.
    assert( tos is-list? )

    foreach
        space .corner-list
    next
;
