\ Functions for sample lists.

\ Check TOS for sample-list.
: is-sample-list? ( tos -- t )
    assert( tos is-list? )
    
    dup list-is-empty?
    if
        drop
        true
    else
        list-get-links link-get-data
        assert( is-sample? )
        true
    then
;

\ Deallocate a sample list.
: sample-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-sample-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate square instances in the list.
        [ ' sample-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a sample-list
: .sample-list ( list0 -- )
    \ Check arg.
    assert( tos is-sample-list? )

    [ ' .sample ] literal swap .list
;
