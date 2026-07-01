\ Functions ag]ctions can exeute to test the result of a given state.

\ Action x
: action-x-get-sample ( sta -- smpl )
    \ Check arg.
    assert-tos-is-state

    dup state-get-num-bits      \ sta nb
    over state-get-number       \ sta nb num

    \ Test for ...0XX0, change bit 3.
    #9 over and                 \ sta nb num test1
    0=
    if
        #8 or                   \ sta nb rslt1
        swap state-new          \ sta sta2
        swap sample-new         \ smpl
        exit
    then

    \ Test for ...1XX1, change bit 3.
    #9 over and                 \ sta nb num test1
    #9 =
    if
        #8 xor                  \ sta nb rslt1
        swap state-new          \ sta sta2
        swap sample-new         \ smpl
        exit
    then

    \ Then its in 0XX1 or 1X0X, no change.
                                \ sta nb num
    2drop                       \ sta
    dup sample-new              \ smpl
;

