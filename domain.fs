\ Implement a Domain struct and functions.

#31379 constant domain-struct-id
    #7 constant domain-struct-number-cells

\ Struct fields
0                                   constant domain-header-disp         \ 16-bits [0] struct id, [1] use count, [2] instance id (8 bits), num-bits (8 bits)
domain-header-disp          cell+   constant domain-parent-disp         \ A session ref.
domain-parent-session-disp  cell+   constant domain-actions-disp        \ An action list.
domain-actions-disp         cell+   constant domain-current-state-disp  \ A state/value.
domain-current-state-disp   cell+   constant domain-max-region-disp     \ A region with all valid bits set to X.
domain-max-region-disp      cell+   constant domain-all-bits-mask-disp  \ A mask of all bits set to 1.
domain-all-bits-mask-disp   cell+   constant domain-ms-bit-mask-disp    \ A mask with the most significant bit set to one.


0 value domain-mma \ Storage for domain mma instance.

\ Init domain mma, return the addr of allocated memory.
: domain-mma-init ( num-items -- ) \ sets domain-mma.
    dup 1 <
    abort" domain-mma-init: Invalid number of items."

    cr ." Initializing Domain store."
    domain-struct-number-cells swap mma-new to domain-mma
;

\ Check instance type.
: is-allocated-domain? ( addr -- bool )
    dup domain-mma mma-is-item? \ addr bool
    if
        struct-get-id
        domain-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for domain, unconventional, leaves stack unchanged.
: assert-tos-is-domain ( tos -- tos )
    dup is-allocated-domain?
    false? if
        s" TOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-tos-is-domain to assert-tos-is-domain-xt

\ Check NOS for domain, unconventional, leaves stack unchanged.
: assert-nos-is-domain ( nos tos -- nos tos )
    over is-allocated-domain?
    false? if
        s" NOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-nos-is-domain to assert-nos-is-domain-xt

\ Start accessors.

\ Return the parent session of the domain.
: domain-get-parent ( dom0 -- ses )
    \ Check arg.
    assert-tos-is-domain

    domain-parent-disp +    \ Add offset.
    @                       \ Fetch the field.
;

' domain-get-parent to domain-get-parent-xt

