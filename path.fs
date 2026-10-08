
\ Implement a struct and functions for a set of regioncorrs used to find a path
\ around regions to avoid.

#53197 constant path-struct-id
    #3 constant path-struct-number-cells

\ Struct fields
0                               constant path-header-disp           \ 16-bits [0] struct id [1] use count
path-header-disp        cell+   constant path-avoid-list-disp       \ A regioncorr-list that should be avoided.
path-avoid-list-disp    cell+   constant path-traverse-list-disp    \ A regioncorrr- list of regions that can be traversed through.
                                                                    \ That is the maximum-X regioncorr minus the avoid list.
0 value path-mma \ Storage for state mma instance.

\ Init path mma, return an address of allocated memory.
: path-mma-init ( num-items -- ) \ sets path-mma.
    dup 1 <
    abort" path-mma-init: Invalid number of items."

    cr ." Initializing Path store."
    path-struct-number-cells swap mma-new to path-mma
;

\ Check if tos is an allocated path.
: is-path? ( tos -- bool )
    dup path-mma mma-is-item?  \ tos bool
    if
        struct-get-id
        path-struct-id =       \ bool
    else
        drop
        false                       \ f
    then
;

\ Start accessors.

\ Return the avoid list field from a path instance.
: path-get-avoid-list ( pth0 -- regc-lst )
    \ Check arg.
    assert( tos is-path? )

    path-avoid-list-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the avoid list field from a path instance, use only in this file.
: _path-set-avoid-list ( regc-lst1 pth0 -- )
    \ Check args.
    assert( tos is-path? )
    assert( nos is-regioncorr-list? )

    \ Store list
    path-avoid-list-disp +  \ Add offset.
    !struct                 \ Set the field.
;

\ Return the traverse list field from a path instance.
: path-get-traverse-list ( pth0 -- regc-lst )
    \ Check arg.
    assert( tos is-path? )

    path-traverse-list-disp +   \ Add offset.
    @                           \ Fetch the field.
;

\ Set the traverse list field from a path instance, use only in this file.
: _path-set-traverse-list ( regc-lst1 pth0 -- )
    \ Check args.
    assert( tos is-path? )
    assert( nos is-regioncorr-list? )

    \ Store list
    path-traverse-list-disp +   \ Add offset.
    !struct                     \ Set the field.
;

\ End accessors.

\ Create a path from a state-list.
\ Given a list-to-avoid and the session max-regions.
: path-new ( regc-avd1 regc0 -- pth )
    \ Check arg.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr-list? )

    \ Allocate instance.
    path-struct-id path-mma
    struct-allocate                 \ regc-avd1 regc0 pthx

    \ Calc traverse-list.

    \ Make max-x regioncorr list.
    swap list-new                   \ regc-avd1 pthx regc0 regc-max-lst'
    tuck list-push-struct           \ regc-avd1 pthx regc-max-lst'

    \ Subtract avoid-list from max-list.
    #2 pick                         \ regc-lst0 pthx regc-max-lst' regc-lst0
    over                            \ regc-lst0 pthx regc-max-lst' regc-lst0 regc-max-lst'
    regioncorr-list-subtract        \ regc-lst0 pthx regc-max-lst' regc-trv-lst'
    swap regioncorr-list-deallocate \ regc-lst0 pthx regc-trv-lst'
    swap                            \ regc-lst0 regc-trv-lst' pthx

    \ Store the lists.
    tuck _path-set-traverse-list   \ regc-lst0 pthx
    tuck _path-set-avoid-list      \ pthx
;

\ Print a state-list corresponding to the session domain list.
: .path ( pth0 -- )
    \ Check arg.
    assert( tos is-path? )

    ." ( pth "
    cr #2 spaces ." avoid:    "
    dup path-get-avoid-list
    .regioncorr-list
    cr #2 spaces ." traverse: "
    path-get-traverse-list
    .regioncorr-list
    ." )"
;

\ Deallocate the given path, if its use count is 1 or 0.
: path-deallocate ( pth0 -- )
    \ Check arg.
    assert( tos is-path? )

    dup struct-get-use-count            \ pth0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup path-get-avoid-list         \ pth0 regc-lst
        regioncorr-list-deallocate

        dup path-get-traverse-list      \ pth0 regc-lst
        regioncorr-list-deallocate

        \ Deallocate instance.
        path-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

