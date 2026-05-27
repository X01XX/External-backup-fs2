\ The mask struct, storing a mask number.
#61719 constant mask-id
    #2 constant mask-struct-number-cells

\ mask struct fields.
0                       constant mask-header-disp   \ 16 bits, [0] id, [1] use count [2] Number bits ( 8 bits ).
mask-header-disp cell+  constant mask-number-disp

0 value mask-mma    \ Storage for the mask mma instance addr.

1 cells #8 * value cell-bits

\ Init mask mma.
: mask-mma-init ( num-items -- )    \ sets mask-mma.
    dup 1 <
    if
        ." mask-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Mask store."
    mask-struct-number-cells swap mma-new to mask-mma
;

\ Check instance type.
: is-allocated-mask ( tos -- bool )
    dup mask-mma mma-is-item    \ addr bool
    if
        struct-get-id
        mask-id =               \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for mask, unconventional, leaves stack unchanged.
: assert-tos-is-mask ( tos -- tos )
    dup is-allocated-mask
    if exit then

    s" TOS is not an allocated mask"
    .abort-xt execute
;

\ Check NOS for mask, unconventional, leaves stack unchanged.
: assert-nos-is-mask ( nos tos -- nos tos )
    over is-allocated-mask
    if exit then

    s" NOS is not an allocated mask"
    .abort-xt execute
;

\ Start accessors.

\ Get the number of bits.
: mask-get-num-bits ( msk0 -- nb )
    \ Check arg.
    assert-tos-is-mask

    4c@
;

\ Set the number of bits.
: _mask-set-num-bits ( nb msk0 -- )
    4c!
;

\ Get mask number.
: mask-get-number ( msk0 -- lst0 )
    mask-number-disp + @
;

\ Set mask number.
: _mask-set-number ( lst1 msk0 -- )
    mask-number-disp + !
;

\ Return the msb for a given number of bits.
\ e.g. 8 for 4 bits.
: _msb-from-num-bits ( nb0 -- msb )
    1 swap
    1-
    lshift
 ;

\ Return the maximum number for a given number of bits.
\ e.g. 15 for 4 bits.
: _max-num-from-num-bits ( nb0 -- num )
    _msb-from-num-bits
    1-
    1 lshift
    1+
;

\ Return a new mask struct instance address, with given data list and number bits.
: mask-new ( num1 nb0 -- msk )
    \ Check args.

    \ Check number bits.
    dup 1 < abort" Number bits < 1?"
    dup cell-bits > abort" Number bits too large"

    \ Check number.
    2dup                    \ num1 nb0 num1 nb0
    _max-num-from-num-bits  \ num1 nb0 num1 max
    u> abort" Number too large for number bits given"

    \ Allocate a mask instance.
    mask-id mask-mma        \ num1 nb0 id mma
    struct-allocate         \ num1 nb0 msk

    \ Set number bits.
    tuck                    \ num1 msk nb0 msk
    _mask-set-num-bits   \ num1 msk

    \ Store number given.
    tuck                    \ msk num1 msk
    _mask-set-number        \ msk
;

\ Return a copy of a mask.
: mask-copy ( msk0 -- msk )
    \ Check arg.
    assert-tos-is-mask

    dup mask-get-number         \ msk0 num1
    swap mask-get-num-bits   \ num1 nb
    mask-new
;

\ Return mask remainder and mask lsb, if mask is non-zero.
: mask-split-lsb ( msk0 -- msk-rem msk-lsb t | f )
    \ Check arg.
    assert-tos-is-mask

    \ Get mask remainder and lsb.
    dup mask-get-number         \ msk0 num
    split-lsb                   \ msk0, rem lsb t | f
    if
        rot                     \ rem lsb msk0
        mask-get-num-bits    \ rem lsb nb
        tuck                    \ rem nb lsb nb
        mask-new                \ rem nb msk-lsb
        -rot                    \ msk-lsb rem nb
        mask-new                \ msk-lsb msk-rem
        swap                    \ msk-rem msk-lsb
        true
    else                        \ msk0
        drop
        false
    then
;

\ Store a character representation to a given address,
\ return the number characters stored.
\ Caller must set, or accumulate, the number bits prefix
\ of the string.
: mask-str ( addr1 msk0 --  nc )
    \ Check arg.
    assert-tos-is-mask

    \ Save string length.
    dup mask-get-num-bits    \ addr1 msk0 nc
    -rot                        \ nc addr1 msk0

    \ Store prefix.
    [char] m #2 pick c!
    swap 1+ swap

    \ Setup for bit-position loop.
    dup mask-get-number         \ nc addr1 msk0 num
    swap                        \ nc addr1 num msk0
    mask-get-num-bits        \ nc addr1 num nc
    tuck                        \ nc addr1 nc num nc
    _msb-from-num-bits          \ nc addr1 nc num ms-bit
    rot                         \ nc addr1 num ms-bit nc
    0

    do
        \ Apply msb to mask, to get an isolated bit.
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

        \ Adjust msb mask.
        1 rshift                \ nc addr1 num ms-bit
    loop
    2drop drop                  \ nc
    1+
