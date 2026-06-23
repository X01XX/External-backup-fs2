\ Implement a rule struct and functions.
\
\ Represent how a rule changes bits.

#23131 constant rule-id
    #5 constant rule-struct-number-cells

\ Struct fields.
0                       constant rule-header-disp   \ 16-bits, [0] struct id, [1] use count, [2] Number bits ( 8 bits )
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

: is-allocated-rule? ( addr -- bool )    \ Check if an address is within the rule array.
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
    dup is-allocated-rule?
    false? if
        s" TOS is not an allocated rule."
        .abort-xt execute
    then
;

: assert-nos-is-rule ( nos tos -- nos tos ) \ Check NOS for rule, unconventional, leaves stack unchanged.
    over is-allocated-rule?
    false? if
        s" NOS is not an allocated rule."
        .abort-xt execute
    then
;

: assert-3os-is-rule ( 3os nos tos -- 3os nos tos ) \ Check 3OS for rule, unconventional, leaves stack unchanged.
    #2 pick is-allocated-rule?
    false? if
        s" 3OS is not an allocated rule."
        .abort-xt execute
    then
;

\ Start accessors.

: rule-get-m00 ( rul0 -- u) \ Return the m00 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m00 ( u1 rul0 -- )  \ Set the m00 field of a rule instance, use only in this file.
    rule-m00-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m01 ( rul0 -- u) \ Return the m01 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m01 ( u1 rul0 -- )  \ Set the m01 field of a rule instance, use only in this file.
    rule-m01-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m11 ( rul0 -- u) \ Return the m11 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m11 ( u1 rul0 -- )  \ Set the m11 field of a rule instance, use only in this file.
    rule-m11-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m10 ( rul0 -- u) \ Return the m10 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m10 ( u1 rul0 -- )  \ Set the m10 field of a rule instance, use only in this file.
    rule-m10-disp + \ Add offset.
    !               \ Set the field.
;

\ Get the number of bits.
: rule-get-num-bits ( rul0 -- nb )
    \ Check arg.
    assert-tos-is-rule

    4c@
;

\ Set the number of bits.
: _rule-set-num-bits ( nb rul0 -- )
    4c!
;

\ End accessors.

: rule-new2 ( u-result u-initial nb -- rul )    \ Create a rule from two numbers on the stack.
    dup 1 <
    if
        cr ." rule-new2: " .s cr
        abort
    then

    dup #64 >
    if
        cr ." rule-new2: " .s cr
        abort
    then

    \ Allocate instance.
    rule-id rule-mma
    struct-allocate         \ u-r u-i nb rul

    \ Set number bits.
    tuck _rule-set-num-bits \ u-r u-i rul

    \ Store fields.
    over !not               \ u-r u-i rul u-i-not
    #3 pick !not            \ u-r u-i rul u-i-not u-r-not
    and                     \ u-r u-i rul m00
    over _rule-set-m00      \ u-r u-i rul

    over !not               \ u-r u-i rul u-i-not
    #3 pick                 \ u-r u-i rul u-i-not u-r
    and                     \ u-r u-i rul m01
    over _rule-set-m01      \ u-r u-i rul

    over                    \ u-r u-i rul u-i
    #3 pick                 \ u-r u-i rul u-i u-r
    and                     \ u-r u-i rul m11
    over _rule-set-m11      \ u-r u-i rul

    over                    \ u-r u-i rul u-i
    #3 pick !not            \ u-r u-i rul u-i u-r-not
    and                     \ u-r u-i rul m10
    over _rule-set-m10      \ u-r u-i rul

    \ Return result.
    nip nip                 \ rul
;

: rule-new ( u-result u-initial -- rul )    \ Create a rule from two numbers on the stack.
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    \ Get current number bits, from current domain.
    current-num-bits-gbl    \ u-result u-initial nb

    rule-new2               \ rul
;

: rule-get-masks ( rul0 -- m00 m01 m11 m10 )    \ Push all four masks onto stack.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m00 swap   \ m00 rule
    dup rule-get-m01 swap   \ m00 m01 rule
    dup rule-get-m11 swap   \ m00 m01 m11 rule
    rule-get-m10            \ m00 m01 m11 m10
;

\ Return a rule's X->0 mask.
: rule-get-x0-mask ( rul0 -- mask )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m00        \ rul0 m00
    swap rule-get-m10       \ m00 m10
    and
;

\ Return a rule's X->1 mask.
: rule-get-x1-mask ( rul0 -- mask )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m11        \ rul0 m11
    swap rule-get-m01       \ m11 m01
    and
;

\ Return true if two rules have a different number of bits.
: rules-dif-num-bits? ( rul1 rul0 -- flag )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    rule-get-num-bits   \ rul1 nb0
    swap                \ nb0 rul1
    rule-get-num-bits   \ nb0 nb1
    <>
;

