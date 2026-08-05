\ corner-list tests.

\ For use with list function, having xt signature is ( item link-data -- flag )
: corner-test-rate ( val corner -- bool ) corner-get-rate = ;

: corner-list-test-x
    \ Init data.
    s" (r0X0X rxX1X r1XxX)" list-from-string-a  \ reg-lst
    s" (cxX1X c0X0X c1XxX)" list-from-string-a  \ reg-lst crn-lst

    \ Set rates.
    2dup corner-list-calc-set-rate              \ reg-lst crn-lst

    \ Get max rate.
    [ ' corner-get-rate ] literal               \ reg-lst crn-lst xt
    over                                        \ reg-lst crn-lst xt crn-lst
    list-max-value                              \ reg-lst crn-lst val
    cr ." max is: " dup . cr

    \ Test.
    dup 2 <> abort" max not 2?"

    [ ' corner-test-rate ] literal              \ reg-lst crn-lst val xt
    swap                                        \ reg-lst crn-lst xt val
    #2 pick                                     \ reg-lst crn-lst xt val crn-lst
    list-find                                   \ reg-lst crn-lst, crnx t | f

    if
        cr ." max found: " dup .corner cr       \ reg-lst crn-lst crnx
    else
        cr ." max not found" cr abort
    then

    \ Test
    s" s0101" state-from-string-a               \ reg-lst crn-lst crnx s5'
    over corner-get-anchor-state                \ reg-lst crn-lst crnx s5' anc
    over states-eq? invert abort" state 5 anchor not found?"

    \ Deallocate.
    state-deallocate                            \ reg-lst crn-lst crnx
    drop                                        \ reg-lst crn-lst
    corner-list-deallocate
    region-list-deallocate

     \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-list-test-x - Ok"
;
