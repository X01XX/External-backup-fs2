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
: regioncorrint-new ( regc-lst0 regc -- regc t | f )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr-list? )
    \ cr ." regioncorrint-new: start: " .stack cr

    over list-get-length #2 <
    if
        2drop
        false
        exit
    then

    2dup swap regioncorr-list-all-superset?
    ifnot
        2drop
        false
    then

    \ Allocate space.
    regioncorrint-struct-id regioncorrint-mma
    struct-allocate                         \ regc-lst0 regc regci

    \ Store intersection.
    tuck _regioncorrint-set-intersection    \ regc-lst0 regci

    \ Store list.
    tuck _regioncorrint-set-list           \ regci
    true
    \ cr ." regioncorrint-new: end: " .stack cr
;

' regioncorrint-new to regioncorrint-new-xt

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

: regioncorrints-share-regioncorr? ( regci1 regci0 -- bool )
    \ Check args.
    assert( tos is-regioncorrint? )
    assert( nos is-regioncorrint? )

    \ Get regioncorr lists.
    swap regioncorrint-get-list
    swap regioncorrint-get-list         \ regc-lst1 regc-lst0

    \ Get intersection of lists.
    [ ' = ] literal -rot                \ xt regc-lst1 regc-lst0
    list-intersection-struct            \ regc-int-list

    \ Return.
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        regioncorr-list-deallocate
        true
    then
;

' regioncorrints-share-regioncorr? to regioncorrints-share-regioncorr?-xt

: regioncorrints-shared-regioncorr ( regci1 regci0 -- bool )
    \ Check args.
    assert( tos is-regioncorrint? )
    assert( nos is-regioncorrint? )

    \ Get regioncorr lists.
    swap regioncorrint-get-list
    swap regioncorrint-get-list         \ regc-lst1 regc-lst0

    \ Get intersection of lists.
    [ ' = ] literal -rot                \ xt regc-lst1 regc-lst0
    list-intersection-struct            \ regc-int-list
;

' regioncorrints-shared-regioncorr to regioncorrints-shared-regioncorr-xt

\ Return the length of the regioncorr list.
: regioncorrint-get-length ( regc0 -- len )
    \ Check args.
    assert( tos is-regioncorrint? )

    regioncorrint-get-list      \ regc-lst
    list-get-length             \ len
;

' regioncorrint-get-length to regioncorrint-get-length-xt

\ Check if a list could be a regioncorrint definintion.
: regioncorrint-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regioncorrint-valid-definition?: start: " .stack cr

    \ Check list length.
    dup list-get-length #3 =
    ifnot
        drop false
        \ cr ." regioncorrint-list-definition?: exit 1" cr
        exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." regioncorrint-list-definition?: exit 2" cr
        exit
    then

    s" regci"                           \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." regioncorrint-list-definition?: exit 3" cr
        exit
    then

    \ Check regioncorr intersection.
    dup list-get-second-item            \ lst second
    is-regioncorr?                      \ lst bool
    ifnot
        drop false
        \ cr ." regioncorrint-list-definition?: exit 4: " .stack cr
        exit
    then

    dup list-get-third-item             \ lst third
    is-regioncorr-list?                 \ lst bool
    ifnot
        drop false
        \ cr ." regioncorrint-list-definition?: exit 5" cr
        exit
    then

    dup list-get-third-item             \ lst third
    list-get-length #2 <                 \ lst bool
    if
        drop false
        \ cr ." regioncorrint-list-definition?: exit 7" cr
        exit
    then

    drop
    true
    \ cr ." regioncorrint-list-definition?: end: " .stack cr
;

\ Return a regioncorr from a list.
: regioncorrint-from-list ( lst -- regc t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regioncorrint-from-list: start: " .stack cr

    \ cr dup .struct-list cr
    dup regioncorrint-list-definition?     \ lst bool

    ifnot
        \ cr ." regioncorrint-from-list: exit 1" cr
        drop false exit
    then

    dup list-get-third-item             \ lst regc-lst
    swap list-get-second-item           \ regc-lst second

    \ Allocate new regioncorrint.
    regioncorrint-new                   \ regci t | f
    if
        true
    else
        false
    then
    \ cr ." regioncorrint-from-list: end: " .stack cr
;

\ Return a regioncorrint from a string.
\ Like ( regci ( regc 0 0 (r1000 r1010)) (( regc 0 0 (r1000 r1010)) ( regc 0 0 (r1000 r1010))))
\ ( hint-string  intersection-regioncorr  regionrorr-list )
: regioncorrint-from-string ( str-addr str-n -- regc t | f )
    \ cr ." regioncorrint-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        exit
    then

    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        exit
    then

    dup list-get-first-item                 \ lst item
    is-regioncorrint?                       \ lst bool
    ifnot
        struct-list-deallocate
        false
        exit
    then

    dup list-pop                            \ lst, item t | f
    invert abort" pop failed?"
    swap list-deallocate                    \ item

    true
;

\ Return a regioncorritn from a string, or abort.
: regioncorrint-from-string-a ( str-addr str-n -- regc )
    regioncorr-from-string    \ regc t | f
    false? abort" regioncorrint-from-string-a failed?"
;
