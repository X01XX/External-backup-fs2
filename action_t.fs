
: action-test-basic
    \ Init action.
    [ ' calc-result-x ] literal
    #4 0 0 action-new      \ act

    cr dup .action cr

    \ Add A->A
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1

    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    s" (rX0XX rXXX0 rX1XX rXXX1)" list-from-string-a    \ act reg-lst'
    over action-get-groups group-list-regions           \ act reg-lst' grp-regs'
    2dup region-lists-eq?                               \ act reg-lst' grp-regs' bool
    if
        region-list-deallocate
        region-list-deallocate
    else
        cr ." problem?"  abort cr
    then

    \ Add B->B, adjacent F
    s" s1011->s1011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool

    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add to B->B
    s" s1011->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Update A->A
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not returned true?" abort then

    cr dup .action cr

    \ Test results.
    dup action-get-adj-pairs list-get-length 2 <> abort" invalid number of adj pairs"
    dup action-get-adj-regions list-get-length 5 <> abort" invalid number of adj regions"
    dup action-get-nadj-pairs list-get-length 0 <> abort" invalid number of nadj pairs"
    dup action-get-nadj-regions list-get-length 1 <> abort" invalid number of nadj regions"
    dup action-get-states-in-one-region list-get-length 3 <> abort" invalid number of states in one region"
    dup action-get-defining-regions list-get-length 3 <> abort" invalid number of defining regions"
    dup action-get-corners list-get-length 3 <> abort" invalid number of corners"
    dup action-get-corner-clusters 1 <> abort" invalid number of corner clusters"

    \ Deallocate
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
    #4 0 0 action-new      \ act

    cr dup .action cr

    \ Add 4->4
    s" s0100->s0100" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    \ Add 1->3
    s" s0001->s0011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add 3->1
    s" s0011->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add 9->9
    s" s1001->s1001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Update square 9, to make it MSN with square F.
    s" s1001->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot
        cr ." sample did not change sqr9?" abort
    then

    cr dup .action cr

    \ Update square F, to make it Compatible with square 9.
    s" s1111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot
        cr ." sample did not change sqrF?" abort
    then

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-check-incompatible-pairs-for-changed-square - Ok"
;

: action-test-corners
    \ Init action.
    [ ' calc-result-x ] literal
    #4 0 0 action-new      \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add 8->A
    s" s1000->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Test result.
    dup action-get-corners                  \ act crns
    list-get-length                         \ act len
    1 <> abort" number corners ne 1?"

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-corners - Ok"
;

: action-test-calc-additional-corners
    \ Init action.
    [ ' calc-result-x ] literal
    #4 0 0 action-new      \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    \ Add D->F
    s" s1101->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    ifnot ." Did not return true?" abort then

    cr dup .action cr

    s" s0101" state-from-string-a           \ act sta5'
    dup                                     \ act sta5' sta5'
    #2 pick action-get-corners              \ act sta5' sta5' crn-lst
    corner-list-find                        \ act sta5', crn5 t | f
    invert abort" corner not found?"

    swap state-deallocate                   \ act crn5
    over action-calc-additional-corners     \ act crn-lst
    cr ." Additional corners: " dup .corner-list cr

    \ Deallocate
    corner-list-deallocate
    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-calc-additional-corners - Ok"
;

: action-tests
    action-test-basic
    action-test-check-incompatible-pairs-for-changed-square
    action-test-corners
    action-test-calc-additional-corners
    cr
;
