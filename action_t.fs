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

: action-test-anchor-discovery-corners
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
    over action-add-sample drop             \ act bool

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
    list-is-not-empty? abort" Corner clusters?"

    cr ." Getting actneeds ..." cr

    s" s0101" state-from-string-a           \ act sta'
    2dup swap                               \ act sta1 sta1 act
    action-calc-anchor-discovery-needs      \ act sta', ned-lst' bool
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

: action-test-anchor-discovery-clusters
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
    over action-add-sample drop             \ act bool

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
    action-calc-anchor-discovery-needs      \ act sta', ned-lst' bool
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

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    cr dup .action cr

    \ Add 5->4
    s" s0101->s0100" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    cr dup .action cr

    \ Add 6->E
    s" s0110->s1110" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    cr dup .action cr

    \ Create fake domain current state.
    s" s1100" state-from-string-a swap          \ sta act
    2dup action-calc-resolve-uncertainty-needs  \ sta act, ned-lst' t | f
    if
        cr ." needs: " dup .struct-list cr

        \ Test needs.
        dup list-get-length #2 <> abort" needs len ne 2?"

        s" s1100" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1100?"
        state-deallocate                    \ act sta' ned-lst'

        s" s1111" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1111?"
        state-deallocate                    \ act sta' ned-lst'

        actneed-list-deallocate
    else
        cr ." no needs found" cr abort
    then

    \ Add a single-sample B->B to XX11.
    \ Add B->B
    s" s1011->s1011" sample-from-string-a   \ sta  act smpl
    over action-add-sample drop             \ sta act bool

    cr dup .action cr

    2dup action-calc-resolve-uncertainty-needs  \ sta act, ned-lst' t | f
    if
        cr ." needs: " dup .struct-list cr

        \ Test needs.
        dup list-get-length #2 <> abort" needs len ne 2?"

        s" s1100" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1100?"
        state-deallocate                    \ act sta' ned-lst'

        s" s1011" state-from-string-a dup   \ act sta' ned-lst' stax' stax'
        actneed-type-resunc                 \ act sta' ned-lst' stax' stax' typ
        #3 pick                             \ act sta' ned-lst' stax' stax' typ ned-lst'
        actneed-list-contains               \ act sta' ned-lst' stax' bool
        invert abort" actneed list does not contain need for s1011?"
        state-deallocate                    \ act sta' ned-lst'

        actneed-list-deallocate
    else
        cr ." no needs found" cr abort
    then

    \ Deallocate.
    action-deallocate
    state-deallocate

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
    over action-add-sample drop             \ act bool

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

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
    over action-add-sample drop             \ act bool

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

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
    over action-add-sample drop             \ act bool

    \ Add 5->5
    s" s0101->s0101" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    \ Add 6->7
    s" s0110->s0111" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

    \ Add 6->6
    s" s0110->s0110" sample-from-string-a   \ act smpl
    over action-add-sample drop             \ act bool

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

\ Return ~a + ~b regions from a pair list.
: action-calc-regions-from-dis-pairs ( pr-lst1 act0 -- reg-lst )
    list-new                                \ pr-lst1 act0 def-regs
    over action-get-num-bits                \ pr-lst1 act0 def-regs nb
    region-max-x                            \ pr-lst1 act0 def-regs reg-max
    over list-push-struct                   \ pr-lst1 act0 def-regs

    rot                                     \ act0 def-regs pr-lst1

    foreach                                 \ act0 def-regs pr-lnk regx
        region-get-states                   \ act0 def-regs pr-lnk sta1 sta1
        state-~a+~b                         \ act0 def-regs pr-lnk reg-lst'
        dup                                 \ act0 def-regs pr-lnk reg-lst' reg-lst'
        #3 pick                             \ act0 def-regs pr-lnk reg-lst' reg-lst' def-regs
        region-list-intersections-nosubs    \ act0 def-regs pr-lnk reg-lst' def-regs2

        \ Clean up.
        swap region-list-deallocate         \ act0 def-regs pr-lnk def-regs2
        rot region-list-deallocate          \ act0 pr-lnk def-regs2
        swap                                \ act0 def-regs2 pr-lnk
    next-item
                                            \ act0 def-regs
    nip                                     \ def-regs
