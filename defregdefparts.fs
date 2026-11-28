
\ Implement a struct, and functions, for a defregdefparts, that is a defining-region,
\ with a list of regions within it that are not overlapped byy any other possible region.

#31973 constant defregdefparts-struct-id
    #3 constant defregdefparts-struct-number-cells

\ Struct fields
0                                   constant defregdefparts-header-disp  \ 16-bits [0] struct id [1] use count
defregdefparts-header-disp  cell+   constant defregdefparts-region-disp  \ A defining region.
defregdefparts-region-disp  cell+   constant defregdefparts-parts-disp   \ A list of defining parts within the defining region.

0 value defregdefparts-mma \ Storage for defregdefparts mma instance.

\ Init defregdefparts mma, return an address of allocated memory.
: defregdefparts-mma-init ( num-items -- ) \ sets defregdefparts-mma.
    dup 1 <
    abort" defregdefparts-mma-init: Invalid number of items."

    cr ." Initializing DefRegDefParts store."
    defregdefparts-struct-number-cells swap mma-new to defregdefparts-mma
;

\ Check if tos is an allocated defregdefparts.
: is-defregdefparts? ( tos -- bool )
    dup defregdefparts-mma mma-is-item? \ tos bool
    if
        struct-get-id
        defregdefparts-struct-id =      \ bool
    else
        drop
        false                           \ f
    then
;

\ Start accessors.

\ Return the region field from a defregdefparts instance.
: defregdefparts-get-region ( defregdefparts0 -- reg )
    \ Check arg.
    assert( tos is-defregdefparts? )

    defregdefparts-region-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ Set the region field from a defregdefparts instance, use only in this file.
: _defregdefparts-set-region ( reg1 defregdefparts0 -- )
    \ Check args.
    assert( tos is-defregdefparts? )
    assert( nos is-region? )

    \ Store list
    defregdefparts-region-disp +    \ Add offset.
    !struct                         \ Set the field.
;

\ Return the defining parts field from a defregdefparts instance.
: defregdefparts-get-parts ( defregdefparts0 -- reg-lst )
    \ Check arg.
    assert( tos is-defregdefparts? )

    defregdefparts-parts-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the defining parts field from a defregdefparts instance, use only in this file.
: _defregdefparts-set-parts ( reg-lst1 defregdefparts0 -- )
    \ Check args.
    assert( tos is-defregdefparts? )
    assert( nos is-region-list? )

    \ Store list
    defregdefparts-parts-disp + \ Add offset.
    !struct                     \ Set the field.
;

\ End accessors.

\ Create a defregdefparts from a region and  defining parts list.
: defregdefparts-new ( reg-lst1 reg0 -- dfprcts )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-region-list? )

    \ Allocate instance.
    defregdefparts-struct-id defregdefparts-mma
    struct-allocate                 \ reg-lst1 reg0 defregdefparts

    \ Store fields.
    tuck _defregdefparts-set-region \ reg-lst1 defregdefparts
    tuck _defregdefparts-set-parts  \ defregdefparts
;

\ Print a defregdefparts instance.
: .defregdefparts ( dfrgcts0 -- )
    \ Check arg.
    assert( tos is-defregdefparts? )

    ." ( defregdefparts "
    dup defregdefparts-get-region .region
    space
    defregdefparts-get-parts
    .region-list
    ." )"
;

\ Deallocate the given defregdefparts, if its use count is 1 or 0.
: defregdefparts-deallocate ( defregdefparts0 -- )
    \ Check arg.
    assert( tos is-defregdefparts? )

    dup struct-get-use-count            \ defregdefparts0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup defregdefparts-get-region        \ defregdefparts0 reg
        region-deallocate

        dup defregdefparts-get-parts         \ defregdefparts0 reg-lst
        region-list-deallocate

        \ Deallocate instance.
        defregdefparts-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check TOS for defregdefparts-list.
