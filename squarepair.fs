\ Process square pairs, a list of two squares.

\ Return true if tos is a square-pair.
: is-square-pair? ( tos -- t )
    tos is-square-list?     \ tos bool
    ifnot
        drop
        false
        exit
    then

    list-get-length         \ len
    #2 =
;

\ Deallocate a square list.
: square-pair-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-square-pair? )

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
    assert( tos is-square-pair? )

    dup list-get-first-item square-get-state    \ sqr-pr0 sta1
    swap list-get-second-item square-get-state  \ sta1 sta2
    states-distance
;

\ Return the sum of the number of samples of two squares.
: square-pair-get-num-samples ( sqr-pr0 -- u )
    \ Check arg.
    assert( tos is-square-pair? )

    dup list-get-first-item square-get-num-samples      \ sqr-pr0 ns1
    swap list-get-second-item square-get-num-samples    \ ns1 sn2
    +
;

\ Return a region, using a square-pair's square regions.
: square-pair-to-region ( sqr-pr0 --- reg )
    \ cr ." square-pair-to-region: " .stack-gbl cr
    \ Check arg.
    assert( tos is-square-pair? )

    dup list-get-first-item square-get-state    \ sqr-pr0 sta1
    swap list-get-second-item square-get-state  \ sta1 sta2
    region-new
;
