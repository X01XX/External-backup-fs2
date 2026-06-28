\ Implement a group struct and functions.

#43717 constant group-id
    #5 constant group-struct-number-cells

\ Struct fields
0                           constant group-header-disp      \ 16 bits, [0] struct id, [1] use count (16), [2] pnc (8 bits), valid (8 bits)
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

\ Check instance type.
: is-allocated-group? ( addr -- bool )
    dup group-mma mma-is-item   \ addr bool
    if
        struct-get-id
        group-id =              \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for group, unconventional, leaves stack unchanged.
: assert-tos-is-group ( tos -- tos )
    dup is-allocated-group?
    false? if
        s" TOS is not an allocated group"
       .abort-xt execute
    then
;

\ Check NOS for group, unconventional, leaves stack unchanged.
: assert-nos-is-group ( nos tos -- nos tos )
    over is-allocated-group?
    false? if
        s" NOS is not an allocated group"
       .abort-xt execute
    then
;

' assert-nos-is-group to assert-nos-is-group-xt

\ Start accessors.

\ Return the group region.
: group-get-region ( grp0 -- reg )
    \ Check arg.
    assert-tos-is-group

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
    assert-tos-is-group

    group-s-region-disp +   \ Add offset.
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
    assert-tos-is-group

    4c@
    0<>     \ Change 255 to -1
;

: _group-set-pnc ( bool sqr -- )
    4c!
;

\ Return group 8-bit valid value, as a bool.
: group-get-valid ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-group

    5c@
    0<>     \ Change 255 to -1
;

\ Set the valid flag to the given bool value.
: _group-set-valid ( bool sqr -- )
    5c!
;

: group-get-rules ( sqr0 -- rul-lst )
    \ Check arg.
    assert-tos-is-group

    group-rules-disp + @
;

: _group-set-rules ( rul-lst1 sqr0 -- )
    group-rules-disp +
    !struct
;

\ Return the group squares.
: group-get-squares ( grp0 -- reg )
    \ Check arg.
    assert-tos-is-group

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
    assert-tos-is-group

    group-get-rules
    list-get-length
;

\ End accessors.

\ Set the valid flag to true.
: _group-set-to-valid ( grp0 -- )
   true swap
   _group-set-valid
;

\ Set the valid flag to false.
: _group-set-to-invalid ( grp0 -- )
    cr ." Group: " dup group-get-region .region space ." invalidated" cr
    false swap
    _group-set-valid
;

\ Calc, and set, group s-region, based on group square list.
: _group-calc-set-s-region ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

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
    assert-tos-is-group

    dup group-get-s-region swap \ reg-old reg1 grp0
    _group-calc-set-s-region    \ reg-old
    region-deallocate           \ Deallocate last, so struct field is never invalid.
;

\ Calc, and set, group rules, based on group square list.
\ Also set group-valid flag.
: _group-calc-set-rules ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup group-get-squares   \ grp0 sqr-lst
    square-list-calc-rules  \ grp0, ruls t | f
    if
        swap                \ ruls grp0
        dup _group-set-to-valid
        _group-set-rules
    else
        _group-set-to-invalid
    then
;

\ Replace current rules with new rules.
: _group-update-rules ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup group-get-rules swap    \ ruls-old grp0
    _group-calc-set-rules       \ ruls-old
    rule-list-deallocate        \ Deallocate last, so struct field is never invalid.
;

\ Calc, and set, group pnc, based on group square list.
: _group-calc-set-pnc ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    \ Check if group region EQ group s-region.
    dup group-get-region            \ grp0 reg
    over group-get-s-region         \ grp0 reg r-reg
    regions-eq?                     \ grp0 bool
    if
    else
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

\ Return a new group, given a region and square-list.
\ Return false if the given square list is empty,
\ or contains an incompatible pair,
\ or are not all within the given region.
: group-new    ( sqrs1 reg0 -- grp t | f )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-list

    \ Check for empty list.
    over list-is-empty?
    if
        2drop
        false
        \ cr ." group-new: exit 1" cr
        exit
    then

    \ Check squares are in region, save the square's region.
    over square-list-region         \ sqrs1 reg0, s-reg t | f
    invert abort" group-new: square-list-region failed?"
    2dup swap                       \ sqrs1 reg0 s-reg' s-reg reg0
    region-superset?                \ sqrs1 reg0 s-reg' bool
    if
    else
        region-deallocate
        2drop
        false
        exit
    then
    -rot                            \ s-reg' sqrs1 reg0

    \ Get square rules, and check all squares are compatible.
    over square-list-calc-rules     \ s-reg' sqrs1 reg0, ruls' t | f
    if
    else
        2drop
        region-deallocate
        false
        \ cr ." group-new: exit 2" cr
        exit
    then

    -rot                            \ s-reg' ruls' sqrs1 reg0

    \ Allocate instance.
    group-id group-mma              \ s-reg' ruls' sqrs1 reg0 id mma
    struct-allocate                 \ s-reg' ruls' sqrs1 reg0 grp

    \ Set group to valid.
    dup _group-set-to-valid         \ s-reg' ruls' sqrs1 reg0 grp

    \ Set region.
    tuck                            \ s-reg' ruls' sqrs1 grp reg0 grp
    _group-set-region               \ s-reg' ruls' sqrs1 grp

    \ Set squares.
    tuck                            \ s-reg' ruls' grp sqrs1 grp
    _group-set-squares              \ s-reg' ruls' grp

    \ Set rules
    tuck _group-set-rules           \ s-reg' grp

    \ Set s-region
    tuck _group-set-s-region        \ grp

    \ Set pnc
    dup _group-calc-set-pnc         \ grp
    true
