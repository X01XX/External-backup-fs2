\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ The corner starts with a state, the anchor, that is only in one region.
\ It is implied that states adjacent to the anchor, external to the region,
\ are dissimilar to the anchor.  That can be tested.
\
\ An adjacent, external, state that is similar to the anchor, where both have
\ been sampled to pnc, invalidates the corner.
#53719 constant corner-struct-id
    #4 constant corner-struct-number-cells

\ Struct fields
0                                   constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count, [2] Rate ( 8 bits ).
                                                                            \ Rate will be the number of adjacent states that are only in one region.
corner-header-disp          cell+   constant corner-anchor-state-disp       \ The anchor square state.
corner-anchor-state-disp    cell+   constant corner-region-disp             \ Region the anchor is in, according to ~A + ~B calculation.
corner-region-disp          cell+   constant corner-adjacent-states-disp    \ All adjacent to anchor, external, states.


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

\ Return the anchor-state field from a corner instance.
: corner-get-anchor-state ( crn0 -- sta )
    \ Check arg.
    assert( tos is-corner? )

    corner-anchor-state-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the anchor-state field from a corner instance, use only in this file.
: _corner-set-anchor-state ( sqr1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state? )

    corner-anchor-state-disp +      \ Add offset.
    !struct                         \ Set the field.
;

\ Return the adjacent-states list field from a corner instance.
: corner-get-adjacent-states ( crn0 -- sta-lst )
    \ Check arg.
    assert( tos is-corner? )

    corner-adjacent-states-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the adjacent-states list field from a corner instance, use only in this file.
: _corner-set-adjacent-states ( sta-lst1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state-list? )

    corner-adjacent-states-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the region field from a corner instance.
: corner-get-region ( crn0 -- reg )
    \ Check arg.
    assert( tos is-corner? )

    corner-region-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the region field from a corner instance, use only in this file.
: _corner-set-region ( reg1 crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region? )

    corner-region-disp +       \ Add offset.
    !struct                             \ Set the field.
;

\ Get the rate value.
: corner-get-rate ( crn0 -- rt )
    \ Check arg.
    assert( tos is-corner? )

    4c@
;

