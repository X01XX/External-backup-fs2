\ Implement a group struct and functions.

#43717 constant group-struct-id
    #5 constant group-struct-number-cells

\ Struct fields
0                           constant group-header-disp      \ 16 bits, [0] struct id, [1] use count (16),
                                                            \ [2] pnc (8 bits) Action instance ID ( 8 bits )
                                                            \ [3] Domain instance ID ( 8 bits )
group-header-disp   cell+   constant group-region-disp      \ The group region.
group-region-disp   cell+   constant group-s-region-disp    \ A Region covered by the group's base-pn squares, often a proper subset of the group-region.
group-s-region-disp cell+   constant group-squares-disp     \ A square-list.
group-squares-disp  cell+   constant group-rules-disp       \ A rule-list.

0 value group-mma \ Storage for group mma instance.

\ Init group mma, return the addr of allocated memory.
: group-mma-init ( num-items -- ) \ sets group-mma.
    dup 1 <
    if
        ." group-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Group store."
    group-struct-number-cells swap mma-new to group-mma
;

\ Check if tos is an allocated group.
: is-group? ( addr -- bool )
    dup group-mma mma-is-item?  \ addr bool
    if
        struct-get-id
        group-struct-id =       \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the group action id.
: group-get-act-inst-id ( grp0 -- id )
    \ Check arg.
    assert( tos is-group? )

    5c@                 \ Fetch the field.
;

\ Set the group action id.
\ Do not inc parent use count.
: _group-set-act-inst-id ( id1 grp0 -- )
    5c!
;

\ Return the group domain id.
: group-get-dom-inst-id ( grp0 -- id )
    \ Check arg.
    assert( tos is-group? )

    6c@                 \ Fetch the field.
;

\ Set the group domain id.
: _group-set-dom-inst-id ( id1 grp0 -- )
    6c!
;

\ Return the group region.
: group-get-region ( grp0 -- reg )
    \ Check arg.
    assert( tos is-group? )

    group-region-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set the region of a group instance, use only in this file.
: _group-set-region ( reg1 grp0 -- )
    group-region-disp + \ Add offset.
    !struct             \ Set the field.
;

\ Return the group squares region.
: group-get-s-region ( grp0 -- reg )
    \ Check arg.
    assert( tos is-group? )

    group-s-region-disp +   \ Add offset.group-check-changed-square
    @                       \ Fetch the field.
;

\ Set the square region of a group instance, use only in this file.
: _group-set-s-region ( reg1 grp0 -- )
    group-s-region-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return group's pnc value.
: group-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert( tos is-group? )

    4c@
    0<>     \ Change 255 to -1
;

: _group-set-pnc ( bool sqr -- )
    4c!
;

: group-get-rules ( sqr0 -- rul-lst )
    \ Check arg.
    assert( tos is-group? )

    group-rules-disp + @
;

: _group-set-rules ( rul-lst1 sqr0 -- )
    group-rules-disp +
    !struct
;

\ Return the group squares.
: group-get-squares ( grp0 -- reg )
    \ Check arg.
    assert( tos is-group? )

    group-squares-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the squares field of a group instance, use only in this file.
: _group-set-squares ( sqr-lst grp0 -- )
    group-squares-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return a group's pn value.
: group-get-pn ( grp0 -- pn )
    \ Check arg.
    assert( tos is-group? )

    group-get-rules
    list-get-length
;

\ End accessors.

\ Calc, and set, group s-region, based on group square list.
: _group-calc-set-s-region ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    dup group-get-squares       \ grp0 sqr-lst
    square-list-region          \ grp0, r-reg t | f
    0= abort" _group-calc-set-s-region: group s-region not found?"

    dup                         \ grp0 r-reg r-reg
    #2 pick group-get-region    \ grp0 r-reg r-reg g-reg
    region-superset?            \ grp0 r-reg bool
    if
        swap _group-set-s-region
    else
        cr ." _group-calc-set-s-region: s-region not subset group region?"
    then
;

\ Replace current s-region with new region.
: _group-update-s-region ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    dup group-get-s-region swap \ reg-old reg1 grp0
    _group-calc-set-s-region    \ reg-old
    region-deallocate           \ Deallocate last, so struct field is never invalid.
;

\ Calc, and set, group rules, based on group square list.
: _group-calc-set-rules ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    dup group-get-squares   \ grp0 sqr-lst
    square-list-calc-rules  \ grp0, ruls t | f
    if
        swap                \ ruls grp0
        _group-set-rules
    else
        cr ." Invalid group? " group-get-region .region abort
    then
;

\ Replace current rules with new rules.
: _group-update-rules ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    dup group-get-rules swap    \ ruls-old grp0
    _group-calc-set-rules       \ ruls-old
    rule-list-deallocate        \ Deallocate last, so struct field is never invalid.
