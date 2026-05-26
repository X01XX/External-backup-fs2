\ Implement a rule struct and functions.
\
\ Represent how a rule changes bits.

#23131 constant rule-id
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

: is-allocated-rule ( addr -- flag )    \ Check if an address is within the rule array.
    dup rule-mma mma-is-item    \ addr bool
    if
        struct-get-id
        rule-id =               \ bool
    else
        drop
        false                   \ f
    then
;

: assert-tos-is-rule ( tos -- tos ) \ Check TOS for rule, unconventional, leaves stack unchanged.
    dup is-allocated-rule
    false? if
        s" TOS is not an allocated rule."
        .abort-xt execute
    then
;

: assert-nos-is-rule ( nos tos -- nos tos ) \ Check NOS for rule, unconventional, leaves stack unchanged.
    over is-allocated-rule
    false? if
        s" NOS is not an allocated rule."
        .abort-xt execute
    then
;

: assert-3os-is-rule ( 3os nos tos -- 3os nos tos ) \ Check 3OS for rule, unconventional, leaves stack unchanged.
    #2 pick is-allocated-rule
    false? if
        s" 3OS is not an allocated rule."
        .abort-xt execute
    then
;

\ Start accessors.

: rule-get-m00 ( rul0 -- msk ) \ Return the m00 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m00 ( sta1 rul0 -- )  \ Set the m00 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-mask

    rule-m00-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m01 ( rul0 -- msk ) \ Return the m01 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m01 ( sta1 rul0 -- )  \ Set the m01 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-mask

    rule-m01-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m11 ( rul0 -- msk ) \ Return the m11 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m11 ( sta1 rul0 -- )  \ Set the m11 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-mask

    rule-m11-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m10 ( rul0 -- msk ) \ Return the m10 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m10 ( sta1 rul0 -- )  \ Set the m10 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-mask

    rule-m10-disp + \ Add offset.
    !struct         \ Set the field.
;

\ End accessors.

: rule-new ( sta-result sta-initial -- rul )    \ Create a rule from two masks on the stack.
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state

    rule-id rule-mma        \ s-r s-i id mma
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

: .rule ( rul0 -- ) \ Print a rule.
    \ Check arg.
    assert-tos-is-rule

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
    assert-tos-is-rule

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

