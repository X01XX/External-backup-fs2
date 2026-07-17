: corner-test-new
    s" s1010->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-new - Ok"
;

\ Add a dissimilar square, displacing a non-dissimilar square.
\ An new ~a+~b intersection with possible-regions.
: corner-test-add-square1
    \ Init corner.
    s" s0101->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    s" s0110->s0110" sample-from-string-a square-new    \ crn sqr6
    2dup swap corner-add-square                         \ crn sqr6 bool
    ifnot
        true abort" add square 6 failed?"
    then
    cr ." crn: " over .corner cr

    s" s1001->s1110" sample-from-string-a square-new    \ crn sqr6 sqr9
    dup                                                 \ crn sqr6 sqr9 sqr9
    #3 pick                                             \ crn sqr6 sqr9 sqr9 crn
    corner-add-square                                   \ crn sqr6 sqr9 bool
    ifnot
        true abort" add square 9 failed?"
    then
    cr ." crn: " #2 pick .corner cr

    \ Add between square.
    s" s0111->s1111" sample-from-string-a square-new    \ crn sqr6 sqr9 sqr7
    dup                                                 \ crn sqr6 sqr9 sqr7 sqr7
    #4 pick                                             \ crn sqr6 sqr9 sqr7 sqr7 crn
    corner-add-square                                   \ crn sqr6 sqr9 sqr7 bool
    ifnot
        true abort" add square 4 failed?"
    then
    cr ." crn: " #3 pick .corner cr

    \ Clean up, sqr6 already deallocated, sqr9 and sqr7 will be deallocated below.
    2drop drop                                          \ crn

    \ Check square pairs.
    s" (r01x1 rxX01)"                                   \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-square-pairs                     \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." square-pairs not right?" abort then
    region-list-deallocate                              \ crn

    \ Check possible regions.
    s" (rX0XX r1XXX rXXX0 r0X0X rXX1X rX10X)"           \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-possible-regions                 \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." possible-regions not right?" abort then
    region-list-deallocate                              \ crn

    \ Check dissimilar-squares.
    s" (s0111 s1001)"                                   \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-dissimilar-squares                  \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." dissimilar-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Check other-squares.
    s" ()"                                              \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-other-squares                       \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." other-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Deallocate.
    corner-deallocate

\   structinfo-list-store structinfo-list-print-memory-use

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-add-square1 - Ok"
;


\ Add a dissimilar square, displacing a dissimilar square.
\ An new ~a+~b intersection with possible-regions.
: corner-test-add-square2
    \ Init corner.
    s" s0101->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    s" s0110->s1110" sample-from-string-a square-new    \ crn sqr6
    2dup swap corner-add-square                         \ crn sqr6 bool
    ifnot
        true abort" add square 6 failed?"
    then
    cr ." crn: " over .corner cr

    s" s1001->s1110" sample-from-string-a square-new    \ crn sqr6 sqr9
    dup                                                 \ crn sqr6 sqr9 sqr9
    #3 pick                                             \ crn sqr6 sqr9 sqr9 crn
    corner-add-square                                   \ crn sqr6 sqr9 bool
    ifnot
        true abort" add square 9 failed?"
    then
    cr ." crn: " #2 pick .corner cr

    \ Add between square.
    s" s0111->s1111" sample-from-string-a square-new    \ crn sqr6 sqr9 sqr7
    dup                                                 \ crn sqr6 sqr9 sqr7 sqr7
    #4 pick                                             \ crn sqr6 sqr9 sqr7 sqr7 crn
    corner-add-square                                   \ crn sqr6 sqr9 sqr7 bool
    ifnot
        true abort" add square 4 failed?"
    then
    cr ." crn: " #3 pick .corner cr

    \ Clean up, sqr6 already deallocated, sqr9 and sqr7 will be deallocated below.
    2drop drop                                          \ crn

    \ Check square pairs.
    s" (r01x1 rxX01)"                                   \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-square-pairs                     \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." square-pairs not right?" abort then
    region-list-deallocate                              \ crn

    \ Check possible regions.
    s" (rX0XX r1XXX rXXX0 r0X0X rXX1X rX10X)"           \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-possible-regions                 \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." possible-regions not right?" abort then
    region-list-deallocate                              \ crn

    \ Check dissimilar-squares.
    s" (s0111 s1001)"                                   \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-dissimilar-squares                  \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." dissimilar-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Check other-squares.
    s" ()"                                              \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-other-squares                       \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." other-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-add-square2 - Ok"
;

