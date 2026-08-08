
: inc-pairs-test-priority-non-adjacent-needs

    s" (rxX00 r010X rxX01 r10xX rXX11 r000X)" region-list-from-string-a \ pr-lst'
    dup                                 \ pr-lst' pr-lst'
    inc-pairs-priority-non-adjacent-needs

    \ Deallocate
    region-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." inc-pairs-test-priority-non-adjacent-needs - Ok"
;

: inc-pair-tests
    inc-pairs-test-priority-non-adjacent-needs
    cr
;
