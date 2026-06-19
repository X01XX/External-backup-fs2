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
: corner-get-num-bits ( crn0 -- nb )
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
    dup corner-get-num-bits             \ crn nb
    region-list-max-x                   \ crn reg-lst
    over _corner-set-regions            \ crn
;

\ Return a list of regions the anchor is in.
: corner-anchor-regions ( crn0 -- reg-lst )
    \ Check arg.
    assert-tos-is-corner

    dup corner-get-anchor-square    \ crn0 anc-sqr
    square-get-state                \ crn0 anc-sta
    swap corner-get-regions         \ anc-sta reg-lst
    region-list-state-in            \ ret-lst
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
    dup corner-get-regions              \ crn0 ext-sta-lst
    .region-list                        \ crn0

    space ." anchor regions: "
    dup corner-anchor-regions           \ crn0 reg-lst'
    dup .region-list                    \ crn0 reg-lst'
    region-list-deallocate              \ crn0

    ." )"
                                        \ crn0
    drop
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
: corner-can-square-be-added? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    \ Check number bits.
    over square-get-num-bits
    over corner-get-num-bits
    <> abort" number bit difference?"

    \ Check if square is already used.
    2dup corner-uses-square?                \ sqr1 crn0 bool
    if
        2dup
        false
        \ cr ." exit 1" cr
        exit
    then

    \ Check if square is More samples needed.
    over                                    \ sqr1 crn0 sqr1
    over corner-get-anchor-square           \ sqr1 crn0 sqr1 anc
    squares-compare                         \ sqr1 crn0 char

    case
        [char] C of
            dup corner-get-anchor-square    \ sqr1 crn0 anc
            swap                            \ sqr1 anc crn0
            corner-get-similar-squares      \ sqr1 anc sqr-lst
            square-list-any-between?        \ bool
            invert
            \ cr ." exit 2" cr
        endof
        [char] I of
            dup corner-get-anchor-square    \ sqr1 crn0 anc
            swap                            \ sqr1 anc crn0
            corner-get-dissimilar-squares   \ sqr1 anc sqr-lst
            square-list-any-between?        \ bool
            invert
            \ cr ." exit 3" .s cr
        endof
        [char] M of
            2drop
            false
            \ cr ." exit 4" cr
        endof
        true abort" Invalid comparison result"
    endcase
;

