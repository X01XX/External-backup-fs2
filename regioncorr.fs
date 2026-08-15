\ Implement a struct and functions for a region list corresponding to domains.

#47317 constant regioncorr-struct-id
    #2 constant regioncorr-struct-number-cells

\ Struct fields
0                                   constant regioncorr-header-disp \ 16-bits [0] struct id [1] use count
                                                                    \ [2] Positive value ( 8 bits ) Negative value ( 8 bits )

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
: is-regioncorr? ( addr -- bool )
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

    \ Store list
    regioncorr-list-disp +    \ Add offset.
    !struct                   \ Set the field.
;

\ End accessors.

\ Create a regioncorr from a region-list corresponding, in order, to domains.
: regioncorr-new ( reg-lst0 -- addr)
    \ check arg.
    assert( tos is-region-list? )

    dup list-get-length
    number-domains-gbl
    <> abort" regioncorr-new: invalid list length?"

    \ Allocate space.
    regioncorr-struct-id regioncorr-mma
    struct-allocate             \ reg-lst0 regc

    \ Store list.
    tuck                          \ regc reg-lst0 regc
    _regioncorr-set-list          \ regc
;

\ Return a copy of a regioncorr.
: regioncorr-copy ( regc0 -- regc )
    \ Check arg.
    assert( tos is-regioncorr? )

    regioncorr-get-list     \ reg-lst
    regioncorr-new
;

\ Print a region-list corresponding to the session domain list.
: .regioncorr ( regc0 -- )
    \ Check arg.
    assert( tos is-regioncorr? )

    regioncorr-get-list             \ lst
    list-get-links                  \ link0
    get-domain-list-gbl             \ link0 dom-lst
    list-get-links                  \ link0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ link0 d-link domx
        domain-set-current-gbl      \ link0 d-link

        over link-get-data          \ link0 d-link reg0
        .region                     \ link0 d-link

        swap link-get-next          \ d-link link0
        swap link-get-next          \ link0 d-link
        dup if
            space
        then
    repeat
                                    \ link0
    drop
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
    get-domain-list-gbl list-get-links      \ link1 link0 d-link

    begin
        ?dup
    while
                                    \ link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ link1 link0 d-link domx
        domain-set-current-gbl      \ link1 link0 d-link

        \ Compare regions.
        #2 pick link-get-data       \ link1 link0 d-link reg2
        #2 pick link-get-data       \ link1 link0 d-link reg2 reg1
        region-superset?            \ link1 link0 d-link bool
        ifnot
            \ Non-superset found.
            3drop
            false
            \ space ." bool: " dup .bool cr
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0 d-link
        rot link-get-next           \ link0 d-link link1
        rot link-get-next           \ d-link link1 link0
        rot link-get-next           \ link1 link0 d-link
    repeat
                                    \ link1 link0
    2drop                           \
    true                            \ bool
    \ space ." bool: " dup .bool cr
;

\ Return true if TOS is a subset of its corresponding region in NOS.
: regioncorr-subset? ( regc1 regc0 -- bool )
    swap regioncorr-superset?
;

: regioncorr-intersects? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0
    get-domain-list-gbl list-get-links        \ link1 link0 d-link

    begin
        ?dup
    while
                                    \ link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ link1 link0 d-link domx
        domain-set-current-gbl      \ link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ link1 link0 d-link reg1
        #2 pick link-get-data       \ link1 link0 d-link reg1 reg0
        regions-intersect?          \ link1 link0 d-link bool
        ifnot
            3drop
            false
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0 d-link
        rot link-get-next           \ link0 d-link link1
        rot link-get-next           \ d-link link1 link0
        rot link-get-next           \ link1 link0 d-link
    repeat
                                    \ link1 link0
    2drop                           \
    true
;

\ Return a new regioncorr, with one item replaced.
: regioncorr-copy-except ( reg2 cnt1 rc0 -- rc )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( 3os is-region? )

    regioncorr-get-list         \ reg2 cnt1 reg-lst
    list-copy-except-struct     \ reg-lst2
    regioncorr-new              \ rc
;

\   Return regc0 minus regc1, a list of regioncorr.
: regioncorr-subtract ( regc1 regc0 -- regc-lst t | f )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

