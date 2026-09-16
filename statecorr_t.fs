\ Test statecorr functions.

: statecorr-test-from-string
    s" (stac (s0100 s1010))" list-from-string       \ lst t | f
    invert abort" statecorr-test-new: list-from-string: string not parsed"

    \ Test.
    dup list-get-first-item is-statecorr? invert abort" list-from-string failed?"
    dup list-get-first-item statecorr-get-list list-get-length #2 <> abort" list len not 2?"

    struct-list-deallocate

    s" (stac (s0100 s1010))" string-to-stack        \ x* t | f
    invert abort" statecorr-test-new: string-to-stack: string not parsed"

    \ Test (uses list-from-string).
    dup is-statecorr? invert abort" string-to-stack failed?"

    statecorr-deallocate

    s" (stac (s0100 s1010))" statecorr-from-string        \ stac t | f
    invert abort" statecorr-test-new: statecorr-from-string: statecorr not parsed"

    \ Test (uses list-from-string).
    dup is-statecorr? invert abort" statecorr-from-string failed?"

    statecorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." statecorr-test-from-string - Ok"
;

: statecorr-test-distance
    s" (stac (s0000 s00000)) (stac (s1000 s10101))" string-to-stack-a

    \ cr dup .statecorr space ." vs " over .statecorr

    2dup statecorr-distance            \ stac1 stac0 u

    \ space ." distance = " dup dec. cr

    \ Test.
    #4 <> abort" distance not 4?"

    \ Deallocate.
    statecorr-deallocate
    statecorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." statecorr-test-distance - Ok"
;

: statecorrs-test-eq?
    s" (stac (s0000 s00000)) (stac (s1000 s10101))" string-to-stack-a

    \ cr dup .statecorr space ." eq? " over .statecorr

    2dup statecorrs-eq?             \ stac1 stac0 bool
    abort" statecorrs eq?"

    \ Deallocate.
    statecorr-deallocate
    statecorr-deallocate

    s" (stac (s0001 s00000)) (stac (s0001 s00000))" string-to-stack-a

    \ cr dup .statecorr space ." eq? " over .statecorr

    2dup statecorrs-eq?             \ stac1 stac0 bool
    invert abort" statecorrs neq?"

    \ Deallocate.
    statecorr-deallocate
    statecorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." statecorr-test-eq? - Ok"
;

: statecorr-tests
    statecorr-test-from-string
    statecorrs-test-eq?
    statecorr-test-distance
    cr
;
