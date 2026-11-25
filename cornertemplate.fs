
\ Implement a struct, and functions, for a corner template, which may
\ be a superset of an actual corner.
\
\ With two partial corner clusters, one could be chosen to
\ fill out a corner template cluster, replacing the two partial corner clusters
\ with one complete corner cluster.

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
: cornertemplate-new ( reg-lst1 reg0 -- ct t | f )
    \ Check arg.
    assert( tos is-region? )
    assert( nos is-region-list? )

    over region-list-common-intersection    \ reg-lst1 reg0, int' t | f
    ifnot
        2drop
        false
        exit
    then

    2dup regions-eq?                        \ reg-lst1 reg0, int' bool
    swap region-deallocate                  \ reg-lst1 reg0 bool
    ifnot
        2drop
        false
        exit
    then

    \ Allocate instance.
    cornertemplate-struct-id cornertemplate-mma
    struct-allocate                         \ reg-lst1 reg0 ct

    \ Store fields.
    tuck _cornertemplate-set-intersection   \ reg-lst1 ct
    tuck _cornertemplate-set-regions        \ ct
    true
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

\ Deallocate a cornertemplate list.
: cornertemplate-list-deallocate ( crntmpl-lst -- )
    dup struct-get-use-count                    \ crntmpl-lst uc
    #2 < if
        [ ' cornertemplate-deallocate ] literal over     \ crntmpl-lst xt crntmpl-lst
        list-apply                              \ Deallocate cornertemplate instances in the list.

        list-deallocate                         \ Deallocate list and links.
    else
        struct-dec-use-count
    then
;

\ Return a list of cornerteplates connected to a given cornertemplate,
\ by having adjacent intersections.
: cornertemplate-list-connections ( crntmpl1 crntmpl-lst0 -- crntmpl-lst t | f )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-cornertemplate? )

    \ Init return list.
    list-new -rot                       \ crntmpl-lst crntmpl1 crntmpl-lst0

    foreach                             \ crntmpl-lst crntmpl1 crntmpl-lnk crntmpl-x
        cornertemplate-get-intersection \ crntmpl-lst crntmpl1 crntmpl-lnk crnint-x
        #2 pick                         \ crntmpl-lst crntmpl1 crntmpl-lnk crnint-x crntmpl1
        cornertemplate-get-intersection \ crntmpl-lst crntmpl1 crntmpl-lnk crnint-x crnint1
        regions-adjacent?               \ crntmpl-lst crntmpl1 crntmpl-lnk bool
        if
            \ Get other cornertemplate.
            dup link-get-data           \ crntmpl-lst crntmpl1 crntmpl-lnk crntmpl-x

            \ Store cornertemplate.
            #3 pick                     \ crntmpl-lst crntmpl1 crntmpl-lnk crntmpl-x crntmpl-lst
            list-push-struct            \ crntmpl-lst crntmpl1 crntmpl-lnk
        then
    next-item
                                        \ crntmpl-lst crntmpl1
    drop
    check-list-return
;

