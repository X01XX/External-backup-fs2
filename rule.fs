\ Implement a rule struct and functions.
\
\ Represent how a rule changes bits.

#23131 constant rule-struct-id
    #5 constant rule-struct-number-cells

\ Struct fields.
0                       constant rule-header-disp   \ 16-bits, [0] struct id, [1] use count.
rule-header-disp cell+  constant rule-m00-disp      \ 0->0 mask mask.
rule-m00-disp    cell+  constant rule-m01-disp      \ 0->1 mask mask.
rule-m01-disp    cell+  constant rule-m11-disp      \ 1->1 mask mask.
rule-m11-disp    cell+  constant rule-m10-disp      \ 1->0 mask mask.

0 value rule-mma    \ Storage for rule mma instance.

: rule-mma-init ( num-items -- ) \ Init rule mma, return the addr of allocated memory.
    dup 1 <
    abort" rule-mma-init: Invalid number of items."

    cr ." Initializing Rule store."
    rule-struct-number-cells swap mma-new to rule-mma
;

\ Check instance type.

\ Check if tos is an allocated rule.
: is-rule? ( addr -- flag )    \ Check if an address is within the rule array.
    dup rule-mma mma-is-item?   \ addr bool
    if
        struct-get-id
        rule-struct-id =        \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

: rule-get-m00 ( rul0 -- msk ) \ Return the m00 field of a rule instance.
    \ Check arg.
    assert( tos is-rule? )

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m00 ( sta1 rul0 -- )  \ Set the m00 field of a rule instance, use only in this file.
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-mask? )

    rule-m00-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m01 ( rul0 -- msk ) \ Return the m01 field of a rule instance.
    \ Check arg.
    assert( tos is-rule? )

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m01 ( sta1 rul0 -- )  \ Set the m01 field of a rule instance, use only in this file.
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-mask? )

    rule-m01-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m11 ( rul0 -- msk ) \ Return the m11 field of a rule instance.
    \ Check arg.
    assert( tos is-rule? )

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m11 ( sta1 rul0 -- )  \ Set the m11 field of a rule instance, use only in this file.
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-mask? )

    rule-m11-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m10 ( rul0 -- msk ) \ Return the m10 field of a rule instance.
    \ Check arg.
    assert( tos is-rule? )

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m10 ( sta1 rul0 -- )  \ Set the m10 field of a rule instance, use only in this file.
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-mask? )

    rule-m10-disp + \ Add offset.
    !struct         \ Set the field.
;

\ End accessors.

: rule-new ( sta-result sta-initial -- rul )    \ Create a rule from two masks on the stack.
    \ Check args.
    assert( tos is-state? )
    assert( nos is-state? )
    assert( 2dup states-same-num-bits? )

    rule-struct-id rule-mma \ s-r s-i id mma
    struct-allocate         \ s-r s-i rul

    \ Store fields.
    over                    \ s-r s-i rul s-i
    state-invert-to-mask    \ s-r s-i rul m-i-not'
    #3 pick                 \ s-r s-i rul m-i-not' s-r
    state-invert-to-mask    \ s-r s-i rul m-i-not' m-r-not'
    2dup mask-and           \ s-r s-i rul m-i-not' m-r-not' m00
    swap mask-deallocate    \ s-r s-i rul m-i-not' m00
    swap mask-deallocate    \ s-r s-i rul m00
    over _rule-set-m00      \ s-r s-i rul

    over                    \ s-r s-i rul s-r
    state-invert-to-mask    \ s-r s-i rul m-i-not'
    #3 pick                 \ s-r s-i rul m-i-not' s-r
    over swap               \ s-r s-i rul m-i-not' m-i-not' s-r
    state-and-mask-to-mask  \ s-r s-i rul m-i-not' m01
    swap mask-deallocate    \ s-r s-i rul m01
    over _rule-set-m01      \ s-r s-i rul

    over                    \ s-r s-i rul s-i
    #3 pick                 \ s-r s-i rul s-i s-r
    state-and-state-to-mask \ s-r s-i rul m11
    over _rule-set-m11      \ s-r s-i rul

    over                    \ s-r s-i rul s-i
    #3 pick                 \ s-r s-i rul s-i s-r
    state-invert-to-mask    \ s-r s-i rul s-i m-r-not'
    tuck swap               \ s-r s-i rul m-r-not' m-r-not' s-i
    state-and-mask-to-mask  \ s-r s-i rul m-r-not' m10
    swap mask-deallocate    \ s-r s-i rul m10
    over _rule-set-m10      \ s-r s-i rul

    \ Return result.
    nip nip                 \ rul
