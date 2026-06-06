\ Test state-list functions.

: state-list-test-or-items

    \ Init state list.
    s" (s10101 s10110 s10111)"      \ c-addr u
    state-list-from-string-a        \ sta-lst'

    \ Get result.
    dup state-list-or-items         \ sta-lst' sta'

    \ Test result.
    s" s10111" state-from-string-a  \ sta-lst' sta' sta_t'
    2dup states-eq?                 \ sta-lst' sta' sta_t' bool
    invert abort" states ne?"

    \ Deallocate
    state-deallocate
    state-deallocate
    state-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-list-test-or-items - Ok"
;

: state-list-test-and-items

    \ Init state list.
    s" (s10101 s10110 s10111)"      \ c-addr u
    state-list-from-string-a        \ sta-lst'

    \ Get result.
    dup state-list-and-items        \ sta-lst' sta'

    \ Test result.
    s" s10100" state-from-string-a  \ sta-lst' sta' sta_t'
    2dup states-eq?                 \ sta-lst' sta' sta_t' bool
    invert abort" states ne?"

    \ Deallocate
    state-deallocate
    state-deallocate
    state-list-deallocate

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." state-list-test-and-items - Ok"
;

: state-list-tests
    state-list-test-or-items
    state-list-test-and-items
;

\ s10101
\ s10110
\ s10111
