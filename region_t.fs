\ Test region functions.

: region-test-basic

    \ Test region-new.
    #5 #4 state-new             \ sta
    #6 #4 state-new             \ sta sta
    region-new                  \ reg

    \ Test .region works.
    cr ." region: " dup .region \ reg

    \ Test region-str produces the expected output.
    pad 1+ over                 \ reg pad+ reg
    region-str                  \ reg nc
    pad c!                      \ reg
    pad string@                 \ reg c-addr cnt
    s" r01Xx"                   \ reg c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test region-deallocate.
    region-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." region-test-basic - Ok"
;

: region-tests
    region-test-basic
;
