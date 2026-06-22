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
    [ ' .group-region ] literal swap list-apply
    ." )"
;


