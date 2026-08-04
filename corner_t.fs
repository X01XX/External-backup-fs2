: corner-test-new
    s" s0101 r0XX1" string-to-stack corner-new  \ crn
    \ cr ." crn: " dup .corner cr

    \ Test.
    dup corner-get-adjacent-states              \ crn sta-lst
    list-get-length                             \ crn len
    2 <> abort" not 2 adjacent states?"         \ crn

    \ Deallocate.
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-new - Ok"
;

: corner-test-states
    s" s0101 r0XX1" string-to-stack corner-new  \ crn
    \ cr ." crn: " dup .corner cr

    dup corner-states                       \ crn sta-lst'
    \ cr ." states: " dup .state-list cr

    dup list-get-length                     \ crn sta-lst' len
    3 <> abort" len ne 3?"

    \ Deallocate.
    state-list-deallocate
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-states - Ok"
;

: corner-test-is-proper-superset?
    s" s0100 rXXX0" string-to-stack corner-new  \ crn1
    \ cr ." crn1: " dup .corner cr

    s" s0101 r0XX1" string-to-stack corner-new  \ crn1 crn2
    \ cr ." crn2: " dup .corner cr

    2dup corner-is-proper-superset?         \ crn1 crn2 bool
    ifnot
        cr ." not superset?" cr abort
    then

    swap
    2dup corner-is-proper-superset?         \ crn1 crn2 bool
    if
        cr ." superset?" cr abort
    then

    \ Deallocate.
    corner-deallocate
    corner-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." corner-test-states - Ok"
;

: corner-tests
    corner-test-new
    corner-test-states
    corner-test-is-proper-superset?
;
