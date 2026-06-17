
: square-list-test-any-between?

    \ Init anchor
    s" s0101->s1111" sample-from-string-a   \ smpl
    square-new                              \ anc

    \ Init list.
    list-new                                \ anc lst

    \ Fill list.
    s" s0100->s1111" sample-from-string-a   \ anc lst smpl
    square-new                              \ anc lst sqr
    over list-push-struct                   \ anc lst

    s" s1111->s1111" sample-from-string-a   \ anc lst smpl
    square-new                              \ anc lst sqr
    over list-push-struct                   \ anc lst

    \ Test if 4 is between C and 5.
    s" s1100->s1111" sample-from-string-a   \ anc lst smpl
    square-new                              \ anc lst sqr-c
    #2 pick over #3 pick                    \ anc lst sqr-c anc sqr-c lst
    square-list-any-between?                \ anc lst sqr-c bool
    if
    else
        true abort" 4 is not between s0101 and s1100?"
    then

    \ Test nothing is between 9 and 5.
    s" s1001->s1111" sample-from-string-a   \ anc lst sqr-c smpl
    square-new                              \ anc lst sqr-c sqr-9
    #3 pick over #4 pick                    \ anc lst sqr-c sqr-9 anc sqr-9 lst
    square-list-any-between?                \ anc lst sqr-c sqr-9 bool
    if
        true abort" Something between 0101 and 1001?"
    then

    \ Deallocate.
    square-deallocate
    square-deallocate
    square-list-deallocate
    square-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-any-between? - Ok"
;

: square-list-tests
    square-list-test-any-between?
    cr
;
