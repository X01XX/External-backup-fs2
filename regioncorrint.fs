\ Implement a struct and functions for a regioncorr intersection.

#61981 constant regioncorrint-struct-id
    #3 constant regioncorrint-struct-number-cells

\ Struct fields
0                                       constant regioncorrint-header-disp          \ 16-bits [0] struct id [1] use count
regioncorrint-header-disp       cell+   constant regioncorrint-intersection-disp    \ A regioncorr.
regioncorrint-intersection-disp cell+   constant regioncorrint-list-disp            \ A list of two, or more, regioncorrs that all intersect.

0 value regioncorrint-mma  \ Storage for region mma instance.

\ Init regioncorrint mma, return an address of allocated memory.
: regioncorrint-mma-init ( num-items -- ) \ sets regioncorrint-mma.
    dup 1 <
    abort" regioncorrint-mma-init: Invalid number of items."

    cr ." Initializing RegionCorrInt store."
    regioncorrint-struct-number-cells swap mma-new to regioncorrint-mma
;

\ Check if tos is an allocated regioncorrint.
: is-regioncorrint? ( tos -- bool )
    dup regioncorrint-mma mma-is-item? \ addr bool
    if
        struct-get-id
        regioncorrint-struct-id =   \ bool
    else
        drop
        false                       \ f
    then
;

' is-regioncorrint? to is-regioncorrint?-xt

\ Start accessors.

\ Return the intersection field from a regioncorrint instance.
: regioncorrint-get-intersection ( regci0 -- regci-lst )
    \ Check arg.
    assert( tos is-regioncorrint? )

    regioncorrint-intersection-disp +   \ Add offset.
    @                                   \ Fetch the field.
;

\ ' regioncorrint-get-intersection to regioncorrint-get-intersection-xt

\ Set the intersection field from a regioncorrint instance, use only in this file.
: _regioncorrint-set-intersection ( regci regc0 -- )
    \ Check args.
    assert( tos is-regioncorrint? )
    assert( nos is-regioncorr? )

    \ Store list
    regioncorrint-intersection-disp +   \ Add offset.
    !struct                             \ Set the field.
;

\ Return the list field from a regioncorrint instance.
: regioncorrint-get-list ( regci0 -- regci-lst )
    \ Check arg.
    assert( tos is-regioncorrint? )

    regioncorrint-list-disp + \ Add offset.
    @                         \ Fetch the field.
;

' regioncorrint-get-list to regioncorrint-get-list-xt

\ Set the list field from a regioncorrint instance, use only in this file.
: _regioncorrint-set-list ( regci-lst1 regc0 -- )
    \ Check args.
    assert( tos is-regioncorrint? )
    assert( nos is-regioncorr-list? )
    assert( nos list-get-length 1 > )

    \ Store list
    regioncorrint-list-disp + \ Add offset.
    !struct                   \ Set the field.
;

\ End accessors.

\ Create a regioncorr from a region-list.
: regioncorrint-new ( regc-lst0 regc -- regc )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr-list? )
    assert( 2dup swap regioncorr-list-all-superset? )

    \ Allocate space.
    regioncorrint-struct-id regioncorrint-mma
    struct-allocate                         \ regc-lst0 regc regci

    \ Store intersection.
    tuck _regioncorrint-set-intersection    \ regc-lst0 regci

    \ Store list.
    tuck _regioncorrint-set-list           \ regci
;

\ Print a region-list corresponding to the session domain list.
: .regioncorrint ( regci0 -- )
    \ Check arg.
    assert( tos is-regioncorrint? )

    ." ( regci "
    dup regioncorrint-get-intersection .regioncorr
    regioncorrint-get-list              \ lst
    .regioncorr-list
    ." )"
;

' .regioncorrint to .regioncorrint-xt

\ Deallocate the given regci, if its use count is 1 or 0.
: regioncorrint-deallocate ( regci0 -- )
    \ Check arg.
    assert( tos is-regioncorrint? )

    dup struct-get-use-count                \ regc0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate intersection.
        dup regioncorrint-get-intersection  \ regc0 reg-lst
        regioncorr-deallocate

        \ Deallocate fields.
        dup regioncorrint-get-list          \ regc0 reg-lst
        regioncorr-list-deallocate

        \ Deallocate instance.
        regioncorrint-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

