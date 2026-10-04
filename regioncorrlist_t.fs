\ regioncorr-list tests.

: regioncorr-list-test-split-by-intersections
    s" (regc 1 -1 (r0X0X r00x0x)) (regc #2 #-2 (rx1x1 r0x1x1))" list-from-string-a

    dup regioncorr-list-split-by-intersections             \ regc-lst0, regc-lst t | f
    invert abort" split failed?"

    cr ." intersections: " dup .regioncorr-list cr

    \ Test.
    s" ( regc 3  -3 (r0101 r00101)) ( regc 1  -1 (r0X0X r00x00)) ( regc 1  -1 (r0X0X r0000x)) ( regc 1  -1 (r0X00 r00x0x)) ( regc 1  -1 (r000X r00x0x)) ( regc 2  -2 (rx1x1 r0x111)) ( regc 2  -2 (rx1x1 r011x1)) ( regc 2  -2 (rx111 r0x1x1)) ( regc 2  -2 (r11x1 r0x1x1))"
    list-from-string-a
    2dup regioncorr-lists-eq?
    invert abort" unexpected results?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections - Ok"
;

: regioncorr-list-test-split-by-intersections2
    s" (regc 1 -1 (rxxxx)) (regc #2 -#2 (rxxx1)) (regc #3 #-3 (rx1x1)) (regc #4 #-4 (rx1x1))" list-from-string-a

    s" start: " #2 pick .regioncorr-list-prefix cr

    dup regioncorr-list-split-by-intersections             \ regc-lst0, regc-lst t | f
    invert abort" split failed?"

    cr s" intersections: " #2 pick .regioncorr-list-prefix cr

    \ Test.
    s" ( regc #10  #-10 (rX1X1)) ( regc #3  #-3 (rX0X1)) ( regc 1  -1 (rxxx0))"
    list-from-string-a
    2dup regioncorr-lists-eq?
    invert abort" unexpected results?"

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-split-by-intersections2 - Ok"
;

\ Get the complement of a regc, 5, find intersections, and count
\ the number of intersections of each fragment.
\ A fragment intersection of gt 2 fragments may be useful.
\
\ In the complement 5 case, any start->goal,
\ where the start, and goal, are not equal, and not 5,
\ can be start->goal within a complement region,
\ or start->A->goal.
\
\ Create a lst of regionints.
: regioncorr-list-test-map-routes
    \ Calc complement list.
    s" (( regc 0  0 (r0101)) ( regc 0 0 (r1111)))" list-from-string-a

    dup regioncorr-list-complement
    cr s" Complements: " #2 pick .regioncorr-list-prefix

    dup regioncorr-list-map-routes  \ avoid-lst comp-lst ?

    \ Display.
\    s" regcorrints: " #2 pick .regioncorrint-list-prefix

    \ Test.

    \ Deallocate.
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorr-list-test-map-routes - Ok"
;

