
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
