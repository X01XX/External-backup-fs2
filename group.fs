\ Implement a group struct and functions.                                                                                                         

#43717 constant group-id
    #5 constant group-struct-number-cells

\ Struct fields
0                           constant group-header-disp      \ 16 bits, [0] struct id, [1] use count (16), [1] pnc (8 bits)
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

: _group-set-pnc ( pnc sqr -- )
    4c!
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

: _group-update-r-region ( reg1 grp0 -- )
    \ Check arg.
    assert-tos-is-group
    assert-nos-is-region

    dup group-get-r-region -rot \ reg-old reg1 grp0
    _group-set-r-region         \ reg-old
    region-deallocate           \ Deallocate last, so struct field is never invalid.
;

: _group-update-rules ( rul-lst1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-rule-list

    dup group-get-rules -rot    \ ruls-old ruls1 grp0
    _group-set-rules            \ ruls-old
    rule-list-deallocate        \ Deallocate last, so struct field is never invalid.
;

\ End accessors.

\ Return a new group, given a region and square-list.
: group-new    ( sqrs1 reg0 -- grp )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-list

    over list-is-empty?
    if
        ." empty square list?"
        abort
    then

   \ Allocate space.
    group-id group-mma          \ sqrs1 reg0 id mma
    struct-allocate             \ sqrs1 reg0 grp

    \ Set region.
    tuck                        \ sqrs1 grp  r grp
    _group-set-region           \ sqrs1 grp

    \ Set r-region
    over square-list-region     \ sqrs1 grp , reg t | f
    0= abort" region not found?"
    over _group-set-r-region    \ sqrs1 grp

    \ Set rules
    over square-list-get-rules  \ sqrs1 grp , ruls t | f
    0=
    if  dup group-get-region cr ." Group: " .region
        space ." Group squares cannot form rules."
        space over .square-list cr
        abort
    then
                                \ sqrs1 grp  rules
    over _group-set-rules       \ sqrs1 grp

    \ Set pnc
    \ over square-list-pnc      \ sqrs1 grp  pnc
    false
    over _group-set-pnc         \ sqrs1 grp
                                                                                                                                                  
    \ Set squares
    tuck                        \ grp  sqrs1 grp
    _group-set-squares          \ grp
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
        dup group-get-r-region region-deallocate
        dup group-get-rules rule-list-deallocate
        dup group-get-squares square-list-deallocate
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

: .group ( grp -- )
    \ Check arg.
    assert-tos-is-group

    ." Grp: "
    dup group-get-region .region
    space ." - "
    dup group-get-r-region .region
    space
    dup group-get-rules  .rule-list
    space
    group-get-squares   .square-list-states
;

\ Print a group region.
: .group-region ( grp -- )
    \ Check arg.
    assert-tos-is-group

    group-get-region .region
;

\ Check a square in a group that has changed.
\ Return true if the group is Ok, false otherwise.
: group-check-square ( sqr1 grp0 -- sqr-pr t | f )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

    tuck                        \ grp0 sqr1 grp0
    group-get-squares           \ grp0 sqr1 sqr-lst
    square-list-check-square    \ grp0, sqr-pr t | f

    if
        \ Group is invalidated.
        nip
        true
    else
        \ Group is Ok.
        \ group-check-pn-pnc.
        cr ." TODO" cr
        drop
    then
;

\ Attempt to add a square to a group.
\ If the addition would invalidate the group,
\ return true and an incompatible square pair.
: group-add-square ( sqr1 grp0 -- sqr-pr t | f )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square
    
    \ Check that the square is a subset of the group's region.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-region       \ sqr1 grp0 sta reg
    region-superset-of-state?   \ sqr1 grp0 bool
    invert abort" square is not subset of group region?"

    \ Check that the square is not already in the square list.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-squares      \ sqr1 grp0 sta sqr-lst
    square-list-find            \ sqr1 grp0, sqr t | f
    abort" square already in group list?"

    \ Check if the square can be added, without invalidating the group.
    2dup group-check-square     \ sqr1 grp0, sqr-pr t | f
    if
        \ The group is now invalid.
        2drop
        false
    else
        \ The group remains valid.

        \ Add the square to the square list.
        over                        \ sqr1 grp0 sqr1
        over group-get-squares      \ sqr1 grp0 sqr1 sqr-lst
        list-push-struct            \ sqr1 grp0

        \ Check if the new square is in the r-region.
        over square-get-state       \ sqr1 grp0 sta
        over group-get-r-region     \ sqr1 grp0 sta r-reg
        if
            \ no op
        else
            \ Update r-region and rules.
        then

        \ group-check-pn-pnc.
    then
;