;

\ Return a rule from a sample.
: rule-new-from-sample ( smpl0 -- rul )
    \ Check arg.
    assert( tos is-sample? )

    dup sample-get-result       \ smpl0 rslt
    swap sample-get-initial     \ rslt init
    rule-new                    \ rul
;

: .rule ( rul0 -- ) \ Print a rule.
    \ Check arg.
    assert( tos is-rule? )

    \ Get the bit-change masks.
    dup rule-get-m00 swap   \ m00 rul0
    dup rule-get-m01 swap   \ m00 m01 rul0
    dup rule-get-m11 swap   \ m00 m01 m11 rul0
    rule-get-m10            \ m00 m01 m11 m10

    \ Prep for loop.
    dup mask-get-num-bits    \ m00 m01 m11 m10 nb
    -1 swap                     \ m00 m01 m11 m10 -1 nb
    1-                          \ m00 m01 m11 m10 -1 nb-

    do
        \ Init sum.
        0               \ m00 m01 m11 m10 | sum

        \ Check m00
        i               \ m00 m01 m11 m10 | sum i
        #5 pick         \ m00 m01 m11 m10 | sum i m00
        mask-bit        \ m00 m01 m11 m10 | sum bt
        if
            1+          \ m00 m01 m11 m10 | sum
        then

        \ Check m01
        i               \ m00 m01 m11 m10 | sum i
        #4 pick         \ m00 m01 m11 m10 | sum i m01
        mask-bit        \ m00 m01 m11 m10 | sum bt
        if
            #2 +        \ m00 m01 m11 m10 | sum
        then

        \ Check m11
        i               \ m00 m01 m11 m10 | sum i
        #3 pick         \ m00 m01 m11 m10 | sum i m11
        mask-bit        \ m00 m01 m11 m10 | sum bt
        if
            #4 +        \ m00 m01 m11 m10 | sum
        then

        \ Check m10
        i               \ m00 m01 m11 m10 | sum i
        #2 pick         \ m00 m01 m11 m10 | sum i m10
        mask-bit        \ m00 m01 m11 m10 | sum bt
        if
            #8 +        \ m00 m01 m11 m10 | sum
        then

        \ Print rule position.
        \ Of 4 masks, one or two can have a bit set and be valid.
        \ Not zero, three or four.
        case
              0 of ." 0?" endof
              1 of ." 00" endof
             #2 of ." 01" endof
             #3 of ." 0X" endof
             #4 of ." 11" endof
             #5 of ." XX" endof
             #6 of ." X1" endof
             #7 of ." 3?" endof
             #8 of ." 10" endof
             #9 of ." X0" endof
            #10 of ." Xx" endof
            #11 of ." 3?" endof
            #12 of ." 1X" endof
            #13 of ." 3?" endof
            #14 of ." 3?" endof
            #15 of ." 4?" endof
        endcase

        ." /"
    1 -loop

    \ m00 m01 m11 m10
    2drop 2drop
;

