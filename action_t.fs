
: action-test-basic
    #4 action-new      \ act

    cr dup .action cr

    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-basic - Ok"
;

: action-tests
    action-test-basic
;
