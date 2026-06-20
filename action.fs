\ Implement an Action struct and functions.                                                                                                       

#29717 constant action-id
    #4 constant action-struct-number-cells

\ Struct fields
0                                       constant action-header-disp             \ 16 bits, [0] Struct id, [1] Use count 
action-header-disp              cell+   constant action-squares-disp            \ A square list.
action-squares-disp             cell+   constant action-incompatible-pairs-disp \ A region list.  States that define the regions are incompatible.
action-incompatible-pairs-disp  cell+   constant action-possible-regions-disp   \ A region list.

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

\ End accessors

: action-new ( num-bits -- addr)

    \ Allocate space.
    action-id action-mma                \ nb id mma
    struct-allocate                     \ nb act

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
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    cr ." Action: "
    cr #4 spaces ." Squares:        " dup action-get-squares .square-list
    cr #4 spaces ." Incompat pairs: " dup action-get-incompatible-pairs .region-list
    cr #4 spaces ." Poss regions:   " action-get-possible-regions .region-list
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

\ Add a square to the action square list.
: action-add-square ( sqr1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    over square-get-state       \ sqr1 act0 sta
    over action-find-square     \ sqr1 act0, sqr t | f
    if
        cr ." square already exists in square list" abort
    then

    \ Store the square.
    2dup action-get-squares     \ sqr1 act0 sqr1 sqr-lst
    list-push-struct            \ sqr1 act0

    \ Clean up, return.
    2drop
;

\ Add a sample, return true if the sample changed
\ a square.
: action-add-sample ( smpl1 act0 -- bool )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-sample

    over sample-get-initial     \ smpl1 act0 initial
    over action-find-square     \ smpl1 act0, sqr t | f
    if
        #2 pick                 \ smpl1 act0 sqr smpl1
        swap                    \ smpl1 act0 smpl1 sqr
        square-add-sample       \ smpl1 act0 bool
        nip nip
    else
        over                    \ smpl1 act0 smpl1
        square-new              \ smpl1 act0 sqr1
        over                    \ smpl1 act0 sqr1 act0
        action-add-square       \ smpl1 act0
        2drop
        true
    then
;

