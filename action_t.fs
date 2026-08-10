\ Test action functions.

: action-test-new
    \ Run function..
    [ ' calc-result-x ] literal
    s" rXXXX 0 0" string-to-stack
    action-new              \ act

    \ Display results.
    cr dup .action cr

    \ Test results.
    dup action-get-num-bits #4 <> abort" num bits s/b 4?"
    dup action-get-inst-id 0<> abort" inst-id s/b 0?"
    dup action-get-parent 0<> abort" parent s/b 0?"

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-new - Ok"
;

: action-test-add-sample
    \ Init action.
    [ ' calc-result-x ] literal
    s" rXXXX 0 0" string-to-stack
    action-new                              \ act

    cr dup .action cr

    \ Add A->A, create a new square.
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Add F->7, create a new square.
    s" s1111->s0111" sample-from-string-a   \ act smpl1

    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

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

    \ Add B->B, adjacent F, create a new square.
    s" s1011->s1011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool

    invert abort" Did not return true?"

    cr dup .action cr

    \ Add to B->B, update an existing square.
    s" s1011->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Update A->A, update an existing square.
    s" s1010->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not returned true?"

    cr dup .action cr

    \ Test results.
    dup action-get-adj-pairs list-get-length 2 <> abort" invalid number of adj pairs"
    dup action-get-adj-regions list-get-length 5 <> abort" invalid number of adj regions"
    dup action-get-nadj-pairs list-get-length 0 <> abort" invalid number of nadj pairs"
    dup action-get-nadj-regions list-get-length 1 <> abort" invalid number of nadj regions"
    dup action-get-states-in-one-region list-get-length 3 <> abort" invalid number of states in one region"
    dup action-get-defining-regions list-get-length 3 <> abort" invalid number of defining regions"
    dup action-get-corners list-get-length 3 <> abort" invalid number of corners"
    dup action-get-corner-clusters list-get-length 1 <> abort" invalid number of corner clusters"

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-add-sample - Ok"
;

\ Set up an incompatible pair, then use updates to
\ change them to more sample needed, then compatible.
: action-test-check-incompatible-pairs-for-changed-square
    \ Init action.
    [ ' calc-result-x ] literal
    s" rXXXX 0 0" string-to-stack
    action-new                              \ act

    cr dup .action cr

    \ Add 4->4
    s" s0100->s0100" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    \ Add 1->3
    s" s0001->s0011" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Add 3->1
    s" s0011->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Add 9->9
    s" s1001->s1001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Update square 9, to make it MSN with square F.
    s" s1001->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" sample did not change sqr9?"

    cr dup .action cr

    \ Update square F, to make it Compatible with square 9.
    s" s1111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" sample did not change sqrF?"

    cr dup .action cr

    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-check-incompatible-pairs-for-changed-square - Ok"
;

: action-test-corners
    \ Init action.
    [ ' calc-result-x ] literal
    s" rXXXX 0 0" string-to-stack
    action-new                              \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Add 8->A
    s" s1000->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

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

: action-test-corners2
    \ Init action.
    [ ' calc-result-x ] literal
    s" rXXXX 0 0" string-to-stack
    action-new                              \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Add D->F
    s" s1101->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample                  \ act bool
    invert abort" Did not return true?"

    cr dup .action cr

    \ Test
    dup action-get-corner-clusters          \ act clstr-lst
    dup list-get-length                     \ act clstr-lst len
    1 <> abort" corner cluster list length ne 1?"

    dup list-get-first-item                 \ act clstr-lst clstr
    list-get-length                         \ act clstr-lst len
    3 <> abort" corner cluster length ne 3?"
    drop                                    \ act

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." action-test-corners2 - Ok"
;

: action-tests
    action-test-new
    action-test-add-sample
    action-test-check-incompatible-pairs-for-changed-square
    action-test-corners
    action-test-corners2
    cr
;
