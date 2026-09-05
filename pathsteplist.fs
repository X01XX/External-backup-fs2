
\ Check TOS for pathstep-list.
: is-pathstep-list? ( tos -- bool )
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
    is-pathstep?            \ bool
;

\ Print a pathstep-list
: .pathstep-list ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathstep-list? )

    [ ' .pathstep ] literal swap .list
;

: .pathstep-list-prefix ( c-addr u list0 -- )
    \ Check arg.
    assert( tos is-pathstep-list? )
    cr
    rot                 \ u list0 c-addr
    #2 pick             \ u list0 c-addr u
    type                \ u list0

    list-get-links      \ u lnk

    begin
        ?dup
    while
        dup link-get-data .pathstep

        link-get-next
        dup 0<> if
            over cr spaces
        then
    repeat
                        \ u
    drop
    cr
;

: pathstep-list-deallocate ( pthstp-lst0 -- )
    \ Check arg.
    assert( tos is-pathstep-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ pthstp-lst0 uc
    #2 < if
        \ Deallocate pathstep instances in the list.
        [ ' pathstep-deallocate ] literal over      \ pthstp-lst0 xt pthstp-lst0
        list-apply                                  \ pthstp-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Push a pathstep to the beginning of a pathstep list.
\ If the list is not empty, the next step from field
\ should equal the new step to field.
: pathstep-list-push ( pthstp1 pthstp-lst0 -- )
    \ Check args.
    assert( tos is-pathstep-list? )
    assert( nos is-pathstep? )

    dup list-get-length         \ pthstp1 pthstp-lst0 len
    0>                          \ pthstp1 pthstp-lst0 bool
    if
        dup list-get-first-item \ pthstp1 pthstp-lst0 first
        pathstep-get-from       \ pthstp1 pthstp-lst0 from
        #2 pick                 \ pthstp1 pthstp-lst0 from pthstp1
        pathstep-get-to         \ pthstp1 pthstp-lst0 from to
        \ cr ." from " over .regioncorr space ." to: " dup .regioncorr cr
        regioncorrs-eq-regions? \ pthstp1 pthstp-lst0 bool
        ifnot
            cr ." from-to intra pathstep error?" cr
            abort
        then
    then

    list-push-struct
;

\ Push a pathstep to the end of a pathstep list.
\ If there is a previous step, the previous step to field
\ should equal the new step from field.
: pathstep-list-push-end ( pthstp1 pthstp-lst0 -- )
    \ Check args.
    assert( tos is-pathstep-list? )
    assert( nos is-pathstep? )

    dup list-get-length         \ pthstp1 pthstp-lst0 len
    0>                          \ pthstp1 pthstp-lst0 bool
    if
        dup list-get-last-item  \ pthstp1 pthstp-lst0 last
        pathstep-get-to         \ pthstp1 pthstp-lst0 to
        #2 pick                 \ pthstp1 pthstp-lst0 to pthstp1
        pathstep-get-from       \ pthstp1 pthstp-lst0 to from
        regioncorrs-eq-regions? \ pthstp1 pthstp-lst0 bool
        ifnot
            cr ." to-from intra pathstep error?" cr
            abort
        then
    then

    list-push-end-struct
;

\ Return the start regioncorr of a non-empty pathstep-list.
: pathstep-list-get-from ( pthstp-lst -- regc )
    \ Check arg.
    assert( tos is-pathstep-list? )

    list-get-first-item     \ pthstp
    pathstep-get-from       \ regc
;

\ Return the end regioncorr of a non-empty pathstep-list.
: pathstep-list-get-to ( pthstp-lst -- regc )
    \ Check arg.
    assert( tos is-pathstep-list? )

    list-get-last-item      \ pthstp
    pathstep-get-to         \ regc
;

\ Rate regioncorrs within a pathsteps, given a regioncorr list.
: pathstep-list-rate ( regc-lst1 pthstp-lst0 -- )
    \ Check args.
    assert( tos is-pathstep-list? )
    assert( nos is-regioncorr-list? )

    foreach                     \ regc-lst1 pthstp-lnk0 pthstp0
        #2 pick swap            \ regc-lst1 pthstp-lnk0 regc-lst1 pthstp0
        pathstep-rate           \ regc-lst1 pthstp-lnk0
    next
                                \ regc-lst1
    drop
;
