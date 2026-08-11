\ Test region functions.

: region-test-basic

    \ Test region-new.
    s" s0101 s0110" string-to-stack region-new  \ reg

    \ Test .region works.
    cr ." region: " dup .region     \ reg

    \ Test result.
    dup region-get-state-0          \ reg sta0
    state-get-number #6 =
    false? abort" result not as expected"

    dup region-get-state-1          \ reg sta1
    state-get-number #5 =
    false? abort" result not as expected"

    \ Test region-deallocate.
    region-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." region-test-basic - Ok"
;

: region-test-intersection

    \ Test all possible valid intersections, and the reverse order.
    s" r001101X_001101X" region-from-string-a   \ reg1
    s" r0011XXX_0X1X01X" region-from-string-a   \ reg1 reg2
    s" r0X1X01X_0011XXX" region-from-string-a   \ reg1 reg2 reg3
    2dup                                        \ reg1 reg2 reg3 | reg2 reg3
    region-intersection                         \ reg1 reg2 reg3 | reg4 t | f
    if
        dup                                     \ reg1 reg2 reg3 | reg4 reg4
        #4 pick                                 \ reg1 reg2 reg3 | reg4 reg4 reg1
        regions-eq?                             \ reg1 reg2 reg3 | reg4 bool
        if
            region-deallocate                   \ reg1 reg2 reg3
            region-deallocate                   \ reg1 reg2
            region-deallocate                   \ reg1
            region-deallocate                   \
        else
            ." region ne?"
            abort
        then
    else
        ." region-interseciton failed?"
        abort
    then

    \ Test non-intersections, and the reverse order.
    s" r0" region-from-string-a         \ reg1
    s" r1" region-from-string-a         \ reg1 reg2
    2dup                                \ reg1 reg2 | reg1 reg2
    region-intersection                 \ reg1 reg2 | reg3 t | f
    if
        ." regions intersect?"
        abort
    then

    2dup swap region-intersection       \ reg1 reg2 | reg3 t | f
    if
        ." regions intersect?"
        abort
    then
    region-deallocate                   \ reg1
    region-deallocate                   \

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." region-test-intersection - Ok"
;

: region-tests
    region-test-basic
    region-test-intersection
;
