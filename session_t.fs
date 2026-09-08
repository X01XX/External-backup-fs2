
: session-test-new
    session-new                     \ sess

    #4 over session-add-domain      \ sess dom
    drop

    #6 over session-add-domain      \ sess dom
    drop

    \ Add valued regioncorrs.
    s" ( regc 0  -1 (r01XX r0000XX))" string-to-stack-a
    over _session-add-valued-regioncorr

    s" ( regc 0  -1 (r0X1X r000X1X))" string-to-stack-a
    over _session-add-valued-regioncorr

    dup session-init-after-domains  \ sess

    cr dup .session cr

    \ Deallocate.
    session-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." session-test-new - Ok"
;

: session-tests
    session-test-new
    cr
;
