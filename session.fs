\ Implement a Session struct and functions.

#31319 constant session-struct-id
    #7 constant session-struct-number-cells

\ Struct fields
0                                       constant session-header-disp                \ 16-bits [0] struct id [1] use count
session-header-disp             cell+   constant session-domains-disp               \ A domain-list, kind of like senses.
session-domains-disp            cell+   constant session-step-num-disp              \ Starts at zero.
session-step-num-disp           cell+   constant session-max-regions-disp           \ A regioncorr of maximum regions, corresponding in order to the domain list.

session-max-regions-disp        cell+   constant session-valued-regioncorrs-disp    \ A valued regioncorr list, added during session definition.

session-valued-regioncorrs-disp cell+   constant session-paths-disp                 \ A list of path structs, made from lists of regioncorr-lists to avoid.
                                                                                    \
                                                                                    \ Lists of increasingly fewer, more negative, regioncorrs, to avoid.
                                                                                    \ Like ( (-1 -2 ) ( -2 ) ()).
                                                                                    \
                                                                                    \ If your start, and goal, are not in any negative regioncorr, you want to avoid
                                                                                    \ -1, -2 regioncorrs.
                                                                                    \
                                                                                    \ If your start, or goal, are in a -1 regioncorr, you want to avoid
                                                                                    \ -2 regions, but not -1 regioncorrs.
                                                                                    \
                                                                                    \ If your start, or goal, are in a -2 regioncorr, proceed without avoiding any regioncorr.
session-paths-disp              cell+   constant session-sesneed-list-disp          \ A sesneed list.

0 value session-mma     \ Storage for session mma instance.

\ Init session mma, return the addr of allocated memory.
: session-mma-init ( num-items -- ) \ sets region-mma.
    dup 1 <
    abort" session-mma-init: Invalid number of items."

    cr ." Initializing Session store."
    session-struct-number-cells swap mma-new to session-mma
;

\ Check if tos is an allocated session.
: is-session? ( addr -- bool )
    dup session-mma mma-is-item?    \ addr bool
    if
        struct-get-id
        session-struct-id =         \ bool
    else
        drop
        false                       \ f
    then
;

' is-session? to is-session?-xt

\ Start accessors.

: session-get-domains ( sess0 -- lst )  \ Return the domain-list from an session instance.
    \ Check arg.
    assert( tos is-session? )

    session-domains-disp +  \ Add offset.
    @                       \ Fetch the field.
;

' session-get-domains to session-get-domains-xt

: _session-set-domains ( lst sess0 -- ) \ Set the domain-list for an session instance.
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-list? )

    session-domains-disp +  \ Add offset.
    !struct                 \ Set the field.
;

: session-get-step-num ( sess0 -- u )
\ Check arg.
    assert( tos is-session? )

    session-step-num-disp + \ Add offset.
    @                       \ Fetch the field.
;

: _session-set-step-num ( sess0 -- u )
    session-step-num-disp + \ Add offset.
    !                       \ Fetch the field.
;

: session-get-max-regions ( sess0 -- regc-lst )  \ Return the max regions list.
    \ Check arg.
    assert( tos is-session? )

    session-max-regions-disp +  \ Add offset.
    @                           \ Fetch the field.
;

: _session-set-max-regions ( regc1 sess0 -- ) \ Set the max regions list.
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-regioncorr? )

    session-max-regions-disp +  \ Add offset.
    !struct                     \ Set the field.
;

: session-get-valued-regioncorrs ( sess0 -- regc-lst )  \ Return the valued regioncorr list.
    \ Check arg.
    assert( tos is-session? )

    session-valued-regioncorrs-disp +   \ Add offset.
    @                                   \ Fetch the field.
;

: _session-set-valued-regioncorrs ( regc-lst sess0 -- ) \ Set the valued regioncorr list.
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-regioncorr-list? )

    session-valued-regioncorrs-disp +   \ Add offset.
    !struct                             \ Set the field.
;

: session-get-paths ( sess0 -- avd-lst )  \ Return the avoid list-of-lists.
    \ Check arg.
    assert( tos is-session? )

    session-paths-disp +    \ Add offset.
    @                       \ Fetch the field.
;

: _session-set-paths ( pth-lst1 sess0 -- ) \ Set the avoid lists-of-lists field.
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-path-list? )

    session-paths-disp +    \ Add offset.
    !struct                 \ Set the field.
;

: session-inc-step-num ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )

    session-step-num-disp + \ Add offset.
    1 swap +!               \ Add one to the field.
;

\ Return a regioncorr of max domain regions.
: session-calc-max-regions ( sess0 -- regc-max )

    \ Get domain-list.
    dup session-get-domains         \ sess0 dom-lst

    \ Init return list.
    list-new swap                   \ sess0 reg-lst dom-lst

    foreach                         \ sess0 reg-lst d-link domx
        domain-get-max-region       \ sess0 reg-lst d-link max-reg
        #2 pick                     \ sess0 reg-lst d-link max-reg reg-lst
        region-list-push-end        \ sess0 reg-lst d-link
    next-item
                                    \ sess0 reg-lst

    nip
    regioncorr-new
;

' session-calc-max-regions to session-calc-max-regions-xt

\ Check a valued regioncorr has the the correct length, and has regions
\ with the correct number of bits in each region list position.
: _session-check-valued-regioncorr ( regc1 sess0 -- )
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-regioncorr? )

    \ Check regioncorr value is not 0/0.
    over regioncorr-get-pos-value       \ regc1 sess0 pos
    0= if
        over regioncorr-get-neg-value   \ regc1 sess0 neg
        0= abort" Valued regioncorr has no values?"
    then

    \ Check regioncorr length.
    over regioncorr-get-list list-get-length    \ regc1 sess0 regc-len
    over session-get-domains list-get-length    \ regc1 sess0 regc-len dom-len
    <> abort" Valued regioncorr length invalid?"

    \ Check bit corresponding bit values.
    over regioncorr-get-list list-get-links \ regc1 sess0 reg-lnk
    over session-get-domains list-get-links \ regc1 sess0 reg-lnk dom-lnk
    begin
        ?dup
    while
        over link-get-data region-get-num-bits
        over link-get-data domain-get-num-bits
        <> abort" Valued regioncorr invalid num bits?"

        link-get-next swap
        link-get-next swap
    repeat
                                    \ regc1 sess0 reg-lnk
    2drop drop                      \ regc1 sess0
;

\ Check that valued regioncorrs are the correct length, and have regions
\ with the correct number of bits in each region list position.
: _session-check-valued-regioncorrs ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )

    dup session-get-valued-regioncorrs      \ sess0 regc-lst
    foreach                                 \ sess0 regc-lnx regcx
        #2 pick                             \ sess0 regc-lnx regcx sess0
        _session-check-valued-regioncorr    \ sess0 regc-lnx
    next-item
                                            \ sess0
    drop
;

: session-get-sesneed-list ( sess0 -- ned-lst )
    \ Check arg.
    assert( tos is-session? )

    session-sesneed-list-disp + \ Add offset.
    @                           \ Fetch the field.
;

: _session-set-sesneed-list ( ned-lst sess0 -- )
    session-sesneed-list-disp + \ Add offset.
    !struct                     \ Set the field.