\ Set the parent session of an domain.
: _domain-set-parent ( ses1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-parent-disp +    \ Add offset.
    !                       \ Set the field.
;

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert-tos-is-domain

    domain-actions-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert-tos-is-domain
    assert-nos-is-action-list

    domain-actions-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the instance ID from an domain instance.
: domain-get-inst-id ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    4c@
;

' domain-get-inst-id to domain-get-inst-id-xt

\ Set the instance ID of an domain instance.
: domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 0<
    abort" Invalid instance id"

    over #255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the number bits used by a domain instance.
: domain-get-num-bits ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    5c@
;

' domain-get-num-bits to domain-get-num-bits-xt

\ Set the number bits used by a domain instance, use only in this file.
: _domain-set-num-bits ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 1 <
    abort" Invalid number of bits."

    over cell #8 * >
    abort" Invalid number of bits."

    \ Set inst id.
    5c!
;

\ Return the current state from a domain instance.
: domain-get-current-state ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-state-disp +
    @
;

' domain-get-current-state to domain-get-current-state-xt

\ Set the current state of a domain instance.
: domain-set-current-state ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-value

    \ Set inst id.
    domain-current-state-disp +
    !
;

\ Return the max-region of the domain.
: domain-get-max-region ( dom0 -- reg )
    \ Check arg.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-max-region to domain-get-max-region-xt

\ Set the max region of the domain.
: _domain-set-max-region ( reg1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    !struct                     \ Set the field.
;

\ Return the all-bits-mask of the domain.
: domain-get-all-bits-mask ( dom0 -- msk )
    \ Check arg.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-all-bits-mask to domain-get-all-bits-mask-xt

\ Set the max region of the domain.
: _domain-set-all-bits-mask ( msk1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    !                               \ Set the field.
;

\ Return the ms-bit-mask of the domain.
: domain-get-ms-bit-mask ( dom0 -- msk )
    \ Check arg.
    assert-tos-is-domain

    domain-ms-bit-mask-disp +   \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-ms-bit-mask to domain-get-ms-bit-mask-xt

\ Set the max region of the domain.
: _domain-set-ms-bit-mask ( msk1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-ms-bit-mask-disp +   \ Add offset.
    !                           \ Set the field.
;

\ End accessors.

\ Create a domain, given the number of bits to be used.
\
\ The domain instance ID defaults to zero.
\ The instance ID will likely be reset to match its position in a list,
\ using domain-set-inst-id, which avoids duplicates and may be useful as an index into the list.
\
\ The current state defaults to zero, but can be set with domain-set-current-state.
: domain-new ( nb1 ses0 -- dom )
    \ Check arg.
    assert-tos-is-session-xt execute

    \ Check number bits.
    over 1 < abort" Number bits < 1?"
    over
    \ Get max num bits.
    1 cells #8 *
    > abort" Number bits too large?"

    \ Allocate space.
    domain-struct-id domain-mma     \ nb1 ses0 id mma
    struct-allocate                 \ nb1 ses0 dom

    \ Set instance ID, based on its position in the session domain list.
    over                            \ nb1 ses0 dom sess0
    session-get-number-domains-xt   \ nb1 ses0 dom sess0 xt
    execute                         \ nb1 ses0 dom nd
    over                            \ nb1 ses0 dom nd dom
    domain-set-inst-id              \ nb1 ses0 dom

    \ Set num bits.
    rot over                        \ ses0 dom nb1 dom
    _domain-set-num-bits            \ ses0 dom

    \ Set parent session field.
    tuck                            \ dom ses0 dom
    _domain-set-parent-session      \ dom

    \ Set actions list.
    list-new                        \ dom lst
    2dup swap                       \ dom lst lst dom
    _domain-set-actions             \ dom lst

    \ Add action 0.
    \ When making multi-step plans of all regions, a no-op for one domain preserves
    \ knowledge of all result states for subsequent steps.
    [ ' act-0-get-sample ] literal  \ dom lst xt
    #2 pick                         \ dom lst xt dom
    action-new dup                  \ dom lst act act
    rot                             \ dom act act lst
    action-list-push-end            \ dom act

    \ Set all bits mask.
    dup domain-get-num-bits \ dom u
    all-bits                \ dom mask
    over _domain-set-all-bits-mask

    \ Set max region.
    dup domain-get-all-bits-mask    \ dom msk
    0                               \ dom msk 0
    #2 pick domain-get-num-bits     \ dom msk 0 nb
    region-new2                     \ dom regx
    over _domain-set-max-region     \ dom

    \ Set the most significant bit mask.
    dup domain-get-num-bits \ dom u
    1-                      \ dom u'
    1 swap lshift           \ dom mask
    over _domain-set-ms-bit-mask    \ dom

    \ Set arbitrary current state.
    0 over domain-current-state-disp + ! \ dom
;

\ Print a domain.
: .domain ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup domain-get-inst-id
    cr cr ." Dom: " dec.

    dup domain-get-num-bits ." num-bits: " dec. space
    dup domain-get-actions
    list-get-length
    ."  num actions: " dec.
    dup domain-get-current-state ." cur: " .value
    cr
    domain-get-actions .action-list
;

\ Deallocate a domain.
: domain-deallocate ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup domain-get-actions action-list-deallocate
        dup domain-get-max-region region-deallocate

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: domain-add-action ( xt1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    tuck                        \ dom0 xt1 dom0

    action-new                  \ dom0 actx

    swap                        \ actx dom0
    2dup                        \ actx dom0 actx dom0
    domain-get-actions          \ actx dom0 actx act-lst
    action-list-push-end        \ actx dom0

    domain-set-current-action   \
;

\ Get a sample from an action in a domain.
\ Call only from session-get-sample, since current-domain in set there.
: domain-get-sample ( act1 dom0 -- smpl )
     \ Check args.
    assert-tos-is-domain
    assert-nos-is-action

    \ Get action sample.
    dup domain-get-current-state    \ act1 dom0 | d-sta
    #2 pick                         \ act1 dom0 | d-sta act1
    action-get-sample               \ act1 dom0 | smpl

    \ Set domain current state.
    dup sample-get-result           \ act1 dom0 | smpl sta
    #2 pick                         \ act1 dom0 | smpl sta dom
    domain-set-current-state        \ act1 dom0 | smpl

\    cr
\    over domain-get-inst-id cr ." Dom: " #3 dec.r   \ act1 dom0 | smpl
\    space #2 pick action-get-inst-id ." Act: " #3 dec.r   \ smpl
\    space dup .sample
\    cr

    swap                            \ act1 smpl dom
    domain-update-session-points    \ act1 smpl

    nip
;

' domain-get-sample to domain-get-sample-xt

\ Return a action, given a action ID.
: domain-find-action ( u1 dom0 -- act t | f )
    \ cr ." domain-find-action: Dom: " dup domain-get-inst-id . space over . cr
    \ Check args.
    assert-tos-is-domain
    over 0< if
        2drop
        false
        exit
    then

    tuck domain-get-actions \ dom0 u1 act-lst
    2dup list-get-length    \ dom0 u1 act-lst u1 len
    >= if                   \ dom0 u1 act-lst
        3drop
        false
        exit
    then

    list-get-item               \ dom0 act
    tuck swap                   \ act act dom0
    domain-set-current-action   \ act
    true
;

: domain-get-number-actions ( dom -- na )
    \ Check arg.
    assert-tos-is-domain

    domain-get-actions      \ act-lst
    list-get-length         \ len
;

' domain-get-number-actions to domain-get-number-actions-xt

