\ Test region functions.

: region-test-basic

    \ Test region-new.
    s" s0101" state-from-string-a   \ sta
    s" s0110" state-from-string-a   \ sta sta
    
    region-new                      \ reg

    \ Test .region works.
    cr ." region: " dup .region     \ reg

    \ Test region-str produces the expected output.
    pad 1+ over                     \ reg pad+ reg
    region-str                      \ reg nc
    pad c!                          \ reg
    pad string@                     \ reg c-addr cnt
    s" r01Xx"                       \ reg c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test region-deallocate.
    region-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-test-basic - Ok"
;

: region-test-intersection

    \ Test intersection.
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

    \ Test non-intersection.
    s" r0" region-from-string-a         \ reg1
    s" r1" region-from-string-a         \ reg1 reg2
    2dup                                \ reg1 reg2 | reg1 reg2
    region-intersection                 \ reg1 reg2 | reg3 t | f
    if
        ." regions intersect?"
        abort
    then

    2dup swap                           \ reg1 reg2 | reg2 reg1
        region-intersection             \ reg1 reg2 | reg3 t | f
    if
        ." regions intersect?"
        abort
    then
    region-deallocate                   \ reg1
    region-deallocate                   \

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-test-intersection - Ok"
;

: region-tests
    region-test-basic
    region-test-intersection
;