: rule-deallocate ( rul0 -- )   \ Deallocate a rule.
    \ Check arg.
    assert( tos is-rule? )

    dup struct-get-use-count      \ rule-addr count
    dup 0< abort" rule-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate masks.
        dup rule-get-m00 mask-deallocate
        dup rule-get-m01 mask-deallocate
        dup rule-get-m11 mask-deallocate
        dup rule-get-m10 mask-deallocate

        \ Deallocate instance.
        rule-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return false if a string is not a representation of a rule.
\
\ Otherwise, generate a rule from the string.
\ Like s" 00/01/11/10/X0/x0/X1/x1/XX/xx/Xx/xX/" rule-from-string
: rule-from-string ( c-addr u -- rul t | f )

    \ Try early exit, if possible.

    \ Check min length.
    dup #3 <
    if
        2drop
        false
        exit
    then

    \ Check length is a multiple of 3.
    dup #3 mod 0<>
    if
        2drop
        false
        exit
    then

    \ Check the first separator character.
    over #2 + c@
    [char] / <>
    if
        over #2 + c@
        [char] _ <>
        if
            2drop
            false
            exit
        then
    then

    \ Get number rule bits.
    #3 /                    \ c-addr rb

    \ init rule masks.
    0 over mask-new -rot    \ m00 c-addr rb
    0 over mask-new -rot    \ m00 m01 c-addr rb
    0 over mask-new -rot    \ m00 m01 m11 c-addr rb
    0 over mask-new -rot    \ m00 m01 m11 m10 c-addr rb

    0 do                    \ m00 m01 m11 m10 c-addr

        \ For each set of 3 characters, left to right.

        \ Shift masks left 1 bit.
        #1 pick mask-lshift-1
        #2 pick mask-lshift-1
        #3 pick mask-lshift-1
        #4 pick mask-lshift-1
        dup c@
        case
            [char] 0 of     \ m00 m01 m11 m10 c-addr
                1+ dup c@
                case
                    [char] 0 of
                        \ 00 found
                        #4 pick mask-add-1
                    endof
                    [char] 1 of
                        \ 01 found
                        #3 pick mask-add-1
                    endof
                    \ Unrecognized char, or 0X, exit with false.
                    2drop
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    unloop
                    false
                    exit
                endcase
            endof
            [char] 1 of     \ m00 m01 m11 m10 c-addr
                1+ dup c@
                case
                    [char] 0 of
                        \ 10 found
                        #1 pick mask-add-1
                    endof
                    [char] 1 of
                        \ 11 found
                        #2 pick mask-add-1
                    endof
                    \ Unrecognized char, or 1X, exit with false.
                    2drop
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    unloop
                    false
                    exit
                endcase
            endof
            [char] X of     \ m00 m01 m11 m10 c-addr
                1+ dup c@
                case
                    [char] 0 of
                        \ X0 found
                        #4 pick mask-add-1  \ m00
                        #1 pick mask-add-1  \ m10
                    endof
                    [char] 1 of
                        \ X1 found
                        #3 pick mask-add-1  \ m01
                        #2 pick mask-add-1  \ m11
                    endof
                    [char] X of
                        \ XX found
                        #4 pick mask-add-1  \ m00
                        #2 pick mask-add-1  \ m11
                    endof
                    [char] x of
                        \ Xx found
                        #3 pick mask-add-1  \ m01
                        #1 pick mask-add-1  \ m10
                    endof
                    \ Unrecognized char, exit with false.
                    2drop
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    unloop
                    false
                    exit
                endcase
            endof
            [char] x of     \ m00 m01 m11 m10 c-addr
                1+ dup c@
                case
                    [char] 0 of
                        \ x0 found
                        #4 pick mask-add-1  \ m00
                        #1 pick mask-add-1  \ m10
                    endof
                    [char] 1 of
                        \ x1 found
                        #3 pick mask-add-1  \ m01
                        #2 pick mask-add-1  \ m11
                    endof
                    [char] X of
                        \ xX found
                        #3 pick mask-add-1  \ m01
                        #1 pick mask-add-1  \ m10
                    endof
                    [char] x of
                        \ xx found
                        #4 pick mask-add-1  \ m00
                        #2 pick mask-add-1  \ m11
                    endof
                    \ Unrecognized char, exit with false.
                    2drop
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    mask-deallocate
                    unloop
                    false
                    exit
                endcase
            endof
            \ Unrecognized char.
            2drop
            mask-deallocate
            mask-deallocate
            mask-deallocate
            mask-deallocate
            unloop
            false
            exit
        endcase

        \ Check separator.
        1+ dup c@
        case
            [char] / of
            endof
            [char] _ of
            endof
            \ Unrecognized char, exit with false.
            2drop
            mask-deallocate
            mask-deallocate
            mask-deallocate
            mask-deallocate
            unloop
            false
            exit
        endcase

        \ Point to next rule bit.
        1+

    loop
                                \ m00 m01 m11 m10 c-addr+
    drop                        \ m00 m01 m11 m10

    \ Allocate a new rule.
    rule-struct-id rule-mma
    struct-allocate             \ m00 m01 m11 m10 rul

    \ Load masks.
    tuck _rule-set-m10             \ m00 m01 m11 rul
    tuck _rule-set-m11             \ m00 m01 rul
    tuck _rule-set-m01             \ m00 rul
    tuck _rule-set-m00             \ rul

    true
