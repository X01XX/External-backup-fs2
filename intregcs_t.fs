
\ Try three similar ways to convert a intregcs string representation into a struct instance.
\ A wrinkle is that sub definitions of regioncorrs must be converted first.
\ intregcs-from-string and string-to-stack use list-from-string in
\ slightly different ways.
\ A few extraneous structs, and a number, are added to exercise list-from-string a little more.
: intregcs-test-from-string
    \     hint  intersection        list of regioncorr intersectors
    s" ( iregcs ( regc 1 -2 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01)))) rx101  #55" list-from-string
    invert abort" list-from-string failed?"

    \ Test.
    dup is-list? invert abort" list-from string failed?"
    dup list-get-length #3 <> abort" list-from string failed?"
    dup list-get-first-item is-intregcs? invert abort" list-from string failed?"
    dup list-get-second-item is-region? invert abort" list-from string failed?"
    dup list-get-third-item #55 <> abort" list-from string failed?"

    \ Display
    cr ." intregcs test 1:  " dup .struct-list cr

    s" ( iregcs ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 2 -1 (r0X01))))" intregcs-from-string
    invert abort" intregcs-from-string failed?"

    \ Test.
    dup is-intregcs? invert abort" intregcs-from string failed?"

    \ Display
    cr ." intregcs test 2:   " dup .intregcs cr

    s" r1x1x s10001 ( iregcs ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01))))" string-to-stack
    invert abort" string-to-stack failed?"

    \ Test.
    dup is-intregcs? invert abort" string-to-stack failed?"
    over is-state? invert abort" string-to-stack failed?"
    #2 pick is-region? invert abort" string-to-stack failed?"

    \ Display
    cr ." intregcs test 3:   " dup .intregcs cr

    \ Deallocate.
    intregcs-deallocate    \ Test 3
    state-deallocate
    region-deallocate
    intregcs-deallocate    \ Test 2
    struct-list-deallocate      \ Test 1

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-test-from-string - Ok"
;

: intregcs-test-min-distance
    s" ( iregcs ( regc 0 0 (r1010)) (( regc 0 0 (r101X)) ( regc 0 0 (rX010))))" string-to-stack-a
    s" ( iregcs ( regc 0 0 (r0100)) (( regc 0 0 (r010X)) ( regc 0 0 (rX100))))" string-to-stack-a

    2dup
    intregcs-min-distance           \ intregcs1 intregcs0 u

    \ Diplay.
    cr ." distance: " dup dec. cr

    \ Test.
    #2 <> abort" distance not 2?"

    \ Deallocate.
    intregcs-deallocate
    intregcs-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." intregcs-test-min-distance - Ok"
;

: intregcs-tests
    intregcs-test-from-string
    intregcs-test-min-distance
    cr
;
