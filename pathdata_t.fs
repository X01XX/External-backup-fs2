
: pathdata-test-new
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    #-2 pathdata-new                                                    \ avd-lst pd

    \ Display.
    cr dup .pathdata cr

    \ Deallocate
    pathdata-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." pathdata-test-inew - Ok"
;

\ Test 1->B, both within X0XX. Neither are in regioncorr intersections.
: pathdata-test-find-path
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    0 pathdata-new                                                      \ avd-lst pd

    \ Generate to-from regioncorrs. B and 1 are within X0XX.
    s" ( regc 0 0 (r1011)) ( regc 0 0 (r0001))" string-to-stack-a       \ avd-lst pd to from

    \ Find path.
    2dup                                                                \ avd-lst pd to from to from
    #4 pick                                                             \ avd-lst pd to from to from pd
    pathdata-find-path                                                  \ avd-lst pd to from, pthstp-lst t | f

    if
        \ Display results.
        cr ." path: " dup .pathstep-list cr

        \ Test results.
        dup list-get-length 1 <> abort" PathStep list len ne 1?"

        \ Check pathstep start.
        s" ( regc 1  0 (r0001))" string-to-stack-a
        over pathstep-list-get-from
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Check pathstep end.
        s" ( regc 1  0 (r1011))" string-to-stack-a
        over pathstep-list-get-to
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Test pathstep within.
        s" ( regc 0 0 (rX0XX))" string-to-stack-a                       \ avd-lst pd to from pthstp-lst tst-reg
        over list-get-first-item pathstep-get-within                    \ avd-lst pd to from pthstp-lst tst-reg regc
        over regioncorrs-eq-regions?                                    \ avd-lst pd to from pthstp-lst tst-reg bool
        ifnot cr ." unexpected within?" then                            \ avd-lst pd to from pthstp-lst tst-reg

        \ Clean up.
        regioncorr-deallocate
        pathstep-list-deallocate
    else
        cr ." path not found" cr abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    pathdata-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." pathdata-test-find-path - Ok"
;

\ Test 4->B, in intersecting regioncorrs X0XX and XXX0. Neither are in regioncorr intersections.
: pathdata-test2-find-path
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ sess avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    0 pathdata-new                                                      \ avd-lst pd

    \ Generate to-from regioncorrs. B is within X0XX, 4 is within XXX0.
    \ X0XX and XXX0 intersect at X0X0.
    s" ( regc 0 0 (r1011)) ( regc 0 0 (r0100))" string-to-stack-a       \ avd-lst pd to from

    \ Find path.
    2dup                                                                \ avd-lst pd to from to from
    #4 pick                                                             \ avd-lst pd to from to from pd
    pathdata-find-path                                                  \ avd-lst pd to from, pthstp-lst t | f

    if
        \ Display results.
        cr s" path: " #2 pick .pathstep-list-prefix cr

        \ Test results.
        dup list-get-length #2 <> abort" PathStep list len ne 2?"

        \ Check pathstep start.
        s" ( regc 1  0 (r0100))" string-to-stack-a
        over pathstep-list-get-from
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Check pathstep end.
        s" ( regc 1  0 (r1011))" string-to-stack-a
        over pathstep-list-get-to
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Clean up.
        pathstep-list-deallocate
    else
        cr ." path not found" cr abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    pathdata-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." pathdata-test2-find-path - Ok"
;

\ Test 7->D, in non-intersecting 0X1X and 1X0X. Neither are in regioncorr intersections.
: pathdata-test3-find-path
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    0 pathdata-new                                                      \ avd-lst pd

    \ Generate to-from regioncorrs. 7 is within 0X1X, D is within 1X0X, non-intersecting regions.
    s" ( regc 0 0 (r1101)) ( regc 0 0 (r0111))" string-to-stack-a       \ avd-lst pd to from

    \ Find path.
    2dup                                                                \ avd-lst pd to from to from
    #4 pick                                                             \ avd-lst pd to from to from pd
    pathdata-find-path                                                  \ avd-lst pd to from, pthstp-lst t | f

    if
        \ Display results.
        cr s" path: " #2 pick .pathstep-list-prefix cr

        \ Check pathstep start.
        s" ( regc 1  0 (r0111))" string-to-stack-a
        over pathstep-list-get-from
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Check pathstep end.
        s" ( regc 1  0 (r1101))" string-to-stack-a
        over pathstep-list-get-to
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Clean up.
        pathstep-list-deallocate
    else
        cr ." path not found" cr abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    pathdata-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." pathdata-test3-find-path - Ok"
;

\ Test 3->C, in (0X1X X0XX) and (1X0X XXX0). Both are in regioncorr intersections.
: pathdata-test4-find-path
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    0 pathdata-new                                                      \ avd-lst pd

    \ Generate to-from regioncorrs. 7 is within 0X1X, D is within 1X0X, non-intersecting regions.
    s" ( regc 0 0 (r1100)) ( regc 0 0 (r0011))" string-to-stack-a       \ avd-lst pd to from

    \ Find path.
    2dup                                                                \ avd-lst pd to from to from
    #4 pick                                                             \ avd-lst pd to from to from pd
    pathdata-find-path                                                  \ avd-lst pd to from, pthstp-lst t | f

    if
        \ Display results.
        cr s" path: " #2 pick .pathstep-list-prefix cr

        \ Test results.
        \ dup list-get-length #3 <> abort" PathStep list len ne 3?"

        \ Check pathstep start.
        s" ( regc 1  0 (r0011))" string-to-stack-a
        over pathstep-list-get-from
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Check pathstep end.
        s" ( regc 1  0 (r1100))" string-to-stack-a
        over pathstep-list-get-to
        over regioncorrs-eq-regions? invert abort" from not matched?"
        regioncorr-deallocate

        \ Clean up.
        pathstep-list-deallocate
    else
        cr ." path not found" cr abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    pathdata-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." pathdata-test4-find-path - Ok"
;

: pathdata-tests
    pathdata-test-new
    pathdata-test-find-path
    pathdata-test2-find-path
    pathdata-test3-find-path
    pathdata-test4-find-path
;
