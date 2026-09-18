\ Functions for random picks, where subsequent picks never repeat previous picks.

\ Return a nemuric list, 0 to limit, exclusive.
: pick-list ( num -- lst )
    assert( tos 0> )
    assert( tos #255 < )

    list-new swap   \ ret-lst num
    0 do
        i over list-push
    loop
;

\ Randomly pick an item from a list of integers.
\ The pick list is altered by removing one item.
: random-pick ( pk-lst -- num t | f )
    assert( tos is-list? )

    dup list-is-empty?
    if
        drop
        false
        exit
    then

    dup list-get-length             \ pk-lst len
    random                          \ pk-lst inx
    swap list-remove-item           \ itm
    true
;

\ Return a pick list for a list-of-lists.
\ Given ( () ( a b) ( c d e)), return ( (2 2) (2 1) (2 0) (1 1) (1 0)).
: lol-pick-list ( lol0 -- pck-lst )
    \ Init return listi, counter.
    list-new swap 0 swap            \ ret-lst cnt lol0

    foreach                         \ ret-lst cnt lol0-lnk lstx
        list-get-length             \ ret-lst cnt lol0-lnk len
        0 ?do
            \ cr over dec. space i dec.
            \ Init pick sub-list.
            list-new                \ ret-lst cnt lol0-lnk lstx

            \ Add counters.
            i over list-push        \ ret-lst cnt lol0-lnk lstx
            #2 pick over list-push  \ ret-lst cnt lol0-lnk lstx
            \ space dup .struct-list

            \ Save sub-list to return pick list.
            #3 pick list-push-struct
        loop
        \ Inc counter.
        swap 1+ swap
    next-item
                                    \ ret-lst cnt
    drop
;



