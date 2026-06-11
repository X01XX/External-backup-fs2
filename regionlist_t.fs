\ Test region-list functions.

: region-list-test-defining-regions

    \ Init region-list to max region.
    #4 all-bits #4 state-new            \ all-sta
    0 #4 state-new                      \ all-nta 0-sta
    region-new                          \ reg-max
    list-new tuck list-push-struct      \ reg-lst'

    \ Calc one pair.
    s" s0101" state-from-string-a       \ reg-lst' sta5'
    s" s0111" state-from-string-a       \ reg-lst' sta5' sta7'
    2dup #4 pick                        \ reg-lst' sta5' sta7' sta5' sta7' reg-lst'
    regionlist-cumulative-~a+~b         \ reg-lst' sta5' sta7' reg-lst2'
    swap state-deallocate
    swap state-deallocate
    swap region-list-deallocate         \ reg-lst2
    cr ." ~5 + ~7: " dup .region-list

    \ Get defining regions info.
    dup
    region-list-defining-regions-parts          \ reg-lst' def-lst'
    cr ." defining: " dup structinfo-list-print-struct-list
    structinfo-list-deallocate-struct-list      \ reg-lst' def-lst'
    cr
    \ Calc one pair.
    s" s0101" state-from-string-a       \ reg-lst' sta5'
    s" s1101" state-from-string-a       \ reg-lst' sta5' stad'
    2dup #4 pick                        \ reg-lst' sta5' stad' sta5' stad' reg-lst'
    regionlist-cumulative-~a+~b         \ reg-lst' sta5' stad' reg-lst2'
    swap state-deallocate
    swap state-deallocate
    swap region-list-deallocate         \ reg-lst2
    cr ." (~5 + ~7) & (~5 + ~D): " dup .region-list

    \ Get defining regions info.
    dup
    region-list-defining-regions-parts      \ reg-lst' def-lst'
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

: region-list-test-evaluate-for-corners

    \ Init state list.
    list-new                            \ sta-lst'

    \ Init region-list to max region.
    #4 all-bits #4 state-new            \ sta-lst' all-sta
    0 #4 state-new                      \ sta-lst' all-nta 0-sta
    region-new                          \ sta-lst' reg-max
    list-new tuck list-push-struct      \ sta-lst' reg-lst'

    \ Calc one pair.
    s" s0101" state-from-string-a       \ sta-lst' reg-lst' sta5'
    s" s0111" state-from-string-a       \ sta-lst' reg-lst' sta5' sta7'
    2dup #4 pick                        \ sta-lst' reg-lst' sta5' sta7' sta5' sta7' reg-lst'
    regionlist-cumulative-~a+~b         \ sta-lst' reg-lst' sta5' sta7' reg-lst2'
    swap                                \ sta-lst' reg-lst' sta5' reg-lst2' sta7'
    #4 pick state-list-push             \ sta-lst' reg-lst' sta5' reg-lst2'
    swap                                \ sta-lst' reg-lst' reg-lst2' sta5'
    #3 pick state-list-push             \ sta-lst' reg-lst' reg-lst2'
    swap region-list-deallocate         \ sta-lst' reg-lst2'
    cr ." ~5 + ~7: " dup .region-list

    \ Get defining regions info.
    dup
    region-list-defining-regions-parts              \ sta-lst' reg-lst' def-lst'
    cr ." defining: " dup structinfo-list-print-struct-list
    structinfo-list-deallocate-struct-list          \ reg-lst' def-lst'
    cr
    \ Calc one pair.
    s" s0000" state-from-string-a       \ sta-lst' reg-lst' sta0'
    s" s1000" state-from-string-a       \ sta-lst' reg-lst' sta0' sta8'
    2dup #4 pick                        \ sta-lst' reg-lst' sta0' sta8' sta0' sta8' reg-lst'
    regionlist-cumulative-~a+~b         \ sta-lst' reg-lst' sta0' sta8' reg-lst2'
    swap                                \ sta-lst' reg-lst' sta0' reg-lst2' sta8'
    #4 pick state-list-push             \ sta-lst' reg-lst' sta0' reg-lst2'
    swap                                \ sta-lst' reg-lst' reg-lst2' sta0'
    #3 pick state-list-push             \ sta-lst' reg-lst' reg-lst2'
    swap region-list-deallocate         \ sta-lst' reg-lst2'
    cr ." (~5 + ~7) & (~0 + ~8): " dup .region-list

    \ Get defining regions info.
    dup
    region-list-defining-regions-parts  \ sta-lst' reg-lst' def-lst'
    cr ." defining: " dup structinfo-list-print-struct-list

    \ Check results.
    s" ((rXX1X (r0111)) (r1XXX (r1000)))" list-from-string-a  \ sta-lst' reg-lst' def-lst' tst-list'
    cr ." test lt: " dup structinfo-list-print-struct-list

    2dup lists-eq?
    if
    else
        cr ." lists ne?" cr
        abort
    then
    structinfo-list-deallocate-struct-list                  \ sta-lst' reg-lst' def-lst'
    structinfo-list-deallocate-struct-list                  \ sta-lst' reg-lst'

    \ Add 9, 1.
    s" s1001" state-from-string-a                           \ sta-lst' reg-lst' sta9'
    #2 pick state-list-push                                 \ sta-lst' reg-lst'
    s" s0001" state-from-string-a                           \ sta-lst' reg-lst' sta1'
    #2 pick state-list-push                                 \ sta-lst' reg-lst'

    \ Evaluate.
    2dup region-list-evaluate-for-corners                   \ sta-lst' reg-lst'

    \ Clean up.                                     \ sta-lst' reg-lst'
    region-list-deallocate                          \ sta-lst'
    state-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-list-test-evaluate-for-corners - Ok"
;

: region-list-tests
    region-list-test-defining-regions
    region-list-test-evaluate-for-corners
;
