
: regcints-list-test-generation-from-complement-list
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-split-by-intersections                          \ avd-lst cmp-lst, spl-lst t | f
    invert abort" split failed?"

    cr s" Split by ints:  " #2 pick .regioncorr-list-prefix

    \ Init regcints list.
    list-new                        \ avd-lst cmp-lst spl-lst regcis-lst
    #2 pick                         \ avd-lst cmp-lst spl-lst regcis-lst cmp-l

    foreach                         \ avd-lst cmp-lst spl-lst regcis-lst cmp-lnk cmpx
        drop
    next


    \ Deallocate.
    regcints-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regcints-list-test-generation-from-complement-list - Ok"
;

: regcints-list-tests
    regcints-list-test-generation-from-complement-list
    cr
;
