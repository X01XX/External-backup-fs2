
: action-test-basic
    #4 action-new      \ act

    cr dup .action cr

    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    \ cr ." at 1: " .stack-gbl cr
    cr dup .action cr
    \ cr ." at 2: " .stack-gbl cr

    s" s1111->s0111" sample-from-string-a   \ act smpl1
    \ cr ." at 3: " .stack-gbl cr
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then
    \ cr ." at 4: " .stack-gbl cr

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-basic - Ok"
;

: action-tests
    action-test-basic
;
