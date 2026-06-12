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
square-sample-list-disp cell+   constant square-rules-disp      \ A list of 0, 1 or 2 rules. Matches PN value of 0, 1 o 2.
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
    over dup 1 < swap
    #3 > or
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
;

: _square-set-pnc ( bool sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-bool

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

: square-get-samples ( sqr0 -- smpl-lst )
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
    dup sample-get-result           \ smpl rslt
    over sample-get-initial         \ smpl rslt initl
    rule-new                        \ smpl rul

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

: square-add-sample ( smpl sqr0 -- bool )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-sample

    \ Init changed flag for the following process.
    false over _square-set-changed

    cr ." TODO" cr
;
