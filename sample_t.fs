\ Test sample functions.

: sample-test-basic

    \ Test sample-new.
    #5 #4 state-new             \ sta
    #6 #4 state-new             \ sta sta
    sample-new                  \ smp

    \ Test .sample works.
    cr ." sample: " dup .sample \ smp

    \ Test sample-str produces the expected output.
    pad 1+ over                 \ smp pad+ smp
    sample-str                  \ smp nc
    pad c!                      \ smp
    pad string@                 \ smp c-addr cnt
    s" (s0110->s0101)"          \ smp c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test sample-deallocate.
    sample-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." sample-test-basic - Ok"
;

: sample-tests
    sample-test-basic
;
