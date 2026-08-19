\ Implement a struct and functions for a region list corresponding to domains.
\
\ So the regions may be of different number of bits, and operations
\ on regioncorr lists are by corresponding item.

#47317 constant regioncorr-struct-id
    #2 constant regioncorr-struct-number-cells

\ Struct fields
0                                   constant regioncorr-header-disp \ 16-bits [0] struct id [1] use count
                                                                    \ optional values [2] Positive value ( 8 bits ) Negative ( abs ) value ( 8 bits )

regioncorr-header-disp    cell+     constant regioncorr-list-disp   \ Region list corresponding, in bits used, to the session domain list.


0 value regioncorr-mma \ Storage for region mma instance.

\ Init region mma, return the addr of allocated memory.
: regioncorr-mma-init ( num-items -- ) \ sets regioncorr-mma.
    dup 1 <
    abort" regioncorr-mma-init: Invalid number of items."

    cr ." Initializing RegionCorr store."
    regioncorr-struct-number-cells swap mma-new to regioncorr-mma
;

\ Check if tos is an allocated regioncorr.
: is-regioncorr? ( xRtosaddr -- bool )
    dup regioncorr-mma mma-is-item? \ addr bool
    if
        struct-get-id
        regioncorr-struct-id =      \ bool
    else
        drop
        false                       \ f
    then
;

' is-regioncorr? to is-regioncorr?-xt

\ Start accessors.

\ Return the list field from a region instance.
: regioncorr-get-list ( regc0 -- lst )
    \ Check arg.
    assert( tos is-regioncorr? )

    regioncorr-list-disp +    \ Add offset.
    @                         \ Fetch the field.
;

' regioncorr-get-list to regioncorr-get-list-xt

\ Set the list field from a region instance, use only in this file.
: _regioncorr-set-list ( lst1 regc0 -- )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-region-list? )
    assert( nos list-is-empty? invert )

    \ Store list
    regioncorr-list-disp +    \ Add offset.
    !struct                   \ Set the field.
;

\ Get the positive value.
: regioncorr-get-pos-value ( regc0 -- val )
    \ Check args.
    assert( tos is-regioncorr? )

    4c@                 \ val
;

