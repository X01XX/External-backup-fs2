\ Functions for incompatible pairs, expressed as regions lists.

\ Of incompatible, non-adjacent, pairs, find pairs that will contribute
\ to corners likely in regions with the greatest number of edges.
: inc-pairs-priority-non-adjacent-needs ( reg-lst0 -- ned-lst )
    \ Check arg.
    assert( tos is-region-list? )

    \ Make a list of all non-adjacent pairs, and all adjacent pairs.
    list-new swap                           \ adj-prs' reg-lst0
    list-new swap                           \ adj-prs' nadj-prs' reg-lst0

    foreach                                 \ adj-prs' nadj-prs' reg-lnk
        dup link-get-data                   \ adj-prs' nadj-prs' reg-lnk regx
        dup region-states-adjacent?         \ adj-prs' nadj-prs' reg-lnk regx bool
        if
            #3 pick                         \ adj-prs' nadj-prs' reg-lnk regx adj-prs'
        else
            #2 pick                         \ adj-prs' nadj-prs' reg-lnk regx nadj-prs'
        then
        list-push-struct                    \ adj-prs' nadj-prs' reg-lnk
    next
                                            \ adj-prs' nadj-prs'
    \ Get states in non-adjacent pairs.
    dup region-list-states                  \ adj-prs' nadj-prs' nadj-stas'

    \ Get maximum number of connections a non-adjacent state may be part ef.

    \ Init maximum connections-per-state value.
    0 over                                  \ adj-prs' nadj-prs' nadj-stas' max nadj-stas'

    foreach                                 \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk

        \ Get number of occurences in the non-adjacent list.
        dup link-get-data                   \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk stax
        #4 pick                             \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk stax nadj-stas'
        region-list-num-state-in            \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk nadj-num-in

        \ Get number of occurences in the adjacent list.
        over link-get-data                  \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk nadj-num-in stax
        #6 pick                             \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk nadj-num-in stax adj-prs'
        region-list-num-state-in            \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk nadj-num-in adj-num-in

        \ Add priority for some pairs in the adjacent list.
        20 *                                \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk nadj-num-in adj-num-in

        \ Add adjacent and non-adjacent values.
        +                                   \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk num-in

        \ Update max value.
        rot                                 \ adj-prs' nadj-prs' nadj-stas' nadj-stas-lnk num-in max
        max                                 \ adj-prs' nadj-prs' nadj-stas' nadj-stas-lnk max
        swap                                \ adj-prs' nadj-prs' nadj-stas' max nadj-stas-lnk
    next
                                            \ adj-prs' nadj-prs' nadj-stas' max

    cr ." max connections of any state: " dup . cr

    \ Get states that are at the maximum value. One is a possible maximum value.

    \ Init priority state list.
    list-new                                \ adj-prs' nadj-prs' nadj-stas' max pri-stas'

    #2 pick                                 \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas'
    foreach                                 \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-sna-lnk
        \ Get number of occurences in the non-adjacent list.
        dup link-get-data                   \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk stax
        #5 pick                             \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk stax nadj-prs'
        region-list-num-state-in            \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in

        \ Get number of occurences in the adjacent list.
        over link-get-data                  \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in stax
        #7 pick                             \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in stax adj-prs'
        region-list-num-state-in            \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in adj-num-in

        \ Add priority for some pairs in the adjacent list.
        20 *                                \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in adj-num-in

        \ Add adjacent and non-adjacent values.
        +                                   \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk num-in

        \ Check max value.
        #3 pick                             \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk num-in max
        =                                   \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk bool
        if
            \ Add state to priority states list.
            dup link-get-data               \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk stax
            #2 pick                         \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk stax pri-stas'
            list-push-struct                \ adj-prs' nadj-prs' nadj-stas' max pri-stas' nadj-stas-lnk
        then
    next
                                            \ adj-prs' nadj-prs' nadj-stas' max pri-stas'
    nip                                     \ adj-prs' nadj-prs' nadj-stas' pri-stas'

    \ Get priority regions.
    dup                                     \ adj-prs' nadj-prs' nadj-stas' pri-stas' pri-stas'
    #3 pick                                 \ adj-prs' nadj-prs' nadj-stas' pri-stas' pri-stas' nadj-prs'
    region-list-states-in                   \ adj-prs' nadj-prs' nadj-stas' pri-stas' pri-regs'

    cr ." priority pairs: " dup .region-list cr

    \ Clean up.
    swap state-list-deallocate              \ adj-prs' nadj-prs' nadj-stas' pri-regs'
    swap state-list-deallocate              \ adj-prs' nadj-prs' pri-regs'
    swap region-list-deallocate             \ adj-prs' pri-regs'
    swap region-list-deallocate             \ pri-regs'

    region-list-deallocate
;
