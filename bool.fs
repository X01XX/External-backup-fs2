' 0= alias false?

\ Check TOS for bool, unconventional, leaves stack unchanged.
: assert-tos-is-bool ( tos -- tos )
    dup 0=
    over -1 =
    or
    false? if
        s" TOS is not bool"
        .abort-xt execute
    then
;

\ Check NOS for bool, unconventional, leaves stack unchanged.
: assert-nos-is-bool ( nos tos -- nos tos )
    over dup 0=
    swap -1 =
    or
    false? if
        s" NOS is not bool"
        .abort-xt execute
    then
;

: .bool ( b -- )
    \ Check arg.
    assert-tos-is-bool

    false?
    if
        ." f"
    else
        ." t"
    then
;
