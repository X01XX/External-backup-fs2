
\ Implement a struct, and functions, for a defregcongparts, that is a defining-region,
\ with a list of regions that are congreunt intersections between defining parts of the
\ defining region and adjacent defining parts of other defininr regions.

#53173 constant defregcongparts-struct-id
    #3 constant defregcongparts-struct-number-cells

\ Struct fields
0                                       constant defregcongparts-header-disp  \ 16-bits [0] struct id [1] use count
defregcongparts-header-disp    cell+   constant defregcongparts-region-disp  \ A defining region.
defregcongparts-region-disp    cell+   constant defregcongparts-parts-disp   \ A list of defining parts within the defining region.

0 value defregcongparts-mma \ Storage for defregcongparts mma instance.

\ Init defregcongparts mma, return an address of allocated memory.
: defregcongparts-mma-init ( num-items -- ) \ sets defregcongparts-mma.
    dup 1 <
    abort" defregcongparts-mma-init: Invalid number of items."

    cr ." Initializing DefRegCongParts store."
    defregcongparts-struct-number-cells swap mma-new to defregcongparts-mma
;

\ Check if tos is an allocated defregcongparts.
: is-defregcongparts? ( tos -- bool )
    dup defregcongparts-mma mma-is-item?   \ tos bool
    if
        struct-get-id
        defregcongparts-struct-id =        \ bool
    else
        drop
        false                               \ f
    then
;

\ Start accessors.

\ Return the region field from a defregcongparts instance.
: defregcongparts-get-region ( defregcongparts0 -- reg )
    \ Check arg.
    assert( tos is-defregcongparts? )

    defregcongparts-region-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the region field from a defregcongparts instance, use only in this file.
: _defregcongparts-set-region ( reg1 defregcongparts0 -- )
    \ Check args.
    assert( tos is-defregcongparts? )
    assert( nos is-region? )

    \ Store list
    defregcongparts-region-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the defining parts field from a defregcongparts instance.
: defregcongparts-get-parts ( defregcongparts0 -- reg-lst )
    \ Check arg.
    assert( tos is-defregcongparts? )

    defregcongparts-parts-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ Set the defining parts field from a defregcongparts instance, use only in this file.
: _defregcongparts-set-parts ( reg-lst1 defregcongparts0 -- )
    \ Check args.
    assert( tos is-defregcongparts? )
    assert( nos is-region-list? )

    \ Store list
    defregcongparts-parts-disp +    \ Add offset.
    !struct                         \ Set the field.
;

\ End accessors.

\ Create a defregcongparts from a region and  defining parts list.
: defregcongparts-new ( reg-lst1 reg0 -- dfprcts )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-region-list? )

    \ Allocate instance.
    defregcongparts-struct-id defregcongparts-mma
    struct-allocate                 \ reg-lst1 reg0 defregcongparts

    \ Store fields.
    tuck _defregcongparts-set-region \ reg-lst1 defregcongparts
    tuck _defregcongparts-set-parts  \ defregcongparts
;

\ Print a defregcongparts instance.
: .defregcongparts ( dfrgcts0 -- )
    \ Check arg.
    assert( tos is-defregcongparts? )

    ." ( defregcongparts "
    dup defregcongparts-get-region .region
    space
    defregcongparts-get-parts
    .region-list
    ." )"
;

\ Deallocate the given defregcongparts, if its use count is 1 or 0.
: defregcongparts-deallocate ( defregcongparts0 -- )
    \ Check arg.
    assert( tos is-defregcongparts? )

    dup struct-get-use-count            \ defregcongparts0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup defregcongparts-get-region        \ defregcongparts0 reg
        region-deallocate

        dup defregcongparts-get-parts         \ defregcongparts0 reg-lst
        region-list-deallocate

        \ Deallocate instance.
        defregcongparts-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check TOS for defregcongparts-list.
: is-defregcongparts-list? ( tos -- bool )
    dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-defregcongparts?      \ bool
;

\ Return a defregcts list from a defregcongparts list.
: defregcongparts-list-to-defregcts-list ( dfrgcprts-lst0 -- dfrgcts-lst t | f )
    \ Check args.
    assert( tos is-defregcongparts-list? )

    \ Init defregcts list.
    list-new                                        \ dfrgcprts-lst0 dfrgcts-lst'
    over                                            \ dfrgcprts-lst0 dfrgcts-lst' dfrgcprts-lst0
    foreach                                         \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        \ cr ." def-cngrsx: " dup .struct-list
        \ dup list-get-first-item space .region
        \ dup list-get-first-item region-number-edges space ." edgs " dec.

        dup defregcongparts-get-region region-number-edges
        over defregcongparts-get-parts list-get-length  \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs len
        over <                                      \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs bool
        if
            drop                                    \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        else
            over defregcongparts-get-parts          \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx nedgs cngrsx
            any-x-of-n                              \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol'
            \ dup .struct-list
            \ cr

            \ Check each combination of congruent intersections for a common intersection.

            \ Init corner template list.
            list-new                                \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst'
            over
            foreach                                 \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk crgrsx
                region-list-common-intersection     \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk, reg t | f
                if
                    \ Make corner template.
                    over link-get-data swap         \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk crgrsx reg
                    cornertemplate-new              \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk, ct' t | f
                    invert abort" cornertemplate-new failed?"

                    \ Store corner template.
                    #2 pick                         \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk ct' ct-lst'
                    list-push-struct                \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' crgrs-lnk
                then
            next-item
                                                    \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst'
            \ cr #12 spaces dup .struct-list cr

            \ Check corner template list for the defining region.
            dup list-is-empty?
            if
                list-deallocate
            else
                \ Make DefRegCts
                #2 pick                             \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' def-cngrsx
                defregcongparts-get-region          \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' ct-lst' def-reg
                defregcts-new                       \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' dfregctsx
                \ cr #12 spaces dup .defregcts cr
                #4 pick                             \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol' dfregctsx dfrgcts-lst'
                list-push-struct                    \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx crgrsx-lol'
            then
        then
        struct-list-deallocate                      \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
                                                    \ dfrgcprts-lst0 dfrgcts-lst' def-cnrgrs-lnk def-cngrsx
        drop
    next-item
                                                    \ dfrgcprts-lst0 dfrgcts-lst'
    cr s" def reg cts: " #2 pick .struct-list-prefix

    nip
    check-list-return
;
