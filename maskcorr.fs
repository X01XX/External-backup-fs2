\ Implement a struct and functions for a mask list corresponding to domains.
\
\ So the masks may be of different number of bits, and operations
\ on maskcorr list pairs are by corresponding items.

#53717 constant maskcorr-struct-id
    #2 constant maskcorr-struct-number-cells

\ Struct fields
0                                   constant maskcorr-header-disp \ 16-bits [0] struct id [1] use count
maskcorr-header-disp    cell+       constant maskcorr-list-disp   \ Mask list corresponding, in bits used, to the session domain list.

0 value maskcorr-mma    \ Storage for mask mma instance.

\ Init maskcorr mma, return an address of allocated memory.
: maskcorr-mma-init ( num-items -- )    \ sets maskcorr-mma.
    dup 1 <
    abort" maskcorr-mma-init: Invalid number of items."

    cr ." Initializing MaskCorr store."
    maskcorr-struct-number-cells swap mma-new to maskcorr-mma
;

\ Check if tos is an allocated maskcorr.
: is-maskcorr? ( tos -- bool )
    dup maskcorr-mma mma-is-item?   \ tos bool
    if
        struct-get-id
        maskcorr-struct-id =        \ bool
    else
        drop
        false                       \ f
    then
;

' is-maskcorr? to is-maskcorr?-xt

\ Start accessors.

\ Return the list field from a maskcorr instance.
: maskcorr-get-list ( mskc0 -- mskc-lst )
    \ Check arg.
    assert( tos is-maskcorr? )

    maskcorr-list-disp +    \ Add offset.
    @                       \ Fetch the field.
;

' maskcorr-get-list to maskcorr-get-list-xt

\ Set the list field from a maskcorr instance, use only in this file.
: _maskcorr-set-list ( mskc-lst1 mskc0 -- )
    \ Check args.
    assert( tos is-maskcorr? )
    assert( nos is-mask-list? )
    \ assert( nos list-is-empty? invert )

    \ Store list
    maskcorr-list-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a maskcorr from a mask-list.
: maskcorr-new ( msk-lst0 -- mskc )
    \ Check arg.
    assert( tos is-mask-list? )
    assert( tos list-is-not-empty? )

    \ Allocate instance.
    maskcorr-struct-id maskcorr-mma
    struct-allocate                     \ msk-lst0 mskc

    \ Store list.
    tuck                                \ mskc msk-lst0 mskc
    _maskcorr-set-list                  \ mskc

;

\ Print a mask-list corresponding to the session domain list.
: .maskcorr ( mskc0 -- )
    \ Check arg.
    assert( tos is-maskcorr? )

    ." ( mskc "
    maskcorr-get-list               \ lst
    .mask-list
    ." )"
;

' .maskcorr to .maskcorr-xt

\ Deallocate the given mskc, if its use count is 1 or 0.
: maskcorr-deallocate ( mskc0 -- )
    \ Check arg.
    assert( tos is-maskcorr? )

    dup struct-get-use-count            \ mskc0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate fields.
        dup maskcorr-get-list           \ mskc0 msk-lst
        mask-list-deallocate

        \ Deallocate instance.
        maskcorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Check if a list could be a maskcorr definintion.
: maskcorr-list-definition? ( lst -- bool )
    \ Check arg.
    assert( tos is-list? )
    \ cr ." maskcorr-list-definition?: start: " dup .struct-list cr

    \ Check hint token.
    dup list-get-first-item             \ lst first
    is-token?-xt execute
    ifnot
        drop false
        \ cr ." maskcorr-list-definition?: exit 1: false " cr
        exit
    then

    s" mskc"                            \ lst c-addr u
    #2 pick list-get-first-item         \ lst c-addr u first
    token-eq-string                     \ lst bool
    ifnot
        drop false
        \ cr ." maskcorr-list-definition?: exit 2: false " cr
        exit
    then

    \ Check mask list.
    dup list-get-second-item            \ lst second
    is-mask-list?
    ifnot
        drop false
        \ cr ." maskcorr-list-definition?: exit 3: false " cr
        exit
    then

    dup list-get-second-item            \ lst second
    list-is-empty?
    if
        drop false
        \ cr ." maskcorr-list-definition?: exit 4: false " cr
        exit
    then

    drop
    true
    \ cr ." maskcorr-list-definition?: end: true" cr
;

\ Return a maskcorr from a list.
: maskcorr-from-list ( lst -- mskc t | f)
    \ Check arg.
    assert( tos is-list? )
    \ cr ." maskcorr-from-list: start" cr

    dup maskcorr-list-definition?     \ lst bool
    ifnot
        drop false
        \ cr ." maskcorr-from-list: end false" cr
        exit
    then

    \ Allocate new maskcorr.
    dup list-get-second-item            \ lst second
    maskcorr-new                        \ lst mskc

    nip                                 \ mskc

    true
    \ cr ." maskcorr-from-list: end true: " over .maskcorr cr
;
\ Return a maskcorr from a string.
\ Like ( mskc (s1010 s1010))
: maskcorr-from-string ( str-addr str-n -- mskc t | f )
    \ cr ." maskcorr-from-string: start: " 2dup type cr

    \ Convert string to list.
    list-from-string-xt execute             \ lst t | f
    ifnot
        false
        \ cr ." maskcorr-from-string: exit 1: false" cr
        exit
    then
                                            \ lst
    dup list-get-length 1 <>
    if
        struct-list-deallocate
        false
        \ cr ." maskcorr-from-string: exit 2: false" cr
        exit
    then
                                            \ lst
    dup list-get-first-item                 \ lst itm
    is-maskcorr?                            \ lst bool
    if
        dup list-pop-struct                 \ lst itm
        invert abort" pop faled?"
        swap list-deallocate                \ itm
        true
        \ cr ." maskcorr-from-string: end: true"  .mskck cr
        exit
    else
        struct-list-deallocate
        false
        \ cr ." maskcorr-from-string: exit 3: false" cr
        exit
    then
;

\ Return a maskcorr from a string, or abort.( mskc 1  -4 (rx1x1 r0x111))
: maskcorr-from-string-a ( str-addr str-n -- mskc )
    maskcorr-from-string    \ mskc t | f
    false? abort" maskcorr-from-string failed?"
;

\ Return the number of bits set.
: maskcorr-num-bits-set ( mskc0 -- nbs )
    \ Check args.
    assert( tos is-maskcorr? )

    \ Init counter.
    0 swap                  \ cnt mskc0

    \ Prep for loop.
    maskcorr-get-list       \ cnt msk-lst

    foreach                 \ cnt msk-lnk mskx
        mask-num-bits-set   \ cnt msk-lnk num
        rot +               \ msk-lnk cnt+
        swap                \ cnt+ msk-lnk
    next-item
;

\ Return true if two maskcorrs are equal.
: maskcorrs-eq? ( mskc1 mskc0 -- bool )
    \ Check args.
    assert( tos is-maskcorr? )
    assert( nos is-maskcorr? )

    maskcorr-get-list list-get-links swap         \ lnk0 msk-lst1
    maskcorr-get-list list-get-links              \ lnk0 lnk1

    begin
        ?dup
    while
        over link-get-data      \ lnk0 lnk1 msk0
        over link-get-data      \ lnk0 lnk1 msk0 msk1
        masks-eq?               \ lnk0 lnk1 bool
        ifnot
            2drop
            false
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
                                \ lnk0
    drop
    true
;
