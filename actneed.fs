\ Implement a need struct and functions for an action need.

#19717 constant actneed-struct-id
    #3 constant actneed-struct-number-cells

\ Struct fields
0                               constant actneed-header-disp    \ 16 bits' [0] struct id, [1] use count, [2] Type (8 bits).
                                                                \ Action instance id (8 bits), Domain instance ID ( 8 bits ).
actneed-header-disp    cell+   constant actneed-target-disp     \ A state.
actneed-target-disp    cell+   constant actneed-info-disp       \ Zero, or a Token struct, depending on actneed type.

0 value actneed-mma \ Storage for actneed mma instance.

\ actneed type values and names.
 1 value actneed-type-min
 1 value actneed-type-snig      \ State not in the s-region of any group.
#2 value actneed-type-cls       \ Confirm logical structure.
#3 value actneed-type-ils       \ Improve logical structure.
#4 value actneed-type-cg        \ Confirm group.
#5 value actneed-type-cas       \ Corner anchor state.
#6 value actneed-type-cds       \ Corner adjacent, dissimilar, state.
#6 value actneed-type-max

\ Return true if tos is a valid actneed type.
: is-actneed-type? ( u -- bool )
    dup  actneed-type-min >=    \ u bool
    swap actneed-type-max <=   \ bool bool
    and
;

\ Init actneed mma, return the addr of allocated memory.
: actneed-mma-init ( num-items -- ) \ sets actneed-mma.
    dup 1 <
    abort" actneed-mma-init: Invalid number of items."

    cr ." Initializing ActNeed store."
    actneed-struct-number-cells swap mma-new to actneed-mma
;

\ Check if tos is an allocated actneed.
: is-actneed? ( tos -- bool )
    dup actneed-mma mma-is-item?    \ tos bool
    if
        struct-get-id
        actneed-struct-id =         \ bool
    else
        drop
        false
    then
;

\ Start accessors.

\ Return the actneed domain id.
: actneed-get-dom-inst-id ( actned0 -- id )
    \ Check arg.
    assert( tos is-actneed? )

    6c@                 \ Fetch the field.
;

\ Set the actneed domain id.
: actneed-set-dom-inst-id ( id1 actned0 -- )
    6c!
;

\ Return the action instance id.
: actneed-get-act-inst-id ( actned0 -- act )
    \ Check arg.
    assert( tos is-actneed? )

    5c@                 \ Fetch the field.
;

\ Set the action instance id, use only in this file.
: actneed-set-act-inst-id ( id1 actned0 -- )
    \ Check args.

    5c!                 \ Set the field.
;

\ Return the target field from a actneed instance.
: actneed-get-target ( actned0 -- targ )
    \ Check arg.
    assert( tos is-actneed? )

    actneed-target-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return true if tos is a valid target.
: is-target? ( arg -- bool )
    dup is-state?
    if drop true exit then

    is-region-list?
;

\ Set the actneed target field.
: _actneed-set-target ( targ1 ned0 -- )

    actneed-target-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Get actneed type.
: actneed-get-type ( ned0 -- type )
    \ Check arg.
    assert( tos is-actneed? )

    4c@
;

\ Set actneed type.
: _actneed-set-type ( typ1 ned0 -- )
    4c!
;

\ Return actneed info field.
: actneed-get-info ( ned -- tkn | 0 )
    \ Check arg.
    assert( tos is-actneed? )

    actneed-info-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set actneed info field, may be zero or token instance.
: _actneed-set-info ( tkn ned -- )
    actneed-info-disp +
    over 0=
    if
        !
    else
        !struct
    then
;

\ End accessors.

\ Create a actneed given:
\ A Domain instance id.
\ An action instance id.
\ A need type.
\ A target, a state.
\ A token for extra info, may be zero.
\
: actneed-new ( tkn4 targ3 typ2 act-id1 dom-id0 -- addr)
    \ Check args.
    assert( tos is-valid-inst-id? )
    assert( nos is-valid-inst-id? )
    assert( 3os is-actneed-type? )
    assert( 4os is-state? )
    assert( 5os 0= if true else 5os is-token? then )

    \ Allocate instance.
    actneed-struct-id actneed-mma
    struct-allocate                 \  tkn4 targ3 typ2 act-id1 dom-id0 ned

    \ Store fields.
    tuck actneed-set-dom-inst-id    \ tkn4 targ3 typ2 act-id1 ned
    tuck actneed-set-act-inst-id    \ tkn4 targ3 typ2 ned
    tuck _actneed-set-type          \ tkn4 targ3 ned
    tuck _actneed-set-target        \ tkn4
    tuck _actneed-set-info          \ ned
;

: .actneed-dom-act-targ ( actned -- )
    ." Dom: " dup actneed-get-dom-inst-id #2 dec.r space
    ." Act: " dup actneed-get-act-inst-id #2 dec.r space
    ." Target: " actneed-get-target .state
;

\ Print an actneed.
: .actneed ( actned0 -- )
    \ Check arg.
    assert( tos is-actneed? )

    ." ("
    dup .actneed-dom-act-targ

    dup actneed-get-type
    case
        actneed-type-snig of
            space ." State not covered by group"
        endof
        actneed-type-cls of
            space ." Confirm incompatible pair "
            actneed-get-info .token
            \ region-get-states .state space ." and " .state
        endof
        actneed-type-ils of
            space ." Get sample between incompatible pair "
            actneed-get-info .token
            \ region-get-states .state space ." and " .state
        endof
        actneed-type-cg of
            space ." Confirm group "
            actneed-get-info .token
            \ .region
        endof
        actneed-type-cas of
            space ." Corner confirm anchor square "
            actneed-get-info .token
            \ .corner
        endof
        actneed-type-cds of
            space ." Corner confirm dissimilar square "
            actneed-get-info .token
            \ .corner
        endof
        ." Unrecognized type value" abort
    endcase
                    \ actned0
    drop
    ." )"
;

\ Deallocate a need.
: actneed-deallocate ( actned0 -- )
    \ Check arg.
    assert( tos is-actneed? )

    dup struct-get-use-count      \ ned0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        dup actneed-get-target state-deallocate
        dup actneed-get-info 0> if dup actneed-get-info token-deallocate then

        \ Deallocate instance.
        actneed-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a satte satisfies a need.
: actneed-satisfied-by? ( sta1 actned0 -- bool )
    \ Check arg.
    assert( tos is-actneed? )
    assert( nos is-state? )

    actneed-get-target          \ sta1 n-sta

    states-eq?
;
