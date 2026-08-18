\ Test region-list functions.

: region-list-test-defining-regions

    \ Init region-list to max region.
    s" (rXXXX)" list-from-string-a      \ reg-lst'

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
    cr ." defining: " dup .struct-list
    struct-list-deallocate                      \ reg-lst' def-lst'
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
    cr ." defining: " dup .struct-list

    \ Check results.
    s" ((r0X0X (r0101)) (rXX1X (r0111)) (r1XXX (r1101)))" list-from-string-a  \ reg-lst' def-lst' tst-list'
    \ cr ." test lt: " dup .struct-list

    2dup lists-eq?
    if
    else
        cr ." lists ne?" cr
        abort
    then

    \ Clean up.                                     \ reg-lst' def-lst' tst-list'
    struct-list-deallocate                          \ reg-lst' def-lst'
    struct-list-deallocate                          \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
     check-project-deallocated

    cr ." region-list-test-defining-regions - Ok"
;

: region-list-test-evaluate-for-corners

    \ Init state list.
    list-new                            \ sta-lst'

    \ Init region-list to max region.
    s" (rXXXX)" list-from-string-a      \ sta-lst' reg-lst'

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
    region-list-defining-regions-parts          \ sta-lst' reg-lst' def-lst'
    cr ." defining: " dup .struct-list
    struct-list-deallocate                      \ reg-lst' def-lst'
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
    cr ." defining: " dup .struct-list

    \ Check results.
    s" ((rXX1X (r0111)) (r1XXX (r1000)))" list-from-string-a  \ sta-lst' reg-lst' def-lst' tst-list'
    cr ." test lt: " dup .struct-list

    2dup lists-eq?
    if
    else
        cr ." lists ne?" cr
        abort
    then
    struct-list-deallocate                      \ sta-lst' reg-lst' def-lst'
    struct-list-deallocate                      \ sta-lst' reg-lst'

    \ Add 9, 1.
    s" s1001" state-from-string-a               \ sta-lst' reg-lst' sta9'
    #2 pick state-list-push                     \ sta-lst' reg-lst'
    s" s0001" state-from-string-a               \ sta-lst' reg-lst' sta1'
    #2 pick state-list-push                     \ sta-lst' reg-lst'

    \ Evaluate.
    2dup region-list-evaluate-for-corners       \ sta-lst' reg-lst'

    \ Clean up.                                 \ sta-lst' reg-lst'
    region-list-deallocate                      \ sta-lst'
    state-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." region-list-test-evaluate-for-corners - Ok"
;

: region-list-test-proper-intersections
    s" (rx10x r1x0x rxxx1 r0xxx)" list-from-string-a    \ reg-lst'

    dup region-list-proper-intersections                \ reg-lst', reg-ints' t | f
    invert abort" no intersections?"

    \ Display.
    cr cr ." ints of " over .region-list space ." are: " dup .region-list cr

    \ Test.
    dup list-get-length #5 <> abort" s/b 5 regions"

    dup region-list-proper-intersections                \ reg-lst' reg-ints', reg-ints2' t | f
    invert abort" no intersections?"

    \ Display.
    cr ." ints of " over .region-list space ." are: " dup .region-list cr

    \ Test.
    dup list-get-length #2 <> abort" s/b 2 regions"

    dup region-list-proper-intersections            \ reg-lst' reg-ints' reg-ints2', reg-ints3' t | f
    abort" intersections?"

    \ Clean up.                                     \ reg-lst' reg-ints' reg-ints2'
    region-list-deallocate                          \ reg-lst' reg-ints'
    region-list-deallocate                          \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." region-list-test-proper-intersections - Ok"
;

