\ Implement an Action struct and functions.

#29717 constant action-struct-id
   #15 constant action-struct-number-cells

\ Struct fields
0                                                   constant action-header-disp                         \ 16 bits, [0] Struct id, [1] Use count [2] Number bits ( 8 bits )
                                                                                                        \ Action instance ID ( 8 bits ) [3] Domain instance ID.
action-header-disp                          cell+   constant action-max-region-disp                     \ Maximum region.

action-max-region-disp                      cell+   constant action-squares-disp                        \ A square list.

action-squares-disp                         cell+   constant action-adj-pairs-disp                      \ A region list.
                                                                                                        \ Adjacent, incompatible, states, define the regions.
action-adj-pairs-disp                       cell+   constant action-adj-regions-disp                    \ A region list.
                                                                                                        \ By calculating ~A + ~B from adj pairs.

action-adj-regions-disp                     cell+   constant action-nadj-pairs-disp                     \ A region list.
                                                                                                        \ Not Adjacent, Incompatible, states, define the regions.
                                                                                                        \ Both states of a region are within at least one region
                                                                                                        \ in action-ai-regions.
action-nadj-pairs-disp                      cell+   constant action-nadj-regions-disp                   \ A region list.
                                                                                                        \ By calculating ~A + ~B from nadj pairs.

action-nadj-regions-disp                    cell+   constant action-possible-regions-disp               \ A region list.
                                                                                                        \ By intersecting action-ai-regions and action-nai-regions

action-possible-regions-disp                cell+   constant action-states-in-one-region-disp           \ A state list of square states in only one possible region,
                                                                                                        \ possible anchors.

action-states-in-one-region-disp            cell+   constant action-defining-regions-disp               \ A list of possible regions that are defining regions.
                                                                                                        \ Developed from action-states-in-one-region,
                                                                                                        \ instead of doing a lot of region subtractions.

action-defining-regions-disp                cell+   constant action-states-not-in-defining-regions-disp \ A state list of states not in defining regions.

action-states-not-in-defining-regions-disp  cell+   constant action-corners-disp                        \ A cornor list, from incompatible pairs and possible regions.
action-corners-disp                         cell+   constant action-corner-clusters-disp                \ A list of corner clusters, a list of lists of corners.
action-corner-clusters-disp                 cell+   constant action-groups-disp                         \ A group list.

action-groups-disp                          cell+   constant action-function-disp                       \ A function to run to get a sample for a state.


0 value action-mma \ Storage for action mma instance.

\ Init action mma, return the addr of allocated memory.
: action-mma-init ( num-items -- ) \ sets action-mma.
    dup 1 <
    abort" action-mma-init: Invalid number of items."

    cr ." Initializing Action store."
    action-struct-number-cells swap mma-new to action-mma
;

\ Check if tos is an allocated action.
: is-action? ( addr -- bool )
    dup action-mma mma-is-item? \ addr bool
    if
        struct-get-id
        action-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

' is-action? to is-action?-xt

\ Start accessors.

\ Return the action domain id.
: action-get-dom-inst-id ( act0 -- id )
    \ Check arg.
    assert( tos is-action? )

    6c@                 \ Fetch the field.
;

\ Set the action domain id.
: _action-set-dom-inst-id ( id1 act0 -- )
    6c!
;

\ Get the action id.
: action-get-inst-id ( act0 -- id )
    \ Check arg.
    assert( tos is-action? )

    5c@
;

' action-get-inst-id to action-get-inst-id-xt

\ Set the action id.
: action-set-inst-id ( id act0 -- )
    5c!
;

\ Return the maximum region from an action instance.
: action-get-max-region ( act0 -- max-reg )
    \ Check arg.
    assert( tos is-action? )

    action-max-region-disp +    \ Add offset.
    @                           \ Fetch the field.
;

\ Set the maximum region of an action instance.
: _action-set-max-region ( max-reg1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )

    action-max-region-disp +    \ Add offset.
    !struct                     \ Set the field.
;

\ Return the square-list from an action instance.
: action-get-squares ( act0 -- sqr-lst )
    \ Check arg.
    assert( tos is-action? )

    action-squares-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square-list of an action instance, use only in this file.
: _action-set-squares ( sqr-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square-list? )

    action-squares-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the adjacent incompatible pairs list from an action instance.
: action-get-adj-pairs ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-adj-pairs-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the adjacent incompatible pairs list of an action instance, use only in this file.
: _action-set-adj-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-adj-pairs-disp + \ Add offset.
    !struct                 \ Set the field.
;

\ Return the non-adjacent incompatible pairs list from an action instance.
: action-get-nadj-pairs ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-nadj-pairs-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the non-adjacent incompatible pairs list of an action instance, use only in this file.
: _action-set-nadj-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-nadj-pairs-disp + \ Add offset.
    !struct                 \ Set the field.
;

\ Return the adjacent regions list from an action instance.
: action-get-adj-regions ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-adj-regions-disp +   \ Add offset.
    @                           \ Fetch the field.
;

\ Set the adjacent regions list of an action instance, use only in this file.
: _action-set-adj-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-adj-regions-disp +   \ Add offset.
    !struct                     \ Set the field.
;

\ Return the non-adjacent regions list from an action instance.
: action-get-nadj-regions ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-nadj-regions-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the nnon-adjacent regions list of an action instance, use only in this file.
: _action-set-nadj-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-nadj-regions-disp +  \ Add offset.
    !struct                     \ Set the field.
;

\ Return the possible-regions list from an action instance.
: action-get-possible-regions ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-possible-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the possible-regions list of an action instance, use only in this file.
: _action-set-possible-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-possible-regions-disp +  \ Add offset.
    !struct                         \ Set the field.
;

\ Return the states-in-one-region list from an action instance.
: action-get-states-in-one-region ( act0 -- sta-lst )
    \ Check arg.
    assert( tos is-action? )

    action-states-in-one-region-disp +  \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the states-in-one-region list of an action instance, use only in this file.
: _action-set-states-in-one-region ( sta-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state-list? )

    action-states-in-one-region-disp +  \ Add offset.
    !struct                             \ Set the field.
;

\ Return the defining-regions list from an action instance.
: action-get-defining-regions ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-defining-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the defining-regions list of an action instance, use only in this file.
: _action-set-defining-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-defining-regions-disp +  \ Add offset.
    !struct                         \ Set the field.
;

\ Return the states-not-in-defining-regions list from an action instance.
: action-get-states-not-in-defining-regions ( act0 -- sta-lst )
    \ Check arg.
    assert( tos is-action? )

    action-states-not-in-defining-regions-disp +    \ Add offset.
    @                                               \ Fetch the field.
;

\ Set the states-not-in-defining-regions list of an action instance, use only in this file.
: _action-set-states-not-in-defining-regions ( sta-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state-list? )

    action-states-not-in-defining-regions-disp +    \ Add offset.
    !struct                                         \ Set the field.
;

\ Return the group-list from an action instance.
: action-get-groups ( act0 -- grp-lst )
    \ Check arg.
    assert( tos is-action? )

    action-groups-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the group-list of an action instance, use only in this file.
: _action-set-groups ( grp-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-group-list? )

    action-groups-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return the function xt that implements the action.
: action-get-function ( act0 -- xt )
    \ Check arg.
    assert( tos is-action? )

    action-function-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the function xt that implements an action.
: _action-set-function ( xt act0 -- )
    \ Check args.
    assert( tos is-action? )

    action-function-disp +  \ Add offset.
    !                       \ Set the field.
;

\ Return the corner-list from an action instance.
: action-get-corners ( act0 -- crn-lst )
    \ Check arg.
    assert( tos is-action? )

    action-corners-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the corner-list of an action instance, use only in this file.
: _action-set-corners ( crn-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner-list? )

    action-corners-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the corner-cluster-list from an action instance.
: action-get-corner-clusters ( act0 -- crn-lol )
    \ Check arg.
    assert( tos is-action? )

    action-corner-clusters-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the corner-cluster-list of an action instance, use only in this file.
: _action-set-corner-clusters ( crn-lol1 act0 -- )
    \ Check args.
    \ cr ." _action-set-corner-clusters: start: " .stack cr
    assert( tos is-action? )
    assert( nos is-corner-lol? )

    action-corner-clusters-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ End accessors

\ Start update functions.

\ Update possible regions from adjacent incompatible pairs.
: _action-update-adj-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )
    \ cr ." _action-update-adj-regions: " over .region-list cr

    dup action-get-adj-regions          \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-adj-regions             \ pos-regs
    region-list-deallocate
;

\ Update possible regions from non-adjacent incompatible pairs.
: _action-update-nadj-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )
    \ cr ." _action-update-nadj-regions: " over .region-list cr

    dup action-get-nadj-regions         \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-nadj-regions            \ pos-regs
    region-list-deallocate
;

\ Update the possible-regions list of an action instance, use only in this file.
: _action-update-possible-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )
    \ cr ." _action-update-possible-regions " over .region-list cr

    dup action-get-possible-regions     \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-possible-regions        \ pos-regs
    region-list-deallocate
;

\ Update the corner-cluster list of an action instance, use only in this file.
: _action-update-corner-clusters ( crn-lol1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner-lol? )

    dup action-get-corner-clusters  \ crn-lol1 act0 crn-lol
    -rot                            \ crn-lol crn-lol1 act0
    _action-set-corner-clusters     \ crn-lol
    corner-lol-deallocate
;

\ Update the states-not-in-defining-regions list of an action instance, use only in this file.
: _action-update-states-not-in-defining-regions ( sta-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state-list? )

    dup action-get-states-not-in-defining-regions   \ reg-lst1 act0 pos-regs
    -rot                                            \ pos-regs reg-lst1 act0
    _action-set-states-not-in-defining-regions      \ pos-regs
    state-list-deallocate
;

\ Update the defining-regions list of an action instance, use only in this file.
: _action-update-defining-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    dup action-get-defining-regions     \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-defining-regions        \ pos-regs
    region-list-deallocate
;

\ Update the corner list of an action instance, use only in this file.
: _action-update-corners ( crn-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner-list? )

    dup action-get-corners  \ crn-lst1 act0 crn-lst
    -rot                    \ crn-lst crn-lst1 act0
    _action-set-corners     \ crn-lst
    corner-list-deallocate
;

\ Update the states-in-one-region list of an action instance, use only in this file.
: _action-update-states-in-one-region ( sta-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state-list? )

    dup action-get-states-in-one-region \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-states-in-one-region    \ pos-regs
    state-list-deallocate
;

\ End update functions.

\ Get the number of bits.
: action-get-num-bits ( act0 -- nb )
    \ Check arg.
    assert( tos is-action? )

    action-get-max-region
    region-get-num-bits
;

\ Return a new action, given a function to run to get a sample,
\ and the number of bits being used.
: action-new ( xt2 max-region1 dom-id0 -- addr)
    \ cr ." action-new: start: " .stack cr
    assert( tos is-valid-inst-id? )
    assert( nos is-region? )
    assert( nos region-all-x? )

    \ Allocate instance.
    action-struct-id action-mma         \ xt2 mr1 dom-id0 struct-id mma
    struct-allocate                     \ xt2 mr1 dom-id0 act

    \ Set parent domain intance id.
    tuck _action-set-dom-inst-id        \ xt2 mr1 act

    \ Set action instance id.
    0 over action-set-inst-id           \ xt2 mr1 act

    \ Init adj regions list.
    list-new                            \ xt2 mr1 act lst
    #2 pick                             \ xt2 mr1 act lst mr1
    over list-push-struct               \ xt2 mr1 act lst
    over                                \ xt2 mr1 act lst act
    _action-set-adj-regions             \ xt2 mr1 act

    \ Init non-adj regions list.
    list-new                            \ xt2 mr1 act lst1
    #2 pick                             \ xt2 mr1 act lst mr1
    over list-push-struct               \ xt2 mr1 act lst
    over                                \ xt2 mr1 act lst act
    _action-set-nadj-regions            \ xt2 mr1 act

    \ Init possible-regions list.
    list-new                            \ xt2 mr1 act lst
    #2 pick                             \ xt2 mr1 act lst mr1
    over list-push-struct               \ xt2 mr1 act lst
    over                                \ xt2 mr1 act lst act
    2dup _action-set-possible-regions   \ xt2 mr1 act
    _action-set-defining-regions        \ xt2

    \ Set maximum region.
    tuck _action-set-max-region         \ xt2 act

    \ Set function.
    tuck _action-set-function           \ act

    \ Init empty list fields.
    list-new over _action-set-squares
    list-new over _action-set-adj-pairs
    list-new over _action-set-nadj-pairs
    list-new over _action-set-groups
    list-new over _action-set-states-in-one-region
    list-new over _action-set-states-not-in-defining-regions
    list-new over _action-set-corners
    list-new over _action-set-corner-clusters
