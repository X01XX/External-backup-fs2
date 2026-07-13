\ Implement a Session struct and functions.

#31319 constant session-struct-id
    #2 constant session-struct-number-cells

\ Struct fields
0                                               constant session-header-disp                        \ 16-bits [0] struct id [1] use count:w

session-header-disp                     cell+   constant session-domains-disp                       \ A domain-list, kind of like senses.


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
        session-id =                \ bool
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

\ End accessors.

\ Return an regc of max domain regions.
: session-calc-max-regions ( sess0 -- regioncorr )

    \ Get domain-list.
    dup session-get-domains         \ sess0 dom-lst

    \ Init return list.
    list-new swap                   \ sess0 reg-lst dom-lst

    \ Prep for loop.
    list-get-links                  \ sess0 reg-lst d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ sess0 reg-lst d-link domx
        #3 pick                     \ sess0 reg-lst d-link domx sess0
        session-set-current-domain  \ sess0 reg-lst d-link

        \ Add next region.
        dup link-get-data           \ sess0 reg-lst d-lisk domx
        domain-get-max-region       \ sess0 reg-lst d-lisk max-reg
        #2 pick                     \ sess0 reg-lst d-lisk max-reg reg-lst
        region-list-push-end        \ sess0 reg-lst d-lisk

        link-get-next               \ sess0 reg-lst d-link
    repeat
                                    \ sess0 reg-lst
    nip                             \ reg-lst
    regioncorr-new
;

' session-calc-max-regions to session-calc-max-regions-xt

\ Create a session instance.
: session-new ( -- sess ) \ new session pushed onto session stack.

    structinfo-list-store structinfo-list-project-deallocated-xt execute

    \ cr ." session-new: start " .s cr
    \ Allocate space.
    session-id session-mma
    struct-allocate                 \ ses

    \ Set domains list.
    list-new                        \ ses lst
    over _session-set-domains       \ ses

    dup to current-session-store
;

\ Print a session.
: .session ( sess0 -- )
    \ Check arg.
    assert( tos is-session? )

    cr ." Sess: "
    dup session-get-domains
    dup list-get-length
    ."  num domains: " dec.
    ." domains "

                                                \ sess0 dom-lst
    list-get-links                              \ sess0 link
    begin
        ?dup
    while
        dup link-get-data                       \ sess0 link dom

        \ Print domain
        .domain

        link-get-next                           \ sess0 link
    repeat

    drop
;

: session-deallocate ( sess0 -- ) \ Deallocate a session.
    \ Check arg.
    assert( tos is-session? )

    \ Clear fields.
    dup session-get-domains domain-list-deallocate

    \ Deallocate session.
    session-mma mma-deallocate

    0 to current-session-store

    structinfo-list-store structinfo-list-project-deallocated-xt execute
;

\ Return a list of states, one for each domain, in domain list order.
: session-get-current-states ( sess0 -- sta-corr-lst )
    \ Check args.
    assert( tos is-session? )

    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 sta-lst dom-lst

    list-get-links                  \ cur-dom sess0 sta-lst link

    begin
        ?dup
    while
        dup link-get-data           \ cur-dom sess0 sta-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ cur-dom sess0 sta-lst link stax
        #2 pick                     \ cur-dom sess0 sta-lst link stax sta-lst
        list-push-end               \ cur-dom sess0 sta-lst link

        link-get-next               \ cur-dom sess0 sta-lst link
    repeat
                                    \ cur-dom sess0 sta-lst

    \ Restore original current domain.
    -rot                            \ sta-lst cur-dom sess0
    session-set-current-domain      \ sta-lst
;