;

: _session-update-sesneed-list ( ned-lst sess0 -- )
    dup session-get-sesneed-list    \ ned-lst sess0 old-neds
    -rot                            \ old-neds ned-lst sess0
    _session-set-sesneed-list       \ old-neds
    sesneed-list-deallocate
;

\ End accessors.

\ Process the valued-regioncorr list.
: _session-process-valued-regioncorrs ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )
\ test with no regioncorrs.

    \ Check if no valued regioncorrs.
    dup session-get-valued-regioncorrs      \ sess0 regc-lst
    \ cr ." _session-process-valued-regioncorrs: start: " dup .regioncorr-list cr

    list-get-length                         \ sess0 num
    0= if
        \ Push avoid-nothing list.
        list-new                            \ sess0 avd-lst
        over session-get-max-regions        \ sess0 avd-lst regc-max
        path-new                            \ sess0 pth
        swap session-get-paths              \ pth pth-lst
        list-push-end-struct
        exit
    then
\ Test with one regc.
\ Test with all pos regcs.
    \ Calc valued regioncorrs fragments.
    dup session-get-valued-regioncorrs      \ sess0 regc-lst
    regioncorr-list-split-by-intersections  \ sess0, spl-lst' t | f
    invert abort" split failed?"

    cr s" fragments: " #2 pick .regioncorr-list-prefix cr

