\ Implement an Action struct and functions.                                                                                                       

#29717 constant action-id
    #5 constant action-struct-number-cells

\ Struct fields
0                                       constant action-header-disp             \ 16 bits, [0] Struct id, [1] Use count [2] Number bits ( 8 bits ).
action-header-disp              cell+   constant action-squares-disp            \ A square list.
action-squares-disp             cell+   constant action-incompatible-pairs-disp \ A region list.  States that define the regions are incompatible.
action-incompatible-pairs-disp  cell+   constant action-possible-regions-disp   \ A region list.
action-possible-regions-disp    cell+   constant action-groups-disp             \ A group list.

0 value action-mma \ Storage for action mma instance.

\ Init action mma, return the addr of allocated memory.
: action-mma-init ( num-items -- ) \ sets action-mma.
    dup 1 <
    abort" action-mma-init: Invalid number of items."

    cr ." Initializing Action store."
    action-struct-number-cells swap mma-new to action-mma
;

\ Check instance type.
: is-allocated-action? ( addr -- bool )
    dup action-mma mma-is-item  \ addr bool
    if
        struct-get-id
        action-id =             \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for action, unconventional, leaves stack unchanged.
: assert-tos-is-action ( tos -- tos )
    dup is-allocated-action?
    false? if
        s" TOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-tos-is-action to assert-tos-is-action-xt

\ Check NOS for action, unconventional, leaves stack unchanged.
: assert-nos-is-action ( nos tos -- nos tos )
    over is-allocated-action?
    false? if
        s" NOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-nos-is-action to assert-nos-is-action-xt

\ Check 3OS for action, unconventional, leaves stack unchanged.
: assert-3os-is-action ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-action?
    false? if
        s" 3OS is not an allocated action"
       .abort-xt execute
    then
;

\ Start accessors.

\ Get the number of bits.
: action-get-num-bits ( act0 -- nb )
    \ Check arg.
    assert-tos-is-action

    4c@
;

\ Set the number of bits.
: _action-set-num-bits ( nb act0 -- )
    4c!
;

