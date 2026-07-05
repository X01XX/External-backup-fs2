
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

    \ Init square-list.
    list-new                        \ lst

    \ Create a pn2 square.
    s" s0010->s1111" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr2
    s" s0010->s1110" sample-from-string-a   \ lst sqr2 smpl
    over square-add-sample                  \ lst sqr2 bool
    \ Check for change to pn.
    if else ." add sample 1 did not cause change?" abort then

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
    if else ." add sample 2 caused no change?" abort then

    dup square-list-find-incompatible-pair  \ sqr2 sqr1a sqr1b lst, sqr-pr t | f
    if
        \ cr dup .list-raw cr
        \ cr ." incompatible pair: " dup .square-list cr

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
    \ cr ." at end: " .stack-gbl cr
    \ cr .s cr
    square-list-deallocate              \ sqr2 sqr1a sqr1b lst
    square-list-deallocate              \ sqr2 sqr1a sqr1b
    2drop drop

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-find-incompatible-pair - Ok"
;

\ Find incompatible pair based on min distance.
: square-list-test-find-incompatible-pair-dis
    \ Init square-list.
    list-new                        \ lst

    \ Create a pn2 square.
    s" s0100->s0100" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr2a
    s" s0100->s0000" sample-from-string-a   \ lst sqr2a smpl
    over square-add-sample                  \ lst sqr2a bool
    \ Check for change to pn.
    if else ." add sample 1 did not cause change?" abort then
    dup #2 pick list-push-struct            \ lst sqr2a
    swap                                    \ sqr2a lst

    \ Create another pn2 square.
    s" s0111->s0111" sample-from-string-a   \ sqr2a lst smpl
    square-new                              \ sqr2a lst sqr2b
    s" s0111->s0011" sample-from-string-a   \ sqr2a lst sqr2b smpl
    over square-add-sample                  \ sqr2a lst sqr2b bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then
    swap                                    \ sqr2a sqr2b lst
    2dup list-push-struct                   \ sqr2a sqr2b lst

    \ Make an incompatible pn1 square.
    s" s1011->s1011" sample-from-string-a   \ sqr2a sqr2b lst smpl
    square-new                              \ sqr2a sqr2b lst sqr1
    s" s1011->s1011" sample-from-string-a   \ sqr2a sqr2b lst sqr1 smpl
    over square-add-sample                  \ sqr2a sqr2b lst sqr1 bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then
    swap                                    \ sqr2a sqr2b sqr1 lst
    2dup list-push-struct                   \ sqr2a sqr2b sqr1 lst

    \ cr ." at 1: " .stack-gbl cr
    dup square-list-find-incompatible-pair  \ sqr2a sqr2b sqr1 lst, sqr-pr t | f
    \ cr ." at 2: " .stack-gbl cr
    if
        \ cr ." incompatible pair: " dup .square-list cr
        \ cr dup .list-raw cr
        \ cr .s cr
    else
        cr ." no incompatible pairs?" abort
    then

    \ exit 3 was confirmed.
    \ cr ." At 33: " .stack-gbl cr
    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    \ cr ." At 44: " .stack-gbl cr
    #4 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2b
    \ cr ." sqr2b: " dup .square cr
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2b sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr2b not found?" abort
    then

    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    #3 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1 sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr1 not found?" abort
    then

    \ Deallocate.                           \ sqr2a sqr2b sqr1 lst sqr-pr
    square-list-deallocate
    nip nip nip
    square-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-find-incompatible-pair-dis - Ok"
;

\ Find incompatible pair based on max number samples.
: square-list-test-find-incompatible-pair-ns
    \ Init square-list.
    list-new                        \ lst

    \ Create a pn2 square.
    s" s0100->s0100" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr2a
    s" s0100->s0000" sample-from-string-a   \ lst sqr2a smpl
    over square-add-sample                  \ lst sqr2a bool
    \ Check for change to pn.
    if else ." add sample 1 did not cause change?" abort then

    dup #2 pick list-push-struct            \ lst sqr2a
    swap                                    \ sqr2a lst

    \ Create another pn2 square.
    s" s0111->s0111" sample-from-string-a   \ sqr2a lst smpl
    square-new                              \ sqr2a lst sqr2b
    s" s0111->s0011" sample-from-string-a   \ sqr2a lst sqr2b smpl
    over square-add-sample                  \ sqr2a lst sqr2b bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then

    \ Add another sample.
    s" s0111->s0111" sample-from-string-a   \ sqr2a lst sqr2b smpl
    over square-add-sample                  \ sqr2a lst sqr2b bool
    \ Check for change to pn.
    if ." add sample 3 caused change?" abort then

    swap                                    \ sqr2a sqr2b lst
    2dup list-push-struct                   \ sqr2a sqr2b lst

    \ Make an incompatible pn1 square.
    s" s1101->s1101" sample-from-string-a   \ sqr2a sqr2b lst smpl
    square-new                              \ sqr2a sqr2b lst sqr1
    s" s1101->s1101" sample-from-string-a   \ sqr2a sqr2b lst sqr1 smpl
    over square-add-sample                  \ sqr2a sqr2b lst sqr1 bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then
    swap                                    \ sqr2a sqr2b sqr1 lst
    2dup list-push-struct                   \ sqr2a sqr2b sqr1 lst

    dup square-list-find-incompatible-pair  \ sqr2a sqr2b sqr1 lst, sqr-pr t | f
    if
        \ cr ." incompatible pair: " dup .square-list cr
    else
        cr ." no incompatible pairs?" abort
    then

    \ cr ." pair: " dup .square-list cr