: is-defregdefparts-list? ( tos -- bool )
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
    is-defregdefparts?      \ bool
;

\ Return a defregcongparts list from a defregdefparts list.
: defregdefparts-list-to-defreg-cong-parts-list ( dfrgprts-lst0 -- dfrgcgprts-lst t | f )
    \ Check args.
    assert( tos is-defregdefparts-list? )

\ def-regs: (rxx10 rxx11 rxx01 rxx00)
\
\ def reg def prts: (rxx10 (rxx10))
\                  (rxx11 (rxx11))
\                  (rxx01 (rxx01))
\                  (rxx00 (rxx00))

    \ Init congruent regions list.
    list-new                                        \ dfrgprts-lst0 cngrs-regs'

    \ Check each possible pair for adjacency,
    \ if so, check adjacency of defining parts.
    over                                            \ dfrgprts-lst0 cngrs-regs' dfrgprts-lst0
    foreach                                         \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk dfrgprts-x
        \ Compare current region with all following regions.
        defregdefparts-get-region                   \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x
        over link-get-next                          \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt
        begin
            ?dup
        while
            dup link-get-data                       \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt dfrgprts-y
            defregdefparts-get-region               \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt reg-y
            #2 pick                                 \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt reg-y reg-x
            regions-adjacent?                       \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt bool
            if
                #2 pick                             \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt dfrgprts-lnk
                link-get-data                       \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt dfrgprts-y
                defregdefparts-get-parts            \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt prts-x

                over link-get-data                  \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt prts-x dfrgprts-y
                defregdefparts-get-parts            \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt prts-x prts-y
                region-list-congruent-regions       \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt, cngrs-regs2' t | f
                if
                    dup                             \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt cngrs-regs2' cngrs-regs2'
                    #5 pick                         \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt cngrs-regs2' cngrs-regs2' cngrs-regs'
                    region-list-append-nosubs       \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt cngrs-regs2'
                    region-list-deallocate          \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x dfrgprts-lnk-nxt
                then
            then

            link-get-next
        repeat
                                                    \ dfrgprts-lst0 cngrs-regs' dfrgprts-lnk reg-x
        drop
    next-item
                                                    \ dfrgprts-lst0 cngrs-regs'

    cr ." cngrs: " dup .region-list cr

    \ Collect congruent regions per defining regions.

    \ Init list.
    list-new swap                                   \ dfrgprts-lst0 def-cnrgrs' cngr-regs'
    #2 pick                                         \ dfrgprts-lst0 def-cnrgrs' cngr-regs' dfrgprts-lst0
    foreach                                         \ dfrgprts-lst0 def-cnrgrs' cngr-regs' dfrgprts-lnk defrgprts-x
        defregdefparts-get-region                   \ dfrgprts-lst0 def-cnrgrs' cngr-regs' dfrgprts-lnk dreg-x
        #2 pick                                     \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk dreg-x cngr-regs'
        [ ' regions-intersect? ] literal -rot       \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk xt dreg-x cngr-regs'
        list-find-all-struct                        \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk int-regs'

        dup list-is-empty?
        if
            list-deallocate                         \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk
        else
            \ Save region, int regs.
            over link-get-data                      \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk int-regs' defrgprts-X
            defregdefparts-get-region               \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk int-regs' dregx
            defregcongparts-new                     \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk def-cngrs-x'
            #3 pick                                 \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk def-cngrs-x' def-cnrgrs'
            list-push-struct                        \ dfrgprts-lst0 def-cnrgrs' cngr-regs' def-lnk
        then
    next-item
                                                    \ dfrgprts-lst0 def-cnrgrs' cngr-regs'
    \ Clean up.
    region-list-deallocate                          \ dfrgprts-lst0 def-cnrgrs'
    nip                                             \ def-cnrgrs'

    cr s" def reg / congr ints: " #2 pick .struct-list-prefix
    check-list-return
;

