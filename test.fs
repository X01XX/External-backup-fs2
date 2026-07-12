
\ : endit POSTPONE then ; immediate
\ : else2 POSTPONE else ; immediate
: ifnot POSTPONE 0= POSTPONE if ; immediate

: x ifnot ." false" else ." true" then ;

