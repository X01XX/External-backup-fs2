\ Test square functions.

: square-test-basic

    \ Test square-new.
    s" s0101->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr
    cr ." square 1 smpl: " dup .square             \ sqr

    \ Test square.
    dup square-get-pn 1 <> abort" pn ne 1?"
    dup square-get-pnc abort" pnc true?"
    s" (00/11/01/11/)" rule-list-from-string-a  \ sqr rul-lst'
    over square-get-rules                       \ sqr rul-lst' rul-lst
    over                                        \ sqr rul-lst' rul-lst rul-lst'
    rule-lists-eq?                              \ sqr rul-lst' bool
    false? abort" rule lists ne?"
    rule-list-deallocate                        \ rul

    \ Add a sample 2.
    s" s0101->s0111" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    invert abort" square did not change?"
    cr ." square 2 smpl: " dup .square

    \ Add a sample 3.
    s" s0101->s0111" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    abort" square changed?"
    cr ." square 3 smpl: " dup .square             \ sqr

    \ Add a sample 4.
    s" s0101->s0111" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    if
        dup square-get-pn 1 <> abort" pn ne 1?"
        dup square-get-pnc false? abort" pnc false?"
    else
        true abort" square did not change?"
    then
    cr ." square 4 smpl: " dup .square             \ sqr

    \ Add a sample 5.
    s" s0101->s0101" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    if
        dup square-get-pn 0 <> abort" pn ne 0?"
        dup square-get-pnc false? abort" pnc false?"
        dup square-get-rules list-is-empty? false? abort" rule list not empty?"
    else
        true abort" square did not change?"
    then
    cr ." square 5 smpl: " dup .square             \ sqr

    \ Add a sample 6.
    s" s0101->s0111" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    if
        true abort" square changed?"
    else
        dup square-get-pn 0 <> abort" pn ne 0?"
        dup square-get-pnc false? abort" pnc false?"
        dup square-get-rules list-is-empty? false? abort" rule list not empty?"
    then
    cr ." square 6 smpl: " dup .square             \ sqr

    \ Add a sample 7.
    s" s0101->s0101" sample-from-string-a   \ sqr smpl
    over square-add-sample                  \ sqr bool
    if
        dup square-get-pn #2 <> abort" pn ne 2?"
        dup square-get-pnc false? abort" pnc false?"
        s" (00/11/00/11/ 00/11/01/11/)" rule-list-from-string-a \ sqr rul-lst'
        over square-get-rules                                   \ sqr rul-lst' rul-lst
        over                                                    \ sqr rul-lst' rul-lst rul-lst'
        rule-lists-eq?                                          \ sqr rul-lst' bool
        false? abort" rule lists ne?"
        rule-list-deallocate
    else
        true abort" square did not change?"
    then
    cr ." square 7 smpl: " dup .square             \ sqr

    \ Test square-deallocate.
    square-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-test-basic - Ok"
;

: squares-test-compare-pnx-pn0
    \ Make a pn1 square.
    s" s0100->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr-pn1
    cr ." square pn1: " dup .square         \ sqr-pn1

    \ Make a pn0 square.
    s" s0101->s0111" sample-from-string-a   \ sqr-pn1 smpl
    square-new                              \ sqr-pn1 sqr-pn0
    s" s0101->s0101" sample-from-string-a   \ sqr-pn1 sqr-pn0 smpl
    over square-add-sample drop             \ sqr-pn1 sqr-pn0
    s" s0101->s0100" sample-from-string-a   \ sqr-pn1 sqr-pn0 smpl
    over square-add-sample drop             \ sqr-pn1 sqr-pn0
    cr ." square pn0: " dup .square         \ sqr-pn1 sqr-pn0

    \ Compare.
    2dup squares-compare-pnx-pn0            \ sqr-pn1 sqr-pn0 char
    [char] M =
    if
        cr ." compare pn1 pn0 to M - Ok"
    else
        true abort" comparison not M?"
    then

    \ Change sqr-pn1 to pnc == true.
    s" s0100->s0111" sample-from-string-a   \ sqr-pn1 sqr-pn0 smpl
    #2 pick square-add-sample drop          \ sqr-pn1 sqr-pn0
    s" s0100->s0111" sample-from-string-a   \ sqr-pn1 sqr-pn0 smpl
    #2 pick square-add-sample drop          \ sqr-pn1 sqr-pn0
    s" s0100->s0111" sample-from-string-a   \ sqr-pn1 sqr-pn0 smpl
    #2 pick square-add-sample drop          \ sqr-pn1 sqr-pn0

    \ Compare.
    2dup squares-compare-pnx-pn0            \ sqr-pn1 sqr-pn0 char
    [char] I =
    if
        cr ." compare pn1 pn0 to I - Ok"
    else
        true abort" comparison not I?"
    then

    \ Clean up.
    square-deallocate
    square-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-test-compare-pnx-pn0 - Ok"
