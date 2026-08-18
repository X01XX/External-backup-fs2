\ Test regioncorr functions.

: regioncorr-test-new
    s" regc 3 -1 (r100 r0x0x)"

    list-from-string        \ lst t | f
    if
        \ Display.
        cr ." regioncorr: " dup .struct-list cr

        \ Test.
        dup list-get-length 1 <> abort" list len not one?"
        dup list-get-first-item
        is-regioncorr? invert abort" list first element not a regioncorr?"

        struct-list-deallocate
    else
        cr ." list not parsed" cr abort
    then

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-new - Ok"
;

: regioncorr-tests
    regioncorr-test-new
    cr
;
