
: intregcs-list-test-add-item?
    \ Test one common regioncorr, one not common.

    \ Init items to work on.
    s" ( iregcs ( regc 0 0 (r0101)) (( regc 0 0 (r01XX)) ( regc 0 0 (rXX01))))" intregcs-from-string-a
    s" (( iregcs ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ iregcs iregcs-lst bool

    \ Display.
    cr ." Add ok? " dup .bool cr

    \ Test.
    invert abort" Add not ok?"

    \ Deallocate.
    intregcs-list-deallocate
    intregcs-deallocate

    \ Test no common regioncorrs.

    \ Init items to work on.
    s" ( iregcs ( regc 0 0 (r1000)) (( regc 0 0 (r1X0X)) ( regc 0 0 (rX00X))))" intregcs-from-string-a
    s" (( iregcs ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ iregcs iregcs-lst bool

    \ Display.
    cr ." Add ok? " dup .bool cr

    \ Test.
    abort" Add ok?"

    \ Deallocate.
    intregcs-list-deallocate
    intregcs-deallocate

    \ Test all common regioncorrs.

    \ Init items to work on.
    s" ( iregcs ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX))))" intregcs-from-string-a
    s" (( iregcs ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    2dup intregcs-list-add-item?    \ iregcs iregcs-lst bool

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
    s" (( iregcs ( regc 0 0 (r0101)) (( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))) ( iregcs ( regc 0 0 (r0110)) (( regc 0 0 (rXX10)) ( regc 0 0 (r01XX)))))" list-from-string-a

    \ Do it.
    dup intregcs-list-regioncorr-list      \ iregcs-lst regc-lst

    \ Display.
    cr ." regc lst: " dup .regioncorr-list

    \ Test.
    s" (( regc 0 0 (r01XX)) ( regc 0 0 (r01XX)) ( regc 0 0 (rXX01)))" list-from-string-a \ iregcs-lst regc-lst regc-lst2
    2dup regioncorr-lists-eq? invert abort" lists ne?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    intregcs-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-list-test-regioncorr-list - Ok"
;

: intregcs-list-test-generate
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-split-by-intersections                          \ avd-lst cmp-lst, spl-lst t | f
    invert abort" split failed?"

    \ cr s" Split by ints:  " #2 pick .regioncorr-list-prefix             \ avd-lst cmp-lst spl-lst

    \ Remove regc value 1 fragments, which do not intersect two, or more, regioncorrs.
    dup regioncorr-list-regioncorrs-gt-pos-1                            \ avd-lst cmp-lst spl-lst spl-lst2
    cr s" Split by ints2: " #2 pick .regioncorr-list-prefix

    swap regioncorr-list-deallocate                                     \ avd-lst cmp-lst spl-lst2

    \ Generate intregcs list.

    2dup intregcs-list-generate                                         \ avd-lst cmp-lst spl-lst2 intregcs-lst

    cr s" intregcs: " #2 pick .intregcs-list-prefix cr

    \ Test.
    dup list-get-length #8 <> abort" list length not 8?"


    \ Deallocate.
    intregcs-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-list-test-generate - Ok"
;

: intregcs-list-tests
    intregcs-list-test-regioncorr-list
    \ intregcs-list-test-add-item?
    intregcs-list-test-generate
    cr
;
