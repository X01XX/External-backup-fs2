\ A struct to allow keeping integers deeper in the stack, while still being able to manipulate them.
\
\ I can do a <num> pick to get an integer from the stack, but I cannot change the integer and return it to its place.
\ With a reference to an integer struct on the stack, I can do a <num> pick, and change it.
\
\ Hopefully, this makes r-to and r-from operations unneeded.

#23173 constant integer-struct-id
    #2 constant integer-struct-number-cells

\ Struct fields
0                           constant integer-header-disp   \ 16-bits [0] struct id [1] use count.
integer-header-disp  cell+  constant integer-number-disp  \ A number.

0 value integer-mma \ Storage for integer mma instance.

\ Init integer mma, return the addr of allocated memory.
: integer-mma-init ( num-items -- ) \ sets integer-mma.
    dup 1 <
    abort" integer-mma-init: Invalid number of items."

    cr ." Initializing Integer store."
    integer-struct-number-cells swap mma-new to integer-mma
;

\ Check if tos is an allocated integer.
: is-integer? ( tos -- flag )
    dup integer-mma mma-is-item? \ addr bool
    if
        struct-get-id
        integer-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the number field from a integer instance.
: integer-get-number ( int0 -- num )
    \ Check arg.
    assert( tos is-integer? )

    integer-number-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the state-1 field from a integer instance.
: integer-set-number ( num1 int0 -- )
    \ Check arg.
    assert( tos is-integer? )

    \ Get second state.
    integer-number-disp +   \ Add offset.
    !                       \ Fetch the field.
;

\ End accessors.

\ Create a integer from two states on the stack.
: integer-new ( num0 -- int )

    \ Allocate instance.
    integer-struct-id integer-mma   \ num0 id mma
    struct-allocate                 \ num0 int

    \ Store number.
    tuck integer-set-number         \ int
;

\ Deallocate a integer.
: integer-deallocate ( reg0 -- )
    \ Check arg.
    assert( tos is-integer? )

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" integer-deallocate: Invalid use count"

    #2 <
    if
        \ Deallocate instance.
        integer-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .integer ( int0 -- )
    \ Check arg.
    assert( tos is-integer? )

    integer-get-number
    dec.
;

