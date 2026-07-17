\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-struct-id
    #6 constant corner-struct-number-cells

\ Struct fields
0                                       constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp              cell+   constant corner-anchor-square-disp      \ The anchor square.
corner-anchor-square-disp       cell+   constant corner-square-pairs-disp       \ A list of regions, no supersets, of the anchor and another square.
                                                                                \ Similar to the action incompatible pairs, except the
                                                                                \ other square can be Incompatible, or not incompatible.
corner-square-pairs-disp        cell+   constant corner-dissimilar-squares-disp \ Dissimilar, squares, from the square-pairs list.
                                                                                \ Some may need more samples. Some may not be adjacent.
corner-dissimilar-squares-disp  cell+   constant corner-other-squares-disp      \ Not dissimilar squares, from the square-pair list.
                                                                                \ Some may need more samples. Some may not be adjacent.
corner-other-squares-disp       cell+   constant corner-possible-regions-disp   \ Possibel regions the anchor is in, according to ~A + ~B calculations.
                                                                                \ An adjacent, similar, square, in GT one region, invalidates the corner.

\ Needs: Meta, resolve needs for cornerns by some criteria. Number of possible regions in, sharing squares with other corners, ...
\        pnc anchor.
\        pnc dissimilar squares.
\        Adjacent dissimilar squares, in preference to non-adjacent dissimilar squares.
\        Squares to resolve multiple possible regions, adjacent to the anchor, within a region intersection.

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

\ Return the square-pairs field from a corner instance.
: corner-get-square-pairs ( crn0 -- sqr )
    \ Check arg.
    assert( tos is-corner? )

    corner-square-pairs-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the square-pairs field from a corner instance, use only in this file.
: _corner-set-square-pairs ( reg-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region-list? )

    corner-square-pairs-disp +      \ Add offset.
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

\ Return the other-squares list field from a corner instance.
: corner-get-other-squares ( crn0 -- sta-lst )
    \ Check arg.
    assert( tos is-corner? )

    corner-other-squares-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the other-squares list field from a corner instance, use only in this file.
: _corner-set-other-squares ( sta-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square-list? )

    corner-other-squares-disp +       \ Add offset.
    !struct                             \ Set the field.
;

\ Return the possible-regions list field from a corner instance.
: corner-get-possible-regions ( crn0 -- reg-lst )
    \ Check arg.
    assert( tos is-corner? )

    corner-possible-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the possible-regions list field from a corner instance, use only in this file.
: _corner-set-possible-regions ( reg-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region-list? )

    corner-possible-regions-disp +      \ Add offset.
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
: corner-new ( sqr0 -- crn )
    \ Check args.
    assert( tos is-square? )

    \ Allocate space.
    corner-struct-id corner-mma         \ sta1 id mma
    struct-allocate                     \ sta1 crn

    \ Store anchor square.
    tuck _corner-set-anchor-square      \ crn

    \ Store square-pairs.
    list-new over                       \ crn lst crn
    _corner-set-square-pairs            \ crn

    \ Store dissimilar square list.
    list-new                            \ crn lst
    over _corner-set-dissimilar-squares \ crn

    \ Store other square list.
    list-new                            \ crn lst
    over _corner-set-other-squares      \ crn

    \ Store regions list.
    dup corner-get-num-bits             \ crn nb
    region-list-max-x                   \ crn reg-lst
    over _corner-set-possible-regions   \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    ." (anchor: "

    dup corner-get-anchor-square        \ crn0 sta
    .square-state                       \ crn0

    space ." square pairs: "
    dup corner-get-square-pairs         \ crn0 s-pr-lst
    .region-list                        \ crn0

    ."  dissimilar squares: "
    dup corner-get-dissimilar-squares   \ crn0 ext-sta-lst
    .square-list-states                 \ crn0

    ."  other squares: "
    dup corner-get-other-squares        \ crn0 ext-sta-lst
    .square-list-states                 \ crn0

    space ." possible regions: "
    dup corner-get-possible-regions     \ crn0 ext-sta-lst
    .region-list                        \ crn0

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
    corner-get-square-pairs         \ sta1 pr-lst
    region-list-uses-state?         \ bool
    cr ." corner-uses-square?: " dup .bool cr
;

