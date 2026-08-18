\ Functions for incompatible pairs, expressed as regions lists.

\ Of incompatible, non-adjacent, pairs, find pairs that will contribute
\ to corners likely in regions with the greatest number of edges.
: inc-pairs-priority-non-adjacent-pairs ( nadj-prs1 adj-prs0 -- pr-lst t | f )
    \ Check arg.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ Check if there are no non-adjacent pairs.
    over list-is-empty?                     \ nadj-prs1 adj-prs0 bool
    if
        2drop
        false
        exit
    then

    \ Check if there is only one pair.
    over list-get-length 1 =                \ nadj-prs1 adj-prs0 bool
    if
        nip                                 \ nadj-prs0
        list-copy-struct                    \ pr-lst
        true
        exit
    then

    \ Get states in non-adjacent pairs.
    over region-list-states                  \ nadj-prs adj-prs nadj-stas'

    \ Get maximum number of connections a non-adjacent state may be part ef.

    \ Init maximum connections-per-state value.
    0 over                                  \ nadj-prs adj-prs nadj-stas' max nadj-stas'

    foreach                                 \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk stax

        \ Get number of occurences in the non-adjacent list.
        #5 pick                             \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk stax nadj-stas'
        region-list-num-state-in            \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk nadj-num-in

        \ Get number of occurences in the adjacent list.
        over link-get-data                  \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk nadj-num-in stax
        #5 pick                             \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk nadj-num-in stax adj-prs'
        region-list-num-state-in            \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk nadj-num-in adj-num-in

        \ Add priority for some pairs in the adjacent list.
        #20 *                               \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk nadj-num-in adj-num-in

        \ Add adjacent and non-adjacent values.
        +                                   \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk num-in

        \ Update max value.
        rot                                 \ nadj-prs adj-prs nadj-stas' nadj-stas-lnk num-in max
        max                                 \ nadj-prs adj-prs nadj-stas' nadj-stas-lnk max
        swap                                \ nadj-prs adj-prs nadj-stas' max nadj-stas-lnk
    next
                                            \ nadj-prs adj-prs nadj-stas' max

    \ cr ." max connections rate of any state: " dup . cr

    \ Get states that are at the maximum value. One is a possible maximum value.

    \ Init priority state list.
    list-new                                \ nadj-prs adj-prs nadj-stas' max pri-stas'

    #2 pick                                 \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas'
    foreach                                 \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-sna-lnk stax
        \ Get number of occurences in the non-adjacent list.
        #6 pick                             \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk stax nadj-prs'
        region-list-num-state-in            \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in

        \ Get number of occurences in the adjacent list.
        over link-get-data                  \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in stax
        #6 pick                             \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in stax adj-prs'
        region-list-num-state-in            \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in adj-num-in

        \ Add priority for some pairs in the adjacent list.
        #20 *                               \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk nadj-num-in adj-num-in

        \ Add adjacent and non-adjacent values.
        +                                   \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk num-in

        \ Check max value.
        #3 pick                             \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk num-in max
        =                                   \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk bool
        if
            \ Add state to priority states list.
            dup link-get-data               \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk stax
            #2 pick                         \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk stax pri-stas'
            list-push-struct                \ nadj-prs adj-prs nadj-stas' max pri-stas' nadj-stas-lnk
        then
    next
                                            \ nadj-prs adj-prs nadj-stas' max pri-stas'
    nip                                     \ nadj-prs adj-prs nadj-stas' pri-stas'

    \ Get priority regions.
    dup                                     \ nadj-prs adj-prs nadj-stas' pri-stas' pri-stas'
    #4 pick                                 \ nadj-prs adj-prs nadj-stas' pri-stas' pri-stas' nadj-prs'
    region-list-states-in                   \ nadj-prs adj-prs nadj-stas' pri-stas' pri-regs'

    \ cr ." priority pairs: " dup .region-list cr

    \ Clean up.
    swap state-list-deallocate              \ nadj-prs adj-prs nadj-stas' pri-regs'
    swap state-list-deallocate              \ nadj-prs adj-prs pri-regs'
    nip nip                                 \ pri-regs'

    true
;
