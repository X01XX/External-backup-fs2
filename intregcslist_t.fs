
: intregcs-list-test-add-item?
    \ Test one common regioncorr, one not common.

    \ Init items to work on.
    s" ( regci ( regc 0 0 (r0101)) (( regc 0 0 (r01XX)) ( regc 0 0 (rXX01))))" intregcs-from-string-a
    s" (( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ regci regci-lst bool

    \ Display.
    cr ." Add ok? " dup .bool cr

    \ Test.
    invert abort" Add not ok?"

    \ Deallocate.
    intregcs-list-deallocate
    intregcs-deallocate

    \ Test no common regioncorrs.

    \ Init items to work on.
    s" ( regci ( regc 0 0 (r1000)) (( regc 0 0 (r1X0X)) ( regc 0 0 (rX00X))))" intregcs-from-string-a
    s" (( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ regci regci-lst bool

    \ Display.
    cr ." Add ok? " dup .bool cr

    \ Test.
    abort" Add ok?"

    \ Deallocate.
    intregcs-list-deallocate
    intregcs-deallocate

    \ Test all common regioncorrs.

    \ Init items to work on.
    s" ( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX))))" intregcs-from-string-a
    s" (( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ regci regci-lst bool

    \ Display.
    cr ." Add ok? " dup .bool cr

    \ Test.
    abort" Add ok?"

    \ Deallocate.
    intregcs-list-deallocate
    intregcs-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-list-test-add-item? - Ok"
;

: intregcs-list-test-regioncorr-list
    \ Init list to work on.
    s" (( regci ( regc 0 0 (r0101)) (( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))) ( regci ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    \ Do it.
    dup intregcs-list-regioncorr-list      \ regci-lst regc-lst

    \ Display.
    cr ." regc lst: " dup .regioncorr-list

    \ Test.
    s" (( regc 0 0 (r01XX)) ( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))" list-from-string-a \ regci-lst regc-lst regc-lst2
    2dup regioncorr-lists-eq? invert abort" lists ne?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    intregcs-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-list-test-regioncorr-list - Ok"
;

: intregcs-list-tests
    intregcs-list-test-regioncorr-list
    intregcs-list-test-add-item?
    cr
;
