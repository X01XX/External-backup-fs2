
: regcints-list-test-generate
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

    \ Generate regcints list.

    2dup regcints-list-generate                                         \ avd-lst cmp-lst spl-lst2 regcints-lst
    cr s" regcints: " #2 pick .regcints-list-prefix cr

    \ Test.
    dup list-get-length #4 <> abort" list length not 4?"

    \ Deallocate.
    regcints-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regcints-list-test-generate - Ok"
;

: regcints-list-tests
    regcints-list-test-generate
    cr
;
