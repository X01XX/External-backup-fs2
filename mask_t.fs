\ Test mask functions.

: mask-test-basic
    \ Test mask-new.
    s" m0101" mask-from-string-a    \ msk

    \ Test .mask works.
    cr ." mask: " dup .mask         \ msk

    \ Test result.
    dup mask-get-number #5 =
    false? abort" result not as expected"

    \ Test mask-deallocate.
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-basic - Ok"
;

: mask-test-eq
    s" m000101" mask-from-string-a  \ msk5
    s" m000100" mask-from-string-a  \ msk5 msk4
    s" m000101" mask-from-string-a  \ msk5 msk4 msk5b

    #2 pick over masks-eq?          \ msk5 msk4 msk5b bool
    false? abort" masks not eq?"

    2dup masks-eq?                  \ msk5 msk4 msk5b bool
    abort" masks  eq?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-eq - Ok"
;

: mask-test-bit
    s" m0101" mask-from-string-a    \ msk5
    #3 over mask-bit 0<> abort" Isvalid bit value?"
    #2 over mask-bit 1 <> abort" Isvalid bit value?"
    1 over mask-bit 0<> abort" Isvalid bit value?"
    0 over mask-bit 1 <> abort" Isvalid bit value?"

    \ Clean up.
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-bit - Ok"
;

: mask-test-and
    s" m0101" mask-from-string-a    \ msk5
    s" m0110" mask-from-string-a    \ msk5 msk6
    2dup                            \ msk5 msk6 msk5 msk6
    mask-and                        \ msk5 msk6 msk56

    s" m0100" mask-from-string-a    \ msk5 msk6 m-k56 msk4
    2dup masks-eq?                  \ msk5 msk6 m-k56 msk4 bool
    false? abort" mask and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-and - Ok"
;

: mask-test-invert
    s" m0101" mask-from-string-a    \ msk5
    dup mask-invert                 \ msk5 msk~5
    s" m1010" mask-from-string-a    \ msk5 msk~5 msk10

    2dup masks-eq?                  \ msk5 msk~5 msk10 bool
    false? abort" mask ne 10?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-invert - Ok"
;

: mask-test-dif-num-bits?
    s" m0101" mask-from-string-a    \ msk54
    s" m0100" mask-from-string-a    \ msk54 msk44
    s" m101" mask-from-string-a     \ msk54 msk44 msk53

    2dup masks-dif-num-bits?        \ msk54 msk44 msk53 bool
    invert abort" masks have same num bits?"

    #2 pick #2 pick                 \ msk54 msk44 msk53 msk54 msk44
    masks-dif-num-bits?             \ msk54 msk44 msk53 bool
    abort" masks don't have the same num bits?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." mask-test-same-num-bits - Ok"
;

: mask-tests
    mask-test-basic
    mask-test-eq
    mask-test-bit
    mask-test-and
    mask-test-invert
    mask-test-dif-num-bits?
;
