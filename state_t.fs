\ Test state functions.

: state-test-basic
    \ Test state-new.
    #5 #4 state-new              \ sta

    \ Test .state works.
    cr ." state: " dup .state     \ sta

    \ Test state-str produces the expected output.
    pad 1+ over                 \ sta pad+ sta
    state-str                   \ sta nc
    pad c!                      \ sta
    pad string@                 \ sta c-addr cnt
    s" s0101"                    \ sta c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test state-deallocate.
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-basic - Ok"
;

: state-test-eq
    #5 #6 state-new              \ sta5
    #4 #6 state-new              \ sta5 sta4
    #5 #6 state-new              \ sta5 sta4 sta5b

    #2 pick over state-eq        \ sta5 sta4 sta5b bool
    false? abort" states not eq?"

    2dup state-eq                \ sta5 sta4 sta5b bool
    abort" states  eq?"

    \ Clean up.
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-eq - Ok"
;

: state-test-bit
    #5 #4 state-new              \ sta5
    #3 over state-bit 0<> abort" Isvalid bit value?"
    #2 over state-bit 1 <> abort" Isvalid bit value?"
    1 over state-bit 0<> abort" Isvalid bit value?"
    0 over state-bit 1 <> abort" Isvalid bit value?"

    \ Clean up.
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-bit - Ok"
;

: state-test-and
    #5 #4 state-new         \ sta5
    #6 #4 state-new         \ sta5 sta6
    2dup state-and          \ sta5 sta6 msk56

    #4 #4 mask-new          \ sta5 sta6 msk56 msk4
    2dup mask-eq            \ sta5 sta6 msk56 msk4 bool
    false? abort" state and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-and - Ok"
;

: state-test-and-mask
    #6 #4 mask-new          \ msk6
    #5 #4 state-new         \ msk6 sta5
    2dup state-and-mask     \ msk6 sta5 msk56

    #4 #4 mask-new          \ msk6 sta5 msk56 msk4
    2dup mask-eq            \ msk6 sta5 msk56 msk4 bool
    false? abort" state and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate
    mask-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-and-mask - Ok"
;

: state-test-invert
    #5 #4 state-new          \ sta5
    dup state-invert         \ sta5 msk~5
    #10 #4 mask-new          \ sta5 msk~5 msk10

    2dup state-eq            \ sta5 msk~5 msk10 bool
    false? abort" state ne 10?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-invert - Ok"
;

: state-test-same-num-bits?
    #5 #4 state-new              \ sta54
    #4 #4 state-new              \ sta54 sta44
    #5 #3 state-new              \ sta54 sta44 sta53

    2dup state-same-num-bits?    \ sta54 sta44 sta53 bool
    abort" states have same num bits?"

    #2 pick #2 pick             \ sta54 sta44 sta53 sta54 sta44
    state-same-num-bits?        \ sta54 sta44 sta53 bool
    false? abort" states don't have the same num bits?"

    \ Clean up.
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-test-same-num-bits - Ok"
;

: state-tests
    state-test-basic
    state-test-eq
    state-test-bit
    state-test-and
    state-test-and-mask
    state-test-invert
    state-test-same-num-bits?
;