: rules-eq? ( rul1 rul0 -- flag ) \ Return true if two rules are equal.
    \ Check arg.
    assert-tos-is-rule
    assert-nos-is-rule
    2dup rules-dif-num-bits? abort" rules do not have the same number bits?"

    \ Check m00
    2dup rule-get-m00           \ rul1 rul0 rul1 0m00
    swap rule-get-m00           \ rul1 rul0 0m00 1m00
    <> if
        2drop
        false
        exit
    then

    \ Check m01
    2dup rule-get-m01           \ rul1 rul0 rul1 0m01
    swap rule-get-m01           \ rul1 rul0 0m01 1m01
    <> if
        2drop
        false
        exit
    then

    \ Check m11
    2dup rule-get-m11           \ rul1 rul0 rul1 0m11
    swap rule-get-m11           \ rul1 rul0 0m11 1m11
    <> if
        2drop
        false
        exit
    then

    \ Check m10
    rule-get-m10                \ rul1 0m10
    swap rule-get-m10           \ 0m10 1m10
    <> if
        false
    else
        true
    then
;

: .rule ( rul0 -- ) \ Print a rule.
    \ Check arg.
    assert-tos-is-rule

    \ Set up masks and most-significant-bit,
    \ the basis of each cycle.
    dup rule-get-num-bits ms-bit        \ rul ms

    >r                                  \ rul rs: ms
    rule-get-masks                      \ m00 m01 m11 m10 rs: ms
    r>                                  \ m00 m01 m11 m10 ms

    begin
        dup
    while               \ ms-bit is gt 0
                        \ m00 m01 m11 m10 ms |
        0               \ m00 m01 m11 m10 ms | 0

        \ Check m00
        #5 pick         \ m00 m01 m11 m10 ms | 0 m00
        #2 pick         \ m00 m01 m11 m10 ms | 0 m00 ms
        and             \ m00 m01 m11 m10 ms | 0 zero-or-non-zero
        if
            1+          \ m00 m01 m11 m10 ms | sum
        then

        \ Check m01
        #4 pick         \ m00 m01 m11 m10 ms | sum m01
        #2 pick         \ m00 m01 m11 m10 ms | sum m01 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #2 +        \ m00 m01 m11 m10 ms | sum
        then

        \ Check m11
        #3 pick         \ m00 m01 m11 m10 ms | sum m11
        #2 pick         \ m00 m01 m11 m10 ms | sum m11 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #4 +        \ m00 m01 m11 m10 ms | sum
        then

        \ Check m10
        #2 pick         \ m00 m01 m11 m10 ms | sum m10
        #2 pick         \ m00 m01 m11 m10 ms | sum m10 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #8 +        \ m00 m01 m11 m10 ms | sum
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
        \ Deallocate instance.
        rule-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: rule-calc-initial-region ( rul0 -- reg0 ) \ Return rule initial region.
    \ Check arg.
    assert-tos-is-rule

    \ Save number bits.
    dup rule-get-num-bits swap  \ nb rul0

    rule-get-masks      \ nb m00 m01 m11 m10
    or -rot             \ nb most-ones m00 m01
    or !not             \ nb most-ones most-zeros
    rot region-new2
;

: rule-calc-result-region ( rul0 -- reg0 )  \ Return rule result region.
    \ Check arg.
    assert-tos-is-rule

    \ Save number bits.
    dup rule-get-num-bits swap  \ nb rul0

    rule-get-masks      \ nb m00 m01 m11 m10
    -rot                \ nb m00 m10 m01 m11
    or -rot             \ nb most-ones m00 m10
    or !not             \ nb most-ones most-zeros
    rot region-new2
;

: rule-intersects-changes? ( csgs1 rul0 -- flag )    \ Return true if a rule's change intersects a changes' changes.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    over changes-get-m01 over rule-get-m01 and
    0<> if
        2drop
        true
        exit
    then

    rule-get-m10 swap changes-get-m10 and
    0<>
;

: rule-initial-region-intersects ( reg1 rul0 -- bool )  \ Return trdiff regioncorrlist_t.fs /media/earl/Archive/fs2/regioncorrlist_t.fsue if a rule's initial region intersects a region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-initial-region    \ reg1 initial'
    tuck region-intersects?     \ initial' bool
    swap region-deallocate      \ bool
;

: rule-result-region-intersects ( reg1 rul0 -- bool )   \ Return true if a rule's result region intersects a region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-result-region     \ reg1 initial'
    tuck region-intersects?     \ initial' bool
    swap region-deallocate      \ bool
;

: rule-all-bits-set ( rul0 -- flag )    \ Return true if all bit positions in a rule are represented.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-num-bits   \ rul0 nb
    all-bits swap           \ all rul0

    \ Or all mask bits.
    rule-get-masks          \ all m00 m01 m11 m10
    or or or                \ all m-all

    \ Check that all bit positions are used.
    =
;