\ Return true if any similar, or dissimilar, square is not currently used
\ and is between the anchor and another closest square.
: corner-can-square-be-added? ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )
    assert( over square-get-num-bits over corner-get-num-bits = )

    \ Check if square is already used.
    2dup corner-uses-square?                \ sqr1 crn0 bool
    if
        2dup
        false
        cr ." corner-can-square-be-added?: 1: " dup .bool cr
        \ cr ." exit 1" cr
        exit
    then

    \ Check if square anchor region is a superset of any square pair.
    swap square-get-state                   \ crn0 sta1
    over corner-get-anchor-state            \ crn0 sta1 anc-sta
    region-new                              \ crn0 reg'         \ anchor state is state-0 in region.
    tuck                                    \ reg' crn0 reg'
    swap corner-get-square-pairs            \ reg' reg' pr-lst
    region-list-any-subset-of? invert       \ reg' bool

    \ Clean up.
    swap region-deallocate                  \ bool
    
    cr ." corner-can-square-be-added?: 2: " dup .bool cr
;

\ Update corner square-pairs with new pairs.
: corner-update-square-pairs ( reg-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region-list? )

    dup corner-get-square-pairs     \ reg-lst1 crn0 crn-regs'
    -rot                            \ crn-regs' reg-lst1 crn0
    _corner-set-square-pairs        \ crn-regs'
    region-list-deallocate
;

\ Update corner possible regions with new regions.
: corner-update-possible-regions ( reg-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region-list? )

    dup corner-get-possible-regions \ reg-lst1 crn0 crn-regs'
    -rot                            \ crn-regs' reg-lst1 crn0
    _corner-set-possible-regions    \ crn-regs'
    region-list-deallocate
;

\ Recalculate possible-regions from dissimilar square-pairs.
\ When a close dissimilar square becomes similar, or a similar
\ square is between the anchor and a dissimilar square.
\ A closer dissimilar square requires only one intersection with
\ the existing possible regions.
: corner-recalc-possible-regions ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    \ Init new possible regions list.
    dup corner-get-num-bits                     \ crn0 nb
    region-list-max-x                           \ crn0 pos-lst'

    over corner-get-square-pairs                \ crn0 pos-lst' pr-lst

    foreach                                     \ crn0 pos-lst' pr-lnk
        dup link-get-data                       \ crn0 pos-lst' pr-lnk regx

        \ Check if pair is dissimilar.
        region-get-state-1                      \ crn0 pos-lst' pr-lnk sta1         \ The other square's state.
        #3 pick                                 \ crn0 pos-lst' pr-lnk sta1 crn0
        corner-get-dissimilar-squares           \ crn0 pos-lst' pr-lnk sta1 dis-lst
        square-list-find                        \ crn0 pos-lst' pr-lnk, sqr t | f

        if
            drop                                \ crn0 pos-lst' pr-lnk

            \ Get pair possible regions.
            dup link-get-data                   \ crn0 pos-lst' pr-lnk regx
            dup region-get-state-0              \ crn0 pos-lst' pr-lnk regx sta0
            swap region-get-state-1             \ crn0 pos-lst' pr-lnk sta0 sta1
            state-~a+~b                         \ crn0 pos-lst' pr-lnk pr-lst'

            \ Update pos-lst.
            #2 pick                             \ crn0 pos-lst' pr-lnk pr-lst' pos-lst'
            over                                \ crn0 pos-lst' pr-lnk pr-lst' pos-lst' pr-lst'
            region-list-intersections-nosubs    \ crn0 pos-lst' pr-lnk pr-lst' new-pos-lst'
            swap region-list-deallocate         \ crn0 pos-lst' pr-lnk new-pos-lst'
            rot region-list-deallocate          \ crn0 pr-lnk new-pos-lst'
            swap                                \ crn0 new-pos-lst' pr-lnk
        then
    next
                                                \ crn0 pos-lst'
    swap corner-update-possible-regions         \
;

