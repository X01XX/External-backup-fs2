\ The state struct, storing a state number.
#61717 constant state-struct-id
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
: is-allocated-state? ( tos -- flag )
    dup state-mma mma-is-item  \ addr bool
    if
        struct-get-id
        state-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for state, unconventional, leaves stack unchanged.
: assert-tos-is-state ( tos -- tos )
    dup is-allocated-state?
    if exit then

    s" TOS is not an allocated state"
    .abort-xt execute
;

\ Check NOS for state, unconventional, leaves stack unchanged.
: assert-nos-is-state ( nos tos -- nos tos )
    over is-allocated-state?
    if exit then

    s" NOS is not an allocated state"
    .abort-xt execute
;

\ Check 3OS for state, unconventional, leaves stack unchanged.
: assert-3os-is-state ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-state?
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
    2dup                        \ num1 nb0 num1 nb0
    _max-num-from-num-bits      \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a state instance.
    state-struct-id state-mma   \ num1 nb0 id mma
    struct-allocate             \ num1 nb0 sta

    \ Set number bits.
    tuck                        \ num1 sta nb0 sta
    _state-set-num-bits         \ num1 sta

    \ Store number given.
    tuck                        \ sta num1 sta
    _state-set-number           \ sta
;

\ Print a state struct instance.
: .state (  sta0 -- )
    \ Check arg.
    assert-tos-is-state

    \ Print prefix.
    [char] s emit

    \ Setup for bit-position loop.
    dup state-get-number        \ sta0 num
    swap                        \ num sta0
    state-get-num-bits          \ num nb
    dup _msb-from-num-bits      \ num nb ms-bit
    swap                        \ num ms-bit nb
    0

    do
        \ Apply msb to state, to get an isolated bit.
                                \ num ms-bit
        2dup and                \ num ms-bit bit

        if
            [char] 1 emit
        else
            [char] 0 emit
        then

        \ Adjust msb state.
        1 rshift                \ num ms-bit
    loop
    2drop
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
    2dup states-dif-num-bits? abort" state-and-state-to-mask: num bits ne?"

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
    2dup states-dif-num-bits? abort" state-xor-to-mask: num bits ne?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    xor                     \ sta1 num
    swap                    \ num sta1
    state-get-num-bits   \ num nb
    mask-new                \ msk
;

\ Return the Boolean OR of two states, as a state
: state-or ( sta1 sta0 -- sta )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" state-or: num bits ne?"

    over state-get-number   \ sta1 sta0 num1
    swap state-get-number   \ sta1 num1 num0
    or                      \ sta1 num
    swap                    \ num sta1
    state-get-num-bits      \ num nb
    state-new               \ msk
;

\ Return the Boolean OR of two states, as a state.
: state-or-mask ( msk1 sta0 -- sta )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-mask
    over mask-get-num-bits
    over state-get-num-bits
    <> abort" state-or-mask: num bits ne?"

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
    2dup states-dif-num-bits? abort" state-and: num bits ne?"

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
    <> abort" state-and-mask: num bits ne?"

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
    <> abort" state-and-mask-to-mask: num bits ne?"

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
    2dup states-dif-num-bits? abort" states-eq?: num bits ne?"

    state-get-number        \ sta1 num0
    swap state-get-number   \ num0 num1
    =
;

\ Return true if two states are adjacent.
: states-adjacent? ( sta1 sta0 -- bool )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states-adjacent?: num bits ne?"

    state-get-number        \ sta1 num0
    swap state-get-number   \ num0 num1
    xor                     \ x
    only-one-bit-set?       \ bool
;

\ Return the copy of a state.
: state-copy ( sta0 -- sta )
    \ Check arg.
    assert-tos-is-state

    dup state-get-number    \ sta0 num
    swap state-get-num-bits \ num nb
    state-new
;

\ Return a different bit mask.
: state-dif-mask ( sta1 sta0 -- msk )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states-dif-mask: num bits ne?"

    \ Save number bits.
    dup state-get-num-bits -rot \ nb sta1 sta0

    \ Get state numbers.
    state-get-number swap       \ nb u0 sta1
    state-get-number            \ nb u0 u1

    \ Calc dif.
    xor                         \ nb dif

    \ Return mask.
    swap mask-new               \ msk
;

\ Return true if a state (tos) is between two other states.
: state-between? ( sta2 sta1 sta0 -- bool )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    assert-3os-is-state
    2dup states-dif-num-bits? abort" states-between?: num bits ne?"
    #2 pick over states-dif-num-bits? abort" states-between?: num bits ne?"

    \ Get sta0 dif masks.
    tuck                    \ sta2 sta0 sta1 sta0
    state-dif-mask          \ sta2 sta0 dif1
    -rot                    \ dif1 sta2 sta0
    state-dif-mask          \ dif1 dif2

    \ Check if dif masks have the same bit set.
    2dup mask-and           \ dif1 dif2 dif12
    dup mask-is-zero?       \ dif1 dif2 dif12 bool

    \ Clean up.
    swap mask-deallocate    \ dif1 dif2 bool
    swap mask-deallocate    \ dif1 bool
    swap mask-deallocate
;

: states-distance ( sta1 sta0 -- u )
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state
    2dup states-dif-num-bits? abort" states-distance: num bits ne?"

    state-dif-mask      \ msk'
    dup mask-count-bits \ msk' u
    swap mask-deallocate
;
