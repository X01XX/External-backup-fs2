
: inc-pairs-test-priority-non-adjacent-pairs

    s" (rxX00 r010X rxX01 r10xX rXX11 r000X)" region-list-from-string-a \ pr-lst'
    dup                                         \ pr-lst' pr-lst'
    inc-pairs-priority-non-adjacent-pairs       \ pr-lst', pri-prs' t | f
    invert abort" priority regions not found?"

    cr ." priority pairs: " dup .region-list

    \ Test.
    s" (rxX00 rxX01)" region-list-from-string-a \ pr-lst' pri-prs' tst-prs'
    2dup region-lists-eq?                       \ pr-lst' pri-prs' tst-prs' bool
    invert abort" pairs not ecpected?"

    \ Deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." inc-pairs-test-priority-non-adjacent-pairs - Ok"
;

: inc-pair-tests
    inc-pairs-test-priority-non-adjacent-pairs
    cr
;
