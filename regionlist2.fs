\ Functions for region lists.

\ Calculate ~A + ~B for two states, and intersect the result with a region-list
\ to produce a cumulative list.
: regionlist-cumulative-~a+~b ( sta2 sta1 reg-lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-state
    assert-nos-is-state

    -rot                                \ reg-lst0 sta2 sta1
    state-~a+~b                         \ reg-lst0 reg-lst'
    tuck                                \ reg-lst' reg-lst0 reg-lst'
    region-list-intersections-nosubs    \ reg-lst' ret-lst
    swap region-list-deallocate         \ ret-lst
;

