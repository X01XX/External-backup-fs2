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

: square-tests
    square-test-basic
;
