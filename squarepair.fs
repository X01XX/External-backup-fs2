\ Process square pairs, a list of two squares.

\ Return true if tos is a square-pair.
: assert-tos-is-square-pair ( lst0 -- bool )
    assert-tos-is-square-list
    dup list-get-length
    #2 =
;

\ Deallocate a square list.
: square-pair-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-square-pair

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

\ Return the distance of two squares.
: square-pair-get-distance ( sqr-pr0 -- u )
    \ Check arg.
    assert-tos-is-square-list

    dup list-get-first-item square-get-state    \ sqr-pr0 sta1
    swap list-get-second-item square-get-state  \ sta1 sta2
    states-distance
;

\ Return the sum of the number of samples of two squares.
: square-pair-get-num-samples ( sqr-pr0 -- u )
    \ Check arg.
    assert-tos-is-square-list

    dup list-get-first-item square-get-num-samples      \ sqr-pr0 ns1
    swap list-get-second-item square-get-num-samples    \ ns1 sn2
    +
;
