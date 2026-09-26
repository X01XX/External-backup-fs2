
#37171 constant planstep-struct-id
    #4 constant planstep-struct-number-cells

\ Struct fields
0                                   constant planstep-header-disp       \ 16-bits [0] struct id [1] use count.
planstep-header-disp        cell+   constant planstep-actionstep-disp   \ ActionStep to take.
planstep-actionstep-disp    cell+   constant planstep-from-disp         \ From StateCorr.
planstep-from-disp          cell+   constant planstep-to-disp           \ To StateCorr.

0 value planstep-mma \ Storage for planstep mma instance.

\ Init planstep mma, return the addr of allocated memory.
: planstep-mma-init ( num-items -- ) \ sets planstep-mma.
    dup 1 <
    abort" planstep-mma-init: Invalid number of items."

    cr ." Initializing PlanStep store."
    planstep-struct-number-cells swap mma-new to planstep-mma
;

\ Check if tos is an allocated planstep.
: is-planstep? ( tos -- flag )
    dup planstep-mma mma-is-item? \ addr bool
    if
        struct-get-id
        planstep-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the actionstep field from a planstep instance.
: planstep-get-actionstep ( plnstp0 -- actstp )
    \ Check arg.
    assert( tos is-planstep? )

    planstep-actionstep-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the from actionstep field from a planstep instance, use only in this file.
: _planstep-set-actionstep ( actstp1 plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )
    assert( nos is-actionstep? )

    planstep-actionstep-disp +  \ Add offset.
    !struct                     \ Set the field.
;

\ Return the from statecorr field from a planstep instance.
: planstep-get-from ( plnstp0 -- sta0 )
    \ Check arg.
    assert( tos is-planstep? )

    planstep-from-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the from statecorr field from a planstep instance, use only in this file.
: _planstep-set-from ( stac1 plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )
    assert( nos is-statecorr? )

    planstep-from-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return the "to" statecorr field from a planstep instance.
: planstep-get-to ( plnstp0 -- stac1 )
    \ Check arg.
    assert( tos is-planstep? )

    \ Get second state.
    planstep-to-disp +      \ Add offset.
    @                       \ Fetch the field.
;

\ Set the "to" statecorr field from a planstep instance, use only in this file.
: _planstep-set-to ( stac1 plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )
    assert( nos is-statecorr? )

    planstep-to-disp +      \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a planstep from two statecorrs on the stack.
\ The statecorrs cannot be the same.
: planstep-new ( stac2 stac1 actstp0 -- plnstp )
    \ Check args.
    assert( tos is-actionstep? )
    assert( nos is-statecorr? )
    assert( 3os is-statecorr? )
    assert( #2 pick #2 pick statecorrs-eq? invert )

    \ Allocate instance.
    planstep-struct-id planstep-mma \ stac2 stac1 actstp0 id mma
    struct-allocate                 \ stac2 stac1 actstp0 plnstp

    \ Store fields.
    tuck _planstep-set-actionstep   \ stac2 stac1 plnstp
    tuck _planstep-set-from         \ stac2 plnstp
    tuck _planstep-set-to           \ plnstp
;

\ Print a planstep.
: .planstep ( plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )

    s" ( plnstp " type
    space dup planstep-get-actionstep .actionstep
    space ." From: " dup planstep-get-from .statecorr
    space ." To: " planstep-get-to .statecorr
    s" )" type
;

\ Deallocate a planstep.
: planstep-deallocate ( plnstp0 -- )
    \ Check arg.
    assert( tos is-planstep? )

    dup struct-get-use-count      \ plnstp0 count
    dup 0< abort" planstep-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup planstep-get-actionstep actionstep-deallocate
        dup planstep-get-from statecorr-deallocate
        dup planstep-get-to statecorr-deallocate

        \ Deallocate instance.
        planstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;
