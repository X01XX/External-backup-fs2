\ Implement an Action struct and functions.

#29717 constant action-struct-id
   #12 constant action-struct-number-cells

\ Struct fields
0                                                   constant action-header-disp                         \ 16 bits, [0] Struct id, [1] Use count [2] Number bits ( 8 bits )
                                                                                                        \ Action instance ID ( 8 bits ).
action-header-disp                          cell+   constant action-parent-disp                         \ Domain ref, or 0.
action-parent-disp                          cell+   constant action-squares-disp                        \ A square list.
action-squares-disp                         cell+   constant action-incompatible-pairs-disp             \ A region list.  States that define the regions are incompatible.
action-incompatible-pairs-disp              cell+   constant action-possible-regions-disp               \ A region list.
action-possible-regions-disp                cell+   constant action-states-in-one-region-disp           \ A state list of states in only one possible region, possible anchors.
action-states-in-one-region-disp            cell+   constant action-defining-regions-disp               \ A list of possible regions that are defining regions.
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

\ Return the parent from an action instance.
: action-get-parent ( act0 -- dom )
    \ Check arg.
    assert( tos is-action? )

    action-parent-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the parent of an action instance, use only in this file.
\ Do not inc parent use count.
: _action-set-parent ( dom1 act0 -- )
    action-parent-disp +    \ Add offset.
    !                       \ Set the field.
;

\ Get the number of bits.
: action-get-num-bits ( act0 -- nb )
    \ Check arg.
    assert( tos is-action? )

    4c@
;

\ Set the number of bits.
: _action-set-num-bits ( nb act0 -- )
    4c!
;

\ Get the action id.
: action-get-inst-id ( act0 -- id )
    \ Check arg.
    assert( tos is-action? )

    5c@
;

' action-get-inst-id to action-get-inst-id-xt

\ Set the action id.
: _action-set-inst-id ( id act0 -- )
    5c!
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

\ Return the incompatible pairs list from an action instance.
: action-get-incompatible-pairs ( act0 -- reg-lst )
    \ Check arg.
    assert( tos is-action? )

    action-incompatible-pairs-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the incompatible-pairs list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    action-incompatible-pairs-disp +    \ Add offset.
    !struct                             \ Set the field.
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

\ Update the possible-regions list of an action instance, use only in this file.
: _action-update-possible-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-region-list? )

    dup action-get-possible-regions     \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-possible-regions        \ pos-regs
    region-list-deallocate
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
    \ cr ." _action-set-corner-clusters: start: " .stack-gbl cr
    assert( tos is-action? )
    assert( nos is-corner-lol? )

    action-corner-clusters-disp +   \ Add offset.
    !struct                         \ Set the field.
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

\ End accessors

