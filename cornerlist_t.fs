\ corner-list tests.

: corner-list-test-corner-additional-corners
    \ Init data.
    s" (r0X0X rxX1X r1XxX)" list-from-string-a          \ reg-lst

    \ Init corner list.
    list-new                                            \ reg-lst crn-lst
    s" (crn s0111 rxX1X (s0101))" corner-from-string    \ reg-lst crn-lst, crn t | f
    invert abort" corner-from-string failed?"
    over list-push-struct                               \ reg-lst crn-lst

    s" (crn s0101 r0X0X (s1101 s0111))" corner-from-string  \ reg-lst crn-lst, crn t | f
    invert abort" corner-from-string failed?"
    over list-push-struct                               \ reg-lst crn-lst

    s" (crn s1101 r1XxX (s0101))" corner-from-string    \ reg-lst crn-lst, crn t | f
    invert abort" corner-from-string failed?"
    over list-push-struct                               \ reg-lst crn-lst

    \ s" (cxX1X c0X0X c1XxX)" list-from-string-a  \ reg-lst crn-lst

    \ Set rates.
    2dup corner-list-calc-set-rate              \ reg-lst crn-lst

    \ Get max rate.
    [ ' corner-get-rate ] literal               \ reg-lst crn-lst xt
    over                                        \ reg-lst crn-lst xt crn-lst
    list-max-value                              \ reg-lst crn-lst val
    cr ." max is: " dup . cr

    \ Test.
    dup #2 <> abort" max not 2?"

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

    #2 pick                                     \ reg-lst crn-lst crnx reg-lst
    swap                                        \ reg-lst crn-lst reg-lst crnx
    corner-additional-corners                   \ reg-lst crn-lst, crn-lst t | f
    if
        cr ." addl corners found: " dup .struct-list cr
    else
        cr ." NO addl corners found: " cr abort
    then

    \ Number of corners = number defining regions?

    \ Deallocate.
    corner-list-deallocate
    corner-list-deallocate
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-list-test-corner-additional-corners - Ok"
;

: corner-list-test-defining-regions
    s" ((crn s0011 rX01X (s0001 s0111)) (crn s1100 r1XX0 (s1101 s0100)) (crn s1001 r10XX (s1101 s0001)) (crn s0110 rXX10 (s0111 s0100)))"
    list-from-string-a                  \ crn-lst'
    cr ." crn lst: " dup .struct-list cr

    dup corner-list-defining-regions    \ crn-lst' reg-lst1'
    cr ." def regs: " dup .region-list cr

    \ Deallocate.
    region-list-deallocate
    corner-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-list-test-defining-regions - Ok"
;

: corner-list-tests
    corner-list-test-corner-additional-corners
    corner-list-test-defining-regions
    cr
;
