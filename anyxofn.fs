
\ Return a copy of a list, with each item within enclosed in a list.
: make-list-of-each-item ( lst0 -- lst )
    \ Check arg.
    assert( tos is-list? )

    \ Init return list.
    list-new swap               \ ret-lst lst0

    foreach                     \ ret-lst lnk item
        list-new tuck           \ ret-lst lnx i-lst item i-lst
        list-push-struct        \ ret-lst lnk i-lst
        #2 pick                 \ ret-lst lnk i-lst ret-lst
        list-push-end-struct    \ ret-lst lnk
    next-item
;

\ Return a list of all x combinations of n struct instances in a list.
: any-x-of-n ( num1 lst0 -- lol )
    \ Check args.
    assert( tos is-list? )
    assert( tos list-is-not-empty? )
    assert( nos 0 > )
    assert( nos over list-get-length <= )

    \ cr ." any-x-of-n: start: " over dec. space dup .struct-list cr

    over #2 <                   \ num1 lst0
    if
        nip                     \ lst0
        make-list-of-each-item  \ lst
        exit
    then

    \ Decrement number, for n-1 of x.
    swap 1 - swap               \ num1 lst0

    \ Init return list.
    list-new -rot               \ ret-lst num1 lst0

    \ Preserve the passed list, caller to deallocate, deconstructed here.
    list-copy-struct            \ ret-lst num1 lst'

    \ Set up start and limit for loop.
    \ The last x-1 items in the list will not have enough items left to combine to make x items.
    dup list-get-length         \ ret-lst num1 lst' len
    #2 pick - 0                 \ ret-lst num1 lst' limit start

    \ For each item, combine with ( n-1 ) of x.
    do                          \ ret-lst num1 lst'
        \ Save first item, get n-1 of x of the following items.
        dup list-pop-struct     \ ret-lst num1 lst', item0 t | f
        invert abort" pop failed?"

        #2 pick #2 pick         \ ret-lst num1 lst' item0 num1 lst'
        recurse                 \ ret-lst num1 lst' item0 lol'

        \ Add first item to each n-1 of x.
        dup
        foreach                 \ ret-lst num1 lst' item0 lol' lol-lnk itmx
            #3 pick swap        \ ret-lst num1 lst' item0 lol' lol-lnk item0 itmx
            list-push-struct    \ ret-lst num1 lst' item0 lol' lol-lnk
        next-item

        \ Save n of x lists to return.
        dup #5 pick             \ ret-lst num1 lst' item0 lol' lol' ret-lst
        list-append-struct      \ ret-lst num1 lst' item0 lol'
        struct-list-deallocate
        drop
    loop
                                \ ret-lst num1 lst' ( x-1 items left )
    struct-list-deallocate      \ ret-lst num1
    drop
;

: any-x-of-n-test
    list-new
    #3 #4 state-new over list-push-struct
    #4 #4 state-new over list-push-struct
    #5 #4 state-new over list-push-struct
    #6 #4 state-new over list-push-struct   \ sta-lst'
    cr ." states: " dup .state-list cr

    #3 over any-x-of-n                      \ sta-lst' sta-lol'
    cr ." ret: " dup .struct-list cr

    \ Test.
    s" ((s0110 s0101 s0100) (s0110 s0101 s0011) (s0110 s0100 s0011) (s0101 s0100 s0011))" list-from-string-a \ sta-lst' sta-lol 'sta-lol-tst'
    [ ' state-lists-eq? ] literal           \ sta-lst' sta-lol 'sta-lol-tst' xt
    #2 pick #2 pick struct-lists-eq?        \ sta-lst' sta-lol 'sta-lol-tst' bool
    invert abort" unexpected result?"

    \ Deallocate.
    struct-list-deallocate
    struct-list-deallocate
    state-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated
;
