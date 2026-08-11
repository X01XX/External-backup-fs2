\ Test domain functions.

: domain-test-new

    \ Run function.
    #4 1 domain-new           \ dom

    \ Display results.
    cr dup .domain cr

    \ Test results.
    dup domain-get-actions
    list-get-length 1 <> abort" num actions s/b one?"

    dup domain-get-num-bits #4 <> abort" num bits s\b 4?"

    dup domain-get-inst-id 1 <> abort" id s/b 1?"

    \ Clean up.
    domain-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." domain-test-new - Ok"
;

: domain-test-add-action
    \ Run function.
    #4 0  domain-new                    \ dom

    [ ' dom-0-act2-get-result ] literal \ dom xt
    over                                \ dom xt dom
    domain-add-action                   \ dom

    \ Diplay results.
    cr dup .domain cr

    \ Test results.
    dup domain-get-actions
    list-get-length #2 <> abort" s/b two actions?"

    \ Clean up.
    domain-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." domain-test-add-action - Ok"
;

: domain-tests
    domain-test-new
    domain-test-add-action
    cr
;
