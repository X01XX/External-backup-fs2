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
