
\ Implement a struct, and functions, for a cornertemplate, which may
\ be a superset of an actual corner.

#17317 constant cornertemplate-struct-id
    #3 constant cornertemplate-struct-number-cells

\ Struct fields
0                                           constant cornertemplate-header-disp         \ 16-bits [0] struct id [1] use count
cornertemplate-header-disp          cell+   constant cornertemplate-intersection-disp   \ A region that is a common intersection of multiple regions.
cornertemplate-intersection-disp    cell+   constant cornertemplate-regions-disp        \ Defining parts of a defining region.
                                                                                        \ A region-list.

0 value cornertemplate-mma \ Storage for cornertemplate mma instance.

\ Init cornertemplate mma, return an address of allocated memory.
: cornertemplate-mma-init ( num-items -- ) \ sets cornertemplate-mma.
    dup 1 <
    abort" cornertemplate-mma-init: Invalid number of items."

    cr ." Initializing CornerTemplate store."
    cornertemplate-struct-number-cells swap mma-new to cornertemplate-mma
;

\ Check if tos is an allocated cornertemplate.
: is-cornertemplate? ( tos -- bool )
    dup cornertemplate-mma mma-is-item? \ tos bool
    if
        struct-get-id
        cornertemplate-struct-id =      \ bool
    else
        drop
        false                           \ f
    then
;

\ Return true if the passed list could be a ct list.
: is-cornertemplate-list? ( tos -- bool )
    dup is-list?            \ tos bool
    ifnot
        drop
        false
        \ cr ." is-cornertemplate-list?: exit 1" cr
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        \ cr ." is-cornertemplate-list?: exit 2" cr
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-cornertemplate?             \ bool
    \ cr ." is-cornertemplate-list?: end: " dup .bool cr
;

\ Start accessors.

\ Return the intersection field from a cornertemplate instance.
: cornertemplate-get-intersection ( ct0 -- reg )
    \ Check arg.
    assert( tos is-cornertemplate? )

    cornertemplate-intersection-disp +  \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the intersection field from a cornertemplate instance, use only in this file.
: _cornertemplate-set-intersection ( reg1 ct0 -- )
    \ Check args.
    assert( tos is-cornertemplate? )
    assert( nos is-region? )

    \ Store list
    cornertemplate-intersection-disp +  \ Add offset.
    !struct                             \ Set the field.
;

\ Return the regions field from a cornertemplate instance.
: cornertemplate-get-regions ( ct0 -- reg-lst )
    \ Check arg.
    assert( tos is-cornertemplate? )

    cornertemplate-regions-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the regions field from a cornertemplate instance, use only in this file.
: _cornertemplate-set-regions ( reg-lst1 ct0 -- )
    \ Check args.
    assert( tos is-cornertemplate? )
    assert( nos is-region-list? )

    \ Store list
    cornertemplate-regions-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ End accessors.

\ Create a cornertemplate from a region and region-list.
: cornertemplate-new ( reg-lst1 reg0 -- ct )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-region-list? )

    \ Allocate instance.
    cornertemplate-struct-id cornertemplate-mma
    struct-allocate                         \ reg-lst1 reg0 ct

    \ Store fields.
    tuck _cornertemplate-set-intersection   \ reg-lst1 ct
    tuck _cornertemplate-set-regions        \ ct
;

\ Print a cornertemplate instance.
: .cornertemplate ( ct0 -- )
    \ Check arg.
    assert( tos is-cornertemplate? )

    ." (ct "
    dup cornertemplate-get-intersection .region
    space
    cornertemplate-get-regions
    space .region-list
    ." )"
;

\ Deallocate the given cornertemplate, if its use count is 1 or 0.
: cornertemplate-deallocate ( ct0 -- )
    \ Check arg.
    assert( tos is-cornertemplate? )

    dup struct-get-use-count                \ ct0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup cornertemplate-get-intersection \ ct0 ct-lst
        region-deallocate

        dup cornertemplate-get-regions      \ ct0 ct-lst
        region-list-deallocate

        \ Deallocate instance.
        cornertemplate-mma mma-deallocate
    else
        struct-dec-use-count
    then
;