\ A valid intersection may not have the same initial region as the intersection
\ of the two rules initial regions.
\ As X1 & Xx = 01, X1 & XX = 11, X0 & Xx = 10, X0 & XX = 00.
: rule-intersection ( rul1 rul0 -- rul t | f )    \ Return the valid result of a rule intersection, or false.
    \ Check arg.
    assert-tos-is-rule
    assert-nos-is-rule
    2dup rules-dif-num-bits? abort" rules do not have the same number bits?"

    \ Save number bits.
    dup rule-get-num-bits -rot  \ nb rul1 rul0

    \ Intersect m00         \ nb rul1 rul0
    dup rule-get-m00        \ nb rul1 rul0 | 0m00
    #2 pick rule-get-m00    \ nb rul1 rul0 | 0m00 1m00
    and                     \ nb rul1 rul0 | m00
    -rot                    \ nb m00 | rul1 rul0

    \ Intersect m01         \ nb m00 | rul1 rul0
    dup rule-get-m01        \ nb m00 | rul1 rul0 | 0m01
    #2 pick rule-get-m01    \ nb m00 | rul1 rul0 | 0m01 1m01
    and                     \ nb m00 | rul1 rul0 | m01
    -rot                    \ nb m00 m01 | rul1 rul0

    \ Intersect m11         \ nb m00 m01 | rul1 rul0
    dup rule-get-m11        \ nb m00 m01 | rul1 rul0 | 0m11
    #2 pick rule-get-m11    \ nb m00 m01 | rul1 rul0 | 0m11 1m11
    and                     \ nb m00 m01 | rul1 rul0 | m11
    -rot                    \ nb m00 m01 m11 | rul1 rul0

    \ Intersect m01         \ nb m00 m01 m11 | rul1 rul0
    rule-get-m01            \ nb m00 m01 m11 | rul1 0m01
    swap rule-get-m01       \ nb m00 m01 m11 | 0m11 1m01
    and                     \ nb m00 m01 m11 m10

    \ Start new rule.
    rule-id rule-mma
    struct-allocate         \ nb m00 m01 m11 m10 rul

    \ Set each field.
    tuck _rule-set-m10      \ nb m00 m01 m11 rul
    tuck _rule-set-m11      \ nb m00 m01 rul
    tuck _rule-set-m01      \ nb m00 rul
    tuck _rule-set-m00      \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits \ rul

    \ Check rule.
    dup rule-all-bits-set   \ rul flag

    if
        true                \ rul flag
    else
        rule-deallocate
        false
    then
;

: rule-change-mask ( rul0 -- mask ) \ Return a rule's change mask.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m01        \ rul0 m01
    swap rule-get-m10       \ m01 m10
    or                      \ mask
;

: rule-union ( rul1 rul0 -- rul t | f )   \ Return the result of a rule union, if valid.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule
    2dup rules-dif-num-bits? abort" rules do not have the same number bits?"

    \ Save number bits.
    dup rule-get-num-bits -rot  \ nb rul1 rul0

    \ Combine m00           \ nb rul1 rul0
    over rule-get-m00       \ nb rul1 rul0 | 1m00
    over rule-get-m00       \ nb rul1 rul0 | 1m00 0m00
    or                      \ nb rul1 rul0 | m00
    -rot                    \ nb m00 | rul1 rul0

    \ Combine m01           \ nb m00 | rul1 rul0
    over rule-get-m01       \ nb m00 | rul1 rul0 | 1m01
    over rule-get-m01       \ nb m00 | rul1 rul0 | 1m01 0m01
    or                      \ nb m00 | rul1 rul0 | m01
    -rot                    \ nb m00 m01 | rul1 rul0

    \ Combine m11           \ nb m00 m01 | rul1 rul0
    over rule-get-m11       \ nb m00 m01 | rul1 rul0 | 1m11
    over rule-get-m11       \ nb m00 m01 | rul1 rul0 | 1m11 0m11
    or                      \ nb m00 m01 | rul1 rul0 | m11
    -rot                    \ nb m00 m01 m11 | rul1 rul0

    \ Combine m10           \ nb m00 m01 m11 | rul1 rul0
    rule-get-m10            \ nb m00 m01 m11 | rul1 0m10
    swap rule-get-m10       \ nb m00 m01 m11 | 0m10 1m10
    or                      \ nb m00 m01 m11 m10

    \ Start new rule.
    rule-id rule-mma
    struct-allocate         \ nb m00 m01 m11 m10 rul

    \ Set each field.
    tuck _rule-set-m10      \ nb m00 m01 m11 rul
    tuck _rule-set-m11      \ nb m00 m01 rul
    tuck _rule-set-m01      \ nb m00 rul
    tuck _rule-set-m00      \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits \ rul

    \ Check for 0->X.
    dup rule-get-m00
    over rule-get-m01
    and
    if
        rule-deallocate
        false
        exit
    then

    \ Check for 1->X.
    dup rule-get-m11
    over rule-get-m10
    and
    if
        rule-deallocate
        false
    else
        true
    then
;

: rule-union-by-changes ( rul1 rul0 -- rul t | f )    \ If two rule changes (m01, m10) are equal, form a union.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    \ Check if a union can be made.
    over rule-change-mask       \ rul1 rul0 rcm1
    over rule-change-mask       \ rul1 rul0 rcm1 rcm0
    <> if
        2drop
        false
        exit
    then

    \ Make union.
    rule-union
;

