\ Implement a struct and functions for a regioncorr intersection and its intersectors.

#23173 constant intregcs-struct-id
    #3 constant intregcs-struct-number-cells

\ Struct fields
0                                  constant intregcs-header-disp          \ 16-bits [0] struct id [1] use count
intregcs-header-disp       cell+   constant intregcs-intersection-disp    \ A regioncorr.
intregcs-intersection-disp cell+   constant intregcs-regioncorrs-disp     \ A list of two, or more, regioncorrs that all intersect.

0 value intregcs-mma  \ Storage for region mma instance.

\ Init intregcs mma, return an address of allocated memory.
: intregcs-mma-init ( num-items -- ) \ sets intregcs-mma.
    dup 1 <
    abort" intregcs-mma-init: Invalid number of items."

    cr ." Initializing IntRegcs store."
    intregcs-struct-number-cells swap mma-new to intregcs-mma
;

\ Check if tos is an allocated intregcs.
: is-intregcs? ( tos -- bool )
    dup intregcs-mma mma-is-item?   \ addr bool
    if
        struct-get-id
        intregcs-struct-id =        \ bool
    else
        drop
        false                       \ f
    then
;

' is-intregcs? to is-intregcs?-xt

\ Start accessors.

\ Return the intersection field from a intregcs instance.
: intregcs-get-intersection ( iregcs0 -- regc )
    \ Check arg.
    assert( tos is-intregcs? )

    intregcs-intersection-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ ' intregcs-get-intersection to intregcs-get-intersection-xt

\ Set the intersection field from a intregcs instance, use only in this file.
: _intregcs-set-intersection ( regc iregcs0 -- )
    \ Check args.
    \ cr ." _intregcs-set-intersection: " .stack cr
    assert( tos is-intregcs? )
    assert( nos is-regioncorr? )

    \ Store list
    intregcs-intersection-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the list field from a intregcs instance.
: intregcs-get-regioncorrs ( iregcs0 -- regcs-lst )
    \ Check arg.
    assert( tos is-intregcs? )

    intregcs-regioncorrs-disp + \ Add offset.
    @                           \ Fetch the field.
;

' intregcs-get-regioncorrs to intregcs-get-regioncorrs-xt

\ Set the list field from a intregcs instance, use only in this file.
: _intregcs-set-regioncorrs ( regc-lst1 iregcs0 -- )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-regioncorr-list? )
    assert( nos list-get-length 1 > )

    \ Store list
    intregcs-regioncorrs-disp + \ Add offset.
    !struct                     \ Set the field.
;

\ End accessors.

\ Create a regioncorr from a region-list.
: intregcs-new ( regc-lst0 regc -- iregcs t | f )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr-list? )
    \ cr ." intregcs-new: start: " .stack cr

    over list-get-length #2 <
    if
        2drop
        false
        \ cr ." intregcs-new: exit 1: " cr
        exit
    then

    2dup swap regioncorr-list-all-superset?
    ifnot
        2drop
        false
        \ cr ." intregcs-new: exit 2: " cr
    then

    \ Allocate space.
    intregcs-struct-id intregcs-mma
    struct-allocate                     \ regc-lst0 regc iregcs

    \ Store intersection.
    tuck _intregcs-set-intersection     \ regc-lst0 iregcs

    \ Store regioncorrs.
    tuck _intregcs-set-regioncorrs      \ iregcs
    true
    \ cr ." intregcs-new: end: " .stack cr
;

' intregcs-new to intregcs-new-xt

\ Print a region-list corresponding to the session domain list.
: .intregcs ( iregcs0 -- )
    \ Check arg.
    assert( tos is-intregcs? )

    ." ( iregcs "
    dup intregcs-get-intersection .regioncorr
    intregcs-get-regioncorrs               \ lst
    .regioncorr-list
    ." )"
;

' .intregcs to .intregcs-xt

