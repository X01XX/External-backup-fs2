
: group-test-new
    \ Init square-list.
    list-new                                    \ sqr-lst
    s" s1010->s1010" square-from-string-a       \ sqr-lst sqr1
    over list-push-struct                       \ sqr-lst
    s" rXX10" region-from-string-a              \ sqr-lst reg

    group-new                                   \ grp

    cr ." group: " dup .group

    \ Check the group is valid.
    dup group-get-valid                         \ grp bool
    if
    else
        cr ." group-test-new: group not valid? " cr abort
    then

    \ Deallocate.
    group-deallocate

    \ Test empty square list.
    list-new                                    \ lst
    s" rX0XX" region-from-string-a              \ lst reg
    cr group-new                                \ grp
    cr dup .group
    group-deallocate

    \ Test incompatible square list.
    list-new                                    \ lst
    s" s1000->s1000" square-from-string-a over list-push-struct
    s" s1001->s0001" square-from-string-a over list-push-struct
    s" rX0XX" region-from-string-a              \ lst reg
    cr
    group-new                                   \ grp
    cr dup .group
    group-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." group-test-new - Ok"
;

: group-test-check-changed-square
    \ Init group.
    list-new                                                            \ lst
    s" s1000->s1000" square-from-string-a tuck over list-push-struct    \ sqr8 lst
    s" s1111->s1111" square-from-string-a tuck over list-push-struct    \ sqr8 sqrf lst
    s" rXXXX" region-from-string-a                                      \ sqr8 sqrf lst reg
    cr
    group-new                                                           \ sqr8 sqrf grp
    cr ." initial group: " dup .group

    \ Change a square to be pn = 2.
    s" s1000->s0000" sample-from-string-a               \ sqr8 sqrf grp smpl
    #3 pick square-add-sample                           \ sqr8 sqrf grp bool
    drop                                                \ sqr8 sqrf grp
    \ cr ." at 1: " .stack-gbl cr
    #2 pick over group-check-changed-square             \ sqr8 sqrf grp
    \ cr ." at 2: " .stack-gbl cr
    cr ." after changing sqr8: " dup .group cr

    \ Make sqrf incompatible with sqr8.
    s" s1111->s1111" sample-from-string-a               \ sqr8 sqrf grp smpl
    #2 pick square-add-sample                           \ sqr8 sqrf grp bool
    drop
    2dup group-check-changed-square                     \ sqr8 sqrf grp
    cr ." after changing sqrf: " dup .group cr
    
    \ Deallocate.
    \ cr ." at 3: " .stack-gbl cr
    group-deallocate
    2drop

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." group-test-check-changed-square - Ok"
;

: group-tests
    group-test-new
    group-test-check-changed-square
    cr
;