\ Save fragments?

    \ Check if no negative regioncorrs.
    over session-get-valued-regioncorrs     \ sess0 spl-lst' regc-lst
    regioncorr-list-number-negative         \ sess0 spl-lst' num
    0= if
        \ Clean up.
        list-deallocate                     \ sess0

        \ Push avoid-nothing list.
        list-new path-new swap              \ pth sess0
        session-get-paths                   \ pth pth-lst
        list-push-struct
        exit
    then

    \ Make sorted list of fragment negative values.
    list-new                                \ sess0 spl-lst' val-lst'
    over                                    \ sess0 spl-lst' val-lst' spl-lst'
    foreach                                 \ sess0 spl-lst' val-lst' spl-lnk regcx
        regioncorr-get-neg-value            \ sess0 spl-lst' val-lst' spl-lnk neg
        [ ' = ] literal swap                \ sess0 spl-lst' val-lst' spl-lnk xt neg
        #3 pick                             \ sess0 spl-lst' val-lst' spl-lnk xt neg val-lst'
        list-member?                        \ sess0 spl-lst' val-lst' spl-lnk bool
        ifnot
            dup link-get-data               \ sess0 spl-lst' val-lst' spl-lnk regcx
            regioncorr-get-neg-value        \ sess0 spl-lst' val-lst' spl-lnk neg
            #2 pick                         \ sess0 spl-lst' val-lst' spl-lnk neg val-lst'
            list-push-end                   \ sess0 spl-lst' val-lst' spl-lnk
        then
    next-item

                                            \ sess0 spl-lst' val-lst'
    \ Sort value list, descending.
    [ ' > ] literal over list-sort          \ sess0 spl-lst' val-lst'
    cr ." vals: " [ ' . ] literal over .list cr

    \ Make sets of negative valued regioncorrs.
    list-new                                \ sess0 spl-lst' val-lst' avoid-lol'

    over                                    \ sess0 spl-lst' val-lst' avoid-lol' val-lst'
    foreach                                 \ sess0 spl-lst' val-lst' avoid-lol' val-lnk valx
        #4 pick                             \ sess0 spl-lst' val-lst' avoid-lol' val-lnk valx spl-lst'
        regioncorr-list-negative-valued     \ sess0 spl-lst' val-lst' avoid-lol' val-lnk neg-lst'
        dup                                 \ sess0 spl-lst' val-lst' avoid-lol' val-lnk neg-lst' neg-lst'
        #3 pick                             \ sess0 spl-lst' val-lst' avoid-lol' val-lnk neg-lst' neg-lst' avoid-lol'
        regioncorr-list-append-nodups       \ sess0 spl-lst' val-lst' avoid-lol' val-lnk neg-lst'
        regioncorr-list-deallocate          \ sess0 spl-lst' val-lst' avoid-lol' val-lnk
        \ cr ." avoid list: " over .regioncorr-list cr
        over list-copy-struct               \ sess0 spl-lst' val-lst' avoid-lol' val-lnk avoid-lst2'

        \ Calc regions to traverse.
        #5 pick session-get-max-regions     \ sess0 spl-lst' val-lst' avoid-lol' val-lnk avoid-lst2' regc-max
        path-new                            \ sess0 spl-lst' val-lst' avoid-lol' val-lnk pth'
        \ cr ." path: " dup .path cr
        #5 pick                             \ sess0 spl-lst' val-lst' avoid-lol' val-lnk pth' sess0
        session-get-paths                   \ sess0 spl-lst' val-lst' avoid-lol' val-lnk pth' pth-lst
        list-push-struct                    \ sess0 spl-lst' val-lst' avoid-lol' val-lnk
    next-item

    \ Clean up.                             \ sess0 spl-lst' val-lst' avoid-lol'
    regioncorr-list-deallocate              \ sess0 spl-lst' val-lst'
    list-deallocate                         \ sess0 spl-lst'
    regioncorr-list-deallocate              \ sess0

    \ Push avoid-nothing list.
    list-new                                \ sess0 avd-lst
    over session-get-max-regions            \ sess0 avd-lst regc-max
    path-new                                \ sess0 pth

    swap session-get-paths                  \ pth pth-lst
    list-push-end-struct
;

\ Create a session instance, given a valued-regioncorr avoid list, and a domain-list.
: session-new ( regc-lst1 dom-lst0 -- sess ) \ new session pushed onto session stack.
    \ cr ." session-new: start " .s cr

    \ Allocate instance.
    session-struct-id session-mma
    struct-allocate                         \ vregc-lst1 dom-lst0 ses

    \ Set given fields.
    tuck _session-set-domains               \ vregc-lst1 ses
    tuck _session-set-valued-regioncorrs    \ sess

    dup _session-check-valued-regioncorrs   \ sess

    \ Init other fields.
    0 over _session-set-step-num
    dup session-calc-max-regions            \ sess max-regs
    over _session-set-max-regions           \ sess

    \ Process valued regioncorr list.
    list-new over _session-set-paths

    dup _session-process-valued-regioncorrs

    list-new over _session-set-sesneed-list

    dup to session-store
;

: session-valid-dom-id? ( id sess -- bool )
    \ Check arg.
    assert( tos is-session? )

    over 0< if 2drop false exit then

    session-get-domains list-get-length
    <
;

\ Print a session.
: .session ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )

    cr ." Session: Num Domains: "
    dup session-get-domains list-get-length dec.
    space ." Max regions: "
    dup session-get-max-regions regioncorr-get-list .region-list cr

    s" Valued Regioncorrs: "
    #2 pick session-get-valued-regioncorrs
    .regioncorr-list-prefix

    dup session-get-paths
    dup list-get-length
    0=
    if
        cr ." Paths: None"
        drop
    else
        cr ." Paths:"
        foreach
            cr .path
        next-item
    then

    dup session-get-domains
                                                \ sess0 dom-lst
    foreach                                     \ sess0 dom-lnk dom
        \ Print domain
        .domain
    next-item

    drop
;

: session-deallocate ( sess0 -- ) \ Deallocate a session.
    \ Check arg.
    assert( tos is-session? )

    \ Clear fields.
    dup session-get-domains domain-list-deallocate
    dup session-get-max-regions regioncorr-deallocate
    dup session-get-valued-regioncorrs regioncorr-list-deallocate
    dup session-get-paths path-list-deallocate
    dup session-get-sesneed-list sesneed-list-deallocate

    \ Deallocate session.
    session-mma mma-deallocate

    0 to session-store
;

\ Return a list of states, one for each domain, in domain list order.
: session-get-current-states ( sess0 -- stac )
    \ Check args.
    assert( tos is-session? )

    list-new                        \ sess0 sat-lst
    over session-get-domains        \ sess0 sta-lst dom-lst

    foreach                         \ sess0 sta-lst link dom
        domain-get-current-state    \ sess0 sta-lst link stax
        #2 pick                     \ sess0 sta-lst link stax sta-lst
        list-push-end-struct        \ sess0 sta-lst link
    next-item
                                    \ sess0 sta-lst

    nip                             \ sta-lst
    statecorr-new
    \ cr ." session-get-current-states: end: " dup .statecorr cr
;

: session-get-current-regions ( sess0 -- regc )  \ Return a list of regions, one for each domain state, in domain list order.
    \ Check args.
    assert( tos is-session? )

    \ Init return list.
    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 reg-lst dom-lst

    foreach                         \ cur-dom sess0 reg-lst link domx

        domain-get-current-state    \ cur-dom sess0 reg-lst link stax
        dup region-new              \ cur-dom sess0 reg-lst link regx
        #2 pick                     \ cur-dom sess0 reg-lst link regx reg-lst
        list-push-end               \ cur-dom sess0 reg-lst link
    next-item
                                    \ cur-dom sess0 reg-lst
    nip nip                         \ reg-lst

    regioncorr-new
;

: .session-current-states ( sess0 -- )  \ Print a list of current states.
    \ Check args.
    assert( tos is-session? )

    dup session-get-domains         \ sess0 dom-lst
    list-get-links                  \ sess0 d-link
    ." ("
    begin
        ?dup
    while
        dup link-get-data           \ sess0 d-link domx
        domain-get-current-state    \ sess0 d-link d-sta
        .state                      \ sess0 d-link

        link-get-next               \ sess0 d-link-nxt
        dup 0<> if
            space
        then
    repeat
                                    \ sess0
    drop                            \
    ." )"
;

\ Return a domain, given a domain ID.
: session-find-domain ( u1 sess0 -- dom t | f )
    \ Check args.
    assert( tos is-session? )
    over 0< if
        2drop
        false
        exit
    then

    tuck session-get-domains    \ sess0 u1 dom-lst
    2dup list-get-length        \ sess0 u1 dom-lst u1 len
    >= if                       \ sess0 u1 dom-lst
        3drop
        false
        exit
    then

    list-get-item               \ sess0 dom
    nip                         \ dom
    true
;

\ Return the number of domains.
: session-get-number-domains ( sess0 -- u )
    \ Check arg.
    assert( tos is-session? )

    session-get-domains
    list-get-length
;

' session-get-number-domains to session-get-number-domains-xt

\ Check current state for an actneed.
\ If it matches, run the action, return true.
: session-check-current-state-for-actneed ( actneed sess0 -- bool )
    \ Look up domain instance.
    over actneed-get-dom-inst-id    \ actned1 sess0 dom-id
    over                            \ actned1 sess0 dom-id sess0
    session-find-domain             \ actned1 sess0, domx t | f
    invert abort" domain not found?"

    \ Check if states are equal.
    dup domain-get-current-state    \ actned1 sess0 domx d-sta
    #3 pick actneed-get-target      \ actned1 sess0 domx d-sta targ
    states-eq?                      \ actned1 sess0 domx bool
    ifnot
        2drop drop
        false
        exit
    then

    \ Look up action instance.
    #2 pick actneed-get-act-inst-id \ actned1 sess0 domx act-id
    over domain-find-action         \ actned1 sess0 domx, act t | f
    invert abort" action not found?"

    \ Run action.
    swap                            \ actned1 sess0 act domx
    domain-get-sample               \ actned1 sess0 smpl
    2drop drop
    true
;

\ Return regioncorr list to avoid, may be empty.
: session-find-path-instance ( stac2 stac1 sess0 -- pth )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-statecorr? )
    assert( 3os is-statecorr? )

    \ Find regioncorr list to avoid.
    session-get-paths                   \ stac2 stac1 pth-lol
    foreach                             \ stac2 stac1 pth-lnk pthx
        #3 pick #3 pick                 \ stac2 fstac1 pth-lnk pthx gstac2 fstac1
        rot                             \ stac2 fstac1 pth-lnk gstac2 fstac1 pthx
        path-get-avoid-list             \ stac2 fstac1 pth-lnk gstac2 fstac1 avd-lst
        regioncorrlist-neither-stac-in? \ stac2 fstac1 pth-lnk bool
        if
            link-get-data               \ stac2 fstac1 pthx
            nip nip                     \ regc-lst
            exit
        then
    next-item
    cr ." drop through?" cr abort
;

\ Plan a step in a path from a statecorr to another statecorr,
\ use recursion to get more steps.
\ The excursion-limit allows non-incremental steps, if needed, a given number of times.
: session-make-plan-next-step ( goal-stac2 from-stac1 avoid-list excursion-limit sess0 -- plnstp t | f )
    \ Check args.
    assert( tos is-session? )
    assert( 3os is-regioncorr-list? )
    assert( 4os is-statecorr? )
    assert( 5os is-statecorr? )

    \ Check staecorrs equal.
    #4 pick #4 pick statecorrs-eq?
    if
        cr ." statecorrs eq? " cr abort
    then

    \ Check excursion limit.
    over 0<                                             \ gstac2 fstac1 avd-lst ex-lm sess0 bool
    if
        2drop 2drop drop
        false
        exit
    then

    \ Init options list.
    list-new                                            \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst'

    \ Prep for loop
    #4 pick statecorr-get-list list-get-links           \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk
    #2 pick session-get-domains list-get-links          \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk

    begin
        ?dup
    while
        over link-get-data                              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk f-stax
        over link-get-data                              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk f-stax domx

        \ Get/Store options
        domain-get-forward-steps                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk, actstp-lst' t | f
        if
            dup                                         \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk actstp-lst' actstp-lst'
            #4 pick list-append-struct                  \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk actstp-lst'
            actionstep-list-deallocate                    \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk d-lnk
        then

        link-get-next swap
        link-get-next swap
    repeat
                                                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' f-lnk
    drop                                                \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst'

    \ cr s" options: " #2 pick .actionstep-list-prefix cr

    \ Filter out options that do not go toward the goal, or enter regioncorrs to avoid.

    \ Init new opt list.
    list-new                                            \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2'

    \ Calc region from gstac2 fstac1.
    #6 pick #6 pick statecorr-union                     \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc'

    #2 pick                                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lst'
    foreach                                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx
        \ Make tests, or results, true means skip the actionstep.

        \ Init bool.
        false                                           \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool

        \ Calc actionstep-to result.
        #9 pick                                         \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool fstac1
        #2 pick actionstep-insert-to-state              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt'

        \ Check if step result is outside of the expected region.
        dup                                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' stac-nxt'
        #5 pick                                         \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' stac-nxt' regc'
        regioncorr-superset-of-statecorr? invert        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' bool
        rot or                                          \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx stac-nxt' bool
        swap                                            \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt'

        \ Check if step result is in the avoid list.
        dup                                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' stac-nxt'
        #10 pick                                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' stac-nxt' avd-lst
        regioncorr-list-stac-in?                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx bool stac-nxt' bool
        rot                                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx stac-nxt' bool bool
        or                                              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx stac-nxt' bool

        \ Check actionstep alt-to result.
        #2 pick actionstep-get-alt-to 0<>
        if
            cr ." session-make-plan-next-step: todo check actionstep alt-to" cr
        then

        \ Check bool, any true leads to exclusion.
        if                                              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx stac-nxt'
            statecorr-deallocate                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx
            drop                                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk
        else                                            \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk actstpx stac-nxt'
            \ Make a PlanStep.
            swap                                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk stac-nxt' actstpx
            #9 pick statecorr-copy swap                 \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk stac-nxt' fstac1 actstpx
            planstep-new                                \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk plnstp
            #3 pick                                     \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk plnstp opt-lst2'
            list-push-struct                            \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc' opt-lnk
        then

    next-item
                                                        \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' regc'
    regioncorr-deallocate                               \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2'

    \ cr s" direct planstep list: " #2 pick .planstep-list-prefix cr

    dup list-is-empty?
    ifnot
        dup list-get-length                             \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' len
        random                                          \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' inx
        over list-remove-item-struct                    \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' opt-lst2' plnstp
        swap planstep-list-deallocate                   \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst' plnstp
        swap actionstep-list-deallocate                 \ gstac2 fstac1 avd-lst ex-lm sess0 plnstp
        2nip 2nip nip                                   \ plnstp
        true
        exit
    then

    planstep-list-deallocate                              \ gstac2 fstac1 avd-lst ex-lm sess0 opt-lst'
    actionstep-list-deallocate                            \ gstac2 fstac1 avd-lst ex-lm sess0
    2drop 2drop drop
    false
;

: session-make-plan3 ( goal-stac2 from-stac1 avd-lst exc-lim sess0 -- pln t | f )
    \ Check args.
    assert( tos is-session? )
    assert( 3os is-regioncorr-list? )
    assert( 4os is-statecorr? )
    assert( 5os is-statecorr? )

    \ Init plantsep list.
    list-new                                                \ gstac2 fstac1 avd-lst exc-lim sess0 lst

    \ Set first from statecorr.
    #4 pick                                                 \ gstac2 fstac1 avd-lst exc-lim sess0 lst from

    \ Get next step until goal is reached, or failure.
    begin
        \ Get next step.
        #6 pick swap                                        \ gstac2 fstac1 avd-lst exc-lim sess0 lst gstac2 from
        #5 pick #5 pick #5 pick                             \ gstac2 fstac1 avd-lst exc-lim sess0 lst gstac2 from avd-lst exc-lim sess0
        session-make-plan-next-step                         \ gstac2 fstac1 avd-lst exc-lim sess0 lst, plnstp t | f
        ifnot
            planstep-list-deallocate                        \ gstac2 fstac1 avd-lst exc-lim sess0
            2drop 2drop drop
            false
            exit
        then

        \ Check if new step gets to the goal.
        dup planstep-get-to                                 \ gstac2 fstac1 avd-lst exc-lim sess0 lst plnstp to-stac
        #7 pick                                             \ gstac2 fstac1 avd-lst exc-lim sess0 lst plnstp to-stac gstac2
        statecorrs-eq?                                      \ gstac2 fstac1 avd-lst exc-lim sess0 lst plnstp bool
        if
            \ Goal found.
            over list-push-end-struct                       \ gstac2 fstac1 avd-lst exc-lim sess0 lst
            2nip 2nip nip                                   \ lst
            plan-new
            true
            exit
        then

        \ Save step, set up for getting next step.
        dup planstep-get-to swap                            \ gstac2 fstac1 avd-lst exc-lim sess0 lst from-next plnstp
        #2 pick list-push-end-struct                        \ gstac2 fstac1 avd-lst exc-lim sess0 lst from-next
    again
;

\ Return a plan, from statecorr to statecorr, of a given list
\ of two, or more statecorrs.
: session-make-plan2 ( stac-lst3 avd-lst2 exc-lim1 sess0 -- pln t | f )
    \ Check args.
    assert( tos is-session? )
    assert( 3os is-regioncorr-list? )
    assert( 4os is-statecorr-list? )
    \ cr ." session-make-plan2: start: "

    \ Init return plan.
    list-new plan-new                                       \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln

    #4 pick                                                 \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lst3
    foreach                                                 \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk stacx
        over link-get-next                                  \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk stacx lnk-nxt
        ?dup if
            link-get-data                                   \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk stacx sta-nxt
            \ cr ." from: " dup .statecorr space ." to: " over .statecorr cr
            swap                                            \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk stac-nxt stacx
            #6 pick #3                                      \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk | to from avd-lst2 exc-lim
            #6 pick                                         \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk | to from avd-lst2 exc-lim sess0
            session-make-plan3                              \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk, pln t | f
            ifnot
                drop                                        \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln
                plan-deallocate                             \ stac-lst3 avd-lst2 exc-lim1 sess0
                2drop 2drop
                false
                exit
            then

            \ Process partial step plan.                    \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk pln'
            \ cr ." partial plan: " dup .plan cr
            dup plan-get-steps                              \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk pln' pln-stps
            #3 pick                                         \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk pln' pln-stps ret-pln
            plan-append-steps                               \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk pln'
            plan-deallocate                                 \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk
        else
            drop                                            \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln stac-lnk
        then
    next-item
                                                            \ stac-lst3 avd-lst2 exc-lim1 sess0 ret-pln
    \ cr ." Final plan: " dup .plan cr
    2nip nip nip                                            \ ret-pln
    true
;

\ Set the plan value based on valued-regioncorrs each step to-field is in.
: session-set-plan-value-cost ( pln1 sess0 -- )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-plan? )

    \ Get valued regioncorrs to scan.
    session-get-valued-regioncorrs      \ pln1 vregc-lst

    \ Set up counters.
    0 integer-new -rot                  \ cst-int' pln1 vregc-lst
    0 integer-new -rot                  \ cst-int' val-int' pln1 vregc-lst

    \ Prep for loop on plan steps.
    over plan-get-steps                 \ cst-int' val-int' pln vreg-lst plnstp-lst

    foreach                             \ cst-int' val-int' pln vreg-lst plnstp-lnk plnstpx
        \ Get planstep value, cost.
        planstep-get-to                     \ cst-int' val-int' pln vreg-lst plnstp-lnk sta-t
        #2 pick                             \ cst-int' val-int' pln vreg-lst plnstp-lnk sta-t vreg-lst
        regioncorr-list-get-statecorr-rates \ cst-int' val-int' pln vreg-lst plnstp-lnk val-num cst-num

        \ Inc cost integer.
        #6 pick                         \ cst-int' val-int' pln vreg-lst plnstp-lnk val-num cst-num cst-int'
        integer-plus-num                \ cst-int' val-int' pln vreg-lst plnstp-lnk val-num

        \ Inc value integer.
        #4 pick                         \ cst-int' val-int' pln vreg-lst plnstp-lnk val-num val-int
        integer-plus-num                \ cst-int' val-int' pln vreg-lst plnstp-lnk
    next-item
                                        \ cst-int' val-int' pln vreg-lst
    drop                                \ cst-int' val-int' pln

    \ Set plan value.
    over integer-get-number             \ cnt-int' val-int' pln num
    over plan-set-value

    \ Set plan cost.
    #2 pick integer-get-number          \ cnt-int' val-int' pln num
    swap plan-set-cost                  \ cnt-int' val-int'

    \ Clean up.
    integer-deallocate
    integer-deallocate
