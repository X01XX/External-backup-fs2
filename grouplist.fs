\ Functions for group lists.

\ Check TOS for group-list.
: is-group-list? ( tos -- bool )
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
    is-group?           \ bool
;

\ Deallocate a group list.
: group-list-deallocate ( grp-lst0 -- )
    \ Check arg.
    assert( tos is-group-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate group instances in the list.
        [ ' group-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a group list.
: .group-list ( grp-lst0 -- )
    \ Check arg.
    assert( tos is-group-list? )

    ." ("
    foreach                 \ grp-lnk grpx
        cr #8 spaces .group
    next-item
    ." )"
;

: .group-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-group-list? )
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

    foreach             \ u lnk grpx
        .group

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

\ Return a list of groups a state should be in.
: group-list-superset-of-state ( sta1 grp-lst0 -- grp-lst t | f )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-state? )

    \ Init return list.
    list-new swap                   \ sta1 ret-lst grp-lst0

    \ Prep for loop.
    foreach                         \ sta1 ret-lst grp-lnk grpx
        #3 pick swap                \ sta1 ret-lst grp-lnk sta1 grpx
        group-get-region            \ sta1 ret-lst grp-lnk sta1 grp-reg
        region-superset-of-state?   \ sta1 ret-lst grp-lnk bool
        if
            dup link-get-data       \ sta1 ret-lst grp-lnk grpx
            #2 pick                 \ sta1 ret-lst grp-lnk grpx ret-lst
            list-push-struct        \ sta1 ret-lst grp-lnk
        then
    next-item
                                    \ sta1 ret-lst
    \ Clean up.
    nip                             \ ret-lst

    check-list-return
;

\ Return a list of groups a square should be in.
: group-list-superset-of-square ( sqr1 grp-lst0 -- grp-lst t | f )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-square? )

    swap square-get-state swap  \ sta grp-lst0
    group-list-superset-of-state
;

\ Print a list of group regions.
: .group-list-regions ( grp-lst0 -- )
    \ Check args.
    assert( tos is-group-list? )

    ." ("
    list-get-links          \ lnk
    begin
        ?dup
    while
        dup link-get-data   \ lnk grp
        group-get-region    \ lnk reg
        .region             \ lnk

        link-get-next       \ lnk
        dup 0<> if space then
    repeat
    ." )"
;

\ Find a group in a list, by region, if any.
: group-list-member? ( reg1 list0 -- bool )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-region? )

    [ ' group-region-eq? ] literal -rot list-member?
;

: group-list-regions ( grp-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-group-list? )

    \ Init return list.
    list-new swap           \ ret-lst grp-lst0

    foreach                 \ ret-lst grp-lnk grpx
        group-get-region    \ ret-lst grp-lnk grp-reg
        #2 pick             \ ret-lst grp-lnk grp-reg ret-lst
        list-push-struct    \ ret-lst grp-lnk
    next-item
;

\ Find a group in a list, by state, if any.
: group-list-find ( reg1 list0 -- grp t | f )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-region? )

    [ ' group-region-eq? ] literal -rot list-find
;

\ Remove group-list-state-in-any?a group from a list.
\ If use count becomes zero, deallocate it.
: group-list-remove ( grp1 grp-lst0 -- )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-group? )

    [ ' = ] literal -rot            \ xt grp1 grp-lst0
    list-remove-struct              \ grp t | f
    if
        dup struct-get-use-count    \ grp uc
        0=
        if
            group-deallocate
        else
            drop
        then
    then
;

\ Return true if a state is in any group s-region.
: group-list-state-in-any-s-region? ( sta1 grp-lst0 -- bool )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-state? )
    \ cr ." group-list-state-in-any-s-region? " over .state space dup .group-list cr

    foreach                         \ sta1 grp-lnk grpx
        #2 pick swap                \ sta1 grp-lnk sta1 grpx
        group-state-in-s-region?    \ sta1 grp-lnk bool
        if
            2drop
            true
            \ cr ." group-list-state-in-any-s-region? true" cr
            exit
        then
    next-item
                                    \ sta1
    drop
    false
    \ cr ." group-list-state-in-any-s-region? false" cr
;

\ Return action needs for groups in a group-list.
\ Caller to set actionid, domain-id.
: group-list-get-needs ( sta1 grp-lst0 -- ned-lst t | f )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-state? )

    \ Init return list.
    list-new -rot               \ ret-lst sta1 grp-lst0

    2dup                                \ ret-lst sta1 grp-lst0 sta1 grp-lst0
    group-list-state-in-any-s-region?   \ ret-lst sta1 grp-lst0 bool
    if
        2drop
    else
        \ cr ." group-list-get-needs: at 1: " .stack cr
        \ Make actneed.
        0                       \ ret-lst sta1 grp-lst0 tkn
        #2 pick                 \ ret-lst sta1 grp-lst0 tkn targ
        actneed-type-snig       \ ret-lst sta1 grp-lst0 tkn targ ned-typ
        0 0                     \ ret-lst sta1 grp-lst0 tkn targ ned-typ act-id dom-id
        actneed-new             \ ret-lst sta1 grp-lst0 actned
        \ cr ." group-list-get-needs: at 2: " .stack cr
        \ Store actneed.
        #3 pick                 \ ret-lst sta1 grp-lst0 actned ret-lst
        list-push-struct        \ ret-lst sta1 grp-lst0
        2drop
    then

    \ cr ." group-list needs found: " .stack cr

    check-list-return
;
