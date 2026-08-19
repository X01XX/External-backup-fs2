\ regioncorr-list tests.

: regioncorr-list-test-split-by-intersections
    s" (regc 0 0 (r0X0X r00x0x)) (regc 0 0 (rx1x1 r0x1x1))" list-from-string-a
    cr .stack-gbl cr

    \ regioncorr-list-split-by-intersections

    \ Deallocate.
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections - Ok"
;

: regioncorr-list-tests
    regioncorr-list-test-split-by-intersections
    cr
;
