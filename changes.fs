\ Implement the changes struct and functions.
\
\ This is used to limit the generated needs of a Domain to what is achievable,
\ given the current state and all possible predictable action/group changes.
\ A bit position may never change, or its changeability may vary over time.
\
\ Rule pairs can be mutually exclusive, or have a required order.
\
\ It may not be possible to return to the initial state, which the
\ earliest actions change, and add the changes to the rule store.
\ You don't know how you got to the initial state, it just is.
\
\ So the prediction of what is achievable may be over-optimistic.

#31973 constant changes-id
    #3 constant changes-struct-number-cells

\ Struct fields.
0                           constant changes-header-disp    \ 16-bits [0] struct id, [1] use count.
changes-header-disp cell+   constant changes-m01-disp       \ 0->1 mask.
changes-m01-disp    cell+   constant changes-m10-disp       \ 1->0 mask.

0 value changes-mma    \ Storage for changes mma instance.

\ Init changes mma, return the addr of allocated memory.
: changes-mma-init ( num-items -- ) \ sets changes-mma.
    dup 1 <
    abort" changes-mma-init: Invalid number of items."

    cr ." Initializing Changes store."
    changes-struct-number-cells swap mma-new to changes-mma
;

\ Check instance type.

: is-allocated-changes? ( addr -- bool )
    dup changes-mma mma-is-item \ addr bool
    if
        struct-get-id
        changes-id =            \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for changes, unconventional, leaves stack unchanged.
: assert-tos-is-changes ( tos -- tos )
    dup is-allocated-changes?
    false? if
        s" TOS is not an allocated changes."
       .abort-xt execute
    then
;

\ Check NOS for changes, unconventional, leaves stack unchanged.
: assert-nos-is-changes ( nos tos -- nos tos )
    over is-allocated-changes?
    false? if
        s" NOS is not an allocated changes."
       .abort-xt execute
    then
;

\ Check 3OS for changes, unconventional, leaves stack unchanged.
: assert-3os-is-changes ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-changes?
    false? if
        s" 3OS is not an allocated changes."
       .abort-xt execute
    then
;

\ Check 4OS for changes, unconventional, leaves stack unchanged.
: assert-4os-is-changes ( 4os 3os nos tos -- 4os 3os nos tos )
    #3 pick is-allocated-changes?
    false? if
        s" 4OS is not an allocated changes."
       .abort-xt execute
    then
;

\ Check 5OS for changes, unconventional, leaves stack unchanged.
: assert-5os-is-changes ( 4os 3os nos tos -- 4os 3os nos tos )
    #4 pick is-allocated-changes?
    false? if
        s" 5OS is not an allocated changes."
       .abort-xt execute
    then
;

\ Start accessors.

\ Return the m01 field of a changes instance.
: changes-get-m01 ( cngs0 -- u)
    \ Check arg.
    assert-tos-is-changes

    changes-m01-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the m01 field of a changes instance, use only in this file.
: _changes-set-m01 ( u1 cngs0 -- )
    changes-m01-disp +  \ Add offset.
    !                   \ Set the field.
;

\ Return the m10 field of a changes instance.
: changes-get-m10 ( cngs0 -- u)
    \ Check arg.
    assert-tos-is-changes

    changes-m10-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the m10 field of a changes instance, use only in this file.
: _changes-set-m10 ( u1 cngs0 -- )
    changes-m10-disp +  \ Add offset.
    !                   \ Set the field.
;

\ End accessors.

\ Allocate a changes, setting id and use count only, use only in this file.
: _changes-allocate ( -- cngs )
    \ Allocate space.
    changes-id changes-mma      \ id mma
    struct-allocate             \ cngs
;

\ Create a changes from two numbers on the stack.
: changes-new ( msk-m10 msk-m01 -- addr)
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    _changes-allocate       \ m10 m01 addr

    \ Store fields.
    tuck                    \ m10 addr m01 addr
    _changes-set-m01        \ m10 addr

    tuck                    \ addr m10 addr
    _changes-set-m10        \ addr
;

\ Deallocate a changes.
: changes-deallocate ( cngs0 -- )
    \ Check arg.
    assert-tos-is-changes

    dup struct-get-use-count      \ cngs0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate instance.
        changes-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return the union of two changes.
: changes-calc-union ( cngs1 cngs0 -- cngs )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    over changes-get-m10    \ cngs1 cngs0 1m10
    over changes-get-m10    \ cngs1 cngs0 1m10 0m10
    or                      \ cngs1 cngs0 m10'
    -rot                    \ m10' cngs1 cngs0
    changes-get-m01         \ m10' cngs1 0m01
    swap                    \ m10' 0m01 cngs1
    changes-get-m01         \ m10' 0m01 1m01
    or                      \ m10' m01'
    changes-new             \ cngs
;

\ Return a state with all possible changes applied.
: changes-apply-to-state ( sta1 cngs0 -- sta )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-value

    2dup changes-get-m10        \ sta1 cngs0 sta1 m10
    and                         \ sta1 cngs0 msk10
    swap changes-get-m01        \ sta1 msk10 m01
    #2 pick !not                \ sta1 msk10 m01 ~sta1
    and                         \ sta1 msk10 msk01
    or                          \ sta1 msk
    xor                         \ result state
;

: .changes ( cngs -- )
    \ Check arg.
    assert-tos-is-changes

    ." (m10: " dup changes-get-m10 .value
    ." , m01: " changes-get-m01 .value ." )"
