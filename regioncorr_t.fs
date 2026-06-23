

: regioncorr-test-superset

    \ Init lists.
    s" (r0XX1 r0XX0X)" regioncorr-from-string-a \ regc1
    s" (r0XXX r0XXXX)" regioncorr-from-string-a \ regc1 regc2

    2dup regioncorr-superset?       \ regc1 regc2 flag
    0= abort" List is not superset?"

    \ Try the reverse.
    swap                            \ regc2 regc1
    2dup regioncorr-superset?       \ regc2 regc1 flag
    abort" List is superset?"

    \ Clean up.
    regioncorr-deallocate           \ regc2
    regioncorr-deallocate           \

    cr ." regioncorr-test-superset: Ok"
;

: regioncorr-test-intersects

    \ Init lists.
    s" (r0XX1 r0XX0X)" regioncorr-from-string-a \ regc1
    s" (r0XXX r0XXXX)" regioncorr-from-string-a \ regc1 regc2

    \ Test intersection.
    2dup regioncorr-intersects?   \ regc1 regc2 bool
    false? abort" Lists do not intersect?"

    regioncorr-deallocate              \ regc1

    \ Test non-intersection.
    s" (r1XXX r0XXXX)" regioncorr-from-string-a \ regc1 lst3

    \ cr ." lst3: " dup .regioncorr cr

    2dup regioncorr-intersects?   \ regc1 lst3 bool
    abort" Lists do intersect?"

    \ Clean up.
    regioncorr-deallocate              \ regc1
    regioncorr-deallocate              \

    cr ." regioncorr-test-intersects: Ok"
;

: regioncorr-test-subtract
    \ Init lists.
    s" (rX1X1 r0X10X)" regioncorr-from-string-a \ regc1
    s" (r0X0X r0XXX1)" regioncorr-from-string-a \ regc1 regc2

    \ cr dup .regioncorr space ." - " over .regioncorr

    \ Test subtraction.
    2dup regioncorr-subtract      \ regc1 regc2, regc-lst t | f
    invert abort" Subtract failed?"

    \ cr ." = " dup .regioncorr-list cr

    \ Check result.
    s" ((r0X0X r0X0X1)(r0X0X r0XX11)(r0X00 r0XXX1)(r000X r0XXX1))"
    regioncorr-list-from-string-a       \ regc1 regc2 regc-lst tst-lst
    2dup lists-eq?                      \ regc1 regc2 regc-lst tst-lst bool
    if
    else
        cr ." Lists ne?" cr
        abort
    then

    \ Clean up.
    regioncorr-list-deallocate                  \ regc1 regc2 regc-lst
    regioncorr-list-deallocate                  \ regc1 regc2
    regioncorr-deallocate                       \ regc1
    regioncorr-deallocate

    cr ." regioncorr-test-subtract: Ok"
;

: regioncorr-test-complement
    \ Init list 1.
    s" (rX1X1 rXX10X)" regioncorr-from-string-a \ regc

    \ cr ." regc: " dup .regioncorr

    dup                                 \ regc regc
    regioncorr-complement               \ regc regc-lst

    \ Test the result.
    s" ((rxxxx rxx0xx)(rxxxx rxxx1x)(rxxx0 rxxxxx)(rx0xx rxxxxx))"
    regioncorr-list-from-string-a       \ regc regc-lst tst-lst

    2dup lists-eq?                      \ regc regc-lst tst-lst bool
    if
    else
        cr ." Lists ne?" cr
        abort
    then

    \ Clean up list-of-lists.
    regioncorr-list-deallocate          \ regc regc-lst
    regioncorr-list-deallocate          \ regc

    regioncorr-deallocate

    cr ." regioncorr-test-complement: Ok"
;

: regioncorr-test-distance
    \ Init lists.
    s" (rX1X1 r0X10X)" regioncorr-from-string-a   \ regc1
    s" (r1001 r0XX11)" regioncorr-from-string-a   \ regc1 regc2

    2dup regioncorr-distance                    \ regc1 regc2 dist
    #2 <> abort" Distance not 2?"

    regioncorr-deallocate                       \ regc1
    regioncorr-deallocate                       \

    cr ." regioncorr-test-distance: Ok"
;

\ Assume domain0 is 4-bit, Domain 1 is 5-bit.
: regioncorr-tests
    session-new                                     \ ses

    \ Init domain 0.
    #4 over domain-new                              \ ses dom0
    over                                            \ ses dom0 ses
    session-add-domain                              \ ses

    \ Init domain 1.
    #5 over domain-new                              \ ses dom0
    over                                            \ sess dom0 ses
    session-add-domain                              \ sess

    regioncorr-test-superset
    regioncorr-test-intersects
    regioncorr-test-subtract
    regioncorr-test-complement
    regioncorr-test-distance
    cr

    session-deallocate
;

