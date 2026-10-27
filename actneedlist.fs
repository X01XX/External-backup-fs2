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

\ Print a actneed list one line at a time, aligned with a given prefix.
: .actneed-list-prefix ( c-addr u stac-lst0 -- )
    \ Check arg.
    assert( tos is-actneed-list? )
    cr
    rot                 \ u stac-lst0 c-addr
    #2 pick             \ u stac-lst0 c-addr u
    type                \ u stac-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk grpx
        .actneed

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
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

\ Return true if an actneed in a list has a given type and target.
: actneed-list-contains ( sta2 typ1 lst0 -- bool )
    \ Check args.
    assert( tos is-actneed-list? )
    assert( nos is-actneed-type? )
    assert( 3os is-state? )

    list-get-links          \ sta2 typ1 link
    begin
        ?dup
    while
        dup link-get-data   \ sta2 typ1 link nedx
        actneed-get-type    \ sta1 typ1 link n-typ
        #2 pick             \ sta1 typ1 link n-typ typ1
        =                   \ sta1 typ1 link bool
        if
            dup link-get-data   \ sta2 typ1 link nedx
            actneed-get-target  \ sta2 typ1 link n-sta
            #3 pick             \ sta2 typ1 link n-sta sta2
            states-eq?          \ sta2 typ1 link bool
            if
                2drop drop
                true
                exit
            then
        then

        link-get-next
    repeat
                            \ sta2 typ1
    2drop                   \
    false
;
