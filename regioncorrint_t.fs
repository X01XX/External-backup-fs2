
: regioncorrint-test-from-string
    \ Init lists to work on.
    \ list-from-string, called by regioncorrint-from-string, interprets
    \ the second element as a regioncorr, the third element as a regioncorr list.
    \     hint  intersection        regioncorr intersectors
    s" ( regci ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01))))" regioncorrint-from-string
    invert abort" regioncorrint-from-string failed?"

    \ Display
    cr ." regioncorrint:  " dup .regioncorrint cr

    \ Deallocate.
    regioncorrint-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrint-test-from-string - Ok"
;

: regioncorrint-tests
    regioncorrint-test-from-string
    cr
;
