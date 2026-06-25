
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

: group-tests
    group-test-new
    cr
;