: region-list-test-split-by-intersections
    s" (rx10x r1x0x rxxx1 r0xxx)" list-from-string-a        \ reg-lst'

    dup region-list-split-by-intersections          \ reg-lst', reg-ints' t | f
    invert abort" no intersections?"

    \ Display.
    cr cr ." ints of " over .region-list space ." are: " dup .region-list cr

    dup                                             \ reg-lst' reg-ints' reg-ints'
    foreach                                         \ reg-lst' reg-ints' int-lnk intx
        cr dup .region
        #3 pick                                     \ reg-lst' reg-ints' int-lnk intx reg-lst'
        region-list-supersets-of                    \ reg-lst' reg-ints' int-lnk regs-in'
        space ." in " dup .region-list
        region-list-deallocate                      \ reg-lst' reg-ints' int-lnk
    next
    cr

    \ Test.
    dup list-get-length #11 <> abort" s/b 11 regions"

    \ Test that the results cover the whole original list.
    2dup swap region-list-subtract                  \ reg-lst' reg-ints' sub-regs'
    \ cr ." sub regs: " dup .region-list cr
    dup list-get-length 0<> abort" ints don't cover original regions"

    \ Test no result regions are a proper intersection of any region in the original list.
    over                                            \ reg-lst' reg-ints' sub-regs' reg-ints'
    #3 pick                                         \ reg-lst' reg-ints' sub-regs' reg-ints' reg-lst'
    region-list-any-proper-intersections?           \ reg-lst' reg-ints' sub-regs' bool
    abort" some are proper intersections?"

    \ Clean up.                                     \ reg-lst' reg-ints' sub-regs'
    region-list-deallocate                          \ reg-lst' reg-ints'
    region-list-deallocate                          \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." region-list-test-split-by-intersections - Ok"
;

: region-list-test-split-by-intersections2
    s" (rxxxx rxxx1 rx1x1)" list-from-string-a      \ reg-lst'

    dup region-list-split-by-intersections          \ reg-lst', reg-ints' t | f
    invert abort" no intersections?"

    \ Display.
    cr cr ." ints of " over .region-list space ." are: " dup .region-list cr

    dup                                             \ reg-lst' reg-ints' reg-ints'
    foreach                                         \ reg-lst' reg-ints' int-lnk intx
        cr dup .region
        #3 pick                                     \ reg-lst' reg-ints' int-lnk intx reg-lst'
        region-list-supersets-of                    \ reg-lst' reg-ints' int-lnk regs-in'
        space ." in " dup .region-list
        region-list-deallocate                      \ reg-lst' reg-ints' int-lnk
    next
    cr

    \ Test.
    dup list-get-length #3 <> abort" s/b 3 regions"

    \ Test that the results cover the whole original list.
    2dup swap region-list-subtract                  \ reg-lst' reg-ints' sub-regs'
    \ cr ." sub regs: " dup .region-list cr
    dup list-get-length 0<> abort" ints don't cover original regions"

    \ Test no result regions are a proper intersection of any region in the original list.
    over                                            \ reg-lst' reg-ints' sub-regs' reg-ints'
    #3 pick                                         \ reg-lst' reg-ints' sub-regs' reg-ints' reg-lst'
    region-list-any-proper-intersections?           \ reg-lst' reg-ints' sub-regs' bool
    abort" some are proper intersections?"

    \ Clean up.                                     \ reg-lst' reg-ints' sub-regs'
    region-list-deallocate                          \ reg-lst' reg-ints'
    region-list-deallocate                          \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." region-list-test-split-by-intersections2 - Ok"
;

: region-list-test-defining-region-parts
    s" (rx10x r1x0x rxxx1 r0xxx)" list-from-string-a    \ reg-lst'

    dup region-list-defining-region-parts               \ reg-lst' def-regs'
    cr cr ." defining parts of: " over .region-list space ." are: " dup .region-list cr cr
    dup                                                 \ reg-lst' def-regs' def-regs'
    foreach                                             \ reg-lst' def-regs' def-lnk regx
        dup .region
        #3 pick                                         \ reg-lst' def-regs' def-lnk regx reg-lst'
        region-list-supersets-of                        \ reg-lst' def-regs' def-lnk sup-lst'
        space ." in " dup .region-list cr               \ reg-lst' def-regs' def-lnk sup-lst'
        region-list-deallocate                          \ reg-lst' def-regs' def-lnk
    next

    \ Clean up.                                         \ reg-lst' def-regs'
    region-list-deallocate                              \ reg-lst'
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." region-list-test-defining-region-parts - Ok"
;

: region-list-tests
    region-list-test-defining-regions
    region-list-test-evaluate-for-corners
    region-list-test-proper-intersections
    region-list-test-split-by-intersections
    region-list-test-split-by-intersections2
    region-list-test-defining-region-parts
    cr
;
