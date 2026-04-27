\ Test region functions.

: region-test-new
    #5 #4 state-new             \ sta
    #6 #4 state-new             \ sta sta
    region-new                  \ reg
    cr ." region: " dup .region \ reg
    region-deallocate
    structinfo-list-store structinfo-list-project-deallocated
    cr ." region-test-new - Ok"
;

: region-tests
    region-test-new
;
