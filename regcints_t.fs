\ Try three similar ways to convert a regcints string representation into a struct instance.
\ A wrinkle is that sub definitions of regioncorrs must be converted first.
\ regcints-from-string and string-to-stack use list-from-string in
\ slightly different ways.
\ A few extraneous structs, and a number, are added to exercise list-from-string a little more.
: regcints-test-from-string
    \     hint  intersection        list of regioncorr intersectors
    s" ( regcis ( regc 1 -2 (rX101)) (( regc 0 0 (r0101)) ( regc 0 0 (r1101)))) rx101  #55" list-from-string
    invert abort" list-from-string failed?"

    \ Test.
    dup is-list? invert abort" list-from string failed?"
    dup list-get-length #3 <> abort" list-from string failed?"
    dup list-get-first-item is-regcints? invert abort" list-from string failed?"
    dup list-get-second-item is-region? invert abort" list-from string failed?"
    dup list-get-third-item #55 <> abort" list-from string failed?"

    \ Display
    cr ." regcints test 1:  " dup .struct-list cr

    s" ( regcis ( regc 1 -2 (rX101)) (( regc 0 0 (r0101)) ( regc 0 0 (r1101))))" regcints-from-string
    invert abort" regcints-from-string failed?"

    \ Test.
    dup is-regcints? invert abort" regcints-from string failed?"

    \ Display
    cr ." regcints test 2:   " dup .regcints cr

    s" r1x1x s10001 ( regcis ( regc 1 -2 (rX101)) (( regc 0 0 (r0101)) ( regc 0 0 (r1101))))" string-to-stack
    invert abort" string-to-stack failed?"

    \ Test.
    dup is-regcints? invert abort" string-to-stack failed?"
    over is-state? invert abort" string-to-stack failed?"
    #2 pick is-region? invert abort" string-to-stack failed?"

    \ Display
    cr ." regcints test 3:   " dup .regcints cr

    \ Deallocate.
    regcints-deallocate    \ Test 3
    state-deallocate
    region-deallocate
    regcints-deallocate    \ Test 2
    struct-list-deallocate      \ Test 1

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regcints-test-from-string - Ok"
;


: regcints-tests
    regcints-test-from-string
    cr
;
