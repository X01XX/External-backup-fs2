: corner-test-new
    s" s1010->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-new - Ok"
;

: corner-tests
    corner-test-new
;