\ Return the square-list from an action instance.
: action-get-squares ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-squares-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square-list of an action instance, use only in this file.
: _action-set-squares ( sqr-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square-list

    action-squares-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the incompatible pairs list from an action instance.
: action-get-incompatible-pairs ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-incompatible-pairs-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the incompatible-pairs list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    action-incompatible-pairs-disp +    \ Add offset.
    !struct                             \ Set the field.
;

\ Update the incompatible-pairs list of an action instance, use only in this file.
: _action-update-incompatible-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    dup action-get-incompatible-pairs     \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-incompatible-pairs        \ pos-regs
    region-list-deallocate
;

\ Return the possible-regions list from an action instance.
: action-get-possible-regions ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-possible-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the possible-regions list of an action instance, use only in this file.
: _action-set-possible-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    action-possible-regions-disp +  \ Add offset.
    !struct                         \ Set the field.
;

\ Update the possible-regions list of an action instance, use only in this file.
: _action-update-possible-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    dup action-get-possible-regions     \ reg-lst1 act0 pos-regs
    -rot                                \ pos-regs reg-lst1 act0
    _action-set-possible-regions        \ pos-regs
    region-list-deallocate
;

\ Return the square-list from an action instance.
: action-get-groups ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-groups-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square-list of an action instance, use only in this file.
: _action-set-groups ( grp-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-group-list

    action-groups-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors

: action-new ( num-bits -- addr)

    \ Allocate space.
    action-id action-mma                \ nb id mma
    struct-allocate                     \ nb act

    \ Set number bits.
    2dup _action-set-num-bits           \ nb act

    \ Set squares list.
    list-new                            \ nb act lst
    over _action-set-squares            \ nb act

    \ Set incompatible-pairs list.
    list-new                            \ nb act lst
    over                                \ nb act lst act
    _action-set-incompatible-pairs      \ nb act

    \ Set possible-regions list.
    list-new                            \ nb act lst
    rot                                 \ act lst nb
    region-max-x                        \ act lst reg-max
    over list-push-struct               \ act lst
    over                                \ act lst act
    _action-set-possible-regions        \ act

    \ Set initial group list.
    list-new over _action-set-groups    \ act
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    cr ." Action: "
    cr #4 spaces ." Squares:        " dup action-get-squares .square-list
    cr #4 spaces ." Incompat pairs: " dup action-get-incompatible-pairs .region-list
    cr #4 spaces ." Poss regions:   " dup action-get-possible-regions .region-list
    cr #4 spaces ." Groups:         " action-get-groups .group-list
    cr
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        dup action-get-incompatible-pairs region-list-deallocate
        dup action-get-possible-regions region-list-deallocate
        dup action-get-groups group-list-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Find a square, given a state.
: action-find-square ( sta1 act0 -- sqr t | f )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-state

    action-get-squares      \ sta1 sqr-lst
    square-list-find        \ sqr t | f
;

\ Check an existing square, changed by a new result.
: action-check-changed-square ( sqr1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    2drop
    cr ." action-check-changed-square: todo" cr
;

\ Add a group to the group list.
: action-add-group ( grp1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-group
    cr ." Action " space ." Adding group: " over .group cr

    action-get-groups        \ grp1 grp-lst
    list-push-struct
;

\ Check a new square.
: action-check-new-square ( sqr1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square
    \ cr ." action-check-new-square: start: " .stack-gbl cr

    over square-get-state over      \ sqr1 act0 sta act0
    action-get-groups               \ sqr1 act0 sta grp-lst

    group-list-superset-of-state    \ sqr1 act0, grp-lst t | f
    if
        cr ." new square in groups: " dup .group-list-regions cr
        cr ." action-check-new-square: todo" cr
        group-list-deallocate
    else
        cr ." new square not in any groups. " cr
        over square-get-state               \ sqr1 act0 sta
        over action-get-possible-regions    \ sqr1 act0 sta reg-lst
        region-list-regions-state-in        \ sqr1 act0 regs-in-lst
        cr ." new square in possible regions: " dup .region-list cr

        \ Check each region for new group, or new incompatible pair.
        dup list-get-links                  \ sqr1 act0 regs-in-lst' regs-lnk

        begin
            ?dup
        while
            dup link-get-data                   \ sqr1 act0 regs-in-lst' regs-lnk regx
            #3 pick                             \ sqr1 act0 regs-in-lst' regs-lnk regx act0
            action-get-squares                  \ sqr1 act0 regs-in-lst' regs-lnk regx sqr-lst
            square-list-in-region               \ sqr1 act0 regs-in-lst' regs-lnk sqr-in-lst'
            dup                                 \ sqr1 act0 regs-in-lst' regs-lnk sqr-in-lst' sqr-in-lst'
            square-list-find-incompatible-pair  \ sqr1 act0 regs-in-lst' regs-lnk sqr-in-lst', sqr-pr t | f
            if
                \ Process incompatible pair.
                cr ." todo" abort
            else
                \ Add new group.
                over link-get-data              \ sqr1 act0 regs-in-lst' regs-lnk sqr-is-lst' regx
                group-new                       \ sqr1 act0 regs-in-lst' regs-lnk grp
                #3 pick                         \ sqr1 act0 regs-in-lst' regs-lnk grp act0
                action-add-group                \ sqr1 act0 regs-in-lst' regs-lnk
            then

            link-get-next
        repeat

        
        region-list-deallocate
    then

    2drop
    \ cr ." action-check-new-square: end: " .stack-gbl cr
;

\ Add a new square to the action square list.
: action-add-new-square ( sqr1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square
    \ cr ." action-add-new-square: start: " .stack-gbl cr

    over square-get-state       \ sqr1 act0 sta
    over action-find-square     \ sqr1 act0, sqr t | f
    if
        cr ." square already exists in square list" abort
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
    assert-tos-is-action
    assert-nos-is-sample
    \ cr ." action-add-sample: start: " .stack-gbl cr

    over sample-get-initial     \ smpl1 act0 initial
    over action-find-square     \ smpl1 act0, sqr t | f
    if
        rot                     \ act0 sqr smpl1
        over                    \ act0 sqr smpl1 sqr
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
        over                    \ smpl1 act0 sqr1 act0
        action-add-new-square   \ smpl1 act0
        2drop
        true
    then
    \ cr ." action-add-sample: end: " .stack-gbl cr
;

