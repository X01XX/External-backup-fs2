\ Test sample functions.

: sample-test-basic

    \ Test sample-new.
    s" s0101" state-from-string-a   \ sta
    s" s0110" state-from-string-a   \ sta sta
    sample-new                      \ smp

    \ Test .sample works.
    cr ." sample: " dup .sample     \ smp

    \ Test initial.
    dup sample-get-initial          \ smp initial
    state-get-number                \ smp num
    #6 =
    false? abort" Result not as expected"

    \ Test result.
    dup sample-get-result           \ smp result
    state-get-number                \ smp num
    #5 =
    false? abort" Result not as expected"

    \ Test sample-deallocate.
    sample-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." sample-test-basic - Ok"
;

: sample-tests
    sample-test-basic
;
