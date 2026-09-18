
: random-pick-test
    s" (() (a b) (c d e))" list-from-string-a

    cr ." list: " dup .struct-list cr

    dup lol-pick-list                   \ lst0 pk-lst
    cr ." pick list: " dup .struct-list cr

    dup list-get-length                 \ lst0 pk-lst len
    pick-list                           \ lst0 pk-lst pk-lst2

    dup list-get-length                 \ lst0 pk-lst pk-lst2 len
    0
    do                                  \ lst0 pk-lst pk-lst2
        dup random-pick                 \ lst0 pk-lst pk-lst2, itm t | f
        invert abort" list empty?"

        \ Pick item from list.
        #2 pick                         \ lst0 pk-lst pk-lst2 itm pk-lst
        list-get-item                   \ lst0 pk-lst pk-lst2 itm2

        \ Get first-level list from original list-of-lists.
        dup list-get-first-item         \ list0 pk-lst pk-lst2 itm first
        #4 pick                         \ list0 pk-lst pk-lst2 itm first list0
        list-get-item                   \ list0 pk-lst pk-lst2 itm itmx
        \ space dup .struct-list

        over list-get-second-item       \ list0 pk-lst pk-lst2 itm itmx itm2
        swap list-get-item              \ list0 pk-lst pk-lst2 itm itmxy
        cr dup .token
        2drop                           \ list0 pk-lst pk-lst2
    loop

    \ Deallocate.
    list-deallocate
    struct-list-deallocate
    struct-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." random-pick-test: Ok"
;

