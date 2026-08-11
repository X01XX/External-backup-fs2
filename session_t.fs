
: session-test-new
    \ Run function.
    session-new                 \ sess

    \ Display results.
    cr dup .session cr

    #4 over session-add-domain  \ sess dom
    drop

    \ Display results.
    cr dup .session cr

    \ Test results.

    \ Clean up.
    session-deallocate

    \ Check for memory leaks.
    structinfo-list-store-project-deallocated

    cr ." session-test-new - Ok"
;

: session-tests
    session-test-new
    cr
;
