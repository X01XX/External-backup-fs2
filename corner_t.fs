: corner-test-new
    list-new                                \ sqr-lst
    s" s1010->s0101" sample-from-string-a   \ sqr-lst c-addr u
    square-new                              \ sqr-lst sqr
    corner-new                              \ crn
    cr ." crn: " dup .corner cr

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-new - Ok"
;

: corner-tests
    corner-test-new
;
