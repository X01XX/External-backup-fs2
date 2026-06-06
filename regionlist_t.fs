\ Test region-list functions.

: region-list-test-defining-regions

    \ Init regions to extract info from.
    s" (r1XXX rXX1X rX0XX rXXX0 r0X0X)"
    region-list-from-string-a               \ reg-lst'
    cr ." For: " dup .region-list

    \ Get defining regions info.
    dup
    region-list-defining-regions            \ reg-lst' def-lst'
    cr ." defining: " dup structinfo-list-print-struct-list

    \ Check results.
    s" ((r0X0X (r0101)) (rXX1X (r0111)) (r1XXX (r1101)))" list-from-string-a  \ reg-lst' def-lst' tst-list'
    \ cr ." test lt: " dup structinfo-list-print-struct-list

    2dup lists-eq?
    if
    else
        cr ." lists ne?" cr
        abort
    then

    \ Clean up.                                     \ reg-lst' def-lst' tst-list'
    structinfo-list-deallocate-struct-list          \ reg-lst' def-lst'
    structinfo-list-deallocate-struct-list          \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-list-test-defining-regions - Ok"
;

\ Calculate ~A + ~B for two states, and intersect the result with a region-list
\ to produce a cumulative list.
: regionlist-cumulative-~a+~b ( reg-lst2 sta1 sta0 -- reg-lst )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    assert-3os-is-region-list

    state-~a+~b                         \ reg-lst2 reg-lst'
    tuck                                \ reg-lst' reg-lst2 reg-lst'
    region-list-intersections-nosubs    \ reg-lst' ret-lst
    swap region-list-deallocate         \ ret-lst
;

: region-list-test-defining-regions2

    #4 all-bits #4 state-new            \ all-sta
    0 #4 state-new                      \ all-nta 0-sta
    region-new                          \ reg-max
    list-new tuck list-push-struct      \ reg-lst'

    \ Calc one pair.
    dup                                 \ reg-lst' reg-lst'
    s" s0101" state-from-string-a tuck  \ reg-lst' sta5' reg-lst' sta5'
    s" s0110" state-from-string-a -rot  \ reg-lst' sta5' sta6' reg-lst' sta5'
    #2 pick                             \ reg-lst' sta5' sta6' reg-lst' sta5' sta6'
    regionlist-cumulative-~a+~b         \ reg-lst' sta5' sta6' reg-lst2'
    swap state-deallocate
    swap state-deallocate
    swap region-list-deallocate         \ reg-lst2
    cr ." ~5 + ~6: " dup .region-list

    \ Calc one pair.
    dup                                 \ reg-lst' reg-lst'
    s" s0101" state-from-string-a tuck  \ reg-lst' sta5' reg-lst' sta5'
    s" s1001" state-from-string-a -rot  \ reg-lst' sta5' sta9' reg-lst' sta5'
    #2 pick                             \ reg-lst' sta5' sta9' reg-lst' sta5' sta9'
    regionlist-cumulative-~a+~b         \ reg-lst' sta5' sta9' reg-lst2'
    swap state-deallocate
    swap state-deallocate
    swap region-list-deallocate         \ reg-lst2
    cr ." (~5 + ~6) & (~5 + ~9): " dup .region-list

    \ Clean up.
    structinfo-list-deallocate-struct-list

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-list-test-defining-regions2 - Ok"
;

: region-list-tests
    region-list-test-defining-regions
    region-list-test-defining-regions2
;