\ Add a similar square, displacing a similar square.
: corner-test-add-square3
    \ Init corner.
    s" s0101->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    s" s0110->s0110" sample-from-string-a square-new    \ crn sqr6
    2dup swap corner-add-square                         \ crn sqr6 bool
    ifnot
        true abort" add square 6 failed?"
    then
    cr ." crn: " over .corner cr

    s" s1001->s1110" sample-from-string-a square-new    \ crn sqr6 sqr9
    dup                                                 \ crn sqr6 sqr9 sqr9
    #3 pick                                             \ crn sqr6 sqr9 sqr9 crn
    corner-add-square                                   \ crn sqr6 sqr9 bool
    ifnot
        true abort" add square 9 failed?"
    then
    cr ." crn: " #2 pick .corner cr

    \ Add between square.
    s" s0111->s0111" sample-from-string-a square-new    \ crn sqr6 sqr9 sqr7
    dup                                                 \ crn sqr6 sqr9 sqr7 sqr7
    #4 pick                                             \ crn sqr6 sqr9 sqr7 sqr7 crn
    corner-add-square                                   \ crn sqr6 sqr9 sqr7 bool
    ifnot
        true abort" add square 4 failed?"
    then
    cr ." crn: " #3 pick .corner cr

    \ Clean up, sqr6 already deallocated, sqr9 and sqr7 will be deallocated below.
    2drop drop                                          \ crn

    \ Check square pairs.
    s" (r01x1 rxX01)"                                   \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-square-pairs                     \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." square-pairs not right?" abort then
    region-list-deallocate                              \ crn

    \ Check possible regions.
    s" (rX1XX rXX1X r0XXX rXXX0 r1XXX rX0XX)"           \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-possible-regions                 \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." possible-regions not right?" abort then
    region-list-deallocate                              \ crn

    \ Check dissimilar-squares.
    s" (s1001)"                                         \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-dissimilar-squares                  \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." dissimilar-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Check other-squares.
    s" (s0111)"                                         \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-other-squares                       \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." other-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-add-square3 - Ok"
;

\ Add a similar square, displacing a dissimilar square.
\ Whole recalc of possible-regions.
: corner-test-add-square4

    \ Init corner.
    s" s0101->s0101" sample-from-string-a square-new corner-new
    cr ." crn: " dup .corner cr

    s" s0110->s1110" sample-from-string-a square-new    \ crn sqr6
    2dup swap corner-add-square                         \ crn sqr6 bool
    ifnot
        true abort" add square 6 failed?"
    then
    cr ." crn: " over .corner cr

    s" s1001->s1110" sample-from-string-a square-new    \ crn sqr6 sqr9
    dup                                                 \ crn sqr6 sqr9 sqr9
    #3 pick                                             \ crn sqr6 sqr9 sqr9 crn
    corner-add-square                                   \ crn sqr6 sqr9 bool
    ifnot
        true abort" add square 9 failed?"
    then
    cr ." crn: " #2 pick .corner cr

    \ Add between square.
    s" s0111->s0111" sample-from-string-a square-new    \ crn sqr6 sqr9 sqr7
    dup                                                 \ crn sqr6 sqr9 sqr7 sqr7
    #4 pick                                             \ crn sqr6 sqr9 sqr7 sqr7 crn
    corner-add-square                                   \ crn sqr6 sqr9 sqr7 bool
    ifnot
        true abort" add square 4 failed?"
    then
    cr ." crn: " #3 pick .corner cr

    \ Clean up, sqr6 already deallocated, sqr9 and sqr7 will be deallocated below.
    2drop drop                                          \ crn

    \ Check square pairs.
    s" (r01x1 rxX01)"                                   \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-square-pairs                     \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." square-pairs not right?" abort then
    region-list-deallocate                              \ crn

    \ Check possible regions.
    s" (r1XXX rXX1X rX0XX rXXX0 rX1XX r0XXX)"           \ crn c-addr u
    region-list-from-string-a dup                       \ crn reg-lst' reg-lst'
    #2 pick corner-get-possible-regions                 \ crn reg-lst' reg-lst' pr-lst
    region-lists-eq?                                    \ crn reg-lst' bool
    ifnot cr ." possible-regions not right?" abort then
    region-list-deallocate                              \ crn

    \ Check dissimilar-squares.
    s" (s1001)"                                         \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-dissimilar-squares                  \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." dissimilar-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Check other-squares.
    s" (s0111)"                                         \ crn c-addr u
    state-list-from-string-a                            \ crn sta-lst'
    over corner-get-other-squares                       \ crn sta-lst' pr-lst
    square-list-states                                  \ crn sta-lst' pr-sta-lst'
    2dup state-lists-eq?                                \ crn sta-lst' pr-sta-lst' bool
    ifnot cr ." other-squares not right?" abort then
    state-list-deallocate                               \ crn sta-lst'
    state-list-deallocate                               \ crn

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-add-square4 - Ok"
;

: corner-tests
    corner-test-new
    corner-test-add-square1
    corner-test-add-square2
    corner-test-add-square3
    corner-test-add-square4
;