\ exit 4 was confirmed. 0111 is different that the arbitrary result of 0100 in
\ square-list-test-find-incompatible-pair-ns2
    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    #4 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2b
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2b sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr2b not found?" abort
    then

    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    #3 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1 sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr1 not found?" abort
    then

    \ Deallocate.                           \ sqr2a sqr2b sqr1 lst sqr-pr
    square-list-deallocate
    nip nip nip
    square-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-find-incompatible-pair-ns - Ok"
;

\ Find incompatible pair based on max number samples, arbitray choice.
: square-list-test-find-incompatible-pair-ns2
    \ Init square-list.
    list-new                        \ lst

    \ Create a pn2 square.
    s" s0100->s0100" sample-from-string-a   \ lst smpl
    square-new                              \ lst sqr2a
    s" s0100->s0000" sample-from-string-a   \ lst sqr2a smpl
    over square-add-sample                  \ lst sqr2a bool
    \ Check for change to pn.
    if else ." add sample 1 did not cause change?" abort then
    dup #2 pick list-push-struct            \ lst sqr2a
    swap                                    \ sqr2a lst

    \ Create another pn2 square.
    s" s0111->s0111" sample-from-string-a   \ sqr2a lst smpl
    square-new                              \ sqr2a lst sqr2b
    s" s0111->s0011" sample-from-string-a   \ sqr2a lst sqr2b smpl
    over square-add-sample                  \ sqr2a lst sqr2b bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then
    swap                                    \ sqr2a sqr2b lst
    2dup list-push-struct                   \ sqr2a sqr2b lst

    \ Make an incompatible pn1 square.
    s" s1101->s1101" sample-from-string-a   \ sqr2a sqr2b lst smpl
    square-new                              \ sqr2a sqr2b lst sqr1
    s" s1101->s1101" sample-from-string-a   \ sqr2a sqr2b lst sqr1 smpl
    over square-add-sample                  \ sqr2a sqr2b lst sqr1 bool
    \ Check for change to pn.
    if else ." add sample 2 did not cause change?" abort then
    swap                                    \ sqr2a sqr2b sqr1 lst
    2dup list-push-struct                   \ sqr2a sqr2b sqr1 lst

    dup square-list-find-incompatible-pair  \ sqr2a sqr2b sqr1 lst, sqr-pr t | f
    if
        \ cr ." incompatible pair: " dup .square-list cr
    else
        cr ." no incompatible pairs?" abort
    then

    \ cr ." pair: " dup .square-list cr
\ Square 0100 is in the pair, but thats arbitrary, the first pair in the list of pairs.
\ exit 5 was confirmed.
    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    #5 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2a
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr2a sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr2a not found?" abort
    then

    [ ' = ] literal                         \ sqr2a sqr2b sqr1 lst sqr-pr xt
    #3 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1
    #2 pick                                 \ sqr2a sqr2b sqr1 lst sqr-pr xt sqr1 sqr-pr
    list-member?                            \ sqr2a sqr2b sqr1 lst sqr-pr bool
    if
    else
        cr ." sqr1 not found?" abort
    then

    \ Deallocate.                           \ sqr2a sqr2b sqr1 lst sqr-pr
    square-list-deallocate
    square-list-deallocate
    2drop drop

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-list-test-find-incompatible-pair-ns2 - Ok"
;

: square-list-tests
    square-list-test-any-between?
    square-list-test-between-any
    square-list-test-find-incompatible-pair
    square-list-test-find-incompatible-pair-dis
    square-list-test-find-incompatible-pair-ns
    square-list-test-find-incompatible-pair-ns2
    cr
;