;

\ Return a plan to change from a statecorr to a different statecorr,
\ given a list of regioncorrs to avoid.
: session-make-plan ( pth-lst2 avd-lst1 sess0 -- pln t | f )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-regioncorr-list? )
    assert( 3os is-statecorr-list? )
    \ cr ." session-make-plan: for: " #2 pick .statecorr-list

    \ Try forward direction.
\    #2 pick #2 pick                                         \ pth-lst2 avd-lst1 sess0 pth-lst2 avd-lst1
\    #3                                                      \ pth-lst2 avd-lst1 sess0 pth-lst2 avd-lst1 exc-lim
\    #3 pick                                                 \ pth-lst2 avd-lst1 sess0 pth-lst2 avd-lst1 exc-lim sess0
\    session-make-plan2                                      \ pth-lst2 avd-lst1 sess0, pln t | f
\    if
\       tuck swap                                           \ pth-lst2 avd-lst1 pln pln sess0
\        session-set-plan-value-cost                        \ pth-lst2 avd-lst1 pln
\        nip nip
\        true
\        exit
\    then

    \ Try the backward direction.                           \ pth-lst2 avd-lst1 sess0 sess0
    \ Literal backward-chaining may need to get through X->0 and X->1 bit changes.
    \ That could be done, but we would have to deal with regioncorrs instead of statecorrs,
    \ linking partial plans that may have intersecting, but not equal, joints would involve rippling the change
    \ throughout the plans.
    #2 pick                                                 \ pth-lst2 avd-lst1 sess0 pth-lst2
    list-reverse-struct                                     \ pth-lst2 avd-lst1 sess0 pth-lst2'
    dup                                                     \ pth-lst2 avd-lst1 sess0 pth-lst2' pth-lst2'
    #3 pick                                                 \ pth-lst2 avd-lst1 sess0 pth-lst2' pth-lst2' avd-lst
    #3                                                      \ pth-lst2 avd-lst1 sess0 pth-lst2' pth-lst2' avd-lst exc-lim
    #4 pick                                                 \ pth-lst2 avd-lst1 sess0 pth-lst2' pth-lst2' avd-lst exc-lim sess0
    session-make-plan2                                      \ pth-lst2 avd-lst1 sess0 pth-lst2', b-pln t | f
    ifnot
        statecorr-list-deallocate                           \ pth-lst2 avd-lst1 sess0
        2drop drop
        false
        exit
    then
    swap statecorr-list-deallocate                           \ pth-lst2 avd-lst1 sess0 b-pln
    \ cr ." backward plan: " dup .plan cr

    \ Get plan as a statecorr-to-statecorr list, reversed.
    dup plan-statecorr-path                                 \ pth-lst2 avd-lst1 sess0 b-pln stac-lst
    swap plan-deallocate                                    \ pth-lst2 avd-lst1 sess0 stac-lst
    dup list-reverse-struct                                 \ pth-lst2 avd-lst1 sess0 stac-lst stac-rev'
    swap statecorr-list-deallocate                          \ pth-lst2 avd-lst1 sess0 stac-rev'
    \ cr ." forward path: " dup .statecorr-list cr

    \ Get forward plan from statecorr list.
    dup                                                     \ pth-lst2 avd-lst1 sess0 stac-rev' | stac-rev'
    #3 pick #3                                              \ pth-lst2 avd-lst1 sess0 stac-rev' | stac-rev' avd-lst1 exc-lim
    #4 pick                                                 \ pth-lst2 avd-lst1 sess0 stac-rev' | stac-rev' avd-lst1 exc-lim sess0
    session-make-plan2                                      \ pth-lst2 avd-lst1 sess0 stac-rev', pln t | f
    if
        swap statecorr-list-deallocate                      \ pth-lst2 avd-lst1 sess0 pln
        tuck swap                                           \ pth-lst2 avd-lst1 pln pln sess0
        session-set-plan-value-cost                         \ pth-lst2 avd-lst1 pln
        nip nip
        true
        exit
    then

    statecorr-list-deallocate                               \ pth-lst2 avd-lst1 sess0
    2drop drop
    false
