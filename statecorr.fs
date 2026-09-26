
\ Implement a struct and functions for a state list corresponding to domains.
\
\ So the states may be of different number of bits, and operations
\ on statecorr list pairs are by corresponding items.

#61979 constant statecorr-struct-id
    #2 constant statecorr-struct-number-cells

\ Struct fields
0                                   constant statecorr-header-disp \ 16-bits [0] struct id [1] use count
statecorr-header-disp    cell+      constant statecorr-list-disp   \ State list corresponding, in bits used, to the session domain list.

0 value statecorr-mma \ Storage for state mma instance.

\ Init statecorr mma, return an address of allocated memory.
: statecorr-mma-init ( num-items -- ) \ sets statecorr-mma.
    dup 1 <
    abort" statecorr-mma-init: Invalid number of items."

    cr ." Initializing StateCorr store."
    statecorr-struct-number-cells swap mma-new to statecorr-mma
;

\ Check if tos is an allocated statecorr.
: is-statecorr? ( tos -- bool )
    dup statecorr-mma mma-is-item?  \ tos bool
    if
        struct-get-id
        statecorr-struct-id =       \ bool
    else
        drop
        false                       \ f
    then
;

' is-statecorr? to is-statecorr?-xt

\ Start accessors.

\ Return the list field from a statecorr instance.
: statecorr-get-list ( stac0 -- stac-lst )
    \ Check arg.
    assert( tos is-statecorr? )

    statecorr-list-disp +   \ Add offset.
    @                       \ Fetch the field.
;

' statecorr-get-list to statecorr-get-list-xt

\ Set the list field from a statecorr instance, use only in this file.
: _statecorr-set-list ( stac-lst1 stac0 -- )
    \ Check args.
    assert( tos is-statecorr? )
    assert( nos is-state-list? )
    \ assert( nos list-is-empty? invert )

    \ Store list
    statecorr-list-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a statecorr from a state-list.
: statecorr-new ( sta-lst0 -- stac )
    \ Check arg.
    assert( tos is-state-list? )
    assert( tos list-is-not-empty? )

    \ Allocate instance.
    statecorr-struct-id statecorr-mma
    struct-allocate                     \ sta-lst0 stac

    \ Store list.
    tuck                                \ stac sta-lst0 stac
    _statecorr-set-list                 \ stac

;

\ Print a state-list corresponding to the session domain list.
: .statecorr ( stac0 -- )
    \ Check arg.
    assert( tos is-statecorr? )

    ." ( stac "
    statecorr-get-list             \ lst
    .state-list
    ." )"
;

' .statecorr to .statecorr-xt

\ Deallocate the given stac, if its use count is 1 or 0.
: statecorr-deallocate ( stac0 -- )
    \ Check arg.
    assert( tos is-statecorr? )

    dup struct-get-use-count            \ stac0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup statecorr-get-list          \ stac0 sta-lst
        state-list-deallocate

        \ Deallocate instance.
        statecorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check if a list could be a statecorr definintion.
: statecorr-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." statecorr-list-definition?: start: " dup .struct-list cr

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." statecorr-list-definition?: exit 1: false " cr
        exit
    then

    s" stac"                            \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." statecorr-list-definition?: exit 2: false " cr
        exit
    then

    \ Check state list.
    dup list-get-second-item            \ lst second
    is-state-list?
    ifnot
        drop false
        \ cr ." statecorr-list-definition?: exit 3: false " cr
        exit
    then

    dup list-get-second-item            \ lst second
    list-is-empty?
    if
        drop false
        \ cr ." statecorr-list-definition?: exit 4: false " cr
        exit
    then

    drop
    true
    \ cr ." statecorr-list-definition?: end: true" cr
;