\ Set the positive value.
: _regioncorr-set-pos-value ( val regc0 -- )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos 0 >= )
    assert( nos #256 < )

    4c!
;

\ Add a value to a regioncorr's positive value.
\ Skip if the addition results in an invalid value.
: regioncorr-add-pos-value ( val regc0 -- )
    assert( tos is-regioncorr? )
    assert( nos 0 >= )
    assert( nos #256 < )

    dup regioncorr-get-pos-value    \ val regc0 pos
    rot +                           \ regc0 new-pos

    dup #256 < ifnot 2drop exit then
    dup 0 >= ifnot 2drop exit then

    swap _regioncorr-set-pos-value  \
;

\ Get the negative value.
: regioncorr-get-neg-value ( regc0 -- val )
    \ Check args.
    assert( tos is-regioncorr? )

    5c@                 \ val
    -1 *                \ -val
;

\ Set the negative value.
: _regioncorr-set-neg-value ( val regc0 -- )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos 0 <= )
    assert( nos #-256 > )

    swap abs swap       \ val regc0
    5c!
;

\ Add a value to a regioncorr's negative value.
\ Skip if the addition results in an invalid value.
: regioncorr-add-neg-value ( val regc0 -- )
    assert( tos is-regioncorr? )
    assert( nos 0 <= )
    assert( nos #-256 > )

    dup regioncorr-get-neg-value    \ val regc0 neg
    rot +                           \ regc0 new-neg

    dup #-256 > ifnot 2drop exit then
    dup 0 <= ifnot 2drop exit then

    swap _regioncorr-set-neg-value  \
;

\ End accessors.

\ Create a regioncorr from a region-list.
: regioncorr-new ( reg-lst0 -- regc )
    \ Check arg.
    assert( tos is-region-list? )

    \ Allocate space.
    regioncorr-struct-id regioncorr-mma
    struct-allocate                     \ reg-lst0 regc

    \ Store list.
    tuck                                \ regc reg-lst0 regc
    _regioncorr-set-list                \ regc

    0 over _regioncorr-set-pos-value    \ regc
    0 over _regioncorr-set-neg-value    \ regc
;

\ Return a copy of a regioncorr.
: regioncorr-copy ( regc0 -- regc )
    \ Check arg.
    assert( tos is-regioncorr? )

    dup regioncorr-get-pos-value swap   \ pos regc0
    dup regioncorr-get-neg-value swap   \ pos neg regc0

    regioncorr-get-list                 \ pos neg reg-lst
    regioncorr-new                      \ pos neg regc

    tuck _regioncorr-set-neg-value      \ pos regc
    tuck _regioncorr-set-pos-value      \ regc
;

\ Print a region-list corresponding to the session domain list.
: .regioncorr ( regc0 -- )
    \ Check arg.
    assert( tos is-regioncorr? )

    ." ( regc "
    dup regioncorr-get-pos-value    \ regc pos
    dec.
    dup regioncorr-get-neg-value    \ regc neg
    space dec.

    regioncorr-get-list             \ lst
    .region-list
    ." )"
;

' .regioncorr to .regioncorr-xt

\ Deallocate the given regc, if its use count is 1 or 0.
: regioncorr-deallocate ( regc0 -- )
    \ Check arg.
    assert( tos is-regioncorr? )

    dup struct-get-use-count            \ regc0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup regioncorr-get-list   \ regc0 reg-lst
        region-list-deallocate

        \ Deallocate instance.
        regioncorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if TOS is a superset of its corresponding region in NOS.
: regioncorr-superset? ( regc1 regc0 -- bool )
    \ cr ." regioncorr-superset?: " dup .regioncorr space ." sup " over .regioncorr
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ link0 regc1
    regioncorr-get-list list-get-links swap \ link1 link0

    begin
        ?dup
    while
                                    \ link1 link0
        \ Compare regions.
        over link-get-data          \ link1 link0 reg2
        over link-get-data          \ link1 link0 reg2 reg1
        region-superset?            \ link1 link0 bool
        ifnot
            \ Non-superset found.
            2drop
            false
            \ space ." bool: " dup .bool cr
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0
        swap link-get-next          \ link0 link1
        swap link-get-next          \ link1 link0
    repeat
                                    \ link1
    drop                            \
    true                            \ bool
    \ space ." bool: " dup .bool cr
;

\ Return true if TOS is a subset of its corresponding region in NOS.
: regioncorr-subset? ( regc1 regc0 -- bool )
    swap regioncorr-superset?
;

\ Return true if two regioncorrs intersect.
: regioncorrs-intersect? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0

    begin
        ?dup
    while
        \ Check regions
        over link-get-data          \ link1 link0 reg1
        over link-get-data          \ link1 link0 reg1 reg0
        regions-intersect?          \ link1 link0  bool
        ifnot
            2drop
            false
            exit
        then

        \ Prep for next cycle.
                                     \ link1 link0
        swap link-get-next           \ link0 link1
        swap link-get-next           \ link1 link0
    repeat
                                    \ link1
    drop                            \
    true
;

\ Return a new regioncorr, with one item replaced.
: regioncorr-copy-except ( reg2 cnt1 regc0 -- regc )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( 3os is-region? )

    -rot                            \ regc0 reg2 cnt1
    #2 pick                         \ regc0 reg2 cnt1 regc0
    regioncorr-get-list             \ regc0 reg2 cnt1 reg-lst
    list-copy-except-struct         \ regc0 reg-lst2
    regioncorr-new                  \ regc0 regc

    \ Set positive and negative values.
    over regioncorr-get-pos-value   \ regc0 regc pos
    over _regioncorr-set-pos-value  \ regc0 regc
    swap regioncorr-get-neg-value   \ regc neg
    over _regioncorr-set-neg-value  \ regc
;

\ Return regc0 minus regc1, giving a regioncorr list.
: regioncorr-subtract ( regc1 regc0 -- regc-lst )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ cr ." regioncorr-subtract: regioncorr: " dup .regioncorr
    \ cr ." minus:      " over .regioncorr

    \ Check for a superset subtrahend.
    2dup swap
    regioncorr-superset?                    \ regc1 regc0 bool
    if
        2drop
        list-new
        exit
    then

    \ Check that the two lists intersect.
    2dup regioncorrs-intersect?             \ regc1 regc0 bool
    ifnot
        list-new                            \ regc1 regc0 ret-lst
        tuck list-push-struct               \ regc1 ret-lst
        nip
        exit
    then

    \ Save regc0
    tuck                                    \ regc0 regc1 regc0

    \ Init return list, and item counter.
    list-new -rot                           \ regc0 ret-lst regc1 regc0

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ regc0 ret-lst link0 regc1
    regioncorr-get-list list-get-links swap \ regc0 ret-lst link1 link0

    #3 pick regioncorr-get-list             \ regc0 ret-lst link1 link0 reg-lst
    list-get-length                         \ regc0 ret-lst link1 link0 len
    0                                       \ regc0 ret-lst link1 link0 len 0
    do
        \ Subtract two regioncorrs.
        over link-get-data                  \ regc0 ret-lst link1 link0 reg1
        over link-get-data                  \ regc0 ret-lst link1 link0 reg1 reg0

        \ Check for superset subtrahend.
        2dup swap                           \ regc0 ret-lst link1 link0 reg1 reg0 reg0 reg1
        region-superset?                    \ regc0 ret-lst link1 link0 reg1 reg0 bool
        if
            \ No action on superset subtrahend.
            \ But it is known that not all subtrahend regions are supersets,
            \ due to the earlier test.
            2drop                           \ regc0 ret-lst link1 link0 d-link
        else
            \ If the subtrahend is not a superset, it must intersect,
            \ due to the earlier test.

            \ cr dup .region space ." - " over .region
            region-subtract                 \ regc0 ret-lst link1 link0 reg-lst
            \ space ." = " dup .region-list cr

            dup list-get-length 0= abort" region subtraction failed?"

            \ Generate result regioncorrs
            dup list-get-links              \ regc0 ret-lst link1 link0 reg-lst link
            begin
                ?dup
            while
                dup link-get-data           \ regc0 ret-lst link1 link0 reg-lst link | regx
                i                           \ regc0 ret-lst link1 link0 reg-lst link | regx ctr
                #7 pick                     \ regc0 ret-lst link1 link0 reg-lst link | regx ctr regc0
                regioncorr-copy-except      \ regc0 ret-lst link1 link0 reg-lst link | reg-lst2
                #5 pick                     \ regc0 ret-lst link1 link0 reg-lst link | reg-lst2 ret-lst
                list-push-struct            \ regc0 ret-lst link1 link0 reg-lst link

                link-get-next
            repeat
                                            \ regc0 ret-lst link1 link0 reg-lst
            region-list-deallocate          \ regc0 ret-lst link1 link0
        then

        \ Prep for next cycle.              \ regc0 ret-lst link1 link0
        swap link-get-next                  \ regc0 ret-lst link0 link1
        swap link-get-next                  \ regc0 ret-lst link1 link0
    loop

    \ Clean up.                             \ regc0 ret-lst link1 link0
    2drop                                   \ regc0 ret-lst
    nip                                     \ ret-lst
    \ cr ." =           " dup .regioncorr-list-xt execute cr
;

' regioncorr-subtract to regioncorr-subtract-xt

: is-valid-neg-value? ( val -- bool )
    dup 0> if drop false exit then
    abs #256 <
;

: is-valid-pos-value? ( val -- bool )
    dup 0< if drop false exit then
    #256 <
;

\ Check if a list could be a regioncorr definintion.
: regioncorr-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regioncorr-valid-list?: start: " dup .struct-list cr

    \ Check list length.
    dup list-get-length #4 =
    ifnot
        \ cr ." regioncorr-valid-list?: exit 1: " cr
        drop false exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        \ cr ." regioncorr-valid-list?: exit 2: " cr
        drop false exit
    then

    s" regc"                            \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        \ cr ." regioncorr-valid-list?: exit 3: " cr
        drop false exit
    then

    \ Check positive value.
    dup list-get-second-item            \ lst second
    is-valid-pos-value?
    ifnot
        \ cr ." regioncorr-valid-list?: exit 4: " cr
        drop false exit
    then

    \ Check-negative value.
    dup list-get-third-item             \ lst third
    is-valid-neg-value?
    ifnot
        \ cr ." regioncorr-valid-list?: exit 5: "( regc 1  -4 (rx1x1 r0x111))
        drop false exit
    then

    \ Check region list.
    dup list-get-fourth-item            \ lst fourth
    is-region-list?
    ifnot
        \ cr ." regioncorr-valid-list?: exit 6: " cr
        drop false exit
    then

    dup list-get-fourth-item            \ lst fourth
    list-is-empty?
    if
        \ cr ." regioncorr-valid-list?: exit 7: " cr
        drop false exit
    then

    drop
    true
    \ cr ." regioncorr-valid-list?: end: " dup .bool cr
;

\ Return a regioncorr from a list.
: regioncorr-from-list ( lst -- regc t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regioncorr-from-list: start" cr

    dup regioncorr-list-definition?     \ lst bool
    ifnot
        drop false exit
    then

    \ Allocate new regioncorr.
    dup list-get-fourth-item            \ lst fourth
    regioncorr-new                      \ lst regc

    \ Set positive value.
    over list-get-second-item           \ lst regc pos
    over _regioncorr-set-pos-value      \ lst regc

    \ Set negative value.
    swap list-get-third-item            \ regc neg
    over _regioncorr-set-neg-value      \ regc

    true
    \ cr ." regioncorr-from-list: end: " .stack-gbl cr
;
\ Return a regioncorr from a string.
\ Like (1 -2 (rxxxx r1010))
\ Like (0 0 (rxxxx r1010))
: regioncorr-from-string ( str-addr str-n -- regc t | f )
    \ cr ." regioncorr-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        exit
    then

    dup regioncorr-from-list                \ lst, regc t | f
    ifnot
        struct-list-deallocate
    else
        swap
        struct-list-deallocate
        true
    then
;


\ Return a regioncorr from a string, or abort.( regc 1  -4 (rx1x1 r0x111))
: regioncorr-from-string-a ( str-addr str-n -- regc )
    regioncorr-from-string    \ regc t | f
    false? abort" regioncorr-from-string-a failed?"
;

\ Return the number of bits different between two regioncorr.
: regioncorr-distance ( regc1 regc0 -- nb )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init counter.
    0 -rot                  \ cnt regc1 regc0

    \ Prep for loop.
    regioncorr-get-list list-get-links swap   \ cnt link0 regc1
    regioncorr-get-list list-get-links swap   \ cnt link1 link0

    begin
        ?dup
    while
        \ Add one region pair distance.( regc 1  -4 (rx1x1 r0x111))
        rot                     \ link1 link0 cnt
        #2 pick link-get-data   \ link1 link0 cnt reg1
        #2 pick link-get-data   \ link1 link0 cnt reg1 reg0
        region-distance         \ link1 link0 cnt dist
        +                       \ link1 link0 cnt
        -rot                    \ cnt link1 link0

        \ Point to next pair.
        swap link-get-next
        swap link-get-next
    repeat
                                \ cnt link1
    drop                        \ cnt
;

\ Return true if two regioncorr are adjacent.
: regioncorr-adjacent? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    regioncorr-distance   \ nb
    1 =
;

\ Return the intersection of two regioncorrs.
: regioncorr-intersection ( regc1 regc0 -- regc t | f )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Check for no intersection.
    2dup regioncorrs-intersect?             \ regc1 regc0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Add up values.
    over regioncorr-get-pos-value           \ regc1 regc0 val1
    over regioncorr-get-pos-value           \ regc1 regc0 val1 val0
    + -rot                                  \ pos regc1 regc0

    over regioncorr-get-neg-value           \ pos regc1 regc0 val1
    over regioncorr-get-neg-value           \ pos regc1 regc0 val1 val0
    + -rot                                  \ pos neg regc1 regc0

    \ Init return list.
    list-new -rot                           \ pos neg reg-lst regc1 regc0

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ pos neg reg-lst link0 regc1
    regioncorr-get-list list-get-links swap \ pos neg reg-lst link1 link0

    begin
        ?dup
    while                                   \ pos neg reg-lst link1 link0
        \ Check regions
        over link-get-data                  \ pos neg reg-lst link1 link0 reg1
        over link-get-data                  \ pos neg reg-lst link1 link0 reg1 reg0

        region-intersection                 \ pos neg reg-lst link1 link0 reg-int t | f
        invert abort" intersection failed?"

        #3 pick                             \ pos neg reg-lst link1 link0 reg-int reg-lst
        region-list-push-end                \ pos neg reg-lst link1 link0

        \ Prep for next cycle.              \ pos neg reg-lst link1 link0
        swap link-get-next                  \ pos neg reg-lst link0 link1
        swap link-get-next                  \ pos neg reg-lst link1 link0
    repeat
                                            \ pos neg reg-lst link1
    drop                                    \ pos neg reg-lst

    \ Make return regioncorr.
    regioncorr-new                          \ pos neg regc

    tuck _regioncorr-set-neg-value          \ pos regc
    tuck _regioncorr-set-pos-value          \ regc

    true
;

\ Return true if two regioncorrs region lists are equal.
: regioncorrs-eq-regions? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Get region lists.
    regioncorr-get-list swap                \ reg-lst0 regc1
    regioncorr-get-list swap                \ reg-lst1 reg-lst0

    region-lists-corr-eq?
;

\ Return true if two regioncorrs are equal.
: regioncorrs-eq? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Check positive value.
    over regioncorr-get-pos-value           \ regc1 regc0 p1
    over regioncorr-get-pos-value           \ regc1 regc0 p1 p0
    <> if 2drop false exit then

    \ Check negative value.
    over regioncorr-get-neg-value           \ regc1 regc0 n1
    over regioncorr-get-neg-value           \ regc1 regc0 n1 n0
    <> if 2drop false exit then

    \ Get region lists.
    regioncorr-get-list swap                \ reg-lst0 regc1
    regioncorr-get-list swap                \ reg-lst1 reg-lst0

    region-lists-corr-eq?
;

\ Set tos values to zero.
: regioncorr-init-values ( regc0 -- )
    \ Check arg.
    assert( tos is-regioncorr? )

    0 over _regioncorr-set-pos-value
    0 swap _regioncorr-set-neg-value
;