;

\ Return a rule from a string, or abort.
: rule-from-string-a ( c-addr u -- rul )
    rule-from-string    \ rul t | f
    invert abort" rule-from-string failed."
;

\ Return the number of bits used for a rule.
: rule-get-num-bits ( rul0 -- nb )
    \ Check arg.
    assert( tos is-rule? )

    rule-get-m00            \ m00
    mask-get-num-bits       \ nb
;

\ Return true if two rules have a different number of bits.
: rules-dif-num-bits? ( rul1 rul0 -- flag )
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-rule? )

    rule-get-num-bits  \ rul1 nb0
    swap               \ nb0 rul1
    rule-get-num-bits  \ nb0 nb1
    <>
;

\ Return true if two rules have the same number of bits.
: rules-same-num-bits? ( rul1 rul0 -- flag )
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-rule? )

    rule-get-num-bits  \ rul1 nb0
    swap               \ nb0 rul1
    rule-get-num-bits  \ nb0 nb1
    =
;

\ Return true if a rule is valid after a union.
: rule-valid-union? ( rul0 -- bool )
    \ Check arg.
    assert( tos is-rule? )

    \ Check for 1X.
    dup rule-get-m11        \ rul0 m11
    over rule-get-m10       \ rul0 m11 m10
    mask-and                \ rul0 m1x
    dup mask-is-zero?       \ rul0 m1x bool
    if
        mask-deallocate
    else
        mask-deallocate
        drop
        false
        exit
    then

    \ Check for 0X.
    dup rule-get-m00        \ rul0 m00
    over rule-get-m01       \ rul0 m00 m01
    mask-and                \ rul0 m0x
    dup mask-is-zero?       \ rul0 m0x bool
    swap mask-deallocate    \ rul0 bool
    nip                     \ bool
;

\ Return true if a rule is valid after an intersection.
: rule-valid-intersection? ( rul0 -- bool )
    \ Check arg.
    assert( tos is-rule? )

    \ Get mask for 0X.
    dup rule-get-m00        \ rul0 m00
    over rule-get-m01       \ rul0 m00 m01
    mask-or                 \ rul0 msk0x'

    \ Get mask for 1X.
    over rule-get-m11       \ rul0 msk0x' m11
    #2 pick rule-get-m10    \ rul0 msk0x' m11 m10
    mask-or                 \ rul0 msk0x' msk1x'

    \ Or both masks.
    2dup mask-or            \ rul0 msk0x' msk1x' mskxx'
    swap mask-deallocate    \ rul0 msk0x' mskxx'
    swap mask-deallocate    \ rul0 mskxx'

    \ Check results cover all bit positions.
    dup mask-all-bits?      \ rul0 mskxx' bool

    \ Clean up, return.
    swap mask-deallocate    \ rul0 bool
    nip                     \ bool
