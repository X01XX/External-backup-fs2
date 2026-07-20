\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-struct-id
    #4 constant corner-struct-number-cells

\ Struct fields
0                                       constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp              cell+   constant corner-anchor-square-disp      \ The anchor square.
corner-anchor-square-disp       cell+   constant corner-dissimilar-squares-disp \ Dissimilar, squares, from the square-pairs list.
corner-dissimilar-squares-disp  cell+   constant corner-possible-region-disp    \ Possible region the anchor is in, according to ~A + ~B calculation.

\ Needs: Meta, resolve needs for cornerns by some criteria. Dissimilar squares only in one region, ...
\        pnc anchor.
\        pnc dissimilar squares.


0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check if tos is an allocated corner.
: is-corner? ( addr -- bool )
    dup corner-mma mma-is-item? \ addr bool
    if
        struct-get-id
        corner-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the anchor-square field from a corner instance.
: corner-get-anchor-square ( crn0 -- sqr )
    \ Check arg.
    assert( tos is-corner? )

    corner-anchor-square-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the anchor-square field from a corner instance, use only in this file.
: _corner-set-anchor-square ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    corner-anchor-square-disp +     \ Add offset.
    !struct                         \ Set the field.
;

\ Return the dissimilar-squares list field from a corner instance.
: corner-get-dissimilar-squares ( crn0 -- sta-lst )
    \ Check arg.
    assert( tos is-corner? )

    corner-dissimilar-squares-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the dissimilar-squares list field from a corner instance, use only in this file.
: _corner-set-dissimilar-squares ( sta-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square-list? )

    corner-dissimilar-squares-disp +    \ Add offset.
    !struct                             \ Set the field.
;

\ Return the possible-region field from a corner instance.
: corner-get-possible-region ( crn0 -- reg )
    \ Check arg.
    assert( tos is-corner? )

    corner-possible-region-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the possible-region field from a corner instance, use only in this file.
: _corner-set-possible-region ( reg1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region? )

    corner-possible-region-disp +       \ Add offset.
    !struct                             \ Set the field.
;

\ End accessors.

\ Return the anchor-square field state from a corner instance.
: corner-get-anchor-state ( crn0 -- sta )
    \ Check arg.
    assert( tos is-corner? )

    corner-get-anchor-square    \ sqr
    square-get-state
;

\ Return a corner's number bits.
: corner-get-num-bits ( crn0 -- nb )
    \ Check args.
    assert( tos is-corner? )

    corner-get-anchor-square        \ sta
    square-get-num-bits
;

\ Create a corner, given an anchor square.
: corner-new ( reg2 sqr-lst1 anc-sqr0 -- crn )
    \ Check args.
    assert( tos is-square? )

    \ Allocate space.
    corner-struct-id corner-mma         \ reg2 sqr-lst1 anc-sqr0 id mma
    struct-allocate                     \ reg2 sqr-lst1 anc-sqr0 crn

    \ Store anchor square.
    tuck _corner-set-anchor-square      \ reg2 sqr-lst1 crn

    \ Store dissimilar square list.
    tuck _corner-set-dissimilar-squares \ reg2 crn

    \ Store region list.
    tuck _corner-set-possible-region    \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    ." (anchor: "

    dup corner-get-anchor-square        \ crn0 sta
    .square-state                       \ crn0

    ."  dissimilar squares: "
    dup corner-get-dissimilar-squares   \ crn0 ext-sta-lst
    .square-list-states                 \ crn0

    space ." possible region: "
    dup corner-get-possible-region      \ crn0 reg
    .region                             \ crn0

    ." )"
                                        \ crn0
    drop
;

\ Return true if a square is used by a corner.
: corner-uses-square? ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    \ Check sqr1 eq anchor.
    dup corner-get-anchor-square    \ sqr1 crn0 anc
    #2 pick =                       \ sqr1 crn0 bool
    if
        2drop
        true
        cr ." corner-uses-square?: " dup .bool cr
        exit
    then

    \ Check square-pairs.
    swap square-get-state swap      \ sta1 crn0
    corner-get-dissimilar-squares   \ sta1 pr-lst
    square-list-find                \ sqr t | f
    if
        drop
        true
    else
        false
    then
    \ cr ." corner-uses-square?: " dup .bool cr
;

\ Return true if a square is not currently used in a corner,
\ and is incompatible and adjacent to the anchor square.
: corner-can-square-be-added? ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )
    assert( over square-get-num-bits over corner-get-num-bits = )

    \ Check if square is already used as an anchor, or in the dissimilar square list.
    2dup corner-uses-square?                \ sqr1 crn0 bool
    if
        2drop
        false
        cr ." corner-can-square-be-added?: exit 1: " dup .bool cr
        exit
    then

    \ Check square is adjacent to the anchor square.
    2dup corner-get-anchor-square           \ sqr1 crn0 sqr1 asc-sqr
    squares-adjacent?                       \ sqr1 crn0 bool
    ifnot
        2drop
        false
        cr ." corner-can-square-be-added?: exit 2: " dup .bool cr
        exit
    then

    \ Check if square is incompatible to the anchor square.
    corner-get-anchor-square                \ sqr1 asc-sqr
    squares-compare                         \ char
    [char] I =

    cr ." corner-can-square-be-added?: end: " dup .bool cr
;

: corner-add-square ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )
    assert( over square-get-num-bits over corner-get-num-bits = )

    2dup corner-can-square-be-added?    \ sqr1 crn0 bool
    ifnot 2drop false exit then

    corner-get-dissimilar-squares       \ sqr1 sqr-lst
    list-push-struct
    true
;

\ Update corner possible region with new regions.
: corner-update-possible-region ( reg1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region? )

    dup corner-get-possible-region  \ reg1 crn0 crn-reg'
    -rot                            \ crn-reg' reg1 crn0
    _corner-set-possible-region     \ crn-reg'
    region-deallocate
;

\ Add a close, dissimilar square, to the corner.
\ Close meaning: No square is between the anchor square and the square to be added.
\ The square may be pnc, or need more samples.
\ The square may, or may not, be adjacent to the anchor square, although adjacent is preferable.
: corner-add-dissimilar-square ( sqr1 crn0 -- )
    cr ." corner-add-dissimilar-square: start: todo"
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )


    2drop
;

\ Validate a corner.
\ Recalc if needed.
: corner-is-valid? ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    abort" TODO"
;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup corner-get-anchor-square square-deallocate
        dup corner-get-dissimilar-squares square-list-deallocate
        dup corner-get-possible-region region-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check the anchor, after a change.
: corner-check-anchor ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    true abort" TODO"
;

\ Check a dissimilar square, after a change.
: corner-check-dissimilar-square ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    true abort" TODO"
;

\ Check if a new square can be added.
: corner-check-new-square ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    cr ." TODO" cr
;

\ Return true if a square is the corner anchor.
: corner-square-is-anchor? ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    dup corner-get-anchor-square    \ sqr1 anc
    =
;



