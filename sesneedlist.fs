\ Functions for sesneed lists.

\ Check TOS for sesneed-list.
: is-sesneed-list? ( tos -- t )
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
    is-sesneed?             \ bool
;

\ Deallocate a sesneed list.
: sesneed-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-sesneed-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate sesneed instances in the list.
        [ ' sesneed-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a sesneed-list
: .sesneed-list ( sned-lst0 -- )
    \ Check args.
    assert( tos is-sesneed-list? )

    \ Init counter
    0 swap                  \ cnt sned-lst0

    \ Scan sesneeds
    list-get-links          \ cnt sned-lst0
    begin
        ?dup
    while
        dup link-get-data   \ cnt sned-lnk nedx

        \ Print sesneed and count.
        cr
        #2 pick #3 dec.r
        space
        .sesneed

        \ Update count.
        swap 1+ swap

        link-get-next       \ cnt sned-lnk
    repeat
    cr
                            \ cnt
    drop
;

\ Print a sesneed list one line at a time, aligned with a given prefix.
: .sesneed-list-prefix ( c-addr u sesned-lst0 -- )
    \ Check arg.
    assert( tos is-sesneed-list? )
    cr
    rot                 \ u regc-lst0 c-addr
    #2 pick             \ u regc-lst0 c-addr u
    type                \ u regc-lst0

    dup list-is-empty?
    if
        ." None" cr
        2drop
        exit
    then

    dup list-get-length         \ u ned-lst len
    swap list-get-links swap    \ u regc-lnk len
    0                           \ u regc-lnk len 0

    do                          \ u reg-lnk
        \ Print counter to allow picking a specific need by number.
        i #3 dec.r space

        \ Print need.
        dup link-get-data
        .sesneed cr

        link-get-next
        dup 0<>
        if
            \ Print spaces equal to the prefix.
            over spaces
        then
    loop
                                \ u 0
    2drop
    cr
;