;

\ Return the union of two rules.
: rule-union ( rul1 rul0 -- rul t | f )
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-rule? )
    assert( 2dup rules-same-num-bits? )

    \ Get union m00.
    over rule-get-m00
    over rule-get-m00
    mask-or -rot            \ m00' rul1 rul0

    \ Get union m01.
    over rule-get-m01
    over rule-get-m01
    mask-or -rot            \ m00' m01' rul1 rul0

    \ Get union m11.
    over rule-get-m11
    over rule-get-m11
    mask-or -rot            \ m00' m01' m11' rul1 rul0

    \ Get union m10.
    over rule-get-m10
    over rule-get-m10
    mask-or -rot            \ m00' m01' m11' m10' rul1 rul0

    \ Make new rule.
    2drop                   \ m00' m01' m11' m10'

    \ Init rule.
    rule-struct-id rule-mma \ m00' m01' m11' m10' id mma
    struct-allocate         \ m00' m01' m11' m10' rul

    \ Load rule.
    tuck _rule-set-m10      \ m00' m01' m11' rul
    tuck _rule-set-m11      \ m00' m01' rul
    tuck _rule-set-m01      \ m00' rul
    tuck _rule-set-m00      \ rul

    \ Return
    dup rule-valid-union?   \ rul bool
    if
        true
    else
        rule-deallocate
        false
    then
;

\ Return the intersection of two rules.
: rule-intersection ( rul1 rul0 -- rul t | f )
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-rule? )
    assert( 2dup rules-same-num-bits? )

    \ Get intersection m00.
    over rule-get-m00
    over rule-get-m00
    mask-and -rot           \ m00' rul1 rul0

    \ Get intersection m01.
    over rule-get-m01
    over rule-get-m01
    mask-and -rot           \ m00' m01' rul1 rul0

    \ Get intersection m11.
    over rule-get-m11
    over rule-get-m11
    mask-and -rot           \ m00' m01' m11' rul1 rul0

    \ Get intersection m10.
    over rule-get-m10
    over rule-get-m10
    mask-and -rot           \ m00' m01' m11' m10' rul1 rul0

    \ Make new rule.
    2drop                   \ m00' m01' m11' m10'

    \ Init rule.
    rule-struct-id rule-mma \ m00' m01' m11' m10' id mma
    struct-allocate         \ m00' m01' m11' m10' rul

    \ Load rule.
    tuck _rule-set-m10      \ m00' m01' m11' rul
    tuck _rule-set-m11      \ m00' m01' rul
    tuck _rule-set-m01      \ m00' rul
    tuck _rule-set-m00      \ rul

    \ Return.
    dup rule-valid-intersection?    \ rul bool
    if
        true
    else
        rule-deallocate
        false
    then
;

\ Return true if two rules are equal.
: rules-eq? ( rul1 rul0 -- bool )
    \ Check args.
    assert( tos is-rule? )
    assert( nos is-rule? )
    assert( 2dup rules-same-num-bits? )

    \ Check m00.
    over rule-get-m00 over rule-get-m00 masks-eq?   \ rul1 rul0 bool
    if
    else
        2drop
        false
        exit
    then

    \ Check m01
    over rule-get-m01 over rule-get-m01 masks-eq?   \ rul1 rul0 bool
    if
    else
        2drop
        false
        exit
    then

    \ Check m11
    over rule-get-m11 over rule-get-m11 masks-eq?   \ rul1 rul0 bool
    if
    else
        2drop
        false
        exit
    then

    \ Check m10
    rule-get-m10 swap rule-get-m10 masks-eq?   \ bool
;
