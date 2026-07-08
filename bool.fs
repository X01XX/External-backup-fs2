' 0= alias false?

\ Check TOS for bool, unconventional, leaves stack unchanged.
: is-bool? ( tos -- t )
    dup 0=
    over -1 =
    or
    if
        drop
        true
    else
        s" TOS is not bool"
        .abort-xt execute
    then
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
