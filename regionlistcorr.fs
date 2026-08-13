\ Return true if a tos region-list is a corresponding superset of a nos region-list.
\ The lists must be of equal length.  Corresponding regions must have the same number bits.
: region-list-corr-superset? ( reg-lst1 reg-lst0 -- bool )
    assert( tos is-region-list? )
    assert( nos is-region-list? )
    assert( 2dup lists-equal-length? )

    swap list-get-links         \ reg-lst0 reg-lnk1
    swap list-get-links         \ reg-lnk1 reg-lnk0

    foreach                     \ reg-lnk1 reg-lnk0
        over link-get-data      \ reg-lnk1 reg-lnk0 reg1
        over link-get-data      \ reg-lnk1 reg-lnk0 reg1 reg0
        region-superset?        \ reg-lnk1 reg-lnk0 bool
        ifnot
            2drop
            false
            exit
        then

        swap link-get-next swap
    next
                                \ reg-lnk1
    drop
    true
;

\ Return true if a tos region-list is a corresponding superset of a nos state-list.
\ The list must be of equal length.  Corresponding region/state pairs must have the same number bits.
: region-list-corr-superset-of-states? ( sta-lst1 reg-lst0 -- bool )
    assert( tos is-region-list? )
    assert( nos is-state-list? )
    assert( 2dup lists-equal-length? )

    swap list-get-links             \ reg-lst0 reg-lnk1
    swap list-get-links             \ reg-lnk1 reg-lnk0

    foreach                         \ reg-lnk1 reg-lnk0
        over link-get-data          \ reg-lnk1 reg-lnk0 sta1
        over link-get-data          \ reg-lnk1 reg-lnk0 sta1 reg0
        region-superset-of-state?   \ reg-lnk1 reg-lnk0 bool
        ifnot
            2drop
            false
            exit
        then

        swap link-get-next swap     \ reg-lnk1-nxt reg-lst0
    next
                                    \ reg-lnk1
    drop
    true
;
