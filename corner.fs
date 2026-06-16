\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-id
    #5 constant corner-struct-number-cells

\ Struct fields
0                                       constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp              cell+   constant corner-anchor-square-disp      \ The anchor state.
corner-anchor-square-disp       cell+   constant corner-dissimilar-squares-disp \ Dissimilar, closest, square list.
corner-dissimilar-squares-disp  cell+   constant corner-similar-squares-disp    \ Similar, closest, square list.
corner-similar-squares-disp     cell+   constant corner-regions-disp            \ Regions the anchor is in.

0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check instance type.
: is-allocated-corner? ( addr -- bool )
    dup corner-mma mma-is-item  \ addr bool
    if
        struct-get-id
        corner-id =             \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for corner, unconventional, leaves stack unchanged.
: assert-tos-is-corner ( tos -- tos )
    dup is-allocated-corner?
    false? if
        s" TOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Check NOS for corner, unconventional, leaves stack unchanged.
: assert-nos-is-corner ( nos tos -- nos tos )
    over is-allocated-corner?
    false? if
        s" NOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the anchor-square field from a corner instance.
: corner-get-anchor-square ( crn0 -- sqr )
    \ Check arg.
    assert-tos-is-corner

    corner-anchor-square-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Return the anchor-square field state from a corner instance.
: corner-get-anchor-state ( crn0 -- sta )
    \ Check arg.
    assert-tos-is-corner

    corner-get-anchor-square    \ sqr
    square-get-state
;

\ Return the dissimilar-squares list field from a corner instance.
: corner-get-dissimilar-squares ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-dissimilar-squares-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Return the similar-squares list field from a corner instance.
: corner-get-similar-squares ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-similar-squares-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Return the regions list field from a corner instance.
: corner-get-regions ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-regions-disp +           \ Add offset.
    @                               \ Fetch the field.
;

\ Set the anchor-square field from a corner instance, use only in this file.
: _corner-set-anchor-square ( sta1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    corner-anchor-square-disp +     \ Add offset.
    !struct                         \ Set the field.
;

\ Set the dissimilar-squares list field from a corner instance, use only in this file.
: _corner-set-dissimilar-squares ( sta-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square-list

    corner-dissimilar-squares-disp +    \ Add offset.
    !struct                             \ Set the field.
;

\ Set the similar-squares list field from a corner instance, use only in this file.
: _corner-set-similar-squares ( sta-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square-list

    corner-similar-squares-disp +       \ Add offset.
    !struct                             \ Set the field.
;

\ Set the regions list field from a corner instance, use only in this file.
: _corner-set-regions ( reg-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-region-list

    corner-regions-disp +               \ Add offset.
    !struct                             \ Set the field.
;

\ End accessors.

\ Return a corner's number bits.
: corner-get-number-bits ( crn0 -- nb )
    \ Check args.
    assert-tos-is-corner

    corner-get-anchor-square        \ sta
    square-get-num-bits
;

\ Create a corner, given an anchor square.
: corner-new ( sqr0 -- crn )
    \ Check args.
    assert-tos-is-square

    \ Allocate space.
    corner-id corner-mma                \ sta1 id mma
    struct-allocate                     \ sta1 crn

    \ Store anchor square.
    tuck _corner-set-anchor-square       \ crn

    \ Store dissimilar square list.
    list-new                            \ crn lst
    over _corner-set-dissimilar-squares \ crn

    \ Store similar square list.
    list-new                            \ crn lst
    over _corner-set-similar-squares    \ crn

    \ Store regions list.
    dup corner-get-number-bits          \ crn nb
    region-list-max-x                   \ crn reg-lst
    over _corner-set-regions            \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    ." (anchor: "

    dup corner-get-anchor-square        \ crn0 sta
    .square-state                       \ crn0

    ."  dissimilar squares: "
    dup corner-get-dissimilar-squares   \ crn0 ext-sta-lst
    .square-list-states                 \ crn0

    ."  similar squares: "
    dup corner-get-similar-squares      \ crn0 ext-sta-lst
    .square-list-states                 \ crn0

    ."  regions: "
    corner-get-regions                  \ ext-sta-lst
    .region-list                        \

    ." )"
;

\ Return true if a square is used by a corner.
: corner-uses-square? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    \ Check sqr1 eq anchor.
    dup corner-get-anchor-square    \ sqr1 crn0 anc
    #2 pick =                       \ sqr1 crn0 bool
    if
        2drop
        true
        exit
    then

    \ Check sqr1 eq any similar square.
    [ ' = ] literal                 \ sqr1 crn0 xt
    #2 pick #2 pick                 \ sqr1 crn0 xt sqr1 crn0
    corner-get-similar-squares      \ sqr1 crn0 sqr1 sim-lst
    list-member?                    \ sqr1 crn0
    if
        2drop
        true
        exit
    then

    \ Check sqr1 eq any dissimilar square.
    [ ' = ] literal                 \ sqr1 crn0 xt
    #2 pick #2 pick                 \ sqr1 crn0 xt sqr1 crn0
    corner-get-dissimilar-squares   \ sqr1 crn0 sqr1 dis-lst
    list-member?                    \ sqr1 crn0
    if
        2drop
        true
        exit
    then
                                    \ sqr1 crn0
    2drop
    false
;

\ Return true if any similar, or dissimilar, square is between the anchor and a given square.
: corner-square-can-be-added? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    \ Check if square is already used.
    2dup corner-uses-square?                \ sqr1 crn0 bool
    if
        2dup
        false
        exit
    then

    \ Check if square is More samples needed.
    over                                    \ sqr1 crn0 sqr1
    over corner-get-anchor-square           \ sqr1 crn0 sqr1 anc
    squares-compare                         \ sqr1 crn0 char

    case
        [char] C of
            2dup                            \ sqr1 crn0 sqr1 crn0
            dup corner-get-anchor-square    \ sqr1 crn0 sqr1 crn0 anc
            swap                            \ sqr1 crn0 sqr1 anc crn0
            corner-get-similar-squares      \ sqr1 crn0 sqr1 anc sqr-lst
            square-list-any-between?        \ sqr1 crn0 sqr1 bool
            invert
        endof
        [char] I of
            2dup                            \ sqr1 crn0 sqr1 crn0
            dup corner-get-anchor-square    \ sqr1 crn0 sqr1 crn0 anc
            swap                            \ sqr1 crn0 sqr1 anc crn0
            corner-get-dissimilar-squares   \ sqr1 crn0 sqr1 anc sqr-lst
            square-list-any-between?        \ sqr1 crn0 sqr1 bool
            invert
        endof
        [char] M of
            2drop
            false
        endof
    endcase
;

\ Recalculate a corner, when a close dissimilar square becomes similar.
\ A closer dissimilar square requires only an extra intersection with the existing regions.
: corner-recalc ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

;

\ Validate a corner.
\ Recalc if needed.
: corner-validate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup corner-get-anchor-square square-deallocate
        dup corner-get-dissimilar-squares square-list-deallocate
        dup corner-get-similar-squares square-list-deallocate
        dup corner-get-regions region-list-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