;

: action-square-states-in-one-region ( act0 -- sta-lst )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-square-states-in-one-region: start" cr

    \ Init return list.
    list-new                    \ act0 ret-lst

    \ Prep for loop.
    over action-get-possible-regions    \ act0 ret-lst pos-lst
    #2 pick action-get-squares          \ act0 ret-lst pos-lst sqr-lst

    foreach                             \ act0 ret-lst pos-lst sqr-lnk sqrx
        square-get-state                \ act0 ret-lst pos-lst sqr-lnk sta
        #2 pick                         \ act0 ret-lst pos-lst sqr-lnk sta pos-lst
        region-list-num-state-in        \ act0 ret-lst pos-lst sqr-lnk u
        1 =                             \ act0 ret-lst pos-lst sqr-lnk bool
        if
            dup link-get-data           \ act0 ret-lst pos-lst sqr-lnk sqrx
            square-get-state            \ act0 ret-lst pos-lst sqr-lnk stax
            #3 pick                     \ act0 ret-lst pos-lst sqr-lnk stax ret-lst
            list-push-struct            \ act0 ret-lst pos-lst sqr-lnk
        then
    next-item
                                \ act0 ret-lst pos-lst
    drop nip                    \ ret-lst
    \ cr ." action-square-states-in-one-region: end" cr
;

\ Print parent domain id.
: .action-parent-inst-id ( act0 -- )
   \ Check arg.
    assert( tos is-action? )

    action-get-dom-inst-id
    ." Dom: " dec.
;

\ Print id.
: .action-inst-id ( act0 -- )
   \ Check arg.
    assert( tos is-action? )

    action-get-inst-id
    ." Act: " dec.
;

\ Print both inst ids.
: .action-inst-ids ( act0 -- )
   \ Check arg.
    \ dup is-action?
    \ ifnot
    \     ." ?????"
    \     exit
    \ then
    assert( tos is-action? )

    dup .action-parent-inst-id
    .action-inst-id
;

\ Print a action.
: .action ( act0 -- )
    \ cr ." .action: start: " .stack cr
    \ Check arg.
    assert( tos is-action? )

    cr dup .action-parent-inst-id space dup .action-inst-id
    cr
    s"     Squares:              " #2 pick action-get-squares .square-list-prefix
    cr
    #4 spaces ." Adjacent pairs:       " dup action-get-adj-pairs
    dup list-is-empty?
    if
        drop
        ." None"
    else
        .region-list
    then
    cr
    #4 spaces ." Adj pair regions:     " dup action-get-adj-regions .region-list
    cr cr
    #4 spaces ." Non-adjacent pairs:   " dup action-get-nadj-pairs
    dup list-is-empty?
    if
        drop
        ." None"
    else
        .region-list
    then
    cr
    #4 spaces ." Non-adj pair regions: " dup action-get-nadj-regions .region-list
    cr cr
    #4 spaces ." Possible regions:     " dup action-get-possible-regions .region-list
    cr cr
    #4 spaces ." Sqrs in one poss reg: " dup action-get-states-in-one-region
    dup list-is-empty?
    if
        drop
        ." None"
    else
        .state-list
    then
    cr cr
    #4 spaces ." Defining regions:     " dup action-get-defining-regions
    dup list-is-empty?
    if
        drop
        ." None"
    else
        .region-list
    then
    cr cr
    #4 spaces ." Sqrs not in def regs: " dup action-get-states-not-in-defining-regions
    dup list-is-empty?
    if
        drop
        ." None"
    else
        .state-list
    then
    cr
    s"     Corners:              " #2 pick action-get-corners .corner-list-prefix
    cr
    s"     Corner clusters:      " #2 pick action-get-corner-clusters .corner-clusters-prefix
    cr
    s"     Groups:               " #2 pick action-get-groups .group-list-prefix
    drop
    cr
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." deallocating action: " dup action-get-inst-id dec. cr

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup action-get-max-region region-deallocate
        dup action-get-squares square-list-deallocate
        dup action-get-adj-pairs region-list-deallocate
        dup action-get-adj-regions region-list-deallocate
        dup action-get-nadj-pairs region-list-deallocate
        dup action-get-nadj-regions region-list-deallocate
        dup action-get-possible-regions region-list-deallocate
        dup action-get-groups group-list-deallocate
        dup action-get-states-in-one-region state-list-deallocate
        dup action-get-defining-regions region-list-deallocate
        dup action-get-states-not-in-defining-regions state-list-deallocate
        dup action-get-corners corner-list-deallocate
        dup action-get-corner-clusters corner-lol-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Find a square, given a state.