;

\ Calc, and set, group pnc, based on group square list.
: _group-calc-set-pnc ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    \ Check if group region EQ group s-region.
    dup group-get-region            \ grp0 reg
    over group-get-s-region         \ grp0 reg r-reg
    regions-eq?                     \ grp0 bool
    ifnot
        false
        swap                        \ false grp0
        _group-set-pnc
        exit
    then

    \ Look for a pair of pnc squares.
    dup group-get-squares               \ grp0 sqr-lst
    square-list-pnc-squares             \ grp0, pnc-lst' t | f
    if
        dup square-list-region          \ grp0, pnc-lst', pnc-reg' t | f
        if
            swap square-list-deallocate \ grp0 pnc-reg'
            2dup swap                   \ grp0 pnc-reg' pnc-reg' grp0
            group-get-region            \ grp0 pnc-reg' pnc-reg' grp-reg
            regions-eq?                 \ grp0 pnc-reg' bool
            swap region-deallocate      \ grp0 bool
            if
                true
                swap                    \ false grp0
                _group-set-pnc
            else
                false
                swap                    \ false grp0
                _group-set-pnc
            then
        else
            true abort" unexpected"
        then
    else
        false
        swap                        \ false grp0
        _group-set-pnc
    then
;

\ Return a new group, given a domain id, action id, region and square-list.
\ Return false if the given square list is empty,
\ or contains an incompatible pair,
\ or are not all within the given region.
\
\ Due to the incompatible pairs logic, every group region will match
\ a possible region, and no squares in the group will be incompatible.
: group-new    ( sqrs4 reg3 action-inst-id2 domain-inst-id1 -- grp t | f )
    \ Check args.
    assert( tos 0 >= )
    assert( tos 256 < )
    assert( nos 0 >= )
    assert( nos 256 < )
    assert( 3os is-region? )
    assert( 4os is-square-list? )

    \ Calc the square list's region.
    #3 pick                         \ sqrs4 reg3 act-id2 dom-id1 | sqrs4
    square-list-region              \ sqrs4 reg3 act-id2 dom-id1 | s-reg' t | f
    invert abort" group-new: square-list-region failed?"

    \ Check if square list region is a subset of the given group region.
    dup #4 pick                     \ sqrs4 reg3 act-id2 dom-id1 | s-reg' s-reg' reg3
    region-superset?                \ sqrs4 reg3 act-id2 dom-id1 | s-reg' bool
    ifnot
        region-deallocate
        2drop 2drop
        false
        \ cr ." group-new: exit 1" cr
        exit
    then

    \ Calc square rules.
    #4 pick                         \ sqrs4 reg3 act-id2 dom-id1 | s-reg' sqrs4
    square-list-calc-rules          \ sqrs4 reg3 act-id2 dom-id1 | s-reg', ruls' t | f
    ifnot
        region-deallocate
        2drop 2drop
        false
        \ cr ." group-new: exit 2" cr
        exit
    then

    \ Allocate instance.
    group-struct-id group-mma       \ sqrs4 reg3 act-id2 dom-id1 | s-reg' ruls' id mma
    struct-allocate                 \ sqrs4 reg3 act-id2 dom-id1 | s-reg' ruls' grp

    \ Set fields.
    tuck _group-set-rules           \ sqrs4 reg3 act-id2 dom-id1 | s-reg' grp
    tuck _group-set-s-region        \ sqrs4 reg3 act-id2 dom-id1 | grp
    tuck _group-set-dom-inst-id     \ sqrs4 reg3 act-id2 grp
    tuck _group-set-act-inst-id     \ sqrs4 reg3 grp
    tuck _group-set-region          \ sqrs4 grp
    tuck _group-set-squares         \ grp

    dup _group-calc-set-pnc         \ grp

    true
;

: group-deallocate ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    dup struct-get-use-count    \ grp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate instance.
        dup group-get-region region-deallocate
        dup group-get-s-region region-deallocate
        dup group-get-squares square-list-deallocate
        dup group-get-rules rule-list-deallocate

        group-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .group ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    ." ("
    dup group-get-region .region
    space ." pnc: " dup group-get-pnc .bool

    space ." - "
    dup group-get-s-region .region
    space
    dup group-get-rules  .rule-list

    space
    group-get-squares   .square-list-states

    ." )"
;

: .group-region ( grp0 -- )
    \ Check arg.
    assert( tos is-group? )

    group-get-region        \ reg
    .region
;

\ Return true if a group's region equals its s-region.
: _group-region-eq-s-region? ( grp0 -- bool )
    \ Check arg.
    assert( tos is-group? )

    dup group-get-region        \ grp0 reg
    swap group-get-s-region     \ reg s-reg
    regions-eq?