\ 0->X becomes 0->0, 1->X becomes 1->1, by the principle of Least Change.
\ There is no possibility of an X->x position in the result rule.
: rule-new-region-to-region ( reg-to reg-from -- rul )  \ Return a rule for translating one region (tos) to another.
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-region
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    \ 2dup cr .region space .region cr

    \ Save number bits.
    dup region-get-num-bits -rot        \ nb reg-to reg-from

    2dup change-masks-region-to-region  \ nb reg-to reg-from r10 r01
    2swap                               \ nb r10 r01 reg-to reg-from

    \ Get reg-from masks.
    dup region-x-mask -rot              \ nb r10 r01 fx reg-to reg-from
    dup region-edge-0-mask -rot         \ nb r10 r01 fx f0 reg-to reg-from
    region-edge-1-mask swap             \ nb r10 r01 fx f0 f1 reg-to

    \ Get reg-to masks.
    dup region-x-mask swap              \ nb r10 r01 fx f0 f1 tx reg-to
    dup region-edge-0-mask swap         \ nb r10 r01 fx f0 f1 tx t0 reg-to
    region-edge-1-mask                  \ nb r10 r01 fx f0 f1 tx t0 t1

    \ Calc combined masks.
    #5 pick #3 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx
    #6 pick #3 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0
    #7 pick #3 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1
    #7 pick #6 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x
    #8 pick #6 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00
    #8 pick #8 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x
    #9 pick #7 pick and                 \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11

    \ Calc r00.
    #2 pick #6 pick #8 pick #6 pick     \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | m00 mx0 mxx m0x
    or or or                            \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00

    \ Calc r11.
     over #6 pick #9 pick #5 pick       \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00 m11 mx1 mxx m1x
     or or or                           \ nb r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00 r11

     \ Clean up.
     2nip 2nip 2nip 2nip 2nip 2nip      \ nb r10 r01 fx r00 r11
     rot drop                           \ nb r10 r01 r00 r11

    \ Init rule
    rule-id rule-mma
    struct-allocate                     \ nb r10 r01 r00 r11 rul

    \ Build rule.
    tuck _rule-set-m11                  \ nb r10 r01 r00 rul
    tuck _rule-set-m00                  \ nb r10 r01 rul
    tuck _rule-set-m01                  \ nb r10 rul
    tuck _rule-set-m10                  \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits             \ rul
;

: rule-restrict-initial-region ( reg1 rul0 -- rul t | f )   \ Return a rule restricted to an intersecting initial region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    over region-get-num-bits    \ reg1 rul0 rnb
    over rule-get-num-bits      \ reg1 rul0 rnb unb
    <> abort" num bits mismatch?"
    \ cr ." rule-restrict-initial-region: " over .region space dup .rule cr

    2dup rule-initial-region-intersects  \ reg1 rul0 bool
    false? if
        2drop
        false
        exit
    then

    \ Save number bits.
    dup rule-get-num-bits -rot  \ nb reg1 reg0

    swap                        \ nb rul0 reg1
    dup region-high-state swap  \ nb rul0 high reg1
    region-low-state            \ nb rul0 high low

    !not                        \ nb rul0 ones zeros

    #2 pick rule-get-m00        \ nb rul0 ones zeros | m00
    over and                    \ nb rul0 ones zeros | n00
    #3 pick rule-get-m01        \ nb rul0 ones zeros | n00 m01
    rot and                     \ nb rul0 ones n00 n01
    2swap                       \ nb n00 n01 rul0 ones

    over rule-get-m11           \ nb n00 n01 rul0 ones m11
    over and                    \ nb n00 n01 rul0 ones n11
    -rot                        \ nb n00 n01 n11 rul0 ones
    swap                        \ nb n00 n01 n11 ones rul0
    rule-get-m10 and            \ nb n00 n01 n11 n10

    rule-id rule-mma
    struct-allocate             \ nb n00 n01 n11 n10 rul

    tuck                        \ nb n00 n01 n11 rul r10 rul
    _rule-set-m10               \ nb n00 n01 n11 rul

    tuck                        \ nb n00 n01 rul n11 rul
    _rule-set-m11               \ nb n00 n01 rul

    tuck                        \ nb n00 rul n01 rul
    _rule-set-m01               \ nb n00 rul

    tuck                        \ nb rul n00 rul
    _rule-set-m00               \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits             \ rul

    true
;

: rule-restrict-result-region ( reg1 rul0 -- rul t | f )    \ Return a rule restricted to a result region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    over region-get-num-bits    \ reg1 rul0 rnb
    over rule-get-num-bits      \ reg1 rul0 rnb unb
    <> abort" num bits mismatch?"

    2dup rule-result-region-intersects  \ reg1 rul0 bool
    false? if
        2drop
        false
        exit
    then

    \ Save number bits.
    dup rule-get-num-bits -rot  \ nb reg1 reg0

    swap                        \ nb rul0 reg1
    dup region-high-state swap  \ nb rul0 high reg1
    region-low-state            \ nb rul0 high low

    !not                        \ nb rul0 ones zeros

    #2 pick rule-get-m00        \ nb rul0 ones zeros | m00
    over and                    \ nb rul0 ones zeros | n00
    #3 pick rule-get-m10        \ nb rul0 ones zeros | n00 m10
    rot and                     \ nb rul0 ones n00 n10
    2swap                       \ nb n00 n10 rul0 ones

    over rule-get-m11           \ nb n00 n10 rul0 ones m11
    over and                    \ nb n00 n10 rul0 ones n11
    -rot                        \ nb n00 n10 n11 rul0 ones
    swap                        \ nb n00 n10 n11 ones rul0
    rule-get-m01 and            \ nb n00 n10 n11 n01

    rule-id rule-mma
    struct-allocate             \ nb n00 n10 n11 n01 rul

    tuck                        \ nb n00 n10 n11 rul r01 rul
    _rule-set-m01               \ nb n00 n10 n11 rul

    tuck                        \ nb n00 n10 rul n11 rul
    _rule-set-m11               \ nb n00 n10 rul

    tuck                        \ nb n00 rul n10 rul
    _rule-set-m10               \ nb n00 rul

    tuck                        \ nb rul n00 rul
    _rule-set-m00               \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits             \ rul

    true
