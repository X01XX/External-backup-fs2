\ Functions for mask lists.

\ Check TOS for mask-list.
: is-mask-list? ( tos -- t )
    assert( tos is-list? )
    
    dup list-is-empty?
    if
        drop
        true
    else
        list-get-links link-get-data
        assert( is-mask? )
        true
    then
;

\ Deallocate a mask list.
: mask-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-mask-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate mask instances in the list.
        [ ' mask-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a mask-list
: .mask-list ( list0 -- )
    \ Check arg.
    assert( tos is-mask-list? )

    [ ' .mask ] literal swap .list
;

\ Push a mask to a mask-list.
: mask-list-push ( reg1 list0 -- )
    \ Check args.
    assert( tos is-mask-list? )
    assert( nos is-mask? )

    list-push-struct
;

\ Push a mask to the end of a mask-list.
: mask-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert( tos is-mask-list? )
    assert( nos is-mask? )

    list-push-end-struct
;

\ Return a mask-list from a string.
: mask-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-mask? ] literal over   \ lst xt lst
        list-apply-all-true?                    \ lst bool
        if
            true
        else
            structinfo-list-deallocate-struct-list-xt execute
            false
        then
    else
        false
    then
;
