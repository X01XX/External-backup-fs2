
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
    ifnot
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
     structinfo-list-store-project-deallocated

    cr ." square-list-test-any-between? - Ok"
;

: square-list-test-between-any
    \ Init anchor
    s" s0101->s1111" sample-from-string-a   \ smpl
    square-new                              \ anc

    \ Init list.
    list-new                                \ anc lst

    \ Fill list.
    s" s0010->s1111" sample-from-string-a   \ anc lst smpl
    square-new                              \ anc lst sqr2
    swap                                    \ anc sqr2 lst
    2dup list-push-struct                   \ anc sqr2 lst

    s" s1110->s1111" sample-from-string-a   \ anc sqr2 lst smpl
    square-new                              \ anc sqr2 lst sqr
    over list-push-struct                   \ anc sqr2 lst

    \ Test if 3 is between 5 and 2, but not E.
    s" s0011->s1111" sample-from-string-a   \ anc sqr2 lst smpl
    square-new                              \ anc sqr2 lst sqr-c
    #3 pick over #3 pick                    \ anc sqr2 lst sqr-c anc sqr-c lst
    square-list-between-any                 \ anc sqr2 lst sqr-c lst
    cr ." between: 3 is 5 and " dup .square-list-states cr

    dup list-get-length 1 <> abort" list does not have exactly one item?"
    dup list-get-first-item                 \ anc sqr2 lst sqr-c lst itm1
    #4 pick                                 \ anc sqr2 lst sqr-c lst itm1 sqr2
    <> abort" list item is not square 2?"

    \ Deallocate.
    square-list-deallocate
    square-deallocate
    square-list-deallocate
    drop    \ two square-list-deallocates, above, deallocated square 2.
    square-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-list-test-between-any - Ok"
;

\ Test finding adjacent, and non-adjacent, square pairs.
: square-list-test-find-incompatible-pairs

    \ Init square-list.
    list-new                                \ lst

    \ Create square 5.
    s" s0101->s0101" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr5

    \ Add square to list. Save ref for testing results.
    over list-push-struct                   \ lst

    \ Create adjacent, incompatible, to square 5, square 7.
    s" s0111->s0110" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr7

    \ Add square to list.
    over list-push-struct                   \ lst

    \ Create non-adjacent, incompatible, to square 5, square C.
    s" s1100->s1000" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqrC

    \ Add square to list.
    over list-push-struct                   \ lst

    \ Find sqr5 sqr7 pair.
    dup square-list-find-adj-incompatible-pairs \ lst, sqr-pr t | f
    if
        cr ." found " dup .region-list cr
        dup list-get-first-item                 \ lst regx
        region-states-adjacent?                 \ lst bool
        invert abort" states not adjacent?"
        region-list-deallocate
    then

    \ Find sqr5 sqrC pair.
    dup square-list-find-nadj-incompatible-pairs \ lst, sqr-pr t | f
    if
        cr ." found " dup .region-list cr
        dup list-get-first-item                 \ lst regx
        region-states-adjacent?                 \ lst bool
        abort" states adjacent?"
        region-list-deallocate
    then

    \ Deallocate.                       \ lst
    square-list-deallocate              \

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-list-test-find-incompatible-pair - Ok"
;

: square-list-tests
    square-list-test-any-between?
    square-list-test-between-any
    square-list-test-find-incompatible-pairs
    cr
;
