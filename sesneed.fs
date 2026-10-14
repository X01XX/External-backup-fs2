\ Implement a sesneed struct and functions.

#23173 constant sesneed-struct-id
    #3 constant sesneed-struct-number-cells

\ Struct fields
0                           constant sesneed-header-disp    \ 16 bits' [0] struct id, [1] use count, [2] Type (8 bits).
sesneed-header-disp cell+   constant sesneed-target-disp    \ A statecorr.
sesneed-target-disp cell+   constant sesneed-actneed-disp   \ Zero, or an ActNeed instance.

0 value sesneed-mma \ Storage for sesneed mma instance.

\ Need type values and names.
 1 value sesneed-type-min
#1 value sesneed-type-act      \ An Action need.
#2 value sesneed-type-exn      \ Session, exit negative statecorr.
#3 value sesneed-type-spos     \ Session, seek positive statecorr.
#3 value sesneed-type-max

\ Return true if tos is a valid sesneed type.
: is-sesneed-type? ( u -- bool )
    dup  sesneed-type-min >=    \ u bool
    swap sesneed-type-max <=    \ bool bool
    and
;

\ Init sesneed mma, return the addr of allocated memory.
: sesneed-mma-init ( num-items -- ) \ sets sesneed-mma.
    dup 1 <
    abort" sesneed-mma-init: Invalid number of items."

    cr ." Initializing SesNeed store."
    sesneed-struct-number-cells swap mma-new to sesneed-mma
;

\ Check if tos is an allocated sesneed.
: is-sesneed? ( tos -- bool )
    dup sesneed-mma mma-is-item?    \ tos bool
    if
        struct-get-id
        sesneed-struct-id =         \ bool
    else
        drop
        false
    then
;

\ Start accessors.

\ Return the target field from a sesneed instance.
: sesneed-get-target ( ned0 -- targ )
    \ Check arg.include sesneed.fs
    assert( tos is-sesneed? )

    sesneed-target-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the target field from a sesneed instance, use only in this file.
: _sesneed-set-target ( targ1 ned0 -- )

    sesneed-target-disp +  \ Add offset.
    !struct             \ Set the field.
;

: sesneed-get-type ( ned0 -- type )
    \ Check arg.
    assert( tos is-sesneed? )

    4c@
;

: _sesneed-set-type ( typ1 ned0 -- )
    4c!
;

: sesneed-get-actneed ( ned -- actneed | 0 )
    \ Check arg.
    assert( tos is-sesneed? )

    sesneed-actneed-disp +  \ Add offset.
    @                       \ Fetch the field.
;

: _sesneed-set-actneed ( tkn ned -- )
    sesneed-actneed-disp +
    over 0=
    if
        !
    else
        !struct
    then
;

\ End accessors.

\ Create a sesneed given:
\ A sesneed type.
\ A target.
\ An actneed, or zero.
\
: sesneed-new ( actned2 targ1 typ0 -- addr )
    \ Check args.
    assert( tos is-sesneed-type? )
    assert( nos is-statecorr? )
    assert( 3os 0= if true else 3os is-actneed? then )

    \ Allocate instance.
    sesneed-struct-id sesneed-mma
    struct-allocate                 \  actned2 targ1 typ0 ned

    \ Store fields.
    tuck _sesneed-set-type          \ actned2 targ1 ned
    tuck _sesneed-set-target        \ actned2 ned
    tuck _sesneed-set-actneed       \ ned
;

\ Print a sesneed.
: .sesneed ( ned0 -- )
    \ Check arg.
    assert( tos is-sesneed? )

    ." ( sesneed targ: "
    dup sesneed-get-target .statecorr

    dup sesneed-get-type
    case
        sesneed-type-act of
            dup sesneed-get-actneed space ." ActNeed: " .actneed
        endof
        sesneed-type-exn of
            space ." Exit negative states"
        endof
        sesneed-type-spos of
            space ." Seek positive states"
        endof
        ." Unrecognized type value" abort
    endcase
    drop
    ." )"
;

\ Deallocate a sesneed.
: sesneed-deallocate ( ned0 -- )
    \ Check arg.
    assert( tos is-sesneed? )

    dup struct-get-use-count      \ ned0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        dup sesneed-get-target statecorr-deallocate
        dup sesneed-get-actneed 0> if dup sesneed-get-actneed actneed-deallocate then

        \ Deallocate instance.
        sesneed-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if nos satisfies a sesneed.
: sesneed-satisfied-by? ( sta1 ned0 -- bool )
    \ Check arg.
    assert( tos is-sesneed? )
    assert( nos is-statecorr? )

    sesneed-get-target             \ sta1 targ

    statecorrs-eq?
;