;

: session-calc-plan ( stac-to1 sess0 -- pln t | f )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-statecorr? )

    \ Get current states.
    dup
    session-get-current-states                  \ stac-to1 sess0 stac-from'
    swap                                        \ stac-to1 stac-from' sess0

    \ Find path instance to use.
    #2 pick #2 pick #2 pick                     \ stac-to1 stac-from' sess0 stac-to1 stac-from' sess0
    session-find-path-instance                  \ stac-to1 stac-from' sess0 pth

    \ Find path.
    #3 pick #3 pick #2 pick                     \ stac-to1 stac-from' sess0 pth stac-to1 stac-from' pth
    path-get-traverse-list                      \ stac-to1 stac-from' sess0 pth stac-to1 stac-from' regc-trvl
    regioncorr-list-find-path                   \ stac-to1 stac-from' sess0 pth, pth-from-to' t | f
    if
        \ cr ." path found: " dup .statecorr-list cr
    else
        \ cr ." path not found" cr
        2drop                                   \ stac-to1 stac-from'
        statecorr-list-deallocate               \ stac-to1
        drop
        false
        exit
    then

    \ Make plan.
    dup                                         \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to'
    #2 pick path-get-avoid-list                 \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to' avd-lst
    #4 pick                                     \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to' avd-lst sess0
    session-make-plan                           \ stac-to1 stac-from' sess0 pth pth-from-to', pln' t | f
    if
        \ cr ." plan found: " dup .plan cr
        swap statecorr-list-deallocate          \ stac-to1 stac-from' sess0 pth pln'
        nip nip                                 \ stac-to1 stac-from' pln'
        swap statecorr-deallocate               \ stac-to1 pln'
        nip                                     \ pln'
        true
    else
        \ cr ." plan not found" cr
        statecorr-list-deallocate               \ stac-to1 stac-from' sess0 pth
        2drop                                   \ stac-to1 stac-from'
        statecorr-deallocate                    \ stac-to1
        drop
        false
        exit
    then