;

: squares-test-compare-pn1-pn1

    \ Create pn1 square.
    s" s0100->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr1

    \ Create a compatible second square.
    s" s0101->s0111" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2

    \ Test compatibility.
    2dup                                    \ sqr1 sqr2 sqr1 sqr2
    squares-compare-pn1-pn1                 \ sqr1 sqr2 C
    [char] C =
    if
        cr ." square-test-compare-pn1-pn1 = C - Ok"
        square-deallocate
    else
        true abort" pn1 pn1 not C?"
    then

    \ Create an incompatible second square.
    s" s0101->s0011" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2

    \ Test compatibility.
    2dup                                    \ sqr1 sqr2 sqr1 sqr2
    squares-compare-pn1-pn1                 \ sqr1 sqr2 I
    [char] I =
    if
        cr ." square-test-compare-pn1-pn1 = I - Ok"
        square-deallocate
    else
        true abort" pn1 pn1 not I?"
    then

    square-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-test-compare-pn1-pn1 - Ok"
;

: squares-test-compare-pn1-pn2

    \ Create pn1 square.
    s" s0100->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr-pn1

    \ Create a second square with pn2.
    s" s0101->s0100" sample-from-string-a   \ sqr-pn1 smpl
    square-new                              \ sqr-pn1 sqr-pn2
    s" s0101->s0111" sample-from-string-a   \ sqr-pn1 sqr-pn2 smpl
    over square-add-sample drop             \ sqr-pn1 sqr-pn2

    \ Test compatibility.
    2dup                                    \ sqr-pn1 sqr-pn2 sqr-pn1 sqr-pn2
    squares-compare-pn1-pn2                 \ sqr-pn1 sqr-pn2 M
    [char] M =
    if
        cr ." square-test-compare-pn1-pn2 = M - Ok"
    else
        true abort" pn1 pn1 not M?"
    then

    \ Make pn1 square incompatible by adding a sample.
    s" s0100->s0111" sample-from-string-a   \ sqr-pn1 sqr-pn2 smpl
    #2 pick                                 \ sqr-pn1 sqr-pn2 smpl sqr-pn1
    square-add-sample drop                  \ sqr-pn1 sqr-pn2

    \ Test compatibility.
    2dup                                    \ sqr-pn1 sqr-pn2 sqr-pn1 sqr-pn2
    squares-compare-pn1-pn2                 \ sqr-pn1 sqr-pn2 I
    [char] I =
    if
        cr ." square-test-compare-pn1-pn2 = I - Ok"
        swap square-deallocate              \ sqr-pn2
    else
        true abort" pn1 pn1 not I?"
    then

    \ Create incompatible pn1 square.
    s" s0100->s1111" sample-from-string-a   \ sqr-pn2 smpl
    square-new                              \ sqr-pn2 sqr-pn1

    \ Compare.
    swap                                    \ sqr-pn1 sqr-pn2
    2dup                                    \ sqr-pn1 sqr-pn2 sqr-pn1 sqr-pn2
    squares-compare-pn1-pn2                 \ sqr-pn1 sqr-pn2 I
    [char] I =
    if
        cr ." square-test-compare-pn1-pn2 = I - Ok"
        swap square-deallocate              \ sqr-pn2
    else
        true abort" pn1 pn1 not I?"
    then

    square-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-test-compare-pn1-pn2 - Ok"
;

: squares-test-compare-pn2-pn2

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." square-test-compare-pn2-pn2 - Ok"
;


: square-tests
    square-test-basic
    squares-test-compare-pnx-pn0
    squares-test-compare-pn1-pn1
    squares-test-compare-pn1-pn2
    squares-test-compare-pn2-pn2
;
