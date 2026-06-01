\ Functions for region lists.

\ Check if tos is an empty list, or has a region instance as its first item.
: assert-tos-is-region-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if nos is an empty list, or has a region instance as its first item.
: assert-nos-is-region-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if 3os is a list, if non-empty, with the first item being a region.
: assert-3os-is-region-list ( 3os nos tos -- 3os nos tos )
    assert-3os-is-list
    #2 pick list-is-not-empty?
    if
        #2 pick list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if 4os is a list, if non-empty, with the first item being a region.
: assert-4os-is-region-list ( 4os 3os nos tos -- 4os 3os nos tos )
    assert-4os-is-list
    #3 pick list-is-not-empty?
    if
        #3 pick list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Deallocate a region list.
: region-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-region-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' region-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a region-list
: .region-list ( list0 -- )
    \ Check arg.
    assert-tos-is-region-list

    [ ' .region ] literal swap .list
;

\ Push a region to a region-list.
: region-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-struct
;

\ Push a region to the end of a region-list.
: region-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-end-struct
;

\ Return a region-list from a string.
: region-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-allocated-region ] literal over  \ lst xt lst
        list-apply-all-true?                    \ lst bool
        if
            true
        else
            structinfo-list-deallocate-struct-list-xt execute
            false
        then
    else
        false
    then
;

\ Return a region-list from a string, or abort.
: region-list-from-string-a ( c-addr u -- reg-lst )
    region-list-from-string \ reg-list t | f
    invert abort" region-list-from-string failed."
;

\ Return true if two region lists are equal.
: region-lists-eq? ( reg-lst1 reg-lst0 -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ Check lengths.
    over list-get-length            \ reg-lst1 reg-lst0 len1
    over list-get-length            \ reg-lst1 reg-lst0 len1 len0
    <>                              \ reg-lst1 reg-lst0 bool
    if
        2drop
        false
        exit
    then

    \ Check elements.
    list-get-links                  \ reg-lst1 lnk
    begin
        ?dup
    while
        [ ' regions-eq? ] literal   \ reg-lst1 lnk xt
        over link-get-data          \ reg-lst1 lnk xt regx
        #3 pick                     \ reg-lst1 lnk xt regx reg-lst1
        list-member                 \ reg-lst1 lnk bool
        if
        else
            2drop
            false
            exit
        then

        link-get-next
    repeat
    drop
    true
;

\ Remove the first subset region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove-subset ( reg list -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-subset? ] literal        \ reg1 list0  xt
    -rot                                \ xt reg1 list0

    list-remove                         \ reg2 t | f
    if
        region-deallocate
        true
    else
        false
    then
;

\ Push a region onto a list, if there are no supersets in the list.
\ If there are no supersets in the list, delete any subsets and push the region.
\ Return true if the region is added to the list.
: region-list-push-nosubs ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 list0 reg1 list0
    [ ' region-superset? ] literal          \ reg1 list0 reg1 list0 xt
    -rot                                    \ reg1 list0 xt reg1 list0
    list-member                             \ reg1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 list0

    \ Remove all subsets.
    begin
        2dup                                \ reg1 list0 reg1 list0
        region-list-remove-subset           \ reg1 list0 | flag
    while
    repeat

    \ Add region to list.                   \ reg1 list0
    region-list-push
    true
;

\ Return a list of region intersections with a region-list, no subsets.
: region-list-intersections-nosubs ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ list1 list0
    list-get-links                  \ list1 link0
    list-new -rot                   \ ret-list list1 link0
    begin
        ?dup
    while
                                    \ ret-list list1 link0
        dup link-get-data           \ ret-list list1 link0 data0
        #2 pick list-get-links      \ ret-list list1 link0 data0 link1

        begin
            ?dup
        while
            dup link-get-data       \ ret-list list1 link0 data0 link1 data1
            #2 pick                 \ ret-list list1 link0 data0 link1 data1 data0
            region-intersection     \ ret-list list1 link0 data0 link1, reg-int t | f
            if
                                        \ ret-list list1 link0 data0 link1 reg-int
                dup                     \ ret-list list1 link0 data0 link1 reg-int reg-int
                #6 pick                 \ ret-list list1 link0 data0 link1 reg-int reg-int ret-list
                region-list-push-nosubs \ ret-list list1 link0 data0 link1 reg-int flag
                if
                    drop
                else
                    region-deallocate
                then
            then
                                    \ ret-list list1 link0 data0 link1
            link-get-next
        repeat
        drop                        \ ret-list list1 link0

        link-get-next
    repeat
                                    \ ret-list list1
    drop
;

\ Combine two reigion-lists, deleting subsets.
: region-list-union-nosubs ( reg-lst1 reg-lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ Inti return list.
    list-new                \ reg-lst1 reg-lst0 ret-lst

    \ Prep for loop 1.
    swap list-get-links     \ reg-lst1 ret-lst link0

    begin
        ?dup
    while
        dup link-get-data       \ reg-lst1 ret-lst link0 reg0x
        #2 pick                 \ reg-lst1 ret-lst link0 reg0x ret-lst
        region-list-push-nosubs \ reg-lst1 ret-lst link0 bool
        drop

        link-get-next
    repeat
                                \ reg-lst1 ret-lst
    \ Prep for loop 2.
    swap list-get-links         \ ret-lst link1

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst link1 reg1x
        #2 pick                 \ ret-lst link1 reg1x ret-lst
        region-list-push-nosubs \ ret-lst link1 bool
        drop

        link-get-next
    repeat
                                \ ret-lst
;
