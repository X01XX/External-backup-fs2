
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

: session-test-make-plan
    \ Make valued-regioncorr list.
    list-new                        \ regc-lst
    s" ( regc 0  -1 (r01XX r00000))" string-to-stack-a over list-push-struct
    s" ( regc 0  -1 (r0X1X r00000))" string-to-stack-a over list-push-struct

    \ Make domain-list.
    list-new                        \ regc-lst dom-lst

    \ Make domain.
    #4 domain-new                   \ regc-lst dom-lst dom

    \ Add actions to domain.
    [ ' dom-0-act1-get-result ] literal over domain-add-action
    [ ' dom-0-act2-get-result ] literal over domain-add-action
    [ ' dom-0-act3-get-result ] literal over domain-add-action
    [ ' dom-0-act4-get-result ] literal over domain-add-action

    \ Pre-load action 1.
    1 over domain-find-action invert abort" act not found?" \ regc-lst dom-lst dom act1
    s" s0000->s0001" sample-from-string-a over action-add-sample invert abort" sample not added?"
    s" s1111->s1110" sample-from-string-a over action-add-sample invert abort" sample not added?"
    drop

    \ Pre-load action 2.
    #2 over domain-find-action invert abort" act not found?" \ regc-lst dom-lst dom act2
    s" s0000->s0010" sample-from-string-a over action-add-sample invert abort" sample not added?"
    s" s1111->s1101" sample-from-string-a over action-add-sample invert abort" sample not added?"
    drop

    \ Pre-load action 3.
    #3 over domain-find-action invert abort" act not found?" \ regc-lst dom-lst dom act3
    s" s0000->s0100" sample-from-string-a over action-add-sample invert abort" sample not added?"
    s" s1111->s1011" sample-from-string-a over action-add-sample invert abort" sample not added?"
    drop

    \ Pre-load action 4.
    #4 over domain-find-action invert abort" act not found?" \ regc-lst dom-lst dom act4
    s" s0000->s1000" sample-from-string-a over action-add-sample invert abort" sample not added?"
    s" s1111->s0111" sample-from-string-a over action-add-sample invert abort" sample not added?"
    drop

    \ Store domain.
    over domain-list-push-end       \ regc-lst dom-lst

    \ Make second domain.
    #5 domain-new                   \ regc-lst dom-lst dom
    over domain-list-push-end       \ regc-lst dom-lst

    session-new                     \ sess

    cr dup .session cr

    s" ( stac (s0001 s00001))" statecorr-from-string-a \ sess goal
    s" ( stac (s1111 s00001))" statecorr-from-string-a \ sess goal from

    2dup                                        \ sess goal from goal from
    #4 pick                                     \ sess goal from goal from sess
    session-make-plan                           \ sess goal from, pln t | f
    if
        cr ." plan found" cr
\        dup .plan
\        plan-deallocate
    else
        cr ." plan not found" cr
    then

    \ Check memory use.
    .memory-use

    \ Deallocate.
    cr ." Deallocating ..." cr
    statecorr-deallocate
    statecorr-deallocate
    session-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." session-test-make-plan - Ok"
;

: session-tests
    session-test-new
    session-test-make-plan
    cr
;