;

\ Return the defining regions from the intersection of a list of region lists.
: action-intersect-lol-regions ( regs-lol1 act0 -- def-regs )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-list? )

    \ Init work list.
    dup action-get-max-region               \ regs-lol1 act0 max-reg
    list-new tuck list-push-struct          \ regs-lol1 act0 wrk-lst'

    rot                                     \ act0 wrk-lst' regs-lol1
    foreach                                 \ act0 wrk-lst' regs-lol1-lnk reg-lst-x
        #2 pick                             \ act0 wrk-lst' regs-lol1-lnk reg-lst-x wrk-lst'
        region-list-intersections-nosubs    \ act0 wrk-lst' regs-lol1-lnk wrk-lst-nxt'
        rot region-list-deallocate          \ act0 regs-lol1-lnk wrk-lst2'
        swap                                \ act0 wrk-lst-nxt' regs-lol1-lnk
    next-item
                                            \ act0 wrk-lst'
    dup region-list-defining-regions        \ act0 wrk-lst' def-regs'
    swap region-list-deallocate             \ act0 def-regs'
    nip                                     \ def-regs'
;


\ Calc possible regions for each pair cluster, keep correspondence to list position.
: action-pair-clusters-to-possible-region-clusters ( pr-lol1 act0 -- reg-lol )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-list? )

    \ Init result list.
    list-new                                \ pr-lol1 act0 pos-reg-clstrs'

    rot                                     \ act0 pos-reg-clstrs' pr-lol1
    foreach                                 \ act0 pos-reg-clstrs' pr-lol-lnk pr-lst-x
        \ Calc possible regions for current pair cluster.
        #3 pick                             \ act0 pos-reg-clstrs' pr-lol-lnk pr-lst-x act0
        action-calc-regions-from-dis-pairs  \ act0 pos-reg-clstrs' pr-lol-lnk pos-regs'

        \ Store cluster.
        #2 pick                             \ act0 pos-reg-clstrs' pr-lol-lnk pos-regs' pos-reg-clstrs'
        list-push-end-struct                \ act0 pos-reg-clstrs' pr-lol-lnk
    next-item
                                            \ act0 pos-reg-clstrs'
    nip
;

