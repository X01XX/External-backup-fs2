\ Test region-list functions.

: region-list-test-defining-regions

    \ Init regions to extract info from.
    s" (r1XXX rXX1X rX0XX rXXX0 r0X0X)"
    region-list-from-string-a               \ reg-lst'

    \ Get defining regions info.
    dup
    region-list-defining-regions            \ reg-lst' def-lst'
    \ cr ." defining: " dup structinfo-list-print-struct-list cr

    \ Check results.
    s" ((r0X0X (r0101)) (rXX1X (r0111)) (r1XXX (r1101)))" list-from-string-a  \ reg-lst' def-lst' tst-list'
    \ cr ." test lt: " dup structinfo-list-print-struct-list cr

    2dup lists-eq?
    if
    else
        cr ." lists ne?" cr
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

: region-list-test-defining-regions2

    s" s0101" state-from-string-a       \ sta5
    s" s0110" state-from-string-a       \ sta5 sta6
    s" s1001" state-from-string-a       \ sta5 sta6 sta9
    #2 pick #2 pick state-~a+~b         \ sta5 sta6 sta9 reg-56-lst
    cr ." ~5 + ~6: " dup .region-list cr

    #3 pick #2 pick state-~a+~b         \ sta5 sta6 sta9 reg-56-lst reg-59-lst
    cr ." ~5 + ~9: " dup .region-list cr

    2dup region-list-intersections-nosubs   \ sta5 sta6 sta9 reg-56-lst reg-59-lst reg-569-lst
    cr ." (~5 + ~6) & (~5 + ~9): " dup .region-list cr

    dup region-list-defining-regions    \ sta5 sta6 sta9 reg-56-lst reg-59-lst reg-569-lst def-lst
    cr ." defining: " dup structinfo-list-print-struct-list cr

    \ Clean up.
    structinfo-list-deallocate-struct-list
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-list-test-defining-regions2 - Ok"
;

: region-list-tests
    region-list-test-defining-regions
    region-list-test-defining-regions2
    cr
;
