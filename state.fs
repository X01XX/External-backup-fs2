\ The state struct, storing a state number.
#61717 constant state-id
    #2 constant state-struct-number-cells

\ State struct fields.
0                         constant state-header-disp   \ 16 bits, [0] id, [1] use count [2] Number bits ( 8 bits ).
state-header-disp cell+   constant state-number-disp

0 value state-mma \ Storage for the state mma instance addr.

\ Init state mma.
: state-mma-init ( num-items -- ) \ sets state-mma.
    dup 1 <
    if
        ." state-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing State store."
    state-struct-number-cells swap mma-new to state-mma
;

\ Check instance type.
: is-allocated-state ( tos -- flag )
    dup state-mma mma-is-item  \ addr bool
    if
        struct-get-id
        state-id =              \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for state, unconventional, leaves stack unchanged.
: assert-tos-is-state ( tos -- tos )
    dup is-allocated-state
    if exit then

    s" TOS is not an allocated state"
    .abort-xt execute
;

\ Check NOS for state, unconventional, leaves stack unchanged.
: assert-nos-is-state ( nos tos -- nos tos )
    over is-allocated-state
    if exit then

    s" NOS is not an allocated state"
    .abort-xt execute
;

\ Start accessors.

\ Get the number of bits.
: state-get-num-bits ( sta0 -- nb )
    \ Check arg.
    assert-tos-is-state

    4c@
;

\ Set the number of bits.
: _state-set-num-bits ( nb sta0 -- )
    4c!
;

\ Get state number.
: state-get-number ( sta0 -- lst0 )
    state-number-disp + @
;

\ Set state number.
: _state-set-number ( lst1 sta0 -- )
    state-number-disp + !
;

\ Return a new state struct instance address, with given data list and number bits.
: state-new ( num1 nb0 -- sta )
    \ Check args.

    \ Check number bits.
    dup 1 < abort" Number bits < 1?"
    dup cell-bits > abort" Number bits too large"

    \ Check number.
    2dup                    \ num1 nb0 num1 nb0
    _max-num-from-num-bits  \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a state instance.
    state-id state-mma      \ num1 nb0 id mma
    struct-allocate         \ num1 nb0 sta

    \ Set number bits.
    tuck                    \ num1 sta nb0 sta
    _state-set-num-bits  \ num1 sta

    \ Store number given.
    tuck                     \ sta num1 sta
    _state-set-number        \ sta
;

\ Store a character representation to a given address,
\ return the number characters stored.
\ Caller must set, or accumulate, the number bits prefix
\ of the string.
: state-str (  addr1 sta0 -- nc )
    \ Check arg.
    assert-tos-is-state

    \ Save string length.
    dup state-get-num-bits   \ addr1 sta0 nc
    -rot                        \ nc addr1 sta0

    \ Store prefix.
    [char] s #2 pick c!
    swap 1+ swap

    \ Setup for bit-position loop.
    dup state-get-number        \ nc addr1 sta0 num
    swap                        \ nc addr1 num sta0
    state-get-num-bits       \ nc addr1 num nc
    tuck                        \ nc addr1 nc num nc
    _msb-from-num-bits          \ nc addr1 nc num ms-bit
    rot                         \ nc addr1 num ms-bit nc
    0

    do
        \ Apply msb to state, to get an isolated bit.
                                \ nc addr1 num ms-bit
        2dup and                \ nc addr1 num ms-bit bit

        if
            [char] 1            \ nc addr1 num ms-bit chr
        else
            [char] 0            \ nc addr1 num ms-bit chr
        then

        \ Store char to pad.
        #3 pick                 \ nc addr1 num ms-bit chr pad+
        c!                      \ nc addr1 num ms-bit

        \ Point to next nc addr1 char.
        rot 1+ -rot             \ nc addr1 num ms-bit

        \ Adjust msb state.
        1 rshift                \ nc addr1 num ms-bit
    loop
    2drop drop                  \ nc
    1+
;

\ Print a state struct instance.
: .state ( sta0 -- )
\ Check arg.
    assert-tos-is-state

    \ Calc string target address.
    pad 1+ swap         \ pad+ sta0

    \ Put state string into pad.
    state-str           \ nc

    \ Set string length.
    pad c!              \

    \ Move pad string to stack.
    pad string@         \ c-addr u

    \ Output string.
    type
;

\ Deallocate a state.
: state-deallocate ( sta -- )
    \ Check arg.
    assert-tos-is-state

    dup struct-get-use-count    \ sta count

    dup 0< abort" state-deallocate: Invalid use count"

    #2 <
    if
        state-mma mma-deallocate \ Deallocate instance.
    else
        struct-dec-use-count
    then
;

\ Return true if two states have a different number of bits.
: states-dif-num-bits? ( sta1 sta0 -- flag )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state

    state-get-num-bits   \ sta1 nb0
    swap                 \ nb0 sta1
    state-get-num-bits   \ nb0 nb1
    <>
;

\ Return a state inverted, as a mask.
: state-invert-to-mask ( sta0 -- mask )
    \ Check arg.
    assert-tos-is-state

    dup                         \ sta0 sta0
    state-get-num-bits       \ sta0 nb
    _max-num-from-num-bits      \ sta0 max

    over                        \ sta0 max sta0
    state-get-number            \ sta0 max num

    xor                         \ sta0 invert

    swap                        \ invert sta0
    state-get-num-bits       \ invert nb
    mask-new                    \ msk
