\ Implement a struct and functions for a step within a regioncorr.

#53197 constant pathstep-struct-id
    #4 constant pathstep-struct-number-cells

\ Struct fields
0                               constant pathstep-header-disp            \ 16-bits [0] struct id [1] use count
pathstep-header-disp    cell+   constant pathstep-from-disp              \ A regioncorr.
pathstep-from-disp      cell+   constant pathstep-to-disp                \ A regioncorr.
pathstep-to-disp        cell+   constant pathstep-within-disp            \ A regioncorr.

0 value pathstep-mma    \ Storage for region mma instance.

\ Init pathstep mma, return an address of allocated memory.
: pathstep-mma-init ( num-items -- ) \ sets pathstep-mma.
    dup 1 <
    abort" pathstep-mma-init: Invalid number of items."

    cr ." Initializing PathStep store."
    pathstep-struct-number-cells swap mma-new to pathstep-mma
;

\ Check if tos is an allocated pathstep.
: is-pathstep? ( tos -- bool )
    dup pathstep-mma mma-is-item?   \ addr bool
    if
        struct-get-id
        pathstep-struct-id =        \ bool
    else
        drop
        false                       \ f
    then
;

' is-pathstep? to is-pathstep?-xt

\ Start accessors.

\ Return the from field from a pathstep instance.
: pathstep-get-from ( pthstp0 -- sta0 )
    \ Check arg.
    assert( tos is-pathstep? )

    pathstep-from-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the from field from a pathstep instance, use only in this file.
: _pathstep-set-from ( regc1 pthstp0 -- )
    \ Check arg.
    assert( tos is-pathstep? )
    assert( nos is-regioncorr? )

    pathstep-from-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return the to field from a pathstep instance.
: pathstep-get-to ( pthstp0 -- regc )
    \ Check arg.
    assert( tos is-pathstep? )

    pathstep-to-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the to field from a pathstep instance, use only in this file.
: _pathstep-set-to ( regc1 pthstp0 -- )
    \ Check arg.
    assert( tos is-pathstep? )
    assert( nos is-regioncorr? )

    pathstep-to-disp +      \ Add offset.
    !struct                 \ Set the field.
;

\ Return the within field from a pathstep instance.
: pathstep-get-within ( pthstp0 -- regc )
    \ Check arg.
    assert( tos is-pathstep? )

    pathstep-within-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the within field from a pathstep instance, use only in this file.
: _pathstep-set-within ( regc1 pthstp0 -- )
    \ Check arg.
    assert( tos is-pathstep? )
    assert( nos is-regioncorr? )

    pathstep-within-disp +  \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a PathStep.
: pathstep-new ( within2 to1 from0 -- pthstp )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr? )
    assert( 3os is-regioncorr? )

    dup                             \ within2 to1 from0 from0
    #3 pick                         \ within2 to1 from0 from0 within2
    regioncorr-superset?            \ within2 to1 from0 bool
    invert abort" within not superset from?"

    over                            \ within2 to1 from0 to1
    #3 pick                         \ within2 to1 from0 to1 within2
    regioncorr-superset?            \ within2 to1 from0 bool
    invert abort" within not superset to?"

    \ Allocate space.
    pathstep-struct-id pathstep-mma \ within2 to1 from0 id mma
    struct-allocate                 \ within2 to1 from0 pthstp

    \ Store fields.
    tuck _pathstep-set-from         \ within2 to1 pthstp
    tuck _pathstep-set-to           \ within2 pthstp
    tuck _pathstep-set-within       \ pthstp
;

: .pathstep ( pthstp0 -- )
    \ Check arg.
    assert( tos is-pathstep? )

    ." ( pthstp "
          dup   pathstep-get-from   .regioncorr \ lst
    space dup   pathstep-get-to     .regioncorr \ lst
    space       pathstep-get-within .regioncorr \ lst
    ." )"
;

: pathstep-deallocate ( pthstp0 -- )
    \ Check arg.
    assert( tos is-pathstep? )

    dup struct-get-use-count            \ pthstp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup pathstep-get-from           \ pthstp0 reg-lst
        regioncorr-deallocate

        dup pathstep-get-to             \ pthstp0 reg-lst
        regioncorr-deallocate

        dup pathstep-get-within         \ pthstp0 reg-lst
        regioncorr-deallocate

        \ Deallocate instance.
        pathstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