\ Return a statecorr from a list.
: statecorr-from-list ( lst -- stac t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." statecorr-from-list: start" cr

    dup statecorr-list-definition?     \ lst bool
    ifnot
        drop false
        \ cr ." statecorr-from-list: end false" cr
        exit
    then

    \ Allocate new statecorr.
    dup list-get-second-item            \ lst second
    statecorr-new                       \ lst stac

    nip                                 \ stac

    true
    \ cr ." statecorr-from-list: end true: " over .statecorr cr
;
\ Return a statecorr from a string.
\ Like ( stac (s1010 s1010))
: statecorr-from-string ( str-addr str-n -- stac t | f )
    \ cr ." statecorr-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." statecorr-from-string: exit 1: false" cr
        exit
    then
                                            \ lst
    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." statecorr-from-string: exit 2: false" cr
        exit
    then
                                            \ lst
    dup list-get-first-item                 \ lst itm
    is-statecorr?                           \ lst bool
    if
        dup list-pop-struct                 \ lst itm
        invert abort" pop faled?"
        swap list-deallocate                \ itm
        true
        \ cr ." statecorr-from-string: end: true"  .stack cr
        exit
    else
        struct-list-deallocate
        false
        \ cr ." statecorr-from-string: exit 3: false" cr
        exit
    then
;

\ Return a statecorr from a string, or abort.( stac 1  -4 (rx1x1 r0x111))
: statecorr-from-string-a ( str-addr str-n -- stac )
    statecorr-from-string    \ stac t | f
    false? abort" statecorr-from-string failed?"
;

\ Return the number of bits different between two statecorr.
: statecorr-distance ( regc1 regc0 -- nb )
    \ Check args.
    assert( tos is-statecorr? )
    assert( nos is-statecorr? )

    \ Init counter.
    0 -rot                  \ cnt regc1 regc0

    \ Prep for loop.
    statecorr-get-list list-get-links swap   \ cnt link0 regc1
    statecorr-get-list list-get-links swap   \ cnt link1 link0

    begin
        ?dup
    while
        \ Add one state pair distance.( regc 1  -4 (rx1x1 r0x111))
        rot                     \ link1 link0 cnt
        #2 pick link-get-data   \ link1 link0 cnt reg1
        #2 pick link-get-data   \ link1 link0 cnt reg1 reg0
        states-distance         \ link1 link0 cnt dist
        +                       \ link1 link0 cnt
        -rot                    \ cnt link1 link0

        \ Point to next pair.
        swap link-get-next
        swap link-get-next
    repeat
                                \ cnt link1
    drop                        \ cnt
;

\ Return true if two statecorrs are equal.
: statecorrs-eq? ( stac1 stac0 -- bool )
    \ Check args.
    assert( tos is-statecorr? )
    assert( nos is-statecorr? )

    statecorr-get-list list-get-links swap         \ lnk0 sta-lst1
    statecorr-get-list list-get-links              \ lnk0 lnk1

    begin
        ?dup
    while
        over link-get-data      \ lnk0 lnk1 sta0
        over link-get-data      \ lnk0 lnk1 sta0 sta1
        states-eq?             \ lnk0 lnk1 bool
        ifnot
            2drop
            false
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
                                \ lnk0
    drop
    true
;

\ Ruturn a regioncorr from the union of two statecorrs.
: statecorr-union ( stac1 stac0 -- regc )
    \ Check args.
    assert( tos is-statecorr? )
    assert( nos is-statecorr? )

    \ Init region list.
    list-new -rot               \ reg-lst stac1 stac0

    \ Prep for loop.
    statecorr-get-list
    list-get-links swap         \ reg-lst stac-lnk stac1

    statecorr-get-list
    list-get-links              \ reg-lst stac-lnk stac-lnk

    begin
        ?dup
    while
        \ Get two states.
        over link-get-data      \ reg-lst stac-lnk stac-lnk stacx
        over link-get-data      \ reg-lst stac-lnk stac-lnk stacx stacy

        \ Make region.
        region-new              \ reg-lst stac-lnk stac-lnk reg

        \ Store region, same order.
        #3 pick                 \ reg-lst stac-lnk stac-lnk reg reg-lst
        list-push-end-struct    \ reg-lst stac-lnk stac-lnk

        \ Prep for next loop cycle.
        link-get-next swap
        link-get-next
    repeat
                                \ reg-lst stac-lnk
    \ Clean up.
    drop                        \ reg-lst

    \ Return.
    regioncorr-new              \ regc
;

\ Return a copy of a statecorr.
: statecorr-copy ( stac0 -- stac )
    \ Check arg.
    assert( tos is-statecorr? )

    statecorr-get-list                 \ sta-lst
    statecorr-new                      \ neg stac
;
