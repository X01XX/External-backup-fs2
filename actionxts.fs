\ Functions to execute to get the result from a given state.

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

: dom-0-act5-get-result ( current-state -- result )
    \ Check arg.
    assert( tos is-state? )

    dup state-get-number        \ csta cnum
    dup invert #10 and #10 =    \ csta cnum bool
    if                          \ 0X0X
        #1 xor                  \ csta rnum
    else                        \ 1XXX, XX1X
        #4 xor                  \ csta rnum
    then

    swap                        \ rnum csta
    state-get-num-bits          \ rnum nb
    state-new                   \ rsta
;


