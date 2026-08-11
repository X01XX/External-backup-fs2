\ Test state functions.

: state-test-basic
    \ Test state-new.
    s" s0101" state-from-string-a   \ sta

    \ Test .state works.
    cr ." state: " dup .state       \ sta

    \ Test result.
    dup state-get-number #5 =
    false? abort" result not as expected"

    \ Test state-deallocate.
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-basic - Ok"
;

: states-test-eq?
    s" s000101" state-from-string-a \ sta5
    s" s000100" state-from-string-a \ sta5 sta4
    s" s000101" state-from-string-a \ sta5 sta4 sta5b

    #2 pick over states-eq?         \ sta5 sta4 sta5b bool
    false? abort" states not eq?"

    2dup states-eq?                 \ sta5 sta4 sta5b bool
    abort" states  eq?"

    \ Clean up.
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." states-test-eq? - Ok"
;

: state-test-bit
    s" s0101" state-from-string-a   \ sta5
    #3 over state-bit 0<> abort" Isvalid bit value?"
    #2 over state-bit 1 <> abort" Isvalid bit value?"
    1 over state-bit 0<> abort" Isvalid bit value?"
    0 over state-bit 1 <> abort" Isvalid bit value?"

    \ Clean up.
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-bit - Ok"
;

: state-test-and
    s" s0101" state-from-string-a   \ sta5
    s" s0110" state-from-string-a   \ sta5 sta6
    2dup                            \ sta5 sta6 sta5 sta6
    state-and-state-to-mask         \ sta5 sta6 msk56

    s" m0100" mask-from-string-a    \ sta5 sta6 msk56 msk4
    2dup masks-eq?                  \ sta5 sta6 msk56 msk4 bool
    false? abort" state and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-and - Ok"
;

: state-test-and-mask
    s" m0110" mask-from-string-a    \ msk6
    s" s0101" state-from-string-a   \ msk6 sta5
    2dup                            \ msk6 sta5 msk6 sta5
    state-and-mask-to-mask          \ msk6 sta5 msk56

    s" m0100" mask-from-string-a    \ msk6 sta5 msk56 msk4
    2dup masks-eq?                  \ msk6 sta5 msk56 msk4 bool
    false? abort" state and ne 4?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate
    mask-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-and-mask - Ok"
;

: state-test-invert-to-mask
    s" s0101" state-from-string-a   \ sta5
    dup                             \ sta5 sta5
    state-invert-to-mask            \ sta5 msk~5
    s" m1010" mask-from-string-a    \ sta5 msk~5 msk10

    2dup masks-eq?                  \ sta5 msk~5 msk10 bool
    false? abort" state ne 10?"

    \ Clean up.
    mask-deallocate
    mask-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-invert - Ok"
;

: state-test-same-num-bits?
    s" s0101" state-from-string-a   \ sta54
    s" s0100" state-from-string-a   \ sta54 sta44
    s" s101"  state-from-string-a   \ sta54 sta44 sta53

    2dup states-dif-num-bits?       \ sta54 sta44 sta53 bool
    invert abort" states have same num bits?"

    #2 pick #2 pick                 \ sta54 sta44 sta53 sta54 sta44
    states-dif-num-bits?            \ sta54 sta44 sta53 bool
    abort" states don't have the same num bits?"

    \ Clean up.
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-same-num-bits? - Ok"
;

: state-test-complement
    \ Init state.
    s" s1010" state-from-string-a   \ sta0'

    \ Get complement.
    dup state-complement            \ sta0' reg-lst'

    \ Check results.
    s" (rx1xx rxxx1 r0xxx rxx0x)" region-list-from-string-a

    2dup region-lists-eq?           \ sta0' reg-lst' reg-lst2' bool
    invert abort" region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-complement - Ok"
;

: state-test-~a+~b

    \ Init two states.
    s" s1101" state-from-string-a   \ stad'
    s" s0101" state-from-string-a   \ sta5'
    s" s0111" state-from-string-a   \ stad' sta5' sta6'

    \ Get ~5 + ~7.
    2dup                            \ stad' sta5' sta6' sta5' sta6'
    state-~a+~b                     \ stad' sta5' sta6' reg-lst'
    cr ." ~5 + ~7: " dup .region-list

    \ Check results.
    s" (rxx0x rxxx0 rx0xx rxx1x r1xxx)"
    region-list-from-string-a       \ stad' sta5' sta6' 57-lst' tst-lst'
    2dup region-lists-eq?           \ stad' sta5' sta6' 57-lst' tst-lst' bool
    invert abort" region lists ne?"
    region-list-deallocate          \ stad' sta5' sta6' 57-lst'

    \ Get ~5 + ~D
    #2 pick                         \ stad' sta5' sta6' 57-lst' sta5'
    #4 pick                         \ stad' sta5' sta6' 57-lst' sta5' stad'
    state-~a+~b                     \ stad' sta5' sta6' 57-lst' 5d-lst'
    cr ." ~5 + ~D: " dup .region-list

    \ Check results.
    s" (r0xxx rxxx0 rx0xx rxx1x r1xxx)"
    region-list-from-string-a       \ stad' sta5' sta6' 57-lst' 5d-lst' tst-lst'
    2dup region-lists-eq?           \ stad' sta5' sta6' 57-lst' 5d-lst' tst-lst' bool
    invert abort" region lists ne?"
    region-list-deallocate          \ stad' sta5' sta6' 57-lst' 5d-lst

    \ Get intersections of 57-lst and 5d-lst.
    2dup region-list-intersections-nosubs   \ stad' sta5' sta6' 57-lst' 5d-lst' 57d-lst'
    cr ." (~5 + ~7) & (~5 + ~D): " dup .region-list

    \ Check results.
    s" (r1XXX rXX1X rX0XX rXXX0 r0X0X)"
    region-list-from-string-a       \ stad' sta5' sta6' 57-lst' 5d-lst' 57d-lst' tst-lst
    2dup region-lists-eq?           \ stad' sta5' sta6' 57-lst' 5d-lst' 57d-lst' tst-lst' bool
    invert abort" region lists ne?"
    region-list-deallocate          \ stad' sta5' sta6' 57-lst' 5d-lst' 57d-lst'

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    state-deallocate
    state-deallocate
    state-deallocate

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-test-~a+~b - Ok"
;

: state-test-region-from-corner
    s" (s1100 (s0100 s1101))" list-from-string-a    \ sta-lst

    dup state-regions-from-corner                   \ sta-lst reg-lst

    cr ." regs: " dup .region-list cr

    region-list-deallocate                          \ sta-lst
    structinfo-list-deallocate-struct-list

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-region-from-corner - Ok"
;

: state-test-region-from-corners
    s" ((s1100 (s0100 s1101)) (s0111 (s0101 s1111)))" list-from-string-a    \ sta-lst

    dup state-regions-from-corners                  \ sta-lst reg-lst

    cr ." regs: " dup .region-list cr

    region-list-deallocate                          \ sta-lst
    structinfo-list-deallocate-struct-list

    \ Check for memory leaks.
     structinfo-list-store-project-deallocated

    cr ." state-region-from-corners - Ok"
;

: state-tests
    state-test-basic
    states-test-eq?
    state-test-bit
    state-test-and
    state-test-and-mask
    state-test-invert-to-mask
    state-test-same-num-bits?
    state-test-complement
    state-test-~a+~b
    \ state-region-from-corner
;
