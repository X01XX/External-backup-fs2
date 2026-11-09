\ The token struct, storing a token of up to 15 characters.
#59797 constant token-struct-id
   #21 constant token-struct-number-cells

token-struct-number-cells 1 - #8 * value token-number-bytes

\ Token struct fields.
0                       constant token-header-disp   \ 16 bits, [0] id, [1] use count.
token-header-disp cell+ constant token-string-disp

0 value token-mma       \ Storage for the token mma instance addr.

\ Init token mma.
: token-mma-init ( num-items -- ) \ sets token-mma.
    dup 1 <
    if
        ." token-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Token store."
    token-struct-number-cells swap mma-new to token-mma
;

\ Check if tos is an allocated token.
: is-token? ( tos -- bool )
    dup token-mma mma-is-item? \ addr bool
    if
        struct-get-id           \ id
        token-struct-id =       \ bool
    else
        drop
        false                   \ f
    then
;

' is-token? to is-token?-xt

: sb-token? ( tos -- bool )
    dup token-mma mma-within-array  \ tkn0 bool
    if
        dup struct-get-id           \ tkn0 id
        ?dup 0=
        if
            cr ." sb-token?: Use after deallocation " hex. cr
            false
            exit
        then

        dup token-struct-id =       \ tkn0 id bool
        if
            2drop
            true
        else
            cr ." sb-token?: Instance id invalid? " hex. space hex. cr
            false
        then
    else                            \ tkn0
        cr ." sb-token?: Invalid address? " hex. cr
        false
    then
;

\ Start accessors.

\ Get token data cell.
: token-get-string ( tkn -- c-addr u )
    \ Check arg.
    assert( tos sb-token? )

    token-string-disp + string@
;

\ Copy a token string to the pad and leave string on stack.
: token-string-to-pad ( tkn -- c-addr u )
    \ Check arg.
    assert( tos sb-token? )

    token-get-string        \ c-addr u
    pad string!             \
    pad string@             \ c-addr u
;

\ Set token data cell.
: token-set-string ( c-addr u tkn -- )
    \ Check args.
    assert( tos sb-token? )

    token-string-disp + string!
;

\ End accessors.

\ Trim spaces from the left side of a string.
: ltrim ( c-addr u -- c-addr' u' )
    begin
        dup
    while
        over c@ #32 =
        if
            swap 1+ swap
            1-
        else
            exit
        then
    repeat
;

\ Trim spaces from the right side of a string.
: rtrim ( c-addr u -- c-addr u' )
    begin
        dup
    while
        2dup + 1- c@ #32 =
        if
            1-
        else
            exit
        then
    repeat
;

\ Return a new token struct instance address, with given data value.
\ An empty string, or a string of spaces, will return false.
: token-new ( c-addr u -- tkn t | f )
    \ Check args.
    assert( tos #256 < )
    assert( tos -1 > )

    dup token-number-bytes <
    ifnot
        2drop
        false
        exit
    then
    over 0 <
    if
        2drop
        false
        exit
    then

    token-struct-id token-mma
    struct-allocate             \ c-addr u tkn

    \ Store string.
    -rot                        \ tkn c-addr u
    #2 pick                     \ tkn c-addr u tkn
    token-set-string            \ tkn
    true
;

\ Print a token struct.
: .token ( tkn -- )
    \ Check arg.
    assert( tos sb-token? )

    token-get-string type
;

\ Print a token struct instance.
: .token-with-parens ( tkn -- )
    \ Check arg.
    assert( tos sb-token? )

    [char] " emit
    token-get-string type
    [char] " emit
;

\ Return true if two tokens are equal.
: tokens-eq? ( tkn1 tkn2 -- flag )
    \ Check args.
    assert( tos sb-token? )
    assert( nos sb-token? )

    token-get-string        \ tkn1 c-addr2 u2
    rot                     \ c-addr2 u2 tkn1
    token-get-string        \ c-addr2 u2 c-addr1 u1
    str=                    \ result
;

\ Return true if a token is equal to a string.
: token-eq-string ( c-addr u tkn0 -- flag )
    \ Check args.
    assert( tos sb-token? )

    token-get-string        \ c-addr u c-addr u
    str=                    \ flag
;

\ Deallocate a token.
: token-deallocate ( tkn0 -- )
    \ Check if tos is a valid instance.
    dup token-mma mma-within-array  \ tkn0 bool
    ifnot
        cr ." token-deallocate: Deallocation of an invalid address? " dup hex. cr abort
    then

    \ Check if instance is already deallocated.
    dup get-first-word              \ tkn0, id t | f
    invert abort" token-deallocate: get-first-word failed?"

    0=                              \ tkn0 bool
    if
        cr ." token-deallocate: Double deallocation of " dup hex. cr abort
    then

    \ Check instance use count.
    dup struct-get-use-count        \ tkn count
    dup 0<
    if
        cr ." token-deallocate: Invalid use count" cr abort
    then

    #2 <
    if
        \ Deallocate instance.
        token-mma mma-deallocate2
    else
        struct-dec-use-count
    then
;

\ Concatenate an a string to the end of a token's existing string.
: token-concatenate ( c-addr n tkn0 -- t | f )
    \ Check arg.
    assert( tos sb-token? )
    assert( nos #256 < )
    assert( nos -1 > )

    \ Get address of token string.
    token-string-disp + \ c-addr n c-addr

    \ Check string lengths added up are within bounds.
    dup c@                  \ c-addr n c-addr cur-len
    #2 pick +               \ c-addr n c-addr nxt-len
    token-number-bytes <    \ c-addr n c-addr bool
    ifnot
        2drop drop
        false
        exit
    then
                            \ c-addr n c-addr
    +place
    true
;
