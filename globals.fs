
\ A store for the current session.
0 value current-session-store

: current-session-gbl ( -- ses )
    current-session-store   \ ses
    ?dup
    if
    else
        cr ." No session is allocated."
        abort
    then
;

\ Return the current domain, instead of passing it down through arguments.
: current-domain-gbl
    current-session-gbl
    session-get-current-domain-xt
    execute
;

\ Return the current domain id.
: current-domain-id-gbl
    current-domain-gbl
    domain-get-inst-id-xt
    execute
;

\ Return the current domain all bits mask.
: current-all-bits-mask-gbl
    current-domain-gbl
    domain-get-all-bits-mask-xt
    execute
;

\ Return the current domain most significant bit mask.
: current-ms-bit-mask-gbl
    current-domain-gbl
    domain-get-ms-bit-mask-xt
    execute
;

\ Return the current domain max-region.
: current-max-region-gbl
    current-domain-gbl
    domain-get-max-region-xt
    execute
;

\ Return the current domain number bits.
: current-num-bits-gbl
    current-domain-gbl
    domain-get-num-bits-xt
    execute
;

\ Return the current action, instead of passing it down through arguments.
: current-action-gbl
    current-domain-gbl
    domain-get-current-action-xt
    execute
;

\ Return the current actino id.
: current-action-id-gbl
    current-action-gbl
    action-get-inst-id-xt
    execute
;

: .stack-gbl
    .stack-structs-xt execute
;

\ Set the current domain.
: domain-set-current-gbl ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain-xt execute

    dup domain-get-parent-session-xt execute
    session-set-current-domain-xt execute
;

\ Return the number of domains stored in the current session.
: number-domains-gbl ( -- num )
    current-session-gbl
    session-get-number-domains-xt execute
;

\ Return a reference to the current session's domain list.
: get-domain-list-gbl
    current-session-gbl
    session-get-domains-xt execute
;

\ Return a list of maximum domain regions.
: regioncorr-max-regions-gbl
    current-session-gbl
    session-calc-max-regions-xt execute
;
