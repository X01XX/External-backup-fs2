\ Test action functions.

: action-test-new
    \ Run function..
    [ ' noop ] literal
    s" rXXXX 1" string-to-stack-a
    action-new              \ act

    \ Display results.
    cr dup .action cr

    \ Test results.
    dup action-get-num-bits #4 <> abort" num bits s/b 4?"
    dup action-get-inst-id 0<> abort" inst-id s/b 0?"
    dup action-get-dom-inst-id 1 <> abort" parent s/b 1?"

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-new - Ok"
;

: action-test-add-sample
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
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
        cr ." action-test-add-sample: action groups " dup .region-list space ." unexpected?" cr
        abort
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
    dup action-get-adj-pairs list-get-length #2 <> abort" invalid number of adj pairs"
    dup action-get-adj-regions list-get-length #5 <> abort" invalid number of adj regions"
    dup action-get-nadj-pairs list-get-length 0<> abort" invalid number of nadj pairs"
    dup action-get-nadj-regions list-get-length 1 <> abort" invalid number of nadj regions"
    dup action-get-states-in-one-region list-get-length #3 <> abort" invalid number of states in one region"
    dup action-get-defining-regions list-get-length #3 <> abort" invalid number of defining regions"
    dup action-get-corners list-get-length #3 <> abort" invalid number of corners"
    dup action-get-corner-clusters list-get-length 1 <> abort" invalid number of corner clusters"

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-add-sample - Ok"
;

\ Set up an incompatible pair, then use updates to
\ change them to more sample needed, then compatible.
: action-test-check-incompatible-pairs-for-changed-square
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    cr dup .action cr

    \ Add 4->4
    s" s0100->s0100" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    \ Add 1->3
    s" s0001->s0011" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Add 3->1
    s" s0011->s0001" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Add 9->9
    s" s1001->s1001" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    \ Add F->7
    s" s1111->s0111" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

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
    check-project-deallocated

    cr ." action-test-check-incompatible-pairs-for-changed-square - Ok"
;

: action-test-corners
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Add 8->A
    s" s1000->s1010" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Test result.
    dup action-get-corners                  \ act crns
    list-get-length                         \ act len
    1 <> abort" number corners ne 1?"

    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corners - Ok"
;

: action-test-corners2
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    cr dup .action cr

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    \ Add 7->F
    s" s0111->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Add D->F
    s" s1101->s1111" sample-from-string-a   \ act smpl1
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Test
    dup action-get-corner-clusters          \ act clstr-lst
    dup list-get-length                     \ act clstr-lst len
    1 <> abort" corner cluster list length ne 1?"

    dup list-get-first-item                 \ act clstr-lst clstr
    list-get-length                         \ act clstr-lst len
    #3 <> abort" corner cluster length ne 3?"
    drop                                    \ act

    \ Deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corners2 - Ok"
;

: ?action-test-anchor-discovery-corners
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    cr dup .action cr

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add 4->5
    s" s0100->s0101" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act bool

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add D->D
    s" s1101->s1101" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add A->A
    s" s1010->s1010" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add B->B
    s" s1011->s1011" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    dup action-get-corner-clusters          \ act crsc-lst
    list-get-length #2 <> abort" Corner clusters len not 2?"

    cr ." Getting actneeds ..." cr

    s" s0101" state-from-string-a           \ act sta'
    2dup swap                               \ act sta1 sta1 act
    ?action-calc-anchor-discovery-needs      \ act sta', ned-lst' bool
    if
        cr s" Needs: " #2 pick .struct-list-prefix cr

        \ Test needs.
        s" s0111" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-adisc                  \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s0111, for anchor discovery?"
        state-deallocate                    \ act sta' ned-lst'

        s" s1100" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-adisc                  \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s0111, for anchor discovery?"
        state-deallocate                    \ act sta' ned-lst'

        actneed-list-deallocate
    else
        cr ." No needs" cr
    then

    cr over .action cr

    \ Deallocate
    state-deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-anchor-discovery-corners - Ok"
;

: ?action-test-anchor-discovery-clusters
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    cr dup .action cr

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add D->D
    s" s1101->s1101" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add 7->7
    s" s0111->s0111" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    \ Add 8->7
    s" s1000->s0111" sample-from-string-a   \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    2dup swap action-add-sample drop        \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    dup action-get-corner-clusters          \ act crnc-lst
    list-is-empty? abort" No corner clusters?"

    cr ." Getting actneeds ..." cr

    s" s0101" state-from-string-a           \ act sta'
    2dup swap                               \ act sta' sta' act
    ?action-calc-anchor-discovery-needs      \ act sta', ned-lst' bool
    if
        cr s" Needs: " #2 pick .struct-list-prefix cr

        \ Test needs.
        s" s1100" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-adisc                  \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1100, for anchor discovery?"
        state-deallocate                    \ act sta' ned-lst'

        s" s1001" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-adisc                  \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1001, for anchor discovery?"
        state-deallocate                    \ act sta' ned-lst'

        actneed-list-deallocate
    else
        cr ." No needs" cr
    then

    cr over .action cr

    \ Deallocate
    state-deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-anchor-discovery-clusters - Ok"
;

: action-test-calc-resolve-uncertainty-needs
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    \ Add 1->1
    s" s0001->s0001" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 7->7
    s" s0111->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 3->A
    s" s0011->s1010" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->F
    s" s0110->s1111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 9->0
    s" s1001->s0000" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    \ Create fake domain current state.
    s" s1100" state-from-string-a swap          \ sta act
    2dup action-calc-resolve-uncertainty-needs  \ sta act, ned-lst' t | f
    if
        cr s" needs: " #2 pick .struct-list-prefix cr

        \ Test needs.
        dup list-get-length #2 < abort" needs len lt 2?"

        s" s1101" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1101?"
        state-deallocate                    \ act sta' ned-lst'

        s" s0100" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s0100?"
        state-deallocate                    \ act sta' ned-lst'

        actneed-list-deallocate
    else
        cr ." no needs found" cr abort
    then

    \ Deallocate.
    action-deallocate
    dup is-state?
    if
        state-deallocate
    else
        drop
    then

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-calc-resolve-uncertainty-needs - Ok"
;

\ Take an actionstep that produces an unexpected result,
\ update an existing square.
: action-test-take-step-1
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    0 s" s0100" state-from-string-a s" s0101" state-from-string-a 0 0 actionstep-new
                                            \ act actstp
    swap                                    \ actstp act
    cr ." actionstep: " over .actionstep cr

    2dup action-take-step                   \ actstp act smpl'

    cr over .action cr

    \ Test result.
    over action-get-squares list-get-length #2 <> abort" s/b two squares"
    s" s0101" state-from-string-a dup       \ actstp act smpl' sta5 sta5
    #3 pick action-get-squares              \ actstp act smpl' sta5 sta5 sqr-lst
    square-list-find                        \ actstp act smpl' sta5, sqr5 t | f
    invert abort" square not found?"

    square-get-pn #2 <> abort" sqr 5 pn not 2?"

    \ Deallocate.
    state-deallocate
    sample-deallocate
    action-deallocate
    actionstep-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-take-step-1 - Ok"
;

\ Take an actionstep that produces an unexpected result,
\ Create a new square.
: action-test-take-step-2
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    0 s" s0110" state-from-string-a s" s0111" state-from-string-a 0 0 actionstep-new
                                            \ act actstp
    swap                                    \ actstp act
    cr ." actionstep: " over .actionstep cr

    2dup action-take-step                   \ actstp act smpl'

    cr over .action cr

    \ Test result.
    over action-get-squares list-get-length #3 <> abort" s/b three squares"

    \ Deallocate.
    sample-deallocate
    action-deallocate
    actionstep-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-take-step-2 - Ok"
;

\ Take an actionstep that produces an unexpected result,
\ don't create a new square, due to the alternate possible result being matched.
: action-test-take-step-3
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                              \ act

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    cr dup .action cr

    s" s0111" state-from-string-a s" s0110" state-from-string-a s" s0111" state-from-string-a 0 0 actionstep-new
                                            \ act actstp
    swap                                    \ actstp act
    cr ." actionstep: " over .actionstep cr

    2dup action-take-step                   \ actstp act smpl'

    cr over .action cr

    \ Test result.
    over action-get-squares list-get-length #2 <> abort" s/b two squares"

    \ Deallocate.
    sample-deallocate
    action-deallocate
    actionstep-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-take-step-3 - Ok"
;

: action-test-corner-cluster-calc
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                                                      \ act

    \ Init adjacent, dissimilar, pair list.
    s" (rx100 r01x0 r1x01 rX001 r00x1 r011x r0x11 r110x r1X11 r00X0)"
    list-from-string-a                  \ act' pr-lst'
    cr ." Incompatible Pairs: " dup .struct-list cr

    \ Calc regions from pairs.
    2dup swap                                       \ act' pr-lst' pr-lst1 act'
    action-calc-possible-regions-from-dis-pairs     \ act' pr-lst' pos-regs'
    cr ." Possible regions: " dup .region-list cr

    #2 pick _action-update-adj-regions              \ act' pr-lst'
    over _action-update-adj-pairs                   \ act'

    dup action-corner-cluster-calc                  \ act'
    dup action-get-corner-clusters                  \ act' crnclstrs

    \ Test cluster list.
    dup list-get-length 1 <> abort" more than one cluster?"
    dup list-get-first-item list-get-length #8 <> abort" not 8 corners?"

    foreach                                         \ act' crnclstrs-lnk crnclstr-x
        cr s" Corner cluster: " #2 pick .struct-list-prefix cr
        drop
    next-item

    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corner-cluster-calc - Ok"
;

: action-test-corner-cluster-calc2
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                                                      \ act

    \ Init adjacent, dissimilar, pair list.
    s" (rx000 r00x0 r111x r1x11)"
    list-from-string-a                  \ act' pr-lst'
    cr ." Incompatible Pairs: " dup .struct-list cr

    \ Calc regions from pairs.
    2dup swap                                       \ act' pr-lst' pr-lst1 act'
    action-calc-possible-regions-from-dis-pairs     \ act' pr-lst' pos-regs'
    cr ." Possible regions: " dup .region-list cr

    dup region-list-defining-regions
    cr ." Defining regions: " dup .region-list cr
    region-list-deallocate

    #2 pick _action-update-adj-regions              \ act' pr-lst'
    over _action-update-adj-pairs                   \ act'

    dup action-corner-cluster-calc                  \ act'
    dup action-get-corner-clusters                  \ act' crnclstrs

    cr s" Corner Clusters: " #2 pick .struct-list-prefix cr

    \ Test cluster list.
    dup list-get-length #2 <> abort" not two clusters?"
    drop

    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corner-cluster-calc2 - Ok"
;

: action-test-corner-cluster-calc3
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                                                      \ act

    \ Add 1->1
    s" s0001->s0001" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 7->7
    s" s0111->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 3->A
    s" s0011->s1010" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 6->F
    s" s0110->s1111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 9->0
    s" s1001->s0000" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    dup .action

    s" s0101" state-from-string-a               \ act' sta'
    dup                                         \ act' sta' sta'
    #2 pick action-calc-resolve-uncertainty-needs   \ act' sta' ned-lst' t | f
    if
        cr s" Needs: " #2 pick .struct-list-prefix cr
        struct-list-deallocate
    else
        cr ." Needs: None" cr
    then

    \ State would normally be kept alive by storage in a domain.
    dup is-state?
    if
        state-deallocate
    else
        drop
    then

    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corner-cluster-calc3 - Ok"
;

\ From possible regions:
\
\   Calc defining regions.
\
\   Calc defining parts of defining regions.
\
\   Calc congruent intersections of adjacent defining parts of adjacent defining regions, areas
\        where adjacent pairs of dissimilar anchors might exist.
\        Where more than one pair may exist, the congruent region has more than one X-bit, that
\        pair has yet to be chosen.
\
\ Using a state from an existing incompatible pair, in only one region, choose the following states
\ from the congruent intersections, to form corners that describe the defining regions.
\
\ In cases where the defining regions were calculated by chance, non-optimal samples, the
\ corners will suggest a more optimal set of samples, some of which may need to be sampled.
\
: action-test-adjacent-defining-parts
    \ s" (r0x0x rx1x1 r10xx rxx10 r1xx0 rx01x)" region-list-from-string-a     \ pos-lst'
     s" (rxx00 rxx01 rxx11 rxx10)" region-list-from-string-a     \ pos-lst'
    cr cr ." pos-regs: " dup .region-list cr

    dup region-list-defining-regions                                        \ pos-lst' def-regs'
    cr ." def-regs: " dup .region-list

    over region-list-defining-region-parts-per-region                       \ pos-lst' def-regs', def-prts' t | f
    invert abort" no defining parts?"
    cr s" def reg def prts: " #2 pick .struct-list-prefix

    \ Clean up.
    rot region-list-deallocate                                              \ def-regs' def-prts'

    \ Init congruent adjacent regions.
    list-new                                        \ def-regs' def-prts' cngr-regs'

    \ Check each possible pair for adjacency,
    \ if so, check adjacency of defining parts.
    over                                            \ def-regs' def-prts' cngr-regs' def-prts'
    foreach                                         \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x
        \ Compare current region with all following regions.
        over link-get-next                          \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt
        begin
            ?dup
        while
            dup link-get-data list-get-first-item   \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt reg-n
            #2 pick list-get-first-item             \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt reg-n reg-x
            regions-adjacent?                       \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt bool

            if
                dup link-get-data                   \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n

                \ cr #2 pick list-get-first-item .region space ." adjacent to " dup list-get-first-item .region

                dup list-get-second-item            \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n regs-n
                \ space ." check " dup .region-list
                #3 pick list-get-second-item        \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n regs-n regsx
                \ space ." and " dup .region-list
                region-list-congruent-regions       \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n, cngr-regs2' t | f
                if
                    \ space ." congruent " dup .region-list
                    dup                             \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n cngr-regs2' cngr-regs2'
                    #6 pick                         \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n cngr-regs2' cngr-regs2' cngr-regs'
                    region-list-append-nosubs       \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n cngr-regs2'
                    region-list-deallocate          \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt def-prts-n
                    \ cr
                else
                    \ cr
                then
                drop                                \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x def-prts-lnk-nxt
            then

            link-get-next
        repeat
                                                    \ def-regs' def-prts' cngr-regs' def-prts-lnk def-prts-x
        drop                                        \ def-regs' def-prts' cngr-regs' def-prts-lnk
    next-item
                                                    \ def-regs' def-prts' cngr-regs'
    cr ." cngrs: " dup .region-list cr

    \ Clean up.
    swap struct-list-deallocate                     \ def-prts' cngr-regs'

    \ Collect congruent regions per defining regions.

    \ Init list.
    list-new swap                                   \ def-prts' def-cnrgrs' cngr-regs'
    #2 pick                                         \ def-prts' def-cnrgrs' cngr-regs' def-regs'
    foreach                                         \ def-prts' def-cnrgrs' cngr-regs' def-lnk dregx
        #2 pick                                     \ def-prts' def-cnrgrs' cngr-regs' def-lnk dregx cngr-regs'
        [ ' regions-intersect? ] literal -rot       \ def-prts' def-cnrgrs' cngr-regs' def-lnk xt dregx cngr-regs'
        list-find-all-struct                        \ def-prts' def-cnrgrs' cngr-regs' def-lnk int-regs'

        \ Save region, int regs.
        list-new tuck list-push-struct              \ def-prts' def-cnrgrs' cngr-regs' def-lnk reg-cngrs'
        over link-get-data                          \ def-prts' def-cnrgrs' cngr-regs' def-lnk reg-cngrs' dregx
        over list-push-struct                       \ def-prts' def-cnrgrs' cngr-regs' def-lnk reg-cngrs'
        #3 pick list-push-struct                    \ def-prts' def-cnrgrs' cngr-regs' def-lnk
    next-item
                                                    \ def-prts' def-cnrgrs' cngr-regs'
    \ Clean up.
    region-list-deallocate                          \ def-prts' def-cnrgrs'
    swap region-list-deallocate                     \ def-cnrgrs'

    cr s" def reg / congr ints: " #2 pick .struct-list-prefix

    \ Collect defining regions and their corner templates.

    \ Init defregcts list.
    list-new                                        \ def-cnrgrs' dfrgcts-lst'
    over                                            \ def-cnrgrs' dfrgcts-lst' def-cnrgrs'
    foreach                                         \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        \ cr ." def-cngrsx: " dup .struct-list
        \ dup list-get-first-item space .region
        \ dup list-get-first-item region-number-edges space ." edgs " dec.

        dup list-get-first-item region-number-edges
        over list-get-second-item list-get-length   \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs len
        over <                                      \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs bool
        if
            drop                                    \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        else
            over list-get-second-item               \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs cngrsx
            any-x-of-n                              \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol'
            \ dup .struct-list
            \ cr

            \ Check each combination of congruent intersections for a common intersection.

            \ Init corner template list.
            list-new                                \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst'
            over
            foreach                                 \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk crgrsx
                region-list-common-intersection     \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk, reg t | f
                if
                    \ Make corner template.
                    over link-get-data swap         \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk crgrsx reg
                    cornertemplate-new              \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk, ct' t | f
                    invert abort" cornertemplate-new failed?"

                    \ Store corner template.
                    #2 pick                         \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk ct' ct-lst'
                    list-push-struct                \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk
                then
            next-item
                                                    \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst'
            \ cr #12 spaces dup .struct-list cr

            \ Check corner template list for the defining region.
            dup list-is-empty?
            if
                list-deallocate
            else
                \ Make DefRegCts
                #2 pick                             \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' def-cngrsx
                list-get-first-item                 \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' def-reg
                defregcts-new                       \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' dfregctsx
                \ cr #12 spaces dup .defregcts cr
                #4 pick                             \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' dfregctsx dfrgcts-lst'
                list-push-struct                    \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol'
            then
        then
        struct-list-deallocate                      \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
                                                    \ def-cnrgrs' dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        drop
    next-item
                                                    \ def-cnrgrs' dfrgcts-lst'
    cr s" def reg cts: " #2 pick .struct-list-prefix

    dup
    defregcts-list-cornertemplate-clusters          \ def-cnrgrs' dfrgcts-lst', crn-clstrs' t | f

    if
        cr s" Corner template clusters: " #2 pick .struct-list-prefix

        s" s0100" state-from-string-a               \ def-cnrgrs' dfrgcts-lst', crn-clstrs' stax
        dup                                         \ def-cnrgrs' dfrgcts-lst', crn-clstrs' stax stax
        #2 pick                                     \ def-cnrgrs' dfrgcts-lst', crn-clstrs' stax stax crn-clstrs'
        list-get-first-item                         \ def-cnrgrs' dfrgcts-lst', crn-clstrs' stax stax crn-clstr-f
        cornertemplate-list-predict-all-states      \ def-cnrgrs' dfrgcts-lst', crn-clstrs' stax, sta-lst' t | f
        if
            cr ." Possible state list from: " dup list-get-first-item .state space ." across corners is: " dup .state-list

            nip                                             \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst'
            dup                                             \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst' sta-lst'
            #3 pick                                         \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst' sta-lst' dfrgcts-lst'
            defregcts-list-state-list-contains-all-anchors? \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst' bool
            if
                space ." anchors match defregcts list "
                dup                                     \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst' sta-lst'
                #3 pick                                 \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst' sta-lst' dfrgcts-lst'
                defregcts-list-corners-from-state-list  \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst', crn-lst' t | f
                invert abort" corners from states failed?"
                cr s" All corners: " #2 pick .struct-list-prefix
                struct-list-deallocate                  \ def-cnrgrs' dfrgcts-lst' crn-clstrs' sta-lst'
            else
                space ." anchors do NOT match defregcts list"
            then

            state-list-deallocate                       \ def-cnrgrs' dfrgcts-lst' crn-clstrs'
        else
            state-deallocate                            \ def-cnrgrs' dfrgcts-lst' crn-clstrs'
            cr ." no states found?" cr abort
        then
        struct-list-deallocate                          \ def-cnrgrs' dfrgcts-lst'
    else
        cr ." no clusters"
    then

    \ Clean up.                                         \ def-cnrgrs' dfrgcts-lst'
    struct-list-deallocate                              \ def-cnrgrs'
    struct-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-adjacent-defining-parts - Ok"
;

: action-test-calc-minimum-corners
    \ Init action.
    [ ' noop ] literal
    s" rXXXX 0" string-to-stack-a
    action-new                                                      \ act

    \ Add 0->0
    s" s0000->s0000" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Form XX00.

    \ Add 1->E
    s" s0001->s1110" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 2->D
    s" s0010->s1101" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Form XX01.

    \ Add 5->A
    s" s0101->s1010" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 4->4
    s" s0100->s0100" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 7->7
    s" s0111->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Form XX11.

    \ Add F->F
    s" s1111->s1111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add D->2
    s" s1101->s0010" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add E->1
    s" s1110->s0001" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Form XX10.

    \ Add A->5
    s" s1010->s0101" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add B->B
    s" s1011->s1011" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    \ Add 8->8
    s" s1000->s1000" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act

    dup .action

    \ Run function.
    dup action-calc-minimum-corners         \ crn-clstrs t | f
    if
        cr s" result: " #2 pick .struct-list-prefix cr
        struct-list-deallocate
    then

    \ Deallocate.
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-calc-minimum-corners - Ok"
;

: action-tests
    action-test-new
    action-test-add-sample
    action-test-check-incompatible-pairs-for-changed-square
    action-test-corners
    action-test-corners2
\    action-test-anchor-discovery-corners
\    action-test-anchor-discovery-clusters
    action-test-calc-resolve-uncertainty-needs
    action-test-take-step-1
    action-test-take-step-2
    action-test-take-step-3
    action-test-corner-cluster-calc
    action-test-corner-cluster-calc2
    action-test-corner-cluster-calc3
    action-test-adjacent-defining-parts
    cr
;
