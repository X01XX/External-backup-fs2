\ Test mask functions.

: mask-test-basic
    \ Test mask-new.
    #5 #4 mask-new              \ msk

    \ Test .mask works.
    cr ." mask: " dup .mask     \ msk

    \ Test mask-str produces the expected output.
    pad 1+ over                 \ msk pad+ msk
    mask-str                    \ msk nc
    pad c!                      \ msk
    pad string@                 \ msk c-addr cnt
    s" m0101"                    \ msk c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test mask-deallocate.
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-basic - Ok"
;

: mask-test-eq
    #5 #6 mask-new              \ msk5
    #4 #6 mask-new              \ msk5 msk4
    #5 #6 mask-new              \ msk5 msk4 msk5b

    #2 pick over mask-eq        \ msk5 msk4 msk5b bool
    false? abort" masks not eq?"

    2dup mask-eq                \ msk5 msk4 msk5b bool
    abort" masks  eq?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-eq - Ok"
;

: mask-test-bit
    #5 #4 mask-new              \ msk5
    #3 over mask-bit 0<> abort" Isvalid bit value?"
    #2 over mask-bit 1 <> abort" Isvalid bit value?"
    1 over mask-bit 0<> abort" Isvalid bit value?"
    0 over mask-bit 1 <> abort" Isvalid bit value?"

    \ Clean up.
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-bit - Ok"
;

: mask-test-and
    #5 #4 mask-new          \ msk5
    #6 #4 mask-new          \ msk5 msk6
    2dup                    \ msk5 msk6 msk5 msk6
    mask-and                \ msk5 msk6 msk56

    #4 #4 mask-new          \ msk5 msk6 m-k56 msk4
    2dup mask-eq            \ msk5 msk6 m-k56 msk4 bool
    false? abort" mask and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-and - Ok"
;

: mask-test-invert
    #5 #4 mask-new          \ msk5
    dup mask-invert         \ msk5 msk~5
    #10 #4 mask-new         \ msk5 msk~5 msk10

    2dup mask-eq            \ msk5 msk~5 msk10 bool
    false? abort" mask ne 10?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-invert - Ok"
;

: mask-test-dif-num-bits?
    #5 #4 mask-new              \ msk54
    #4 #4 mask-new              \ msk54 msk44
    #5 #3 mask-new              \ msk54 msk44 msk53

    2dup mask-dif-num-bits?     \ msk54 msk44 msk53 bool
    invert abort" masks have same num bits?"

    #2 pick #2 pick             \ msk54 msk44 msk53 msk54 msk44
    mask-dif-num-bits?          \ msk54 msk44 msk53 bool
    abort" masks don't have the same num bits?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

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
