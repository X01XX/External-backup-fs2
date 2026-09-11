
: session-test-new
    \ Make valued-regioncorr list.
    list-new                        \ regc-lst
\    s" ( regc 0  -1 (r01XX r01X))" string-to-stack-a over list-push-struct
\    s" ( regc 0  -1 (r0X1X r0X0))" string-to-stack-a over list-push-struct
   s" ( regc 0  -1 (r01XX))" string-to-stack-a over list-push-struct
   s" ( regc 0  -1 (r0X1X))" string-to-stack-a over list-push-struct

    \ Make domain-list.
    list-new                        \ regc-lst dom-lst
    #4 domain-new                   \ regc-lst dom-lst dom
    over domain-list-push-end       \ regc-lst dom-lst
\    #3 domain-new                   \ regc-lst dom-lst dom
\    over domain-list-push-end       \ regc-lst dom-lst

    session-new                     \ sess

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
