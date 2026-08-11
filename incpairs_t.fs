
: inc-pairs-test-priority-non-adjacent-pairs

    s" (rxX00 rxX01 r10xX rXX11)" region-list-from-string-a \ nadj-prs'
    s" (r010X r000X)" region-list-from-string-a \ nadj-lst' adj-lst'
    2dup                                        \ nadj-lst' adj-lst' nadj-lst' adj-lst'
    inc-pairs-priority-non-adjacent-pairs       \ nadj-lst' adj-lst', pri-prs' t | f
    invert abort" priority regions not found?"

    cr ." priority pairs: " dup .region-list

    \ Test.
    s" (rxX00 rxX01)" region-list-from-string-a \ nadj-lst' adj-lst' pri-prs' tst-prs'
    2dup region-lists-eq?                       \ nadj-lst' adj-lst' pri-prs' tst-prs' bool
    invert abort" pairs not ecpected?"

    \ Deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." inc-pairs-test-priority-non-adjacent-pairs - Ok"
;

: inc-pair-tests
    inc-pairs-test-priority-non-adjacent-pairs
    cr
;
