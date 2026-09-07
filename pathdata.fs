#61379 constant pathdata-struct-id
    #4 constant pathdata-struct-number-cells

\ Struct fields
0                                       constant pathdata-header-disp           \ 16-bits [0] struct id [1] use count [2] Least negative value (ABS) expected ( 8 bits ).
pathdata-header-disp            cell+   constant pathdata-regioncorr-list-disp  \ Max X region minus zero, or more, regioncorrs with a negative value.
pathdata-regioncorr-list-disp   cell+   constant pathdata-intregcs-list-disp    \ A list of intregcs calculated from the regioncorr-list.
pathdata-intregcs-list-disp     cell+   constant pathdata-regcints-list-disp    \ A list of regcints calculated from the regioncorr-list.

0 value pathdata-mma \ Storage for pathdata mma instance.

\ Init pathdata mma, return the addr of allocated memory.
: pathdata-mma-init ( num-items -- ) \ sets pathdata-mma.
    dup 1 <
    abort" pathdata-mma-init: Invalid number of items."

    cr ." Initializing PathData store."
    pathdata-struct-number-cells swap mma-new to pathdata-mma
;

\ Check if tos is an allocated pathdata.
: is-pathdata? ( tos -- flag )
    dup pathdata-mma mma-is-item?   \ tos bool
    if
        struct-get-id
        pathdata-struct-id =        \ bool
    else
        drop
        false                       \ f
    then
;

\ Start accessors.

\ Get the negative value.
: pathdata-get-neg-value ( regc0 -- val )
    \ Check args.
    assert( tos is-pathdata? )

    4c@                 \ val
    -1 *                \ -val
;

