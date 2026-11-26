
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
        dup defregcts-get-region        \ defregcts0 reg
        region-deallocate

        dup defregcts-get-cts           \ defregcts0 ct-lst
        cornertemplate-list-deallocate

        \ Deallocate instance.
        defregcts-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a state could be an anchor for a defregcts.
: defreg-state-is-anchor? ( sta1 dfrgcts0 -- bool )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-state? )

    defregcts-get-cts                       \ sta1 cts
    cornertemplate-list-state-is-anchor?    \ bool
;

\ Return true if a state list contains a state that could be an anchor.
: defreg-state-list-contains-anchor? ( sta-lst1 dfrgcts0 -- bool )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-state-list? )

    swap                        \ dfrgcts0 sta-lst1
    foreach                     \ dfrgcts0 sta-lnk sta-x
        #2 pick                 \ dfrgcts0 sta-lnk sta-x dfrgcts0
        defreg-state-is-anchor? \ dfrgcts0 sta-lnk bool
        if
            2drop
            true
            exit
        then
    next-item
                                \ dfrgcts0
    drop
    false
;

\ Return corner from a state that matches any cornertemplates.
: defreg-corner-from-state ( sta1 dfregcts -- crn t | f )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-state? )

    2dup defregcts-get-region               \ sta1 cts sta1 reg
    region-superset-of-state?               \ sta1 cts bool
    if
        defregcts-get-region               \ sta1 reg
        corner-new                         \ crn t | f
    else
        2drop
        false
    then
;

\ Return corners from any states that match cornertemplates.
: defreg-corners-from-state-list ( sta-lst1 dfrgcts -- crn-lst t | f )
    \ Check args.
    assert( tos is-defregcts? )
    assert( nos is-state-list? )

    \ Init return list.
    list-new -rot                   \ ret-lst sta-lst1 dfrgcts0

    swap                            \ ret-lst dfrgcts0 sta-lst1
    foreach                         \ ret-lst dfrgcts0 sta-lnk sta-x
        #2 pick                     \ ret-lst dfrgcts0 sta-lnk sta-x dfrgcts0
        defreg-corner-from-state    \ ret-lst dfrgcts0 sta-lnk, sta t | f
        if
            #3 pick                 \ ret-lst dfrgcts0 sta-lnk sta ret-lst
            list-push-struct        \ ret-lst dfrgcts0 sta-lnk
        then
    next-item
                                    \ ret-lst dfrgcts0
    drop

    check-list-return
;

\ Check TOS for defregcts-list.
: is-defregcts-list? ( tos -- bool )
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
    is-defregcts?           \ bool
;

