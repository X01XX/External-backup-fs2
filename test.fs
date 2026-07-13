
\ : endit POSTPONE then ; immediate
\ : else2 POSTPONE else ; immediate
: ifnot POSTPONE 0= POSTPONE if ; immediate

: x ifnot ." false" else ." true" then ;

: list-get-links . ;
: link-get-next . ;

: foreach ( list -- ) postpone list-get-links postpone begin postpone ?dup postpone while ; immediate
: next postpone link-get-next postpone repeat ; immediate

: y foreach dup . dup next ;

