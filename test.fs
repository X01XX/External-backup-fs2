1 assert-level !    \ 0 to turn most asserts off, 1 to turn them on.

' dup alias tos
' over alias nos
: 3os #2 pick ;
: 4os #3 pick ;

: tos-is-1 1 = if true else cr ." tos is not 1" cr abort then ;

: x
    assert( tos-is-1  )
  cr ." body" cr
;
