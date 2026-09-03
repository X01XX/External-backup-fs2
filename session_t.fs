
: session-test-find-path
    \ Run function.
    session-new                 \ sess

    \ Display results.
    cr dup .session cr

    #4 over session-add-domain  \ sess dom
    drop

    \ Display results.
    cr dup .session cr

    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ sess avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ sess avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-split-by-intersections                          \ sess avd-lst cmp-lst, spl-lst t | f
    invert abort" split failed?"

    \ cr s" Split by ints:  " #2 pick .regioncorr-list-prefix           \ sess avd-lst cmp-lst spl-lst

    \ Remove regc value 1 fragments, which do not intersect two, or more, regioncorrs.
    dup regioncorr-list-regioncorrs-gt-pos-1                            \ sess avd-lst cmp-lst spl-lst spl-lst2
    cr s" Split by ints2: " #2 pick .regioncorr-list-prefix

    swap regioncorr-list-deallocate                                     \ sess avd-lst cmp-lst spl-lst2

    \ Generate intregcs list.
    2dup intregcs-list-generate                                         \ sess avd-lst cmp-lst spl-lst2 intregcs-lst
    cr s" intregcs: " #2 pick .intregcs-list-prefix cr

    \ Generate regcints list.
    #2 pick #2 pick regcints-list-generate                              \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst
    cr s" regcints: " #2 pick .regcints-list-prefix cr

    \ Generate to-from regioncorrs. B and 1 are within X0XX.
    s" ( regc 0 0 (r1011)) ( regc 0 0 (r0001))" string-to-stack-a       \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from

    \ Find path.
    #3 pick #3 pick #3 pick #3 pick                                     \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from
    #11 pick                                                            \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from sess
    session-find-path                                                   \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from, pthstp-lst t | f

    if
        \ Display results.
        cr ." path: " dup .pathstep-list cr

        \ Test results.
        dup list-get-length 1 <> abort" PathStep list len ne 1?"
        s" ( regc 0 0 (rX0XX))" string-to-stack-a                       \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from pthstp-lst tst-reg
        over list-get-first-item pathstep-get-within                    \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from pthstp-lst tst-reg regc
        over regioncorrs-eq-regions?                                    \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from pthstp-lst tst-reg bool
        ifnot cr ." unexpected within?" then                            \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from pthstp-lst tst-reg

        \ Clean up.
        regioncorr-deallocate
        pathstep-list-deallocate
    else
        cr ." path not found" cr abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    regcints-list-deallocate
    intregcs-list-deallocate

    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    session-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." session-test-find-path - Ok"
;

: session-test2-find-path
    \ Run function.
    session-new                 \ sess

    \ Display results.
    cr dup .session cr

    #4 over session-add-domain  \ sess dom
    drop

    \ Display results.
    cr dup .session cr

    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ sess avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ sess avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-split-by-intersections                          \ sess avd-lst cmp-lst, spl-lst t | f
    invert abort" split failed?"

    \ cr s" Split by ints:  " #2 pick .regioncorr-list-prefix           \ sess avd-lst cmp-lst spl-lst

    \ Remove regc value 1 fragments, which do not intersect two, or more, regioncorrs.
    dup regioncorr-list-regioncorrs-gt-pos-1                            \ sess avd-lst cmp-lst spl-lst spl-lst2
    cr s" Split by ints2: " #2 pick .regioncorr-list-prefix

    swap regioncorr-list-deallocate                                     \ sess avd-lst cmp-lst spl-lst2

    \ Generate intregcs list.
    2dup intregcs-list-generate                                         \ sess avd-lst cmp-lst spl-lst2 intregcs-lst
    cr s" intregcs: " #2 pick .intregcs-list-prefix cr

    \ Generate regcints list.
    #2 pick #2 pick regcints-list-generate                              \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst
    cr s" regcints: " #2 pick .regcints-list-prefix cr

    \ Generate to-from regioncorrs. B is within X0XX, 4 is within XXX0.
    \ X0XX and XXX0 intersect at X0X0.
    s" ( regc 0 0 (r1011)) ( regc 0 0 (r0100))" string-to-stack-a       \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from

    \ Find path.
    #3 pick #3 pick #3 pick #3 pick                                     \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from
    #11 pick                                                            \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from sess
    session-find-path                                                   \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from, pthstp-lst t | f

    if
        \ Display results.
        cr s" path: " #2 pick .pathstep-list-prefix cr

        \ Test results.
        dup list-get-length #2 <> abort" PathStep list len ne 1?"

        dup pathstep-list-get-from cr ." from: " .regioncorr
        dup pathstep-list-get-to space ." to: " .regioncorr cr

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

    regcints-list-deallocate
    intregcs-list-deallocate

    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    session-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." session-test2-find-path - Ok"
;

: session-test3-find-path
    \ Run function.
    session-new                 \ sess

    \ Display results.
    cr dup .session cr

    #4 over session-add-domain  \ sess dom
    drop

    \ Display results.
    cr dup .session cr

    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a   \ sess avd-lst

    cr s" Items to avoid: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-complement                                      \ sess avd-lst cmp-lst
    cr s" Complements:    " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-split-by-intersections                          \ sess avd-lst cmp-lst, spl-lst t | f
    invert abort" split failed?"

    \ cr s" Split by ints:  " #2 pick .regioncorr-list-prefix           \ sess avd-lst cmp-lst spl-lst

    \ Remove regc value 1 fragments, which do not intersect two, or more, regioncorrs.
    dup regioncorr-list-regioncorrs-gt-pos-1                            \ sess avd-lst cmp-lst spl-lst spl-lst2
    cr s" Split by ints2: " #2 pick .regioncorr-list-prefix

    swap regioncorr-list-deallocate                                     \ sess avd-lst cmp-lst spl-lst2

    \ Generate intregcs list.
    2dup intregcs-list-generate                                         \ sess avd-lst cmp-lst spl-lst2 intregcs-lst
    cr s" intregcs: " #2 pick .intregcs-list-prefix cr

    \ Generate regcints list.
    #2 pick #2 pick regcints-list-generate                              \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst
    cr s" regcints: " #2 pick .regcints-list-prefix cr

    \ Generate to-from regioncorrs. 7 is within 0X1X, D is within 1X0X, non-intersecting regions.
    s" ( regc 0 0 (r1101)) ( regc 0 0 (r0111))" string-to-stack-a       \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from

    \ Find path.
    #3 pick #3 pick #3 pick #3 pick                                     \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from
    #11 pick                                                            \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from intregcs-lst regcints-lst to from sess
    session-find-path                                                   \ sess avd-lst cmp-lst spl-lst2 intregcs-lst regcints-lst to from, pthstp-lst t | f

    if
        \ Display results.
        cr s" path: " #2 pick .pathstep-list-prefix cr

        \ Test results.
        dup list-get-length #2 <> abort" PathStep list len ne 1?"

        dup pathstep-list-get-from cr ." from: " .regioncorr
        dup pathstep-list-get-to space ." to: " .regioncorr cr

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
        cr ." path not found" cr    \ abort
    then

    \ Clean up.
    regioncorr-deallocate
    regioncorr-deallocate

    regcints-list-deallocate
    intregcs-list-deallocate

    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    session-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." session-test3-find-path - Ok"
;

: session-tests
    session-test-find-path
    session-test2-find-path
    session-test3-find-path
    cr
;
