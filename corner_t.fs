: corner-test-new
    s" s1010->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-new - Ok"
;

: corner-test-add-square
    \ Init corner.
    s" s0101->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    s" s0110->s0110" sample-from-string-a square-new    \ crn sqr6
    2dup swap corner-add-square                         \ crn sqr6 bool
    if
    else
        true abort" add square 6 failed?"
    then
    cr ." crn: " over .corner cr

    s" s1001->s1110" sample-from-string-a square-new    \ crn sqr6 sqr9
    dup                                                 \ crn sqr6 sqr9 sqr9
    #3 pick                                             \ crn sqr6 sqr9 sqr9 crn
    corner-add-square                                   \ crn sqr6 sqr9 bool
    if
    else
        true abort" add square 9 failed?"
    then
    cr ." crn: " #2 pick .corner cr

    \ Add between square.
    s" s0111->s1111" sample-from-string-a square-new    \ crn sqr6 sqr9 sqr7
    dup                                                 \ crn sqr6 sqr9 sqr7 sqr7
    #4 pick                                             \ crn sqr6 sqr9 sqr7 sqr7 crn
    corner-add-square                                   \ crn sqr6 sqr9 sqr7 bool
    if
    else
        true abort" add square 4 failed?"
    then
    cr ." crn: " #3 pick .corner cr

    \ Deallocate.
    2drop drop  \ sqr6 already deallocated, sqr9 and sqr7 will be deallocated below.
    corner-deallocate

\   structinfo-list-store structinfo-list-print-memory-use

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-add-square - Ok"
;

: corner-tests
    corner-test-new
    corner-test-add-square
;
