\ Implement a Session struct and functions.

#31319 constant session-struct-id
    #6 constant session-struct-number-cells

\ Struct fields
0                                       constant session-header-disp                \ 16-bits [0] struct id [1] use count
session-header-disp             cell+   constant session-domains-disp               \ A domain-list, kind of like senses.
session-domains-disp            cell+   constant session-step-num-disp              \ Starts at zero.
session-step-num-disp           cell+   constant session-max-regions-disp           \ A regioncorr list of maximum regions, corresponding in order to the domain list.

session-max-regions-disp        cell+   constant session-valued-regioncorrs-disp    \ A valued regioncorr list, added during session definition.

session-valued-regioncorrs-disp cell+   constant session-avoid-lol-disp             \ A list-of-lists of negative valued regioncorrs to avoid, calculated from the
                                                                                    \ session-valued-regioncorrs.
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

: _session-set-max-regions ( regc-lst sess0 -- ) \ Set the max regions list.
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

: session-get-avoid-lol ( sess0 -- avd-lst )  \ Return the avoid list-of-lists.
    \ Check arg.
    assert( tos is-session? )

    session-avoid-lol-disp +    \ Add offset.
    @                           \ Fetch the field.
;

: _session-set-avoid-lol ( avd-lst1 sess0 -- ) \ Set the avoid lists-of-lists field.
    \ Check arg.
    assert( tos is-session? )
    assert( nos is-region-list? )

    session-avoid-lol-disp +    \ Add offset.
    !struct                     \ Set the field.
;

: session-inc-step-num ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )

    session-step-num-disp + \ Add offset.
    1 swap +!               \ Add one to the field.
;

\ Return a regioncorr of max domain regions.
: session-calc-max-regions ( sess0 -- max-regs )

    \ Get domain-list.
    dup session-get-domains         \ sess0 dom-lst

    \ Init return list.
    list-new swap                   \ sess0 reg-lst dom-lst

    foreach                         \ sess0 reg-lst d-link domx
        domain-get-max-region       \ sess0 reg-lst d-link max-reg
        #2 pick                     \ sess0 reg-lst d-link max-reg reg-lst
        region-list-push-end        \ sess0 reg-lst d-link
    next
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
    next
                                            \ sess0
    drop
;

\ End accessors.

\ Process the valued-regioncorr list.
: _session-process-valued-regioncorrs ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )
\ test with no regioncorrs.

    \ Check if no valued regioncorrs.
    dup session-get-valued-regioncorrs      \ sess0 regc-lst
    list-get-length                         \ sess0 num
    0= if
        \ Push avoid-nothing list.
        list-new swap                       \ lst sess0
        session-get-avoid-lol               \ lst avoid-lst
        list-push-struct
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
        list-new swap                       \ lst sess0
        session-get-avoid-lol               \ lst avoid-lst
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
    next

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
        #5 pick                             \ sess0 spl-lst' val-lst' avoid-lol' val-lnk avoid-lst2' sess0
        session-get-avoid-lol               \ sess0 spl-lst' val-lst' avoid-lol' val-lnk avoid-lst2' lol
        list-push-struct                    \ sess0 spl-lst' val-lst' avoid-lol' val-lnk
    next

    \ Clean up.                             \ sess0 spl-lst' val-lst' avoid-lol'
    regioncorr-list-deallocate              \ sess0 spl-lst' val-lst'
    list-deallocate                         \ sess0 spl-lst'
    regioncorr-list-deallocate              \ sess0

    \ Push avoid-nothing list.
    list-new swap                           \ lst sess0
    session-get-avoid-lol                   \ lst avoid-lst
    list-push-end-struct
;

\ Create a session instance, given a valued-regioncorr list, and a domain-list.
: session-new ( vregc-lst1 dom-lst0 -- sess ) \ new session pushed onto session stack.
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
    list-new over _session-set-avoid-lol

    dup _session-process-valued-regioncorrs

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

    dup session-get-avoid-lol
    dup list-get-length
    0=
    if
        cr ." Avoid list-of-lists: None"
        drop
    else
        cr ." Avoid list-of-lists: ("
        foreach
            .regioncorr-list
            link-get-next
            dup 0<>
            if
                cr #22 spaces
            then
        repeat
        ." )"
    then

    dup session-get-domains
                                                \ sess0 dom-lst
    foreach                                     \ sess0 dom-lnk dom
        \ Print domain
        .domain
    next

    drop
;

: session-deallocate ( sess0 -- ) \ Deallocate a session.
    \ Check arg.
    assert( tos is-session? )

    \ Clear fields.
    dup session-get-domains domain-list-deallocate
    dup session-get-max-regions regioncorr-deallocate
    dup session-get-valued-regioncorrs regioncorr-list-deallocate
    dup session-get-avoid-lol struct-list-deallocate

    \ Deallocate session.
    session-mma mma-deallocate

    0 to session-store
;

\ Return a list of states, one for each domain, in domain list order.
: session-get-current-states ( sess0 -- sta-corr-lst )
    \ Check args.
    assert( tos is-session? )

    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 sta-lst dom-lst

    foreach                         \ cur-dom sess0 sta-lst link dom
        domain-get-current-state    \ cur-dom sess0 sta-lst link stax
        #2 pick                     \ cur-dom sess0 sta-lst link stax sta-lst
        list-push-end               \ cur-dom sess0 sta-lst link
    next
                                    \ cur-dom sess0 sta-lst

    nip nip                         \ sta-lst
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
    next
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

\ Return the numebr of domains.
: session-get-number-domains ( sess0 -- u )
    \ Check arg.
    assert( tos is-session? )

    session-get-domains
    list-get-length
;

' session-get-number-domains to session-get-number-domains-xt

: session-do-zero-token-command ( sess -- bool )
    \ Check args.
    assert( tos is-session? )
    \ cr ." session-do-zero-token-command" cr

    session-inc-step-num
;

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: session-eval-user-input ( cmd-lst1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-list? )
    cr ." session-eval-user-input: start " over .token-list cr

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

\ dn, cds?, scs, sas?, tos, to: remember to do session-inc-step-num

    cr ." Did not understand the command." cr
    drop 2drop                          \
    \ cr ." session-eval-user-input: end: 4" cr
    true
;

: session-set-all-needs

;

: session-get-needs

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
    \ dup session-set-all-needs   \ sess0
    \ dup session-get-needs       \ sess0 ned-lst
    \ dup list-is-empty?          \ sess0 ned-lst bool
    true
    if
        cr ." Needs: No needs found" cr
        \ 2drop
    else
        \ drop
        \ cr ." Needs:" cr .need-list cr  \ sess0
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
    cr ." to - Change all domain states, like: to (r0X00 r000X1). Leading zeros are not required."
    cr
    cr ." <state> will usually be like: %0101, leading zeros can be ommitted."
    cr

    \ Display the prompt.
    cr
    s" Enter command: > "       \ sess0 c-addr c
    type                        \ sess0
    \ Get chars, leaves num chars on TOS.
    pad                         \ sess0 p-addr
    dup                         \ sess0 p-addr p-addr
    #80                         \ sess0 p-addr p-addr #80
    accept                      \ sess0 pad-add n

    \ Check for no input.
    dup 0=
    if
        2drop
        dup session-do-zero-token-command
    else
        cr
        list-from-string-xt execute                             \ sess0, lst' t | f
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


