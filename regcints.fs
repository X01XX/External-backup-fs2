\ Implement a struct and functions for a regioncorr and its intersections.

#61979 constant regcints-struct-id
    #3 constant regcints-struct-number-cells

\ Struct fields
0                                  constant regcints-header-disp          \ 16-bits [0] struct id [1] use count
regcints-header-disp       cell+   constant regcints-regioncorr-disp      \ A regioncorr.
regcints-regioncorr-disp   cell+   constant regcints-intersections-disp   \ A list of two, or more, regioncorrs that all intersect.

0 value regcints-mma  \ Storage for region mma instance.

\ Init regcints mma, return an address of allocated memory.
: regcints-mma-init ( num-items -- ) \ sets regcints-mma.
    dup 1 <
    abort" regcints-mma-init: Invalid number of items."

    cr ." Initializing RegcInts store."
    regcints-struct-number-cells swap mma-new to regcints-mma
;

\ Check if tos is an allocated regcints.
: is-regcints? ( tos -- bool )
    dup regcints-mma mma-is-item?   \ addr bool
    if
        struct-get-id
        regcints-struct-id =        \ bool
    else
        drop
        false                       \ f
    then
;

' is-regcints? to is-regcints?-xt

\ Start accessors.

\ Return the regioncorr field from a regcints instance.
: regcints-get-regioncorr ( regci0 -- regci-lst )
    \ Check arg.
    assert( tos is-regcints? )

    regcints-regioncorr-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ ' regcints-get-regioncorr to regcints-get-regioncorr-xt

\ Set the regioncorr field from a regcints instance, use only in this file.
: _regcints-set-regioncorr ( regc-lst1 regcis0 -- )
    \ Check args.
    assert( tos is-regcints? )
    assert( nos is-regioncorr? )

    \ Store list
    regcints-regioncorr-disp +  \ Add offset.\ within2 to1 from0 pthstp
    !struct                     \ Set the field.
;

\ Return the intersections field from a regcints instance.
: regcints-get-intersections ( regci0 -- regci-lst )
    \ Check arg.
    assert( tos is-regcints? )

    regcints-intersections-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ ' regcints-get-intersections to regcints-get-intersections-xt

\ Set the intersections field from a regcints instance, use only in this file.
: _regcints-set-intersections ( regc-lst1 regci0 -- )
    \ Check args.
    assert( tos is-regcints? )
    assert( nos is-regioncorr-list? )
\ within2 to1 from0 pthstp
    \ Store list
    regcints-intersections-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ End accessors.

: regcints-new ( regc-lst1 regc0 -- regcis )
    \ Check args.
    assert( tos is-regioncorr? )
    assert( nos is-regioncorr-list? )

    \ cr ." regcints-new: start: " .stack cr

    2dup swap regioncorr-list-all-subset?
    ifnot
        2drop
        false
    then

    \ Allocate space.
    regcints-struct-id regcints-mma
    struct-allocate                     \ regc-lst0 regc regcis

    \ Store regioncorr.
    tuck _regcints-set-regioncorr      \ regc-lst0 regcis

    \ Store intersections.
    tuck _regcints-set-intersections    \ regcis
    true
    \ cr ." regcints-new: end: " .stack cr
;

: .regcints ( regcis0 -- )
    \ Check arg.
    assert( tos is-regcints? )

    ." ( regcis "
    dup regcints-get-regioncorr .regioncorr \ lst
    regcints-get-intersections              \ lst
    space .regioncorr-list
    ." )"
;

\ ' .regcints to .regcints-xt

: regcints-deallocate ( regcis0 -- )
    \ Check arg.
    assert( tos is-regcints? )

    dup struct-get-use-count            \ regcis0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate regioncorr.
        dup regcints-get-regioncorr    \ regcis0 reg-lst
        regioncorr-deallocate

        \ Deallocate intersections.
        dup regcints-get-intersections \ regcis0 reg-lst
        regioncorr-list-deallocate

        \ Deallocate instance.
        regcints-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check if a list could be a regcints definintion.
: regcints-list-definition? ( tos -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regcints-valid-definition?: start: " .stack cr

    \ Check list length.
    dup list-get-length #3 =
    ifnot
        drop false
        \ cr ." regcints-list-definition?: exit 1" cr
        exit
    then

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?
    ifnot
        drop false
        \ cr ." regcints-list-definition?: exit 2" cr
        exit
    then

    s" regcis"                           \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." regcints-list-definition?: exit 3" cr
        exit
    then

    \ Check regioncorr.
    dup list-get-second-item            \ lst second
    is-regioncorr?                      \ lst bool
    ifnot
        drop false
        \ cr ." regcints-list-definition?: exit 4: " .stack cr
        exit
    then

    \ Check regioncorr intersections list.
    dup list-get-third-item             \ lst third
    is-regioncorr-list?                 \ lst bool
    ifnot
        drop false
        \ cr ." regcints-list-definition?: exit 5" cr
        exit
    then

    drop
    true
    \ cr ." regcints-list-definition?: end: " .stack cr
;

\ Return a regcints from a list.
: regcints-from-list ( tos -- regcis t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." regcints-from-list: start: " .stack cr

    \ cr dup .struct-list cr
    dup regcints-list-definition?    \ lst bool

    ifnot
        \ cr ." regcints-from-list: exit 1" cr
        drop false exit
    then

    dup list-get-third-item             \ lst regc-lst
    swap list-get-second-item           \ regc-lst second

    \ Allocate new regcints.
    regcints-new                        \ regcis t | f
    if
        true
    else
        false
    then
    \ cr ." regcints-from-list: end: " .stack cr
;

\ Return a regcints from a string.
\ Like ( regcis ( regc 0 0 (r1000 r1010)) (( regc 0 0 (r1000 r1010)) ( regc 0 0 (r1000 r1010))))
\ ( hint-string  intersection-regioncorr  regionrorr-list )
: regcints-from-string ( str-addr str-n -- regc t | f )
    \ cr ." regcints-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." regcints-from-string: exit 1 " cr
        exit
    then

    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." regcints-from-string: exit 2 " cr
        exit
    then

    dup list-get-first-item                 \ lst item
    is-regcints?                            \ lst bool
    ifnot
        struct-list-deallocate
        false
        \ cr ." regcints-from-string: exit 3 " cr
        exit
    then

    dup list-pop                            \ lst, item t | f
    invert abort" pop failed?"
    swap list-deallocate                    \ item

    true
    \ cr ." regcints-from-string: end: true " over .regcints cr
;

\ Return a regioncorritn from a string, or abort.
: regcints-from-string-a ( str-addr str-n -- regcis )
    regcints-from-string    \ regc t | f
    false? abort" regcints-from-string-a failed?"
;


