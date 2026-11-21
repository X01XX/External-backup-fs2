
\ Implement a struct, and functions, for a defregcts, that is a defining-region,
\ with a list of corner templates.

#41737 constant defregcts-struct-id
    #3 constant defregcts-struct-number-cells

\ Struct fields
0                           constant defregcts-header-disp  \ 16-bits [0] struct id [1] use count
defregcts-header-disp cell+ constant defregcts-region-disp  \ A defining region.
defregcts-region-disp cell+ constant defregcts-cts-disp     \ A list of corner templates, within the defining region.

0 value defregcts-mma \ Storage for defregcts mma instance.

\ Init defregcts mma, return an address of allocated memory.
: defregcts-mma-init ( num-items -- ) \ sets defregcts-mma.
    dup 1 <
    abort" defregcts-mma-init: Invalid number of items."

    cr ." Initializing DefRegCts store."
    defregcts-struct-number-cells swap mma-new to defregcts-mma
;

\ Check if tos is an allocated defregcts.
: is-defregcts? ( tos -- bool )
    dup defregcts-mma mma-is-item?  \ tos bool
    if
        struct-get-id
        defregcts-struct-id =       \ bool
    else
        drop
        false                       \ f
    then
;

\ Start accessors.

\ Return the region field from a defregcts instance.
: defregcts-get-region ( defregcts0 -- reg )
    \ Check arg.
    assert( tos is-defregcts? )

    defregcts-region-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the region field from a defregcts instance, use only in this file.
: _defregcts-set-region ( reg1 defregcts0 -- )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-region? )

    \ Store list
    defregcts-region-disp + \ Add offset.
    !struct                 \ Set the field.
;

\ Return the corner templates field from a defregcts instance.
: defregcts-get-cts ( defregcts0 -- cts-lst )
    \ Check arg.
    assert( tos is-defregcts? )

    defregcts-cts-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the corner templates field from a defregcts instance, use only in this file.
: _defregcts-set-cts ( cts-lst1 defregcts0 -- )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-cornertemplate-list? )

    \ Store list
    defregcts-cts-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a defregcts from a region and  corner templates list.
: defregcts-new ( cts-lst1 reg0 -- dfprcts )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-cornertemplate-list? )

    \ Allocate instance.
    defregcts-struct-id defregcts-mma
    struct-allocate             \ cts-lst1 reg0 defregcts

    \ Store fields.
    tuck _defregcts-set-region  \ cts-lst1 defregcts
    tuck _defregcts-set-cts     \ defregcts
;

\ Print a defregcts instance.
: .defregcts ( dfrgcts0 -- )
    \ Check arg.
    assert( tos is-defregcts? )

    ." ( defregcts "
    dup defregcts-get-region .region
    space
    defregcts-get-cts
    .struct-list
    ." )"
;

\ Deallocate the given defregcts, if its use count is 1 or 0.
: defregcts-deallocate ( defregcts0 -- )
    \ Check arg.
    assert( tos is-defregcts? )

    dup struct-get-use-count            \ defregcts0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup defregcts-get-region        \ defregcts0 defregcts-lst
        region-deallocate

        dup defregcts-get-cts           \ defregcts0 defregcts-lst
        struct-list-deallocate

        \ Deallocate instance.
        defregcts-mma mma-deallocate
    else
        struct-dec-use-count
    then
;
