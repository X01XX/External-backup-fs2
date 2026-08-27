
\ Try three similar ways to convert a regioncorrint string representation into a struct instance.
\ A wrinkle is that sub definitions of regioncorrs must be converted first.
\ regioncorrint-from-string and string-to-stack use list-from-string in
\ slightly different ways.
\ A few extraneous structs, and a number, are added to exercise list-from-string a little more.
: regioncorrint-test-from-string
    \     hint  intersection        list of regioncorr intersectors
    s" ( regci ( regc 1 -2 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01)))) rx101  #55" list-from-string
    invert abort" list-from-string failed?"

    \ Test.
    dup is-list? invert abort" list-from string failed?"
    dup list-get-length #3 <> abort" list-from string failed?"
    dup list-get-first-item is-regioncorrint? invert abort" list-from string failed?"
    dup list-get-second-item is-region? invert abort" list-from string failed?"
    dup list-get-third-item #55 <> abort" list-from string failed?"

    \ Display
    cr ." regioncorrint test 1:  " dup .struct-list cr

    s" ( regci ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 2 -1 (r0X01))))" regioncorrint-from-string
    invert abort" regioncorrint-from-string failed?"

    \ Test.
    dup is-regioncorrint? invert abort" regioncorrint-from string failed?"

    \ Display
    cr ." regioncorrint test 2:   " dup .regioncorrint cr

    s" r1x1x s10001 ( regci ( regc 0 0 (r0101)) (( regc 0 0 (rX101)) ( regc 0 0 (r0X01))))" string-to-stack
    invert abort" string-to-stack failed?"

    \ Test.
    dup is-regioncorrint? invert abort" string-to-stack failed?"
    over is-state? invert abort" string-to-stack failed?"
    #2 pick is-region? invert abort" string-to-stack failed?"

    \ Display
    cr ." regioncorrint test 3:   " dup .regioncorrint cr

    \ Deallocate.
    regioncorrint-deallocate    \ Test 3
    state-deallocate
    region-deallocate
    regioncorrint-deallocate    \ Test 2
    struct-list-deallocate      \ Test 1

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrint-test-from-string - Ok"
;

: regioncorrint-tests
    regioncorrint-test-from-string
    cr
;