;

: group-deallocate ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

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
    assert-tos-is-group

    ." Grp: "
    dup group-get-region .region
    space ." pnc: " dup group-get-pnc .bool

    dup group-get-valid             \ grp0 valid
    if
        space ." - "
        dup group-get-s-region .region
        space
        dup group-get-rules  .rule-list
    else
        space ." Invalid"
    then
    space
    group-get-squares   .square-list-states

;

\ Print a group region.
: .group-region ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    group-get-region .region
;

\ Return true if a group's region equals its s-region.
: _group-region-eq-s-region? ( grp0 -- bool )
    \ Check arg.
    assert-tos-is-group

    dup group-get-region        \ grp0 reg
    swap group-get-s-region     \ reg s-reg
    regions-eq?
;

\ Check a square in a group that has changed.
\ The change may invalidate the group.
: group-check-changed-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square
    \ cr ." group-check-changed-square: start: " .stack-gbl cr

    \ Check if square is valid.
    dup group-get-valid             \ sqr1 grp0 bool
    if
    else
        cr ." problem? group-check-changed-square: group is invalid." cr
        2drop
        \ cr ." group-check-changed-square: exit 1: " .stack-gbl cr
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
        _group-set-to-invalid       \ sqr1
        drop
    then

    \ cr ." group-check-changed-square: exit 2: " .stack-gbl cr
;

\ Attempt to add a square to a group.
\ The addition may invalidate the group.
: group-add-new-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square
    \ cr ." group-add-new-square: start: " .stack-gbl cr

    \ Check that square is new.
    over square-get-num-samples 1 <> abort" New square gt 1 samples?"

    \ Check that the square is a subset of the group's region.
    over square-get-state                   \ sqr1 grp0 sta
    over group-get-region                   \ sqr1 grp0 sta reg
    region-superset-of-state?               \ sqr1 grp0 bool
    invert abort" square is not subset of group region?"
    \ cr ." group-add-new-square: at 1: " .stack-gbl cr

    \ Check that the square is not already in the square list.
    over square-get-state                   \ sqr1 grp0 sta
    over group-get-squares                  \ sqr1 grp0 sta sqr-lst
    square-list-find                        \ sqr1 grp0, sqr t | f
    abort" square already in group list?"
    \ cr ." group-add-new-square: at 2: " .stack-gbl cr

    \ Check if the square will invalidate the group.

    2dup group-get-squares                  \ sqr1 grp0 sqr1 sqr-lst
    square-list-square-compatible?          \ sqr1 grp0 valid-bool
    \ cr ." group-add-new-square: at 3: " .stack-gbl cr

    \ Add the square to the square list.
    #2 pick #2 pick                         \ sqr1 grp0 valid-bool sqr1 grp0
    group-get-squares                       \ sqr1 grp0 valid-bool sqr1 sqr-lst
    list-push-struct                        \ sqr1 grp0 valid-bool
    \ cr ." group-add-new-square: at 4: " .stack-gbl cr

    \ Check if the new square is in the s-region.
    #2 pick square-get-state                \ sqr1 grp0 valid-bool sta
    #2 pick group-get-s-region              \ sqr1 grp0 valid-bool sta r-reg
    region-superset-of-state?               \ sqr1 grp0 valid-bool r-bool

    \ Process validity result.
    swap                                    \ sqr1 grp0 r-bool valid-bool
    if                                      \ sqr1 grp0 r-bool
        if                                  \ sqr1 grp
            2drop
        else
            \ Check if group pn is 1.
            dup group-get-pn 1 =
            if
                \ Update s-region and rules.
                dup                         \ sqr1 grp0 grp0
                _group-update-s-region      \ sqr1 grp0
                _group-update-rules         \ sqr1
                drop
            then
        then
    else                                    \ sqr1 grp0 r-bool
        drop                                \ sqr1 grp0
        \ Set the valid flag to false
        _group-set-to-invalid               \ sqr1
        drop
    then
;

\ Return true if a group's region is superset of a square's state.
: group-superset-square? ( sqr1 grp0 -- bool )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

    swap square-get-state
    swap group-get-region
    region-superset-of-state?
;

\ Return an incompatible square pair from a group, if any.
: group-get-incompatible-pair ( grp0 -- sqr-pr' t | f )
    \ Check arg.
    assert-tos-is-group

    group-get-squares                   \ sqr-lst
    square-list-find-incompatible-pair  \ sqr-pr t | f )
;
