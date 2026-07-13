\ Implement a Sample struct and functions.
\
\ A initial/result pair of taking an action.
\
\ A initial/result problem, that may be solved with one, or many, actions, all within
\ a single domain.

#23719 constant sample-struct-id
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
: is-allocated-sample? ( tos -- flag )
    dup sample-mma mma-is-item? \ addr bool
    if
        struct-get-id
        sample-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

' is-allocated-sample? alias is-sample?

\ Start accessors.

\ Return the initial field from a sample instance.
: sample-get-initial ( smp0 -- state )
    \ Check arg.
    assert( tos is-sample? )

    sample-initial-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the result field from a sample instance.
: sample-get-result ( smp0 -- state )
    \ Check arg.
    assert( tos is-sample? )

    sample-result-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the initial field from a sample instance, use only in this file.
: _sample-set-initial ( sta1 smp0 -- )
    \ Check args.
    assert( tos is-sample? )
    assert( nos is-state? )

    sample-initial-disp +   \ Add offset.
    !struct                 \ Set initial field.
;

\ Set the result field of a sample instance, use only in this file.
: _sample-set-result ( sta1 smpl0 -- )
    \ Check args.
    assert( tos is-sample? )
    assert( nos is-state? )

    sample-result-disp +    \ Add offset.
    !struct                 \ Set result field.
;

\ End accessors.

\ Create a sample from two numbers on the stack.
\ The numbers may be the same.
: sample-new ( rslt1 init0 -- smpl )
    \ Check args.
    assert( tos is-state? )
    assert( nos is-state? )
    assert( 2dup states-same-num-bits? )

    \ Allocate space.
    sample-struct-id sample-mma
    struct-allocate             \ u1 u2 smpl

    \ Store states
    tuck _sample-set-initial   \ u1  smpl
    tuck _sample-set-result    \ smp
;

\ Print a sample.
: .sample ( smpl0 -- )
    \ Check arg.
    assert( tos is-sample? )

    \ Print the initial state.
    dup sample-get-initial  \ smpl0 initial
    .state                  \ smpl0

    \ Print ->
    s" ->" type

    \ Print the result state.
    sample-get-result       \ result
    .state
;

\ Deallocate a sample.
: sample-deallocate ( smpl0 -- )
    \ Check arg.
    assert( tos is-sample? )

    dup struct-get-use-count      \ smp0 count
    dup 0< abort" sample-deallocate: Invalid use count"

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

\ Return false if a string is not a sample representation.
\
\ Otherwise, return a sample.
: sample-from-string ( c-addr u -- smpl t | f )
    \ Check length GT 4.
    dup #6 <
    if
        2drop
        false
        exit
    then

    \ Check length is an even number.
    dup #2 mod 0=
    if
    else
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

    \ Check for - char.
    dup #2 /        \ c-addr u u2
    #2 pick + 1-    \ c-addr u c-addr2
    c@              \ c-addr u chr
    [char] - <>     \ c-addr u bool
    if
        2drop
        false
        exit
    then

    \ Check for > char.
    dup #2 /        \ c-addr u u2
    #2 pick +       \ c-addr u c-addr2
    c@              \ c-addr u chr
    [char] > <>     \ c-addr u bool
    if
        2drop
        false
        exit
    then

    \ Get state length.
    dup #2 / 1-

    \ Parse initial state.  \ c-addr u l
    nip                     \ c-addr l
    2dup                    \ c-addr l c-addr l
    state-from-string       \ c-addr l, sta-i t | f
    if
    else
        2drop
        false
        exit
    then

    \ Save initial state.
    -rot                    \ sta-i c-addr l

    \ Parse result state.   \ sta-i c-addr l
    tuck                    \ sta-i l c-addr l
    +                       \ sta-i l c-addr+
    #2 +                    \ sta-i l c-addr+
    swap                    \ sta-i c-addr+ l
    state-from-string       \ sta-i, sta-r t | f
    if
    else
        state-deallocate
        false
        exit
    then

    \ Make sample to return.
    swap                \ sta-r sta-i
    sample-new          \ smpl

    true
;

\ Return a sample from a string, or abort.
: sample-from-string-a ( c-addr u -- smpl )
    sample-from-string      \ sta t | f
    invert abort" Invalid sample string"
;

\ Return true if two samples are equal.
: samples-eq? ( smpl1 smpl0 -- bool )
    over sample-get-initial
    over sample-get-initial
    states-eq?
    if
    else
        2drop
        false
        exit
    then

    sample-get-result
    swap
    sample-get-result
    states-eq?
;