;

\ Return the Boolean AND of two states, as a mask.
: state-and-state-to-mask ( sta1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states do not have the same number of bits?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    and                     \ sta1 num
    swap                    \ num sta1
    state-get-num-bits   \ num nb
    mask-new                \ msk
;

\ Return the Boolean XOR of two states, as a mask.
: state-xor-to-mask ( sta1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states do not have the same number of bits?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    xor                     \ sta1 num
    swap                    \ num sta1
    state-get-num-bits   \ num nb
    mask-new                \ msk
;

\ Return the Boolean OR of two states, as a state
: state-or ( sta1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states do not have the same number of bits?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    or                      \ sta1 num
    swap                    \ num sta1
    state-get-num-bits   \ num nb
    state-new               \ msk
;

\ Return the Boolean OR of two states, as a state.
: state-or-mask ( msk1 sta0 -- sta )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask
    over mask-get-num-bits
    over state-get-num-bits
    <> abort" state and mask do not have the same number of bits?"

    over mask-get-number    \ msk1 sta0 num1
    swap state-get-number   \ msk1 num1 num0
    or                      \ msk1 num
    swap                    \ num msk1
    mask-get-num-bits    \ num nb
    state-new               \ sta
;

\ Return the Boolean AND of two states, as a state.
: state-and ( sta1 sta0 -- sta )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states do not have the same number of bits?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    and                     \ sta1 num
    swap                    \ num sta1
    state-get-num-bits   \ num nb
    state-new               \ sta
;

\ Return the Boolean AND of two states, as a state.
: state-and-mask ( msk1 sta0 -- sta )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask
    over mask-get-num-bits
    over state-get-num-bits
    <> abort" state and mask do not have the same number of bits?"

    over mask-get-number    \ msk1 sta0 num1
    swap state-get-number   \ msk1 num1 num0
    and                     \ msk1 num
    swap                    \ num msk1
    mask-get-num-bits    \ num nb
    state-new               \ sta
;

\ Return the Boolean and of two states, as a mask.
: state-and-mask-to-mask ( msk1 sta0 -- mask )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask
    over mask-get-num-bits
    over state-get-num-bits
    <> abort" state and mask do not have the same number of bits?"

    over mask-get-number    \ msk1 sta0 num1
    swap state-get-number   \ msk1 num1 num0
    and                     \ msk1 num
    swap                    \ num msk1
    mask-get-num-bits    \ num nb
    mask-new                \ msk
;

\ Return the state of a given bit number.
: state-bit ( u1 sta0 -- bit )
    \ Check arg.
    assert-tos-is-state

    over                \ u1 sta0 u1
    0< abort" Invalid bit number?"
    2dup                \ u1 sta0 u1 sta0
    state-get-num-bits
    > abort" Invalid bit number?"

    state-get-number    \ u1 num
    swap                \ num u1
    1 swap              \ num 1 u1
    lshift              \ num msk
    and                 \ bit ( could be 0, 1, 2, 4, 8 etc. )
    if
        1
    else
        0
    then
;

\ Return false if a string is not a representation of a state.
\
\ Otherwise, generate a state from the string.
\ Valid chars are 0, 1, and underscore as separator.
\ All bit positions must be specified.
\ Like s" s1010" state-from-string
: state-from-string ( c-addr u --  reg t | f)

    \ Check length GT 1.
    dup #2 <
    if
        2drop
        false
        exit
    then

    \ Check for prefix.
    over c@ [char] s <>
    if
        2drop
        false
        exit
    then

    \ Inc address.
    swap 1+ swap

    \ Dec len.
    1-

    \ Init character counter.
    0 swap              \ c-addr cnt u

    \ Init number
    0 swap              \ c-addr cnt num u
    0                   \ c-addr cnt num u 0

    \ For each character...
    do                  \ c-addr cnt num num0
        \ Get a character.
        #2 pick         \ c-addr cnt num c-addr
        i +             \ c-addr cnt num c-addr+
        c@              \ c-addr cnt num chr

        \ Process character.
        case
            [char] 0 of
                        \ Update num
                        1 lshift
                        \ Update char counter.
                        swap 1+ swap
                    endof
            [char] 1 of
                        \ Update num
                        1 lshift 1+
                        \ Update char counter.
                        swap 1+ swap
                    endof
            [char] _ of
                    endof
            \ Unrecognized character, return false.

            \ Drop stack items.
            2drop
            2drop

            \ Set return bool.
            false

            \ Cancel do loop.
            unloop

            \ Return.
            exit
        endcase
    loop

    \ Create state.         \ c-addr cnt num
    swap                    \ c-addr num cnt
    state-new               \ c-addr msk

    nip                     \ msk
    true
;

\ Return a state from a string, or abart.
: state-from-string-a ( c-addr u -- sta )
    state-from-string   \ sta t | f
    invert abort" Invalid state string"
;

\ Return true if two states are equal.
: states-eq? ( sta1 sta0 -- bool )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states do not have the same number of bits?"

    state-get-number        \ sta1 num0
    swap state-get-number   \ num0 num1
    =
;