;

: _rule-adjust-mask-m00 ( rul -- )  \ Add one to the m00 mask of a rule.
    dup rule-get-m00        \ rul m00
    1+
    swap                    \ m00 rul
    _rule-set-m00
;

: _rule-adjust-mask-m01 ( rul -- )  \ Add one to the m01 mask of a rule.
    dup rule-get-m01        \ rul m01
    1+
    swap                    \ m01 rul
    _rule-set-m01
;

: _rule-adjust-mask-m11 ( rul -- )  \ Add one to the m11 mask of a rule.
    dup rule-get-m11        \ rul m11
    1+
    swap                    \ m11 rul
    _rule-set-m11
;

: _rule-adjust-mask-m10 ( rul -- )  \ Add one to the m10 mask of a rule.
    dup rule-get-m10        \ rul m10
    1+
    swap                    \ m10 rul
    _rule-set-m10
;

: rule-lshift ( rul -- )    \ Shift all masks left by 1 bit position.
    dup rule-get-m00 1 lshift over _rule-set-m00
    dup rule-get-m01 1 lshift over _rule-set-m01
    dup rule-get-m11 1 lshift over _rule-set-m11
    rule-get-m10 1 lshift over _rule-set-m10
;

\ Given initial and result chars,
\ left-shift all rule masks, set the right-most rule mask
\ bit positions.  For the rule-from-string function.
: _rule-adjust-masks ( ci cr rul0 -- bool )  \ Adjust rule, by rule-from-string.
    \ Check arg
    assert-tos-is-rule

    dup rule-lshift
    -rot                \ rul0 ci cr
    swap                \ rul0 cr ci

    \ Process two chars
    case
        [char] 0 of
            case
                [char] 0 of \ process 0->0
                    _rule-adjust-mask-m00
                endof
                [char] 1 of \ process 0->1
                    _rule-adjust-mask-m01
                endof
                \ Unexpected char.
                2drop
                false
                exit
            endcase
        endof
        [char] 1 of
            case
                [char] 0 of \ process 1->0
                    _rule-adjust-mask-m10
                endof
                [char] 1 of \ process 1->1
                    _rule-adjust-mask-m11
                endof
                \ Unexpected char.
                2drop
                false
                exit
            endcase
        endof
        [char] X of
            case
                [char] 0 of \ process X->0 (1->0, 0->0)
                    dup _rule-adjust-mask-m00
                    _rule-adjust-mask-m10
                endof
                [char] 1 of \ process X->1 (1->1, 0->1)
                    dup _rule-adjust-mask-m11
                    _rule-adjust-mask-m01
                endof
                [char] X of \ process X->X (1->1, 0->0)
                    dup _rule-adjust-mask-m11
                    _rule-adjust-mask-m00
                endof
                [char] x of \ process X->x (1->0, 0->1)
                    dup _rule-adjust-mask-m10
                    _rule-adjust-mask-m01
                endof
                \ Unexpected char.
                2drop
                false
                exit
            endcase
        endof
        \ Unexpected char.
        2drop drop
        false
        exit
    endcase
    true
;

\ The slash (/) characters, *including the last*, are important as separators.
\ The uderscore (_) character can also be used.
\ All bit positions must be specified.
: rule-from-string ( c-addr u -- rule t | f )   \ Return a rule from a string, like 00/01/11/10/XX/Xx/X0/X1/

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

    \ Check if it ends with slash.
    2dup                            \ c-addr u c-addr u
    \ Point to last char.
    + 1-                            \ c-addr u c-addr+
    \ Check last char is /.
    c@                              \ c-addr u chr
    [char] / =                      \ c-addr u bool
    if
    else
        2drop
        false
        exit
    then

    \ Init position counter.
    0 -rot                          \ cnt c-addr u

    \ Init rule.
    rule-id rule-mma
    struct-allocate                 \ cnt c-addr u rul
    0 over _rule-set-m00
    0 over _rule-set-m01
    0 over _rule-set-m11
    0 over _rule-set-m10            \ cnt c-addr u rul

    -rot                            \ cnt rul c-addr u

    \ Check each pair of characters.
    0                               \ cnt rul c-addr u 0
    do                              \ cnt rul c-addr
        dup c@                      \ cnt rul c-addr ci

        case
            [char] ( of endof
            [char] ) of endof
            [char] / of             \ cnt rul ci cr c-addr
                -rot                \ cnt rul c-addr ci cr
                #3 pick             \ cnt rul c-addr ci cr rul
                _rule-adjust-masks  \ cnt rul c-addr bool
                if
                else
                    drop
                    rule-deallocate
                    drop
                    false
                    unloop
                    exit
                then
                \ Inc counter
                rot 1+ -rot
            endof
            [char] _ of
                -rot                \ cnt rul c-addr ci cr
                #3 pick             \ cnt rul c-addr ci cr rul
                _rule-adjust-masks  \ cnt rul c-addr bool
                if
                else
                    drop
                    rule-deallocate
                    drop
                    false
                    unloop
                    exit
                then
                \ Inc counter
                rot 1+ -rot
            endof
            tuck                    \ cnt rul cx addr
        endcase
        1+
    loop
                                    \ cnt rul c-addr+
    drop                            \ cnt rul

    swap                            \ rul cnt
    over _rule-set-num-bits         \ rul
    true
