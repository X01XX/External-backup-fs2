\ Test region-list functions.

: region-list-test-defining-regions

    \ Init regions to extract info from.
    s" (r1XXX rXX1X rX0XX rXXX0 r0X0X)"
    region-list-from-string-a               \ reg-lst'

    dup
    region-list-defining-regions            \ reg-lst' def-lst'

    cr ." results: " dup structinfo-list-print-struct-list cr

    s" ((r0X0X (r0101)) (rXX1X (r0111)) (r1XXX (r1101)))" list-from-string  \ reg-lst' def-lst' tst-list' bool
    if
        cr ." test lt: " dup structinfo-list-print-struct-list cr
    else
        cr ." list-from-string: failed?" cr
        abort
    then

    \ Clean up.
    structinfo-list-deallocate-struct-list
    structinfo-list-deallocate-struct-list
    region-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-list-test-defining-regions - Ok"
;

: region-list-tests
    region-list-test-defining-regions
    cr
;