: session-get-current-regions ( sess0 -- regcorr )  \ Return a list of regions, one for each domain state, in domain list order.
    \ Check args.
    assert( tos is-session? )

    \ Save current domain.
    dup session-get-current-domain  \ sess0 cur-dom
    swap                            \ cur-dom sess0

    \ Init return list.
    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 reg-lst dom-lst

    list-get-links                  \ cur-dom sess0 reg-lst link

    begin
        ?dup
    while
        dup link-get-data           \  sess0 reg-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ cur-dom sess0 reg-lst link stax
        dup region-new              \ cur-dom sess0 reg-lst link regx
        #2 pick                     \ cur-dom sess0 reg-lst link regx reg-lst
        list-push-end               \ cur-dom sess0 reg-lst link

        link-get-next               \ cur-dom sess0 reg-lst link
    repeat
                                    \ cur-dom sess0 reg-lst
    \ Restore original current domain.
    -rot                            \ reg-lst cur-dom sess0
    session-set-current-domain      \ reg-lst

    regioncorr-new
;

: .session-current-states ( sess0 -- )  \ Print a list of current states.
    \ Check args.
    assert( tos is-session? )

    \ Save current domain.
    dup session-get-current-domain  \ sess0 cur-dom
    swap                            \ cur-dom sess0

    dup session-get-domains         \ cur-dom sess0 dom-lst
    list-get-links                  \ cur-dom sess0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ cur-dom sess0 d-link domx
        #2 pick                     \ cur-dom sess0 d-link domx sess0
        session-set-current-domain  \ cur-dom sess0 d-link

        dup link-get-data           \ cur-dom sess0 d-link domx
        domain-get-current-state    \ cur-dom sess0 d-link d-sta
        .value                      \ cur-dom sess0 d-link

        link-get-next               \ cur-dom sess0 d-link-nxt
        dup 0<> if
            space
        then
    repeat
                                    \ cur-dom sess0
    \ Restore original current domain.
    session-set-current-domain      \
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
    tuck swap                   \ dom dom sess0
    session-set-current-domain  \ dom
    true
;

: session-add-domain ( dom1 sess0 -- )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-domain? )
    \ cr ." session-add-domain: start " .stack-gbl execute cr

    \ Add domain
    2dup                                \ dom1 sess0 dom1 sess0
    session-get-domains                 \ dom1 sess0 dom1 dom-lst
    domain-list-push-end                \ dom1 sess0

    \ Set current-domain, if it is zero/invalid.
    tuck session-set-current-domain     \ sess0

    session-process-regioncorrrates     \ To get rate 0, max region regc.
;

\ Return the numebr of domains.
: session-get-number-domains ( sess0 -- u )
    \ Check arg.
    assert( tos is-session? )

    session-get-domains
    list-get-length
;

' session-get-number-domains to session-get-number-domains-xt

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: session-eval-user-input ( cmd-lst1 sess0 -- bool )
    \ Check args.
    assert( tos is-session? )
    assert( nos is-list? )

    \ Check for no tokens
    over list-is-empty?                 \ cmd-lst1 sess0 bool
    if
        nip                             \ sess0
        session-do-zero-token-command   \ bool
        exit
    then

    \ Check command.
    over list-get-first-item            \ cmd-lst1 sess0 tkn0

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
            cr ." ps command: invalid number of arguments" cr
        then
        true
        exit
    then

    cr ." Did not understand the command" cr
    drop 2drop                          \
    true
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
    dup session-set-all-needs   \ sess0
    dup session-get-needs       \ sess0 ned-lst
    dup list-get-length         \ sess0 ned-lst len
    dup 0=
    if
        cr ." Needs: No needs found" cr
        2drop
    else
        drop
        cr ." Needs:" cr .need-list cr  \ sess0
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
    cr
    list-from-string-xt execute                             \ sess0, lst' t | f
    if
        2dup swap                                           \ sess0 lst' lst' sess0
        session-eval-user-input                             \ sess0 lst' bool
        swap                                                \ sess0 bool lst'
        structinfo-list-deallocate-struct-list-xt execute   \ sess0 bool
        nip                                                 \ bool
    else
        cr ." Did not understand input string as a list."
        drop
        true
    then
;
