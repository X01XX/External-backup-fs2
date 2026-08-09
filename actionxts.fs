\ Functions to execute to get the result from a given state.

\ Act zero for any domain, return the current state with no change.
: act-0-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    state-copy
;

: dom-0-act1-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-number        \ csta cnum
    #1 xor                      \ csta rnum
    swap                        \ rnum csta
    state-get-num-bits          \ rnum nb
    state-new                   \ rsta
;

: dom-0-act2-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-number        \ csta cnum
    #2 xor                      \ csta rnum
    swap                        \ rnum csta
    state-get-num-bits          \ rnum nb
    state-new                   \ rsta
;

: dom-0-act3-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-number        \ csta cnum
    #4 xor                      \ csta rnum
    swap                        \ rnum csta
    state-get-num-bits          \ rnum nb
    state-new                   \ rsta
;

: dom-0-act4-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-number        \ csta cnum
    #8 xor                      \ csta rnum
    swap                        \ rnum csta
    state-get-num-bits          \ rnum nb
    state-new                   \ rsta
;

\ Result x
: calc-result-x ( sta -- rslt )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-num-bits      \ sta nb
    swap state-get-number       \ nb num

    \ Test for ...0XX0, change bit 3.
    #9 over and                 \ nb num test1
    0=
    if
        #8 or                   \ nb rslt1
        swap state-new          \ rslt
        exit
    then

    \ Test for ...1XX1, change bit 3.
    #9 over and                 \ nb num test1
    #9 =
    if
        #8 xor                  \ nb rslt1
        swap state-new          \ rslt
        exit
    then

    \ Then its in 0XX1 or 1X0X, no change.
                                \ nb num
    state-new                   \ rslt
;

