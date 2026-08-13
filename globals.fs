
\ A store for the current session.
0 value session-store

: .stack-gbl
    .stack-structs-xt execute
;

: display-debug-status
    assert-level @ if
        cr ." debug on"
    else
        cr ." debug off"
    then
;

1 assert-level !    \ 0 to turn most asserts off, 1 to turn them on.
display-debug-status

' dup alias tos
' over alias nos

: 3os ( 3os nos tos -- 3os nos tos 3os )
    #2 pick
;

: 4os ( 4os 3os nos tos -- 4os 3os nos tos 4os )
    #3 pick
;

: 5os ( 5os 4os 3os nos tos -- 5os 4os 3os nos tos 5os )
    #4 pick
;

: is-valid-inst-id? ( n -- bool )
    dup 0 >= swap #256 < and
;

: is-valid-num-bits? ( n -- bool )
    dup 0 > swap [ 1 cells #8 * ] literal <= and
;
