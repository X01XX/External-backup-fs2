
: need-test-new
    \ Try with state target.
    s" 0 s1000 1 1 2" string-to-stack           \ tkn targ ned-typ act-id dom-id
    need-new                                    \ ned

    cr ." need 1: " dup .need cr

    \ Test.
    dup need-get-dom-inst-id 2 <> abort" domain inst id ne 2?"
    dup need-get-act-inst-id 1 <> abort" action inst id ne 1?"
    dup need-get-target is-state? invert abort" target not a state?"
    dup need-get-info 0<> abort" action info s/b zero?"

    \ Try new with region-list target.
    s" 0 (r1100 r1000) 1 1 2" string-to-stack           \ tkn targ ned-typ act-id dom-id
    need-new                                    \ ned

    cr ." need 3: " dup .need cr

    \ Test.
    dup need-get-dom-inst-id 2 <> abort" domain inst id ne 2?"
    dup need-get-act-inst-id 1 <> abort" action inst id ne 1?"
    dup need-get-target is-region-list? invert abort" target not a region?"
    dup need-get-info 0<> abort" action info s/b zero?"

    \ Deallocate.
    need-deallocate
    need-deallocate
    need-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." need-test-new - Ok"
;

: need-tests
    need-test-new
    cr
;
