\ Test domain functions.

: domain-test-new

    4 0 0 domain-new            \ dom

    \ Clean up.
    domain-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." domain-test-new - Ok"
;

: domain-tests
    domain-test-new
    cr
;