\ Return a new action, given a functian to run to get a sample,
\ and the number of bits being used.
: action-new ( xt num-bits inst-id parent -- addr)
    assert( dup if tos is-domain?-xt execute else true then )
    assert( nos 0 >= )
    assert( nos #256 < )
    assert( 3os 1 >= )
    assert( 3os [ 1 cells #8 * ] literal <= )

    \ Allocate space.
    action-struct-id action-mma         \ xt nb inst-id parent struct-id mma
    struct-allocate                     \ xt nb inst-id parent act

    \ Set parent.
    tuck _action-set-parent             \ xx num-bits inst-id act

    \ Set inst id.
    tuck _action-set-inst-id            \ xt nb act

    \ Set number bits.
    2dup _action-set-num-bits           \ xt nb act

    \ Set squares list.
    list-new                            \ xt nb act lst
    over _action-set-squares            \ xt nb act

    \ Set incompatible-pairs list.
    list-new                            \ xt nb act lst
    over                                \ xt nb act lst act
    _action-set-incompatible-pairs      \ xt nb act

    \ Set possible-regions list.
    list-new                            \ xt nb act lst
    rot                                 \ xt act lst nb
    region-max-x                        \ xt act lst reg-max
    over list-push-struct               \ xt act lst
    over                                \ xt act lst act
    _action-set-possible-regions        \ xt act

    \ Set initial group list.
    list-new over _action-set-groups    \ xt act

    \ Set function.
    tuck _action-set-function           \ act

    \ Set squares-in-one-region.
    list-new over _action-set-states-in-one-region

    \ Set defining regions.
    list-new over _action-set-defining-regions

    \ Set states-not-in-defining-regions.
    list-new over _action-set-states-not-in-defining-regions

    \ Set corners.
    list-new over _action-set-corners

    \ Set corner clusters.
    list-new over _action-set-corner-clusters
;

: action-squares-in-one-region ( act0 -- sqr-lst )
    \ Check arg.
    assert( tos is-action? )

    \ Init return list.
    list-new                    \ act0 ret-lst

    \ Prep for loop.
    over action-get-possible-regions    \ act0 ret-lst pos-lst
    #2 pick action-get-squares          \ act0 ret-lst pos-lst sqr-lst

    foreach                             \ act0 ret-lst pos-lst sqr-lnk
        dup link-get-data               \ act0 ret-lst pos-lst sqr-lnk sqrx
        square-get-state                \ act0 ret-lst pos-lst sqr-lnk sta
        #2 pick                         \ act0 ret-lst pos-lst sqr-lnk sta pos-lst
        region-list-num-state-in        \ act0 ret-lst pos-lst sqr-lnk u
        1 =                             \ act0 ret-lst pos-lst sqr-lnk bool
        if
            dup link-get-data           \ act0 ret-lst pos-lst sqr-lnk sqrx
            #3 pick                     \ act0 ret-lst pos-lst sqr-lnk sqrx ret-lst
            list-push-struct            \ act0 ret-lst pos-lst sqr-lnk
        then
    next
                                \ act0 ret-lst pos-lst
    drop nip                    \ ret-lst
;

\ Print parent domain id, if any.
\ Action parent domain ref may be zero.
: .action-parent ( act0 -- )
   \ Check arg.
    assert( tos is-action? )

    action-get-parent           \ dom
    dup ifnot drop exit then    \ Print nothing.

    cr ." .action-parent: todo " cr
    drop
    \ domain-get-id             \ dom-id
    \ ." Dom: " dec.
;

' .action-parent to .action-parent-xt

\ Print a action.
: .action ( act0 -- )
    \ cr ." .action: start: " .stack-gbl cr
    \ Check arg.
    assert( tos is-action? )

    cr ." Action:"
    cr
    s"     Squares:              " #2 pick action-get-squares .square-list-prefix
    cr
    #4 spaces ." Incompatable pairs:   " dup action-get-incompatible-pairs .region-list
    cr cr
    #4 spaces ." Possible regions:     " dup action-get-possible-regions .region-list
    cr cr
    #4 spaces ." Sqrs in one poss reg: " dup action-get-states-in-one-region .state-list
    cr cr
    #4 spaces ." Defining regions:     " dup action-get-defining-regions .region-list
    cr cr
    #4 spaces ." Sqrs not in def regs: " dup action-get-states-not-in-defining-regions .state-list
    cr
    s"     Corners:              " #2 pick action-get-corners .corner-list-prefix
    cr
    s"     Corner clusters:      " #2 pick action-get-corner-clusters .corner-clusters-prefix
    cr
    s"     Groups:               " #2 pick action-get-groups .group-list-prefix
    drop
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert( tos is-action? )

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        dup action-get-incompatible-pairs region-list-deallocate
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

\ Add a group to the group list.
: action-add-group ( grp1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-group? )
    cr ." Action " space ." Adding group: " over .group cr

    action-get-groups        \ grp1 grp-lst
    list-push-struct
;

\ Scan the group list to delete groups that have a region
\ that is not in the possible regions list.
: _action-delete-non-possible-groups ( act0 -- )
    \ Check arg.
    assert( tos is-action? )

    \ Init group list to delete.
    list-new                            \ act0 del-grps

    \ Scan group list, gathering groups to remove from the list.
    over action-get-possible-regions    \ act0 del-grps pos-regs
    #2 pick action-get-groups           \ act0 del-grps pos-regs grp-lst

    foreach                             \ act0 del-grps pos-regs grp-lnk
        [ ' regions-eq? ] literal       \ act0 del-grps pos-regs grp-lnk xt
        over link-get-data              \ act0 del-grps pos-regs grp-lnk xt grpx
        group-get-region                \ act0 del-grps pos-regs grp-lnk xt regx
        #3 pick                         \ act0 del-grps pos-regs grp-lnk xt regx pos-regs
        list-member?                    \ act0 del-grps pos-regs grp-lnk bool
        ifnot
            dup link-get-data           \ act0 del-grps pos-regs grp-lnk grpx
            #3 pick                     \ act0 del-grps pos-regs grp-lnk grpx del-grps
            list-push-struct            \ act0 del-grps pos-regs grp-lnk
        then
    next

    \ Remove the groups from the action group list.
                                        \ act0 del-grps pos-regs
    drop                                \ act0 del-grps
    over action-get-groups              \ act0 del-grps grps-lst
    over                                \ act0 del-grps grps-lst del-grps

    foreach                             \ act0 del-grps grp-lst del-lnk
        [ ' = ] literal                 \ act0 del-grps grp-lst del-lnk xt
        over link-get-data              \ act0 del-grps grp-lst del-lnk xt grpx
        #3 pick                         \ act0 del-grps grp-lst del-lnk xt grpx grp-lst
        list-remove                     \ act0 del-grps grp-lst del-lnk, grpx t | f
        if
            cr ." Orphan group deleted: " dup .group-region cr
            struct-dec-use-count        \ act0 del-grps grp-lst del-lnk
        else
            cr ." remove failed?" cr abort
        then
    next
                                        \ act0 grp-lst grp-lst
                                        \ act0 del-grps grp-lst
    drop                                \ act0 del-grps
    group-list-deallocate               \ act0      The groups are deallocated here.
    drop
;

\ Scan the possible regions list, when a region is not represented in the
\ group list, and has at least one square subset to it,
\ try to add the group.
: _action-add-possible-groups ( act0 -- )
    \ cr ." _action-add-possible-groups: start" cr
    \ Check arg.
    assert( tos is-action? )

    \ Scan group list.
    dup action-get-groups               \ act0 grp-lst
    over action-get-possible-regions    \ act0 grp-lst pos-regs

    foreach                             \ act0 grp-lst pos-lnk
        dup link-get-data               \ act0 grp-lst pos-lnk pos-reg
        #2 pick                         \ act0 grp-lst pos-lnk pos-reg grp-lst
        group-list-member?              \ act0 grp-lst pos-lnk bool
        ifnot
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
                group-new               \ act0 grp-lst pos-lnk in-lst', grp t | f
                if
                    nip                 \ act0 grp-lst pos-lnk grp
                    #3 pick             \ act0 grp-lst pos-lnk grp act0
                    action-add-group    \ act0 grp-lst pos-lnk
                else
                    square-list-deallocate  \ act0 grp-lst pos-lnk
                then
            then
        then
    next
                                        \ act0 pos-regs
    2drop
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

    foreach                                 \ act0 rslt-lst pos-lst sta-lnk
        \ Get region state is in.
        dup link-get-data                   \ act0 rslt-lst pos-lst sta-lnk stax
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
    next
                                            \ act0 rslt-lst pos-lst
    drop nip
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
    foreach                             \ act0 rslt-lst def-regs sta-lst' sta-lnk
        dup link-get-data               \ act0 rslt-lst def-regs sta-lst' sta-lnk stax
        #3 pick                         \ act0 rslt-lst def-regs sta-lst' sta-lnk stax def-regs
        region-list-any-superset-state? \ act0 rslt-lst def-regs sta-lst' sta-lnk bool
        ifnot
            \ Add state to result list.
            dup link-get-data           \ act0 rslt-lst def-regs sta-lst' sta-lnk stax
            #4 pick                     \ act0 rslt-lst def-regs sta-lst' sta-lnk stax rslt-lst
            list-push-struct            \ act0 rslt-lst def-regs sta-lst' sta-lnk
        then
    next
                                        \ act0 rslt-lst def-regs sta-lst'
    state-list-deallocate               \ act0 rslt-lst def-regs
    drop                                \ act0 rslt-lst
    nip
;

\ Return a rate for a corner.
: action-rate-corner ( crn1 act0 -- rt )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-corner? )
    \ cr ." action-rate-corner: start: " .stack-gbl cr

    action-get-possible-regions         \ crn1 pos-lst
    swap corner-get-adjacent-states     \ pos-lst adj-lst

    \ Init counter.
    0 swap                              \ pos-lst cnt adj-lst

    foreach                             \ pos-lst cnt adj-lnk
        dup link-get-data               \ pos-lst cnt adj-lnk stax
        #3 pick                         \ pos-lst cnt adj-lnk stax pos-lst
        region-list-num-state-in        \ pos-lst cnt adj-lnk num-in
        1 =                             \ pos-lst cnt adj-lnk bool
        if
            \ Inc counter.
            swap 1+ swap                \ pos-lst cnt adj-lnk
        then
    next
                                        \ pos-lst cnt
    nip
    \ cr ." action-rate-corner: end: " .stack-gbl cr
    \ cr ." action-rate-corner: end: rate: " dup . cr
;

\ Calc corners, from action-defining-regions and
\ action-squares-in-one-region.
: action-calc-corners ( act0 -- crn-lst )
    \ Check arg.
    assert( tos is-action? )
    \ cr ." action-calc-corners: start: " .stack-gbl cr

    \ Init corner cluster list.
    list-new over _action-update-corner-clusters

    \ Init preliminary list.
    list-new swap                       \ pre-lst act0
    dup action-get-states-in-one-region \ pre-lst act0 stas-in1
    over action-get-defining-regions    \ pre-lst act0 stas-in1 def-lst

    foreach                             \ pre-lst act0 stas-in1 def-lnk
        \ Get squares in only the current defining region.
        dup link-get-data               \ pre-lst act0 stas-in1 def-lnk regx
        #2 pick                         \ pre-lst act0 stas-in1 def-lnk regx stas-in1
        state-list-in-region            \ pre-lst act0 stas-in1 def-lnk stas-in-reg'

        \ cr ." For defining region: " over link-get-data .region space ." squares in: " dup .state-list cr
        dup                             \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-reg'
        foreach                         \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk
            \ Make corner.
            dup link-get-data           \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax
            #3 pick                     \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax def-lnk
            link-get-data               \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk stax regx
            corner-new                  \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn

            \ Store corner.
            #6 pick                     \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk crn pre-lst
            list-push-struct            \ pre-lst act0 stas-in1 def-lnk stas-in-reg' stas-in-lnk
        next

        state-list-deallocate
    next
                                        \ pre-lst act0 stas-in1
    drop                                \ pre-lst act0

    \ Rate each corner.
    over                                \ pre-lst act0 pre-lst
    foreach                             \ pre-lst act0 pre-lnk
        \ Calc rank.
        dup link-get-data               \ pre-lst act0 pre-lnk crnx
        #2 pick                         \ pre-lst act0 pre-lnk crnx act0
        action-rate-corner              \ pre-lst act0 pre-lnk rt

        \ Set rank.
        over link-get-data              \ pre-lst act0 pre-lnk rt crnx
        corner-set-rate                 \ pre-lst act0 pre-lnk
    next
                                        \ pre-lst act0

    \ Init return list.
    list-new                            \ pre-lst act0 ret-lst

    \ Copy defining regions list.
    over action-get-defining-regions    \ pre-lst act0 ret-lst def-lst
    list-copy-struct

    \ while any defining regions left.
    begin
        dup list-get-length
    while

        \    init a corner sub-list.
        list-new                            \ pre-lst act0 ret-lst def-lst crn-sub-lst

        \ Get highest corner rate.
        0                                   \ pre-lst act0 ret-lst def-lst crn-sub-lst max
        #5 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst max pre-lst

        foreach                             \ pre-lst act0 ret-lst def-lst crn-sub-lst max pre-lnk
            dup link-get-data               \ pre-lst act0 ret-lst def-lst crn-sub-lst max pre-lnk crnx
            corner-get-rate                 \ pre-lst act0 ret-lst def-lst crn-sub-lst max pre-lnk rt
            rot                             \ pre-lst act0 ret-lst def-lst crn-sub-lst pre-lnk rt max
            max                             \ pre-lst act0 ret-lst def-lst crn-sub-lst pre-lnk max
            swap                            \ pre-lst act0 ret-lst def-lst crn-sub-lst max pre-lnk
        next
                                            \ pre-lst act0 ret-lst def-lst crn-sub-lst max

        \ Get higest ranked corners from pre-list.

        \ Init max corner list.
        list-new                            \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst
        #6 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lst

        foreach                             \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk
            dup link-get-data               \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk crnx
            corner-get-rate                 \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk rt
            #3 pick                         \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk rt max
            = if                            \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk bool
                \ Add corner to sub-list.
                dup link-get-data           \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk crnx
                #2 pick                     \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk crnx max-crn-lst
                list-push-struct            \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst pre-lnk
            then
        next
                                            \ pre-lst act0 ret-lst def-lst crn-sub-lst max max-crn-lst
        nip                                 \ pre-lst act0 ret-lst def-lst crn-sub-lst max-crn-lst

        \ select a corner from max-crn-lst.
        dup list-get-length                 \ pre-lst act0 ret-lst def-lst crn-sub-lst max-crn-lst len
        random                              \ pre-lst act0 ret-lst def-lst crn-sub-lst max-crn-lst inx
        over list-remove-item-struct        \ pre-lst act0 ret-lst def-lst crn-sub-lst max-crn-lst crnx
        swap corner-list-deallocate         \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx

        \ Delete region from copied defining regions list.
        [ ' = ] literal                     \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx xt
        over corner-get-region              \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx xt crn-reg
        #4 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx xt crn-reg def-lst
        list-remove-struct                  \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx, reg t | f
        ifnot cr ." region not found?" abort then
        drop                                \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx

        \ Add to selected corner to sub-list.
        2dup swap list-push-struct          \ pre-lst act0 ret-lst def-lst crn-sub-lst crnx

        \ Delete corners from pre list that have an anchor in the selected corner region.
        corner-get-region                   \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-reg
        #5 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-reg pre-lst
        corner-list-remove-all-region-match \ pre-lst act0 ret-lst def-lst crn-sub-lst

        \ For each corner in the list, which starts with only one corner, find corners with anchors
        \ that match the adjacent, external, states.
        \ Add found corners to the end of the list, extending the list while it is
        \ being interated on.
        dup                                 \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lst
        \ cr ." at 1: " .stack-gbl cr
        foreach                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk

            \ For each corner in the crn-sub-lst, find corners with an anchor equal to a corner adjacent state.
            dup link-get-data               \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk crnx
            corner-get-adjacent-states      \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lst
            \ cr ." at 2: " .stack-gbl cr
            foreach                         \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk
                dup link-get-data           \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk adjx
                \ cr ." checking state: " dup .state cr
                \ cr ." at 3: " .stack-gbl cr
                #7 pick                     \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk adjx pre-lst
                \ cr ." at 4: " .stack-gbl cr
                corner-list-find            \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk, crn t | f
                if
                    \ Delete region from copied defining regions list.
                    [ ' = ] literal                     \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn xt
                    over corner-get-region              \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn xt crn-reg
                    #6 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn xt crn-reg def-lst
                    list-remove-struct                  \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn, reg t | f
                    ifnot cr ." region not found?" abort then
                    drop                                \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn
                    \ cr ." at 5: " .stack-gbl cr

                    \ Add selected corner to sub-list.
                    dup                                 \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn crn
                    #4 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn crn crn-sub-lst
                    list-push-end-struct                \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn
                    \ cr ." at 6: " .stack-gbl cr

                    \ Delete corners from pre list that have an anchor in the selected corner region.
                    corner-get-region                   \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn-reg
                    #7 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk crn-reg pre-lst
                    corner-list-remove-all-region-match \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lnk adj-lnk
                    \ cr ." at 7: " .stack-gbl cr

                then
            next    \ Corner adjacent state.
        next        \ Corner.

        \ Add corner sub-list to return list.
        dup                                     \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lst
        #3 pick                                 \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-sub-lst ret-lst
        list-append-struct                      \ pre-lst act0 ret-lst def-lst crn-sub-lst

        \ Dealloc, or Add corner-sub-list to action corners.
        dup list-get-length                     \ pre-lst act0 ret-lst def-lst crn-sub-lst len
        1 >
        if
            #3 pick                             \ pre-lst act0 ret-lst def-lst crn-sub-lst act0
            action-get-corner-clusters          \ pre-lst act0 ret-lst def-lst crn-sub-lst crn-clstr-lst
            list-push-end-struct                \ pre-lst act0 ret-lst def-lst
        else
            corner-list-deallocate              \ pre-lst act0 ret-lst def-lst
        then
    repeat

    \ Delete copied defining list.
    region-list-deallocate                      \ pre-lst act0 ret-lst

    \ deallocate pre-list.
    nip swap corner-list-deallocate             \ ret-lst

    \ cr ." action-calc-corners: end: returning: " dup .list-raw cr

    \ cr ." action-calc-corners: end: " .stack-gbl cr
;

\ Evaluate possible regions, to generate corners.
: _action-evaluate-possible-regions ( act0 -- )
    \ cr ." _action-evaluate-possible-regions: start"
    \ Check arg.
    assert( tos is-action? )

    \ Update action-squares-in-one-region.
    dup action-squares-in-one-region                    \ act0 sqr-lst'
    dup square-list-states                              \ act0 sqr-lst' sta-lst'
    #2 pick _action-update-states-in-one-region         \ act0 sqr-lst'
    square-list-deallocate                              \ act0
    
    \ Update defining regions.
    dup action-defining-regions                         \ act0 reg-lst
    over _action-update-defining-regions                \ act0 

    \ Update states-not-in-defining-regions.
    dup action-states-not-in-defining-regions           \ act0 sta-lst
    over _action-update-states-not-in-defining-regions  \ act0

    dup action-calc-corners                             \ act0 crn-lst
    over _action-update-corners                         \ act0

    drop
;

\ Add an incompatible pair, updating incompatible pair list and possible regions list.
: _action-add-incompatible-pair ( sqr-pr act0 -- )
    \ cr ." _action-add-incompatible-pair: start: " .stack-gbl cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square-pair? )

    \ Add square pair to action-incompatible-pairs.
    swap square-pair-to-region          \ act0 reg'
    dup                                 \ act0 reg' reg'
    #2 pick                             \ act0 reg' reg' act0
    action-get-incompatible-pairs       \ act0 reg' reg' pr-lst
    region-list-push-nosups             \ act0 reg' bool
    if
        \ Adjust action-possible-regions.
        dup region-get-state-0          \ act0 reg sta0
        swap region-get-state-1         \ act0 sta0 sta1
        #2 pick                         \ act0 sta0 sta1 act0
        action-get-possible-regions     \ act0 sta0 sta1 pos-regs
        regionlist-cumulative-~a+~b     \ act0 pos-regs2
        over                            \ act0 pos-regs2 act0
        _action-update-possible-regions \ act0
        dup _action-evaluate-possible-regions

        \ Update groups.
        dup _action-delete-non-possible-groups
        \ dup _action-add-possible-groups
    else
        cr ." problem? push-nosups action-check-possible-regions-for-incompatible-pairfailed?"
        region-deallocate
    then
                                        \ act0
    drop
    \ cr ." _action-add-incompatible-pair: end: " .stack-gbl cr
;

\ Using action-incompatible-pairs, recalc all possible regions.
: action-recalc-possible-regions ( act0 -- )
    \ Check arg.
    assert( tos is-action? )

    \ Recalc possible regions.
    list-new                                \ act0 pos-new
    over action-get-num-bits                \ act0 pos-new nb
    region-max-x                            \ act0 pos-new reg-max
    over list-push-struct                   \ act0 pos-new

    over action-get-incompatible-pairs      \ act0 pos-new pr-lst

    foreach                                 \ act0 pos-new pr-lnk
        dup link-get-data                   \ act0 pos-new pr-lnk regx
        region-get-states                   \ act0 pos-new pr-lnk sta1 sta1
        state-~a+~b                         \ act0 pos-new pr-lnk reg-lst'
        dup                                 \ act0 pos-new pr-lnk reg-lst' reg-lst'
        #3 pick                             \ act0 pos-new pr-lnk reg-lst' reg-lst' pos-new
        region-list-intersections-nosubs    \ act0 pos-new pr-lnk reg-lst' pos-new2

        \ Clean up.
        swap region-list-deallocate         \ act0 pos-new pr-lnk pos-new2
        rot region-list-deallocate          \ act0 pr-lnk pos-new2
        swap                                \ act0 pos-new2 pr-lnk
    next
                                            \ act0 pos-new
    swap                                    \ pos-new act0
    tuck _action-update-possible-regions    \ act0
    _action-evaluate-possible-regions       \
;

\ Check the effect on incompatible pairs of a changed square.
\ If any pairs become Compatible, they are deleted, recalc possible
\ regions and delete groups that no longer match a possible region.
\ This may be intensive, since every pair must be recalculated and
\ intersected.
: action-check-incompatible-pairs-for-changed-square ( sqr1 act0 -- )
    \ cr ." action-check-incompatible-pairs-for-changed-square: start: " .stack-gbl cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )

    over square-pn1-samples2            \ sqr1 act0 bool
    if
        \ This change should not affect incompatible pairs.
        2drop
        \ cr ." action-check-incompatible-pairs-for-changed-square: exit 1: " .stack-gbl cr
        exit
    then

    \ Check each pair, accumulate pairs to delete.

    \ Init delete list.
    list-new                            \ sqr1 act0 del-lst

    \ Prep for loop.
    #2 pick square-get-state            \ sqr1 act0 del-lst sta1
    #2 pick                             \ sqr1 act0 del-lst sta1 act0
    action-get-incompatible-pairs       \ sqr1 act0 del-lst sta1 pr-lst

    foreach                             \ sqr1 act0 del-lst sta1 pr-lnk
        over                            \ sqr1 act0 del-lst sta1 pr-lnk sta1
        over link-get-data              \ sqr1 act0 del-lst sta1 pr-lnk sta1 regx
        region-uses-state?              \ sqr1 act0 del-lst sta1 pr-lnk bool
        if
            dup link-get-data           \ sqr1 act0 del-lst sta1 pr-lnk regx
            dup region-get-state-0      \ sqr1 act0 del-lst sta1 pr-lnk regx r-sta0
            #3 pick                     \ sqr1 act0 del-lst sta1 pr-lnk regx r-sta0 sta1
            states-eq?                  \ sqr1 act0 del-lst sta1 pr-lnk regx bool
            \ Get the other state.
            if
                \ Check state 1
                region-get-state-1      \ sqr1 act0 del-lst sta1 pr-lnk r-sta
            else
                \ Check state 0
                region-get-state-0      \ sqr1 act0 del-lst sta1 pr-lnk r-sta
            then
            \ Compare with sqr1.
            #4 pick                     \ sqr1 act0 del-lst sta1 pr-lnk r-sta1 act0
            action-find-square          \ sqr1 act0 del-lst sta1 pr-lnk, sqr t | f
            if
                #5 pick                 \ sqr1 act0 del-lst sta1 pr-lnk sqr sqr1
                squares-compare         \ sqr1 act0 del-lst sta1 pr-lnk char
                \ Allow pairs to go to More Samples Needed. The normal
                \ confirmation by seeking pnc for each square will push
                \ it to Compatible or Incompatible.
                \ If it goes to Incompatible, a complete recalc will be
                \ avoided.
                [char] C =
                if
                    dup link-get-data   \ sqr1 act0 del-lst sta1 pr-lnk regx
                    #3 pick             \ sqr1 act0 del-lst sta1 pr-lnk regx det-lst
                    list-push-struct    \ sqr1 act0 del-lst sta1 pr-lnk
                then
            else
                cr ." square not found?" abort
            then
        then
    next
                                        \ sqr1 act0 del-lst sta1
    drop                                \ sqr1 act0 del-lst

    \ cr ." action-check-incompatible-pairs-for-changed-square: process del list: " .stack-gbl cr

    \ Process del list.
    dup list-is-empty?                  \ sqr1 act0 del-lst bool
    if
        list-deallocate
        2drop
        \ cr ." action-check-incompatible-pairs-for-changed-square: exit 2: " .stack-gbl cr
        exit
    then

    \ Remove pairs.
    dup                                 \ sqr1 act0 del-lst del-lst

    foreach                             \ sqr1 act0 del-lst del-lnk
        [ ' = ] literal                 \ sqr1 act0 del-lst del-lnk xt
        over link-get-data              \ sqr1 act0 del-lst del-lnk xt regx
        cr ." Deleting incompatible pair, it became compatible: " dup .region cr
        #4 pick                         \ sqr1 act0 del-lst del-lnk xt regx act0
        action-get-incompatible-pairs   \ sqr1 act0 del-lst del-lnk xt regx pr-lst
        list-remove-struct              \ sqr1 act0 del-lst del-lnk, reg t | f
        if
            drop
        else
            cr ." problem? region not found"
        then
    next
                                        \ sqr1 act0 del-lst
    region-list-deallocate              \ sqr1 act0

    \ cr ." action-check-incompatible-pairs-for-changed-square: recalc possible regions: " .stack-gbl cr

    dup
    action-recalc-possible-regions      \ sqr1 act0

    \ cr ." action-check-incompatible-pairs-for-changed-square: del groups that no longer match" cr

    \ Delete groups that no longer match a possible region.
    dup
    _action-delete-non-possible-groups  \ sqr1 act0

    2drop
    \ cr ." action-check-incompatible-pairs-for-changed-square: end: " .stack-gbl cr
;

\ Check the possible region list for problems with a given state.
\ Return true if all affected regions are Ok.
: action-check-possible-regions-for-incompatible-pairs2 ( sta1 act0 -- bool )
    \ cr ." check-possible-regions-for-incompatible-pairs2: start" cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-state? )

    \ Init square pair list.
    list-new -rot                                   \ pr-lst sta1 act0

    \ Scan group list.
    dup action-get-squares                          \ pr-lst sta1 act0 sqr-lst
    over action-get-possible-regions                \ pr-lst sta1 act0 sqr-lst pos-regs

    foreach                                         \ pr-lst sta1 act0 sqr-lst pos-lnk
        #3 pick                                     \ pr-lst sta1 act0 sqr-lst pos-lnk sta1
        over link-get-data                          \ pr-lst sta1 act0 sqr-lst pos-lnk sta1 pos-reg
        region-superset-of-state?                   \ pr-lst sta1 act0 sqr-lst pos-lnk bool
        if
            \ Get squares in region.
            dup link-get-data                       \ pr-lst sta1 act0 sqr-lst pos-lnk pos-reg
            #2 pick                                 \ pr-lst sta1 act0 sqr-lst pos-lnk pos-reg sqr-lst
            square-list-in-region                   \ pr-lst sta1 act0 sqr-lst pos-lnk in-lst'
            dup list-is-empty?
            if
                list-deallocate
            else
                dup                                 \ pr-lst sta1 act0 sqr-lst pos-lnk in-lst' in-lst'
                square-list-find-incompatible-pair  \ pr-lst sta1 act0 sqr-lst pos-lnk in-lst', sqr-pr t | f
                if
                    swap square-list-deallocate     \ pr-lst sta1 act0 sqr-lst pos-lnk sqr-pr
                    #5 pick                         \ pr-lst sta1 act0 sqr-lst pos-lnk sqr-pr pr-lst
                    list-push-struct                \ pr-lst sta1 act0 sqr-lst pos-lnk
                else
                    square-list-deallocate          \ pr-lst sta1 act0 sqr-lst pos-lnk
                then
            then
        then
    next
                                                    \ pr-lst sta1 act0 sqr-lst
    drop                                            \ pr-lst sta1 act0
    nip                                             \ pr-lst act0
    over square-pair-list-choose-pair               \ pr-lst act0, sqr-pr t | f
    if
        \ Add the incompatible square pair, altering possible regions.
        over _action-add-incompatible-pair          \ pr-lst act0
        drop                                        \ pr-lst
        square-pair-list-deallocate
        false
    else                                            \ pr-lst act0
        drop                                        \ pr-lst
        square-pair-list-deallocate
        true
    then
    \ cr ." check-possible-regions-for-incompatible-pairs2: end" cr
;

\ Check possible regions, containing a given state,
\ for incompatible square pairs, until no more found.
: action-check-possible-regions-for-incompatible-pairs ( sta1 act0 -- )
    \ cr ." check-possible-regions-for-incompatible-pairs: start" cr
    \ Check arg.
    assert( tos is-action? )
    assert( nos is-state? )

    \ Try once.
    2dup action-check-possible-regions-for-incompatible-pairs2  \ sta1 act0 bool
    if
        \ No group changes required.
        2drop
        exit
    then

    \ Try as many more times as needed.
    begin
        2dup action-check-possible-regions-for-incompatible-pairs2
    until
                                                                \ sta1 act0

    dup _action-delete-non-possible-groups                      \ sta1 act0

    2drop
    \ cr ." check-possible-regions-for-incompatible-pairs: end" cr
;

\ Check an existing square, changed by a new result.
: action-check-changed-square ( sqr1 act0 -- )
    \ cr ." action-check-changed-square: start: " .stack-gbl cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )

    \ Check incompatible pairs, if needed.
    over square-pn1-samples2                \ sqr1 act0 bool
    ifnot
        \ Check incompatible pairs due to a pn, or pnc, change.
        2dup action-check-incompatible-pairs-for-changed-square
    then

    over square-get-state                   \ sqr1 act0 sta
    over                                    \ sqr1 act0 sta act0
    action-check-possible-regions-for-incompatible-pairs

    \ Check for new groups.
    dup _action-add-possible-groups         \ sqr1 act0

    2drop
    \ cr ." action-check-changed-square: end: " .stack-gbl cr
;

\ Add anew square to a list of groups the square is known to be in.
: _action-add-new-square-to-groups ( sqr2 grp-lst act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-group-list? )
    assert( 3os is-square? )

    over                            \ sqr2 grp-lst act0 grp-lst

    foreach                         \ sqr2 grp-lst act0 grp-lnk
        #3 pick over link-get-data  \ sqr2 grp-lst act0 grp-lnk sqr2 grpx
        group-superset-square?      \ sqr2 grp-lst act0 grp-lnk sqr2 grpx
        if
            #3 pick over            \ sqr2 grp-lst act0 grp-lnk sqr2 grp-lnk
            link-get-data           \ sqr2 grp-lst act0 grp-lnk sqr2 grpx
            group-add-new-square    \ sqr2 grp-lst act0 grp-lnk
        then
    next

    \ cr ." action-add-new-square-to-groups: end" cr
    2drop drop
;

\ Check a new square.
: action-check-new-square ( sqr1 act0 -- )
    \ cr ." action-check-new-square: start: " .stack-gbl cr
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    over square-get-num-samples 1 > abort" action-check-new-square: new square has gt 1 samples?"

    over square-get-state over                              \ sqr1 act0 sta1 act0

    action-check-possible-regions-for-incompatible-pairs    \ sqr1 act0

    over square-get-state over              \ sqr1 act0 sta act0
    action-get-groups                       \ sqr1 act0 sta grp-lst
    group-list-superset-of-state            \ sqr1 act0, grp-lst' t | f
    if
        cr ." action-check-new-square: square in groups: " dup .group-list-regions cr
        #2 pick over #3 pick                \ sqr1 act0 grp-lst' sqr1 grp-lst1' act0
        _action-add-new-square-to-groups    \ sqr1 act0 grp-lst'
        group-list-deallocate
    then

    \ Some possible regions may only have the new square in them.
    \ cr ." at 1: " .stack-gbl cr
    dup _action-add-possible-groups         \ sqr1 act0
    2drop
   \  cr ." at 2: " .stack-gbl cr

    \ cr ." action-check-new-square: end: " .stack-gbl cr
;

\ Add a new square to the action square list.
: action-add-new-square ( sqr1 act0 -- )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-square? )
    \ cr ." action-add-new-square: start: " .stack-gbl cr

    over square-get-state       \ sqr1 act0 sta
    over action-find-square     \ sqr1 act0, sqr t | f
    if
        cr ." action-add-new-square: square already exists in square list" abort
    then

    \ Store the square.
    2dup action-get-squares     \ sqr1 act0 sqr1 sqr-lst
    list-push-struct            \ sqr1 act0

    action-check-new-square
    \ cr ." action-add-new-square: end: " .stack-gbl cr
;

\ Add a sample, return true if the sample changed
\ a square.
: action-add-sample ( smpl1 act0 -- bool )
    \ Check args.
    assert( tos is-action? )
    assert( nos is-sample? )
    \ cr ." Action: add sample: " over .sample cr
    \ cr ." action-add-sample: start: " .stack-gbl cr

    over sample-get-initial     \ smpl1 act0 initial
    over action-find-square     \ smpl1 act0, sqr t | f
    if
        rot                     \ act0 sqr smpl1
        over                    \ act0 sqr smpl1 sqr
        cr ." Action: Updating square: " dup .square cr
        square-add-sample       \ act0 sqr bool
        if
            swap                        \ sqr act0
            action-check-changed-square \
            true
        else
            2drop
            false
        then
    else
        over                    \ smpl1 act0 smpl1
        square-new              \ smpl1 act0 sqr1
        cr ." Action: Adding new square: " dup .square cr
        over                    \ smpl1 act0 sqr1 act0
        action-add-new-square   \ smpl1 act0
        2drop
        true
    then
    \ cr ." action-add-sample: end: " .stack-gbl cr
;

\ Return action needs.
: action-calc-needs ( act0 -- nd-lst )
    \ Check arg.
    assert( tos is-action? )

    \ Non-adjacent square pairs, where both states are not in any defining regions.

    \ pnc for selected corner anchors.

    \ pnc for selected corner ae squares.

    \ Confirm defining groups.

    drop
;