;

\ Get rule from a string, abort if the attempt failed.
: rule-from-string-a ( c-addr u -- rule )
    rule-from-string        \ rule t | f
    0= abort" rule-from-string failed?"
;

: rule-get-changes ( rul0 -- cngs ) \ Return a changes struct, from a rule.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m10 swap   \ m10 rul0
    rule-get-m01            \ m10 m01
    changes-new
;

: rule-makes-change ( rul0 -- flag )    \ Return true if a rule changes at least one bit.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m01        \ rul0 m01
    0<> if
        drop
        true
        exit
    then

    rule-get-m10            \ m10
    0<>
;

: rule-restrict-to-region ( reg1 rul0 -- rul t | f )    \ Return a rule restricted, initial and result regions, to a given region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    over region-get-num-bits    \ reg1 rul0 rnb
    over rule-get-num-bits      \ reg1 rul0 rnb unb
    <> abort" num bits mismatch?"

    \ Restrict the rule's initial region.
    2dup                            \ reg1 rul0 reg1 rul0
    rule-restrict-initial-region    \ reg1 rul0, rul1' t | f
    false? if
        2drop
        false
        exit
    then

    \ Restrict the restricted rule's result region to reg1.
                                    \ reg1 rul0 rul1'
    nip                             \ reg1 rul1'
    tuck                            \ rul1' reg1 rul1'
    rule-restrict-result-region     \ rul1', rul2' t | f
    if                              \ rul1' rul2
        swap rule-deallocate        \ rul2'
        true
    else                            \ rul1'
        rule-deallocate
        false
    then
;

: rule-combine ( rul1-to rul0-from -- rul )    \ Return a rule by combining tos rule to nos rule.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule
    2dup rules-dif-num-bits? abort" rules do not have the same number bits?"

    \ Save number bits.
    dup rule-get-num-bits -rot      \ nb reg1 reg0

    over rule-calc-initial-region   \ nb rul1 rul0 rul1-i
    over rule-calc-result-region    \ nb rul1 rul0 rul1-i rul0-r

    2dup region-intersects?         \ nb rul1 rul0 rul1-i rul0-r bool
    false? abort" rules not linked?"

                                    \ nb rul1 rul0 rul1-i rul0-r
    region-deallocate
    region-deallocate               \ nb rul1 rul0

    \ Calc m00, (rul0-m00 & rul1-m00) + (rul0-m01 rlu1-m10).
    dup rule-get-m00                \ nb rul1 rul0 rul0-m00
    #2 pick rule-get-m00            \ nb rul1 rul0 rul0-m00 rul1-m00
    and                             \ nb rul1 rul0 m00-a
    over rule-get-m01               \ nb rul1 rul0 m00-a rul0-m01
    #3 pick rule-get-m10            \ nb rul1 rul0 m00-a rul0-m01 rul1-m10
    and                             \ nb rul1 rul0 m00-a m00-b
    or                              \ nb rul1 rul0 m00
    -rot                            \ nb m00 rul1 rul0

    \ Calc m01, (rul0-m00 & rul1-m01) + (rul0-m01 rlu1-m11).
    dup rule-get-m00                \ nb m00 rul1 rul0 rul0-m00
    #2 pick rule-get-m01            \ nb m00 rul1 rul0 rul0-m00 rul1-m01
    and                             \ nb m00 rul1 rul0 m01-a
    over rule-get-m01               \ nb m00 rul1 rul0 m00-a rul0-m01
    #3 pick rule-get-m11            \ nb m00 rul1 rul0 m00-a rul0-m01 rul1-m11
    and                             \ nb m00 rul1 rul0 m00-a m01-b
    or                              \ nb m00 rul1 rul0 m01
    -rot                            \ nb m00 m01 rul1 rul0

    \ Calc m011, (rul0-m11 & rul1-m11) + (rul0-m10 rlu1-m01).
    dup rule-get-m11                \ nb m00 m01 rul1 rul0 rul0-m11
    #2 pick rule-get-m11            \ nb m00 m01 rul1 rul0 rul0-m11 rul1-m11
    and                             \ nb m00 m01 rul1 rul0 m11-a
    over rule-get-m10               \ nb m00 m01 rul1 rul0 m11-a rul0-m10
    #3 pick rule-get-m01            \ nb m00 m01 rul1 rul0 m11-a rul0-m10 rul1-m01
    and                             \ nb m00 m01 rul1 rul0 m11-a m11-b
    or                              \ nb m00 m01 rul1 rul0 m11
    -rot                            \ nb m00 m01 m11 rul1 rul0

    \ Calc m010, (rul0-m11 & rul1-m10) + (rul0-m10 rlu1-m00).
    dup rule-get-m11                \ nb m00 m01 m11 rul1 rul0 rul0-m11
    #2 pick rule-get-m10            \ nb m00 m01 m11 rul1 rul0 rul0-m11 rul1-m10
    and                             \ nb m00 m01 m11 rul1 rul0 m10-a
    over rule-get-m10               \ nb m00 m01 m11 rul1 rul0 m10-a rul0-m10
    #3 pick rule-get-m00            \ nb m00 m01 m11 rul1 rul0 m10-a rul0-m10 rul1-m00
    and                             \ nb m00 m01 m11 rul1 rul0 m10-a m10-b
    or                              \ nb m00 m01 m11 rul1 rul0 m10
    -rot                            \ nb m00 m01 m11 m10 rul1 rul0
    2drop                           \ nb m00 m01 m11 m10

    \ Make new rule.
    rule-id rule-mma
    struct-allocate                 \ nb m00 m01 m11 m10 rul

    tuck _rule-set-m10              \ nb m00 m01 m11 rul
    tuck _rule-set-m11              \ nb m00 m01 rul
    tuck _rule-set-m01              \ nb m00 rul
    tuck _rule-set-m00              \ nb rul

    \ Set num bits.
    tuck _rule-set-num-bits             \ rul