: action-find-square ( sta1 act0 -- sqr t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    action-get-squares      \ sta1 sqr-lst
    square-list-find        \ sqr t | f
;

\ Return true if a region, in the possible regions list, can be used as a group.
\ This does not check the compatibility of the squares in the group.
: action-valid-group-region2? ( reg1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )
    \ cr ." action-valid-group-region2? start" cr

    \ Check if reg1 is in the defining regions list.
    [ ' regions-eq? ] literal       \ reg1 act0 xt
    #2 pick #2 pick                 \ reg1 act0 xt reg1 act0
    action-get-defining-regions     \ reg1 act0 xt reg1 def-regs
    list-member?                    \ reg1 act0 bool
    if
        2drop
        true
        exit
    then

    \ Check if reg1 minus the defining regions is empty.
    list-new                            \ reg1 act0 reg1-lst'
    #2 pick                             \ reg1 act0 reg1-lst' reg1
    over list-push                      \ reg1 act0 reg1-lst' ( reg1 use count unchanged )
    over action-get-defining-regions    \ reg1 act0 reg1-lst' def-regs
    over region-list-subtract           \ reg1 act0 reg1-lst' rem-lst'

    dup list-is-empty?
    if
        list-deallocate                 \ reg1 act0 reg1-lst'
        list-deallocate                 \ reg1 act0
        2drop
        false
    else
        region-list-deallocate          \ reg1 act0 reg1-lst'
        list-deallocate                 \ reg1 act0
        2drop
        true
    then
;

\ Return true if a region can be used as a group.
\ This does not check the compatibility of the squares in the group.
: action-valid-group-region? ( reg1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )
    \ cr ." action-valid-group-region? start" cr

    \ Check if region is in the possible regions list.
    [ ' regions-eq? ] literal       \ reg1 act0 xt
    #2 pick #2 pick                 \ reg1 act0 xt reg1 act0
    action-get-possible-regions     \ reg1 act0 xt reg1 pos-regs
    list-member?                    \ reg1 act0 bool
    ifnot
        2drop
        false
        exit
    then

    action-valid-group-region2?     \ bool
;

\ Return a list of defining regions in the possible regions.
: action-defining-regions ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    \ Init result list.
    list-new                                \ act0 rslt-lst

    \ Get possible-regions.
    over action-get-possible-regions        \ act0 rslt-lst pos-lst

    \ Get states in one region.
    #2 pick action-get-states-in-one-region \ act0 rslt-lst pos-lst sta-lst

    foreach                                 \ act0 rslt-lst pos-lst sta-lnk stax
        \ Get region state is in.
        #2 pick                             \ act0 rslt-lst pos-lst sta-lnk stax pos-lst
        region-list-state-in                \ act0 rslt-lst pos-lst sta-lnk regs-in'

        \ Check result.
        dup list-get-length                 \ act0 rslt-lst pos-lst sta-lnk regs-in' len
        1 <> abort" state not in exactly one region?"

        \ Check if its already in the list.
        [ ' = ] literal                     \ act0 rslt-lst pos-lst sta-lnk regs-in' xt
        over list-get-first-item            \ act0 rslt-lst pos-lst sta-lnk regs-in' xt regx
        #5 pick                             \ act0 rslt-lst pos-lst sta-lnk regs-in' xt regx rslt-lst
        list-member?                        \ act0 rslt-lst pos-lst sta-lnk regs-in' bool
        ifnot
            \ Add to result list.
            dup list-get-first-item         \ act0 rslt-lst pos-lst sta-lnk regs-in' regx
            #4 pick                         \ act0 rslt-lst pos-lst sta-lnk regs-in' regx rslt-lst
            list-push-struct                \ act0 rslt-lst pos-lst sta-lnk regs-in'
        then
        region-list-deallocate              \ act0 rslt-lst pos-lst sta-lnk
    next-item
                                            \ act0 rslt-lst pos-lst
    drop nip
;

\ Add a group to the group list.
: action-add-group ( grp1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-group? )
    \ cr dup .action-parent-inst-id dup .action-inst-id ." : Adding group: " over .group cr

    action-get-groups   \ act0 grp1 grp-lst
    list-push-struct    \
;

\ Scan the group list to delete groups that have a region
\ that is not in the possible regions list.
: _action-delete-orphaned-groups ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." _action-delete-orphaned-groups: start: " .stack cr

    \ Init group list to delete.
    list-new                            \ act0 del-grps

    \ Scan group list, gathering groups to remove from the list.
    over action-get-groups              \ act0 del-grps grp-lst

    foreach                             \ act0 del-grps grp-lnk grpx
        group-get-region                \ act0 del-grps grp-lnk regx
        #3 pick                         \ act0 del-grps grp-lnk regx act0
        action-valid-group-region?      \ act0 del-grps grp-lnk bool
        ifnot
            dup link-get-data           \ act0 del-grps grp-lnk grpx
            #2 pick                     \ act0 del-grps grp-lnk grpx del-grps
            list-push-struct            \ act0 del-grps grp-lnk
        then
    next-item

    \ Remove the groups from the action group list.
                                        \ act0 del-grps
    dup list-is-empty?
    if
        list-deallocate
        drop
        exit
    then

    over action-get-groups              \ act0 del-grps grps-lst
    over                                \ act0 del-grps grps-lst del-grps

    cr #3 pick .action-inst-ids space ." Deleting orphaned groups: "
    foreach                             \ act0 del-grps grp-lst del-lnk grpx
        space dup group-get-region .region
        #2 pick                         \ act0 del-grps grp-lst del-lnk grpx grp-lst
        group-list-remove               \ act0 del-grps grp-lst del-lnk
    next-item

    cr                                  \ act0 del-grps grp-lst
    drop                                \ act0 del-grps
    group-list-deallocate               \ act0      The groups are deallocated here.
    drop
    \ cr ." _action-delete-orphaned-groups: end: " .stack cr
;

\ Get both action, and domain, instance ids.
: action-get-inst-ids ( act0 -- act-id dom-id )
    assert( tos is-action? )

    dup action-get-inst-id          \ act0 act-id
    swap action-get-dom-inst-id     \ act-id dom-id
;

\ Check square with groups, possibly invalidating and removing some.
: action-udpate-groups-with-new-square ( sqr1 act0 -- )
    \ Check args.group-new failed
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-udpate-groups-with-new-square: start" cr

    \ Init list for groups to be deleted.
    list-new -rot                       \ d-lst' sqr1 act0

    dup action-get-groups               \ d-lst' sqr1 act0 grp-lst

    foreach                             \ d-lst' sqr1 act0 grp-lnk grpx
        \ Check if square is in group.
        #3 pick square-get-state        \ d-lst' sqr1 act0 grp-lnk grpx sta1
        swap group-get-region           \ d-lst' sqr1 act0 grp-lnk sta1 g-reg
        region-superset-of-state?       \ d-lst' sqr1 act0 grp-lnk bool
        if
            \ Add square to group.
            dup link-get-data           \ d-lst' sqr1 act0 grp-lnk grpx
            #3 pick                     \ d-lst' sqr1 act0 grp-lnk grpx sqr1
            swap                        \ d-lst' sqr1 act0 grp-lnk sqr1 grp
            group-add-new-square        \ d-lst' sqr1 act0 grp-lnk bool
            ifnot
                dup link-get-data       \ d-lst' sqr1 act0 grp-lnk grpgroup-new failed
                #4 pick                 \ d-lst' sqr1 act0 grp-lnk grp d-lst'
                list-push-struct        \ d-lst' sqr1 act0 grp-lnk
            then
        then
    next-item
                                        \ d-lst' sqr1 act0
    \ Delete groups that were invalidated.
    nip action-get-groups               \ d-lst' grp-lst
    over                                \ d-lst' grp-lst d-lst'

    foreach                             \ d-lst' grp-lst d-lnk grpx
        [ ' = ] literal swap            \ d-lst' grp-lst d-lnk xt grpx
        #3 pick                         \ d-lst' grp-lst d-lnk xt grpx grp-lst
        list-remove                     \ d-lst' grp-lst d-lnk, grpx t | f
        invert abort" group not found?"
        cr ." Deleting group: " dup group-get-region .region cr
        group-deallocate                \ d-lst' grp-lst d-lnk
    next-item
                                        \ d-lst' grp-lst
    drop                                \ d-lst'
    group-list-deallocate
    \ cr ." action-udpate-groups-with-new-square: end: " .stack cr
;

\ Scan the possible regions list, when a region is not represented in the
\ group list, try to add the group.
: action-add-possible-groups ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-add-possible-groups: start" cr

    \ Scan group list.
    dup action-get-groups               \ act0 grp-lst
    over action-get-possible-regions    \ act0 grp-lst pos-regs

    foreach                             \ act0 grp-lst pos-lnk pos-reg
        \ Check if the square is in the group.
        \ cr ." check pos reg: " dup .region cr
        #2 pick                         \ act0 grp-lst pos-lnk pos-reg grp-lst
        group-list-member?              \ act0 grp-lst pos-lnk bool
        ifnot
            dup link-get-data           \ act0 grp-lst pos-lnk pos-reg
            #3 pick                     \ act0 grp-lst pos-lnk pos-reg act0
            action-valid-group-region2? \ act0 grp-lst pos-lnk bool
            if
                \ Get squares in region.
                dup link-get-data           \ act0 grp-lst pos-lnk pos-reg
                #3 pick                     \ act0 grp-lst pos-lnk pos-reg act0
                action-get-squares          \ act0 grp-lst pos-lnk pos-reg sqr-lst
                square-list-in-region       \ act0 grp-lst pos-lnk in-lst'
                dup list-is-empty?
                if
                    list-deallocate
                else
                    dup                     \ act0 grp-lst pos-lnk in-lst' in-lst'
                    #2 pick link-get-data   \ act0 grp-lst pos-lnk in-lst' in-lst' pos-reg
                    #5 pick                 \ act0 grp-lst pos-lnk in-lst' in-lst' pos-reg act0
                    action-get-inst-ids     \ act0 grp-lst pos-lnk in-lst' in-lst' pos-reg actsid dom-id
                    group-new               \ act0 grp-lst pos-lnk in-lst', grp t | f
                    if
                        nip                         \ act0 grp-lst pos-lnk grp
                        cr #3 pick .action-inst-ids space ." Adding group: " dup .group cr
                        #3 pick                     \ act0 grp-lst pos-lnk grp act0
                        action-add-group            \ act0 grp-lst pos-lnk
                    else
                        \ cr ." group-new failed" cr
                        square-list-deallocate      \ act0 grp-lst pos-lnk
                    then
                then
            then
        then
    next-item
                                        \ act0 pos-regs
    2drop
    \ cr ." action-add-possible-groups: end" cr
;

\ Return square states not in defining regions.
: action-states-not-in-defining-regions  ( act0 -- sta-lst )
    \ Check arg.
    assert( tos is-action? )

    \ Init result list.
    list-new                            \ act0 rslt-lst

    \ Get defining regions.
    over action-get-defining-regions    \ act0 rslt-lst def-regs

    \ Get all square states.
    #2 pick action-get-squares          \ act0 rslt-lst def-regs sqr-lst'
    square-list-states                  \ act0 rslt-lst def-regs sta-lst'

    \ Find states needed.
    dup                                 \ act0 rslt-lst def-regs sta-lst' sta-lst'
    foreach                             \ act0 rslt-lst def-regs sta-lst' sta-lnk stax
        #3 pick                         \ act0 rslt-lst def-regs sta-lst' sta-lnk stax def-regs
        region-list-any-superset-state? \ act0 rslt-lst def-regs sta-lst' sta-lnk bool
        ifnot
            \ Add state to result list.
            dup link-get-data           \ act0 rslt-lst def-regs sta-lst' sta-lnk stax
            #4 pick                     \ act0 rslt-lst def-regs sta-lst' sta-lnk stax rslt-lst
            list-push-struct            \ act0 rslt-lst def-regs sta-lst' sta-lnk
        then
    next-item
                                        \ act0 rslt-lst def-regs sta-lst'
    state-list-deallocate               \ act0 rslt-lst def-regs
    drop                                \ act0 rslt-lst
    nip
;

\ Return a rate for a corner.
: action-corner-calc-set-rate ( crn1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner? )
    \ cr ." action-calc-corner-rate: start: " .stack cr

    action-get-possible-regions         \ crn1 pos-lst
    swap                                \ pos-lst crn1
    corner-calc-set-rate                \
;

\ Return true if a corner is confirmed.
\ This is very strict, while action-corner-possible? is very permissive.
\ They both require the corner anchor state to be in only one possible region.
: action-corner-confirmed? ( crn1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner? )

    \ Check corner is in exactly one possible group.
    over corner-get-anchor-state        \ crn1 act0 sta1
    over action-get-possible-regions    \ crn1 act0 sta1 pos-lst
    region-list-num-state-in            \ crn1 act0 num-in
    1 <>
    if
        2drop
        false
        exit
    then

    \ Get action squares.
    dup action-get-squares              \ crn1 act0 sqr-lst

    \ Get anchor, done if not found.
    #2 pick corner-get-anchor-state     \ crn1 act0 sqr-lst anc-sta
    over square-list-find               \ crn1 act0 sqr-lst, anc-sqr t | f
    ifnot
        2drop drop
        false
        exit
    then

    \ Check anchor square pnc value, done if false.
    dup square-get-pnc                  \ crn1 act0 sqr-lst anc-sqr pnc
    ifnot
        2drop 2drop
        false
        exit
    then

    \ Check adjacent squares.
    #3 pick corner-get-adjacent-states  \ crn1 act0 sqr-lst anc-sqr adj-stas

    \ Check each adjacent square.
    foreach                             \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk adj-sta
        #3 pick square-list-find        \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk, sqr t | f
        if
            \ Check square pnc.
            dup square-get-pnc          \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk sqr pnc
            if
                #2 pick                 \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk sqr anc-sqr
                squares-compare         \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk char
                [char] I =
                ifnot
                    \ Square not incompatible.
                    2drop 2drop drop
                    false
                    exit
                then
            else
                \ Square not pnc.
                2drop 2drop drop
                false
                exit
            then
        else
            \ Square not found.
            2drop 2drop drop
            false
            exit
        then
    next-item

    2drop 2drop
    true
;

\ Return true, if a corner has an anchor in only one possible region
\ and an external, adjacent, square is not compatible.
\ This is very permissive, while action-corner-confirmed? is very strict.
\ They both require the corner anchor state to be in only one possible region.
: action-corner-possible? ( crn1 act0 -- bool )
    \ Check args.
    assert( tos is-action? dup ifnot cr ." Invalid tos action: " .stack cr then )
    assert( nos is-corner? dup ifnot cr ." Invalid nos corner: " .stack cr then )

    \ Check corner is in exactly one possible group.
    over corner-get-anchor-state        \ crn1 act0 sta1
    over action-get-possible-regions    \ crn1 act0 sta1 pos-lst
    region-list-num-state-in            \ crn1 act0 num-in
    1 <>
    if
        2drop
        false
        exit
    then

    \ Get action squares.
    dup action-get-squares              \ crn1 act0 sqr-lst

    \ Get anchor, done if not found.
    #2 pick corner-get-anchor-state     \ crn1 act0 sqr-lst anc-sta
    over square-list-find               \ crn1 act0 sqr-lst, anc-sqr t | f
    ifnot
        2drop drop
        true
        exit
    then

    \ Check anchor square pnc value, done if false.
    dup square-get-pnc                  \ crn1 act0 sqr-lst anc-sqr pnc
    ifnot
        2drop 2drop
        true
        exit
    then

    \ Check adjacent squares.
    #3 pick corner-get-adjacent-states  \ crn1 act0 sqr-lst anc-sqr adj-stas

    \ Check each adjacent square.
    foreach                             \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk adj-sta
        #3 pick square-list-find        \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk, sqr t | f
        if
            \ Check square pnc.
            dup square-get-pnc          \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk sqr pnc
            if
                #2 pick                 \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk sqr anc-sqr
                squares-compare         \ crn1 act0 sqr-lst anc-sqr adj-stas-lnk char
                [char] C =
                if
                    2drop 2drop drop
                    false
                    exit
                then
            else
                drop
            then
        then
    next-item

    2drop 2drop
    true
;

\ Given a corner, generate additional corners that could be
\ attached to adjacent states that are in other defining regions.
: action-calc-additional-corners ( crn1 act0 -- crn-lst t | f )
    assert( tos is-action? )
    assert( nos is-corner? )

    action-get-possible-regions         \ crn1 pos-regs
    swap                                \ pos-regs crn1
    corner-additional-corners           \ crn-lst
;

\ For use with list function, having xt signature is ( item link-data -- flag )
: corner-test-rate ( val corner -- bool ) corner-get-rate = ;

\ Calculate a corner cluster from a rated corner list, and defining regions.
: action-calc-corner-cluster ( crn-lst1 act0 -- crn-lst t | f)
    \ Check arg.
    assert( tos is-action? )
    assert( nos is-corner-list? )
    over list-is-empty? if 2drop false exit then
    \ cr ." action-calc-corner-cluster: start: " .stack cr

    \ Get highest corner rate.
    [ ' corner-get-rate ] literal       \ crn-lst1 act0 xt
    #2 pick                             \ crn-lst1 act0 xt crn-lst1
    list-max-value                      \ crn-lst1 act0 max

    \ Get the first highest ranked corner.
    [ ' corner-test-rate ] literal      \ crn-lst1 act0 max xt
    swap                                \ crn-lst1 act0 xt max
    #3 pick                             \ crn-lst1 act0 xt max crn-lst1
    list-find                           \ crn-lst1 act0, crnx t | f
    invert abort" max rated corner not found?"

    \ Get additional corners.
    over action-get-possible-regions    \ crn-lst1 act0 crnx pos-regs
    swap                                \ crn-lst1 act0 pos-regs crnx
    corner-additional-corners           \ crn-lst1 act0, crn-lst t | f

    if
        nip nip
        true
    else
        2drop
        false
    then

    \ cr ." action-calc-corner-cluster: end: " .stack cr
;

\ Calc corner clusters.
\ Copy the action corner, and defining region, lists.
\
\ Generate one corner cluster after another, until the copied defining
\ list is empty.
\
\ Return a list of corner clusters, that is a list or corner lists.
: action-calc-corner-clusters ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-calc-corner-clusters: start: " .stack cr

    \ Init cluster list.
    list-new                                    \ act0 cstr-lst'

    \ Get, copy, and rate, corner list.
    over action-get-corners                     \ act0 cstr-lst' crn-lst
    list-copy-struct                            \ act0 cstr-lst' crn-lst'

    \ Rate each corner, the number of adjacent states that are only in one region.
    #3 pick                                     \ act0 cstr-lst' crn-lst' act0
    action-get-possible-regions                 \ act0 cstr-lst' crn-lst' pos-regs
    over                                        \ act0 cstr-lst' crn-lst' pos-regs crn-lst'
    corner-list-calc-set-rate                   \ act0 cstr-lst' crn-lst'

    begin
        dup                                     \ act0 cstr-lst' crn-lst' crn-lst'
        #3 pick                                 \ act0 cstr-lst' crn-lst' crn-lst' act0

        action-calc-corner-cluster              \ act0 cstr-lst' crn-lst', clstr' t | f

        if                                      \ act0 cstr-lst' crn-lst', clstr'
            \ Add cluster to cluster list.
            dup                                 \ act0 cstr-lst' crn-lst' clstr' clstr'
            #3 pick                             \ act0 cstr-lst' crn-lst' clstr' clstr' cstr-lst'
            list-push-struct                    \ act0 cstr-lst' crn-lst' clstr'

            \ Remove corners from corner list.
            foreach                             \ act0 cstr-lst' crn-lst' crn-lnk crnx
                corner-get-region               \ act0 cstr-lst' crn-lst' crn-lnk regx

                \ Remove corners from corner list.
                #2 pick                         \ act0 cstr-lst' crn-lst' crn-lnk regx crn-lst'
                corner-list-remove-by-region    \ act0 cstr-lst' crn-lst' crn-lnk
            next-item
                                                \ act0 cstr-lst' crn-lst'
            false                               \ Do not end the loop.
        else                                    \ act0 cstr-lst' crn-lst'
            true                                \ End the loop.
        then
    until

    \ Clean up.
    corner-list-deallocate              \ act0 cstr-lst'

    \ FYI
    dup list-get-length                 \ act0 cstr-lst' len
    1 >                                 \ act0 cstr-lst' bool
    if
        cr ." Multiple corner clusters found!"
        dup .corner-cluster-list
        abort
    then

    \ Save clusters.
    swap                                \ cstr-lst' act0
    _action-update-corner-clusters

    \ cr ." action-calc-corner-clusters: end: " .stack cr
;

\ To save memory, scan corners in the corner clusters, insure groups
\ based on a defining region, start with the square/state of a corner anchor.
: action-update-groups-from-corner-cluster ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." : action-update-groups-from-corner-cluster: start: " .stack cr

    dup action-get-groups           \ act0 grp-lst
    over action-get-corner-clusters \ act0 grp-lst crnc-lst

    dup list-is-empty?
    if
        2drop
        drop
        \ cr ." : action-update-groups-from-corner-cluster: exit 1: "
        exit
    then

    foreach                         \ act0 grp-lst crnc-lnk crn-lst
        foreach                     \ act0 grp-lst crnc-lnk crn-lnk crn
            \ #4 pick                 \ act0 grp-lst crnc-lnk crn-lnk crn act0
            \ .action-inst-ids      \ act0 grp-lst crnc-lnk crn-lnk crn

            \ cr ." corner: " dup .corner
            dup corner-get-region   \ act0 grp-lst crnc-lnk crn-lnk crn reg
            \ space ." region: " dup .region

            #4 pick                 \ act0 grp-lst crnc-lnk crn-lnk crn reg grp-lst
            group-list-find         \ act0 grp-lst crnc-lnk crn-lnk crn, grp t | f
            if
                \ space ." group: " dup .group

                dup                 \ act0 grp-lst crnc-lnk crn-lnk crn grp grp
                group-get-squares   \ act0 grp-lst crnc-lnk crn-lnk crn grp sqr-lst
                list-get-first-item \ act0 grp-lst crnc-lnk crn-lnk crn grp sqr
                square-get-state    \ act0 grp-lst crnc-lnk crn-lnk crn grp sta
                \ space ." starts with: " dup .state

                #2 pick                 \ act0 grp-lst crnc-lnk crn-lnk crn grp sta crn
                corner-get-anchor-state \ act0 grp-lst crnc-lnk crn-lnk crn grp sta csta
                \ space ." anchor: " dup .state

                2dup states-eq?         \ act0 grp-lst crnc-lnk crn-lnk crn grp sta csta bool
                if
                    \ space ." = "
                    2drop 2drop
                else
                    \ space ." <>"
                    cr #7 pick .action-inst-ids space ." Change first square of group " #2 pick .group
                    space ." from " over .state space ." to " dup .state cr

                    nip                         \ act0 grp-lst crnc-lnk crn-lnk crn grp csta
                    swap                        \ act0 grp-lst crnc-lnk crn-lnk crn csta grp
                    group-change-first-square   \ act0 grp-lst crnc-lnk crn-lnk crn
                    drop
                then
            else
                cr ." group not found?" cr
                abort
            then
            \ cr
        next-item
    next-item
                                    \ act0 grp-lst
    drop                            \ act0

    drop

    \ cr ." : action-update-groups-from-corner-cluster: end: " .stack cr
;

\ Calc corners, from action-defining-regions and
\ action-states-in-one-region.
\
\ Sets action-corners and action-corner-clusters.
\
\ When incompatible pairs change:
\   Possible regions change
\   Defining regions change
\   States only in one region change.
\   Corners change.
\   Corner clusters change.
: action-calc-corners ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-calc-corners: start: " .stack cr

    \ Init corner cluster list.
    list-new over _action-update-corner-clusters

    \ Init preliminary corner list.
    list-new swap                       \ pre-lst act0
    dup action-get-states-in-one-region \ pre-lst act0 stas-in1
    over action-get-defining-regions    \ pre-lst act0 stas-in1 def-lst

    foreach                             \ pre-lst act0 stas-in1 def-lnk regx
        \ Get squares in only the current defining region.
        #2 pick                         \ pre-lst act0 stas-in1 def-lnk regx stas-in1
        state-list-in-region            \ pre-lst act0 stas-in1 def-lnk stas-in-reg'

        \ cr ." For defining region: " over link-get-data .region space ." squares in: " dup .state-list cr
        dup                             \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-reg'
        foreach                         \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax
            \ Make corner.
            #3 pick                     \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax def-lnk
            link-get-data               \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax regx
            corner-new                  \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn'
            dup                         \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn' crn'
            #6 pick                     \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn' crn' act0
            action-corner-possible?     \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn' bool
            if
                \ Store corner.
                #6 pick                 \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn' pre-lst
                list-push-struct        \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk
            else
                corner-deallocate       \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk
            then
        next-item

        state-list-deallocate
    next-item
                                        \ pre-lst act0 stas-in1
    drop                                \ pre-lst act0

    tuck _action-update-corners         \ act

    \ Calc and store corner clusters.
    action-calc-corner-clusters
;

\ Evaluate possible regions, to generate corners and corner clusters.
: _action-evaluate-possible-regions ( act0 -- )
    \ cr ." _action-evaluate-possible-regions: start"
    \ Check arg.
    assert( tos is-action? )

    \ Update action-states-in-one-region.
    dup action-square-states-in-one-region              \ act0 sta-lst'
    over _action-update-states-in-one-region            \ act0

    \ Update defining regions.
    dup action-defining-regions                         \ act0 reg-lst
    over _action-update-defining-regions                \ act0

    \ Update states-not-in-defining-regions.
    dup action-states-not-in-defining-regions           \ act0 sta-lst
    over _action-update-states-not-in-defining-regions  \ act0

    \ Update action-corners and action-corner-clusters.
    dup action-calc-corners                             \ act0

    drop
    \ cr ." _action-evaluate-possible-regions: end"
;

\ **********************************
\ * Start incompatible pair logic. *
\ **********************************

\ Return true if the square for sta1, between sta2 and sta3,
\ is incompatible with the square for sta2 or sta3.
: action-check-sqr-between-incompatible ( sta3 sta2 sta1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )
    assert( 3os is-state? )
    assert( 4os is-state? )
    \ cr ." action-check-sqr-between-incompatible: " #3 pick .state #2 pick space .state over space .state
    assert( #3 pick #3 pick #3 pick state-between? )

    \ Look up square for sta1.
    action-get-squares              \ sta3 sta2 sta1 sqr-lst
    swap                            \ sta3 sta2 sqr-lst sta1
    over                            \ sta3 sta2 sqr-lst sta1 sqr-lst
    square-list-find                \ sta3 sta2 sqr-lst, sqr1 t | f
    invert abort" square 1 not found?"

    \ Get square for sta2.
    rot                             \ sta3 sqr-lst sqr1 sta2
    #2 pick                         \ sta3 sqr-lst sqr1 sta2 sqr-lst
    square-list-find                \ sta3 sqr-lst sqr1, sqr2 t | f
    invert abort" square 2 not found?"

    \ Check if sqr1 and sqr2 are incompatible.
    over squares-compare            \ sta3 sqr-lst sqr1 char
    [char] I =
    if
        2drop drop
        true
        exit
    then

    \ Get square for sta3.
    -rot                            \ sqr1 sta3 sqr-lst
    square-list-find                \ sqr1, sqr3 t | f
    invert abort" square 3 not found?"

    \ Check if sqr1 and sqr3 are incompatible.
    squares-compare                 \ char
    [char] I =
;

\ Check the effect on incompatible pairs, of a square.
\ Return a list of pairs that use a given square and are no longer compatible.
: action-pairs-no-longer-incompatible ( sqr2 pr-lst1 act0 -- del-lst t | f )
    \ cr ." action-pairs-no-longer-incompatible: start: " .stack cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )
    assert( 3os is-square? )

    \ Check each pair, accumulate pairs to delete.

    \ Bring region list forward.
    swap                                \ sqr2 act0 pr-lst1

    \ Init delete list.
    list-new                            \ sqr2 act0 pr-lst1 del-lst

    \ Prep for loop.
    #3 pick square-get-state            \ sqr2 act0 pr-lst1 del-lst sta2
    rot                                 \ sqr2 act0 del-lst sta2 pr-lst1

    foreach                             \ sqr2 act0 del-lst sta2 pr-lnk regx
        #2 pick swap                    \ sqr2 act0 del-lst sta2 pr-lnk sta2 regx

        region-uses-state?              \ sqr2 act0 del-lst sta2 pr-lnk bool
        if
            dup link-get-data           \ sqr2 act0 del-lst sta2 pr-lnk regx
            dup region-get-state-0      \ sqr2 act0 del-lst sta2 pr-lnk regx r-sta0
            #3 pick                     \ sqr2 act0 del-lst sta2 pr-lnk regx r-sta0 sta2
            states-eq?                  \ sqr2 act0 del-lst sta2 pr-lnk regx bool
            \ Get the other state.
            if
                \ Check state 1
                region-get-state-1      \ sqr2 act0 del-lst sta2 pr-lnk r-sta
            else
                \ Check state 0
                region-get-state-0      \ sqr2 act0 del-lst sta2 pr-lnk r-sta
            then
            \ Compare with sqr2.
            #4 pick                     \ sqr2 act0 del-lst sta2 pr-lnk r-sta2 act0
            action-find-square          \ sqr2 act0 del-lst sta2 pr-lnk, sqr t | f
            invert abort" square not found?"

            #5 pick                 \ sqr2 act0 del-lst sta2 pr-lnk sqr sqr2
            squares-compare         \ sqr2 act0 del-lst sta2 pr-lnk char
            \ Allow pairs to go to More Samples Needed. The normal
            \ confirmation by seeking pnc for each square will push
            \ it to Compatible or Incompatible.
            \ If it goes to Incompatible, a complete recalc will be
            \ avoided.
            [char] C =
            if
                \ Save pair for deletion.
                dup link-get-data   \ sqr2 act0 del-lst sta2 pr-lnk regx
                #3 pick             \ sqr2 act0 del-lst sta2 pr-lnk regx det-lst
                list-push-struct    \ sqr2 act0 del-lst sta2 pr-lnk
            then
        then
    next-item
                                        \ sqr2 act0 del-lst sta2
    drop                                \ sqr2 act0 del-lst

    nip nip                             \ del-lst

    check-list-return
    \ cr ." action-pairs-no-longer-incompatible: end: " .stack cr
;

\ Check the effect on incompatible pairs, of a pnc square.
\ Return a list of pairs to delete.
: action-pairs-with-incompatible-square-between ( sqr2 pr-lst1 act0 -- del-lst t | f )
    \ cr ." action-pairs-with-incompatible-square-between: start: " .stack cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )
    assert( 3os is-square? )

    \ Check each pair, accumulate pairs to delete.

    \ Bring region list forward.
    swap                                                \ sqr2 act0 pr-lst1

    \ Init delete list.
    list-new                                            \ sqr2 act0 pr-lst1 del-lst

    \ Prep for loop.
    #3 pick square-get-state                            \ sqr2 act0 pr-lst1 del-lst sta2
    rot                                                 \ sqr2 act0 del-lst sta2 pr-lst1

    foreach                                             \ sqr2 act0 del-lst sta2 pr-lnk regx
        #2 pick swap                                    \ sqr2 act0 del-lst sta2 pr-lnk sta2 regx

        region-superset-of-state?                       \ sqr2 act0 del-lst sta2 pr-lnk bool
        if
            over                                        \ sqr2 act0 del-lst sta2 pr-lnk sta2
            over link-get-data                          \ sqr2 act0 del-lst sta2 pr-lnk sta2 regx
            region-uses-state?                          \ sqr2 act0 del-lst sta2 pr-lnk bool
            ifnot
                \ Square is between them.
                2dup                                    \ sqr2 act0 del-lst sta2 pr-lnk sta2 pr-lnk
                link-get-data                           \ sqr2 act0 del-lst sta2 pr-lnk sta2 regx
                region-get-states                       \ sqr2 act0 del-lst sta2 pr-lnk sta2 sta-0 sta-1
                rot                                     \ sqr2 act0 del-lst sta2 pr-lnk sta-0 sta-1 sta2
                #6 pick                                 \ sqr2 act0 del-lst sta2 pr-lnk sta-0 sta-1 sta2 act0
                action-check-sqr-between-incompatible   \ sqr2 act0 del-lst sta2 pr-lnk bool
                if
                    dup link-get-data                   \ sqr2 act0 del-lst sta2 pr-lnk regx
                    #3 pick                             \ sqr2 act0 del-lst sta2 pr-lnk regx del-lst
                    list-push-struct                    \ sqr2 act0 del-lst sta2 pr-lnk
                then
            then
        then
    next-item
                                                        \ sqr2 act0 del-lst sta2
    drop                                                \ sqr2 act0 del-lst

    nip nip                                             \ del-lst

    check-list-return
    \ cr ." action-pairs-with-incompatible-square-between: end: " .stack cr
;

\ Intersect adj and nadj regions, check groups.
: action-recalc-possible-regions ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-recalc-possible-regions: start: " .stack cr

    dup action-get-adj-regions              \ act0 adj-regs
    over action-get-nadj-regions            \ act0 adj-regs nadj-regs
    region-list-intersections-nosubs        \ act0 reg-lst'

    over _action-update-possible-regions    \ act0

    dup action-square-states-in-one-region  \ act0 sta-lst'
    over                                    \ act0 sta-lst' act0
    _action-update-states-in-one-region     \ act0

    _action-evaluate-possible-regions

    \ cr ." action-recalc-possible-regions: end: " .stack cr
;

\ Recalc possible regions, from adjacent, incmpatible pairs.
: action-recalc-adj-pair-regions ( act0 -- )
    list-new                                \ act0 pos-new
    over action-get-num-bits                \ act0 pos-new nb
    region-max-x                            \ act0 pos-new reg-max
    over list-push-struct                   \ act0 pos-new

    over action-get-adj-pairs               \ act0 pos-new pr-lst

    foreach                                 \ act0 pos-new pr-lnk regx
        region-get-states                   \ act0 pos-new pr-lnk sta1 sta1
        state-~a+~b                         \ act0 pos-new pr-lnk reg-lst'
        dup                                 \ act0 pos-new pr-lnk reg-lst' reg-lst'
        #3 pick                             \ act0 pos-new pr-lnk reg-lst' reg-lst' pos-new
        region-list-intersections-nosubs    \ act0 pos-new pr-lnk reg-lst' pos-new2

        \ Clean up.
        swap region-list-deallocate         \ act0 pos-new pr-lnk pos-new2
        rot region-list-deallocate          \ act0 pr-lnk pos-new2
        swap                                \ act0 pos-new2 pr-lnk
    next-item
                                            \ act0 pos-new
    swap                                    \ pos-new act0
    _action-update-adj-regions              \
;

\ Recalc possible regions, from non-adjacent, incompatible pairs.
: action-recalc-nadj-pair-regions ( act0 -- )
    \ Check args.
    assert( tos is-action? )
    \ cr ." action-recalc-nadj-pair-regions: start: " .stack cr

    \ Init result list.
    list-new                                \ act0 pos-new
    over action-get-num-bits                \ act0 pos-new nb
    region-max-x                            \ act0 pos-new reg-max
    over list-push-struct                   \ act0 pos-new

    \ Check each pair.
    over action-get-nadj-pairs              \ act0 pos-new pr-lst

    foreach                                 \ act0 pos-new pr-lnk regx
        region-get-states                   \ act0 pos-new pr-lnk sta1 sta0
        state-~a+~b                         \ act0 pos-new pr-lnk reg-lst'
        dup                                 \ act0 pos-new pr-lnk reg-lst' reg-lst'
        #3 pick                             \ act0 pos-new pr-lnk reg-lst' reg-lst' pos-new
        region-list-intersections-nosubs    \ act0 pos-new pr-lnk reg-lst' pos-new2

        \ Clean up.
        swap region-list-deallocate         \ act0 pos-new pr-lnk pos-new2
        rot region-list-deallocate          \ act0 pr-lnk pos-new2
        swap                                \ act0 pos-new2 pr-lnk
    next-item
                                            \ act0 pos-new
    cr over .action-inst-ids space ." new nadj regions: " dup .region-list cr
    swap                                    \ pos-new act0
    tuck _action-update-nadj-regions        \ act0

    action-recalc-possible-regions          \

    \ cr ." action-recalc-nadj-pair-regions: end: " .stack cr
;

\ Remove any nadj pairs that are no longer within an adj region.
: action-remove-orphaned-nadj-pairs ( act0 -- )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-remove-orphaned-nadj-pairs: start" cr

    \ Init delete list.
    list-new swap                           \ del-lst' act0

    \ Check each pair.
    dup action-get-adj-regions              \ del-lst' act0 adj-regs
    over action-get-nadj-pairs              \ del-lst' act0 adj-regs nadj-prs

    foreach                                 \ del-lst' act0 adj-regs nadj-lnk prx
        #2 pick                             \ del-lst' act0 adj-regs nadj-lnk prx adj-regs
        region-list-any-superset-of?        \ del-lst' act0 adj-regs nadj-lnk bool
        ifnot
            dup link-get-data               \ del-lst' act0 adj-regs nadj-lnk prx
            #4 pick                         \ del-lst' act0 adj-regs nadj-lnk prx del-lst
            region-list-push                \ del-lst' act0 adj-regs nadj-lnk
        then
    next-item
                                            \ del-lst' act0 adj-regs
    drop                                    \ del-lst' act0

    over list-is-empty?
    if
        drop                                \ del-lst'
    else
        \ Delete selected non-adjacent pairs.
        cr dup .action-inst-ids space ." Deleting orphaned non-adjacent pairs: "
        dup action-get-nadj-pairs           \ del-lst' act0 nadj-prs
        #2 pick                             \ del-lst' act0 nadj-prs del-lst'
        foreach                             \ del-lst' act0 nadj-prs del-lnk prx
            \ Print region.
            space dup .region

            \ Remove region.
            #2 pick                         \ del-lst' act0 nadj-prs del-lnk prx nadj-prs
            region-list-remove              \ del-lst' act0 nadj-prs del-lnk
        next-item

        cr                                  \ del-lst' act0 nadj-prs
        2drop                               \ del-lst'
    then

    \ Return.
    region-list-deallocate
    \ cr ." action-remove-orphaned-nadj-pairs: end" cr
;

\ Check for, adjacent, dissimilar, pairs, of a given square,
\ where the pair is in a adj-pair derived region.
: action-check-square-for-new-adj-pairs2 ( sqr1 act0 -- pr-lst t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square-for-new-adj-pairs2: start: " .stack cr

    \ Init return list.
    list-new -rot                                   \ pr-lst sqr1 act0

    dup action-get-adj-regions                      \ pr-lst sqr1 act0 adj-regs
    over action-get-squares                         \ pr-lst sqr1 act0 adj-regs sqr-lst

    foreach                                         \ pr-lst sqr1 act0 adj-regs sqr-lnk sqrx
        \ Check if the square is adjacent to sqr1.
        #4 pick                                     \ pr-lst sqr1 act0 adj-regs sqr-lnk sqrx sqr1
        squares-adjacent?                           \ pr-lst sqr1 act0 adj-regs sqr-lnk bool
        if
            \ Check if the square is incompatible to sqr1.
            dup link-get-data                       \ pr-lst sqr1 act0 adj-regs sqr-lnk sqrx
            #4 pick                                 \ pr-lst sqr1 act0 adj-regs sqr-lnk sqrx sqr1
            squares-compare                         \ pr-lst sqr1 act0 adj-regs sqr-lnk char
            [char] I =                              \ pr-lst sqr1 act0 adj-regs sqr-lnk bool
            if
                \ Check if the two squares are in any currently possible region.

                \ Make a region from the two square states.
                dup link-get-data                   \ pr-lst sqr1 act0 adj-regs sqr-lnk sqrx
                square-get-state                    \ pr-lst sqr1 act0 adj-regs sqr-lnk stax
                #4 pick                             \ pr-lst sqr1 act0 adj-regs sqr-lnk stax sqr1
                square-get-state                    \ pr-lst sqr1 act0 adj-regs sqr-lnk stax sta1
                region-new                          \ pr-lst sqr1 act0 adj-regs sqr-lnk regx'

                \ Check if a possible region is a superset of the region.
                dup                                 \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' regx'
                #3 pick                             \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' regx' adj-regs
                region-list-any-superset-of?          \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' bool
                if
                    \ Check if there is any superset region in nadj pairs.
                    dup                             \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' regx'
                    #4 pick                         \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' regx' act0
                    action-get-adj-pairs            \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' regx' nadj-prs
                    region-list-any-superset-of?    \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' bool
                    if
                        \ Region
                        region-deallocate           \ pr-lst sqr1 act0 adj-regs sqr-lnk
                    else
                        \ Add region to the new pair list.
                        #5 pick                     \ pr-lst sqr1 act0 adj-regs sqr-lnk regx' pr-lst
                        list-push-struct            \ pr-lst sqr1 act0 adj-regs sqr-lnk
                    else
                    then
                else
                    \ Region not within an adj-pair dirived region.
                    region-deallocate               \ pr-lst sqr1 act0 adj-regs sqr-lnk
                then
            then
        then
    next-item
                                                    \ pr-lst sqr1 act0 adj-regs
    \ Return.
    2drop drop                                      \ pr-lst

    check-list-return
    \ cr ." action-check-square-for-new-adj-pairs2: end:   " .stack cr
;

\ Check for, adjacent, dissimilar, pairs, of a given square,
\ where the pair is in an adj-pair derived region.
\ If any found, recalculate a few things.
: action-check-square-for-new-adj-pairs  ( sqr1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square-for-new-adj-pairs: start: " .stack cr

    2dup action-check-square-for-new-adj-pairs2 \ sqr1 act0, pr-lst' t | f
    ifnot
        2drop
        false
        \ cr ." action-check-square-for-new-adj-pairs: exit 1: " .stack cr
        exit
    then

    cr ." Appending adj pairs: " dup .region-list cr
    \ Update the adjacent pair list.
    2dup swap                                   \ sqr1 act0 pr-lst' pr-lst' act0
    action-get-adj-pairs                        \ sqr1 act0 pr-lst' pr-lst' adj-prs
    list-append-struct                          \ sqr1 act0 pr-lst'
    region-list-deallocate                      \ sqr1 act0

    \ Recalc.
    dup action-recalc-adj-pair-regions          \ sqr act0
    dup action-remove-orphaned-nadj-pairs       \ sqr1 act0
    dup action-recalc-nadj-pair-regions         \ sqr1 act0

    \ Return.
    2drop
    true
    \ cr ." action-check-square-for-new-adj-pairs: end: " .stack cr
;

\ Check for, non-adjacent, dissimilar, pairs, of a given square,
\ where the pair is in a possible region.
: action-check-square-for-new-nadj-pairs2 ( sqr1 act0 -- pr-lst t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square-for-new-nadj-pairs: start: " cr

    \ Init return list.
    list-new -rot                           \ pr-lst sqr1 act0

    dup action-get-possible-regions         \ pr-lst sqr1 act0 pos-regs
    over action-get-squares                 \ pr-lst sqr1 act0 pos-regs sqr-lst

    foreach                                 \ pr-lst sqr1 act0 pos-regs sqr-lnk sqrx
        \ Check if the square is adjacent to sqr1.
        #4 pick                             \ pr-lst sqr1 act0 pos-regs sqr-lnk sqrx sqr1
        squares-adjacent?                   \ pr-lst sqr1 act0 pos-regs sqr-lnk bool
        ifnot
            \ Check if the square is incompatible to sqr1.
            dup link-get-data               \ pr-lst sqr1 act0 pos-regs sqr-lnk sqrx
            #4 pick                         \ pr-lst sqr1 act0 pos-regs sqr-lnk sqrx sqr1
            squares-compare                 \ pr-lst sqr1 act0 pos-regs sqr-lnk char
            [char] I =                      \ pr-lst sqr1 act0 pos-regs sqr-lnk bool
            if
                \ Check if the two squares are in any currently possible region.

                \ Make a region from the two square states.
                dup link-get-data           \ pr-lst sqr1 act0 pos-regs sqr-lnk sqrx
                square-get-state            \ pr-lst sqr1 act0 pos-regs sqr-lnk stax
                #4 pick                     \ pr-lst sqr1 act0 pos-regs sqr-lnk stax sqr1
                square-get-state            \ pr-lst sqr1 act0 pos-regs sqr-lnk stax sta1
                region-new                  \ pr-lst sqr1 act0 pos-regs sqr-lnk regx'

                \ Check if a possible region is a superset of the region.
                dup                         \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' regx'
                #3 pick                     \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' regx' pos-regs
                region-list-any-superset-of?  \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' bool
                if

                    \ Check if there is any superset region in nadj pairs.
                    dup                             \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' regx'
                    #4 pick                         \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' regx' act0
                    action-get-nadj-pairs           \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' regx' nadj-prs
                    region-list-any-superset-of?    \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' bool
                    if
                        \ Region
                        region-deallocate           \ pr-lst sqr1 act0 pos-regs sqr-lnk
                    else
                        \ Add region to the new pair list.
                        #5 pick                     \ pr-lst sqr1 act0 pos-regs sqr-lnk regx' pr-lst
                        list-push-struct            \ pr-lst sqr1 act0 pos-regs sqr-lnk
                    else
                    then
                else
                    \ Region not within an adj-pair dirived region.
                    region-deallocate               \ pr-lst sqr1 act0 pos-regs sqr-lnk
                then
            then
        then
    next-item
                                            \ pr-lst sqr1 act0 pos-regs

    \ Return.
    2drop drop                              \ pr-lst

    check-list-return
    \ cr ." action-check-square-for-new-nadj-pairs: end: " cr
;

\ Check for, non-adjacent, dissimilar, pairs, of a given square,
\ where the pair is in an adj-pair derived region.
\ If any found, recalculate a few things.
: action-check-square-for-new-nadj-pairs  ( sqr1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )

    2dup action-check-square-for-new-nadj-pairs2    \ sqr1 act0, pr-lst t | f
    ifnot
        2drop
        false
        exit
    then

    \ Update the non-adjacent pair list.
    cr over .action-inst-ids space ." Appending nadj pairs: " dup .region-list cr
    2dup swap                                   \ sqr1 act0 pr-lst' pr-lst' act0
    action-get-nadj-pairs                       \ sqr1 act0 pr-lst' pr-lst' adj-prs
    list-append-struct                          \ sqr1 act0 pr-lst'
    region-list-deallocate                      \ sqr1 act0

    \ Recalc.
    dup action-recalc-nadj-pair-regions         \ sqr act0
    dup action-recalc-possible-regions          \ sqr act0

    \ Return.
    2drop
    true
;

\ Check if a square affects adjacent, incompatible, pairs.
\ If there is a change, update adj pairs, adj regions, nadj pairs, and nadj regions.
\ Return true if there is a change.
: action-check-square-effect-on-adj-pairs ( sqr1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square-effect-on-adj-pairs: start: " .stack cr

    \ Init any-change flag.
    false -rot                                          \ ch-fl sqr1 act0

    \ Get pairs, no longer incompatible, due to the square, to remove.
    2dup                                                \ ch-fl sqr1 act0 sqr1 act0
    dup action-get-adj-pairs                            \ ch-fl sqr1 act0 sqr1 act0 adj-prs
    swap                                                \ ch-fl sqr1 act0 sqr1 adj-prs act0
    action-pairs-no-longer-incompatible                 \ ch-fl sqr1 act0, del-lst' t | f
    if
        \ Remove pairs.
        cr over .action-inst-ids space ." Deleting adjacent pairs: "
        over action-get-adj-pairs                       \ ch-fl sqr1 act0 del-lst' adj-lst
        over                                            \ ch-fl sqr1 act0 del-lst' adj-lst del-lst'
        foreach                                         \ ch-fl sqr1 act0 del-lst' adj-lst del-lnk regx
            space dup .region cr
            #2 pick                                     \ ch-fl sqr1 act0 del-lst' adj-lst del-lnk regx adj-lst
            region-list-remove                          \ ch-fl sqr1 act0 del-lst' adj-lst del-lnk
        next-item
        cr
                                                        \ ch-fl sqr1 act0 del-lst' nadj-lst
        drop region-list-deallocate                     \ ch-fl sqr1 act0

        \ Recalc adj-regions.
        dup action-recalc-adj-pair-regions              \ ch-fl sqr1 act0

        \ Update change flag.
        rot drop true -rot                              \ ch-fl sqr1 act0
    then

    \ Check for new adj pairs.
    2dup action-check-square-for-new-adj-pairs          \ ch-fl sqr1 act0 bool
    if
        \ Update change flag.
        rot drop true -rot                              \ ch-fl sqr1 act0
    then

    \ Process change flag.
    #2 pick                                             \ ch-fl sqr1 act0 ch-fl
    if
        \ Handle non-adjacent pairs.
        dup action-remove-orphaned-nadj-pairs           \ ch-fl sqr1 act0
        dup action-recalc-nadj-pair-regions             \ ch-fl sqr1 act0
    then

    \ Return.
    2drop                                               \ ch-fl
    \ cr ." action-check-square-effect-on-adj-pairs: end: " .stack cr
;

\ Check if a square affects non-adjacent, incompatible, pairs.
\ If there is a change, update nadj pairs and regions,
\ and return true.
: action-check-square-effect-on-nadj-pairs ( sqr1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )

    \ Init any-change flag.
    false -rot                                          \ ch-fl sqr1 act0

    \ Get pairs, no longer incompatible, due to the square, to remove.
    2dup                                                \ ch-fl sqr1 act0 sqr1 act0
    dup action-get-nadj-pairs                           \ ch-fl sqr1 act0 sqr1 act0 nadj-prs
    swap                                                \ ch-fl sqr1 act0 sqr1 nadj-prs act0
    action-pairs-no-longer-incompatible                 \ ch-fl sqr1 act0, del-lst' t | f
    if
        \ Remove pairs.
        over action-get-nadj-pairs                      \ ch-fl sqr1 act0 del-lst' nadj-lst
        over                                            \ ch-fl sqr1 act0 del-lst' nadj-lst del-lst'
        foreach                                         \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk regx
             cr ." action-check-nadj-pairs-for-square: removing: " dup .region cr
            #2 pick                                     \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk regx nadj-lst
            region-list-remove                          \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk
        next-item
                                                        \ ch-fl sqr1 act0 del-lst' nadj-lst
        drop region-list-deallocate                     \ ch-fl sqr1 act0

        \ Recalc nadj-regions.
        dup action-recalc-nadj-pair-regions             \ ch-fl sqr1 act0

        \ Update change flag.
        rot drop true -rot                              \ ch-fl sqr1 act0
    then

    \ Get pairs to remove 2.
    2dup                                                \ ch-fl sqr1 act0 sqr1 act0
    dup action-get-nadj-pairs                           \ ch-fl sqr1 act0 sqr1 act0 nadj-prs
    swap                                                \ ch-fl sqr1 act0 sqr1 nadj-prs act0
    action-pairs-with-incompatible-square-between       \ ch-fl sqr1 act0, del-lst' t | f
    if
        \ Remove pairs.
        over action-get-nadj-pairs                      \ ch-fl sqr1 act0 del-lst' nadj-lst
        over                                            \ ch-fl sqr1 act0 del-lst' nadj-lst del-lst'
        foreach                                         \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk regx
            cr ." action-check-nadj-pairs-for-square: removing: " dup .region cr
            #2 pick                                     \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk regx nadj-lst
            region-list-remove                          \ ch-fl sqr1 act0 del-lst' nadj-lst del-lnk
        next-item
                                                        \ ch-fl sqr1 act0 del-lst' nadj-lst
        drop region-list-deallocate                     \ ch-fl sqr1 act0

        \ Recalc nadj-regions.
        dup action-recalc-nadj-pair-regions             \ ch-fl sqr1 act0

        \ Update change flag.
        rot drop true -rot                              \ ch-fl sqr1 act0
    then

    \ Check for new adj pairs.
    2dup action-check-square-for-new-nadj-pairs         \ ch-fl sqr1 act0 bool

    if
        \ Update change flag.
        rot drop true -rot                              \ ch-fl sqr1 act0
    then

    \ Process change flag.
    #2 pick                                             \ ch-fl sqr1 act0 ch-fl
    if
        \ Handle groups.
        action-recalc-possible-regions                  \ ch-fl sqr1
        drop                                            \ ch-fl
    else
        2drop                                           \ ch-fl
    then
\    cr ." action-check-square-effect-on-nadj-pairs: end: " .stack cr
;

\ Check for effect of a square on incompatible pairs.
\ Return true if there is a change.
: action-check-square-effect-on-incompatible-pairs ( sqr1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square-effect-on-incompatible-pairs: start: " .stack cr

    2dup action-check-square-effect-on-adj-pairs    \ sqr1 act0 bool

    -rot                                            \ bool sqr1 act0

    action-check-square-effect-on-nadj-pairs        \ bool bool

    or
    \ cr ." action-check-square-effect-on-incompatible-pairs: end: " .stack cr
;

\ ********************************
\ * End incompatible pair logic. *
\ ********************************

\ Check a new, or updated square.
: action-check-square ( sqr1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-check-square: start: " .stack cr

    2dup                                                \ sqr1 act0 sqr1 act0
    action-check-square-effect-on-incompatible-pairs    \ sqr1 act0 bool

    -rot                                                \ bool sqr1 act0

    2dup action-check-square-for-new-adj-pairs          \ bool sqr1 act0 bool
    -rot                                                \ bool bool sqr1 act0
    2dup action-check-square-for-new-nadj-pairs         \ bool bool sqr1 act0 bool

    \ Combine the bools.
    #4 pick or                                          \ bool bool sqr1 act0 bool
    #3 pick or                                          \ bool bool sqr1 act0 bool
    if
        dup _action-delete-orphaned-groups
    then

    dup action-add-possible-groups                      \ bool bool sqr1 act0

    action-update-groups-from-corner-cluster            \ bool bool sqr1

    2drop drop

    \ cr ." action-check-square: end: " .stack cr
;

\ Add a new square to the action square list.
: action-add-new-square ( sqr1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-add-new-square: start: " .stack cr

    over square-get-state       \ sqr1 act0 sta
    over action-find-square     \ sqr1 act0, sqr t | f
    if
        cr ." action-add-new-square: square already exists in square list" abort
    then

    \ Store the square.
    2dup action-get-squares     \ sqr1 act0 sqr1 sqr-lst
    list-push-struct            \ sqr1 act0

    2dup action-udpate-groups-with-new-square

    action-check-square
    \ cr ." action-add-new-square: end: " .stack cr
;

\ Add a sample, return true if the sample changed
\ a square.
: action-add-sample ( smpl1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-sample? )
    \ cr dup .action-parent-inst-id  dup .action-inst-id ." : Add sample: " over .sample cr
    \ cr ." action-add-sample: start: " .stack cr

    over sample-get-initial     \ smpl1 act0 initial
    over action-find-square     \ smpl1 act0, sqr t | f
    if
        rot                     \ act0 sqr smpl1
        over                    \ act0 sqr smpl1 sqr
        \ cr #3 pick .action-parent-inst-id #3 pick .action-inst-id ." : Updating square: " dup .square cr
        square-add-sample       \ act0 sqr bool
        cr #2 pick .action-parent-inst-id #2 pick .action-inst-id ." : Updated square: " over .square cr
        if
            swap                        \ sqr act0
            action-check-square         \
            true
            \ cr ." action-add-sample: exit 1: " cr
        else
            2drop
            false
            \ cr ." action-add-sample: exit 2: " cr
        then
    else
        over                    \ smpl1 act0 smpl1
        square-new              \ smpl1 act0 sqr1
        cr over .action-parent-inst-id over .action-inst-id ." : Adding new square: " dup .square cr
        tuck over               \ smpl1 sqr1 act0 sqr1 act0
        action-add-new-square   \ smpl1 sqr1 act0
        action-check-square     \ smpl1
        drop
        true
        \ cr ." action-add-sample: exit 3: " cr
    then
    \ cr ." action-add-sample: end: " .stack cr
;

\ Get a sample from an action.
\ Depends on the current domain being set correctly.
\ Add the sample to the action.
: action-get-sample ( sta1 act0 -- smpl )
    \ cr ." action-get-sample: start" cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    2dup                    \ sta1 act0 sta1 act0
    action-get-function     \ sta1 act0 sta1 xt
    execute                 \ sta1 act0 r-sta
    rot                     \ act0 r-sta sta1
    sample-new              \ act0 smpl
    tuck                    \ smpl act0 smpl
    swap                    \ smpl smpl act0

    action-add-sample       \ smpl bool
    drop                    \ smpl
;

: action-get-forward-steps ( from-sta1 act0 -- actstp-lst t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    \ cr ." action-get-forward-steps: start: " .stack cr

    \ Init return list.
    list-new swap                       \ f-sta1 ret act0

    action-get-groups                   \ f-sta1 ret grp-lst
    foreach                             \ f-sta1 ret grp-lnk grpx
        #3 pick swap                    \ f-sta1 ret grp-lnk f-sta1 grpx
        group-get-forward-step          \ f-sta1 ret grp-lnk, actstp t | f
        if
            #2 pick                     \ f-sta1 ret grp-lnk actstp ret
            list-push-struct            \ f-sta1 ret grp-lnk
        then
    next-item
                                        \ f-sta1 ret
    nip                                 \ ret

    check-list-return
    \ cr ." action-get-forward-steps: end: " .stack cr
;

\ Set the action and domain ids of a list of needs.
: action-set-ids ( ned-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-actneed-list? )

    action-get-inst-ids     \ ned-lst1 act-id dom-id
    rot                     \ act-id dom-id ned-lst1

    foreach                 \ act-id dom-id ned-lnk actnedx
        #3 pick             \ act-id dom-id ned-lnk actnedx act-id
        over actneed-set-act-inst-id
        #2 pick             \ act-id dom-id ned-lnk actnedx dom-id
        swap actneed-set-dom-inst-id
    next-item
                            \ act-id dom-id
    2drop
;

\ Return a group, given a matchig region.
: action-find-group ( reg1 act0 -- grp t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )

    action-get-groups           \ reg1 grp-lst
    group-list-find             \ grp t | f
;

: action-make-need-dis ( tkn3 sta2 typ1 act0 -- actned )
    \ Check args.
    assert( tos is-action? )
    assert( nos dup actneed-type-cls = swap actneed-type-ils = or )
    assert( 3os is-state? )
    assert( 4os is-token? )

    dup action-get-inst-id swap \ tkn3 sta2 typ1 act-id act0
    action-get-dom-inst-id      \ tkn3 sta2 typ1 act-id dom-id

    actneed-new
;

\ Return true if a state is in more than one possible region.
: action-state-in-many-regions ( sta1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    action-get-possible-regions     \ sta1 reg-lst
    region-list-num-state-in        \ num
    1 >                             \ bool
;

\ Return true if a state does NOT have an adjacent compatible square,
\ outside of a region.
: action-state-possible-anchor-for-region ( sta2 reg1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )
    assert( 3os is-state? )
    assert( #2 pick #2 pick region-superset-of-state? )

    \ Get list of all squares.
    dup action-get-squares                  \ sta2 reg1 act0 sqr-lst

    \ Get the square matching sta2.
    #3 pick over                            \ sta2 reg1 act0 sqr-lst sta2 sqr-lst
    square-list-find                        \ sta2 reg1 act0 sqr-lst, sqr t | f
    invert abort" square not found?"

    \ Get region edge mask.
    #3 pick region-edge-mask                \ sta2 reg1 act0 sqr-lst sqr edg-msk'

    \ Get individual bits from the edge mask.
    dup mask-split                          \ sta2 reg1 act0 sqr-lst sqr edg-msk' msk-lst'
    swap mask-deallocate                    \ sta2 reg1 act0 sqr-lst sqr msk-lst'

    dup                                     \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lst'
    foreach                                 \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk bit-mskx
        #7 pick state-xor-mask              \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta'
        dup                                 \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' adj-sta'
        #5 pick                             \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' adj-sta' sqr-lst
        square-list-find                    \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta', sqrx t | f
        if
            #4 pick                         \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' sqrx sqr
            squares-compare                 \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' char
            [char] C =                      \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' bool
            if
                state-deallocate            \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
                drop                        \ sta2 reg1 act0 sqr-lst sqr msk-lst'
                mask-list-deallocate        \ sta2 reg1 act0 sqr-lst sqr
                2drop 2drop drop
                false
                exit
            else
                state-deallocate            \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
            then
        else
            state-deallocate                \ sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
        then
    next-item
                                            \ sta2 reg1 act0 sqr-lst sqr msk-lst'
    mask-list-deallocate                    \ sta2 reg1 act0 sqr-lst sqr
    2drop 2drop drop
    true
;

\ Return a list of non-pnc square states, or non-sampled states,
\ adjacent to a potential anchor, outside of the anchor's region,
\ to seek samples of.
\ Run action-state-possible-anchor-for-region on the possible anchor state first,
\ it should return true.
: action-anchor-states-to-test ( sta2 reg1 act0 -- sta-lst t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region? )
    assert( 3os is-state? )
    assert( #2 pick #2 pick region-superset-of-state? )

    \ Init non-pnc square state list, state-list.
    >r                                      \ sta2 reg1
    list-new list-new                       \ sta2 reg1 sta-lst' npnc-lst'
    2swap                                   \ sta-lst' npnc-lst' sta2 reg1
    r>                                      \ sta-lst' npnc-lst' sta2 reg1 act0

    \ Scan possible states, populate the lists.

    \ Get list of all squares.
    dup action-get-squares                  \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst

    \ Get the square matching sta2.
    #3 pick over                            \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sta2 sqr-lst
    square-list-find                        \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst, sqr t | f
    invert abort" square not found?"

    \ Get region edge mask.
    #3 pick region-edge-mask                \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr edg-msk'

    \ Get individual bits from the edge mask.
    dup mask-split                          \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr edg-msk' msk-lst'
    swap mask-deallocate                    \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst'

    dup                                     \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lst'
    foreach                                 \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk bit-mskx
        #7 pick state-xor-mask              \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta'
        dup                                 \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' adj-sta'
        #5 pick                             \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' adj-sta' sqr-lst
        square-list-find                    \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta', sqrx t | f
        if
            #4 pick                         \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' sqrx sqr
            squares-compare                 \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' char
            [char] M =                      \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' bool
            if
                #8 pick                     \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' npnc-lst'
                list-push-struct            \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
            else
                state-deallocate            \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
            then
        else
            #9 pick                         \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk adj-sta' sta-lst'
            list-push-struct                \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst' msk-lnk
        then
    next-item
                                            \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr msk-lst'
    mask-list-deallocate                    \ sta-lst' npnc-lst' sta2 reg1 act0 sqr-lst sqr
    2drop 2drop drop                        \ sta-lst' npnc-lst'

    \ Check npnc-lst list.
    dup list-is-not-empty?
    if
        swap                                \ npnc-lst' sta-lst'
        state-list-deallocate
        true
        exit
    then

    list-deallocate                         \ sta-lst'

    \ Check sta-lst.
    dup list-is-not-empty?
    if
        true
        exit
    then

    list-deallocate
    false
;

\ Return action needs, if any.
: action-get-needs ( cur-sta1 act0 -- ned-lst t | f )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    \ Init return list.
    list-new -rot                   \ ret-lst cur-sta1 act0

    \ Check non-adjacent dissimilar pairs for confirmation need.
    dup action-get-nadj-pairs       \ ret-lst cur-sta1 act0 nadj-lst
    foreach                         \ ret-lst cur-sta1 act0 nadj-lnk regx
        \ Get square from state zero.
        dup region-get-state-0 dup  \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 sta-0
        #4 pick                     \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 sta-0 act0
        action-find-square          \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0, sqrx t | f
        invert abort" square find failed?"

        \ Check if not pnc.
        square-get-pnc              \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 bool
        if
            drop                    \ ret-lst cur-sta1 act0 nadj-lnk regx
        else
            \ Make need.
            over region-str swap    \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0
            actneed-type-cls        \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0 typ
            #5 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0 typ act0
            action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk regx actned

            \ Store need.
            #5 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx actned ret-lst
            list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk regx
        then

        \ Get square from state one.
        dup region-get-state-1 dup  \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 sta-1
        #4 pick                     \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 sta-1 act0
        action-find-square          \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1, sqrx t | f
        invert abort" square find failed?"

        \ Check if not pnc.
        square-get-pnc              \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 bool
        if
            2drop                   \ ret-lst cur-sta1 act0 nadj-lnk
        else
            \ Make need.
            swap region-str swap    \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1
            actneed-type-cls        \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1 typ
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1 typ act0
            action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk actned

            \ Store need.
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk actned ret-lst
            list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk
        then
    next-item
                                    \ ret-lst cur-sta1 act0

    #2 pick list-is-not-empty?
    if
        2drop
        true
        exit
    then

    \ Check non-adjacent dissimilar pairs for sample-between need.
    dup action-get-nadj-pairs       \ ret-lst cur-sta1 act0 nadj-lst
    foreach                         \ ret-lst cur-sta1 act0 nadj-lnk regx
        dup #3 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx regx act0
        action-get-squares          \ ret-lst cur-sta1 act0 nadj-lnk regx regx sqr-lst
        square-list-in-region       \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-lst'
        dup list-get-length         \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-lst' len
        #2 =                        \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-lst' bool
        if
            \ No squares between, get first sample.

            \ Get state between, by randomly selecting half the region X bits.
            square-list-deallocate  \ ret-lst cur-sta1 act0 nadj-lnk regx
            dup                     \ ret-lst cur-sta1 act0 nadj-lnk regx regx
            region-get-state-0 swap \ ret-lst cur-sta1 act0 nadj-lnk sta-0 regx
            region-calc-x-mask      \ ret-lst cur-sta1 act0 nadj-lnk sta-0 xmsk'
            dup mask-half-bits      \ ret-lst cur-sta1 act0 nadj-lnk sta-0 xmsk' hmsk'
            swap mask-deallocate    \ ret-lst cur-sta1 act0 nadj-lnk sta-0 hmsk'
            tuck swap               \ ret-lst cur-sta1 act0 nadj-lnk hmsk' hmsk' sta-0
            state-xor-mask          \ ret-lst cur-sta1 act0 nadj-lnk hmsk' sta-b'
            swap mask-deallocate    \ ret-lst cur-sta1 act0 nadj-lnk sta-b'

            \ Make need.
            over link-get-data      \ ret-lst cur-sta1 act0 nadj-lnk sta-b' regx
            region-str swap         \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-b'
            actneed-type-ils        \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-b' typ
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-b' typ act0
            action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk actneed

            \ Store need.
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk actneed ret-lst
            list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk
        else
            \ Squares between, get additional samples.

            \ Isolate squares between.
            dup                         \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-lst' sqr-lst'
            square-list-non-pnc-squares \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-lst', sqr-non-lst' t | f
            ifnot
                cr #3 pick .action cr
                invert abort" action-get-needs: pnc squares between non-adjacent, dissimilar, squares?"
            then
            \ cr ." non-pnc squares: " dup .square-list cr

            swap square-list-deallocate \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-non-lst'

            \ Get squares with highest number of samples.
            dup square-list-max-samples \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-non-lst', sqr-max-lst' t | f
            invert abort" max samples failed?"
            \ cr ." max num samples squares: " dup .square-list cr

            swap square-list-deallocate \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst'

            \ Generate needs.
            dup                         \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lst'
            foreach                     \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk sqrx
                \ Make need.
                square-get-state        \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk stax
                #3 pick region-str      \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk stax tkn
                swap                    \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk tkn stax
                actneed-type-ils        \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk tkn stax typ
                #7 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk tkn stax typ act0
                action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk actned

                \ Store need.
                #7 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk actned ret-lst
                list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst' sqr-max-lnk
            next-item
                                        \ ret-lst cur-sta1 act0 nadj-lnk regx sqr-max-lst'
            square-list-deallocate      \ ret-lst cur-sta1 act0 nadj-lnk regx
            drop                        \ ret-lst cur-sta1 act0 nadj-lnk
        then
    next-item
                                        \ ret-lst cur-sta1 act0

    #2 pick list-is-not-empty?
    if
        2drop
        true
        exit
    then

    \ Check adjacent dissimilar pairs for confirmation need.
    dup action-get-adj-pairs        \ ret-lst cur-sta1 act0 nadj-lst
    foreach                         \ ret-lst cur-sta1 act0 nadj-lnk regx
        \ Get square from state zero.
        dup region-get-state-0 dup  \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 sta-0
        #4 pick                     \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 sta-0 act0
        action-find-square          \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0, sqrx t | f
        invert abort" square find failed?"

        \ Check if not pnc.
        square-get-pnc              \ ret-lst cur-sta1 act0 nadj-lnk regx sta-0 bool
        if
            drop                    \ ret-lst cur-sta1 act0 nadj-lnk regx
        else
            \ Make need.
            over region-str swap    \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0
            actneed-type-cls        \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0 typ
            #5 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx tkn sta-0 typ act0
                action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk regx actned

            \ Store need.
            #5 pick                 \ ret-lst cur-sta1 act0 nadj-lnk regx actned ret-lst
            list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk regx
        then

        \ Get square from state one.
        dup region-get-state-1 dup  \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 sta-1
        #4 pick                     \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 sta-1 act0
        action-find-square          \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1, sqrx t | f
        invert abort" square find failed?"

        \ Check if not pnc.
        square-get-pnc              \ ret-lst cur-sta1 act0 nadj-lnk regx sta-1 bool
        if
            2drop                   \ ret-lst cur-sta1 act0 nadj-lnk
        else
            \ Make need.
            swap region-str swap    \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1
            actneed-type-cls        \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1 typ
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk tkn sta-1 typ act0
            action-make-need-dis    \ ret-lst cur-sta1 act0 nadj-lnk actned

            \ Store need.
            #4 pick                 \ ret-lst cur-sta1 act0 nadj-lnk actned ret-lst
            list-push-struct        \ ret-lst cur-sta1 act0 nadj-lnk
        then
    next-item
                                    \ ret-lst cur-sta1 act0
    #2 pick list-is-not-empty?
    if
        2drop
        true
        exit
    then

    \ Check corners for corner discovery needs.
    \ That is check squares adjacent to an anchor,
    \ that are in more than one possible regions.
    dup action-get-corner-clusters              \ ret-lst cur-sta1 act0 crnc-lst
    list-is-not-empty?
    if
        \ Check corner cluster list for corner discovery needs.
    else
        dup action-get-corners                  \ ret-lst cur-sta1 act0 crnc-lst
        list-is-not-empty?
        if
            \ Check corner list for corner discovery needs.
            dup action-get-possible-regions     \ ret-lst cur-sta1 act0 pos-regs
            over  action-get-corners            \ ret-lst cur-sta1 act0 pos-regs crnc-lst
            foreach                             \ ret-lst cur-sta1 act0 pos-regs crnc-lnk crnx
                corner-get-adjacent-states      \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lst
                foreach                         \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk ae-sta
                    #3 pick                     \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk ae-sta pos-regs
                    region-list-num-state-in    \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk num-in
                    1 >                         \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk bool
                    if
                        cr ." check ae state " dup link-get-data .state
                        dup link-get-data       \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk ae-sta
                        #3 pick                 \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk ae-sta pos-regs
                        region-list-state-in    \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in'
                        space ." in: " dup .region-list
                        dup                     \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in'
                        foreach                 \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk regx
                            space ." reg " dup .region
                            #3 pick             \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk regx ae-lnk
                            link-get-data swap  \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk stax regx
                            #7 pick             \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk stax regx act0
                            action-state-possible-anchor-for-region \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk bool
                            if
                                space ." pos anchor"
                                #2 pick link-get-data   \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk stax
                                over link-get-data      \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk stax regx
                                #7 pick                 \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk stax regx act0
                                action-anchor-states-to-test    \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk, sta-lst' t | f
                                if
                                    space ." test: " dup .state-list
                                    dup                 \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lst'
                                    foreach             \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay
                                        \ Build token for actneed.

                                        \ Start token.
                                        s" adjacent, external, to " \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay c-addr u
                                        token-new                   \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay, tkn' t | f
                                        invert abort" token-new failed?"

                                        \ Add state string to token.
                                        #6 pick link-get-data       \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' stax
                                        state-str-pad               \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' c-addr u
                                        #2 pick token-concatenate   \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' bool
                                        invert abort" token-conatenate failed?"

                                        s"  in "                    \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' c-addr u
                                        #2 pick token-concatenate   \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' bool
                                        invert abort" token-conatenate failed?"

                                        #4 pick link-get-data       \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' regx
                                        region-str-pad              \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' c-addr u
                                        #2 pick token-concatenate   \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay tkn' bool
                                        invert abort" token-conatenate failed?"

                                        dup space .token
                                        token-deallocate            \ ret-lst cur-sta1 act0 pos-regs crnc-lnk ae-lnk regs-in' regs-in-lnk sta-lst' sta-lnk stay
                                        drop
                                    next-item
                                    state-list-deallocate
                                then
                            else
                                space ." not pos anchor"
                            then
                        next-item
                        cr
                        region-list-deallocate
                    then
                next-item
                                                \ ret-lst cur-sta1 act0 pos-regs crnc-lnk
            next-item
                                                \ ret-lst cur-sta1 act0 pos-regs
            drop                                \ ret-lst cur-sta1 act0
        then
    then

    \ Check if the state is in any group.
    2dup action-get-groups          \ ret-lst cur-sta1 act0 cur-sta1 grp-lst
    group-list-get-needs            \ ret-lst cur-sta1 act0, ned-lst' t | f
    if
        dup                         \ ret-lst cur-sta1 act0 ned-lst' ned-lst'
        #2 pick                     \ ret-lst cur-sta1 act0 ned-lst' ned-lst' act0
        action-set-ids              \ ret-lst cur-sta1 act0 ned-lst'

        dup                         \ ret-lst cur-sta1 act0 ned-lst' ned-lst'
        #4 pick                     \ ret-lst cur-sta1 act0 ned-lst' ned-lst' ret-lst
        list-append-struct          \ ret-lst cur-sta1 act0 ned-lst'

        actneed-list-deallocate     \ ret-lst cur-sta1 act0
    then
                                    \ ret-lst cur-sta1 act0

    \ Get more needs, per defining, non-pnc, group.
    dup action-get-defining-regions \ ret-lst cur-sta1 act0 def-lst
    foreach                         \ ret-lst cur-sta1 act0 def-lnk regx
        #2 pick                     \ ret-lst cur-sta1 act0 def-lnk regx act0
        action-find-group           \ ret-lst cur-sta1 act0 def-lnk, grpx t | f
        if
            dup group-get-pnc       \ ret-lst cur-sta1 act0 def-lnk grpx bool
            if
                drop
            else
                group-get-needs         \ ret-lst cur-sta1 act0 def-lnk, ned-lst' t | f
                if
                    dup #3 pick             \ ret-lst cur-sta1 act0 def-lnk ned-lst' ned-lst' act0
                    action-set-ids          \ ret-lst cur-sta1 act0 def-lnk ned-lst'

                    dup                     \ ret-lst cur-sta1 act0 def-lnk ned-lst' ned-lst'
                    #5 pick                 \ ret-lst cur-sta1 act0 def-lnk ned-lst' ned-lst' ret-lst
                    list-append-struct      \ ret-lst cur-sta1 act0 def-lnk ned-lst'
                    actneed-list-deallocate \ ret-lst cur-sta1 act0 def-lnk
                then
            then
        then
    next-item
                                    \ ret-lst cur-sta1 act0
    2drop                           \ ret-lst

   \ cr ." action needs found: " dup list-get-length dec. cr

    check-list-return
;
