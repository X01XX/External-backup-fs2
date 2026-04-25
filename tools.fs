\ Exponential function, exponent s/b GE zero.
\ If at the limit, 2 ^ 63, use "u." instead of "." to see the number.
: exp ( n u -- u2 ) \ u3 = u1^u2,
    dup 0<
    abort" exp: exponent is negative"

    1 swap 0
    ?do
        cr .s
        over *
        dup 0=
        abort" exp: overflow"
    loop
    nip \ Do exponentiation.
;

\ A shorter version of clearstack.
: cs ( -- ) \ Side effect: Data stack is cleared.
    clearstack
;

\ Set base to binary.
: binary  ( -- ) \ Side effect: Session base is changed.
    #2 base !
;

\ Return true if exactly one bit is set.
: one-bit-set ( n -- flag )
    dup 0>
    if
      dup 1 - and
      0=
    else              \ Not GT 0
      dup 0<
      if              \ LT 0
         1 lshift 0=  \ Check for only the MSBit being set.
      \ else zero drops through as the false flag.
      then
    then
;

\ Isolate LSB from a non-zero number.
\ Return changed number and a single-bit number.
: isolate-a-bit ( u1 -- u2 u3 )
    depth
    0= abort" isolate-a-bin: no argument on stack"

    dup 0=
    abort" isolate-a-bit: argument is zero"

    \ Remove lsb.
    dup 1- over and     \ u u-lsb

    \ Isolate lsb.
    tuck xor            \ u-lsb lsb
;

: 3drop ( x y z -- )
    2drop drop
;

\ Return true if a number is between two numbers.
: between-inclusive ( target num1 num2 -- flag )
    2dup < if   \ num1 LT num2
        #2 pick swap     \ target num1 target num2
        \ Check target vs num2.
        > if
            2drop false exit
          then
        \ Check target vs num1.
        < if
            false exit
          then
    else
        #2 pick swap     \ target num1 target num2
        \ Check target vs num2.
        < if
            2drop false exit
          then
        \ Check target vs num1.
        > if
            false exit
          then
    then
    true
;

\ Fetch the [0], 16 bits of a cell.
: 0w@ ( addr -- u )
    uw@
;

\ Store a number in the [0], 16 bits of a cell.
: 0w! ( u addr -- )
    w!
;

\ Fetch the [1], 16 bits of a cell.
: 1w@ ( addr -- u )
    #2 + uw@
;

\ Store a number in the [1], 16 bits of a cell.
: 1w! ( u addr -- )
    #2 + w!
;

\ Fetch the [2] 16 bits of a cell.
: 2w@ ( addr -- u )
    #4 + uw@
;

\ Store a number in the [2], 16 bits of a cell.
: 2w! ( u addr -- )
    #4 + w!
;

\ Fetch the [3] 16 bits of a cell.
: 3w@ ( addr -- u )
    #6 + uw@
;

\ Store a number in the [3], 16 bits of a cell.
: 3w! ( u addr -- )
    #6 + w!
;

\ Fetch the [0], 8 bits of a cell.
: 0c@ ( addr -- u )
    c@
;

\ Store a number in the [0], 8 bits of a cell.
: 0c! ( u addr -- )
    c!
;

\ Fetch the [1], 8 bits of a cell.
: 1c@ ( addr -- u )
    1+ c@
;

\ Store a number in the [1], 8 bits of a cell.
: 1c! ( u addr -- )
    1+ c!
;

\ Fetch the [2], 8 bits of a cell.
: 2c@ ( addr -- u )
    #2 + c@
;

\ Store a number in the [2], 8 bits of a cell.
: 2c! ( u addr -- )
    #2 + c!
;

\ Fetch the [3], 8 bits of a cell.
: 3c@ ( addr -- u )
    #3 + c@
;

\ Store a number in the [3], 8 bits of a cell.
: 3c! ( u addr -- )
    #3 + c!
;

\ Fetch the [4], 8 bits of a cell.
: 4c@ ( addr -- u )
    #4 + c@
;

\ Store a number in the [4], 8 bits of a cell.
: 4c! ( u addr -- )
    #4 + c!
;

\ Fetch the [5], 8 bits of a cell.
: 5c@ ( addr -- u )
    #5 + c@
;

\ Store a number in the [5], 8 bits of a cell.
: 5c! ( u addr -- )
    #5 + c!
;

\ Fetch the [6], 8 bits of a cell.
: 6c@ ( addr -- u )
    #6 + c@
;

\ Store a number in the [6], 8 bits of a cell.
: 6c! ( u addr -- )
    #6 + c!
;

\ Fetch the [7], 8 bits of a cell.
: 7c@ ( addr -- u )
    #7 + c@
;

\ Store a number in the [7], 8 bits of a cell.
: 7c! ( u addr -- )
    #7 + c!
;

\ Store a string on the stack to a given address.
: string! ( string-addr length target-addr -- )
    2dup c!         \ Store the length at target[0].
    1+              \ Point to target[1].
    swap cmove      \ Move characters to target[1].
