
: changes-test-new
    s" m0110 m1010" string-to-stack \ m10 m01
    invert abort" string-to-stack failed?"

    changes-new                     \ cngs
    cr ." changes: " dup .changes cr

    changes-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." changes-test-new - Ok"
;

: changes-tests
    changes-test-new
    cr
;
