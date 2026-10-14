\ Functions for actneed lists.

\ Check TOS for actneed-list.
: is-actneed-list? ( tos -- t )
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
    is-actneed?             \ bool
;

\ Deallocate a actneed list.
: actneed-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-actneed-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate actneed instances in the list.
        [ ' actneed-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a actneed-list
: .actneed-list ( list0 -- )
    \ Check args.
    assert( tos is-actneed-list? )

    \ Init counter
    0 swap                  \ cnt link

    \ Scan actneeds
    list-get-links
    begin
        ?dup
    while
        dup link-get-data   \ cnt link ned

        \ Print actneed and count.
        cr
        #2 pick #3 dec.r
        space
        .actneed

        \ Update count.
        swap 1+ swap

        link-get-next       \ cnt link
    repeat
    cr
                            \ cnt
    drop
;

\ Push a actneed to a actneed-list, unless it is already in the list.
: actneed-list-push ( ned1 list0 -- )
    \ Check args.
    assert( tos is-actneed-list? )
    assert( nos is-actneed? )

    list-push-struct
;

\ Append nos actneed-list to the tos actneed-list.
: actneed-list-append ( lst1 lst0 -- )
    \ Check args.
    assert( tos is-actneed-list? )
    assert( nos is-actneed-list? )

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        #2 pick             \ lst0 link nedx lst0
        actneed-list-push   \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;

\ Return true actneed-list-contains-targetif a actneed in a list has a given target state.
: ?actneed-list-contains-target ( sta1 lst0 -- bool )
    \ Check args.
    assert( tos is-actneed-list? )
    assert( nos is-state? )

    list-get-links          \ sta1 link
    begin
        ?dup
    while
        dup link-get-data   \ sta1 link nedx
        actneed-get-target  \ sta1 link n-sta
        #2 pick             \ sta1 link n-sta sta1
        =                   \ sta1 link
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                            \ sta1
    drop                    \
    false
;

\ Find all actneeds matching a given type.
: actneed-list-find-all-match-type ( typ1 ned-lst0 -- ned-lst t | f )
    \ Check args.
    assert( tos is-actneed-list? )
    assert( nos is-actneed-type? )

    \ Init return list.
    list-new -rot               \ ret-lst typ1 ned-lst0

    \ Prep for loop.
    list-get-links              \ ret-lst typ1 ned-link

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst typ1 ned-link nedx
        actneed-get-type        \ ret-lst typ1 ned-link typx
        #2 pick                 \ ret-lst typ1 ned-link typx typ1
        =                       \ ret-lst typ1 ned-link bool
        if
            dup link-get-data   \ ret-lst typ1 ned-link nedx
            #3 pick             \ ret-lst typ1 ned-link nedx ret-lst
            actneed-list-push   \ ret-lst typ1 ned-link
        then

        link-get-next
    repeat
                                \ ret-lst typ1
    drop                        \ ret-lst

    check-list-return
;