\ Set the rate value.
: corner-set-rate ( rt crn0 -- )
    \ Check args.
    assert( tos is-corner? )
    assert( nos 0 >= )
    assert( nos [ 1 cells #8 * ] literal < )

    4c!
;

\ End accessors.

\ Return a corner's number bits.
: corner-get-num-bits ( crn0 -- nb )
    \ Check args.
    assert( tos is-corner? )

    corner-get-anchor-state        \ sta
    state-get-num-bits
;

\ Create a corner, given a regios and an anchor state.
: corner-new ( sta1 reg0 -- crn t | f )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    assert( over state-get-num-bits over region-get-num-bits = )

    2dup region-superset-of-state?      \ sta1 reg0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Init adjacent state list.
    list-new                            \ sta1 reg0 sta-lst

    \ Fill adjacent states list.
    over region-edge-mask               \ sta1 reg0 sta-lst edg-msk
    dup mask-split                      \ sta1 reg0 sta-lst edg-msk' msk-lst'
    swap mask-deallocate                \ sta1 reg0 sta-lst msk-lst'
    dup                                 \ sta1 reg0 sta-lst msk-lst' msk-lst'
    foreach                             \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx
        \ Calc one anchor-adjacent, external to region, state.
        #5 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx sta1
        state-xor-mask                  \ sta1 reg0 sta-lst msk-lst' msk-lnk sta'

        \ Store the state.
        #3 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk sta' sta-lst
        list-push-struct                \ sta1 reg0 sta-lst msk-lst' msk-lnk
    next-item
                                        \ sta1 reg0 sta-lst msk-lst'
    mask-list-deallocate                \ sta1 reg0 sta-lst

    \ Allocate instance.
    corner-struct-id corner-mma         \ sta1 reg0 sta-lst id mma
    struct-allocate                     \ sta1 reg0 sta-lst crn

    \ Store adjacent states list.
    tuck _corner-set-adjacent-states

    \ Store region.
    tuck _corner-set-region             \ sta1 crn

    \ Store anchor state.
    tuck _corner-set-anchor-state       \ crn

    \ Set rate.
    0 over corner-set-rate              \ crn

    true
;

\ Return a token containing a string representation of a corner.
: corner-token ( crn0 -- tkn )
    \ Check arg.
    assert( tos is-corner? )

    s" (" token-new                 \ crn0, tkn t | f
    invert abort" token-new failed"

    s" crn "                        \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn bool
    invert abort" token-concatenate failed"

    over corner-get-anchor-state    \ crn0 tkn sta
    state-string                    \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn tkn' bool
    invert abort" token-concatenate failed"

    s"  "                           \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn bool
    invert abort" token-concatenate failed"

    over corner-get-region          \ crn0 tkn reg
    region-string                   \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn tkn' bool
    invert abort" token-concatenate failed"

    s"  "                           \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn bool
    invert abort" token-concatenate failed"

    over corner-get-adjacent-states \ crn0 tkn sta-lst
    state-list-string               \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn tkn' bool
    invert abort" token-concatenate failed"

    s" )"                           \ crn0 tkn c-addr u
    #2 pick token-concatenate       \ crn0 tkn bool
    invert abort" token-concatenate failed"

    nip
;

\ Return a string representation of a corner.
: corner-string ( crn0 -- c-addr u )
    \ Check arg.
    assert( tos is-corner? )

    corner-token            \ tkn'
    dup token-string-to-pad \ tkn' c-addr u
    rot token-deallocate    \ c-addr u
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    \ Put corner string on stack.
    corner-string           \ c-addr u

    type
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
        dup corner-get-anchor-state state-deallocate
        dup corner-get-adjacent-states state-list-deallocate
        dup corner-get-region region-deallocate

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

\ Return true if a corner anchor is equal to a given state.
: corner-anchor-eq-state? ( sta1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state? )

    corner-get-anchor-state \ sta1 crn-sta
    states-eq?
;

\ Return true if a state is used by a corner.
: corner-uses-state? ( sta1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-state? )

    over                            \ sta1 crn0 sta1
    over corner-get-anchor-state    \ sta1 crn0 sta1 sta0
    states-eq?                      \ sta1 crn0 bool
    if
        2drop
        true
        exit
    then

    [ ' states-eq? ] literal -rot   \ xt sta1 crn0
    corner-get-adjacent-states      \ xt sta1 crn-stas
    list-member?                    \ bool
;

\ Return a state list with all states in a corner.
: corner-states ( crn0 -- sta-lst )
    \ Check arg.
    assert( tos is-corner? )

    \ Init return list.
    list-new            \ crn0 ret-lst
    over corner-get-anchor-state    \ crn0 ret-lst anc-sta
    over list-push-struct           \ crn0 ret-lst
    swap corner-get-adjacent-states \ ret-lst adj-lst
    over state-list-append          \ ret-lst
;

\ Return true if the tos corner is a proper superset of the nos corner.
: corner-is-proper-superset? ( crn1 crn0 -- bool )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-corner? )

    \ Compare the states.
    corner-states                   \ crn1 stas0'
    swap corner-states              \ stas0' stas1'
    2dup                            \ stas0' stas1' stas0' stas1'
    swap                            \ stas0' stas1' stas1' stas0'
    state-list-is-proper-superset?  \ stas0' stas1' bool

    \ Clean up.
    swap state-list-deallocate      \ stas0' bool
    swap state-list-deallocate      \ bool
;

\ Return a list containing a given corner, and connected corners,
\ that is, a corner cluster.
\ The given corner should have at least one adjacent state that is
\ in only one possible group.
\ A corner cluster has at least two corners.
\ Each corner will have at least one shared state with at least one other corner.
\ The corner list will not contain duplicate corner regions.
: corner-additional-corners ( pos-regs1 crn0 -- crn-lst t | f )
    \ Check args.
    assert( tos is-corner? )
    assert( nos is-region-list? )

    \ Init return list.
    list-new                                \ pos-regs1 crn0 ret-lst
    tuck list-push-struct                   \ pos-regs1 ret-lst
    dup                                     \ pos-regs1 ret-lst ret-lst

    \ Check each corner.
    \ The list starts with one corner.
    \ New corners can be added to the end of the list, which the loop will get to next.
    foreach                                 \ pos-regs1 ret-lst ret-lnk crnx
        corner-get-adjacent-states          \ pos-regs1 ret-lst ret-lnk adj-lst

        \ Check each adjacent state.
        foreach                             \ pos-regs1 ret-lst ret-lnk adj-lnk stax
            #4 pick                         \ pos-regs1 ret-lst ret-lnk adj-lnk stax pos-regs1
            region-list-state-in            \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in'
            dup list-get-length 1 <>        \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' bool
            if
                region-list-deallocate      \ pos-regs1 ret-lst ret-lnk adj-lnk
            else

                \ Check corner is not already added.
                dup list-get-first-item     \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' reg
                #4 pick                     \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' reg ret-lst
                corner-list-find-region-xt  \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' reg ret-lst xt
                execute                     \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in', crn t | f
                if
                    drop                    \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in'
                    region-list-deallocate  \ pos-regs1 ret-lst ret-lnk adj-lnk
                else
                    \ Build corner.
                    over link-get-data      \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' stax
                    over                    \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' stax reg-in'
                    list-get-first-item     \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' stax regx
                    corner-new              \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in', crnx t | f
                    invert abort" corner-new failed?"

                    \ Store corner.
                    #4 pick                 \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' crnx ret-lst
                    list-push-end-struct    \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in'
                    region-list-deallocate  \ pos-regs1 ret-lst ret-lnk adj-lnk
                then
            then
        next-item
    next-item
                                            \ pos-regs1 ret-lst
    swap drop                               \ ret-lst
    dup list-get-length 1 =                 \ ret-lst bool
    if
        \ Return corner argument to its original use count.
        dup list-pop-struct                 \ ret-lst, crn t

        \ Clean up, return.
        2drop
        list-deallocate
        false
    else
        true
    then
;

\ Calculate a corner's rate, the number af adjacent states
\ that are in only one possible region.
\ Set the rate in the corner.
: corner-calc-set-rate ( pos-lst1 crn0 -- )
    tuck corner-get-adjacent-states     \ crn0 pos-lst adj-lst

    \ Init counter.
    0 swap                              \ crn0 pos-lst cnt adj-lst

    foreach                             \ crn0 pos-lst cnt adj-lnk stax
        #3 pick                         \ crn0 pos-lst cnt adj-lnk stax pos-lst
        region-list-num-state-in        \ crn0 pos-lst cnt adj-lnk num-in
        1 =                             \ crn0 pos-lst cnt adj-lnk bool
        if
            \ Inc counter.
            swap 1+ swap                \ crn0 pos-lst cnt adj-lnk
        then
    next-item
                                        \ crn0 pos-lst cnt
    nip swap                            \ cnt crn0
    corner-set-rate
;

\ Check if a list could be a corner definintion.
: corner-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." corner-list-definition?: start: " dup .struct-list cr

    \ Check list length.
    dup list-get-length #4 =
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 1: false" cr
        exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 2: false" cr
        exit
    then

    s" crn"                             \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 3: false" cr
        exit
    then

    \ Check anchor value.
    dup list-get-second-item            \ lst second
    is-state?
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 4: false" cr
        exit
    then

    \ Check region.
    dup list-get-third-item            \ lst third
    is-region?
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 5: false" cr
        exit
    then

    \ Check anchor is in region. required.
    dup list-get-second-item            \ lst anc
    over list-get-third-item            \ lst anc reg
    region-superset-of-state?           \ lst bool
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 8: false" cr
        exit
    then

    \ Checks not strictly required, since the state-list is ignored.

    \ Check state list.
    dup list-get-fourth-item           \ lst fourth
    is-state-list?
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 6: false" cr
        exit
    then

    dup list-get-fourth-item            \ lst fourth
    list-is-empty?
    if
        drop false
        \ cr ." corner-list-definition?: exit 7: false" cr
        exit
    then

    \ Check number of adjacent states.
    dup list-get-third-item             \ lst reg
    region-edge-mask                    \ lst msk'
    dup mask-count-bits                 \ lst msk' u
    swap mask-deallocate                \ lst u
    over list-get-fourth-item           \ lst u sta-lst
    list-get-length =                   \ lst bool
    ifnot
        drop false
        \ cr ." corner-list-definition?: exit 8: false" cr
        exit
    then

    \ Check adjacent states are not in region.
    dup list-get-third-item             \ lst reg
    over list-get-fourth-item           \ lst reg sta-lst
    state-list-none-in-region?          \ lst bool
    ifnot
        drop
        false
        \ cr ." corner-list-definition?: exit 9: false" cr
        exit
    then

    \ Check adjacencies.
    dup list-get-second-item            \ lst anc
    over list-get-fourth-item           \ lst anc sta-lst
    state-list-all-adjacent-to-state?   \ lst bool
    ifnot
        drop
        false
        \ cr ." corner-list-definition?: exit 10: false" cr
        exit
    then

    drop
    true
    \ cr ." corner-list-definition?: end: true" cr
;

\ Return a corner from a list.
: corner-from-list ( lst -- crn t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." corner-from-list: start: " dup .struct-list cr

    dup corner-list-definition?     \ lst bool
    ifnot
        drop false exit
    then

    \ Allocate new corner.
    dup list-get-second-item        \ lst anc
    swap list-get-third-item        \ anc reg
    corner-new                      \ crn t | f

    \ cr ." corner-from-list: end: " over .corner cr
;
\ Return a corner from a string.
\ Like ( regc 1 -2 (rxxxx r1010))
\ Like ( regc 0 0 (rxxxx r1010))
: corner-from-string ( str-addr str-n -- crn t | f )
    \ cr ." corner-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." corner-from-string: exit 1: false" cr
        exit
    then

    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." corner-from-string: exit 2: false" cr
        exit
    then
                                            \ lst
    dup list-get-first-item                 \ lst itm
    is-corner?                           \ lst bool
    if
        dup list-pop-struct                 \ lst itm
        invert abort" pop faled?"
        swap list-deallocate                \ itm
        true
    else
        struct-list-deallocate
        false
    then
    \ cr ." corner-from-string: end: " dup .bool cr
;

\ Return a corner from a string, or abort.
: corner-from-string-a ( str-addr str-n -- regc )
    corner-from-string    \ regc t | f
    false? abort" corner-from-string failed?"
;

\ Return a list of possible regions implied by a corner.
: corner-possible-regions ( crn0 -- reg-lst )
    \ Check args.
    assert( tos is-corner? )

    \ Make max region list.
    dup corner-get-anchor-state             \ crn anc-sta
    dup state-get-num-bits                  \ crn anc-sta num-b
    region-max-x                            \ crn anc-sta max-reg
    list-new tuck                           \ crn anc-sta pos-regs' max-reg pos-regs'
    list-push-struct                        \ crn anc-sta pos-regs'

    \ Calc the effect of each anchor/adjacent-state pair.
    rot corner-get-adjacent-states          \ anc-sta pos-regs' adj-sta-lst

    foreach                                 \ anc-sta pos-regs' adj-lnk sta-adj
        \ Calc regions for this pair.
        #3 pick                             \ anc-sta pos-regs' adj-lnk sta-adj anc-sta
        state-~a+~b                         \ anc-sta pos-regs' adj-lnk reg-lst'

        \ Intersect regions with accumelated regions.
        dup                                 \ anc-sta pos-regs' adj-lnk reg-lst' reg-lst'
        #3 pick                             \ anc-sta pos-regs' adj-lnk reg-lst' reg-lst' pos-regs'
        region-list-intersections-nosubs    \ anc-sta pos-regs' adj-lnk reg-lst' pos-regs-next'

        \ Clean up.
        swap region-list-deallocate         \ anc-sta pos-regs' adj-lnk pos-regs-next'

        \ Replace previous list.
        rot region-list-deallocate          \ anc-sta adj-lnk pos-regs-next'
        swap                                \ anc-sta pos-regs-next' adj-lnk
    next-item
                                            \ anc-sta pos-regs'
    nip                                     \ pos-regs'
;
