
\ Get the complement of a regc, 5, find intersections, and count
\ the number of intersections of each fragment.
\ A fragment intersection of gt 2 fragments may be useful.
\
\ In this case, any start->goal,
\ where the start, and goal, are not equal, and not 5,
\ can be start->goal within a complement region,
\ or start->A->goal.
\
\ Create a lst of regionints.
: regioncorrint-list-test-list-generation
    \ Init.
    s"  (( regc 0  0 (r0101))) (( regc 1  0 (rXXXX)))" string-to-stack-a

    \ Subtract.
    2dup regioncorr-list-subtract               \ regc-lst1 regc-lst0 regc-comp-lst
    cr s" Complement: " #2 pick .regioncorr-list-prefix cr

    dup regioncorr-list-split-by-intersections  \ regc-lst1 regc-lst0 regc-comp-lst, regc-int-lst t | f
    invert abort" split failed?"
    cr s" Intersections: " #2 pick .regioncorr-list-prefix cr

    \ Init regioncorrint list
    list-new                                  \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst

    \ Create regioncorrints.
    over                                        \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lst
    foreach                                     \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regc-intx
        \ cr ." int: " dup .regioncorr
        dup                                     \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regc-intx regc-intx
        #5 pick                                 \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regc-intx regc-intx regc-comp-lst
        regioncorr-list-supersets-of            \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regc-intx sups-lst

        \ space ." sups: " dup .regioncorr-list cr
        dup list-get-length                     \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regc-intx sups-lst len
        1 >
        if
            swap regioncorrint-new              \ regc-lst1 regc-lst0 regc-comp-lst regc-int-lst regcint-lst regc-int-lnk regci
            \ cr ." regioncorrint: " dup .regioncorrint cr
            #2 pick list-push-end-struct
        else
            regioncorr-list-deallocate
            drop
        then
    next

    \ Display.
    s" regcorrints: " #2 pick .regioncorrint-list-prefix

    \ Test.

    \ Deallocate.
    regioncorrint-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate
    regioncorr-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." regioncorrint-list-test-stuff - Ok"
;

: regioncorrint-list-tests
    regioncorrint-list-test-list-generation
    cr
;
