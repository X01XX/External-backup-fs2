\ Test square functions.

: square-test-basic

    \ Test square-new.
    s" s0101->s0111" sample-from-string-a    \ smpl
    square-new                              \ sqr

    \ Test .square works.
    cr ." square: " dup .square     \ smp

    \ Test square-deallocate.
    square-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-test-basic - Ok"
;

: square-tests
    square-test-basic
;