: regioncorr-list-test-path
    s" ( regc 0  0 (r0101)) ( regc 0  0 (r1111))" list-from-string-a    \ regl-avd'
    s" ( regc 0  0 (rxxxx))" list-from-string-a                         \ regl-avd' regl-max'
    2dup regioncorr-list-subtract                                       \ regl-avd' regl-max' regl-trv'
    swap regioncorr-list-deallocate                                     \ regl-avd' regl-trv'
    swap regioncorr-list-deallocate                                     \ regl-trv'

    cr ." result: " dup .regioncorr-list space ." ints: " cr

    dup
    foreach                                 \ regl-trv' reg-trv-lnk3 reg-trv
        cr dup .regioncorr
        #2 pick                             \ regl-trv' reg-lnk3 regx regl-trv'
        foreach                             \ regl-trv' reg-lnk3 regx reg-lnk3 regy
            #2 pick                         \ regl-trv' reg-lnk3 regx reg-lnk3 regy regx
            =                               \ regl-trv' reg-lnk3 regx reg-lnk3 bool
            ifnot
                dup link-get-data           \ regl-trv' reg-lnk3 regx reg-lnk3 regy
                #2 pick                     \ regl-trv' reg-lnk3 regx reg-lnk3 regy regx
                regioncorr-intersection     \ regl-trv' reg-lnk3 regx reg-lnk3, int t | f
                if
                    space dup .regioncorr
                    regioncorr-deallocate   \ regl-trv' reg-lnk3 regx reg-lnk3
                then
            then
        next-item
        drop
    next-item
    cr
    \ Make sta-from.
                                                \ regl-trv'
    s" (stac (s0111))" statecorr-from-string-a  \ regl-trv' sta-f'

    \ Get regioncorrs sta-from is in.
    2dup swap                                   \ regl-trv' sta-f' sta-f' regl-trv'
    regioncorr-list-supersets-of-statecorr      \ regl-trv' sta-f' regl-sup'

    cr ." regioncorrs s0111 is in: " dup .regioncorr-list cr

    \ TODO is sta-t in any regl-sup regioncorr?

    \ Subtract 1-in regcs from traverse regcs.
    [ ' = ] literal                             \ regl-trv' sta-f' regl-sup' xt
    #3 pick                                     \ regl-trv' sta-f' regl-sup' xt regl-trv'
    #2 pick                                     \ regl-trv' sta-f' regl-sup' xt regl-trv' regl-sup'
    list-difference-struct                      \ regl-trv' sta-f' regl-sup' regl-dif'

    cr ." difference: " dup .regioncorr-list cr

    \ Get regcs in regl-dif that intersect with regl-sup.
    dup                                         \ regl-trv' sta-f' regl-sup' regl-dif' regl-dif'
    #2 pick                                     \ regl-trv' sta-f' regl-sup' regl-dif' regl-dif' regl-sup'
    regioncorr-list-intersections               \ regl-trv' sta-f' regl-sup' regl-dif' regl-int'

    cr ." intersectors: " dup .regioncorr-list cr

    \ Make sta-to.
    s" (stac (s0111))" statecorr-from-string-a  \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t'

    \ Find min distance between regioncorrs in regl-int and stac-t.
    \ Minimum distance may be zero.
    #99999 #2 pick                              \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min regl-int'

    foreach                                     \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-lnk reg-intx
        #3 pick swap                            \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-lnk stac-t' regc-intx
        regioncorr-dist-statecorr               \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-lnk dist
        rot min                                 \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' int-lnk min
        swap                                    \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-lnk
    next-item
                                                \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min
    cr ." min dist: " dup dec. cr

    \ Find regioncorrs that are the min distance from.
    list-new                                    \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min'
    #3 pick                                     \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' regl-int'

    foreach                                     \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk reg-intx
        #4 pick swap                            \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk stac-t' regc-intx
        regioncorr-dist-statecorr               \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk dist
        #3 pick                                 \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk dist min
        =                                       \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk bool
        if
            dup link-get-data                   \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk reg-intx
            #2 pick                             \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk reg-intx int-min'
            list-push-struct                    \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' min int-min' int-lnk
        then
    next-item
    nip                                         \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' int-min'

    cr ." min dist regcs: " dup .regioncorr-list cr

    \ Choose one randomly.
    dup list-get-length random                  \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' int-min' inx
    over list-get-item                          \ regl-trv' sta-f' regl-sup' regl-dif' regl-int' stac-t' int-min' regcx
    cr ." chosen min dist regc: " dup .regioncorr cr

    \ Traverse sta-f from a regl-sup regioncorr to the chosen minimum distance intersecting regioncorr.

    drop

    \ Deallocate.
    regioncorr-list-deallocate
    statecorr-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    statecorr-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated
;

: regioncorr-list-tests
    regioncorr-list-test-split-by-intersections
    regioncorr-list-test-split-by-intersections2
    \ regioncorr-list-test-map-routes
    cr
;
