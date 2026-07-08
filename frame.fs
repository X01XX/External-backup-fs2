\ The frame struct, storing up to 5 cells for temporary use within a function.
\ I dislike the r> and >r commands, just as I dislike storing data in the word list.

#61379 constant frame-struct-id
    #6 constant frame-struct-number-cells

\ Frame struct fields.
0                       constant frame-header-disp   \ 16 bits, [0] id, [1] use count.
frame-header-disp cell+ constant frame-disp

0 value frame-mma       \ Storage for the frame mma instance addr.

\ Init frame mma.
: frame-mma-init ( num-items -- ) \ sets frame-mma.
    dup 1 <
    if
        ." frame-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Frame store."
    frame-struct-number-cells swap mma-new to frame-mma
;

\ Check instance type.
: is-allocated-frame? ( tos -- bool )
    dup frame-mma mma-is-item? \ addr bool
    if
        struct-get-id           \ id
        frame-struct-id =       \ bool
    else
        drop
        false                   \ f
    then
;

\ Check TOS for frame.
: is-frame? ( tos -- t )
    dup is-allocated-frame?
    if drop true exit then

    s" not an allocated frame"
    .abort-xt execute
;
\ Start accessors.

\ Get frame data cell 0.
: frame-cell0@ ( frm0 -- u )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    @
;

\ Set frame data cell 0.
: frame-cell0! ( u frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    !
;

\ Get frame data cell 1.
: frame-cell1@ ( frm0 -- u )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    cell+
    @
;

\ Set frame data cell 1.
: frame-cell1! ( u frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    cell+
    !
;

\ Get frame data cell 2.
: frame-cell2@ ( frm0 -- u )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #2 cells +
    @
;

\ Set frame data cell 2.
: frame-cell2! ( u frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #2 cells +
    !
;

\ Get frame data cell 3.
: frame-cell3@ ( frm0 -- u )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #3 cells +
    @
;

\ Set frame data cell 3.
: frame-cell3! ( u frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #3 cells +
    !
;

\ Get frame data cell 4.
: frame-cell4@ ( frm0 -- u )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #4 cells +
    @
;

\ Set frame data cell 4.
: frame-cell4! ( u frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    frame-header-disp +
    #4 cells +
    !
;

\ End accessors.

\ Return a new frame struct instance address, with given data value.
: frame-new ( -- frm0 )
    frame-struct-id frame-mma
    struct-allocate             \ frm
;

\ Print a frame struct instance.
: .frame ( frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    ." Frame: "
    dup frame-cell0@ hex.
    dup frame-cell1@ space hex.
    dup frame-cell2@ space hex.
    dup frame-cell3@ space hex.
    dup frame-cell4@ space hex.
    drop
;

\ Deallocate a frame.
: frame-deallocate ( frm0 -- )
    \ Check arg.
    assert( tos is-frame? )

    dup struct-get-use-count    \ frm0 count
    dup 0< abort" frame-deallocate: Invalid use count"

    dup 0<
    if
        ." frame-deallocate: Invalid use count" abort
    else
        #2 <
        if
            frame-mma mma-deallocate        \ Deallocate instance.
        else
            struct-dec-use-count
        then
    then
;
