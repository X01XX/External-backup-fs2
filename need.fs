\ Implement a need struct and functions.

#19717 constant need-struct-id
    #3 constant need-struct-number-cells

\ Struct fields
0                           constant need-header-disp   \ 16 bits' [0] struct id, [1] use count, [2] Type (8 bits).
                                                        \          Action instance id (8 bits) [3] Domain instance id.
need-header-disp    cell+   constant need-target-disp   \ A state or region-corr.
need-target-disp    cell+   constant need-info-disp     \ Zero, a region, or other struct, depending on need type.

0 value need-mma \ Storage for need mma instance.

\ Need type values and names.
 1 value need-type-min
 1 value need-type-snig     \ State not in the s-region of any group
#2 value need-type-cls      \ Confirm logical structure
#3 value need-type-ils      \ Improve logical structure
#4 value need-type-cg       \ Confirm group
#5 value need-type-cas      \ Corner anchor state
#6 value need-type-cds      \ Corner adjacent, dissimilar, state
#7 value need-type-exn      \ Session, exit negative state.
#8 value need-type-spos     \ Session, seek positive state.
#8 value need-type-max

\ Return true if tos is a valid need type.
: is-need-type? ( u -- bool )
    dup  need-type-min >=    \ u bool
    swap need-type-max <=   \ bool bool
    and
;

\ Init need mma, return the addr of allocated memory.
: need-mma-init ( num-items -- ) \ sets need-mma.
    dup 1 <
    abort" need-mma-init: Invalid number of items."

    cr ." Initializing Need store."
    need-struct-number-cells swap mma-new to need-mma
;

\ Check if tos is an allocated need.
: is-need? ( tos -- bool )
    dup need-mma mma-is-item?   \ tos bool
    if
        struct-get-id
        need-struct-id =        \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the domain instance id.
: need-get-dom-inst-id ( ned0 -- id )
    \ Check arg.
    assert( tos is-need? )

    6c@                 \ Fetch the field.
;

\ Set the domain instance id, use only in this file.
: _need-set-dom-inst-id ( id1 ned0 -- )
    \ Check args.

    6c!                 \ Set the field.
;

\ Return the action instance id.
: need-get-act-inst-id ( ned0 -- act )
    \ Check arg.
    assert( tos is-need? )

    5c@                 \ Fetch the field.
;

\ Set the action instance id, use only in this file.
: _need-set-act-inst-id ( id1 ned0 -- )
    \ Check args.

    5c!                 \ Set the field.
;

\ Return the target field from a need instance.
: need-get-target ( ned0 -- targ )
    \ Check arg.
    assert( tos is-need? )

    need-target-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Return true if tos is a valid target.
: is-target? ( arg -- bool )
    dup is-state?
    if drop true exit then

    is-region-list?
;

\ Set the target field from a need instance, use only in this file.
: _need-set-target ( targ1 ned0 -- )

    need-target-disp +  \ Add offset.
    !struct             \ Set the field.
;

: need-get-type ( ned0 -- type )
    \ Check arg.
    assert( tos is-need? )

    4c@
;

: _need-set-type ( typ1 ned0 -- )

    4c!
;

: need-get-info ( ned -- tkn )
    \ Check arg.
    assert( tos is-need? )

    need-info-disp +    \ Add offset.
    @                   \ Fetch the field.
;

: _need-set-info ( tkn ned -- )
    over 0=
    if
        need-info-disp +
        !
    else
        need-info-disp +
        !struct
    then
;

\ End accessors.

\ Create a need given:
\ A token for extra info, may be zero.
\ A target.
\ A need type.
\ A action intance id.
\ A domain instance id.
\
: need-new ( inf5 targ4 typ3 act-id1 dom-id0 -- addr) \ For non-test code, call action-make-need instead of this.
    \ Check args.
    assert( tos is-valid-inst-id? )
    assert( nos is-valid-inst-id? )
    assert( 3os is-need-type? )

    \ Check extra info.
    #2 pick
    case
        need-type-snig of
            5os 0<> abort" Invalid extra info, s/b zero."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-cls of
            5os is-region? invert abort" Invalid extra info' s/b region."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-ils of
            5os is-region? invert abort" Invalid extra info' s/b region."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-cg of
            5os is-region? invert abort" Invalid extra info' s/b region."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-cas of
            5os is-corner? invert abort" Invalid extra info' s/b corner."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-cds of
            5os is-corner? invert abort" Invalid extra info' s/b corner."
            4os is-state? invert abort" Invalid target, s/b a state."
        endof
        need-type-exn of
            5os 0<> abort" Invalid extra info, s/b zero."
            4os is-region-list? invert abort" Invalid target, s/b a region-list."
        endof
        need-type-spos of
            5os 0<> abort" Invalid extra info, s/b zero."
            4os is-region-list? invert abort" Invalid target, s/b a region-list."
        endof
        cr ." Unrecognized need type value" cr abort
    endcase

    \ Allocate space.
    need-struct-id need-mma
    struct-allocate                 \  tkn5 targ4 typ3 act-id1 dom-id0 ned

    \ Store fields.
    tuck _need-set-dom-inst-id      \ tkn5 targ4 typ3 act-id1 ned
    tuck _need-set-act-inst-id      \ tkn5 targ4 typ3 ned
    tuck _need-set-type             \ tkn5 targ4 ned
    tuck _need-set-target           \ tkn
    tuck _need-set-info             \ ned
