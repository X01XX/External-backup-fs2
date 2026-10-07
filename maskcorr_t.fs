\ Test maskcorr functions.

: maskcorr-test-from-string
    s" (mskc (m0100 m1010))" list-from-string       \ lst t | f
    invert abort" maskcorr-test-new: list-from-string: string not parsed"

    \ Test.
    dup list-get-first-item is-maskcorr? invert abort" list-from-string failed?"
    dup list-get-first-item maskcorr-get-list list-get-length #2 <> abort" list len not 2?"

    struct-list-deallocate

    s" (mskc (m0100 m1010))" string-to-stack        \ x* t | f
    invert abort" maskcorr-test-new: string-to-stack: string not parsed"

    \ Test (uses list-from-string).
    dup is-maskcorr? invert abort" string-to-stack failed?"

    maskcorr-deallocate

    s" (mskc (m0100 m1010))" maskcorr-from-string        \ stac t | f
    invert abort" maskcorr-test-new: maskcorr-from-string: maskcorr not parsed"

    \ Test (uses list-from-string).
    dup is-maskcorr? invert abort" maskcorr-from-string failed?"

    maskcorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." maskcorr-test-from-string - Ok"
;

: maskcorr-test-num-bits-set
    s" (mskc (m1000 m10101))" maskcorr-from-string-a

    \ cr dup .maskcorr cr

    dup maskcorr-num-bits-set       \ mskc0 u

    \ space ." num bits set = " dup dec. cr

    \ Test.
    #4 <> abort" number bits set not 4?"

    \ Deallocate.
    maskcorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." maskcorr-test-num-bits-set - Ok"
;

: maskcorrs-test-eq?
    s" (mskc (m0000 m00000)) (mskc (m1000 m10101))" string-to-stack-a

    \ cr dup .maskcorr space ." eq? " over .maskcorr

    2dup maskcorrs-eq?             \ mskc1 mskc0 bool
    abort" maskcorrs eq?"

    \ Deallocate.
    maskcorr-deallocate
    maskcorr-deallocate

    s" (mskc (m0001 m00000)) (mskc (m0001 m00000))" string-to-stack-a

    \ cr dup .maskcorr space ." eq? " over .maskcorr

    2dup maskcorrs-eq?             \ mskc1 mskc0 bool
    invert abort" maskcorrs neq?"

    \ Deallocate.
    maskcorr-deallocate
    maskcorr-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." maskcorr-test-eq? - Ok"
;

: maskcorr-tests
    maskcorr-test-from-string
    maskcorrs-test-eq?
    maskcorr-test-num-bits-set
    cr
;
