\ Functions for need lists.

\ Check TOS for need-list.
: is-need-list? ( tos -- t )
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
    is-need?                \ bool
;

\ Deallocate a need list.
: need-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-need-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate need instances in the list.
        [ ' need-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a need-list
: .need-list ( list0 -- )
    \ Check args.
    assert( tos is-need-list? )

    \ Init counter
    0 swap                  \ cnt link

    \ Scan needs
    list-get-links
    begin
        ?dup
    while
        dup link-get-data   \ cnt link ned

        \ Print need and count.
        cr
        #2 pick #3 dec.r
        space
        .need

        \ Update count.
        swap 1+ swap

        link-get-next       \ cnt link
    repeat
    cr
                            \ cnt
    drop
;

\ Push a need to a need-list, unless it is already in the list.
: need-list-push ( ned1 list0 -- )
    \ Check args.
    assert( tos is-need-list? )
    assert( nos is-need? )

    list-push-struct
;

\ Append nos need-list to the tos need-list.
: need-list-append ( lst1 lst0 -- )
    \ Check args.
    assert( tos is-need-list? )
    assert( nos is-need-list? )

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        #2 pick             \ lst0 link nedx lst0
        need-list-push      \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;

\ Return true need-list-contains-targetif a need in a list has a given target state.
: ?need-list-contains-target ( sta1 lst0 -- bool )
    \ Check args.
    assert( tos is-need-list? )
    assert( nos is-value? )

    list-get-links          \ sta1 link
    begin
        ?dup
    while
        dup link-get-data   \ sta1 link nedx
        need-get-target     \ sta1 link n-sta
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

\ Find all needs matching a given type.
: need-list-find-all-match-type ( typ1 ned-lst0 -- ned-lst t | f )
    \ Check args.
    assert( tos is-need-list? )
    assert( nos is-need-type? )

    \ Init return list.
    list-new -rot               \ ret-lst typ1 ned-lst0

    \ Prep for loop.
    list-get-links              \ ret-lst typ1 ned-link

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst typ1 ned-link nedx
        need-get-type           \ ret-lst typ1 ned-link typx
        #2 pick                 \ ret-lst typ1 ned-link typx typ1
        =                       \ ret-lst typ1 ned-link bool
        if
            dup link-get-data   \ ret-lst typ1 ned-link nedx
            #3 pick             \ ret-lst typ1 ned-link nedx ret-lst
            need-list-push      \ ret-lst typ1 ned-link
        then

        link-get-next
    repeat
                                \ ret-lst typ1
    drop                        \ ret-lst
    dup list-is-empty?          \ ret-lst bool
    if
        list-deallocate
        false
    else
        true
    then
;