;

\ Print a mask.
: .mask ( msk0 -- )
    \ Check arg.
    assert-tos-is-mask

    \ Calc string target address.
    pad 1+ swap         \ addr msk0

    \ Put mask string into pad.
    mask-str            \ nc

    \ Store string length.
    pad c!              \

    \ Move pad string to stack.
    pad string@         \ c-addr u

    \ Output string.
    type
;

\ Deallocate a mask.
: mask-deallocate ( msk -- )
    \ Check arg.
    assert-tos-is-mask

    dup struct-get-use-count    \ msk count

    dup 0< abort" mask-deallocate: Invalid use count"

    #2 <
    if
        mask-mma mma-deallocate \ Deallocate instance.
    else
        struct-dec-use-count
    then
;

\ Return true if two masks have a different number of bits.
: masks-dif-num-bits? ( msk1 msk0 -- flag )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask

    mask-get-num-bits    \ msk1 nb0
    swap                 \ nb0 msk1
    mask-get-num-bits    \ nb0 nb1
    <>
;

\ Return true if two masks are equal.
: masks-eq? ( msk1 msk0 -- flag )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask
    2dup masks-dif-num-bits? abort" masks do not have the same number of bits?"

    mask-get-number     \ msk1 lst0
    swap                \ lst0 msk1
    mask-get-number     \ lst0 lst1

    =
;

\ Return a mask inverted.
: mask-invert ( msk0 -- msk )
    \ Check arg.
    assert-tos-is-mask

    dup                         \ msk0 msk0
    mask-get-num-bits           \ msk0 nb
    _max-num-from-num-bits      \ msk0 max

    over                        \ msk0 max msk0
    mask-get-number             \ msk0 max num

    xor                         \ msk0 invert

    swap                        \ invert msk0
    mask-get-num-bits           \ invert nb
    mask-new                    \ msk
;

\ Return the Boolean AND of two masks.
: mask-and ( msk1 msk0 -- msk )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask
    2dup masks-dif-num-bits? abort" masks do not have the same number of bits?"

    over mask-get-number   \ msk1 msk0 num1
    swap mask-get-number   \ msk1 num1 num0
    and                    \ msk1 num
    swap                   \ num msk1
    mask-get-num-bits   \ num nb
    mask-new               \ msk
;

\ Return the Boolean OR of two masks.
: mask-or ( msk1 msk0 -- msk )
    \ Check args.
    assert-tos-is-mask
    assert-nos-is-mask
    2dup masks-dif-num-bits? abort" masks do not have the same number of bits?"

    over mask-get-number   \ msk1 msk0 num1
    swap mask-get-number   \ msk1 num1 num0
    or                     \ msk1 num
    swap                   \ num msk1
    mask-get-num-bits   \ num nb
    mask-new               \ msk
;

\ Return the mask of a given bit number.
: mask-bit ( u1 msk0 -- bit )
    \ Check arg.
    assert-tos-is-mask

    over                \ u1 msk0 u1
    0< abort" Invalid bit number?"
    2dup                \ u1 msk0 u1 msk0
    mask-get-num-bits
    > abort" Invalid bit number?"

    mask-get-number     \ u1 num
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

\ Return false if a string is not a representation of a mask.
\
\ Otherwise, generate a mask from the string.
\ Valid chars are 0, 1, and underscore as separator.
\ All bit positions must be specified.
\ Like s" m1010" mask-from-string
: mask-from-string ( c-addr u --  reg t | f)

    \ Check length GT 1.
    dup #2 <
    if
        2drop
        false
        exit
    then

    \ Check for prefix.
    over c@ [char] m <>
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

    \ Create mask.          \ c-addr cnt num
    swap                    \ c-addr num cnt
    mask-new                \ c-addr msk

    nip                     \ sta
    true
;

\ Return true if mask is zero.
: mask-is-zero? ( msk0 -- bool )
    \ Check arg.
    assert-tos-is-mask

    mask-get-number     \ num
    0=                  \ bool
;

\ Shift a mask 1 bit to the left.
: mask-lshift-1 ( msk0 -- )
    \ Check arg.
    assert-tos-is-mask

    \ Shift 1.
    dup mask-get-number     \ msk0 num
    1 lshift                \ msk0 num<

    \ Mask off excess bits, if any.
    over mask-get-num-bits  \ msk0 num< nb
    _max-num-from-num-bits  \ msk0 num< max
    and                     \ msk0 num<

    \ Finish.
    swap _mask-set-number   \
;

\ Add one to a mask.
: mask-add-1 ( msk0 -- )
    \ Check arg.
    assert-tos-is-mask

    dup mask-get-number     \ msk0 num
    1+                      \ msk0 num+

    \ Mask off excess bits, if any.
    over mask-get-num-bits  \ msk0 num+ nb
    _max-num-from-num-bits  \ msk0 num+ max
    and                     \ msk0 num+

    \ Finish.
    swap _mask-set-number   \
;
