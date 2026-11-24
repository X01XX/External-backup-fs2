
: cornertemplate-test-from-string
    s" (ct r0011  (r0x11 r00x1))" list-from-string       \ lst t | f
    invert abort" cornertemplate-test-from-string: list-from-string: string not parsed"

    \ Test.
    dup list-get-first-item is-cornertemplate? invert abort" list-from-string failed?"
    dup list-get-length 1 <> abort" list len not one?"

    struct-list-deallocate

    s" (ct r0011  (r0x11 r00x1))" string-to-stack        \ x* t | f
    invert abort" cornertemplate-test-new: string-to-stack: string not parsed"

    \ Test (uses list-from-string).
    dup is-cornertemplate? invert abort" string-to-stack failed?"

    cornertemplate-deallocate

    s" (ct r0011  (r0x11 r00x1))" cornertemplate-from-string        \ regc t | f
    invert abort" cornertemplate-test-new: cornertemplate-from-string: cornertemplate not parsed"

    \ Test (uses list-from-string).
    dup is-cornertemplate? invert abort" cornertemplate-from-string failed?"

    \ Deallocate.
    cornertemplate-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." cornertemplate-test-from-string - Ok"
;

: cornertemplate-tests
    cornertemplate-test-from-string
;