\ Add a close, dissimilar square, to the corner.
\ Close meaning: No square is between the anchor square and the square to be added.
\ The square may be pnc, or need more samples.
\ The square may, or may not, be adjacent to the anchor square, although adjacent is preferable.
: corner-add-dissimilar-square ( sqr1 crn0 -- )
    cr ." corner-add-dissimilar-square: start"
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    \ Add square to dissimilar square list.
    2dup corner-get-dissimilar-squares list-push-struct

    \ Get anchor-state square-state region.
    over square-get-state                   \ sqr1 crn0 sta1
    over corner-get-anchor-state            \ sqr1 crn0 sta1 sta-anc
    region-new                              \ sqr1 crn0 regx            \ anchor state is state-0 in region.

    \ Get regions in square-pairs that are superset of new region.
    dup                                     \ sqr1 crn0 regx regx
    #2 pick corner-get-square-pairs         \ sqr1 crn0 regx regx pr-lst
    region-list-supersets-of                \ sqr1 crn0 regx sup-lst'

    \ Add new region to square-pairs.
    swap                                    \ sqr1 crn0 sup-lst' regx
    #2 pick corner-get-square-pairs         \ sqr1 crn0 sup-lst' regx pr-lst
    region-list-push-nosups                 \ sqr1 crn0 sup-lst' bool
    ifnot cr ." push-nosups failed?" abort then

    \ Get states of squares to remove from dissimilar-squares and other-squares.
    dup                                     \ sqr1 crn0 sup-lst' sup-lst'
    region-list-states                      \ sqr1 crn0 sup-lst' sta-lst'
    swap region-list-deallocate             \ sqr1 crn0 sta-lst'

    \ Remove squares in dissimilar-squares that match states in
    \ superset ( removed ) pairs.
    over corner-get-dissimilar-squares      \ sqr1 crn0 sta-lst' dis-lst

    2dup square-list-states-in              \ sqr1 crn0 sta-lst' dis-lst sta-in'
    dup list-is-empty?                      \ sqr1 crn0 sta-lst' dis-lst sta-in' bool
    if
        list-deallocate                     \ sqr1 crn0 sta-lst' dis-lst
        drop                                \ sqr1 crn0 sta-lst'
    else
        cr ." Removing dissimilar squares: " dup .state-list cr
        tuck swap                           \ sqr1 crn0 sta-lst' sta-in' sta-in' dis-lst
        square-list-remove-matching-squares \ sqr1 crn0 sta-lst' sta-in' num
        over list-get-length =              \ sqr1 crn0 sta-lst' sta-in' bool
        ifnot
            cr ." problem? number of squares removed does not match" cr
        then
        state-list-deallocate               \ sqr1 crn0 sta-lst'
    then

    \ Remove squares in other-squares that match states in
    \ superset ( removed ) pairs.
    over corner-get-other-squares           \ sqr1 crn0 sta-lst' oth-lst
    2dup square-list-states-in              \ sqr1 crn0 sta-lst' oth-lst sta-in'
    dup list-is-empty?                      \ sqr1 crn0 sta-lst' oth-lst sta-in' bool
    if
        list-deallocate                     \ sqr1 crn0 sta-lst' oth-lst
        drop                                \ sqr1 crn0 sta-lst'
    else
        cr ." Removing other squares: " dup .state-list cr
        tuck swap                           \ sqr1 crn0 sta-lst' sta-in' sta-in' oth-lst
        square-list-remove-matching-squares \ sqr1 crn0 sta-lst' sta-in' num
        over list-get-length =              \ sqr1 crn0 sta-lst' sta-in' bool
        ifnot
            cr ." problem? number of squares removed does not match" cr
        then
        state-list-deallocate               \ sqr1 crn0 sta-lst'
    then

    \ Clean up.
    state-list-deallocate                   \ sqr1 crn0

    \ Get pair possible regions.
    over square-get-state                   \ sqr1 crn0 sta1
    over corner-get-anchor-state            \ sqr1 crn0 sta1 sta0
    state-~a+~b                             \ sqr1 crn0 pr-pos-lst'

    \ Add calculation to current possible-regions.
    dup                                     \ sqr1 crn0 pr-pos-lst' pr-pos-lst'
    #2 pick corner-get-possible-regions     \ sqr1 crn0 pr-pos-lst' pr-pos-lst' crn-pos-lst
    region-list-intersections-nosubs        \ sqr1 crn0 pr-pos-lst' new-crn-pos-lst
    swap region-list-deallocate             \ sqr1 crn0 new-crn-pos-lst
    over corner-update-possible-regions     \ sqr1 crn0

    2drop
;