;
\ Run a planstep.
: session-run-planstep ( plnstp1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-planstep? )

    \ Look up domain instance.
    over planstep-get-actionstep    \ plnstp1 sess0 actstp1
    dup actionstep-get-dom-inst-id  \ plnstp1 sess0 actstp1 dom-id
    #2 pick                         \ plnstp1 sess0 actstp1 dom-id sess0
    session-find-domain             \ plnstp1 sess0 actstp1, dom t | f
    invert abort" domain not found? "

    \ Look up action instance       \ plnstp1 sess0 actstp1 dom
    over actionstep-get-act-inst-id \ plnstp1 sess0 actstp1 dom act-id
    over domain-find-action         \ plnstp1 sess0 actstp1 dom, act t | f
    invert abort" action not found?"

    \ Run action.
    swap                            \ plnstp1 sess0 actstp1 act dom
    domain-get-sample               \ plnstp1 sess0 actstp1 smpl
    drop

    drop                            \ plnstp1 sess0

    \ Check results.
    dup session-get-current-states  \ plnstp1 sess0 ses-cur'
    #2 pick planstep-get-to         \ plnstp1 sess0 ses-cur' plnstp-to
    over statecorrs-eq?             \ plnstp1 sess0 ses-cur' bool
    swap statecorr-deallocate       \ plnstp1 sess0 bool

    \ Return.
    nip nip
;

\ Run a plan, return true if the plan succeeded.
: session-run-plan ( pln1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-plan? )
    \ cr ." session-run-plan: start " over .plan cr

    \ Check plan initial states match session current states.
    dup session-get-current-states  \ pln1 sess0 stac'
    #2 pick plan-get-first-states   \ pln1 sess0 stac' stac
    over statecorrs-eq?             \ pln1 sess0 stac' bool
    swap statecorr-deallocate       \ pln1 sess0 bool
    invert abort" plan first states do not match session current states"

    over plan-get-steps             \ pln1 sess0 stp-lst

    foreach                         \ pln1 sess0 stp-lnk plnstpx
        #2 pick                     \ pln1 sess0 stp-lnk plnstpx sess0
        session-run-planstep        \ pln1 sess0 stp-lnk, bool
        \ cr ." planstep run: " dup .bool cr
        ifnot
            2drop drop
            false
            exit
        then
    next-item
                                    \ pln1 sess0
    2drop
    true
;

\ Do actneed.
\ Return true if a plan was run.
: session-do-sesneed-type-1 ( sesned1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-sesneed? )

    over sesneed-get-actneed    \ sesned1 sess0 actnedx

    \ Check if domain current state satisfies the need.
    2dup swap                               \ sesned1 sess0 actnedx actnedx sess0
    session-check-current-state-for-actneed \ sesned1 sess0 actnedx bool
    if
        2drop drop
        true
        exit
    then

    \ Try a path, plan, run.
    over session-get-current-states         \ sesned1 sess0 actnedx stac-cur'
    over actneed-get-target                 \ sesned1 sess0 actnedx stac-cur' targ
    #2 pick actneed-get-dom-inst-id         \ sesned1 sess0 actnedx stac-cur' targ dom-id
    #2 pick                                 \ sesned1 sess0 actnedx stac-cur' targ dom-id stac-cur'
    statecorr-insert-state                  \ sesned1 sess0 actnedx stac-cur' stac-targ'
    dup                                     \ sesned1 sess0 actnedx stac-cur' stac-targ' stac-targ'
    #4 pick                                 \ sesned1 sess0 actnedx stac-cur' stac-targ' stac-targ' 'sess0
    session-calc-plan                       \ sesned1 sess0 actnedx stac-cur' stac-targ', pln' t | f
    ifnot
        cr ." Plan not found" cr
        statecorr-deallocate                \ sesned1 sess0 actnedx stac-cur'
        statecorr-deallocate                \ sesned1 sess0 actnedx
        2drop drop
        false
        exit
    then
                                            \ sesned1 sess0 actnedx stac-cur' stac-targ' pln'
    dup                                     \ sesned1 sess0 actnedx stac-cur' stac-targ' pln' pln'
    #5 pick                                 \ sesned1 sess0 actnedx stac-cur' stac-targ' pln' pln' sess0
    session-run-plan                        \ sesned1 sess0 actnedx stac-cur' stac-targ' pln' bool
    swap plan-deallocate                    \ sesned1 sess0 actnedx stac-cur' stac-targ' bool
    \ cr ." Plan run: returns: " dup .bool cr

    \ Clean up a little.
    swap statecorr-deallocate               \ sesned1 sess0 actnedx stac-cur' bool
    swap statecorr-deallocate               \ sesned1 sess0 actnedx bool

    ifnot
        2drop drop
        true
        exit
    then

    swap                                    \ sesned1 actnedx sess0
    session-check-current-state-for-actneed \ sesned1 bool
    if
        drop
        true
        exit
    else
        cr ." Plan worked, but result bad?" cr abort
    then
;

\ Try to satisfy a need.
\ Return true if a plan was run.
: session-satisfy-need ( sesned1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-sesneed? )

    over sesneed-get-type       \ sesned1 sess0 typ
    case
        1 of
            session-do-sesneed-type-1
        endof
        #2 of
            2drop
            false               \ sesned1 sess0 bool
        endof
        #3 of
            2drop
            false               \ sesned1 sess0 bool
        endof
        nip nip
        swap false              \ sesned1 sess0 bool type
    endcase
                                \ bool
;

: session-do-zero-token-command ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )
    \ cr ." session-do-zero-token-command" cr

    dup session-inc-step-num

    dup session-get-sesneed-list            \ sess0 sesned-lst

    dup list-is-empty?
    if
        2drop
        exit
    then

    dup list-get-length pick-list           \ sess0 sesned-lst pik-lst

    begin
        dup list-is-not-empty?
    while
        \ Get a randomly chosen need ( no dups ).
        dup random-pick                     \ sess0 sesned-lst pik-lst, inx t | f
        invert abort" random pick failed?"

        #2 pick list-get-item               \ sess0 sesned-lst pik-lst sesnedx
        cr ." Need chosen: " dup .sesneed cr

        \ Satisfy need.
        #3 pick                             \ sess0 sesned-lst pik-lst sesnedx sess0
        session-satisfy-need                \ sess0 sesned-lst pik-lst result ( true = plan run )
        if
            list-deallocate                 \ sess0 sesned-lst
            2drop
            exit
        then
    repeat

    list-deallocate                         \ sess0 sesned-lst
    2drop