\ Given a list of defining regions, and a list of incompatible pair clusters, return
\ a list of incompatible pair clusters that is one shorter, and still
\ results in the same list of defining pairs.
: action-shorten-pair-cluster-list ( defining-regions2 pair-lol1 act0 -- pair-lol t | f )

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
    2dup swap                           \ act' pr-lst' pr-lst1 act'
    action-calc-regions-from-dis-pairs  \ act' pr-lst' reg-lst1'
    cr ." Pair regions: " dup .region-list cr

    \ Get defining regions.
    dup region-list-defining-regions    \ act' pr-lst' reg-lst1' def-regs'
    cr ." Defining regions: " dup .region-list cr

    \ Init state cluster list.
    list-new                            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs'

    \ Get states list.
    #3 pick region-list-states          \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst'
    cr ." States: " dup .struct-list cr


    begin
        dup list-is-not-empty?
    while
        \ Choose a random state to start a state list.
        dup list-get-length                 \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' u
        random                              \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' inx
        over list-get-item                  \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta
        list-new tuck list-push-struct      \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr'

        \ Find states connected to states, using the pair list.
        \ The list grows as new states are added to the end, increasing the number of loop cycles.
        dup                                 \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr'
        foreach                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk stax
            #7 pick                         \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk stax pr-lst'
            region-list-states-connected-to \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk sta-lst2'

            \ Add states to list, no dups.
            dup                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk sta-lst2' sta-lst2'
            #3 pick                         \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk sta-lst2' sta-lst2' sta-clstr'
            state-list-append-end-nodups    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk sta-lst2'
            state-list-deallocate           \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-clstr-lnk
        next-item
                                        \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr'

        \ Get states not collected, for the next loop cycle.
        [ ' states-eq? ] literal            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' xt
        #2 pick #2 pick                     \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' xt sta-lst' sta-clstr'
        list-difference-struct              \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-clstr' sta-dif'

        \ Store the state cluster.
        swap                                \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-dif' sta-clstr'
        #3 pick                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-dif' sta-clstr' sta-clstrs'
        list-push-struct                    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-lst' sta-dif'

        \ Make new state list to parse out more clusters.
        swap state-list-deallocate          \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' sta-dif'
    repeat

    list-deallocate                         \ act' pr-lst' reg-lst1' def-regs' sta-clstrs'

    cr ." State clusters: " dup .struct-list cr

    \ Convert state clusters into adjacent pair clusters.

    \ Init pair clusters list.
    list-new                                \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs'
    over                                    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' sta-clstrs'

    foreach                                 \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' sta-clstrs-lnk clstr
        \ Convert state cluster to pair cluster.
        state-list-to-pair-list             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' sta-clstrs-lnk prs'

        \ Store pair cluster.
        #2 pick                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' sta-clstrs-lnk prs' pr-clstrs'
        list-push-struct                    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' sta-clstrs-lnk
    next-item
                                            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs'
    cr ." Pair clusters: " dup .struct-list cr

    \ Calc possible regions for each pair cluster, keep correspondence to list position.

    \ Init pairdata list.
    list-new                                \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst'

    over                                    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' pr-clstrs'
    foreach                                 \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' pr-clstr-lnk clstr-x
        \ Calc possible regions for current pair cluster.
        pairdata-new                        \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' pr-clstr-lnk prdt-x'

        \ Store pairdata.
        #2 pick                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' pr-clstr-lnk prdt-x' prdt-lst'
        list-push-end-struct                \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' pr-clstr-lnk
    next-item
                                            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst'

    cr s" Pairdata: " #2 pick .struct-list-prefix cr

    \ Try removing an unneeded pairdata instance.
    dup
    pairdata-list-remove-unneeded-instances \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst', prdt-lst2' t | f
    if
        cr s" Pairdata: 2: " #2 pick .struct-list-prefix cr
        swap pairdata-list-deallocate       \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst2'
    else
        cr ." No unneeded instance found" cr
    then

    \ Get corner list(s).
\ cr ." at a: " .stack cr
    #3 pick                                 \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' def-regs'
    over                                    \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' def-regs' prdt-lst'
\ cr ." at b: " .stack cr
    pairdata-list-to-corner-list            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' crnclstr-lst'
\ cr ." at c: " .stack cr
    dup foreach                             \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' crnclstr-lst' crnclstr-lnk crnclstr-x
        cr s" Corner cluster: " #2 pick .struct-list-prefix cr
        drop
    next-item
                                            \ act' pr-lst' reg-lst1' def-regs' sta-clstrs' pr-clstrs' prdt-lst' crnclstr-lst'
\ cr ." at z: " .stack cr
    \ Deallocate.
    struct-list-deallocate
    pairdata-list-deallocate
    struct-list-deallocate
    struct-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    action-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." action-test-corner-cluster-calc - Ok"
;

: action-tests
    action-test-new
    action-test-add-sample
    action-test-check-incompatible-pairs-for-changed-square
    action-test-corners
    action-test-corners2
    action-test-anchor-discovery-corners
    action-test-anchor-discovery-clusters
    action-test-calc-resolve-uncertainty-needs
    action-test-take-step-1
    action-test-take-step-2
    action-test-take-step-3
    \ action-test-corner-cluster-calc
    cr
;