\ Add a close, non-dissimilar square, to the corner.
\ Close meaning: No square is between the anchor square and the square to be added.
\ The square may be pnc, or need more samples.
\ The square may, or may not, be adjacent to the anchor square, although adjacent is preferable.
: corner-add-other-square ( sqr1 crn0 -- )
    cr ." corner-add-other-square: start" cr
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    \ Add square to other square list.
    2dup corner-get-other-squares list-push-struct

    \ Get anchor-state square-state region.
    over square-get-state                   \ sqr1 crn0 sta1
    over corner-get-anchor-state            \ sqr1 crn0 sta1 sta-anc
    region-new                              \ sqr1 crn0 regx            \ anchor state is state-0 in region.

    \ Get regions in square-pairs that are superset of new region.
    dup                                     \ sqr1 crn0 regx regx
    #2 pick corner-get-square-pairs         \ sqr1 crn0 regx regx pr-lst
    region-list-supersets-of                \ sqr1 crn0 regx sup-lst'

    \ Add new region to square-pairs.
    swap                                    \ sqr1 crn0 sup-lst' regx
    #2 pick corner-get-square-pairs         \ sqr1 crn0 sup-lst' regx pr-lst
    region-list-push-nosups                 \ sqr1 crn0 sup-lst' bool
    ifnot cr ." push-nosups failed?" abort then

    \ Get states of squares to remove from dissimilar-squares and other-squares.
    dup                                     \ sqr1 crn0 sup-lst' sup-lst'
    region-list-states                      \ sqr1 crn0 sup-lst' sta-lst'
    swap region-list-deallocate             \ sqr1 crn0 sta-lst'

    \ Remove squares in dissimilar-squares that match states in
    \ superset ( removed ) pairs.
    over corner-get-dissimilar-squares      \ sqr1 crn0 sta-lst' dis-lst
    2dup square-list-states-in              \ sqr1 crn0 sta-lst' dis-lst sta-in'
    dup list-is-empty?                      \ sqr1 crn0 sta-lst' dis-lst sta-in' bool
    if
        list-deallocate                     \ sqr1 crn0 sta-lst' dis-lst
        drop                                \ sqr1 crn0 sta-lst'
    else
        cr ." Removing dissimilar squares: " dup .state-list cr
        tuck swap                           \ sqr1 crn0 sta-lst' sta-in' sta-in' dis-lst
        square-list-remove-matching-squares \ sqr1 crn0 sta-lst' sta-in' num
        over list-get-length =              \ sqr1 crn0 sta-lst' sta-in' bool
        ifnot
            cr ." problem? number of squares removed does not match" cr
        then
        state-list-deallocate               \ sqr1 crn0 sta-lst'

        \ Recalc possible regions.
        over                                \ sqr1 crn0 sta-lst' crn0
        corner-recalc-possible-regions      \ sqr1 crn0 sta-lst'
    then

    \ Remove squares in other-squares that match states in
    \ superset ( removed ) pairs.
    over corner-get-other-squares           \ sqr1 crn0 sta-lst' oth-lst
    2dup square-list-states-in              \ sqr1 crn0 sta-lst' oth-lst sta-in'
    dup list-is-empty?                      \ sqr1 crn0 sta-lst' oth-lst sta-in' bool
    if
        list-deallocate                     \ sqr1 crn0 sta-lst' oth-lst
        drop                                \ sqr1 crn0 sta-lst'
    else
        cr ." Removing other squares: " dup .state-list cr
        tuck swap                           \ sqr1 crn0 sta-lst' sta-in' sta-in' oth-lst
        square-list-remove-matching-squares \ sqr1 crn0 sta-lst' sta-in' num
        over list-get-length =              \ sqr1 crn0 sta-lst' sta-in' bool
        ifnot
            cr ." problem? number of squares removed does not match" cr
        then
        state-list-deallocate               \ sqr1 crn0 sta-lst'
    then

    \ Clean up.
    state-list-deallocate                   \ sqr1 crn0
    2drop
;

\ Add a square to a corner, if possible.
\ Return true if added.
: corner-add-square ( sqr1 crn0 -- bool )
    cr ." corner-add-square: start: " over .square-state cr
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )
    \ cr ." corner-add-square: start: " .stack-gbl cr

    \ Check if it can be added.
    2dup corner-can-square-be-added?    \ sqr1 crn0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Check square relationship to anchor.
    2dup corner-get-anchor-square   \ sqr1 crn0 sqr1 sqr-anc
    squares-compare                 \ sqr1 crn0 char
    [char] I =                      \ sqr1 crn0 bool
    if
        corner-add-dissimilar-square
    else
        corner-add-other-square
    then

    true
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
        dup corner-get-square-pairs region-list-deallocate
        dup corner-get-dissimilar-squares square-list-deallocate
        dup corner-get-other-squares square-list-deallocate
        dup corner-get-possible-regions region-list-deallocate

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

\ Check an othen square, after a change.
: corner-check-other-square ( sqr1 crn0 -- )
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

\ Return true if a square is in the other squares list.
: corner-square-is-in-other-squares? ( sqr1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-square? )

    [ ' = ] literal                 \ sqr1 crn0 xt
    -rot                            \ xt sqr1 crn0
    dup corner-get-other-squares    \ xt sqr1 siw-lst
    list-member?                    \ bool
;

