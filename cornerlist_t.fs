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
        cr ." addl corners found: " dup .corner-list cr
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
    \ Possible regions:     (r1XX0 r10XX r111X rX0X0 rX01X r0X0X rX1X1 rXX10)
    \ Defining regions:     (r10XX r0X0X rXX10 rX01X r1XX0 rX1X1)

    s" ((crn s0011 rX01X (s0001 s0111)) (crn s0001 r0X0X (s0011 s1001)) (crn s0111 rX1X1 (s0110 s0011)) (crn s1001 r10XX (s1101 s0001)) (crn s0110 rXX10 (s0111 s0100)))"
    list-from-string-a                  \ crn-lst'
    cr ." crn lst: " dup .corner-list cr

    dup corner-list-defining-regions    \ crn-lst' reg-lst1'
    cr ." pos regs1: " dup .region-list cr

    s" ((crn s1101 rX1X1 (s1100 s1001)) (crn s1100 r1XX0 (s1101 s0100)) (crn s1001 r10XX (s1101 s0001)) (crn s0100 r0X0X (s0110 s1100)) (crn s0110 rXX10 (s0111 s0100)))"
    list-from-string-a                  \ crn-lst' reg-lst1' crn-lst'
    cr ." crn lst: " dup .corner-list cr

    dup corner-list-defining-regions    \ crn-lst' reg-lst1' crn-lst' reg-lst2'
    cr ." pos regs2: " dup .region-list cr

    [ ' regions-eq? ] literal           \ crn-lst' reg-lst1' crn-lst' reg-lst2' xt
    #3 pick #2 pick                     \ crn-lst' reg-lst1' crn-lst' reg-lst2' xt reg-lst1' reg-lst2'
    list-difference-struct              \ crn-lst' reg-lst1' crn-lst' reg-lst2' reg-lst3'
    cr ." dif regs 1-2: " dup .region-list cr

    [ ' regions-eq? ] literal           \ crn-lst' reg-lst1' crn-lst' reg-lst2' reg-lst3' xt
    #2 pick #5 pick                     \ crn-lst' reg-lst1' crn-lst' reg-lst2' reg-lst3' xt reg-lst2' reg-lst1'
    list-difference-struct              \ crn-lst' reg-lst1' crn-lst' reg-lst2' reg-lst3' reg-lst4'
    cr ." dif regs 2-1: " dup .region-list cr

    \ Deallocate.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    corner-list-deallocate
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