;

\ Check a square in a group that has changed.
\ The change may invalidate the group.
: group-check-changed-square ( sqr1 grp0 -- )
    \ Check args.
    assert( tos is-group? )
    assert( nos is-square? )
    \ cr ." group-check-changed-square: start: " .stack-gbl cr

    \ Check square in group.
    over square-get-state           \ sqr1 grp0 sta
    over group-get-region           \ sqr1 grp0 sta reg0
    region-superset-of-state?       \ sqr1 grp0 bool
    ifnot
        2drop
        exit
    then

    \ Check that the square is already in the square list.
    2dup                            \ sqr1 grp0 sqr1 grp0
    group-get-squares               \ sqr1 grp0 sqr1 sqr-lst
    square-list-member?             \ sqr1 grp0 bool
    invert abort" group-check-changed-square: square not in group square list?"

    \ Check if squares are still compatible.
    2dup                            \ sqr1 grp0 sqr1 grp
    group-get-squares               \ sqr1 grp0 sqr1 sqr-lst
    square-list-square-compatible?  \ sqr1 grp0 bool
    if
        \ Check if square pn GT group pn.
        over square-get-pn          \ sqr1 grp0 s-pn
        over group-get-pn           \ sqr1 grp0 s-pn g-pn
        >
        if
            \ Update s-region and rules.
            dup                     \ sqr1 grp0 grp0
            _group-update-s-region  \ sqr1 grp0
            _group-update-rules     \ sqr1
            drop
        else
            \ Check if square is pnc.
            over square-get-pnc     \ sqr1 grp0 s-pnc
            if
                \ Check if group is pnc ...
                dup group-get-pnc
                if
                    2drop
                else
                    \ Check if group-region equals group-s-region.
                    dup _group-region-eq-s-region?   \ sqr1 grp0 bool
                    if
                        \ See if the square's change allows setting the group's pnc to true.
                        _group-calc-set-pnc \ sqr1
                        drop
                    else
                        2drop
                    then
                then
            else
                2drop
            then
        then
    else                            \ sqr1 grp0
        2drop
    then

    \ cr ." group-check-changed-square: exit 2: " .stack-gbl cr
;

\ Attempt to add a square to a group.
\ The addition may invalidate the group.
: group-add-new-square ( sqr1 grp0 -- )
    \ Check args.
    assert( tos is-group? )
    assert( nos is-square? )
    \ cr ." group-add-new-square: start: " over square-get-state .state cr \ .stack-gbl cr

    \ Check that square is new.
    over square-get-num-samples 1 <> abort" New square gt 1 samples?"

    \ Check that the square is a subset of the group's region.
    over square-get-state                   \ sqr1 grp0 sta
    over group-get-region                   \ sqr1 grp0 sta reg
    region-superset-of-state?               \ sqr1 grp0 bool
    invert abort" square is not subset of group region?"
    \ cr ." group-add-new-square: at 1: " .stack-gbl cr

    \ Check if the square is already in the square list.
    over square-get-state                   \ sqr1 grp0 sta
    over group-get-squares                  \ sqr1 grp0 sta sqr-lst
    square-list-find                        \ sqr1 grp0, sqr t | f
    if
        2drop
        drop
        exit
    then
    \ cr ." group-add-new-square: at 2: " .stack-gbl cr

    \ Check if the square will invalidate the group.

    \ Add the square to the square list.
    2dup                                    \ sqr1 grp0 sqr1 grp0
    group-get-squares                       \ sqr1 grp0 sqr1 sqr-lst
    list-push-struct                        \ sqr1 grp0
    \ cr ." group-add-new-square: at 4: " .stack-gbl cr

    \ Check if the new square is in the s-region.
    over square-get-state                   \ sqr1 grp0 sta
    over group-get-s-region                 \ sqr1 grp0 sta r-reg
    region-superset-of-state?               \ sqr1 grp0 bool

    ifnot
        dup _group-update-s-region          \ sqr1 grp0
        dup _group-update-rules             \ sqr1 grp0
        dup _group-calc-set-pnc             \ sqr1 grp0
    then

    2drop
;

\ Return true if a group's region is superset of a square's state.
: group-superset-square? ( sqr1 grp0 -- bool )
    \ Check args.
    assert( tos is-group? )
    assert( nos is-square? )

    swap square-get-state
    swap group-get-region
    region-superset-of-state?
;

\ Return true if a group region is equal to a given region.
: group-region-eq? ( reg1 grp0 -- bool )
    \ Check args.
    assert( tos is-group? )
    assert( nos is-region? )

    group-get-region    \ reg1 g-reg
    regions-eq?
;