\ Deallocate the given iregcs, if its use count is 1 or 0.
: intregcs-deallocate ( iregcs0 -- )
    \ Check arg.
    assert( tos is-intregcs? )

    dup struct-get-use-count            \ iregcs0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate intersection.
        dup intregcs-get-intersection  \ iregcs0 reg-lst
        regioncorr-deallocate

        \ Deallocate fields.
        dup intregcs-get-regioncorrs   \ iregcs0 reg-lst
        regioncorr-list-deallocate

        \ Deallocate instance.
        intregcs-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: intregcss-share-regioncorr? ( iregcs1 iregcs0 -- bool )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-intregcs? )

    \ Get regioncorr lists.
    swap intregcs-get-regioncorrs
    swap intregcs-get-regioncorrs   \ regc-lst1 regc-lst0

    \ Get intersection of lists.
    [ ' = ] literal -rot            \ xt regc-lst1 regc-lst0
    list-intersection-struct        \ regc-int-list

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

' intregcss-share-regioncorr? to intregcss-share-regioncorr?-xt

: intregcss-shared-regioncorr ( iregcs1 iregcs0 -- bool )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-intregcs? )

    \ Get regioncorr lists.
    swap intregcs-get-regioncorrs
    swap intregcs-get-regioncorrs   \ regc-lst1 regc-lst0

    \ Get intersection of lists.
    [ ' = ] literal -rot            \ xt regc-lst1 regc-lst0
    list-intersection-struct        \ regc-int-list
;

' intregcss-shared-regioncorr to intregcss-shared-regioncorr-xt

\ Return the length of the regioncorr list.
: intregcs-get-length ( iregcs0 -- len )
    \ Check arg.
    assert( tos is-intregcs? )

    intregcs-get-regioncorrs    \ regc-lst
    list-get-length             \ len
;

' intregcs-get-length to intregcs-get-length-xt

\ Check if a list could be a intregcs definintion.
: intregcs-list-definition? ( tos -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." intregcs-valid-definition?: start: " .stack cr

    \ Check list length.
    dup list-get-length #3 =
    ifnot
        drop false
        \ cr ." intregcs-list-definition?: exit 1" cr
        exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." intregcs-list-definition?: exit 2" cr
        exit
    then

    s" iregcs"                          \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        cr ." intregcs-list-definition?: exit 3" cr
        exit
    then

    \ Check regioncorr intersection.
    dup list-get-second-item            \ lst second
    is-regioncorr?                      \ lst bool
    ifnot
        drop false
        \ cr ." intregcs-list-definition?: exit 4: " .stack cr
        exit
    then

    \ Check regioncorr list.
    dup list-get-third-item             \ lst third
    is-regioncorr-list?                 \ lst bool
    ifnot
        drop false
        \ cr ." intregcs-list-definition?: exit 5" cr
        exit
    then

    dup list-get-third-item             \ lst third
    list-get-length #2 <                \ lst bool
    if
        drop false
        \ cr ." intregcs-list-definition?: exit 7" cr
        exit
    then

    drop
    true
    \ cr ." intregcs-list-definition?: end: " .stack cr
;

\ Return a intregcs from a list.
: intregcs-from-list ( tos -- iregcs t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." intregcs-from-list: start: " .stack cr

    \ cr dup .struct-list cr
    dup intregcs-list-definition?    \ lst bool

    ifnot
        \ cr ." intregcs-from-list: exit 1" cr
        drop false exit
    then

    dup list-get-third-item             \ lst regc-lst
    swap list-get-second-item           \ regc-lst second

    \ Allocate new intregcs.
    intregcs-new                        \ iregcs t | f
    if
        true
    else
        false
    then
    \ cr ." intregcs-from-list: end: " .stack cr
;

\ Return a intregcs from a string.
\ Like ( iregcs ( regc 0 0 (r1000 r1010)) (( regc 0 0 (r1000 r1010)) ( regc 0 0 (r1000 r1010))))
\ ( hint-string  intersection-regioncorr  regionrorr-list )
: intregcs-from-string ( str-addr str-n -- regc t | f )
    \ cr ." intregcs-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." intregcs-from-string: exit 1 " cr
        exit
    then

    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." intregcs-from-string: exit 2 " cr
        exit
    then

    dup list-get-first-item                 \ lst item
    is-intregcs?                            \ lst bool
    ifnot
        struct-list-deallocate
        false
        \ cr ." intregcs-from-string: exit 3 " cr
        exit
    then

    dup list-pop                            \ lst, item t | f
    invert abort" pop failed?"
    swap list-deallocate                    \ item

    true
    \ cr ." intregcs-from-string: end: true " over .intregcs cr
;

\ Return a regioncorritn from a string, or abort.
: intregcs-from-string-a ( str-addr str-n -- iregcs )
    intregcs-from-string    \ regc t | f
    false? abort" intregcs-from-string-a failed?"
;

: intregcs-min-distance ( iregcs1 iregcs0 -- u )
    \ Check args.
    assert( tos is-intregcs? )
    assert( nos is-intregcs? )

    intregcs-get-regioncorrs
    swap
    intregcs-get-regioncorrs

    regioncorr-lists-min-distance
;