;

\ Put both changes masks on the stack.
: changes-get-masks ( cngs0 -- m10 m01 )
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m10
    swap changes-get-m01
;

\ Return the changes needed to translate a region (TOS) to another (NOS).
\ 0->1 = X->1 union 0->1.
\ 1->0 = X->0 union 1->0.
: change-masks-region-to-region ( reg-to reg-from -- m10 m01 )
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-region

    \ Get reg-from masks.
    dup region-x-mask -rot      \ fx reg-to reg-from
    dup region-edge-0-mask -rot \ fx f0 reg-to reg-from
    region-edge-1-mask swap     \ fx f0 f1 reg-to

    \ Get reg-to masks.
    dup region-edge-0-mask swap \ fx f0 f1 t0 reg-to
    region-edge-1-mask          \ fx f0 f1 t0 t1

    \ Calc changes m10.
    #4 pick #2 pick and         \ fx f0 f1 t0 t1 | mx0
    #3 pick #3 pick and         \ fx f0 f1 t0 t1 | mx0 m10
    or                          \ fx f0 f1 t0 t1 | c10

    \ Calc changes m01.
    #5 pick #2 pick and         \ fx f0 f1 t0 t1 | c10 mx1
    #5 pick #3 pick and         \ fx f0 f1 t0 t1 | c10 mx1 m01
    or                          \ fx f0 f1 t0 t1 | c10 c01

    \ Clean up.
    2swap 2drop                 \ fx f0 f1 c10 c01
    2swap 2drop                 \ fx c10 c01
    rot drop                    \ c10 c01
;

\ Return changes needed to translate a region (tos) to intersect with another (nos).
: changes-new-region-to-region ( reg-to reg-from -- cngs )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ cr ." changes-new-region-to-region: from: " dup .region space ." to " over .region
    change-masks-region-to-region           \ m10 m01
    changes-new                             \ cngs
    \ space ." = " dup .changes cr

;

\ Return true if two changes intersect, in at least one bit.
: changes-intersect? ( cngs1 cngs0 -- bool )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    over changes-get-m01        \ cngs1 cngs0 1m01
    over changes-get-m01        \ cngs1 cngs0 1m01 0m01
    and 0<> if
        2drop
        true
        exit
    then

    changes-get-m10             \ cngs1 0m10
    swap changes-get-m10        \ 0m10 1m10
    and
    0<>
;

\ Return true if the m01 and m10 masks have a one in the same position.
: ?changes-duplex ( cngs0 -- bool )
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m01     \ cngs0 m01
    swap                    \ m01 cngs0
    changes-get-m10         \ m01 m10
    and                     \ msk
    0<>                     \ bool
;

\ Return the intersection of two changes.
: changes-intersection ( cngs1 cngs0 -- cngs )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    \ Intersect m01.
    over changes-get-m01        \ cngs1 cngs0 1m01
    over changes-get-m01        \ cngs1 cngs0 1m01 0m01
    and                         \ cngs1 cngs0 m01

    \ Intersect m10.
    #2 pick changes-get-m10     \ cngs1 cngs0 m01 1m10
    #2 pick changes-get-m10     \ cngs1 cngs0 m01 1m10 0m10
    and                         \ cngs1 cngs0 m01 m10

    \ Build result changes.
    _changes-allocate           \ cngs1 cngs0 m01 m10 r-cngs
    tuck _changes-set-m10       \ cngs1 cngs0 m01 r-cngs
    tuck _changes-set-m01       \ cngs1 cngs0 r-cngs

    \ Clean up.
    nip nip
;

: changes-null? ( cngs0 -- bool )    \ Return true if changes masks are all zero.
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m01     \ cngs0 m01
    0<> if
        drop
        false
        exit
    then

    changes-get-m10         \ m10
    0=
;

: changes-not-null? ( cngs0 -- bool )    \ Return true if changes masks are not all zero.
    changes-null?
    invert
;

: changes-invert ( cngs0 -- cngs )  \ Return the inversion of a changes masks.
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m10 !not    \ cngs0 m10'

    swap changes-get-m01 !not   \ m10' m01'

    changes-new                 \ cngs
;

: changes-number-changes ( cngs0 -- u ) \ Return number of 1-bits in the changes masks.
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m10 value-num-bits  \ cngs0 u10
    swap                                \ u10 cngs0
    changes-get-m01 value-num-bits      \ u10 u01
    +
;

\ Return true if two changes are equal.
: changes-eq? ( cngs1 cngs0 -- bool )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    2dup =          \ cngs1 cngs0 bool
    if
        2drop
        true
        exit
    then

    changes-get-masks   \ cngs1 0m10 0m01
    rot                 \ 0m10 0m01 cngs1
    changes-get-masks   \ 0m10 0m01 1m10 1m01
    rot                 \ 0m10 1m10 1m01 0m01
    = -rot              \ bool 0m10 1m10
    =                   \ bool bool
    and                 \ bool
;

\ Return true if the tos changes is a superset of the nos changes.
: changes-superset-of ( cngs-sub cngs-sup -- bool )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    over                    \ cngs-sub cngs-sup cngs-sub
    changes-intersection    \ cngs-sub cngs-int'
    tuck                    \ cngs-int' cngs-sub cngs-int'
    changes-eq?             \ cngs-int' bool
    swap changes-deallocate \ bool
;