\ Set the negative value.
: _pathdata-set-neg-value ( val regc0 -- )
    \ Check args.
    assert( tos is-pathdata? )
    assert( nos 0 <= )
    assert( nos #-256 > )

    swap abs swap       \ val regc0
    4c!
;

\ Return the regioncorr-list field from a pathdata instance.
: pathdata-get-regioncorr-list ( pd0 -- regc-lst )
    \ Check arg.
    assert( tos is-pathdata? )

    pathdata-regioncorr-list-disp + \ Add offset.
    @                               \ Fetch the field.
;

\ Set the regioncorr-list field from a pathdata instance, use only in this file.
: _pathdata-set-regioncorr-list ( regc-lst1 pd0 -- )
    \ Check arg.
    assert( tos is-pathdata? )
    assert( nos is-regioncorr-list? )

    pathdata-regioncorr-list-disp + \ Add offset.
    !struct                         \ Set the field.
;

\ Return the intregcs-list field from a pathdata instance.
: pathdata-get-intregcs-list ( pd0 -- regc-lst )
    \ Check arg.
    assert( tos is-pathdata? )

    pathdata-intregcs-list-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the intregcs-list field from a pathdata instance, use only in this file.
: _pathdata-set-intregcs-list ( regc-lst1 pd0 -- )
    \ Check arg.
    assert( tos is-pathdata? )
    assert( nos is-intregcs-list? )

    pathdata-intregcs-list-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the regcints-list field from a pathdata instance.
: pathdata-get-regcints-list ( pd0 -- regc-lst )
    \ Check arg.
    assert( tos is-pathdata? )

    pathdata-regcints-list-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the regcints-list field from a pathdata instance, use only in this file.
: _pathdata-set-regcints-list ( regc-lst1 pd0 -- )
    \ Check arg.
    assert( tos is-pathdata? )
    assert( nos is-regcints-list? )

    pathdata-regcints-list-disp +   \ Add offset.
    !struct                         \ Set the field.
;


\ End accessors.

\ Create a pathdata from a value and regioncorr list.
\ The states may be the same.
: pathdata-new ( regc-lst val -- pd )
    \ Check args.
    assert( tos 0 <= )
    assert( tos #-256 > )
    assert( nos is-regioncorr-list? )
    \ cr ." pathdata-new: start " .stack cr

    \ Allocate space.
    pathdata-struct-id pathdata-mma \ regc-lst val id mma
    struct-allocate                 \ regc-lst val pd

    \ Split regioncorrs by intersections.
    \ All fragments will be subset the original regioncorrs.
    #2 pick                                 \ regc-lst val pd regc-lst
    regioncorr-list-split-by-intersections  \ regc-lst val pd, spl-lst'
    invert abort" split failed?"

    \ Remove regc value 1 fragments, which do not intersect two, or more, regioncorrs.
    dup
    regioncorr-list-regioncorrs-gt-pos-1    \ regc-lst val pd spl-lst' spl-lst2'

    \ Clean up.
    swap regioncorr-list-deallocate         \ regc-lst val pd spl-lst2'

    \ Generate intregcs list.
    #3 pick over                            \ regc-lst val pd spl-lst2' regc-lst spl-lst2'
    intregcs-list-generate                  \ regc-lst val pd spl-lst2' intregcs-lst

    \ Store intregcs-list.
    #2 pick _pathdata-set-intregcs-list     \ regc-lst val pd spl-lst2'

    \ Generate regcints list.
    #3 pick over                            \ regc-lst val pd spl-lst2' regc-lst spl-lst2'
    regcints-list-generate                  \ regc-lst val pd spl-lst2' regcints-lst

    \ Store regcints-list.
    #2 pick _pathdata-set-regcints-list     \ regc-lst val pd spl-lst2'

    \ Clean up.
    regioncorr-list-deallocate              \ regc-lst val pd

    \ Store value.
    tuck _pathdata-set-neg-value            \ regc-lst pd

    \ Store regioncorr list.
    tuck _pathdata-set-regioncorr-list      \ pd

    \ cr ." pathdata-new: end " .stack cr
;

\ Print a pathdata instance.
: .pathdata ( pd0 -- )
    \ Check arg.
    assert( tos is-pathdata? )
    \ cr ." .pathdata: start " .stack cr

    ." ( pathdata value: "
    dup pathdata-get-neg-value dec.

    space cr s"     regioncorrs: " #2 pick pathdata-get-regioncorr-list .regioncorr-list-prefix
    space cr s"     intregcs:    " #2 pick pathdata-get-intregcs-list .intregcs-list-prefix
    space cr s"     regcints:    " #2 pick pathdata-get-regcints-list .regcints-list-prefix
    space ." )"
    drop
    \ cr ." .pathdata: end " .stack cr
;

\ Deallocate a pathdata.
: pathdata-deallocate ( pd0 -- )
    \ Check arg.
    assert( tos is-pathdata? )
    \ cr ." .pathdata-deallocate: start " .stack cr

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" pathdata-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup pathdata-get-regioncorr-list regioncorr-list-deallocate
        dup pathdata-get-intregcs-list intregcs-list-deallocate
        dup pathdata-get-regcints-list regcints-list-deallocate

        \ Deallocate instance.
        pathdata-mma mma-deallocate
    else
        struct-dec-use-count
    then
    \ cr ." .pathdata-deallocate: end " .stack cr
;

\ Return a pathstep list to get from a regioncorr to another, within
\ a set of intersecting regioncorrs.
: pathdata-find-path2 ( to2 from1 pd0 -- pthstp-lst t | f )
    \ Check arg.
    assert( tos is-pathdata? )
    assert( nos is-regioncorr? )
    assert( 3os is-regioncorr? )
    \ cr ." pathdata-find-path2: from " over .regioncorr space ." to " #2 pick .regioncorr cr

    \ Get regcints that intersect the frm1 regioncorr.
    over                                \ to2 frm1 pd0 frm1
    over pathdata-get-regcints-list     \ to2 frm1 pd0 frm1 regcis-lst
    regcints-list-intersections         \ to2 frm1 pd0 regcis-frm-ints'

    \ Get regcints that intersect the to2 regioncorr.
    #3 pick                             \ to2 frm1 pd0 regcis-frm-ints' to2
    #2 pick pathdata-get-regcints-list  \ to2 frm1 pd0 regcis-frm-ints' to2 regcis-lst
    regcints-list-intersections         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints'

    \ Get regcints that intersect both regioncorrs.
    2dup regcints-list-set-intersection \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both'

    \ Check for the easiest PathStep, possibly reached by recursion.
    dup list-is-not-empty?              \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' bool
    if
        \ Make pathstep.

        \ Get an intersecting regcint.
        dup list-get-length             \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' len
        random                          \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' inx
        over list-get-item              \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regcis

        \ Build PathStep-new arguments.
        regcints-get-regioncorr         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regc
        #6 pick                         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regc to2
        over regioncorr-intersection    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regc, to2' t | f
        invert abort" intersection failed?"

        #6 pick                         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regc to2' frm1'
        #2 pick regioncorr-intersection \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' regc to2', frm1' t | f
        invert abort" intersection failed?"

        \ Create new PathStep.
        pathstep-new                    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' pthstp

        \ Store PathStep in list.
        list-new tuck                   \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' pthstp-lst pthstp pthstp-lst
        list-push-struct                \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' regcis-both' pthstp-lst

        \ Clean up.
        swap regcints-list-deallocate   \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' pthstp-lst
        swap regcints-list-deallocate   \ to2 frm1 pd0 regcis-frm-ints' pthstp-lst
        swap regcints-list-deallocate   \ to2 frm1 pd0 pthstp-lst
        nip nip nip                     \ pthstp-lst

        \ Return.
        true
        \ cr ." pathdata-find-path2: exit 1" cr
        exit
    else
        list-deallocate                 \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints'
    then

    \ Check for the second easiest PathStep, possibly reached by recursion.
                                            \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints'

    over regcints-list-union-intersections  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints'
    #4 pick                                 \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints' frm1
    over                                    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints' frm1 frm-ints'
    regioncorr-list-non-intersections       \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints' frm-ints2'
    swap regioncorr-list-deallocate         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2'

    over regcints-list-union-intersections  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints'
    #6 pick                                 \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints' to2
    over                                    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints' to2 to-ints'
    regioncorr-list-non-intersections       \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints' to-ints2'
    swap regioncorr-list-deallocate         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2'

    2dup regioncorr-list-set-intersection   \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints'
    \ cr ." shared intersections: " dup .regioncorr-list cr
    dup list-is-not-empty?
    if
        dup list-get-length                 \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints' len
        random                              \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints' inx
        over list-get-item                  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints' intx
        \ cr ." int chosen: " dup .regioncorr cr

        #6 pick pathdata-get-intregcs-list  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints' intx iregcs-lst
        intregcs-list-find                  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' both-ints', iregcs t | f
        invert abort" intregcs-find failed?"

        \ cr ." found: " dup .intregcs cr
        swap regioncorr-list-deallocate     \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' to-ints2' iregcs
        swap regioncorr-list-deallocate     \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints2' iregcs
        swap regioncorr-list-deallocate     \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs

        \ Init PathStep list.
        list-new                            \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst

        \ Get first path step, from1 to intersection.

        \ Get regioncorr in intregcs that intersects frm1.
        #5 pick                             \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst frm1
        #2 pick intregcs-get-regioncorrs    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst frm1 regc-lst
        regioncorr-list-first-intersection  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst, regc-int t | f
        invert abort" intersection not found?"

        \ Get frm1 intersection with chosen intregcs regioncorr.
        #2 pick intregcs-get-intersection   \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int iregcs-int
        #7 pick                             \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int iregcs-int frm1
        #2 pick regioncorr-intersection     \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int iregcs-int, frm1' t | f
        invert abort" intersection failed?"

        \ Make and save pathstep.
        pathstep-new                        \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst pthstp
        \ cr ." pathstep: frm1 to int: " dup .pathstep cr
        over list-push-struct               \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst

        \ Get second path step, intersection to to2.

        \ Get regioncorr in intregcs that intersects to2.
        #6 pick                             \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst to2
        #2 pick intregcs-get-regioncorrs    \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst to2 regc-lst
        regioncorr-list-first-intersection  \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst, regc-int t | f
        invert abort" intersection not found?"

        \ Get to2 intersection with chosen intregcs regioncorr.
        #7 pick                             \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int to2
        over regioncorr-intersection        \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int to2' t | f
        invert abort" intersection failed?"
        #3 pick intregcs-get-intersection   \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst regc-int to2' iregcs-int

        \ Make and save pathstep.
        pathstep-new                        \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst pthstp
        \ cr ." pathstep: int to to2: " dup .pathstep cr
        over pathstep-list-push-end         \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' iregcs pthstp-lst

        \ cr ." pathStep list: " dup .pathstep-list cr
        \ Cleanup.
        nip                                 \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' pthstp-lst
        swap regcints-list-deallocate       \ to2 frm1 pd0 regcis-frm-ints' pthstp-lst
        swap regcints-list-deallocate       \ to2 frm1 pd0 pthstp-lst
        nip nip nip                         \ pthstp-lst

        \ Return.
        true
        \ cr ." pathdata-find-path2: exit 2" cr
        exit
    else
        list-deallocate                     \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints' to-ints'
    then

    \ Find path between closest pairs between regcis-frm-ints' and regcis-to-ints', using recursion.
                                            \ to2 frm1 pd0 regcis-frm-ints' regcis-to-ints' frm-ints' to-ints'
    rot regcints-list-deallocate            \ to2 frm1 pd0 regcis-frm-ints' frm-ints' to-ints'
    rot regcints-list-deallocate            \ to2 frm1 pd0 frm-ints' to-ints'

    \ Convert regcis-to-ints to intregcs-to-list.
    list-new                                \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst'
    over                                    \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints'
    foreach                                 \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints-lnk intx
        #5 pick pathdata-get-intregcs-list  \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints-lnk intx iregcs-lst
        intregcs-list-find                  \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints-lnk, iregcsx t | f
        invert abort" intregcs not found?"
        #2 pick                             \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints-lnk iregcsx intregcs-to-lst'
        list-push-struct                    \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst' to-ints-lnk
    next
                                            \ to2 frm1 pd0 frm-ints' to-ints' intregcs-to-lst'
    swap regioncorr-list-deallocate         \ to2 frm1 pd0 frm-ints' intregcs-to-lst'

    \ cr s" intregs-to-lst: " #2 pick .intregcs-list-prefix cr

    \ Convert regcis-frm-ints to intregcs-frm-list.
    swap                                    \ to2 frm1 pd0 intregcs-to-lst' frm-ints'
    list-new                                \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst'
    over                                    \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints'
    foreach                                 \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints-lnk intx
        #5 pick pathdata-get-intregcs-list  \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints-lnk intx iregcs-lst
        intregcs-list-find                  \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints-lnk, iregcsx t | f
        invert abort" intregcs not found?"
        #2 pick                             \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints-lnk iregcsx iregcs-frm-lst'
        list-push-struct                    \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst' frm-ints-lnk
    next
                                            \ to2 frm1 pd0 intregcs-to-lst' frm-ints' iregcs-frm-lst'
    swap regioncorr-list-deallocate         \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst'

    \ cr s" intregs-frm-lst: " #2 pick .intregcs-list-prefix cr

    \ Compare each possible from/to pair to get the minimum distance.
    over                                    \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lst'

    \ Init min count.
    max-num >r
    foreach                                 \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk iregcs-t
        #2 pick                             \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk iregcs-t iregcs-frm-lst'
        foreach                             \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk iregcs-f
            #2 pick                         \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk iregcs-f iregcs-t
            \ cr ." comparing " over .intregcs space ." and " dup .intregcs
            intregcs-min-distance           \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk dist
            \ space ." min dist " dup dec. cr
            \ Update min.
            r> min >r
        next
        drop                                \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' intregcs-to-lnk
    next
                                            \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst'
    \ Get min distance.
    r> \ cr ." min dist: " dup dec. cr

    \ Get list of min dist intregc pairs.
    list-new                                \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst'
    #3 pick                                 \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lst'
    foreach                                 \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t
        #4 pick                             \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lst'
        foreach                             \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk iregcs-f
            #2 pick                         \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk iregcs-f iregcs-t
            intregcs-min-distance           \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk dist
            #5 pick =                       \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk bool
            if
                \ Make and store intregs pair ( iregcs-from iregn-to ).
                list-new                    \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair
                #2 pick over                \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair iregcs-t pair
                list-push-struct            \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair
                over link-get-data over     \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair iregcs-f pair
                list-push-struct            \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair
                #4 pick                     \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk pair min-iregcs-lst'
                list-push-struct            \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk iregcs-t iregcs-frm-lnk
            then
        next
        drop                                \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst' intregcs-to-lnk
    next
                                            \ ... intregcs-to-lst' iregcs-frm-lst' min min-iregcs-lst'
    nip                                     \ ... intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst'
    \ cr ." list len = " dup list-get-length dec. cr

    \ Pick a pair.
    dup list-get-length                     \ ... intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' les
    random                                  \ ... intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' inx
    over list-get-item                      \ ... intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs
    \ cr s" iregcs chosen: " #2 pick .intregcs-list-prefix cr

    \ Recurse from intersection in irecs-f -> intersection of irecs-t.
                                            \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs
    dup list-get-second-item                \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs to-intregs
    intregcs-get-intersection               \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs to
    over list-get-first-item                \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs to frm-inregs
    intregcs-get-intersection               \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs to frm
    #6 pick                                 \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs to frm pd0
    recurse                                 \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs, pthstp-lst t | f
    ifnot
        \ cr ." recursion path not found" cr
        drop                                \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst'
        struct-list-deallocate              \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst'
        intregcs-list-deallocate            \ to2 frm1 pd0 intregcs-to-lst'
        intregcs-list-deallocate            \ to2 frm1 pd0
        nip nip nip
        false
        exit
    then

    \ Final path is: frm1 -> intersection in irecs-f ( within some regc in irecs-f ) -> recursion path -> intersection of irecs-t ( within some regc in irecs-t ) -> to2
                                            \ ... min-iregcs-lst' min-iregcs pthstp-lst
    #6 pick                                 \ ... min-iregcs-lst' min-iregcs pthstp-lst frm
    #2 pick list-get-first-item             \ ... min-iregcs-lst' min-iregcs pthstp-lst frm iregcs-frm
    intregcs-pathstep-from                  \ ... min-iregcs-lst' min-iregcs pthstp-lst pthstp-frm
    over pathstep-list-push                 \ ... min-iregcs-lst' min-iregcs pthstp-lst
    \ cr s" from + recursion path: " #2 pick .pathstep-list-prefix cr

    #7 pick                                 \ ... min-iregcs-lst' min-iregcs pthstp-lst to
    #2 pick list-get-second-item            \ ... min-iregcs-lst' min-iregcs pthstp-lst to iregcs-to
    intregcs-pathstep-to                    \ ... min-iregcs-lst' min-iregcs pthstp-lst pthstp-to
    over pathstep-list-push-end             \ ... min-iregcs-lst' min-iregcs pthstp-lst
    \ cr s" from + recursion path + to: " #2 pick .pathstep-list-prefix cr

    \ Clean up.
                                            \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' min-iregcs pthstp
    nip                                     \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' min-iregcs-lst' pthstp
    swap struct-list-deallocate             \ to2 frm1 pd0 intregcs-to-lst' iregcs-frm-lst' pthstp
    swap intregcs-list-deallocate           \ to2 frm1 pd0 intregcs-to-lst' pthstp
    swap intregcs-list-deallocate           \ to2 frm1 pd0 pthstp
    nip nip nip                             \ pthstp

    \ Return.
    true
    \ cr ." pathdata-find-path2: exit 3" cr
;

\ Return a pathstep list to get from a regioncorr to another, within
\ a set of intersecting regioncorrs.
\ Some random choices can be made, so running more than once is a good idea.
: pathdata-find-path ( to2 from1 pd0 -- pthstp-lst t | f )
    \ Check arg.
    assert( tos is-pathdata? )
    assert( nos is-regioncorr? )
    assert( 3os is-regioncorr? )
    \ cr ." pathdata-find-path: from " over .regioncorr space ." to " #2 pick .regioncorr cr

    -rot                                \ pd0 to2 from1
    #2 pick                             \ pd0 to2 from1 pdo
    pathdata-find-path2                 \ pd0, pthstp-lst t | f
    if
        swap                            \ pthstp-lst pd0
        pathdata-get-regioncorr-list    \ pthstp-lst regcs-lst
        over                            \ pthstp-lst regcs-lst pthstp-lst
        pathstep-list-rate              \ pthstp-lst
        true
    else
        drop
        false
    then
;
