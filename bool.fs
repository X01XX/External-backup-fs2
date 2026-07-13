' 0= alias false?

\ Check TOS for bool.
: is-bool? ( tos -- bool )
    dup 0=          \ tos bool
    swap -1 =       \ bool bool
    or              \ bool
;

: .bool ( b -- )
    \ Check arg.
    assert( tos is-bool? )

    if
        ." t"
    else
        ." f"
    then
;
