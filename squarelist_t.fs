
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
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-between-any - Ok"
;

: square-list-test-find-incompatible-pair
\    s" (r1010 (s1000) m1010)" list-from-string-a   \ lst
\    structinfo-list-deallocate-struct-xt           \ lst xt
\    swap                                           \ xt lst
\    list-deallocate-recursive-struct
\
\ Or
\   structinfo-list-deallocate-struct-list
\
\     structinfo-list-store structinfo-list-project-deallocated
\     exit

    \ Init square-list.
    list-new                        \ lst

    \ Create a pn2 square.
    s" s0010->s1111" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr2
    s" s0010->s1110" sample-from-string-a   \ lst sqr2 smpl
    over square-add-sample                  \ lst sqr2 bool
    \ Check for change to pn.
    if else ." add sample 1 failed?" abort then

    \ Add square to list. Save ref for testing results.
    2dup swap list-push-struct              \ lst sqr2
    swap                                    \ sqr2 lst

    \ Create a pn1 square.
    s" s0110->s1111" sample-from-string-a   \ sqr2 lst smpl
    square-new                              \ sqr2 lst sqr1a

    \ Add square to list.
    2dup swap list-push-struct              \ sqr2 lst sqr1a
    swap                                    \ sqr2 sqr1a lst

    \ Create a pn1 square, apparently incompatible with sqr1a
    \ 01/11/11/0X.
    s" s1110->s1110" sample-from-string-a   \ sqr2 lst smpl
    square-new                              \ sqr2 lst sqr1b

    \ Add square to list. Save ref for testing results.
    2dup swap list-push-struct              \ sqr2 lst sqr1b
    swap                                    \ sqr2 sqr1a sqr1b lst

    \ cr ." list: " dup .square-list cr

    \ sqr1a and sqr1b are incompatible with each other, but not with
    \ the higher-pn-level sqr2.
    dup square-list-find-incompatible-pair \ sqr2 sqr1a sqr1b lst, sqr-pr t | f
    if
        cr ." incompatible pairs?: " dup .square-list cr
        abort
    then

    \ Make sqr1b incompatible with sqr2.
    s" s1110->s1110" sample-from-string-a   \ sqr2 sqr1a sqr1b lst smpl
    #2 pick square-add-sample               \ sqr2 sqr1a sqr1b lst bool
    \ Check for no change to pn.
    if ." add sample 2 failed?" abort then

    dup square-list-find-incompatible-pair  \ sqr2 sqr1a sqr1b lst, sqr-pr t | f
    if
        \ cr ." incompatible pairs: " dup .square-list cr

        \ Check square pair contains sqr2.
        [ ' = ] literal                     \ sqr2 sqr1a sqr1b lst sqr-pr xt
        #5 pick                             \ sqr2 sqr1a sqr1b lst sqr-pr xt sqr2
        #2 pick                             \ sqr2 sqr1a sqr1b lst sqr-pr xt sqr2 sqr-pr
        list-member?
        if
        else
            cr ." sqr2 not found?" abort
        then

        \ Check square pair contains sqr1b.
        [ ' = ] literal                     \ sqr2 sqr1a sqr1b lst sqr-pr xt
        #3 pick                             \ sqr2 sqr1a sqr1b lst sqr-pr xt sqr1b
        #2 pick                             \ sqr2 sqr1a sqr1b lst sqr-pr xt sqr1b sqr-pr
        list-member?
        if
        else
            cr ." sqr1b not found?" abort
        then
    else
        cr ." no incompatible pairs?" abort
    then
    
    \ Deallocate.                       \ sqr2 sqr1a sqr1b lst sqr-pr
    square-list-deallocate              \ sqr2 sqr1a sqr1b lst
    nip nip nip                         \ lst
    square-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-find-incompatible-pair - Ok"
;

: square-list-tests
    square-list-test-any-between?
    square-list-test-between-any
    square-list-test-find-incompatible-pair
    cr
;
