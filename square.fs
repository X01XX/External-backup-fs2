\ A square is a memory of a recent samples for a single state.

#23197 constant square-id
    #3 constant square-struct-number-cells
    #4 constant square-number-samples

\ Struct fields
0                               constant square-header-disp     \ Struct kind id, a number.
                                                                \   16 bits.
                                                                \ Use count, a number.
                                                                \   16 bits.
                                                                \ Result Pattern Number (PN).
                                                                \    0 - No pattern.
                                                                \    1 - Only one result.
                                                                \    2 - Two results, in order.  Like 0, 1, 0, 1.  Not 1, 1, 0, 0.
                                                                \        Which result comes first does not matter.
                                                                \    8 bits
                                                                \ Pattern Number Confirmed (PNC), within the most recent
                                                                \    square-number-samples. 1 or 0, for true or false.
                                                                \    8 bits.
                                                                \ Changed flag, 1 or 0, for true or false.
                                                                \    1 if the most recent sample addition changed PR or PNC.
                                                                \    8 bits.
square-header-disp      cell+   constant square-samples-disp    \ A list of square-number-samples samples, earliest sample first.
square-samples-disp     cell+   constant square-rules-disp      \ A list of 0, 1 or 2 rules. Matches PN value of 0, 1 o 2.
                                                                \    If 2 - Order does not matter.

0 value square-mma \ Storage for square mma instance.

\ Init square mma, return the addr of allocated memory.
: square-mma-init ( num-items -- ) \ sets square-mma.
    dup 1 <
    abort" square-mma-init: Invalid number of items."

    cr ." Initializing Square store."
    square-struct-number-cells swap mma-new to square-mma
;

\ Check instance type.
: is-allocated-square? ( addr -- bool )
    dup square-mma mma-is-item  \ addr bool
    if
        struct-get-id
        square-id =             \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for square, unconventional, leaves stack unchanged.
: assert-tos-is-square ( tos -- tos )
    dup is-allocated-square?
    false? if
        s" TOS is not an allocated square"
        .abort-xt execute
    then
;

\ Check NOS for square, unconventional, leaves stack unchanged.
: assert-nos-is-square ( nos tos -- nos tos )
    over is-allocated-square?
    false? if
        s" NOS is not an allocated square"
        .abort-xt execute
    then
;

\ Check nos is a valid pn value.
: assert-nos-is-pn ( nos tos -- nos tos )
    over dup 0 < swap
    #2 > or
    if
        s" nos is not a valid pn value"
        .abort-xt execute
    then
;

\ Start accessors.

: square-get-pn ( sqr0 -- pn )
    \ Check arg.
    assert-tos-is-square

    4c@
;

