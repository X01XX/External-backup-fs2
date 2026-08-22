\ regioncorr-list tests.

: regioncorr-list-test-split-by-intersections
    s" (regc 1 -1 (r0X0X r00x0x)) (regc #2 #-2 (rx1x1 r0x1x1))" list-from-string-a

    dup regioncorr-list-split-by-intersections             \ regc-lst0, regc-lst t | f
    invert abort" split failed?"

    cr ." intersections: " dup .regioncorr-list cr

    \ Test.
    s" ( regc 3  -3 (r0101 r00101)) ( regc 1  -1 (r0X0X r00x00)) ( regc 1  -1 (r0X0X r0000x)) ( regc 1  -1 (r0X00 r00x0x)) ( regc 1  -1 (r000X r00x0x)) ( regc 2  -2 (rx1x1 r0x111)) ( regc 2  -2 (rx1x1 r011x1)) ( regc 2  -2 (rx111 r0x1x1)) ( regc 2  -2 (r11x1 r0x1x1))"
    list-from-string-a
    2dup regioncorr-lists-eq?
    invert abort" unexpected results?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections - Ok"
;

: regioncorr-list-test-split-by-intersections2
    s" (regc 1 -1 (rxxxx)) (regc #2 -#2 (rxxx1)) (regc #3 #-3 (rx1x1)) (regc #4 #-4 (rx1x1))" list-from-string-a

    s" start: " #2 pick .regioncorr-list-prefix cr

    dup regioncorr-list-split-by-intersections             \ regc-lst0, regc-lst t | f
    invert abort" split failed?"

    cr s" intersections: " #2 pick .regioncorr-list-prefix cr

    \ Test.
    s" ( regc #10  #-10 (rX1X1)) ( regc #3  #-3 (rX0X1)) ( regc 1  -1 (rxxx0))"
    list-from-string-a
    2dup regioncorr-lists-eq?
    invert abort" unexpected results?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections2 - Ok"
;

: regioncorr-list-tests
    regioncorr-list-test-split-by-intersections
    regioncorr-list-test-split-by-intersections2
    cr
;
