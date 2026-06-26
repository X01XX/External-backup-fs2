\ Implement a group struct and functions.                                                                                                         

#43717 constant group-id
    #5 constant group-struct-number-cells

\ Struct fields
0                           constant group-header-disp      \ 16 bits, [0] struct id, [1] use count (16), [2] pnc (8 bits), valid (8 bits)
group-header-disp   cell+   constant group-region-disp      \ The group region.
group-region-disp   cell+   constant group-r-region-disp    \ A Region covered by the group rules, often a proper subset of the group-region.
group-r-region-disp cell+   constant group-squares-disp     \ A square-list.
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
: group-get-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-region-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set the region of a group instance, use only in this file.
: _group-set-region ( reg1 addr -- )
    group-region-disp + \ Add offset.
    !struct             \ Set the field.
;

\ Return the group squares region.
: group-get-r-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-r-region-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square region of a group instance, use only in this file.
: _group-set-r-region ( reg1 addr -- )
    group-r-region-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return group 8-bit pnc value, as a bool.
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

\ Set the valid flag to false.
: _group-set-to-valid ( grp0 -- )
   true swap
   _group-set-valid
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

\ Set the valid flag to false.
: _group-set-to-invalid ( grp0 -- )
    dup group-get-valid     \ grp0 bool
    if
        dup group-get-rules \ grp0 ruls
        rule-list-deallocate
    then
    false swap
    _group-set-valid
;

\ Return the group squares.
: group-get-squares ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-squares-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the squares field of a group instance, use only in this file.
: _group-set-squares ( sqr-lst addr -- )
    group-squares-disp +    \ Add offset.
    !struct                 \ Set the field.
;

: group-get-pn ( grp0 -- pn )
    \ Check arg.
    assert-tos-is-group

    group-get-rules
    list-get-length
;

\ End accessors.

\ Calc, and set, group r-region, based on group square list.
: _group-calc-set-r-region ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup group-get-squares       \ grp0 sqr-lst
    square-list-region          \ grp0, r-reg t | f
    0= abort" _group-calc-set-r-region: group r-region not found?"

    dup                         \ grp0 r-reg r-reg
    #2 pick group-get-region    \ grp0 r-reg r-reg g-reg
    region-superset?            \ grp0 r-reg bool
    if
        swap _group-set-r-region
    else
        cr ." _group-calc-set-r-region: r-region not subset group region?"
    then
;

: _group-update-r-region ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup group-get-r-region swap \ reg-old reg1 grp0
    _group-calc-set-r-region    \ reg-old
    region-deallocate           \ Deallocate last, so struct field is never invalid.
;

\ Calc, and set, group rules, based on group square list.
\ Also set group-valid flag.
: _group-calc-set-rules ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup group-get-squares   \ grp0 sqr-lst
    square-list-get-rules   \ grp0, ruls t | f
    if
        swap                \ ruls grp0
        dup _group-set-to-valid
        _group-set-rules
    else
        _group-set-to-invalid
    then
;

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

    \ Check if group region EQ group r-region.
    dup group-get-region            \ grp0 reg
    over group-get-r-region         \ grp0 reg r-reg
    regions-eq?                     \ grp0 bool
    if
    else
        false
        swap                        \ false grp0
        _group-set-pnc
        exit
    then

    \ Look for a pair of pnc squares.
    dup group-get-squares           \ grp0 sqr-lst
    square-list-pnc-squares         \ grp0, pnc-lst' t | f
    if
        dup square-list-region      \ grp0, pnc-lst' pnc-reg'
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
        false
        swap                        \ false grp0
        _group-set-pnc
    then
;

