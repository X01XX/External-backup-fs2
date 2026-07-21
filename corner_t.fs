: corner-test-new
    s" s0101" state-from-string-a           \ sta
    s" r0XX1" region-from-string-a          \ sta reg
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