\    cr ." regioncorr-subtract: "
\    cr ." regioncorr: " dup .regioncorr
\    cr ." minus:      " over .regioncorr

    \ Check for a superset subtrahend.
    2dup swap
    regioncorr-superset?          \ regc1 regc0 bool
    abort" Subtrahend is a superset?"

    \ Check that the two lists intersect.
    2dup regioncorr-intersects?   \ regc1 regc0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Save regc0
    tuck                                \ regc0 regc1 regc0

    \ Init return list, and counter.
    list-new -rot                   \ regc0 ret-lst regc1 regc0
    0 >r                            \ regc0 ret-lst regc1 regc0, r: \ ctr

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ regc0 ret-lst link0 regc1
    regioncorr-get-list list-get-links swap   \ regc0 ret-lst link1 link0
    get-domain-list-gbl list-get-links        \ regc0 ret-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ regc0 ret-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ regc0 ret-lst link1 link0 d-link domx
        domain-set-current-gbl      \ regc0 ret-lst link1 link0 d-link

        \ Subtract two regioncorrs.
        #2 pick link-get-data       \ regc0 ret-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ regc0 ret-lst link1 link0 d-link reg1 reg0

        \ Check for superset subtrahend.
        2dup swap                   \ regc0 ret-lst link1 link0 d-link reg1 reg0 reg0 reg1
        region-superset?            \ regc0 ret-lst link1 link0 d-link reg1 reg0 bool
        if
            \ No action on superset subtrahend.
            \ But it is known that not all subtrahend regions are supersets,
            \ due to the earlier test.
            2drop                   \ regc0 ret-lst link1 link0 d-link
        else
            \ If the subtrahend is not a superset, it must intersect,
            \ due to the earlier test.

            \ cr dup .region space ." - " over .region
            region-subtract             \ regc0 ret-lst link1 link0 d-link reg-lst
            \ space ." = " dup .region-list cr

            dup list-get-length 0= abort" region subtraction failed?"

            \ Generate result regioncorrs
            dup list-get-links          \ regc0 ret-lst link1 link0 d-link reg-lst link
            begin
                ?dup
            while
                dup link-get-data       \ regc0 ret-lst link1 link0 d-link reg-lst link | regx
                r@                      \ regc0 ret-lst link1 link0 d-link reg-lst link | regx ctr
                #8 pick                 \ regc0 ret-lst link1 link0 d-link reg-lst link | regx ctr regc0
                regioncorr-copy-except  \ regc0 ret-lst link1 link0 d-link reg-lst link | reg-lst2
                #6 pick                 \ regc0 ret-lst link1 link0 d-link reg-lst link | reg-lst2 ret-lst
                list-push-struct        \ regc0 ret-lst link1 link0 d-link reg-lst link

                link-get-next
            repeat
                                        \ regc0 ret-lst link1 link0 d-link reg-lst
            region-list-deallocate      \ regc0 ret-lst link1 link0 d-link
        then

        \ Prep for next cycle.
        r> 1+ >r                    \ regc0 ret-lst link1 link0 d-link, r: \ ctr+
                                    \ regc0 ret-lst link1 link0 d-link
        rot link-get-next           \ regc0 ret-lst link0 d-link link1
        rot link-get-next           \ regc0 ret-lst d-link link1 link0
        rot link-get-next           \ regc0 ret-lst link1 link0 d-link
    repeat

    \ Clean up.
    r> drop                         \ regc0 ret-lst link1 link0, r: \
                                    \ regc0 ret-lst link1 link0
    2drop                           \ regc0 ret-lst
    nip                             \ ret-lst
\    cr ." =           " dup .regioncorr-list-xt execute cr
    true
;