\ Return a new group, given a region and a non-empty square-list.
\ Return an incompatible square pair, if any.
\ Allow creation of an invalid group, for testing.
: group-new    ( sqrs1 reg0 -- grp )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-list

   \ Allocate instance.
    group-id group-mma              \ sqrs1 reg0 id mma
    struct-allocate                 \ sqrs1 reg0 grp

    \ Set group to valid.
    dup _group-set-to-valid         \ sqrs1 reg0 grp

    \ Set region.
    tuck                            \ sqrs1 grp reg0 grp
    _group-set-region               \ sqrs1 grp

    \ Set squares.
    tuck                            \ grp sqrs1 grp
    _group-set-squares              \ grp

    \ Check for empty list.
    dup group-get-squares           \ grp sqr-lst
    list-is-empty?
    if
        cr ." problem? group-new: square list empty"
        dup _group-set-to-invalid
        exit
    then

    \ Set r-region
    dup _group-calc-set-r-region    \ grp

    \ Check if squares are compatible.
    dup group-get-squares               \ grp sqr-lst
    square-list-find-incompatible-pair  \ grp, sqp-pr t | f
    if
        cr ." problem? Group-new: incompatible squares" dup .square-list
        square-list-deallocate
        dup _group-set-to-invalid
        exit
    then

    \ Set rules
    dup _group-calc-set-rules       \ grp

    \ Set pnc
    dup _group-calc-set-pnc         \ grp
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

        dup group-get-squares
        list-get-length 0>
        if
            dup group-get-r-region region-deallocate
        then

        dup group-get-squares square-list-deallocate
        dup group-get-valid
        if
            dup group-get-rules
            cr dup .rule-list cr
            rule-list-deallocate
        then

        group-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a group region is equal to a given region.
: group-region-eq ( reg1 grp0 -- flag )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region

    group-get-region
    regions-eq?
;

: .group ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    ." Grp: "
    dup group-get-region .region

    dup group-get-squares
    list-get-length 0>
    if
        space
        ." - "
        dup group-get-r-region .region
    then
    
    dup group-get-valid             \ grp0 valid
    if
        space
        dup group-get-rules  .rule-list
        space
        group-get-squares   .square-list-states
    else
        space
        ." Invalid, states: "
        group-get-squares   .square-list
    then
;

\ Print a group region.
: .group-region ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    group-get-region .region
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
    [ ' = ] literal                 \ sqr1 grp0 xt
    #2 pick #2 pick                 \ sqr1 grp0 xt sqr1 grp0
    group-get-squares               \ sqr1 grp0 xt sqr1 sqr-lst
    list-member?                    \ sqr1 grp0, bool
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
            
            \ Update r-region and rules.
            dup                     \ sqr1 grp0 grp0
            _group-update-r-region  \ sqr1 grp0
            _group-update-rules     \ sqr1
            drop
        else
            \ Check if square pn = group pn.
            over square-get-pn      \ sqr1 grp0 s-pn
            over group-get-pn       \ sqr1 grp0 s-pn g-pn
            =
            if
                \ Check if square is in r-region.
                swap square-get-state       \ grp0 sta
                over group-get-r-region     \ grp0 sta r-reg
                region-superset-of-state?   \ grp0 bool
                if
                    drop
                else
                    \ Update r-region and rules.
                    dup                     \ grp0 grp0
                    _group-update-r-region  \ grp0
                    _group-update-rules     \
                then
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

    \ Check that square.
    over square-get-num-samples 1 <> abort" New square gt 1 samples?"

    \ Check that the square is a subset of the group's region.
    over square-get-state           \ sqr1 grp0 sta
    over group-get-region           \ sqr1 grp0 sta reg
    region-superset-of-state?       \ sqr1 grp0 bool
    invert abort" square is not subset of group region?"

    \ Check that the square is not already in the square list.
    over square-get-state           \ sqr1 grp0 sta
    over group-get-squares          \ sqr1 grp0 sta sqr-lst
    square-list-find                \ sqr1 grp0, sqr t | f
    abort" square already in group list?"

    \ Check if the square will invalidate the group.
    
    2dup group-get-squares          \ sqr1 grp0 sqr1 sqr-lst
    square-list-square-compatible?  \ sqr1 grp0 bool

    \ Add the square to the square list.
    #2 pick #2 pick                 \ sqr1 grp0 bool sqr1 grp0
    group-get-squares               \ sqr1 grp0 bool sqr1 sqr-lst
    list-push-struct                \ sqr1 grp0 bool

    \ Process validity result.
    if
        \ Set the valid flag to false.
        dup _group-set-to-invalid            \ sqr1
        drop
    else
        \ Check if the new square is in the r-region.
        swap square-get-state       \ grp0 sta
        over group-get-r-region     \ grp0 sta r-reg
        region-superset-of-state?   \ grp0 bool
        if
            \ no op.
            drop
        else
            \ Check if group pn is 1.
            dup group-get-pn 1 =
            if
                \ Update r-region and rules.
                dup                     \ grp0 grp0
                _group-update-r-region  \ grp
                _group-update-rules
            then
        then
    then
;
