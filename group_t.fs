
: group-test-new
    \ Init square-list.
    list-new                                    \ sqr-lst
    s" s1010->s1010" square-from-string-a       \ sqr-lst sqr1
    over list-push-struct                       \ sqr-lst
    s" rXX10" region-from-string-a              \ sqr-lst reg

    0 group-new                                 \ grp t | f
    invert abort" group-new failed?"

    cr ." group: " dup .group

    \ Clean up.
    group-deallocate

    \ Test incompatible square list.
    list-new                                    \ lst
    s" s1000->s1000" square-from-string-a over list-push-struct
    s" s1001->s0001" square-from-string-a over list-push-struct
    s" rX0XX" region-from-string-a              \ lst reg
    2dup                                        \ lst reg lst reg
    0 group-new                                 \ lst reg grp t | f
    abort" group-new succeeded?"

    \ Clean up.
    region-deallocate
    square-list-deallocate

    \ Test new with pnc squares that can make the group pnc.

    \ Init square-list.
    list-new                                    \ sqr-lst

    \ Add square 1.
    s" s1010->s1010" sample-from-string-a       \ sqr-lst smpl
    dup square-new                              \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1

    2dup square-add-sample drop                 \ sqr-lst smpl sqr1
    tuck square-add-sample drop                 \ sqr-lst sqr1
    over list-push-struct                       \ sqr-lst

    \ Add square 2.
    s" s0101->s0101" sample-from-string-a       \ sqr-lst smpl
    dup square-new                              \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1
    tuck square-add-sample drop                 \ sqr-lst sqr1
    over list-push-struct                       \ sqr-lst

    s" rXXXX" region-from-string-a              \ sqr-lst reg
    0 group-new                                 \ grp t | f
    invert abort" group-new failed?"

    cr ." group: " dup .group
    dup group-get-pnc invert abort" group not pnc?"

    \ Clean up.
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
    0 group-new                                                         \ sqr8 sqrf, grp t | f
    invert abort" group-new failed?"

    cr ." initial group: " dup .group cr

    \ Check group.
    dup group-get-pn 1 <> abort" Group pn ne 1?"
    s" r1XXX" region-from-string-a                      \ sqr8 sqrf grp reg-tmp'
    over group-get-s-region over                        \ sqr8 sqrf grp reg-tmp' grp-reg reg-tmp'
    regions-eq? invert abort" r-region invalid?"        \ sqr8 sqrf grp reg-tmp'
    region-deallocate                                   \ sqr8 sqrf grp

    \ Change a square to be pn = 2.
    s" s1000->s0000" sample-from-string-a               \ sqr8 sqrf grp smpl
    #3 pick square-add-sample                           \ sqr8 sqrf grp bool
    drop                                                \ sqr8 sqrf grp
    \ cr ." at 1: " .stack-gbl cr
    #2 pick over group-check-changed-square             \ sqr8 sqrf grp
    \ cr ." at 2: " .stack-gbl cr
    cr ." after changing sqr8: " dup .group cr

    \ Check group.
    dup group-get-pn #2 <> abort" Group pn ne 2?"
    s" r1000" region-from-string-a                      \ sqr8 sqrf grp reg-tmp'
    over group-get-s-region over                        \ sqr8 sqrf grp reg-tmp' grp-reg reg-tmp'
    regions-eq? invert abort" r-region invalid?"        \ sqr8 sqrf grp reg-tmp'
    region-deallocate                                   \ sqr8 sqrf grp

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

    \ Test square change than can make the group pnc.

    \ Init square-list.
    list-new                                    \ sqr-lst

    \ Add square 1.
    s" s1010->s1010" sample-from-string-a       \ sqr-lst smpl
    dup square-new                              \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1

    2dup square-add-sample drop                 \ sqr-lst smpl sqr1
    tuck square-add-sample drop                 \ sqr-lst sqr1
    over list-push-struct                       \ sqr-lst

    \ Add square 2.
    s" s0101->s0101" sample-from-string-a       \ sqr-lst smpl
    dup square-new                              \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1
    2dup square-add-sample drop                 \ sqr-lst smpl sqr1

    dup                                         \ sqr-lst smpl sqr1 sqr1
    #3 pick list-push-struct                    \ sqr-lst smpl sqr1
    rot                                         \ smpl sqr1 sqr-lst

    s" rXXXX" region-from-string-a              \ smpl sqr1 sqr-lst reg
    0 group-new                                 \ smpl sqr1 grp t | f
    invert abort" group-new failed?"

    cr ." group: " dup .group
    dup group-get-pnc abort" group pnc true?"

    #2 pick #2 pick square-add-sample           \ smpl sqr1 grp bool
    invert abort" added sample did not make square pnc true?"

    2dup group-check-changed-square
    cr ." group: " dup .group
    dup group-get-pnc invert abort" group not pnc?"

    \ Clean up.
    group-deallocate
    2drop

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." group-test-check-changed-square - Ok"
;

: group-test-add-new-square
    \ Init group.
    list-new                                                            \ lst
    s" s1000->s1000" square-from-string-a over list-push-struct         \ lst
    s" rXXXX" region-from-string-a                                      \ lst reg
    cr
    0 group-new                                                         \ grp t | f
    invert abort" group-new failed?"

    cr ." initial group: " dup .group cr
    \ cr .stack-gbl cr

    \ Add a compatible square.
    s" s1001->s1001" square-from-string-a                               \ grp sqr9
    over
    \ cr ." at 1: " .stack-gbl cr
    group-add-new-square                                                \ grp
    \ cr ." at 2: " .stack-gbl cr
    cr ." group + sqr9: " dup .group cr

    \ Check group.
    s" r100X" region-from-string-a                      \ grp reg-tmp'
    over group-get-s-region over                        \ grp reg-tmp' grp-reg reg-tmp'
    regions-eq? invert abort" r-region invalid?"        \ grp reg-tmp'
    region-deallocate                                   \

    \ Add an incompatible square.
    s" s1011->s0011" square-from-string-a                               \ grp sqr9
    over
    \ cr ." at 1: " .stack-gbl cr
    group-add-new-square                                                \ grp
    \ cr ." at 2: " .stack-gbl cr
    cr ." group + sqrB: " dup .group cr

    \ Deallocate.
    group-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." group-test-add-new-square - Ok"
;

: group-tests
    group-test-new
    group-test-check-changed-square
    group-test-add-new-square
    cr
;
