
: regioncorrint-list-test-additional-item
    \ Init list to work on.
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

: regioncorrint-list-test-regioncorr-list
    \ Init list to work on.
    s" (( regci ( regc 0 0 (r0101)) (( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))) ( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    \ Do it.
    dup regioncorrint-list-regioncorr-list      \ regci-lst regc-lst

    \ Display.
    cr ." regc lst: " dup .regioncorr-list

    \ Test.
    s" (( regc 0 0 (r01XX)) ( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))" list-from-string-a \ regci-lst regc-lst regc-lst2
    2dup regioncorr-lists-eq? invert abort" lists ne?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorrint-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrint-list-test-regioncorr-list - Ok"
;

: regioncorrint-list-tests
    regioncorrint-list-test-regioncorr-list
    \ regioncorrint-list-test-additional-item
    cr
;
