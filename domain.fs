\ Implement a Domain struct and functions.

#31379 constant domain-struct-id
    #7 constant domain-struct-number-cells

\ Struct fields
0                                   constant domain-header-disp         \ 16-bits [0] struct id, [1] use count, [2] instance id (8 bits), num-bits (8 bits)
domain-header-disp          cell+   constant domain-parent-disp         \ A session ref, may be zero for testing.
domain-parent-disp          cell+   constant domain-actions-disp        \ An action list.
domain-actions-disp         cell+   constant domain-current-state-disp  \ A state.
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

\ Check if tos is an allocated domain.
: is-domain? ( addr -- bool )
    dup domain-mma mma-is-item? \ addr bool
    if
        struct-get-id
        domain-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

' is-domain? to is-domain?-xt

\ Start accessors.

\ Return the parent session of the domain.
: domain-get-parent ( dom0 -- ses )
    \ Check arg.
    assert( tos is-domain? )

    domain-parent-disp +    \ Add offset.
    @                       \ Fetch the field.
;

' domain-get-parent to domain-get-parent-xt

\ Set the parent session of an domain.
: _domain-set-parent ( ses1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

    domain-parent-disp +    \ Add offset.
    !                       \ Set the field.
;

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert( tos is-domain? )

    domain-actions-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert( tos is-domain? )
    assert( nos is-action-list? )

    domain-actions-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the instance ID from an domain instance.
: domain-get-inst-id ( dom0 -- u)
    \ Check arg.
    assert( tos is-domain? )

    \ Get intst ID.
    4c@
;

' domain-get-inst-id to domain-get-inst-id-xt

\ Set the instance ID of an domain instance.
: _domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

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
    assert( tos is-domain? )

    \ Get intst ID.
    5c@
;

' domain-get-num-bits to domain-get-num-bits-xt

\ Set the number bits used by a domain instance, use only in this file.
: _domain-set-num-bits ( u1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

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
    assert( tos is-domain? )

    domain-current-state-disp +
    @
;

' domain-get-current-state to domain-get-current-state-xt

\ Set the current state of a domain instance.
: _domain-set-current-state ( sta1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )
    assert( nos is-state? )

    \ Set inst id.
    domain-current-state-disp +
    !
;

\ Return the max-region of the domain.
: domain-get-max-region ( dom0 -- reg )
    \ Check arg.
    assert( tos is-domain? )

    domain-max-region-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-max-region to domain-get-max-region-xt

\ Set the max region of the domain.
: _domain-set-max-region ( reg1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

    domain-max-region-disp +    \ Add offset.
    !struct                     \ Set the field.
;

\ Return the all-bits-mask of the domain.
: domain-get-all-bits-mask ( dom0 -- msk )
    \ Check arg.
    assert( tos is-domain? )

    domain-all-bits-mask-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-all-bits-mask to domain-get-all-bits-mask-xt

\ Set the max region of the domain.
: _domain-set-all-bits-mask ( msk1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

    domain-all-bits-mask-disp +    \ Add offset.
    !                               \ Set the field.
;

\ Return the ms-bit-mask of the domain.
: domain-get-ms-bit-mask ( dom0 -- msk )
    \ Check arg.
    assert( tos is-domain? )

    domain-ms-bit-mask-disp +   \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-ms-bit-mask to domain-get-ms-bit-mask-xt

\ Set the max region of the domain.
: _domain-set-ms-bit-mask ( msk1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

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
: domain-new ( num-bits inst-id parent -- dom )
    \ Check args.

    \ Check number bits.
    #2 pick 1 < abort" Number bits < 1?"

    \ Get max num bits.
    #2 pick [ 1 cells #8 * ] literal > abort" Number bits too large?"

    \ Allocate space.
    domain-struct-id domain-mma     \ nb2 id1 prnt0 id mma
    struct-allocate                 \ nb2 id1 prnt0 dom

    \ Set parent.
    tuck                            \ nb2 id1 dom prnt0 dom
    _domain-set-parent              \ nb2 id1 dom

    \ Set instance ID.
    tuck                            \ nb2 dom id1 dom
    _domain-set-inst-id             \ nb2 dom

    \ Set num bits.
    2dup                            \ nb2 dom nb2 dom
    _domain-set-num-bits            \ nb2 dom

    \ Set actions list.
    list-new                        \ nb2 dom act-lst
    2dup swap                       \ nb2 dom act-lst lst dom
    _domain-set-actions             \ nb2 dom act-lst

    \ Add action 0.
    \ When making multi-step plans of all regions, a no-op for one domain preserves
    \ knowledge of all result states for subsequent steps.
    [ ' act-0-get-result ] literal  \ nb2 dom act-lst xt
    #3 pick                         \ nb2 dom act-lst xt nb2
    0                               \ nb2 dom act-lst xt nb2 id
    #4 pick                         \ nb2 dom act-lst xt nb2 id dom
    action-new                      \ nb2 dom act-lst act
    swap                            \ nb2 dom act act-lst
    action-list-push-end            \ nb2 dom

    \ Set all bits mask.
    over                            \ nb2 dom nb2
    dup all-bits                    \ nb2 dom nb2 mask
    swap mask-new                   \ nb2 dom msk
    over _domain-set-all-bits-mask  \ nb2 dom

    \ Set max region.
    over                            \ nb2 dom nb2
    all-bits                        \ nb2 dom value
    #2 pick state-new               \ nb2 dom sta1

    0                               \ nb2 dom sta1 0
    #3 pick                         \ nb2 dom sta1 0 nb2
    state-new                       \ nb2 dom sta1 sta2

    region-new                      \ nb2 dom regx
    over _domain-set-max-region     \ nb2 dom

    \ Set the most significant bit mask.
    over                            \ nb2 dom nb2
    ms-bit                          \ nb2 dom msb
    #2 pick mask-new                \ nb2 dom mask
    over _domain-set-ms-bit-mask    \ nb2 dom

cr ." domain-new: todo read cs store? change xts to set cur-store?" cr
    \ Set arbitrary current state.
    0 rot state-new                 \ dom sta
    over                            \ dom sta dom
    _domain-set-current-state       \ dom
;

\ Print a domain.
: .domain ( dom0 -- )
    \ Check arg.
    assert( tos is-domain? )

    dup domain-get-inst-id
    cr cr ." Dom: " dec.

    dup domain-get-num-bits ." num-bits: " dec. space
    dup domain-get-actions
    list-get-length
    ."  num actions: " dec.
    dup domain-get-current-state ." cur: " .state
    cr
    domain-get-actions .action-list
;

\ Deallocate a domain.
: domain-deallocate ( dom0 -- )
    \ Check arg.
    assert( tos is-domain? )

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup domain-get-actions action-list-deallocate
        dup domain-get-current-state state-deallocate
        dup domain-get-max-region region-deallocate
        dup domain-get-all-bits-mask mask-deallocate
        dup domain-get-ms-bit-mask mask-deallocate

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: domain-add-action ( xt1 dom0 -- )
    \ Check args.
    assert( tos is-domain? )

    tuck                        \ dom0 xt1 dom0

    action-new                  \ dom0 actx

    swap                        \ actx dom0
    2dup                        \ actx dom0 actx dom0
    domain-get-actions          \ actx dom0 actx act-lst
    action-list-push-end        \ actx dom0
;

\ Get a sample from an action in a domain.
\ Call only from session-get-sample, since current-domain in set there.
: domain-get-sample ( act1 dom0 -- smpl )
     \ Check args.
    assert( tos is-domain? )
    assert( nos is-action? )

    \ Get action sample.
    dup domain-get-current-state    \ act1 dom0 | d-sta
    #2 pick                         \ act1 dom0 | d-sta act1
    action-get-sample               \ act1 dom0 | smpl

    \ Set domain current state.
    dup sample-get-result           \ act1 dom0 | smpl sta
    #2 pick                         \ act1 dom0 | smpl sta dom
    _domain-set-current-state       \ act1 dom0 | smpl

\    cr
\    over domain-get-inst-id cr ." Dom: " #3 dec.r   \ act1 dom0 | smpl
\    space #2 pick action-get-inst-id ." Act: " #3 dec.r   \ smpl
\    space dup .sample
\    cr

    nip nip                         \ smpl
;

' domain-get-sample to domain-get-sample-xt

\ Return a action, given a action ID.
: domain-find-action ( u1 dom0 -- act t | f )
    \ cr ." domain-find-action: Dom: " dup domain-get-inst-id . space over . cr
    \ Check args.
    assert( tos is-domain? )
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
    nip
    true
;

: domain-get-number-actions ( dom -- na )
    \ Check arg.
    assert( tos is-domain? )

    domain-get-actions      \ act-lst
    list-get-length         \ len
;

' domain-get-number-actions to domain-get-number-actions-xt