;

\ Fetch a string from a given address, put on stack.
: string@ ( string-addr -- string-addr+1 length )
    dup c@          \ addr length
    swap 1+ swap    \ addr+1 length
;

\ Return most significant bit mask for a given number of bits.
: ms-bit ( nb0 -- u )
    1 swap              \ 1 nb0
    1-                  \ 1 nb0-
    lshift              \ msb
;

\ Return mask of all bits used, given number of bits.
: all-bits ( nb0 -- u )
    ms-bit
    1-
    1 lshift
    1 +
;

\ The reverse of 2rot.
: -2rot ( -- )
    2rot 2rot
;

\ Return a poor random number, from 0 to a given limit - 1.
\ Mostly used to change a non-empty list length into a random index.
: random ( limit -- result )
    dup 1 < abort" limit is zero?"

    \ Get microseconds.
    utime drop

    \ Get result.
    swap mod
;

\ Return true if three numbers are not all equal.
: 3<> ( u2 u1 u0 -- bool )
    over = -rot = and 0=
;

\ Load the stack with string info and token-count, like:
\ 0 or
\ c-addr c-cnt 1 or
\ c-addr c-cnt c-addr c-cnt 2 or
\ c-addr c-cnt c-addr c-cnt c-addr c-cnt 3 or ...
\
\ The stack will have the token info top-down as in the string left-to-right.
: parse-string ( c-start c-cnt -- [c-addr c-cnt ] token-cnt )
    \ Check for null input.
    dup 0= if           \ c-start 0
        nip
        exit
    then

    \ Prep for loop.    \ c-start c-cnt

    \ Calc string end address.
    2dup + 1-           \ c-start c-cnt c-end

    \ Init token counter.
    0 swap              \ c-start fl t-cnt c-end    \ c-cnt is a filler at this point, so -2rot works.

    \ Setup loop range
    #3 pick             \ c-start fl t-cnt c-end c-start
    over                \ c-start fl t-cnt c-end c-start c-end

    \ Scan the string, end to start.
    \ Loop i value is the current possible start value.
    do                              \ c-start fl t-cnt c-end
        \ Get next char
        i c@                        \ c-start fl t-cnt c-end char

        \ Check for valid, printable, char.
        dup $20 < if
        else
            \ GE space.
            dup $7E > if
            else
                \ LE ~
                \ Check for a separator.
                bl =                        \ c-start fl t-cnt c-end flag

                if
                    \ Check length GT zero.
                    dup i -                 \ c-start fl t-cnt c-end s-len
                    dup                     \ c-start fl t-cnt c-end s-len s-len
                    if                      \ c-start fl t-cnt c-end s-len

                        \ Calc token start.
                        i  1+               \ c-start fl t-cnt c-end s-len t-start

                        \ Prep token definition.
                        swap                \ c-start fl t-cnt c-end t-start t-len

                        \ Store token def deeper into stack.
                        -2rot               \ c-start fl t-cnt c-end

                        \ Update token count.
                        swap 1+ swap        \ c-start fl t-cnt c-end
                    else                    \ c-start fl t-cnt c-end s-len
                        drop                \ c-start fl t-cnt c-end
                    then

                    \ Restart c-end
                    drop i 1-               \ c-start fl t-cnt c-end
                then
            then
        then

        \ Count string index down.
        -1
    +loop
                                    \ c-start fl t-cnt c-end

    \ Check for the last token, if any.

    \ Check length GT zero.
    dup                             \ c-start fl t-cnt c-end c-end
    #4 pick                         \ c-start fl t-cnt c-end c-end c-start
    - 1+                            \ c-start fl t-cnt c-end t-cnt
    dup                             \ c-start fl t-cnt c-end t-cnt t-cnt
    if                              \ c-start fl t-cnt c-end t-cnt
        \ Pepare string definition.
        #4 pick                     \ c-start fl t-cnt c-end t-cnt t-start
        swap                        \ c-start fl t-cnt c-end t-start t-cnt

        \ Store token def deeper into stack.
        -2rot                       \ c-start fl t-cnt c-end

        \ Update token count.
        drop                        \ c-start fl t-cnt
        1+                          \ c-start fl t-cnt
    else                            \ c-start fl t-cnt c-end t-cnt
        2drop                       \ c-start fl t-cnt
    then
    \ Clear stack
    nip nip                         \ t-cnt
;

: 3dup #2 pick #2 pick #2 pick ;

\ Get the first word, 16 bits, of a, possibly invalid, address.
: get-first-word ( addr -- w t | f )
    ['] 0w@ catch           \ addr exception-number | word 0
    dup 0=
    if
        \ Fetch worked.
        drop
        true
        exit
    then

    dup #-9 =
    if
        \ Invalid address.
        2drop
        false
        exit
    then

    \ Other error, likely stack underflow.
    throw
;

\ Drop two items from the stack, return true.
: 2drop-true ( agr1 arg0 -- t )
    2drop
    true
;