\ Given a list of squares in the corner similar square list,
\ remove them from the list.
: corner-remove-similar-squares ( sqr-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square-list

    \ Prep for loop.
    corner-get-similar-squares      \ sqr-lst1 sim-lst
    swap                            \ sim-lst sqr-lst
    list-get-links                  \ sim-lst sqr-lnk

    begin
        ?dup
    while
        \ Set xt.
        [ ' = ] literal             \ sim-lst sqr-lnk xt
        over link-get-data          \ sim-lst sqr-lnk xt sqrx
        #3 pick                     \ sim-lst sqr-lnk xt sqrx sim-lst
        list-remove                 \ sim-lst sqr-lnk, sqrx t | f
        if
            square-deallocate       \ dis-lst sqr-lnk
        else
            true abort" item not found?"
        then

        link-get-next
    repeat
                                    \ sim-lst
    drop
;

\ Given a list of squares in the corner similar square list,
\ remove them from the list.
: corner-remove-dissimilar-squares ( sqr-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square-list

    \ Prep for loop.
    corner-get-dissimilar-squares   \ sqr-lst1 dis-lst
    swap                            \ dis-lst sqr-lst
    list-get-links                  \ dis-lst sqr-lnk

    begin
        ?dup
    while
        \ Set xt.
        [ ' = ] literal             \ dis-lst sqr-lnk xt
        over link-get-data          \ dis-lst sqr-lnk xt sqrx
        #3 pick                     \ dis-lst sqr-lnk xt sqrx dis-lst
        list-remove                 \ dis-lst sqr-lnk, sqrx t | f
        if
            square-deallocate       \ dis-lst sqr-lnk
        else
            true abort" item not found?"
        then

        link-get-next
    repeat
                                    \ dis-lst
    drop
;

\ Update corner regions with new regions.
: corner-update-regions ( reg-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-region-list

    dup corner-get-regions  \ reg-lst1 crn0 crn-regs'
    -rot                    \ crn-regs' reg-lst1 crn0
    _corner-set-regions     \ crn-lst'
    region-list-deallocate
;

\ Adjust regions for a new dissimilar square.
: corner-adjust-regions ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    swap square-get-state               \ crn0 sta1
    over corner-get-anchor-square       \ crn0 sta1 anc
    square-get-state                    \ crn0 sta1 sta-anc
    state-~a+~b                         \ crn0 reg-lst'
    dup                                 \ crn0 reg-lst' reg-lst'
    #2 pick corner-get-regions          \ crn0 reg-lst' reg-lst' crn-regs
    region-list-intersections-nosubs    \ crn0 reg-lst' new-regs

    \ Clean up.
    swap region-list-deallocate         \ crn0 new-regs

    \ Update regions.
    swap corner-update-regions          \
;

\ Add a square to a corner, if possible.
\ Return true if added.
: corner-add-square ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square
    \ cr ." corner-add-square: start: " .stack-gbl cr

    2dup corner-can-square-be-added?        \ sqr1 crn0 bool
    if
    else
        \ Return.
        2drop
        false
        \ cr ." corner-add-square: exit 1" .stack-gbl cr
        exit
    then

    \ Delete squares that the new square will be between, in the similar square list.
    dup corner-get-anchor-square            \ sqr1 crn0 anc
    #2 pick                                 \ sqr1 crn0 anc sqr1
    #2 pick corner-get-similar-squares      \ sqr1 crn0 anc sqr1 sim-lst
    square-list-between-any                 \ sqr1 crn0 btw-lst'
    2dup swap                               \ sqr1 crn0 btw-lst' btw-lst' crn0
    corner-remove-similar-squares           \ sqr1 crn0 btw-lst'
    square-list-deallocate                  \ sqr1 crn0

    \ Delete squares that the new square will be between, in the dissimilar square list.
    dup corner-get-anchor-square            \ sqr1 crn0 anc
    #2 pick                                 \ sqr1 crn0 anc sqr1
    #2 pick corner-get-dissimilar-squares   \ sqr1 crn0 anc sqr1 dis-lst
    square-list-between-any                 \ sqr1 crn0 btw-lst'
    2dup swap                               \ sqr1 crn0 btw-lst' btw-lst' crn0
    corner-remove-dissimilar-squares        \ sqr1 crn0 btw-lst'
    square-list-deallocate                  \ sqr1 crn0

    \ Determine which square-list to add it to.
    dup corner-get-anchor-square            \ sqr1 crn0 anc
    #2 pick                                 \ sqr1 crn0 anc sqr1
    squares-compare                         \ sqr1 crn0 char

    \ Add it.
    case
        [char] I of
            2dup                            \ sqr1 cnr0 sqr1 crn0
            corner-adjust-regions           \ sqr1 crn0
            corner-get-dissimilar-squares   \ sqr1 dis-lst
            list-push-struct
        endof
        [char] C of
            corner-get-similar-squares      \ sqr1 dis-lst
            list-push-struct
        endof
        abort" Invalid comparison result"
    endcase

    \ Return
    true
    \ cr ." corner-add-square: exit 2" .stack-gbl cr
;

\ Recalculate a corner, when a close dissimilar square becomes similar.
\ A closer dissimilar square requires only an extra intersection with the existing regions.
: corner-recalc ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

;

\ Validate a corner.
\ Recalc if needed.
: corner-is-validate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    abort" TODO"
;

\ Return a character, T - True, F - False, M - More samples needed.
: corner-is-valid? ( crn0 -- char )
    \ Check arg.
    assert-tos-is-corner

    abort" TODO"
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

\ Check the anchor, after a change.
: corner-check-anchor ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    true abort" TODO"
;

\ Check a similar square, after a change.
: corner-check-similar-square ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    true abort" TODO"
;

\ Check a dissimilar square, after a change.
: corner-check-dissimilar-square ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    true abort" TODO"
;

\ Check if a new square can be added.
: corner-check-new-square ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    \ Check if a square can be added.
    over                                    \ sqr1 crn0 sqr1
    over corner-get-anchor-square           \ sqr1 crn0 sqr1 anc
    squares-compare                         \ sqr1 crn0 char

    dup [char] M =                          \ sqr1 crn0 char bool
    if
        2drop drop
        exit
    then

    \ Save comparison char.
    -rot                                    \ char sqr1 crn0

    \ Delete squares that the new square will be between, in the similar square list.
    dup corner-get-anchor-square            \ char sqr1 crn0 anc
    #2 pick                                 \ char sqr1 crn0 anc sqr1
    #2 pick corner-get-similar-squares      \ char sqr1 crn0 anc sqr1 sim-lst
    square-list-between-any                 \ char sqr1 crn0 btw-lst'
    2dup swap                               \ char sqr1 crn0 btw-lst' btw-lst' crn0
    corner-remove-similar-squares           \ char sqr1 crn0 btw-lst'
    square-list-deallocate                  \ char sqr1 crn0

    \ Delete squares that the new square will be between, in the dissimilar square list.
    dup corner-get-anchor-square            \ char sqr1 crn0 anc
    #2 pick                                 \ char sqr1 crn0 anc sqr1
    #2 pick corner-get-dissimilar-squares   \ char sqr1 crn0 anc sqr1 dis-lst
    square-list-between-any                 \ char sqr1 crn0 btw-lst'
    2dup swap                               \ char sqr1 crn0 btw-lst' btw-lst' crn0
    corner-remove-dissimilar-squares        \ char sqr1 crn0 btw-lst'
    square-list-deallocate                  \ char sqr1 crn0

    \ Add it.
    rot                                     \ sqr1 crn0 char
    case
        [char] I of
            2dup                            \ sqr1 cnr0 sqr1 crn0
            corner-adjust-regions           \ sqr1 crn0
            corner-get-dissimilar-squares   \ sqr1 dis-lst
            list-push-struct
        endof
        [char] C of
            corner-get-similar-squares      \ sqr1 dis-lst
            list-push-struct
        endof
        abort" Unexpected comparison result"
    endcase
;

\ Return true if a square is the corner anchor.
: corner-square-is-anchor? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    dup corner-get-anchor-square    \ sqr1 anc
    =
;

\ Return true if a square is in the similar squares list.
: corner-square-is-in-similar-squares? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    [ ' = ] literal                 \ sqr1 crn0 xt
    -rot                            \ xt sqr1 crn0
    dup corner-get-similar-squares  \ xt sqr1 siw-lst
    list-member?                    \ bool
;

\ Return true if a square is in the dissimilar squares list.
: corner-square-is-in-dissimilar-squares? ( sqr1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    [ ' = ] literal                     \ sqr1 crn0 xt
    -rot                                \ xt sqr1 crn0
    dup corner-get-dissimilar-squares   \ xt sqr1 siw-lst
    list-member?                        \ bool
;

\ Check a new, or changed, square.
\ Divide and conquer.
: corner-check-changed-square ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square
    over square-get-num-bits
    over corner-get-num-bits
    <> abort" number bit difference?"

    2dup corner-square-is-anchor?                   \ sqr1 crn0 bool
    if
        corner-check-anchor
        exit
    then

    2dup corner-square-is-in-similar-squares?       \ sqr1 crn0 bool
    if
        corner-check-similar-square
        exit
    then

    2dup corner-square-is-in-dissimilar-squares?    \ sqr1 crn0 bool
    if
        corner-check-dissimilar-square
        exit
    then

    corner-check-new-square
;