;

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: session-eval-user-input ( cmd-lst1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-list? )
    \ cr ." session-eval-user-input: start " over .token-list cr

    \ Check command.
    over list-get-first-item            \ cmd-lst1 sess0 tkn0

    dup token-get-string                \ cmd-lst1 sess0 tkn0 c-addr u
    s" q" str=                          \ cmd-lst1 sess0 tkn0 bool
    if
        2drop drop
        false
        \ cr ." session-eval-user-input: end: 2" cr
        exit
    then

    dup token-get-string                \ cmd-lst1 sess0 tkn0 c-addr u
    s" ps" str=                         \ cmd-lst1 sess0 tkn0 bool
    if
        drop                            \ cmd-lst1 sess0
        swap list-get-length            \ sess0 len
        1 =                             \ sess0 bool
        if                              \ sess0
            \ Print Session.
            .session                    \
        else                            \ sess0
            drop                        \
            \ cr ." ps command: invalid number of arguments" cr
        then
        true
        \ cr ." session-eval-user-input: end: 3" cr
        exit
    then

    dup token-get-string                \ cmd-lst1 sess0 tkn0 c-addr u
    s" mu" str=                         \ cmd-lst1 sess0 tkn0 bool
    if
        2drop                           \ cmd-lst1
        list-get-length                 \ len
        1 =
        if
            \ Display Memory Usage.
            .memory-use
        else
            cr ." mu command: invalid number of arguments" cr
        then
        true
        exit
    then

    dup token-get-string                \ cmd-lst1 sess0 tkn0 c-addr u
    s" sas" str=                        \ cmd-lst1 sess0 tkn0 bool
    if
        drop                            \ cmd-lst1 sess0

        \ Check command legth.
        over list-get-length            \ cmd-lst1 sess0 len
        #4 =
        ifnot
             cr ." sas command: invalid number of arguments" cr
            2drop
            true
            exit
        then

        \ Check fourth item is a state.
        over list-get-fourth-item       \ cmp-lst sess0 sta
        is-state?                       \ cmp-lst sess0 bool
        ifnot
            cr ." sas command: invalid state parameter" cr
            2drop
            true
            exit
        then

        \ Check second item is a valid domain id.
        over list-get-second-item       \ cmp-lst sess0 dom-id
        over                            \ cmp-lst sess0 dom-id sess0
        session-valid-dom-id?           \ cmp-lst sess0 bool
        ifnot
            cr ." sas command: invalid domain id parameter" cr
            2drop
            true
            exit
        then

        \ Check third item is a valid action id.
        over list-get-third-item        \ cmp-lst sess0 act-id
        #2 pick list-get-second-item    \ cmp-lst sess0 act-id dom-id
        #2 pick                         \ cmp-lst sess0 act-id dom-id sess0
        session-find-domain             \ cmp-lst sess0 act-id, dom t | f
        invert abort" domain not found?"

        2dup domain-valid-act-id?       \ cmp-lst sess0 act-id dom bool
        ifnot
            cr ." sas command: invalid action id parameter" cr
            2drop 2drop
            true
            exit
        then

        \ Check state num bits equals domain num bits.
        dup domain-get-num-bits         \ cmp-lst sess0 act-id dom dnb
        #4 pick list-get-fourth-item    \ cmp-lst sess0 act-id dom dnb sta
        state-get-num-bits =            \ cmp-lst sess0 act-id dom bool
        ifnot
            cr ." sas command: state num bits is invalid for domain" cr
            2drop 2drop
            true
            exit
        then

        \ Get sample.
        #3 pick list-get-fourth-item    \ cmp-lst sess0 act-id dom sta
        #2 pick #2 pick                 \ cmp-lst sess0 act-id dom sta act-id dom
        domain-get-arbitary-sample      \ cmp-lst sess0 act-id dom smpl
        drop                            \ cmp-lst sess0 act-id dom

        \ Show act.
        domain-find-action              \ cmp-lst sess0, act t | f
        invert abort" action not found?"
        .action

        \ Cleanup.
        2drop
        true

        cr ." Press Enter to continue "
        get-string-from-user            \ true p-addr len
        2drop

        exit
    then

    dup token-get-string                \ cmd-lst1 sess0 tkn0 c-addr u
    s" pa" str=                        \ cmd-lst1 sess0 tkn0 bool
    if
        drop                            \ cmd-lst1 sess0

        \ Check command legth.
        over list-get-length            \ cmd-lst1 sess0 len
        #3 =
        ifnot
             cr ." pa command: invalid number of arguments" cr
            2drop
            true
            exit
        then

        \ Check second item is a valid domain id.
        over list-get-second-item       \ cmp-lst sess0 dom-id
        over                            \ cmp-lst sess0 dom-id sess0
        session-valid-dom-id?           \ cmp-lst sess0 bool
        ifnot
            cr ." pa command: invalid domain id parameter" cr
            2drop
            true
            exit
        then

        \ Check third item is a valid action id.
        over list-get-third-item        \ cmp-lst sess0 act-id
        #2 pick list-get-second-item    \ cmp-lst sess0 act-id dom-id
        #2 pick                         \ cmp-lst sess0 act-id dom-id sess0
        session-find-domain             \ cmp-lst sess0 act-id, dom t | f
        invert abort" domain not found?"

        2dup domain-valid-act-id?       \ cmp-lst sess0 act-id dom bool
        ifnot
            cr ." pa command: invalid action id parameter" cr
            2drop 2drop
            true
            exit
        then

        \ Get action.
        domain-find-action              \ cmp-lst sess0, act t | f
        invert abort" action not found?"

        .action                         \ cmp-lst sess0

        \ Cleanup.
        2drop
        true

        cr ." Press Enter to continue "
        get-string-from-user            \ true p-addr len
        2drop

        exit
    then

\ dn, cds?, scs, ?, tos, to: remember to do session-inc-step-num

    cr ." Did not understand the command." cr
    drop 2drop                          \
    \ cr ." session-eval-user-input: end: 4" cr
    true
;

\ Make a new session need, from an agtion need.
: session-make-sesneed-from-actneed ( actned1 sess0 -- sesned )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-actneed? )

    \ Assemble statecorr-to from current state and action state target.
    session-get-current-states          \ actned1 stac-cur'
    over actneed-get-target             \ actned1 stac-cur' stac-to
    #2 pick actneed-get-dom-inst-id     \ actned1 stac-cur' stac-to dom-id
    #2 pick                             \ actned1 stac-cur' stac-to dom-id stac-cur'
    statecorr-insert-state              \ actned1 stac-cur' stac-to'
    swap statecorr-deallocate           \ actned1 stac-to'

    \ Make session need.
    actneed-type-snig                   \ actned1 stac-to' typ
    sesneed-new                         \ sessned
;

\ Return a list of session needs, if any.
: session-get-needs ( sess0 -- sesned-lst t | f )
    \ Check arg.
    assert( tos is-session? )

    dup session-get-domains     \ sess0 dom-lst
    domain-list-get-needs       \ sess0, actned-lst' t | f
    ifnot
        drop
        false
        exit
    then

    \ Init return list.
    list-new -rot               \ ret-lst sess0 actned-lst'

    \ Prep for loop.
    dup                         \ ret-lst sess0 actned-lst' actned-lst'

    foreach                     \ ret-lst sess0 actned-lst' actned-lnk actnedx
        \ Convert an action need to a session need.
        #3 pick                             \ ret-lst sess0 actned-lst' actned-lnk actnedx sess0
        session-make-sesneed-from-actneed   \ ret-lst sess0 actned-lst' actned-lnk sessneed

        \ Save session need.
        #4 pick                 \ ret-lst sess0 actned-lst' actned-lnk sessneed ret-lst
        list-push-struct        \ ret-lst sess0 actned-lst' actned-lnk
    next-item
                                \ ret-lst sess0 actned-lst'
    actneed-list-deallocate     \ ret-lst sess0
    drop                        \ ret-lst

    check-list-return