\ Append nos cornertemplate list to the tos cornertemplate list end, no duplicates.
: cornertemplate-list-append-end-noeq (  crntmpl-lst1  crntmpl-lst0 -- )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-cornertemplate-list? )

    swap                                        \ crntmpl-lst0 crntmpl-lst1
    list-get-links                              \ crntmpl-lst0 crntmpl-lnk1
    begin
        ?dup
    while
        [ ' = ] literal                         \ crntmpl-lst0 crntmpl-lnk1 xt
        over link-get-data                      \ crntmpl-lst0 crntmpl-lnk1 xt crntmplx
        #3 pick                                 \ crntmpl-lst0 crntmpl-lnk1 xt crntmplx crntmpl-lst0
        list-member?                            \ crntmpl-lst0 crntmpl-lnk1 bool
        ifnot
            dup link-get-data                   \ crntmpl-lst0 crntmpl-lnk1 crntmplx
            #2 pick                             \ crntmpl-lst0 crntmpl-lnk1 crntmplx crntmpl-lst0
            list-push-end-struct                \ crntmpl-lst0 crntmpl-lnk1 bool
        then

        link-get-next
    repeat
                                                \  crntmpl-lst0
    drop
;

\ Check if a list could be a cornertemplate definintion.
: cornertemplate-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." cornertemplate-list-definition?: start: " dup .struct-list cr

    \ Check list length.
    dup list-get-length #3 =
    ifnot
        drop false
        \ cr ." cornertemplate-list-definition?: exit 1: false" cr
        exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." cornertemplate-list-definition?: exit 2: false" cr
        exit
    then

    s" ct"                              \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." cornertemplate-list-definition?: exit 3: false" cr
        exit
    then

    \ Check intersection region.
    dup list-get-second-item            \ lst second
    is-region?
    ifnot
        drop false
        \ cr ." cornertemplate-list-definition?: exit 4: false" cr
        exit
    then

    \ Check regions.
    dup list-get-third-item             \ lst third
    is-region-list?
    ifnot
        drop false
        \ cr ." cornertemplate-list-definition?: exit 5: false" cr
        exit
    then

    \ Check regions have a common intersection, equal to the given intersection.
    dup list-get-third-item             \ lst third
    region-list-common-intersection     \ lst, int' t | f
    if
        over list-get-second-item       \ lst int' ct-reg
        over regions-eq?                \ lst int' bool
        swap region-deallocate          \ lst bool
        ifnot
            drop false
            \ cr ." cornertemplate-list-definition?: exit 6: false" cr
            exit
        then
    else
        drop false
        \ cr ." cornertemplate-list-definition?: exit 7: false" cr
        exit
    then

    drop
    true
    \ cr ." cornertemplate-list-definition?: end: true" cr
;

\ Return a cornertemplate from a list.
: cornertemplate-from-list ( lst -- crn t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." cornertemplate-from-list: start: " dup .struct-list cr

    dup cornertemplate-list-definition?     \ lst bool
    ifnot
        drop false exit
    then

    \ Allocate new cornertemplate.
    dup list-get-third-item                 \ lst reg-lst
    swap list-get-second-item               \ reg-lst reg
    cornertemplate-new                      \ crn t | f

    \ cr ." cornertemplate-from-list: end: " dup if over .cornertemplate else ." cornertemplate-new failed?" then cr
;
\ Return a cornertemplate from a string.
\ Like (ct r0011  (r0x11 r00x1))
: cornertemplate-from-string ( str-addr str-n -- crn t | f )
    \ cr ." cornertemplate-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." cornertemplate-from-string: exit 1: false" cr
        exit
    then

    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." cornertemplate-from-string: exit 2: false" cr
        exit
    then
                                            \ lst
    dup list-get-first-item                 \ lst itm
    is-cornertemplate?                           \ lst bool
    if
        dup list-pop-struct                 \ lst itm
        invert abort" pop faled?"
        swap list-deallocate                \ itm
        true
    else
        struct-list-deallocate
        false
    then
    \ cr ." cornertemplate-from-string: end: " dup .bool cr
;

\ Return a cornertemplate from a string, or abort.
: cornertemplate-from-string-a ( str-addr str-n -- regc )
    cornertemplate-from-string    \ regc t | f
    false? abort" cornertemplate-from-string failed?"
;

\ Return true if a state is in any cornertemplate intersection region of a corner template list.
: cornertemplate-list-state-in-any-intersection? ( sta1 crntmpl-lst0 -- bool )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-state? )

    foreach                             \ sta1 crntmpl-lnk crntmpl-x
        cornertemplate-get-intersection \ sta1 crntmpl-lnk int-x
        #2 pick swap                    \ sta1 crntmpl-lnk sta1 int-x
        region-superset-of-state?       \ sta1 crntmpl-lnk bool
        if
            2drop
            true
            exit
        then
    next-item
                                        \ sta
    drop
    false
;

: cornertemplate-list-adjacent-intersections ( sta1 crntmpl-lst0 -- sta-lst t | f )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-state? )

    \ Init return list.
    list-new -rot                       \ ret-lst sta1 crntmpl-lst0
    foreach                             \ ret-lst sta1 crntmpl-lnk crntmpl-x
        \ Check if the given state is adjacent to the cornertemplate intersection region.
        cornertemplate-get-intersection \ ret-lst sta1 crntmpl-lnk ireg
        #2 pick swap                    \ ret-lst sta1 crntmpl-lnk sta1 ireg
        region-xor-state                \ ret-lst sta1 crntmpl-lnk msk'
        dup mask-only-one-bit?          \ ret-lst sta1 crntmpl-lnk msk' bool
        if
            \ Calc adjacent state within the region.
            dup                         \ ret-lst sta1 crntmpl-lnk msk' msk'
            #3 pick                     \ ret-lst sta1 crntmpl-lnk msk' msk' sta1
            state-xor-mask              \ ret-lst sta1 crntmpl-lnk msk' sta2

            \ Store the state to return.
            #4 pick                     \ ret-lst sta1 crntmpl-lnk msk' sta2 ret-lst
            list-push-end-struct        \ ret-lst sta1 crntmpl-lnk msk'
        then
        mask-deallocate                 \ ret-lst sta1 crntmpl-lnk
    next-item
                                        \ ret-lst sta1
    drop
