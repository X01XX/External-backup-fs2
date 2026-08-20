\ Test regioncorr functions.

: regioncorr-test-new
    s" regc 3 -1 (r100 r0x0x)"

    list-from-string        \ lst t | f
    if
        \ Display.
        cr ." list from string: " dup .struct-list cr

        \ Test.
        dup list-get-length 1 <> abort" list len not one?"
        dup list-get-first-item
        dup is-regioncorr? invert abort" list first element not a regioncorr?"
        dup regioncorr-get-pos-value #3 <> abort" pos value not 3?"
        regioncorr-get-neg-value -1 <> abort" neg value not -1?"

        struct-list-deallocate
    else
        cr ." list not parsed" cr abort
    then

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-new - Ok"
;

: regioncorrs-test-intersect?

    s" (regc 0 0 (r0X0X r00x0x)) (regc 0 0 (rx1x1 r0x1x1))" string-to-stack-a

    2dup regioncorrs-intersect?
    invert abort" regc does not intersect?"

    s" (regc 0 0 (r0X0X r10x0x))" string-to-stack-a
    2dup regioncorrs-intersect?
    abort" regc intersects?"

    \ Deallocate.
    regioncorr-deallocate
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrs-test-intersect? - Ok"
;

: regioncorr-test-intersection

    s" (regc #2 -1 (r0X0X r00x0x)) (regc 1 #-4 (rx1x1 r0x1x1))" string-to-stack-a

    2dup regioncorr-intersection
    invert abort" regc does not intersect?"

    cr #2 pick .regioncorr space ." intersection: " over .regioncorr space ." = " dup .regioncorr cr

    \ Test.
    dup regioncorr-get-pos-value #3 <> abort" pos value not 3?"
    dup regioncorr-get-neg-value #-5 <> abort" pos value not -5?"

    s" ((r0101 r00101))" string-to-stack-a      \ regc1 regc2 regc3 regs-tst
    over regioncorr-get-list                    \ regc1 regc2 regc3 regs-tst regs
    over region-lists-corr-eq?                  \ regc1 regc2 regc3 regs-tst bool
    invert abort" regions not expectied?"

    \ Deallocate.
    region-list-deallocate
    regioncorr-deallocate
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-intersect? - Ok"
;

: regioncorr-test-subtract
    s" (regc #2 -1 (r0X0X r00x0x)) (regc 1 #-4 (rx1x1 r0x1x1))" string-to-stack-a

    cr dup .regioncorr space ." - " over .regioncorr

    2dup regioncorr-subtract            \ regc1 regc0 regc-lst

    space ." = " dup .regioncorr-list cr

    \ Test.
    s" ( regc 1 #-4 (rx1x1 r0x111)) ( regc 1 #-4 (rx1x1 r011x1)) ( regc 1  #-4 (rx111 r0x1x1)) ( regc 1  #-4 (r11x1 r0x1x1))"
    list-from-string-a

    2dup regioncorr-lists-eq? invert abort" unexpected result"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-subtract - Ok"
;

: regioncorr-test-distance
    s" (regc #2 -1 (r0X0X r00x0x)) (regc 1 #-4 (r10x0 r1x1x1))" string-to-stack-a

    cr dup .regioncorr space ." vs " over .regioncorr

    2dup regioncorr-distance            \ regc1 regc0 u

    space ." distance = " dup dec. cr

    \ Test.
    dup #2 <> abort" distance not 2?"

    \ Deallocate.
    drop
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-distance - Ok"
;

: regioncorr-test-superset?
    s" (regc #2 -1 (r10x0 r11101)) (regc 1 #-4 (rx0x0 r1x1x1))" string-to-stack-a

    cr dup .regioncorr space ." vs " over .regioncorr

    2dup regioncorr-superset?            \ regc1 regc0 bool

    space ." superset? = " dup .bool

    \ Test.
    if space ." - Ok" else cr ." not superset?" cr abort then

    \ Check non-superset.
    s" (regc #2 -1 (r10x0 r11100)) (regc 1 #-4 (rx0x0 r1x1x1))" string-to-stack-a

    cr dup .regioncorr space ." vs " over .regioncorr

    2dup regioncorr-superset?            \ regc1 regc0 bool

    space ." superset? = " dup .bool

    \ Test.
    if cr ." superset?" cr abort else space ." - Ok" cr then

    \ Deallocate.
    regioncorr-deallocate
    regioncorr-deallocate
    regioncorr-deallocate
    regioncorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-test-superset? - Ok"
;

: regioncorr-tests
    regioncorr-test-new
    regioncorrs-test-intersect?
    regioncorr-test-subtract
    regioncorr-test-intersection
    regioncorr-test-distance
    regioncorr-test-superset?
    cr
;