: _square-set-pn ( pn sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-pn

    4c!
;

: square-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-square

    5c@
    if
        true
    else
        false
    then
;

: _square-set-pnc ( bool sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-bool

    swap        \ sqr0 bool
    if
        1 swap
    else
        0 swap
    then
    5c!
;

: square-get-changed ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-square

    6c@
;

: _square-set-changed ( bool sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-bool

    6c!
;

: _square-get-samples ( sqr0 -- smpl-lst )
    \ Check arg.
    assert-tos-is-square

    square-samples-disp + @
;

: _square-set-samples ( smpl-lst1 sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-sample-list
    over list-get-length #4 > abort" rule list too long?"

    square-samples-disp +
    !struct
;

: square-get-rules ( sqr0 -- rul-lst )
    \ Check arg.
    assert-tos-is-square

    square-rules-disp + @
;

: _square-set-rules ( rul-lst1 sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-rule-list
    over list-get-length #2 > abort" rule list too long?"

    square-rules-disp +
    !struct
;

\ End accessors.

\ Return a new square, given a state and a sample.
: square-new    ( smpl -- sqr )
    \ Check args.
    assert-tos-is-sample

    \ Calc rule.
    dup rule-new-from-sample        \ smpl rule

    \ Make rule list.
    list-new tuck                   \ smpl rul-lst rul rul-lst
    list-push-struct                \ smpl rul-lst
    swap                            \ rul-lst smpl

    \ Make sample list.
    list-new tuck                   \ rul-lst smpl-lst smpl smpl-lst
    list-push-struct                \ rul-lst smpl-lst

    \ Allocate space.
    square-id square-mma
    struct-allocate                 \ rul-lst smpl-lst sqr

    \ Set header fields.
    1 over _square-set-pn
    false over _square-set-pnc
    true over _square-set-changed

    \ Set samples.
    tuck _square-set-samples        \ rul-lst sqr

    \ Set rules.
    tuck _square-set-rules          \ sqr
;

: _square-update-pn ( pn sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-pn

    dup square-get-pn           \ pn-new sqr0 pn-old
    #2 pick                     \ pn-new sqr0 pn-old pn-new
    = if
        2drop
    else
        tuck _square-set-pn     \ sqr0
        true swap               \ true sqr0
        _square-set-changed
    then
;

: _square-update-pnc ( bool sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-bool

    dup square-get-pnc          \ pn-new sqr0 pn-old
    #2 pick                     \ pn-new sqr0 pn-old pn-new
    = if
        2drop
    else
        tuck _square-set-pnc    \ sqr0
        true swap               \ true sqr0
        _square-set-changed
    then
;

\ Update the rules of a square.
: _square-update-rules ( rul-lst1 sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-rule-list
    over list-get-length #2 > abort" rule list too long?"

    dup square-get-rules    \ rul-lst1 sqr0 rul-lst-old
    -rot                    \ rul-lst-old rul-lst1 sqr0
    _square-set-rules       \ rul-lst-old
    rule-list-deallocate
;

\ Calculate, and update, the square pn value.
: _square-calc-pn ( sqr0 -- )
    \ Check arg.
    assert-tos-is-square

    dup _square-get-samples     \ sqr0 smpl-lst

    \ Check for samples length LT 2.
    dup list-get-length #2 <
    if
        cr ." square number samples LT 2" cr
        2drop
        exit
    then

    \ Check for samples length too long.
    dup list-get-length #4 >
    abort" square number samples GT 4?"

    \ Check for length GT 2.
    dup list-get-length         \ sqr0 smpl-lst len
    #2 > if
        \ Check first and third sample results are equal.
        dup list-get-first-item sample-get-result   \ sqr0 smpl-lst r1
        over list-get-third-item sample-get-result  \ sqr0 smpl-lst r1 r3
        states-eq?                                  \ sqr0 smpl-lst bool
        if
        else
            drop                                    \ sqr0
            0 swap _square-update-pn
            exit
        then
    then

    \ Check fof length GT 3.
    dup list-get-length         \ sqr0 smpl-lst len
    #3 > if
        \ Check second and fourth sample results are equal.
        dup list-get-second-item sample-get-result   \ sqr0 smpl-lst r2
        over list-get-fourth-item sample-get-result  \ sqr0 smpl-lst r2 r4
        states-eq?                                  \ sqr0 smpl-lst bool
        if
        else
            drop                                    \ sqr0
            0 swap _square-update-pn
            exit
        then
    then

    \ Check if first and second sample results are equal.
    dup list-get-first-item sample-get-result   \ sqr0 smpl-lst r0
    swap list-get-second-item sample-get-result \ sqr0 r0 r1
    states-eq?
    if
        1 swap _square-update-pn
    else
        #2 swap _square-update-pn
    then
;

\ Calculate, and update, square pnc value.
\ Run after _square-calc-pn.
: _square-calc-pnc ( sqr0 -- )
    \ Check args.
    assert-tos-is-square

    \ Check for pn 0.
    dup square-get-pn       \ sqr0 pn
    0=
    if
        true swap _square-update-pnc
        exit
    then

    dup _square-get-samples \ sqr0 smpl-lst
    list-get-length         \ sqr0 len
    #3 >                    \ sqr0 bool
    swap _square-update-pnc
;

\ Calculate, and update, square rules.
: _square-calc-rules ( sqr0 -- )
    \ Check args.
    assert-tos-is-square

    dup square-get-pn       \ sqr0 pn
    case
        0 of
            list-new                    \ sqr0 rul-lst
            swap _square-update-rules
        endof
        1 of
            dup _square-get-samples     \ sqr0 smpl-lst
            list-get-first-item         \ sqr0 smpl
            rule-new-from-sample        \ sqr0 rul
            list-new tuck               \ sqr0 rul-lst rul rul-lst
            list-push-struct            \ sqr0 rul-lst
            swap _square-update-rules
        endof
        #2 of
            dup _square-get-samples     \ sqr0 smpl-lst
            list-get-second-item        \ sqr0 smpl2
            rule-new-from-sample        \ sqr0 rul2
            list-new tuck               \ sqr0 rul-lst rul2 rul-lst
            list-push-struct            \ sqr0 rul-lst

            over _square-get-samples    \ sqr0 rul-lst smpl-lst
            list-get-first-item         \ sqr0 rul-lst smpl1
            rule-new-from-sample        \ sqr0 rul-lst rul1
            over list-push-end

            swap _square-update-rules
        endof
        cr ." invalid pn value: " dec. abort
    endcase
;

: square-get-state ( sqr0 -- sta )
    _square-get-samples     \ smpl-lst
    list-get-first-item     \ smpl
    sample-get-initial      \ sta
;

: square-add-sample ( smpl sqr0 -- bool )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-sample
    dup square-get-state        \ smpl sqr0 sta1
    #2 pick sample-get-initial  \ smpl sqr0 sta1 sta2
    states-eq?                  \ smpl sqr0 bool
    if
    else
        cr ." sample initial does not match square state" cr abort
    then

    \ Init changed flag for the following process.
    false over _square-set-changed

    \ Add sample.
    swap                        \ sqr0 smpl
    over _square-get-samples    \ sqr0 smpl smpl-lst
    list-push-end-struct        \ sqr0

    \ Check for sample list too long.
    dup _square-get-samples     \ sqr0 smpl-lst
    list-get-length             \ sqr0 len
    square-number-samples >
    if
        dup _square-get-samples \ sqr0 smpl-lst
        list-pop                \ sqr0, data t | f
        drop
        sample-deallocate       \ sqr0
    then

    dup _square-calc-pn         \ sqr0

    dup _square-calc-pnc        \ sqr0

    dup square-get-changed      \ sqr0 bool
    if
        _square-calc-rules
        true
    else
        drop
        false
    then
;

\ Deallocate a square.
: square-deallocate ( sqr0 -- )
    \ Check arg.
    assert-tos-is-square

    dup struct-get-use-count      \ sqr0 count
    dup 0< abort" square-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate states.
        dup _square-get-samples sample-list-deallocate
        dup square-get-rules   rule-list-deallocate

        \ Deallocate instance.
        square-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Print a square.
: .square ( sqr0 -- )
    \ Check arg.
    assert-tos-is-square

    s" (" type
    dup square-get-state .state
    space s" pnc: " type dup square-get-pnc .bool
    space square-get-rules .rule-list
    s" )" type
;

\ Return the number of samples stored by a square.
: square-get-num-samples ( square -- 1-4 )
    \ Check arg.
    assert-tos-is-square

    _square-get-samples
    list-get-length
;

\ Compare a pn0 square with a pn1, or pn2, square.
\ Return I for Incompatible, M for More samples needed.
: squares-compare-pnx-pn0 ( sqr-pnx sqr-pn0 -- char )
    drop            \ sqr-pnx
    square-get-pnc  \ pnc
    if
        [char] I
    else
        [char] M
    then
;

\ Compare two pn1 squares.
\ Return C for Compatible, I for Incompatible.
: squares-compare-pn1-pn1 ( sqr-pn1b sqr-pn1a -- char )
    square-get-rules list-get-first-item        \ sqr-pn1b rul-pn1a
    swap square-get-rules list-get-first-item   \ rul-pn1a rul-pn1b
    rule-union                                  \ rul t | f
    if
        rule-deallocate
        [char] C
    else
        [char] I
    then
;

\ Compare a pn1 and pn2 square.
\ Return  I for Incompatible, M for More samples needed.
: squares-compare-pn1-pn2 ( sqr-pn1 sqr-pn2 -- char )
    over square-get-num-samples                     \ sqr-pn1 sqr-pn2 ns-pn1
    1 >
    if
        2drop
        [char] I
    else
        swap square-get-rules list-get-first-item   \ sqr-pn2 rul-pn1
        swap square-get-rules                       \ rul-pn1 ruls-pn2
        rule-list-union-superset?                   \ bool
        if
            [char] M
        else
            [char] I
        then
    then
;

\ Compare two pn2 squares.
\ Return C for Compatible, I for Incompatible.
: squares-compare-pn2-pn2 ( sqr-pn2b sqr-pn2a -- char )
    square-get-rules            \ sqr-pn2b ruls-pn2a
    swap square-get-rules       \ ruls-pn2a ruls-pn2b
    rule-list-union             \ rul-lst t | f
    if
        rule-list-deallocate
        [char] C
    else
        [char] I
    then
;

\ Compare two squares for union.
\ Return char C for Compatible, I for Incompatible, M for More samples needed.
: squares-compare ( sqr1 sqr0 -- char )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-square

    over square-get-pn      \ sqr1 sqr0 pn1
    over square-get-pn      \ sqr1 sqr0 pn1 pn0
    case
        0 of
            case
                0 of
                    2drop
                    [char] C
                endof
                1 of
                    squares-compare-pnx-pn0
                endof
                #2 of
                    squares-compare-pnx-pn0
                endof
            endcase
        endof
        1 of
            case
                0 of
                    swap squares-compare-pnx-pn0
                endof
                1 of
                    squares-compare-pn1-pn1
                endof
                #2 of
                    swap squares-compare-pn1-pn2
                endof
            endcase
        endof
        #2 of
            case
                0 of
                    swap squares-compare-pnx-pn0
                endof
                1 of
                    squares-compare-pn1-pn2
                endof
                #2 of
                    squares-compare-pn2-pn2
                endof
            endcase
        endof
    endcase
;