\ Return a regioncorr from a string.
: regioncorr-from-string ( str-addr str-n -- regc t | f )
    \ cr ." regioncorr-from-string: start: " 2dup type cr
    list-from-string-xt execute             \ lst t | f
    if
        [ ' is-region? ] literal            \ lst xt
        over list-apply-all-true?           \ lst bool
        if
            dup region-list-corresponding?  \ lst bool
            if
                regioncorr-new
                true
            else
                region-list-deallocate
                false
            then
        else
            structinfo-list-deallocate-struct-list-xt execute
            false
        then
    else
        false
    then
;

\ Return a regioncorr from a string, or abort.
: regioncorr-from-string-a ( str-addr str-n -- regc )
    regioncorr-from-string    \ regc t | f
    false? abort" regioncorr-from-string-a failed?"
;

\ Return the complement of a regioncorr, a list of regioncorr.
: regioncorr-complement ( regc0 -- cmp-regc-lst )
    \ Check arg.
    assert( tos is-regioncorr? )

    regioncorr-max-regions-gbl          \ regc0 regc-max
    tuck                                \ regc-max regc0 regc-max
    regioncorr-subtract                 \ regc-max, cmp-regc-lst t | f
    false? abort" subtract failed?"

    swap                                \ cmp-regc-lst regc-max
    regioncorr-deallocate               \ cmp-regc-lst
;

\ Return the numbr of bits different between two regioncorr.
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
        \ Add one region pair distance.
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

: regioncorr-intersection ( regc1 regc0 -- regc t | f )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot                           \ reg-lst regc1 regc0

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ reg-lst link0 regc1
    regioncorr-get-list list-get-links swap \ reg-lst link1 link0
    get-domain-list-gbl list-get-links      \ reg-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ reg-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ reg-lst link1 link0 d-link domx
        domain-set-current-gbl      \ reg-lst link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ reg-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ reg-lst link1 link0 d-link reg1 reg0
        region-intersection         \ reg-lst link1 link0 d-link, reg-int t | f
        if                          \ reg-lst link1 link0 d-link reg-int
            #4 pick                 \ reg-lst link1 link0 d-link reg-int reg-lst
            region-list-push-end    \ reg-lst link1 link0 d-link
        else
            3drop                   \ reg-lst
            region-list-deallocate  \
            false                   \ bool
            exit
        then

        \ Prep for next cycle.
                                    \ reg-lst link1 link0 d-link
        rot link-get-next           \ reg-lst link0 d-link link1
        rot link-get-next           \ reg-lst d-link link1 link0
        rot link-get-next           \ reg-lst link1 link0 d-link
    repeat
                                    \ reg-lst link1 link0
    2drop                           \ reg-lst
    regioncorr-new
    true
;

\ Return true if two regioncorrs are equal.
: regioncorrs-eq? ( regc1 regc0 -- bool )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0
    get-domain-list-gbl list-get-links        \ link1 link0 d-link

    begin
        ?dup
    while
                                    \ link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ link1 link0 d-link domx
        domain-set-current-gbl      \ link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ link1 link0 d-link reg1
        #2 pick link-get-data       \ link1 link0 d-link reg1 reg0
        regions-eq?                 \ link1 link0 d-link bool
        ifnot
            3drop
            false
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0 d-link
        rot link-get-next           \ link0 d-link link1
        rot link-get-next           \ d-link link1 link0
        rot link-get-next           \ link1 link0 d-link
    repeat
                                    \ link1 link0
    2drop                           \
    true
;

: ?regioncorr-union ( regc1 regc0 -- regc )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )
    \ cr ." regioncorr-union: " over .regioncorr space ." and: " dup .regioncorr

    \ Init return list.
    list-new -rot                           \ reg-lst regc1 regc0

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ reg-lst link0 regc1
    regioncorr-get-list list-get-links swap \ reg-lst link1 link0
    get-domain-list-gbl list-get-links      \ reg-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ reg-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ reg-lst link1 link0 d-link domx
        domain-set-current-gbl      \ reg-lst link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ reg-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ reg-lst link1 link0 d-link reg1 reg0
        ?region-union               \ reg-lst link1 link0 d-link reg-int'
        #4 pick                     \ reg-lst link1 link0 d-link reg-int' reg-lst
        region-list-push-end        \ reg-lst link1 link0 d-link

        \ Prep for next cycle.
                                    \ reg-lst link1 link0 d-link
        rot link-get-next           \ reg-lst link0 d-link link1
        rot link-get-next           \ reg-lst d-link link1 link0
        rot link-get-next           \ reg-lst link1 link0 d-link
    repeat
                                    \ reg-lst link1 link0
    2drop                           \ reg-lst
    regioncorr-new
    \ space ." = " dup .regioncorr cr
;
