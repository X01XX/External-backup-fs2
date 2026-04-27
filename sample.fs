\ Implement a Sample struct and functions.
\
\ A initial/result pair of taking an action.
\
\ A initial/result problem, that may be solved with one, or many, actions, all within
\ a single domain.

#23719 constant sample-id
    #3 constant sample-struct-number-cells

\ Struct fields
0                           constant sample-header-disp     \ 16-bits, [0] struct id, [1] use count.
sample-header-disp  cell+   constant sample-initial-disp    \ Initial state.
sample-initial-disp cell+   constant sample-result-disp     \ Result state.

0 value sample-mma \ Storage for sample mma instance.

\ Init sample mma, return the addr of allocated memory.
: sample-mma-init ( num-items -- ) \ sets sample-mma.
    dup 1 <
    abort" sample-mma-init: Invalid number of items."

    cr ." Initializing Sample store."
    sample-struct-number-cells swap mma-new to sample-mma
;

\ Check instance type.
: is-allocated-sample ( addr -- flag )
    get-first-word          \ w t | f
    if
        sample-id =
    else
        false
    then
;

\ Check TOS for sample, unconventional, leaves stack unchanged.
: assert-tos-is-sample ( tos -- tos )
    dup is-allocated-sample
    false? if
        s" TOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Check NOS for sample, unconventional, leaves stack unchanged.
: assert-nos-is-sample ( nos tos -- nos tos )
    over is-allocated-sample
    false? if
        s" NOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the initial field from a sample instance.
: sample-get-initial ( smp0 -- state )
    \ Check arg.
    assert-tos-is-sample

    sample-initial-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the result field from a sample instance.
: sample-get-result ( smp0 -- state )
    \ Check arg.
    assert-tos-is-sample

    sample-result-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the initial field from a sample instance, use only in this file.
: _sample-set-initial ( sta1 smp0 -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-state

    sample-initial-disp +   \ Add offset.
    !struct                 \ Set initial field.
;

\ Set the result field of a sample instance, use only in this file.
: _sample-set-result ( sta1 smp0 -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-state

    sample-result-disp +    \ Add offset.
    !struct                 \ Set result field.
;

\ End accessors.

\ Create a sample from two numbers on the stack.
\ The numbers may be the same.
: sample-new ( rslt1 init0 -- smp)
    \ Check args.
    assert-tos-is-state
    assert-nos-is-state

    \ Allocate space.
    sample-id sample-mma
    struct-allocate             \ u1 u2 smp

    \ Store states
    tuck _sample-set-initial   \ u1  smp
    tuck _sample-set-result    \ smp
;

\ Print a sample.
: .sample ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    ." ("
    dup sample-get-initial .state
   ." ->"
   sample-get-result .state
   ." )"
;

\ Deallocate a sample.
: sample-deallocate ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup sample-get-initial state-deallocate
        dup sample-get-result  state-deallocate

        \ Deallocate instance.
        sample-mma mma-deallocate
    else
        struct-dec-use-count
    then
;
