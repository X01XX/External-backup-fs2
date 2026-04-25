\ Implement a rule struct and functions.
\
\ Represent how a rule changes bits.

#23131 constant rule-id
    #5 constant rule-struct-number-cells

\ Struct fields.
0                       constant rule-header-disp   \ 16-bits, [0] struct id, [1] use count.
rule-header-disp cell+  constant rule-m00-disp      \ 0->0 mask.
rule-m00-disp    cell+  constant rule-m01-disp      \ 0->1 mask.
rule-m01-disp    cell+  constant rule-m11-disp      \ 1->1 mask.
rule-m11-disp    cell+  constant rule-m10-disp      \ 1->0 mask.

0 value rule-mma    \ Storage for rule mma instance.

: rule-mma-init ( num-items -- ) \ Init rule mma, return the addr of allocated memory.
    dup 1 <
    abort" rule-mma-init: Invalid number of items."

    cr ." Initializing Rule store."
    rule-struct-number-cells swap mma-new to rule-mma
;

\ Check instance type.

: is-allocated-rule ( addr -- flag )    \ Check if an address is within the rule array.
    get-first-word          \ w t | f
    if
        rule-id =
    else
        false
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

: rule-get-m00 ( rul0 -- val ) \ Return the m00 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m00 ( val1 rul0 -- )  \ Set the m00 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value
   
    rule-m00-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m01 ( rul0 -- val ) \ Return the m01 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m01 ( val1 rul0 -- )  \ Set the m01 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value
   
    rule-m01-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m11 ( rul0 -- val ) \ Return the m11 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m11 ( val1 rul0 -- )  \ Set the m11 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value
   
    rule-m11-disp + \ Add offset.
    !struct         \ Set the field.
;

: rule-get-m10 ( rul0 -- val ) \ Return the m10 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m10 ( val1 rul0 -- )  \ Set the m10 field of a rule instance, use only in this file.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value
   
    rule-m10-disp + \ Add offset.
    !struct         \ Set the field.
;

\ End accessors.

: rule-new ( val-result val-initial -- rul )    \ Create a rule from two values on the stack.
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    rule-id rule-mma        \ v-r v-i id mma
    struct-allocate         \ v-r v-i rul

    \ Store fields.
    over value-invert       \ v-r v-i rul v-i-not'
    #3 pick value-invert    \ v-r v-i rul v-i-not' v-r-not'
    2dup value-and          \ v-r v-i rul v-i-not' v-r-not' m00
    swap value-deallocate   \ v-r v-i rul v-i-not' m00
    swap value-deallocate   \ v-r v-i rul m00
    over _rule-set-m00      \ v-r v-i rul

    over value-invert       \ v-r v-i rul v-i-not'
    #3 pick                 \ v-r v-i rul v-i-not' v-r
    over value-and          \ v-r v-i rul v-i-not' m01
    swap value-deallocate   \ v-r v-i rul m01
    over _rule-set-m01      \ v-r v-i rul

    over                    \ v-r v-i rul v-i
    #3 pick                 \ v-r v-i rul v-i v-r
    value-and               \ v-r v-i rul m11
    over _rule-set-m11      \ v-r v-i rul

    over                    \ v-r v-i rul v-i
    #3 pick value-invert    \ v-r v-i rul v-i v-r-not'
    tuck                    \ v-r v-i rul v-r-not' v-i v-r-not'
    value-and               \ v-r v-i rul v-r-not' m10
    swap value-deallocate   \ v-r v-i rul m10
    over _rule-set-m10      \ v-r v-i rul

    \ Return result.
    nip nip                 \ rul
;

: .rule ( rul0 -- ) \ Print a rule.
    \ Check arg.
    assert-tos-is-rule

    \ Set up masks and most-significant-bit,
    \ the basis of each cycle.
    dup rule-get-m00 _value-get-number swap \ n00 rul0
    dup rule-get-m01 _value-get-number swap \ n00 n01 rul0
    dup rule-get-m11 _value-get-number swap \ n00 n01 n11 rul0
    dup rule-get-m10 _value-get-number swap \ n00 n01 n11 n10 rul0
    rule-get-m10 value-calc-msb             \ m00 m01 m11 m10 msb

    begin
        dup
    while               \ ms-bit is gt 0
                        \ m00 m01 m11 m10 msb |
        0               \ m00 m01 m11 m10 msb | 0

        \ Check m00
        #5 pick         \ m00 m01 m11 m10 msb | 0 m00
        #2 pick         \ m00 m01 m11 m10 msb | 0 m00 msb
        and             \ m00 m01 m11 m10 msb | 0 zero-or-non-zero
        if
            1+          \ m00 m01 m11 m10 msb | sum
        then

        \ Check m01
        #4 pick         \ m00 m01 m11 m10 msb | sum m01
        #2 pick         \ m00 m01 m11 m10 msb | sum m01 msb
        and             \ m00 m01 m11 m10 msb | sum zero-or-non-zero
        if
            #2 +        \ m00 m01 m11 m10 msb | sum
        then

        \ Check m11
        #3 pick         \ m00 m01 m11 m10 msb | sum m11
        #2 pick         \ m00 m01 m11 m10 msb | sum m11 msb
        and             \ m00 m01 m11 m10 msb | sum zero-or-non-zero
        if
            #4 +        \ m00 m01 m11 m10 msb | sum
        then

        \ Check m10
        #2 pick         \ m00 m01 m11 m10 msb | sum m10
        #2 pick         \ m00 m01 m11 m10 msb | sum m10 msb
        and             \ m00 m01 m11 m10 msb | sum zero-or-non-zero
        if
            #8 +        \ m00 m01 m11 m10 msb | sum
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

        1 rshift        \ shift ms bit right one position.
        ." /"
    repeat
    \ m00 m01 m11 m10 0
    2drop 2drop drop
;

: rule-deallocate ( rul0 -- )   \ Deallocate a rule.
    \ Check arg.
    assert-tos-is-rule

    dup struct-get-use-count      \ rule-addr count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate values.
        dup rule-get-m00 value-deallocate
        dup rule-get-m01 value-deallocate
        dup rule-get-m11 value-deallocate
        dup rule-get-m10 value-deallocate

        \ Deallocate instance.
        rule-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

