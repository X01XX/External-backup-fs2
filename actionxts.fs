\ Functions to execute to get the result from a given state.

\ Result x
: calc-result-x ( sta -- rslt )
    \ Check arg.
    assert-tos-is-state

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

