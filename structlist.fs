\ Pop the first struct from a list.
: list-pop-struct ( lst0 -- struct t | f )
    \ Check args.
    assert( tos is-list? )

    list-pop        \ struct t | f
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Pop the last struct from a list.
: list-pop-end-struct ( lst0 -- struct t | f )
    \ Check args.
    assert( tos is-list? )

    list-pop-end    \ struct t | f
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Push a struct to a list.
: list-push-struct ( struct1 list0 -- )
    \ Check args.
    assert( tos is-list? )

    over struct-inc-use-count
    list-push
;

\ Push a struct to the end of a list.
: list-push-end-struct ( struct1 list0 -- )
    \ Check args.
    assert( tos is-list? )

    over struct-inc-use-count
    list-push-end
;

\ Return the union of two lists of structs.
: list-union-struct ( xt list1 list0 -- list-result )
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the intersection of two lists of structs.
: list-intersection-struct ( xt list1 list0 -- list-result )
    list-intersection                   \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the difference of two lists of structs.
: list-difference-struct ( xt list1 list0 -- list-result )
    list-difference                     \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return a list of items that return true for a given xt and data.
: list-find-all-struct ( xt data list0 -- list )
    \ Check args.
    assert( tos is-list? )

    list-find-all                                   \ ret-list
    [ ' struct-inc-use-count ] literal over         \ ret-list xt ret-list
    list-apply                                      \ ret-list
;

\ Return a list of items that return true for a given xt and data.
: list-find-all-struct-recursive ( xt data list0 -- list )
    \ Check args.
    assert( tos is-list? )

    list-find-all-recursive                         \ ret-list
    [ ' struct-inc-use-count ] literal over         \ ret-list xt ret-list
    list-apply-recursive                            \ ret-list
;

\ Return a list that is a copy of a given list, but with a specific item replaced by a given struct item.
: list-copy-except-struct ( new-item2 index1 lst0 -- lst )
     \ Check args.
    assert( tos is-list? )
    over 0< abort" list-copy-except-struct: index negative?"
    over over list-get-length < 0= abort" list-copy-except-struct: index out of range?"

    \ Init return list.
    list-new -rot                   \ new-item2 ret-lst index1 lst0

    list-get-links                  \ new-item2 ret-lst index1 link
    begin
        ?dup
    while                           \ new-item2 ret-lst index1 link
        over 0=
        if
            #3 pick #3 pick         \ new-item2 ret-lst index1 link new-item2 ret-lst
            list-push-end-struct    \ new-item2 ret-lst index1 link
        else
            dup link-get-data       \ new-item2 ret-lst index1 link data
            #3 pick                 \ new-item2 ret-lst index1 link data ret-lst
            list-push-end-struct    \ new-item2 ret-lst index1 link
        then

        \ Dec index.
        swap 1- swap
    next
                                    \ new-item2 ret-lst index1
    drop nip
;

\ Return a copy of a list of structs.
: list-copy-struct ( lst0 -- lst )
    \ Check arg.
    assert( tos is-list? )

    list-copy                           \ ret-lst

    [ ' struct-inc-use-count ] literal  \ ret-lst xt
    over list-apply-recursive           \ ret-lst
;

\ Return a flattened struct list.
: list-flatten-struct ( lst0 -- lst )
    \ Check arg.
    assert( tos is-list? )

    list-flatten                        \ ret-list

    [ ' struct-inc-use-count ] literal  \ ret-lst xt
    over list-apply                     \ ret-lst
;

: list-one-of-each-struct ( lst0 -- lol )
    \ Check arg.
    assert( tos is-list? )

    list-one-of-each                    \ ret-list

    [ ' struct-inc-use-count ] literal  \ ret-lst xt
    over list-apply-recursive           \ ret-lst
;

\ Remove a struct item based on index.
: list-remove-item-struct ( u1 lst0 -- item )
    \ Check arg.
    assert( tos is-list? )

    list-remove-item        \ item
    dup struct-dec-use-count
;

\ Return a copy of a list, except the first item.
: list-copy-after-first-struct ( lst0 -- lst )
    \ Check arg.
    assert( tos is-list? )

    list-copy-after-first               \ ret-list

    [ ' struct-inc-use-count ] literal  \ ret-lst xt
    over list-apply                     \ ret-lst
;

\ Return a list with elements reversed.
: list-reverse-struct ( lst0 -- lst )
    \ Check arg.
    assert( tos is-list? )

    list-reverse                        \ ret-list

    [ ' struct-inc-use-count ] literal  \ ret-lst xt
    over list-apply                     \ ret-lst
;

\ Return true if two struct lists are equal.
: struct-lists-eq? ( xt sct-lst1 sct-lst0 -- bool )
    \ Check args.
    assert( tos is-list? )
    assert( nos is-list? )

    \ Check lengths.
    over list-get-length            \ xt sct-lst1 sct-lst0 len1
    over list-get-length            \ xt sct-lst1 sct-lst0 len1 len0
    <>                              \ xt sct-lst1 sct-lst0 bool
    if
        2drop drop
        false
        exit
    then

    \ Check elements.
    foreach                         \ xt sct-lst1 lnk regx
        #3 pick swap                \ xt sct-lst1 lnk xt regx

        #3 pick                     \ xt sct-lst1 lnk xt regx sct-lst1
        list-member?                \ xt sct-lst1 lnk bool
        if
        else
            2drop drop
            false
            exit
        then
    next
                                    \ xt sct-lst1
    2drop
    true
;

\ Deallocate a list of structs, given
\ a <struct>-deallocate xt.
: list-deallocate-recursive-struct ( xt1 lst0 -- )
    \ Check arg.
    assert( tos is-list? )

    dup struct-get-use-count        \ xt1 lst0 uc
    #2 <                            \ xt1 lst0 bool
    if
        dup                         \ xt1 lst0 lst0
        foreach                     \ xt1 lst0 lnk item
            dup is-list?
            if
                #3 pick swap        \ xt1 lst0 lnk xt1 item
                recurse             \ xt1 lst0 lnk
            else
                dup is-struct?-xt execute
                if
                    #3 pick             \ xt1 lst0 lnk item xt1
                    execute             \ xt1 lst0 lnk
                else
                    drop
                then
            then
        next
        list-deallocate             \ xt1 lst0
        drop
    else
        struct-dec-use-count        \ xt1
        drop
    then
    
\    tuck                        \ lst0 xt1 lst0
\    list-apply-recursive        \ lst0 ( may now be invalid )
\    list-deallocate-recursive
;

: list-remove-struct ( xt item list -- data t | f )
    \ Check arg.
    assert( tos is-list? )

    list-remove         \ data t | f
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Append a nos list to the tos list.
: list-append-struct ( lst1 lst0 -- )
    \ Check arg.
    assert( tos is-list? )
    assert( nos is-list? )

    swap                        \ lst0 lst1

    foreach                     \ lst0 link data
        #2 pick                 \ lst0 link data lst0
        list-push-end-struct    \ lst0 link
    next
                                \ lst0
    drop
;