;

\ Return a corner anchor list, given a state that is in one cornertemplate intersection,
\ of a corner template list-of-lists.
\ This allows seeking needed samples to assemble a minimun corner/sample list, from a possibly
\ non-optimal aggregation of corners that are partly the result of chance.
: cornertemplate-list-predict-all-states ( sta1 crntmpl-lst0 -- sta-lst t | f )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-state? )

    2dup cornertemplate-list-state-in-any-intersection?
    ifnot
        2drop
        false
        exit
    then

    \ Start state list.
    swap                    \ crntmpl-lst0 sta1
    list-new                \ crntmpl-lst0 sta1 sta-lst
    tuck list-push-struct   \ crntmpl-lst0 sta-lst

    \ Fill list with additional potention anchors.
    dup
    foreach                                         \ crntmpl-lst0 sta-lst sta-lnk stax
        #3 pick                                     \ crntmpl-lst0 sta-lst sta-lnk stax crntmpl-lst0
        cornertemplate-list-adjacent-intersections  \ crntmpl-lst0 sta-lst sta-lnk, sta-lst2'
        dup                                         \ crntmpl-lst0 sta-lst sta-lnk sta-lst2' sta-lst2'
        #3 pick                                     \ crntmpl-lst0 sta-lst sta-lnk sta-lst2' sta-lst2' sta-lst
        state-list-append-end-nodups                \ crntmpl-lst0 sta-lst sta-lnk sta-lst2'
        state-list-deallocate                       \ crntmpl-lst0 sta-lst sta-lnk
    next-item
                            \ crntmpl-lst0 sta-lst
    nip
    true
;

\ Return true if a state could be an anchor for any cornertemplate in a list.
: cornertemplate-list-state-is-anchor? ( sta1 crntmpl-lst0 -- bool )
    \ Check args.
    assert( tos is-cornertemplate-list? )
    assert( nos is-state? )

    foreach                             \ sta1 crntmpl-lnk crntmpl-x
        cornertemplate-get-intersection \ sta1 crntmpl-lnk int-x
        #2 pick swap                    \ sta1 crntmpl-lnk sta1 int-x
        region-superset-of-state?       \ sta1 crntmpl-lnk bool
        if
            2drop
            true
            exit
        then
    next-item
                                        \ sta1
    drop
    false
;
