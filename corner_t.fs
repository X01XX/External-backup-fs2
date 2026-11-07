: corner-test-new
    s" s0101 r0X0X" string-to-stack-a corner-new    \ crn t | f
    invert abort" corner-new failed?"
    cr ." crn: " dup .corner cr

    \ Test.
    dup corner-get-adjacent-states              \ crn sta-lst
    list-get-length                             \ crn len
    #2 <> abort" not 2 adjacent states?"        \ crn

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-new - Ok"
;

: corner-test-states
    s" (crn s0101 r0XX1 (s1101 s0100))" corner-from-string  \ crn t | f
    invert abort" corner-new failed?"
    cr ." crn: " dup .corner cr

    dup corner-get-adjacent-states                          \ crn sta-lst'
    cr ." adj states: " dup .state-list cr

    list-get-length                                         \ crn len
    #2 <> abort" len ne 2?"

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-states - Ok"
;

: corner-test-is-proper-superset?
    s" (crn s0100 rXXX0 (s0101))" corner-from-string   \ crn1 t | f
    invert abort" corner-from-string failed?"
    \ cr ." crn1: " dup .corner cr

    s" (crn s0101 r0XX1 (s1101 s0100))" corner-from-string  \ crn1, crn2 t | f
    invert abort" corner-from-string failed?"
    \ cr ." crn2: " dup .corner cr

    \ Test.
    2dup corner-is-proper-superset?                 \ crn1 crn2 bool
    ifnot
        cr ." not superset?" cr abort
    then

    swap
    2dup corner-is-proper-superset?                 \ crn1 crn2 bool
    if
        cr ." superset?" cr abort
    then

    \ Deallocate.
    corner-deallocate
    corner-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-is-proper-superset? - Ok"
;

: corner-test-additional-corners
    \ Set up.
    s" (r00xx r010x rxx1x r1x0x)" list-from-string-a    \ reg-lst
    s" (crn s0101 r010x (s0001 s0111 s1101))" corner-from-string    \ reg-lst, crn t | f
    invert abort" corner-from-string failed?"

    \ Run.
    2dup corner-additional-corners                      \ reg-lst crn, crn-lst t | f
    invert abort" no corner list?"

    \ Display results.
    \ cr ." corner cluster: " dup .corner-list cr

    \ Test results.
    dup list-get-length                                 \ reg-lst crn len
    #4 <> abort" Corner cluster not 4 corners?"

    \ Deallocate.
    corner-list-deallocate
    drop                                                \ Corner deallocated, above.
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-additional-corners - Ok"
;

: corner-test-calc-set-rate
    \ Set up.
    s" (r00xx r010x rxx1x r1x0x)" list-from-string-a    \ reg-lst
    s" (crn s0101 r010x (s0001 s0111 s1101))" corner-from-string    \ reg-lst, crn t | f
    invert abort" corner-from-string failed?"

    \ Run.
    2dup corner-calc-set-rate                           \ reg-lst crn

    \ Display results.
    \ cr ." rate: " dup . cr

    \ Test results.
    dup corner-get-rate                                 \ reg-lst crn rt
    #3 <> abort" corner rate ne 3?"

    \ Deallocate.
    corner-deallocate
    region-list-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-calc-set-rate - Ok"
;

: corner-test-from-string
    s" (crn s0101 r0X0X (s0111 s1101))" list-from-string       \ lst t | f
    invert abort" corner-test-from-string: list-from-string: string not parsed"

    \ Test.
    dup list-get-first-item is-corner? invert abort" list-from-string failed?"
    dup list-get-length 1 <> abort" list len not one?"

    struct-list-deallocate

    s" (crn s0101 r0X0X (s0111 s1101))" string-to-stack        \ x* t | f
    invert abort" corner-test-new: string-to-stack: string not parsed"

    \ Test (uses list-from-string).
    dup is-corner? invert abort" string-to-stack failed?"

    corner-deallocate

    s" (crn s0101 r0X0X (s0111 s1101))" corner-from-string        \ regc t | f
    invert abort" corner-test-new: corner-from-string: corner not parsed"

    \ Test (uses list-from-string).
    dup is-corner? invert abort" corner-from-string failed?"

    corner-deallocate

    \ Check for memory leaks.
    check-project-deallocated

    cr ." corner-test-from-string - Ok"
;

: corner-tests
    corner-test-new
    corner-test-states
    corner-test-is-proper-superset?
    corner-test-additional-corners
    corner-test-calc-set-rate
    corner-test-from-string
;