;

\ Get and save needs.
: session-set-all-needs ( sess0 -- )
    assert( tos is-session? )

    dup session-get-needs           \ sess0, ned-lst t | f
    if
        swap                        \ ned-lst sess0
    else
        list-new swap               \ lst sess0
    then

    \ TODO add session-level needs.

    _session-update-sesneed-list
;

\ Get input of up to TOS characters from user, using the PAD area, up to a given number of characters.
\ Evaluate the input.
\ like: 80 s" Enter command: > " get-user-input
\
\ If this aborts, various things can be done:
\
\ Print all domains, and actions.
\   current-session-gbl  .session
\
\ Print Domain 1.
\    1  current-session-gbl  session-find-domain  drop  .domain
\
\ Print Domain 1, Act 4.
\    1  current-session-gbl  session-find-domain  drop  4  swap  domain-find-action  drop  .action
\
\ Print the squares of domain 1 action 4.
\    1  current-session-gbl  session-find-domain  drop  4  swap  domain-find-action  drop  action-get-squares  .square-list
\
\ Return a bool for continuing the REP loop.
\ Return false if the user enterd the q (quit) command, else true.
: session-get-user-input ( sess0 -- bool )
    \ Check arg.
    assert( tos is-session? )

    \ Display needs.
    dup session-set-all-needs       \ sess0
    dup session-get-sesneed-list    \ sess0 ned-lst
    dup list-is-empty?              \ sess0 ned-lst bool
    if
        cr ." Needs: No needs found" cr
        drop
    else
        cr s" Needs:" rot .sesneed-list-prefix cr   \ sess0
        cr ." Press Enter to randomly choose a need."
    then

    cr ." q - to quit"
    cr
    cr ." ps - Print Session, all domains."
    cr ." pd <domain id> - Print Domain."
    cr ." pa <domain id> <action id> - Print Action."
    cr ." cds <domain ID> <state> - Change Domain current State, to an arbitrary value."
    cr ." psd <domain ID> <action ID> - Print Square Detail, for a given domain/action."
    cr ." scs <domain id> <action id> - Sample the Current State of a domain, with an action."
    cr ." sas <domain id> <action id> <state> - Sample an Arbitrary State. Change domain current state, then sample with an action."
    cr ." dn <number> - Do Need number."
    cr ." mu - Display Memory Use."
    cr ." tos <domain ID> <state> - TO domain State, from the current state, to an arbitrary value, by finding and executing a plan."
    cr ." to - Change all domain states, like: to (r0X00 r000X1), leading zeros are required"
    cr
    cr ." <state> will be like: s0101, leading zeros are required."
    cr

    \ Display the prompt.
    cr
    s" Enter command: > "       \ sess0 c-addr c
    type                        \ sess0

    \ Get string input from user.
    get-string-from-user        \ sess0 c-addr u

    \ Check for no input.
    dup 0=
    if
        2drop
        session-do-zero-token-command
        true
    else
        cr
        list-from-string                                        \ sess0, lst' t | f
        if
            2dup swap                                           \ sess0 lst' lst' sess0
            session-eval-user-input                             \ sess0 lst' bool
            swap                                                \ sess0 bool lst'
            struct-list-deallocate                              \ sess0 bool
            nip                                                 \ bool
        else
            cr ." Did not understand the command." cr
            drop
            true
        then
    then
;

\ Arbitrarily set the domain current states.
: session-set-current-states ( stac1 sess0 -- )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-statecorr? )
    \ cr ." session-set-current-states: start: " over .statecorr cr

    \ Get states list to traverse.
    swap statecorr-get-list list-get-links swap \ sta-lnk sess0

    \ Get domain list to traverse.
    session-get-domains list-get-links          \ sta-lnk dom-lnk

    begin
        ?dup
    while
        \ Set one domain's current state.
        over link-get-data          \ sta-lnk dom-lnk stax
        over link-get-data          \ sta-lnk dom-lnk stax domx
        domain-update-current-state \ sta-lnk dom-lnk

        \ Prep for next cycle.
        link-get-next swap
        link-get-next swap
    repeat
                                    \ sta-lnk
    drop
;

\ Change states, given statecorr-to.
\ Return true if the change was made.
: session-make-it-so ( stac-to1 sess0 -- bool ) \ ref to Star Trek Next Generation.
    \ Check args.
    assert( tos is-session? )
    assert( nos is-statecorr? )

    \ Get current states.
    dup
    session-get-current-states                  \ stac-to1 sess0 stac-from'
    swap                                        \ stac-to1 stac-from' sess0

    \ Find path instance to use.
    #2 pick #2 pick #2 pick                     \ stac-to1 stac-from' sess0 stac-to1 stac-from' sess0
    session-find-path-instance                  \ stac-to1 stac-from' sess0 pth

    \ Find path.
    #3 pick #3 pick #2 pick                     \ stac-to1 stac-from' sess0 pth stac-to1 stac-from' pth
    path-get-traverse-list                      \ stac-to1 stac-from' sess0 pth stac-to1 stac-from' regc-trvl
    regioncorr-list-find-path                   \ stac-to1 stac-from' sess0 pth, pth-from-to' t | f
    if
        \ cr ." path found: " dup .statecorr-list cr
    else
        \ cr ." path not found" cr
        2drop                                   \ stac-to1 stac-from'
        statecorr-list-deallocate               \ stac-to1
        drop
        false
        exit
    then

    \ Make plan.
    dup                                         \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to'
    #2 pick path-get-avoid-list                 \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to' avd-lst
    #4 pick                                     \ stac-to1 stac-from' sess0 pth pth-from-to' pth-from-to' avd-lst sess0
    session-make-plan                           \ stac-to1 stac-from' sess0 pth pth-from-to', pln' t | f
    if
        \ cr ." plan found: " dup .plan cr
        swap statecorr-list-deallocate          \ stac-to1 stac-from' sess0 pth pln'
    else
        \ cr ." plan not found" cr
        statecorr-list-deallocate               \ stac-to1 stac-from' sess0 pth
        2drop                                   \ stac-to1 stac-from'
        statecorr-deallocate                    \ stac-to1
        drop
        false
        exit
    then

    \ Run plan.
    dup                                         \ stac-to1 stac-from' sess0 pth pln' pln'
    #3 pick                                     \ stac-to1 stac-from' sess0 pth pln' pln' sess0
    session-run-plan                            \ stac-to1 stac-from' sess0 pth pln' bool
    if
        cr ." Plan succeeded" cr
        plan-deallocate                         \ stac-to1 stac-from' sess0 pth
        2drop                                   \ stac-to1 stac-from'
        statecorr-deallocate                    \ stac-to1
        drop
        true
    else
        cr ." Plan failed." cr
        plan-deallocate                         \ stac-to1 stac-from' sess0 pth
        2drop                                   \ stac-to1 stac-from'
        statecorr-deallocate                    \ stac-to1
        drop
        false
    then
;
