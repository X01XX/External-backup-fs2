
: action-test-basic
    \ Init action.
    [ ' calc-result-x ] literal
    #4 0 action-new      \ act

    cr dup .action cr

    \ Add A->A
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    \ cr ." at 1: " .stack-gbl cr
    cr dup .action cr
    \ cr ." at 2: " .stack-gbl cr

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1
    \ cr ." at 3: " .stack-gbl cr
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then
    \ cr ." at 4: " .stack-gbl cr

    cr dup .action cr

    s" (rX0XX rXXX0 rX1XX rXXX1)" list-from-string-a    \ act reg-lst'
    over action-get-groups group-list-regions           \ act reg-lst' grp-regs'
    2dup region-lists-eq?                               \ act reg-lst' grp-regs' bool
    if
        region-list-deallocate
        region-list-deallocate
    then

    \ Add B->B, adjacent F
    s" s1011->s1011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    \ Add to B->B
    s" s1011->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    \ Update A->A
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not returned true?" abort then

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-basic - Ok"
;

\ Set up an incompatible pair, then use updates to
\ change them to more sample needed, then compatible.
: action-test-check-incompatible-pairs-for-changed-square
    \ Init action.
    [ ' calc-result-x ] literal
    #4 0 action-new      \ act

    cr dup .action cr

    \ Add 4->4
    s" s0100->s0100" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    \ Add 1->3
    s" s0001->s0011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    \ Add 3->1
    s" s0011->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    \ Add 9->9
    s" s1001->s1001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if else ." Did not return true?" abort then

    cr dup .action cr

    \ Update square 9, to make it MSN with square F.
    s" s1001->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if
    else
        cr ." sample did not change sqr9?" abort
    then

    cr dup .action cr

    \ Update square F, to make it Compatible with square 9.
    s" s1111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    if
    else
        cr ." sample did not change sqrF?" abort
    then

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-check-incompatible-pairs-for-changed-square - Ok"
;

: action-tests
    action-test-basic
    action-test-check-incompatible-pairs-for-changed-square
    cr
;