;

: .target ( targ0 -- )
    assert( tos is-target? )

    dup is-state?
    if
        .state
        exit
    then

    dup is-region-list?
    if
        .region-list
    else
        cr ." .target should not happen" cr
        abort
    then
;

: .need-dom-act-targ ( ned -- )
    ." Dom: " dup need-get-dom-inst-id #3 dec.r space
    ." Act: " dup need-get-act-inst-id #3 dec.r space

    ." Target: "
    need-get-target .target
;

\ Print a need.
: .need ( ned0 -- )
    \ Check arg.
    assert( tos is-need? )

    need-get-type
    case
        need-type-snig of
            .need-dom-act-targ
            space ." State not in a group"
        endof
        need-type-cls of
            dup .need-dom-act-targ
            space ." Confirm incompatible pair "
            need-get-info
            region-get-states .state space ." and " .state
        endof
        need-type-ils of
            dup .need-dom-act-targ
            space ." Get sample between incompatible pair "
            need-get-info
            region-get-states .state space ." and " .state
        endof
        need-type-cg of
            dup .need-dom-act-targ
            space ." Confirm group "
            need-get-info .region
        endof
        need-type-cas of
            dup .need-dom-act-targ
            space ." Corner confirm anchor square "
            need-get-info .corner
        endof
        need-type-cds of
            dup .need-dom-act-targ
            space ." Corner confirm dissimilar square "
            need-get-info .corner
        endof
        need-type-exn of
            ." Target: "
            need-get-target .target
            space ." Exit negative state"
        endof
        need-type-spos of
            ." Target: "
            need-get-target .target
            space ." Seek positive state "
        endof
        ." Unrecognized type value" abort
    endcase
;

: target-deallocate ( targ0 -- )
    assert( tos is-target? )

    dup is-state?
    if
        state-deallocate
        exit
    then

    dup is-region?
    if
        region-deallocate
        exit
    then

    dup is-region-list?
    if
        region-list-deallocate
    else
        cr ." target-deallocate: should not happen" cr
        abort
    then
;


\ Deallocate a need.
: need-deallocate ( ned0 -- )
    \ Check arg.
    assert( tos is-need? )

    dup struct-get-use-count      \ ned0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        dup need-get-target target-deallocate
        dup need-get-info 0> if dup need-get-info token-deallocate then

        \ Deallocate instance.
        need-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if nos satisfies a need.
: need-satisfied-by? ( nos ned0 -- bool )
    \ Check arg.
    assert( tos is-need? )

    need-get-target             \ nos targ

    \ Check if target is a state.
    dup is-state?
    if
        \ nos s/b a state.
        over is-state?
        if
            states-eq?
            exit
        else
            cr ." need-satisfied-by?: argument mismatch?" .stack-gbl cr abort
        then
    then

    \ Check if target is a region.
    dup is-region?
    if
        \ nos s/b a state or region.
        over is-state?
        if
            region-superset-of-state?
            exit
        then
        over is-region?
        if
            region-superset?
            exit
        then
        cr ." need-satisfied-by?: argument mismatch?" .stack-gbl cr abort
    then

    \ Check if target is a region-list.
    dup is-region-list?
    if
        \ nos s/b a region-list or state-list.
        over is-region-list?
        if
            region-list-corr-superset?
            exit
        then
        over is-state-list?
        if
            region-list-corr-superset-of-states?
            exit
        then
    else
        cr ." need-satisfied-by? invalid target?" cr abort
    then
    cr ." need-satisfied-by? invalid nos?" .stack-gbl cr abort
;
