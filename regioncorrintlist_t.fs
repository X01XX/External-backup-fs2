
: regioncorrint-list-test-additional-item
    \ Init lists to work on.
    s" (( regci ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01)))) )" list-from-string-a

    dup regioncorr-list-complement
    cr s" Complements: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-map-routes  \ avoid-lst comp-lst ?

    \ Display.
\    s" regcorrints: " #2 pick .regioncorrint-list-prefix

    \ Test.

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrint-list-test-additional-item - Ok"
;


: regioncorrint-list-tests
    \ regioncorrint-list-test-additional-item
    cr
;
