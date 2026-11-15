\ A struct and functions incompatible pair cluster data.
\

#53171 constant pairdata-struct-id
    #3 constant pairdata-struct-number-cells

\ Struct fields
0                               constant pairdata-header-disp   \ 16-bits [0] struct id [1] use count.
pairdata-header-disp    cell+   constant pairdata-pairs-disp    \ A list of incompatible states expressed at states in a region.
pairdata-pairs-disp     cell+   constant pairdata-regions-disp  \ A list of possible regions implied by the incompatible pairs.

0 value pairdata-mma \ Storage for pairdata mma instance.

\ Init pairdata mma, return the addr of allocated memory.
: pairdata-mma-init ( num-items -- ) \ sets pairdata-mma.
    dup 1 <
    abort" pairdata-mma-init: Invalid number of items."

    cr ." Initializing PairData store."
    pairdata-struct-number-cells swap mma-new to pairdata-mma
;

\ Check if tos is an allocated pairdata.
: is-pairdata? ( tos -- flag )
    dup pairdata-mma mma-is-item? \ addr bool
    if
        struct-get-id
        pairdata-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the pairs list of a pairdata instance.
: pairdata-get-pairs ( prdt0 -- pr-lst )
    \ Check arg.
    assert( tos is-pairdata? )

    pairdata-pairs-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the pairs field of a pairdata instance.
: pairdata-set-pairs ( pr-lst1 prdt0 -- )
    \ Check arg.
    assert( tos is-pairdata? )
    assert( nos is-region-list? )

    \ Get second state.
    pairdata-pairs-disp +   \ Add offset.
    !                       \ Set the field.
;

\ Return the regions list of a pairdata instance.
: pairdata-get-regions ( prdt0 -- reg-lst )
    \ Check arg.
    assert( tos is-pairdata? )

    pairdata-regions-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the regions field of a pairdata instance.
: pairdata-set-regions ( reg-lst1 prdt0 -- )
    \ Check arg.
    assert( tos is-pairdata? )
    assert( nos is-region-list? )

    \ Get second state.
    pairdata-regions-disp + \ Add offset.
    !                       \ Set the field.
;

\ End accessors.

\ Create a pairdata instance from a non-empty pair list.
: pairdata-new ( pr-lst0 -- prdt )
    \ Check arg.
    assert( tos is-region-list? )
    assert( tos list-is-not-empty? )

    \ Allocate instance.
    pairdata-struct-id pairdata-mma \ pr-lst0 id mma
    struct-allocate                 \ pr-lst0 prdt

    \ Calc possible regions.
    over                            \ pr-lst prdt pr-lst
    region-list-calc-possible-regions-from-incompatible-pairs   \ pr-lst prdt pos-regs

    \ Set fields.
    over pairdata-set-regions       \ pr-lst prdt
    tuck pairdata-set-pairs         \ prdt
;

\ Deallocate a pairdata.
: pairdata-deallocate ( prdt0 -- )
    \ Check arg.
    assert( tos is-pairdata? )

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" pairdata-deallocate: Invalid use count"

    #2 <
    if
        dup pairdata-get-pairs region-list-deallocate
        dup pairdata-get-regions region-list-deallocate

        \ Deallocate instance.
        pairdata-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .pairdata ( prdt0 -- )
    \ Check arg.
    assert( tos is-pairdata? )

    ." ( prdt "
    dup pairdata-get-pairs .struct-list
    space pairdata-get-regions .struct-list
    ." )"
;

