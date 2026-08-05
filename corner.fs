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
: corner-get-anchor-state ( crn0 -- sqr )
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
: corner-new ( sta1 reg0 -- crn )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    assert( over state-get-num-bits over region-get-num-bits = )

    2dup region-superset-of-state?      \ sta1 reg0 bool
    ifnot cr ." corner-new: region not superset state?" abort then

    \ Init adjacent state list.
    list-new                            \ sta1 reg0 sta-lst

    \ Fill adjacent states list.
    over region-edge-mask               \ sta1 reg0 sta-lst edg-msk
    dup mask-split                      \ sta1 reg0 sta-lst edg-msk' msk-lst'
    swap mask-deallocate                \ sta1 reg0 sta-lst msk-lst'
    dup                                 \ sta1 reg0 sta-lst msk-lst' msk-lst'
    foreach                             \ sta1 reg0 sta-lst msk-lst' msk-lnk
        \ Calc one anchor-adjacent, external to region, state.
        dup link-get-data               \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx
        #5 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk mskx sta1
        state-xor-mask                  \ sta1 reg0 sta-lst msk-lst' msk-lnk sta'

        \ Store the state.
        #3 pick                         \ sta1 reg0 sta-lst msk-lst' msk-lnk sta' sta-lst
        list-push-struct                \ sta1 reg0 sta-lst msk-lst' msk-lnk
    next
    mask-list-deallocate                \ sta1 reg0 sta-lst

    \ Allocate space.
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
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert( tos is-corner? )

    ." ("
    dup corner-get-region               \ crn0 reg
    .region                             \ crn0

    space ." anchor: "
    dup corner-get-anchor-state         \ crn0 sta
    .state                              \ crn0

    space ." AE: "
    dup corner-get-adjacent-states      \ crn0 ext-sta-lst
    .state-list                         \ crn0

    ." )"
                                        \ crn0
    drop
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
    foreach                                 \ pos-regs1 ret-lst ret-lnk
        dup link-get-data                   \ pos-regs1 ret-lst ret-lnk crnx
        corner-get-adjacent-states          \ pos-regs1 ret-lst ret-lnk adj-lst

        \ Check each adjacent state.
        foreach                             \ pos-regs1 ret-lst ret-lnk adj-lnk
            dup link-get-data               \ pos-regs1 ret-lst ret-lnk adj-lnk stax
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
                    corner-new              \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' crnx

                    \ Store corner.
                    #4 pick                 \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in' crnx ret-lst
                    list-push-end-struct    \ pos-regs1 ret-lst ret-lnk adj-lnk reg-in'
                    region-list-deallocate  \ pos-regs1 ret-lst ret-lnk adj-lnk
                then
            then
        next
    next
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

    foreach                             \ crn0 pos-lst cnt adj-lnk
        dup link-get-data               \ crn0 pos-lst cnt adj-lnk stax
        #3 pick                         \ crn0 pos-lst cnt adj-lnk stax pos-lst
        region-list-num-state-in        \ crn0 pos-lst cnt adj-lnk num-in
        1 =                             \ crn0 pos-lst cnt adj-lnk bool
        if
            \ Inc counter.
            swap 1+ swap                \ crn0 pos-lst cnt adj-lnk
        then
    next
                                        \ crn0 pos-lst cnt
    nip swap                            \ cnt crn0
    corner-set-rate
;

\ Return false if a string is not a representation of a corner,
\
\ Otherwise, generate a corner from the string.
\ Valid chars are 0, 1, X, x, and underscore as separator.
\ All bit positions must be specified.
\ Like s" c01Xx" corner-from-string
\ The anchor will be the region state-0,
\ X will be 1 in state-0, x will be 0 in state-0.
: corner-from-string ( c-addr u --  crn t | f)

    \ Check length GT 1.
    dup #2 <
    if
        2drop
        false
        exit
    then

    \ Check for prefix char.
    over c@ [char] c <>
    if
        2drop
        false
        exit
    then

    \ Inc address.
    swap 1+ swap

    \ Dec len.
    1-

    \ Init character counter.
    0 swap              \ c-addr cnt u

    \ Init state 1, state 0, and do initial value.
    0 swap              \ c-addr cnt num1 u
    0 swap              \ c-addr cnt num1 num0 u
    0                   \ c-addr cnt num1 num0 u 0

    \ For each character...
    do                  \ c-addr cnt num1 num0
        \ Get a character.
        #3 pick         \ c-addr cnt num1 num0 c-addr
        i +             \ c-addr cnt num1 num0 c-addr+
        c@              \ c-addr cnt num1 num0 chr

        \ Process character.
        case
            [char] 0 of
                        \ Leave bit positions as 0/0.
                        \ Update num1
                        swap 1 lshift
                        \ Update num0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] 1 of
                        \ Set bit positions to 1/1.
                        \ Update num1
                        swap 1 lshift 1+
                        \ Update num0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] X of
                        \ Set bit positions to 1/0.
                        \ Update num1
                        swap 1 lshift
                        \ Update num0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] x of
                        \ Set bit positions to 0/1.
                        \ Update num1
                        swap 1 lshift 1+
                        \ Update num0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] _ of
                    endof
            \ Unrecognized character, return false.

            \ Drop stack items.
            2drop
            2drop
            drop

            \ Set return bool.
            false

            \ Cancel do loop.
            unloop

            \ Return.
            exit
        endcase
    loop

    \ Create state 1.       \ c-addr cnt num1 num0
    swap                    \ c-addr cnt num0 num1
    #2 pick                 \ c-addr cnt num0 num1 cnt
    state-new               \ c-addr cnt num0 sta1

    \ Create state 0.
    -rot                    \ c-addr sta1 cnt num0
    swap                    \ c-addr sta1 num0 cnt
    state-new               \ c-addr sta1 sta0

    \ Make new corner, return.
                            \ c-addr sta1 sta0
    tuck region-new         \ csaddr sta0 reg
    corner-new              \ c-addr crn
    nip                     \ crn
    true
;