;

\ Given a rule and reg-to reg-from, return the number of unwanted changes.
\ Used to compare plansteps.
: rule-number-unwanted-changes ( reg-to reg-from rul0 -- u )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick region-get-num-bits
    #2 pick region-get-num-bits
    #2 pick rule-get-num-bits       \ reg-to reg-from rul0 nb nb nb
    over <> abort" num bits ne?"    \ reg-to reg-from rul0 nb nb
    <> abort" num bits ne?"         \ reg-to reg-from rul0

    \ cr ." rule-number-unwanted-changes: reg-to: " #2 pick .region space ." reg-from: " over .region space dup .rule

    \ Get reg-from to rul0 initial region, restrict rule, then get rule result region.
    over                            \ reg-to reg-from rul0 reg-from
    over rule-calc-initial-region   \ reg-to reg-from rul0 reg-from rul-i'
    tuck swap                       \ reg-to reg-from rul0 rul-i' rul-i' reg-from
    region-translate-to-region      \ reg-to reg-from rul0 rul-i' reg-from'
    swap region-deallocate          \ reg-to reg-from rul0 reg-from'

    \ Get rule restricted.
    tuck swap                       \ reg-to reg-from reg-from' reg-from' rul0
    rule-restrict-initial-region    \ reg-to reg-from reg-from', rul0' t | f
    false? abort" rule-restrict-initial-region: failed?"
    swap region-deallocate          \ reg-to reg-from rul0'

    \ Get rule result region.
    dup rule-calc-result-region     \ reg-to reg-from rul0' rul-r'
    swap rule-deallocate            \ reg-to reg-from rul-r'

    \ Get changes for reg-from to rule result region.
    2dup swap                       \ reg-to reg-from rul-r' rul-r' reg-from
    changes-new-region-to-region    \ reg-to reg-from rul-r' cngs-from-rul'
    swap region-deallocate          \ reg-to reg-from cngs-from-rul'

    \ Get changes for reg-from to reg-to.
    -rot                            \ cngs-from-rul' reg-to reg-from
    changes-new-region-to-region    \ cngs-from-rul' cngs-from-to'

    \ Get changes in cngs-from-rul not in cngs-from-to.
    dup changes-invert              \ cngs-from-rul' cngs-from-to' cngs-from-to''
    swap changes-deallocate         \ cngs-from-rul' cngs-from-to''
    2dup changes-intersection       \ cngs-from-rul' cngs-from-to'' cngs-from-rul''
    swap changes-deallocate         \ cngs-from-rul' cngs-from-rul''
    swap changes-deallocate         \ cngs-from-rul''

    \ Get unwanted changes.
    dup changes-number-changes      \ cngs-from-rul'' u
    swap changes-deallocate         \ u

     \ space ." = " dup . cr
;

: rule-can-be-used-first  ( reg-to reg-from rul0 -- bool )    \ Return true if reg-from to rule initial region involves no needed changes.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick region-get-num-bits
    #2 pick region-get-num-bits
    #2 pick rule-get-num-bits       \ reg-to reg-from rul0 nb nb nb
    over <> abort" num bits ne?"    \ reg-to reg-from rul0 nb nb
    <> abort" num bits ne?"         \ reg-to reg-from rul0

    \ Calc reg-from to rule initial-region changes.
    rule-calc-initial-region            \ reg-to reg-from rul-i'
    2dup swap                           \ reg-to reg-from rul-i' rul-i' reg-from
    changes-new-region-to-region        \ reg-to reg-from rul-i' to-cngs'
    swap region-deallocate              \ reg-to reg-from to-cngs'

    \ Calc needed changes reg-from to reg-to.
    -rot                                \ to-cngs' reg-to reg-from
    changes-new-region-to-region        \ to-cngs' ned-cngs'

    \ Check if they intersect.
    2dup changes-intersect?             \ to-cngs' ned-cngs' bool

    \ Clean up.
    swap changes-deallocate
    swap changes-deallocate             \ bool
    false?
;

