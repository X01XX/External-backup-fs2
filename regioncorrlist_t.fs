

: regioncorr-list-test-any-superset
    s" ((rxx10 rXX0XX))" regioncorr-list-from-string-a  \ regc-lst

    s" (rxxx1 rXX0XX)" regioncorr-from-string-a
    swap                                \ regc regc-lst

    2dup regioncorr-list-any-superset?  \ regc regc-lst bool
    abort" Superset found?"

    s" (rxxx1 rXX0XX)" regioncorr-from-string-a
    over regioncorr-list-push           \ regc regc-lst

    2dup regioncorr-list-any-superset?  \ regc regc-lst bool
    false? abort" Superset not found?"

                                        \ regc regc-lst
    regioncorr-list-deallocate          \ regc
    regioncorr-deallocate

    cr ." regioncorr-list-test-any-superset: Ok"
;

: regioncorr-list-test-remove-subsets
    s" (rxx1x rXX0XX)" regioncorr-from-string-a   \ regc

    s" ((rx111 rXX0XX)(rxxx1 rXX0XX)(rxx10 rXX0XX))"
    regioncorr-list-from-string-a               \ regc regc-lst

    \ cr ." before: " dup .regioncorr-list
    2dup regioncorr-list-remove-subsets         \ regc regc-lst
    \ cr ." after:  " dup .regioncorr-list

    \ Check results.
    dup list-get-length                         \ regc regc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (rXXX1 rxx0xx)" regioncorr-from-string-a \ regc regc-lst regc-t
    [ ' regioncorrs-eq? ] literal               \ regc regc-lst regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-t bool
    false? abort" regioncorr (rXXX1 rxx0xx) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst

                                                \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate

    cr ." regioncorr-list-test-remove-subsets: Ok"
;

: regioncorr-list-test-push-nosubs
    s" (rxx1x rXX0XX)" regioncorr-from-string-a   \ regc

    s" ((rx111 rXX0XX)(rxxx1 rXX0XX)(rxx10 rXX0XX))"
    regioncorr-list-from-string-a               \ regc regc-lst

    \ cr ." before: " dup .regioncorr-list
    tuck regioncorr-list-push-nosubs            \ regc-lst bool
    false? abort" push-nosubs failed?"
    \ cr ." after:  " dup .regioncorr-list

    \ Check results.
    s" ((rxx1x rXX0XX)(rxxx1 rXX0XX))"
    regioncorr-list-from-string-a               \ regc-lst tst-lst
    2dup lists-eq?                              \ regc-lst tst-lst bool
    false? abort" list ne?"

                                                \ regc-lst tst-lst
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    cr ." regioncorr-list-test-push-nosubs: Ok"
;

: regioncorr-list-test-copy-nosubs
    \ Init list.
    s" ((r000X r000XX)(r0001 r00010)(r000X r000XX))"
    regioncorr-list-from-string-a               \ regc-lst
    \ cr ." before: " dup .regioncorr-list

    dup                                         \ regc-lst regc-lst
    regioncorr-list-copy-nosubs                 \ regc-lst regc-lst'
    \ cr ." after:  " dup .regioncorr-list

    \ Check result length.
    dup list-get-length                         \ regc regc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (r000X r000XX)" regioncorr-from-string-a \ regc-lst regc-lst' regc-t
    [ ' regioncorrs-eq? ] literal               \ regc-lst regc-lst' regc-t xt
    over #3 pick list-member                    \ regc-lst regc-lst' regc-t bool
    false? abort" regioncorr (r000X r000XX) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc-lst regc-lst'

    \ Clean up.
    swap
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate

    cr ." regioncorr-list-test-copy-nosubs: Ok"
;

: regioncorr-list-test-subtract-regc
    \ Run against an intersecting regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (r000X r001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add intersecting regc.
    s" (rX001 r00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    s" ((rx001 r0000X)(r1001 r00x0x))"
    regioncorr-list-from-string-a               \ regc regc-lst regc-left regc-test

    2dup lists-eq?                              \ regc regc-lst regc-left regc-test
    false? abort" lists ne?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst regc-left
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate

    \ Run against a non-intersection regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (r000X r001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add non-intersecting regc.
    s" (rX011 r00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length 1 <> abort" List length not 1?"

    \ Check result 1.
    s" (rX011 r00X0X)" regioncorr-from-string-a \ regc regc-lst regc-left regc-t
    [ ' regioncorrs-eq? ] literal               \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    false? abort" regioncorr (rX011 r00X0X) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    \ Run against a subset regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (r000X r001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add a subset regc
    s" (r0000 r0010X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length 0<> abort" List length not 0?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    \ Run against a all three options.

    \ Init regc to subtract.
    s" (r000X r001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add intersecting regc.
    s" (rX001 r00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push

    \ Add a subset regc.
    s" (r0000 r0010X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Add non-intersecting regc.
    s" (rX011 r00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst
    \ cr dup .regioncorr-list space ." - " over .regioncorr

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left
    \ space ." = " dup .regioncorr-list cr

    \ Check result.
    s" ((rx011 r00x0x)(rx001 r0000X)(r1001 r00x0x))"
    regioncorr-list-from-string-a               \ regc regc-lst regc-left regc-t
    2dup lists-eq?                              \ regc regc-lst regc-left regc-t bool
    false? abort" lists ne?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst regc-left
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    cr ." regioncorr-list-test-subtract-regc: Ok"
;

: regioncorr-list-test-complement
    \ Init regioncorr-list.
    s" ((rX11X rX1X1X)(rX10X rX1X0X))"
    regioncorr-list-from-string-a               \ regc-lst'

    \ Get complement.
    dup regioncorr-list-complement              \ regc-lst' regc-lst'
    \ cr ." comp1: " dup .regioncorr-list cr

    \ Check results.
    s" ((rxx1x rxxx0x)(rxxxx rx0xxx)(rxx0x rxxx1x)(rx0xx rxxxxx))"
    regioncorr-list-from-string-a               \ regc-lst' regc-lst' tst-lst'
    2dup lists-eq?
    false? abort" Lists ne?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst regc-lst'
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-complement: Ok"
;

: regioncorr-list-test-normalize
    \ Init regioncorr-list.
    list-new                                    \ regc-lst

    \ Add regc 1.
    s" (rX10X rX1X0X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Add regc 2.
    s" (rX11X rX1X0X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Normalize
    dup regioncorr-list-normalize               \ regc-lst regc-lst'

    \ Check results.
    \ cr ." norm: " dup .regioncorr-list cr
    dup list-get-length 1 <> abort" normalized len not 1?"

    \ Check result 1.
    s" (rX1XX rX1X0X)" regioncorr-from-string-a \ regc-lst regc-lst' regc-t
    [ ' regioncorrs-eq? ] literal               \ regc-lst regc-lst' regc-t xt
    over #3 pick list-member                    \ regc-lst regc-lst' regc-t bool
    false? abort" regioncorr (rX1XX rX1X0X) not found in regc-lst' ?"
    regioncorr-deallocate                       \ regc-lst regc-lst'

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-normalize: Ok"
;

: regioncorr-list-test-intersection-fragments
\ Init regioncorr-list.
    list-new                                    \ regc-lst

    \ Add regc 1.
    s" (rX1X1 rXX1X1)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Add regc 2.
    s" (r1XX1 rX1X1X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Normalize
    dup regioncorr-list-intersection-fragments  \ regc-lst regc-lst'
    \ cr dup .regioncorr-list cr

    \ Check results.
    s" ((r01X1 rXX1X1)(rX1X1 rX01X1)(rX1X1 rXX101)(r10X1 rX1X1X)(r1XX1 rX101X)(r1XX1 rX1X10)(r11X1 rX1111))"
    regioncorr-list-from-string-a               \ regc-lst regc-lst' tst-lst'
    2dup lists-eq?                              \ regc-lst regc-lst' tst-lst' bool
    false? abort" Lists ne?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst regc-lst'
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate

    cr ." regioncorr-list-test-intersection-fragments: Ok"
;

: regioncorr-list-tests
    session-new                                     \ ses

    \ Init domain 0.
    #4 over domain-new                              \ ses dom0
    over                                            \ ses dom0 ses
    session-add-domain                              \ ses

    \ Init domain 1.
    #5 over domain-new                              \ ses dom0
    over                                            \ sess dom0 ses
    session-add-domain                              \ sess

    regioncorr-list-test-any-superset
    regioncorr-list-test-remove-subsets
    regioncorr-list-test-push-nosubs
    regioncorr-list-test-copy-nosubs
    regioncorr-list-test-subtract-regc
    regioncorr-list-test-complement
    regioncorr-list-test-normalize
    regioncorr-list-test-intersection-fragments

    session-deallocate

    cr ." regioncorr-list tests: Ok" cr
;
