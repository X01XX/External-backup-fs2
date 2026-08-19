\ regioncorr-list tests.

: regioncorr-list-test-split-by-intersections
    s" (regc 0 0 (r0X0X r00x0x)) (regc 0 0 (rx1x1 r0x1x1))" list-from-string-a
    cr .stack-gbl cr

    dup regioncorr-list-split-by-intersections             \ regc1 regr0, regc-lst t | f
    invert abort" split failed?"

    cr ." intersections: " dup .regioncorr-list cr

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections - Ok"
;

: regioncorr-list-tests
    regioncorr-list-test-split-by-intersections
    cr
;
