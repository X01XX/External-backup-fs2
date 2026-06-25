
: group-test-new
    \ Init square-list.
    list-new                                            \ sqr-lst
    s" s1010->s1010" sample-from-string-a square-new    \ sqr-lst sqr1
    over list-push-struct                               \ sqr-lst
    s" rXX10" region-from-string-a                      \ sqr-lst reg

    group-new                                           \ grp

    cr ." group: " dup .group cr

    \ Check the group is valid.
    dup group-get-valid                                 \ grp bool
    if
    else
        cr ." group-test-new: group not valid? " cr abort
    then

    \ Deallocate.
    group-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." group-test-new - Ok"
;

: group-tests
    group-test-new
    cr
;