: rule-can-be-used-last  ( reg-to reg-from rul0 -- bool )    \ Return true if reg-from to rule result region involves all needed changes.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick region-get-num-bits
    #2 pick region-get-num-bits
    #2 pick rule-get-num-bits       \ reg-to reg-from rul0 nb nb nb
    over <> abort" num bits ne?"    \ reg-to reg-from rul0 nb nb
    <> abort" num bits ne?"         \ reg-to reg-from rul0
    \ cr ." rule-can-be-used-last: rul0: " dup .rule cr

    \ Calc the needed changes, reg-from to reg-to.
    rot                                             \ reg-from rul0 reg-to
    #2 pick                                         \ reg-from rul0 reg-to reg-from
    changes-new-region-to-region                    \ reg-from rul0 cngs-ned'
    -rot                                            \ cngs-ned' reg-from rul0

    \ Calc the rule changes, reg-from to rule result region.
    rule-calc-result-region                         \ cngs-ned' reg-from rul-r'
    tuck swap                                       \ cngs-ned' rul-r' rul-r' reg-from
    changes-new-region-to-region                    \ cngs-ned' rul-r' cngs-rul'
    swap region-deallocate                          \ cngs-ned' cngs-rul'

    \ Check if the rule changes are a superset of the needed changes.
    2dup changes-superset-of                        \ cngs-ned' cngs-rul' bool
    swap changes-deallocate                         \ cngs-ned' bool
    swap changes-deallocate                         \ bool
;

\ Combine tos rule to nos rule.
\ If the tos rule result region does not intersect the nos initial rule,
\ calculate a rule between them.
: ?rule-combine ( rul1 rul0 -- rul )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    over rule-calc-initial-region   \ rul1 rul0 rul1-reg-i
    over rule-calc-result-region    \ rul1 rul0 rul1-reg-i rul0-reg-r

    2dup region-intersects?         \ rul1 rul0 rul1-reg-i rul0-reg-r bool
    if
        region-deallocate
        region-deallocate
        rule-combine                \ rul
        exit
    then

                                    \ rul1 rul0 rul1-reg-i rul0-reg-r
    2dup rule-new-region-to-region  \ rul1 rul0 rul1-reg-i rul0-reg-r rul-between

    swap region-deallocate
    swap region-deallocate          \ rul1 rul0 rul-bet

    tuck                            \ rul1 rul-bet rul0 rul-bet
    swap                            \ rul1 rul-bet rul-bet rul0
    rule-combine                    \ rul1 rul-bet rul-0b

    swap rule-deallocate            \ rul1 rul-0b

    tuck                            \ rul-0b rul1 rul-0b
    rule-combine                    \ rul-0b rul-0b1

    swap rule-deallocate            \ rul-0b1
;

: rule-is-valid? ( rul0 - bool ) \ Return true if a rule is valid.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m00            \ rul0 m00
    over rule-get-m01           \ rul0 m00 m01
    and                         \ rul0 m0x
    0<> if
        \ cr ." rule " over .rule space ." m0x: " dup .value cr
        2drop
        false
        exit
    then

    dup rule-get-m11            \ rul0 m11
    over rule-get-m10           \ rul0 m11 m10
    and                         \ rul0 m1x
    0<> if
        \ cr ." rule " over .rule space ." m1x: " dup .value cr
        2drop
        false
        exit
    then

    dup rule-get-m00 swap       \ m00 rul0
    dup rule-get-m01 swap       \ m00 m01 rul0
    dup rule-get-m11 swap       \ m00 m01 m11 rul0
    rule-get-m10                \ m00 m01 m11 m10
    or or or                    \ mxx
    current-all-bits-mask-gbl   \ mxx dxx
    \ cr ." mxx: " over .value space ." dxx : " dup .value cr

    =                           \ bool
;

: rule-apply-to-region-fc ( reg1 rul0 -- reg t | f )    \ Return the result region from applying a region to a rule's initial region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    over region-get-num-bits
    over rule-get-num-bits
    <> abort" num bits ne?"

    \ Restrict rule initial region.
    rule-restrict-initial-region        \ rul0' t | f
    if
        \ Get result region.
        dup rule-calc-result-region     \ rul0' reg

        \ Clean up.
        swap rule-deallocate            \ reg

        true
    else
        false
    then
;

: rule-apply-to-region-bc ( reg1 rul0 -- reg t | f )    \ Return the initial region from applying a region to a rule's result region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    over region-get-num-bits
    over rule-get-num-bits
    <> abort" num bits ne?"

    \ Restrict rule initial region.
    rule-restrict-result-region         \ rul0' t | f
    if
        \ Get result region.
        dup rule-calc-initial-region    \ rul0' reg

        \ Clean up.
        swap rule-deallocate            \ reg

        true
    else
        false
    then
;

\ Return the result from applying a rule to a state that is within the rule's
\ initial region.
: rule-apply-to-state ( sta1 rul0 - sta-result )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value

    2dup                            \ sta1 rul0 sta1 rul0
    rule-calc-initial-region        \ sta1 rul0 sta1 i-reg'
    tuck                            \ sta1 rul0 i-reg' sta1 i-reg'
    region-superset-of-state?       \ sta1 rul0 i-reg' bool
    swap region-deallocate          \ sta1 rul0 bool
    false? abort" state not within rule initial region?"

    rule-get-changes                \ sta1 cngs'
    tuck                            \ cngs' sta1 cngs'
    changes-apply-to-state          \ cngs' sta-result

    \ Clean up.
    swap changes-deallocate         \ sta-result
;

\ Return true if the TOS rule is a superset of the NOS rule.
: rule-superset-of ( rul-sub rul-sup -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule
    2dup rules-dif-num-bits? abort" rules do not have the same number bits?"

    over rule-intersection      \ rul-sub, rul-int' t | f
    if
        tuck rules-eq?          \ rul-int' bool
        swap rule-deallocate    \ bool
    else
        drop
        false
    then
;


















