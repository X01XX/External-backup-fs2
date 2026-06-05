
\ Return the complement of a state.
: state-complement ( sta0 -- reg-list )
    \ Check arg.
    assert-tos-is-state

    \ Get state of all bits.
    dup state-get-num-bits      \ sta0 nb
    dup                         \ sta0 nb nb
    all-bits                    \ sta0 nb all
    swap state-new              \ sta0 sta-all

    \ Make zero state.
    0                           \ sta0 sta-all 0
    #2 pick                     \ sta0 sta-all 0 sta0
    state-get-num-bits          \ sta0 sta-all 0 nb
    state-new                   \ sta0 sta-all sta-0

    \ Make region of all x.
    region-new                  \ sta0 reg-x'

    \ Get state complement.
    tuck                        \ reg-x' sta0 reg-x'
    region-subtract-state       \ reg-x' reg-lst

    \ Clean up.
    swap region-deallocate      \ reg-lst
;

\ Return the union of, the complements of, two states.
: state-~a+~b ( sta1 sta0 -- reg-lst )
    \ Check arg.
    assert-tos-is-state
    assert-nos-is-state

    \ Get state 0 complement.
    state-complement            \ sta1 reg-lst0'

    \ Get state 1 complement.
    swap                        \ reg-lst0' sta1
    state-complement            \ reg-lst0' reg-lst1'

    \ Get union of complements.
    2dup                        \ reg-lst0' reg-lst1' reg-lst0' reg-lst1'
    region-list-union-nosubs    \ reg-lst0' reg-lst1' ret-lst

    \ Clean up.
    swap region-list-deallocate \ reg-lst0' ret-lst
    swap region-list-deallocate \ ret-lst
;
