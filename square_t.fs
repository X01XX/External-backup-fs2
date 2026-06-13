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
    abort" square changed?"
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
        dup square-get-pn 2 <> abort" pn ne 2?"
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
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-test-basic - Ok"
;

: square-test-compare-pn0
    \ Make a pn0 square.
    s" s0101->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr1
    s" s0101->s0101" sample-from-string-a   \ sqr smpl
    over square-add-sample drop             \ sqr1
    s" s0101->s0100" sample-from-string-a   \ sqr smpl
    over square-add-sample drop             \ sqr1
    cr ." square 1 smpl: " dup .square      \ sqr1

    \ Make another pn0 square.
    s" s0100->s0111" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2
    s" s0100->s0101" sample-from-string-a   \ sqr1 sqr smpl
    over square-add-sample drop             \ sqr1 sqr2
    s" s0100->s0100" sample-from-string-a   \ sqr1 sqr smpl
    over square-add-sample drop             \ sqr1 sqr2
    cr ." square 1 smpl: " dup .square      \ sqr1 sqr2

    \ Test compatibility.
    2dup squares-compare                    \ sqr1 sqr2 c
    [char] C =
    if
        cr ." square-test-compare-pn0 pn0 = C - Ok"
        square-deallocate                   \ sqr1
    else
        true abort" pn0 pn0 not C?"
    then

    \ Make a pn1 square.
    s" s0100->s0111" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2

    \ Test compatibility.
    2dup swap                               \ sqr1 sqr2 sqr2 sqr1
    squares-compare                         \ sqr1 sqr2 m
    [char] M =
    if
        cr ." square-test-compare-pn0 pn1 = M - Ok"
    else
        true abort" pn0 pn1 not M?"
    then

    \ Add to pn1 square to make it pnc == true.
    s" s0100->s0111" sample-from-string-a   \ sqr1 sqr2 smpl
    2dup swap square-add-sample drop
    2dup swap square-add-sample drop
    over square-add-sample drop             \ sqr1 sqr2

    \ Test compatibility.
    2dup swap                               \ sqr1 sqr2 sqr2 sqr1
    squares-compare                         \ sqr1 sqr2 I
    [char] I =
    if
        cr ." square-test-compare-pn0 pn1 = I - Ok"
        square-deallocate
    else
        true abort" pn0 pn1 not I?"
    then

    square-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." square-test-compare-pn0 - Ok"
;

: square-test-compare-pn1
    cr ." square-test-compare-pn1 - Ok"

    \ Create pn1 square.
    s" s0100->s0111" sample-from-string-a   \ smpl
    square-new                              \ sqr1

    \ Create a compatible second square.
    s" s0101->s0111" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2

    \ Test compatibility.
    2dup                                    \ sqr1 sqr2 sqr1 sqr2
    squares-compare                         \ sqr1 sqr2 C
    [char] C =
    if
        cr ." square-test-compare-pn1 pn1 = C - Ok"
        square-deallocate
    else
        true abort" pn1 pn1 not C?"
    then

    \ Create an incompatible second square.
    s" s0101->s0011" sample-from-string-a   \ sqr1 smpl
    square-new                              \ sqr1 sqr2

    \ Test compatibility.
    2dup                                    \ sqr1 sqr2 sqr1 sqr2
    squares-compare                         \ sqr1 sqr2 I
    [char] I =
    if
        cr ." square-test-compare-pn1 pn1 = I - Ok"
        square-deallocate
    else
        true abort" pn1 pn1 not I?"
    then

    square-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated
;

: square-test-compare-pn2
    cr ." square-test-compare-pn2 - Ok"
;

: square-tests
    square-test-basic
    square-test-compare-pn0
    square-test-compare-pn1
    square-test-compare-pn2
;
