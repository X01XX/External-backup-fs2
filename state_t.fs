\ Test state functions.

: state-test-new
    #5 #4 state-new             \ sta
    cr ." state: " dup .state   \ sta
    state-deallocate
    structinfo-list-store structinfo-list-project-deallocated
    cr ." state-test-new - Ok"
;

: state-tests
    state-test-new
;
