\ Check TOS for regioncorr-list.
: is-regioncorr-list? ( tos -- bool )
    dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-regioncorr?          \ bool
;

\ Deallocate a regioncorr list.
: regioncorr-list-deallocate ( regc-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorr-list? if true else cr ." tos not regioncorr-list? " .stack cr false then )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regc-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' regioncorr-deallocate ] literal over    \ regc-lst0 xt regc-lst0
        list-apply                                  \ regc-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Push a regioncorr onto a list, if there are no regioncorrs with
\ duplicate regions in the list.
\ Return true if the regioncorr is added to the list.
: regioncorr-list-push-nodups ( regc1 regc-lst0 -- flag )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Return if any region in the list is a superset of regc1.
    2dup                                    \ regc1 regc-lst0 regc1 regc-lst0
    [ ' regioncorrs-eq-regions? ] literal   \ regc1 regc-lst0 regc1 regc-lst0 xt
    -rot                                    \ regc1 regc-lst0 xt regc1 regc-lst0
    list-member?                            \ regc1 regc-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ regc1 regc-lst0

    \ Add region to list.                   \ regc1 regc-lst0
    list-push-struct
    true
;

\ Remove the first subset region from a regioncorr-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: regioncorr-list-remove-subsets ( regc1 regc-lst0 -- bool )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    [ ' regioncorr-subset? ] literal    \ regc1 regc-lst0  xt
    -rot                                \ xt regc1 regc-lst0

    list-remove                         \ regc2 t | f
    if
        regioncorr-deallocate
        true
    else
        false
    then
;

\ Push a regioncorr onto a list, if there are no supersets in the list.
\ If there are no supersets in the list, delete any subsets and push the region.
\ Return true if the region is added to the list.
: regioncorr-list-push-nosubs ( regc1 regc-lst0 -- flag )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Return if any region in the list is a superset of regc1.
    2dup                                    \ regc1 regc-lst0 regc1 regc-lst0
    [ ' regioncorr-superset? ] literal      \ regc1 regc-lst0 regc1 regc-lst0 xt
    -rot                                    \ regc1 regc-lst0 xt regc1 regc-lst0
    list-member?                            \ regc1 regc-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ regc1 regc-lst0

    \ Remove all subsets.
    begin
        2dup                                \ regc1 regc-lst0 regc1 regc-lst0
        regioncorr-list-remove-subsets      \ regc1 regc-lst0 | flag
    while
    repeat

    \ Add region to list.                   \ regc1 regc-lst0
    list-push-struct
    true
;

\ Return a TOS regioncorr-list minus the NOS regioncorr.
: regioncorr-list-subtract-regioncorr ( regc1 regc-lst0 -- ret-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot                               \ ret-lst regc1 regc-lst0

    \ Scan through the given list.
    foreach                                     \ ret-lst regc1 regc-lnk0 regc2
        #2 pick swap                            \ ret-lst regc1 regc-lnk0 regc1 regc2

        \ Test if equal
        2dup regioncorr-subset?                 \ ret-lst regc1 regc-lnk0 regc1 regc2 flag
        if
            \ Skip, region does not appear in the result.
            2drop
        else
            \ Check if they intersect
            2dup regioncorrs-intersect?         \ ret-lst regc1 regc-lnk0 regc1 regc2 flag
            if
                \ They intersect, there will be some remainder.
                regioncorr-subtract-xt execute  \ ret-lst regc1 regc-lnk0 remainder-lst

                \ Add remainders to the return list
                dup                             \ ret-lst regc1 regc-lnk0 rem-lst rem-lst
                foreach                         \ ret-lst regc1 regc-lnk0 rem-lst rem-lnk rem-reg
                    #5 pick                     \ ret-lst regc1 regc-lnk0 rem-lst rem-lnk rem-reg ret-lst
                    regioncorr-list-push-nosubs \ ret-lst regc1 regc-lnk0 rem-lst rem-lnk flag
                    drop                        \ ret-lst regc1 regc-lnk0 rem-lst rem-lnk
                next
                                                \ ret-lst regc1 regc-lnk0 rem-lst
                regioncorr-list-deallocate      \ ret-lst regc1 regc-lnk0
            else
                \ Add whole region to the result.
                nip                             \ ret-lst regc1 regc-lnk0 regc2
                #3 pick                         \ ret-lst regc1 regc-lnk0 regc2 ret-lst
                regioncorr-list-push-nosubs     \ ret-lst regc1 regc-lnk0 flag
                drop                            \ ret-lst regc1 regc-lnk0
            then
        then
    next
                                                \ ret-lst regc1
    drop                                        \ ret-lst
;

\ Return a list of intersections of any two regioncorrs in a list.
: regioncorr-list-self-intersections-nodups ( regc-lst0 -- ret-lst t | f )
    \ Check arg.
    assert( tos is-regioncorr-list? )
    \ cr ." regioncorr-list-self-intersections-nodups: start" cr
    \ Init return list.
    list-new swap                           \ ret-lst regc-lst0

    list-get-links                          \ ret-lst regc-lnk0
    begin
        ?dup
    while
        dup link-get-next                   \ ret-lst regc-lnk1 regc-lnk2
        begin
            ?dup
        while
            over link-get-data              \ ret-lst regc-lnk1 regc-lnk2 regc1
            over link-get-data              \ ret-lst regc-lnk1 regc-lnk2 regc1 regc2

            regioncorr-intersection         \ ret-lst regc-lnk1 regc-lnk2, regc-int' t | f
            if
                dup                         \ ret-lst regc-lnk1 regc-lnk2 regc-int' regc-int'
                #4 pick                     \ ret-lst regc-lnk1 regc-lnk2 regc-int' regc-int' ret-lst
                regioncorr-list-push-nodups \ ret-lst regc-lnk1 regc-lnk2 regc-int' bool
                if
                    drop                    \ ret-lst regc-lnk1 regc-lnk2
                else
                    regioncorr-deallocate   \ ret-lst regc-lnk1 regc-lnk2
                then
            then
        next
    next
                                            \ ret-lst
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
    \ cr ." regioncorr-list-self-intersections-nodups: end" cr
;

\ Return a copy of a regioncorr-list.
: regioncorr-list-copy ( regc-lst0 -- regc-lst )
    \ Check arg.
    assert( tos is-regioncorr-list? )

    \ Init return list.
    list-new swap               \ ret-lst regc-lst0

    foreach                     \ ret-lst regc-lnk0 regc
        #2 pick                 \ ret-lst regc-lnk0 regc ret-lst
        list-push-end-struct    \ ret-lst regc-lnk0
    next
                                \ ret-lst
;

\ From the TOS regioncorr-list, subtract the NOS regioncorr-list.
: regioncorr-list-subtract ( regc-lst1 regc-lst0 -- ret-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    \ Make a list that way be returned empty, or deallocated.
    regioncorr-list-copy                    \ regc-lst1 ret-lst

    swap                                    \ ret-lst regc-lst1

    \ Process each region in regc-lst1.
    foreach                                 \ ret-lst regc-lnk1 regc0
        rot                                 \ regc-lnk1 regc0 ret-lst
        swap                                \ regc-lnk1 ret-lst regc0
        over                                \ regc-lnk1 retc-lst regc0 ret-lst
        regioncorr-list-subtract-regioncorr \ regc-lnk1 retc-lst retc-lst-new
        -rot                                \ ret-lst-new regc-lnk1 ret-lst
        regioncorr-list-deallocate          \ ret-lst-new regc-lnk1
    next
                                            \ ret-lst
;

\ Append nos regioncorr-list to the tos regioncorr-list, except duplicates.
: regioncorr-list-append-nodups ( regc-lst1 regc-lst0 -- )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    swap                            \ regc-lst0 regc-lst1
    list-get-links                  \ regc-lst0 link
    begin
        ?dup
    while
        dup link-get-data           \ regc-lst0 link regx
        #2 pick                     \ regc-lst0 link regx regc-lst0
        regioncorr-list-push-nodups \ regc-lst0 link bool
        drop

        link-get-next
    repeat
                                    \ regc-lst0
    drop
;

\ Print a regioncorr list.
: .regioncorr-list ( regc-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorr-list? )
    ." ("
    foreach                 \ regc-lnk regcx
        .regioncorr
        link-get-next
        dup 0> if space then
    repeat
    ." )"
;

: .regioncorr-list-prefix ( c-addr u regc-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorr-list? )
    cr
    rot                 \ u regc-lst0 c-addr
    #2 pick             \ u regc-lst0 c-addr u
    type                \ u regc-lst0

    dup list-is-empty?
    if
        ." None"
        2drop
        exit
    then

    foreach             \ u lnk grpx
        .regioncorr

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

: regioncorr-list-rate-regioncorr ( regc1 regc-lst0 -- )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Set nos values to zero.
    over
    regioncorr-init-values                  \ regc1 regc-lst0

    foreach                                 \ regc1 regc-lnk0 regc0
        #2 pick swap                        \ regc1 regc-lnk0 regc1 regc0
        regioncorr-superset?                \ regc1 regc-lnk0 bool
        if
            \ Get superset values.
            dup link-get-data               \ regc1 regc-lnk0 regc0
            dup regioncorr-get-pos-value    \ regc1 regc-lnk0 regc0 pos
            swap regioncorr-get-neg-value   \ regc1 regc-lnk0 pos neg

            \ Set subset values.regioncorr-list-prefix
            #3 pick                         \ regc1 regc-lnk0 pos neg regc1
            regioncorr-add-neg-value        \ regc1 regc-lnk0 pos
            #2 pick                         \ regc1 regc-lnk0 pos regc1
            regioncorr-add-pos-value        \ regc1 regc-lnk0
        then
    next
    drop
;

\ Rate regioncorrs in tos by supersets in nos.
: regioncorr-list-rate-by ( regc-lst1 regc-lst0 -- )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    \ Set rates as an accumulation of superset values.
    foreach                             \ regc-lst1 regc-lnk0 regc0
        #2 pick                         \ regc-lst1 regc-lnk0 regc0 regc-lst1
        regioncorr-list-rate-regioncorr \ regc-lst1 regc-lnk0
    next
    drop
;

\ Split a regioncorr list by intersections.
\ So each result regioncorr is a subset of one, or more, of the original regioncorrs,
\ but never a proper intersection.
\ Regioncorrs with duplicate regions are allowed in the input.
: regioncorr-list-split-by-intersections ( regc-lst0 -- ret-lst t | f )
    \ Check arg.
    assert( tos is-regioncorr-list? )

    \ Save original list.
    dup                                             \ regc-lst0 regc-lst0
    \ Try first pass.
    dup regioncorr-list-self-intersections-nodups   \ regc-lst0  regc-lst0, int-regcs' t | f
    ifnot
        drop
        false
        exit
    then
    \ cr ." first pass: " dup .regioncorr-list cr

    \ Init return list.
    list-new -rot                                   \ regc-lst0  ret-lst regc-lst0 int-regcs'

    \ Get arg regions minus intersections.
    2dup swap                                       \ regc-lst0  ret-lst regc-lst0 int-regcs' int-regcs' regc-lst0
    regioncorr-list-subtract                        \ regc-lst0  ret-lst regc-lst0 int-regcs' remc-lst'
    \ cr ." remainders: " dup .regioncorr-list cr

    \ Add remainders to the return list.
    dup #4 pick                                     \ regc-lst0  ret-lst regc-lst0 int-regcs' remc-lst' remc-lst' ret-lst
    regioncorr-list-append-nodups                   \ regc-lst0  ret-lst regc-lst0 int-regcs' remc-lst'

    \ Replace the current regions with the intersections.
    regioncorr-list-deallocate                      \ regc-lst0  ret-lst regc-lst0 int-regcs'
    swap drop                                       \ regc-lst0  ret-lst int-regcs'

    begin
        dup                                         \ regc-lst0  ret-lst cur-regcs' cur-regcs'
        regioncorr-list-self-intersections-nodups   \ regc-lst0  ret-lst cur-regcs', int-regcs' t | f
        if
            \ Get current regions minus intersections.
            2dup swap                               \ regc-lst0  ret-lst cur-regcs' int-regcs' int-regcs' cur-regcs'
            regioncorr-list-subtract                \ regc-lst0  ret-lst cur-regcs' int-regcs' rem-lst'
            \ cr ." remainders: " dup .regioncorr-list cr

            \ Add remainders to the return list.
            dup #4 pick                             \ regc-lst0  ret-lst cur-regcs' int-regcs' rem-lst' rem-lst' ret-lst
            regioncorr-list-append-nodups           \ regc-lst0  ret-lst cur-regcs' int-regcs' rem-lst'

            \ Replace the current regions with the intersections.
            regioncorr-list-deallocate              \ regc-lst0  ret-lst cur-regcs' int-regcs'
            swap regioncorr-list-deallocate         \ regc-lst0  ret-lst int-regcs'
        else
            \ No new intersections, add whats left.
            2dup swap                               \ regc-lst0  ret-lst cur-regcs' cur-regcs' ret-lst
            regioncorr-list-append-nodups           \ regc-lst0  ret-lst cur-regsc'
            \ cr ." remainders: " dup .regioncorr-list cr
            regioncorr-list-deallocate              \ regc-lst0  ret-lst

            \ Set intersection fragment rates.
            tuck                                    \ ret-lst regc-lst0 ret-lst
            regioncorr-list-rate-by                 \ ret-lst

            \ Return.
            true
            exit
        then
    again
;

' .regioncorr-list to .regioncorr-list-xt

\ Return true if two regioncorr lists are equal.
: regioncorr-lists-eq? ( regc-lst1 regc-lst0 -- bool )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )
    \ cr ." regioncorr-lists-eq?: start: " .stack cr

    \ Check list lengths.
    over list-get-length
    over list-get-length                    \ regc-lst1 regc-lst0 len1 len0
    <>
    if
        2drop
        false
        \ cr ." regioncorr-lists-eq?: exit 1" .s cr
        exit
    then

    \  Check list contents.
    foreach                                 \ regc-lst1 lnk0 regcx
        \ Check if its in the other list.
        [ ' regioncorrs-eq? ] literal swap  \ regc-lst1 lnk0 xt regcx
        #3 pick                             \ regc-lst1 lnk0 xt regcx lst1
        list-member?                        \ regc-lst1 lnk0 flag

        ifnot
            \ dup link-get-data cr ." not a member: " .regioncorr cr
            2drop
            false
            \ cr ." regioncorr-lists-eq?: exit 2" .s cr
            exit
        then
    next
                                            \ regc-lst1
    drop
    true
    \ cr ." regioncorr-lists-eq?: exit 3" .s cr
;

: regioncorr-list-init-pos-value-to-1 ( regc-lst0 -- )
    \ Check arg.
    assert( tos is-regioncorr-list? )

    foreach             \ regc-lnk regc
        regioncorr-init-pos-value-to-1
    next
;

\ Return true if all items in a regioncorr list are a
\ superset of a given regioncorr.
: regioncorr-list-all-superset? ( regc1 regc-lst0 -- bool )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    list-get-links                  \ regc1 regc-lnk0
    begin
        ?dup
    while
        dup link-get-data           \ regc1 regc-lnk0 regcx
        #2 pick                     \ regc1 regc-lnk0 regcx regc1
        swap                        \ regc1 regc-lnk0 regc1 regcx
        regioncorr-superset?        \ regc1 regc-lnk0 regc-lnk1 bool
        ifnot
            2drop drop
            false
            exit
        then
    next
                                    \ regc1
    drop
    true
;

\ Return the complement of a non-empty regioncorr list.
: regioncorr-list-complement ( regc-lst0 -- regc-lst )
    \ Check arg.
    assert( tos is-regioncorr-list? )
    assert( tos list-is-not-empty? )

    \ Init complement list.
    list-new                            \ regc-lst0 comp-lst'
    over list-get-first-item            \ regc-lst0 comp-lst' regc0
    regioncorr-max-x                    \ regc-lst0 comp-lst' regc-max
    over list-push-struct               \ regc-lst0 comp-lst'

    tuck                                \ comp-lst' regc-lst0 comp-lst'
    regioncorr-list-subtract            \ comp-lst' ret-lst
    swap regioncorr-list-deallocate     \ ret-lst
;

\ Return the set intersection of two regioncorr lists.
: regioncorr-list-set-intersection ( regc-lst1 regc-lst0 -- regc-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    [ ' = ] literal -rot                \ xt list1 list0
    list-intersection-struct            \ list-result
;

\ Return the set union of two regioncorr lists.
: regioncorr-list-set-union ( regc-lst1 regc-lst0 -- regc-lst )
    \ Check args.
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr-list? )

    [ ' = ] literal -rot                \ xt list1 list0
    list-union-struct                   \ list-result
;

: regioncorr-list-supersets-of ( regc1 regc-lst0 -- regc-lst )
    \ Check args.
    \ cr ." regioncorr-list-in: start: " .stack cr
    assert( tos is-regioncorr-list? )
    assert( nos is-regioncorr? )

    \ Init return list.
    list-new -rot                   \ ret-lst sta1 reg-lst0

    foreach                         \ ret-lst sta1 reg-lnk0 regx
        #2 pick swap                \ ret-lst sta1 reg-lnk0 sta1 regx
        regioncorr-superset?        \ ret-lst sta1 reg-lnk0 bool
        if
            dup link-get-data       \ ret-lst sta1 reg-lnk0 regx
            #3 pick                 \ ret-lst sta1 reg-lnk0 regx ret-lst
            list-push-struct        \ ret-lst sta1 reg-lnk0
        then
    next
                                    \ ret-lst sta1
    drop
;

\ Given a regioncorr-list find the best list of regioncorrints
\ that cover all regioncorrs.
\ A one element result means that anywhere is reachable from anywhere.
: regioncorr-list-map-routes ( regc-comp-lst0 -- regci-lol )
    \ Check arg.
    cr ." regioncorr-list-map-routes: start" cr
    assert( tos is-regioncorr-list? )

    dup regioncorr-list-split-by-intersections  \ regc-comp-lst1 regc-int-lst
    invert abort" split failed?"

    cr s" Intersections: " #2 pick .regioncorr-list-prefix cr

    \ Init regioncorrint list
    list-new                                    \ regc-comp-lst1 regc-int-lst regci-lst

    \ Create regioncorrints.
    over                                        \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lst
    foreach                                     \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk regc-intx
        \ cr ." int: " dup .regioncorr

        dup                                     \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk regc-intx regc-intx
        #5 pick                                 \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk regc-intx regc-intx regc-comp-lst1
        regioncorr-list-supersets-of            \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk regc-intx sups-lst

        \ space ." sups: " dup .regioncorr-list cr

        dup list-get-length                     \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk regc-intx sups-lst len
        1 >
        if
            swap regioncorrint-new-xt execute   \ regc-comp-lst1 regc-int-lst regci-lst regc-int-lnk, regci t | f
            ifnot
                cr ." regioncorrint-new failed?"
                abort
            then
            \ cr ." regioncorrint-list-map-routes: " dup .regioncorrint cr
            #2 pick list-push-end-struct
        else
            regioncorr-list-deallocate
            drop
        then
    next
                                                \ regc-comp-lst1 regc-int-lst regci-lst
    \ Display.
    s" regcorrints: " #2 pick .regioncorrint-list-prefix-xt execute

    \ Find links between any two regioncorrints.
    dup list-get-links                              \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1
    begin
        ?dup
    while
        dup link-get-next                           \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2
        begin
            ?dup
        while
            over link-get-data                      \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regci1
            over link-get-data                      \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regci1 regci2

            regioncorrints-shared-regioncorr-xt     \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regci1 regci2 xt
            execute                                 \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst'
            dup list-is-empty?                      \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' bool
            if
                list-deallocate
            else
                #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' regci1
                regioncorrint-get-length-xt execute \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' len1
                #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' len1 regci2
                regioncorrint-get-length-xt execute \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' len1 len2
                min                                 \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' min
                over list-get-length                \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' min len
                =
                if
                    \ Skip if all regioncorrs of one regioncorrint intersect.
                    regioncorr-list-deallocate      \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2
                else
                    #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' regci1
                    #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' regci1 regci2
                    cr ." regc: " .regioncorrint-xt execute
                    cr ." and   " .regioncorrint-xt execute
                    cr ." at:   " dup .regioncorr-list  \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst'
                    #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' regc1
                    regioncorrint-get-list-xt execute   \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' lst1
                    #2 pick link-get-data               \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' lst1 regc2
                    regioncorrint-get-list-xt execute   \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' lst1 lst2
                    regioncorr-list-set-union           \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst' regc-lst2'
                    cr ." agg:  " dup .regioncorr-list space dup list-get-length dec.
                    cr
                    regioncorr-list-deallocate          \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2 regc-lst'
                    regioncorr-list-deallocate          \ regc-comp-lst1 regc-int-lst regci-lst regci-lnk1 regci-lnk2
                then
            then
        next
    next

    \ Find intersections for each complement regioncorr.
\    #2 pick                                     \ regc-comp-lst1 regc-int-lst regci-lst regc-comp-lst1
\    foreach                                     \ regc-comp-lst1 regc-int-lst regci-lst regc-comp-lnk1 regc-compx
\        cr ." Checking complement: " dup .regioncorr cr
\        #2 pick                                 \ regc-comp-lst1 regc-int-lst regci-lst regc-comp-lnk1 regc-compx regci-ls
\        regioncorrint-list-regioncorr-in-xt     \ regc-comp-lst1 regc-int-lst regci-lst regc-comp-lnk1 regc-compx regci-lst xt
\        execute                                 \ regc-comp-lst1 regc-int-lst regci-lst regc-comp-lnk1 regci-lst2
\        cr s"    regci's: " #2 pick .regioncorrint-list-prefix-xt execute
\        regioncorrint-list-deallocate-xt execute
\    next

    \ Deallocate.
    regioncorrint-list-deallocate-xt execute
    regioncorr-list-deallocate
    drop
    cr ." regioncorr-list-map-routes: end" cr
;
