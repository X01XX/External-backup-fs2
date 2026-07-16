
\ Return the complement of a state.
: state-complement ( sta0 -- reg-list )
    \ Check arg.
    assert( tos is-state? )

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
    assert( tos is-state? )
    assert( nos is-state? )

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

\ Return regions implied from a corner,
\ a state and a list of dissimilar states. ( sta0 ( sta1 sta2 ... ))
: state-regions-from-corner ( sta-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-state-list? )

    \ Split input list.
    dup list-get-first-item                 \ sta-lst0 anc
    swap list-get-second-item               \ anc sta-lst

    \ Init return list.
    over state-get-num-bits                 \ anc sta-lst nb
    region-list-max-x                       \ anc sta-lst ret-lst
    swap                                    \ anc ret-lst sta-lst

    foreach                                 \ anc ret-lst lnk
        \ Get ~a + ~b.
        dup link-get-data                   \ anc ret-lst lnk stax
        #3 pick                             \ anc ret-lst lnk stax anc
        state-~a+~b                         \ anc ret-lst lnk reg-lst'

        \ Intersect new region list.
        #2 pick                             \ anc ret-lst lnk reg-lst' ret-lst
        over                                \ anc ret-lst lnk reg-lst' ret-lst reg-lst'
        region-list-intersections-nosubs    \ anc ret-lst lnk reg-lst' ret-lst-new'
        swap region-list-deallocate         \ anc ret-lst lnk ret-lst-new'

        \ Replace old return list.
        rot region-list-deallocate          \ anc lnk ret-lst-new'
        swap                                \ anc ret-lst-new' lnk
    next
                                            \ anc ret-lst
    nip
;

\ Return regions implied from a list of corners,
\ a state and a list of dissimilar states. ( sta0 ( sta1 sta2 ... ))
: state-regions-from-corners ( crn-lst0 -- reg-lst )
    \ Init return list.
    dup list-get-first-item                 \ crn-lst0 crn0
    list-get-first-item                     \ crn-lst0 anc0
    state-get-num-bits                      \ crn-lst0 nb
    region-list-max-x                       \ crn-lst0 ret-lst
    swap                                    \ ret-lst crn-lst0

    foreach                                 \ ret-lst lnk
        dup link-get-data                   \ ret-lst lnk crnx
        state-regions-from-corner           \ ret-lst lnk reg-lst'

        \ Intersect new region list.
        #2 pick                             \ ret-lst lnk reg-lst' ret-lst
        over                                \ ret-lst lnk reg-lst' ret-lst reg-lst'
        region-list-intersections-nosubs    \ ret-lst lnk reg-lst' ret-lst-new
        swap region-list-deallocate         \ ret-lst lnk ret-lst-new

        \ Replace old return list.
        rot region-list-deallocate          \ lnk ret-lst-new
        swap                                \ ret-lst-new lnk
    next
                                            \ ret-lst
;
