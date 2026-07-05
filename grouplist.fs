\ Functions for group lists.

\ Check if tos is an empty list, or has a group instance as its first item.
: assert-tos-is-group-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-group
        drop
    then
;

\ Check if nos is an empty list, or has a group instance as its first item.
: assert-nos-is-group-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-group
        drop
    then
;

\ Deallocate a group list.
: group-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-group-list

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
    assert-tos-is-group-list

    list-get-links          \ grp-lnk

    begin
        ?dup
    while
        dup link-get-data   \ grp-lnk grpx
        cr #8 spaces .group

        link-get-next
    repeat
;

: .group-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert-tos-is-group-list
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0
    list-get-links      \ u lnk

    begin
        ?dup
    while
        dup link-get-data .group

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
    assert-tos-is-group-list
    assert-nos-is-state

    \ Init return list.
    list-new swap                   \ sta1 ret-lst grp-lst0

    \ Prep for loop.
    list-get-links                  \ sta1 ret-lst grp-lnk

    begin
        ?dup
    while
        #2 pick                     \ sta1 ret-lst grp-lnk sta1
        over link-get-data          \ sta1 ret-lst grp-lnk sta1 grpx
        group-get-region            \ sta1 ret-lst grp-lnk sta1 grp-reg
        region-superset-of-state?   \ sta1 ret-lst grp-lnk bool
        if
            dup link-get-data       \ sta1 ret-lst grp-lnk grpx
            #2 pick                 \ sta1 ret-lst grp-lnk grpx ret-lst
            list-push-struct        \ sta1 ret-lst grp-lnk
        then

        link-get-next
    repeat
                                    \ sta1 ret-lst
    \ Clean up.
    nip                             \ ret-lst

    \ Return.
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
;

\ Return a list of groups a square should be in.
: group-list-superset-of-square ( sqr1 grp-lst0 -- grp-lst t | f )
    \ Check args.
    assert-tos-is-group-list
    assert-nos-is-square

    swap square-get-state swap  \ sta grp-lst0
    group-list-superset-of-state
;

\ Print a list of group regions.
: .group-list-regions ( grp-lst0 -- )
    \ Check args.
    assert-tos-is-group-list

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

\ Return a list of invalidated groups.
: group-list-get-invalidated-groups ( grp-lst0 -- grp-lst t | f )
    \ Check arg.
    assert-tos-is-group-list

    \ Init return list.
    list-new swap               \ ret-lst grp-lst0

    \ Prep for loop.
    list-get-links              \ ret-lst grp-lnk

    \ Check each group.
    begin
        ?dup
    while
        dup link-get-data       \ ret-lst grp-lnk grpx
        group-get-valid         \ ret-lst grp-lnk bool
        if
        else
            dup link-get-data   \ ret-lst grp-lnk grpx
            #2 pick             \ ret-lst grp-lnk grpx ret-lst
            list-push-struct    \ ret-lst grp-lnk
        then

        link-get-next
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

\ Find a group in a list, by region, if any.
: group-list-member? ( reg1 list0 -- bool )
    \ Check args.
    assert-tos-is-group-list
    assert-nos-is-region

    [ ' group-region-eq? ] literal -rot list-member?
;

: group-list-regions ( grp-lst0 -- reg-lst )
    \ Check arg.
    assert-tos-is-group-list

    \ Init return list.
    list-new swap           \ ret-lst grp-lst0
    list-get-links          \ ret-lst grp-lnk

    begin
        ?dup
    while
        dup link-get-data   \ ret-lst grp-lnk grpx
        group-get-region    \ ret-lst grp-lnk grp-reg
        #2 pick             \ ret-lst grp-lnk grp-reg ret-lst
        list-push-struct    \ ret-lst grp-lnk

        link-get-next
    repeat
;