\ Return corner template clusters that cover all defining regions.
: defregcts-list-cornertemplate-clusters ( dfrgcts-lst0 -- cts-lol t | f)
    \ Check arg.
    assert( tos is-defregcts-list? )

    \ Make list of all corner templates.
    list-new                                        \ dfrgcts-lst0 crntmpl-lst1'
    swap                                            \ crntmpl-lst1' dfrgcts-lst0

    foreach                                         \ crntmpl-lst1' dfrgcts-lnk dfrgcts-x
        defregcts-get-cts                           \ crntmpl-lst1' dfrgcts-lnk cts-lst
        #2 pick                                     \ crntmpl-lst1' dfrgcts-lnk cts-lst crntmpl-lst1'
        list-append-struct                          \ crntmpl-lst1' dfrgcts-lnk
    next-item
                                                    \ crntmpl-lst1'
    \ cr s" All cts: " #2 pick .struct-list-prefix cr

    \ Check for empty list.
    dup list-is-empty?
    if
        list-deallocate
        drop
        false
        exit
    then

    \ Init state cluster list.
    list-new                                \ crntmpl-lst1' crntmpl-clstrs'

    \ Pepare corner template list to parse.
    over list-copy                          \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2'

    \ Process corner
    begin
        dup list-is-not-empty?
    while
        \ Init cornertemplate cluster list.
        dup list-pop-struct                 \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2', crntmtl-f t | f
        invert abort" pop failed?"

        list-new tuck list-push-struct      \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr'

        \ Find cornertemplates connected to cornertemplates, by shared regions.

        \ The list grows as new cornertemplates are added to the end, increasing the number of loop cycles.
        dup                                 \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr'
        foreach                             \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk crntmpl-x
            #5 pick                         \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk crntmpl-x crntmpl-lst1'
            cornertemplate-list-connections \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk, crntmpl-lst3' t | f
            if
                \ Add corner template to corner template cluster list, no dups.
                dup                                     \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk crntmpl-lst3' crntmpl-lst3'
                #3 pick                                 \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk crntmpl-lst3' crntmpl-lst3' crntmpl-clstr'
                cornertemplate-list-append-end-noeq     \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk crntmpl-lst3'
                cornertemplate-list-deallocate          \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-clstr-lnk
            then
        next-item
                                            \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr'

        \ Get states not collected, for the next loop cycle.
        [ ' = ] literal                     \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' xt
        #2 pick #2 pick                     \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' xt crntmpl-lst2' crntmpl-clstr'
        list-difference-struct              \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-clstr' crntmpl-lst-dif'

        \ Store the state cluster.
        swap                                \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-lst-dif' crntmpl-clstr'
        #3 pick                             \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-lst-dif' crntmpl-clstr' crntmpl-clstrs'
        list-push-struct                    \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst2' crntmpl-lst-dif'

        \ Make new state list to parse out more clusters.
        swap cornertemplate-list-deallocate \ crntmpl-lst1' crntmpl-clstrs' crntmpl-lst-dif'
    repeat

    list-deallocate                         \ crntmpl-lst1' crntmpl-clstrs'

    \ cr ." Corner template clusters: " dup .struct-list cr

    swap cornertemplate-list-deallocate     \ crntmpl-clstrs1'

    check-list-return
;

\ Return true if a state list contains anchors for each defregcts in a list.
: defregcts-list-state-list-contains-all-anchors? ( sta-lst1 dfrgcts-lst0 -- bool )
    \ Check args.
    assert( tos is-defregcts-list? )
    assert( nos is-state-list? )

    foreach                                 \ sta-lst1 dfrgcts-lnk dfrgcts-x
        #2 pick swap                        \ sta-lst1 dfrgcts-lnk sta-lst1 dfrgcts-x
        defreg-state-list-contains-anchor?  \ sta-lst1 dfrgcts-lnk bool
        ifnot
            2drop
            false
            exit
        then
    next-item
                                            \ sta-lst1
    drop
    true
;

\ Return a list of corners, given a state-list, of anchors, and a defrects list.
: defregcts-list-corners-from-state-list ( sta-lst1 dfrgcts-lst0 -- crn-lst t | f )
    \ Check args.
    assert( tos is-defregcts-list? )
    assert( nos is-state-list? )

    \ Init return list.
    list-new -rot                           \ ret-lst sta-lst1 dfrgcts-lst0

    foreach                                 \ ret-lst sta-lst1 dfrgcts-lnk dfrgcts-x
        #2 pick swap                        \ ret-lst sta-lst1 dfrgcts-lnk sta-lst1 dfrgcts-x
        defreg-corners-from-state-list      \ ret-lst sta-lst1 dfrgcts-lnk, crn-lst' t | f
        if
            \ Store the corner.
            dup                             \ ret-lst sta-lst1 dfrgcts-lnk crn-lst' crn-lst'
            #4 pick                         \ ret-lst sta-lst1 dfrgcts-lnk crn-lst' crn-lst' ret-lst
            list-append-struct              \ ret-lst sta-lst1 dfrgcts-lnk cn-lst'

            corner-list-deallocate          \ ret-lst sta-lst1 dfrgcts-lnk
        then
    next-item
                                            \ ret-lst sta-lst1
    drop
    check-list-return
;
