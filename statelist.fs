\ Functions for state lists.

\ Check if tos is an empty list, or has a state instance as its first item.
: assert-tos-is-state-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-state
        drop
    then
;

\ Check if nos is an empty list, or has a state instance as its first item.
: assert-nos-is-state-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-state
        drop
    then
;

\ Deallocate a state list.
: state-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-state-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate state instances in the list.
        [ ' state-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a state-list
: .state-list ( list0 -- )
    \ Check arg.
    assert-tos-is-state-list

    [ ' .state ] literal swap .list
;

\ Push a state to a state-list.
: state-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-struct
;

\ Push a state to the end of a state-list.
: state-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-state-list
    assert-nos-is-state

    list-push-end-struct
;

\ Return a state-list from a string.
: state-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-allocated-state ] literal over   \ lst xt lst
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

\ Return a state-list from a string.
: state-list-from-string-a ( c-addr u -- reg-lst )
    state-list-from-string  \ lst t | f
    invert abort" Invalid state-list?"
;
 
\ Return the Boolean OR of all states, in a non-empty state-list.
: state-list-or-items ( sta-lst0 -- sta )
    \ Check args.
    assert-tos-is-state-list
    dup list-get-length 0= abort" empty list?"

    list-get-links          \ lnk
    dup link-get-data       \ lnk stax'
    state-copy swap         \ stax' lnk
    link-get-next           \ stax' lnk-nxt

    begin
        ?dup
    while
        dup link-get-data   \ stax' lnk stay
        rot tuck            \ lnk stax' stay stax'
        state-or            \ lnk stax' staz'
        swap                \ lnk staz' stax'
        state-deallocate    \ lnk staz'
        swap                \ staz' lnk

        link-get-next
    repeat
;

\ Return the Boolean AND of all states, in a non-empty state-list.
: state-list-and-items ( sta-lst0 -- sta )
    \ Check args.
    assert-tos-is-state-list
    dup list-get-length 0= abort" empty list?"

    list-get-links          \ lnk
    dup link-get-data       \ lnk stax'
    state-copy swap         \ stax' lnk
    link-get-next           \ stax' lnk-nxt

    begin
        ?dup
    while
        dup link-get-data   \ stax' lnk stay
        rot tuck            \ lnk stax' stay stax'
        state-and           \ lnk stax' staz'
        swap                \ lnk staz' stax'
        state-deallocate    \ lnk staz'
        swap                \ staz' lnk

        link-get-next
    repeat
;

\ Return a region that holnds all states in a given, non-empty, state-list.
: state-list-region ( sta-lst0 -- reg )
    \ Check args.
    assert-tos-is-state-list
    dup list-get-length 0= abort" empty list?"

    dup state-list-or-items     \ sta-lst0 sta-max-1s
    swap state-list-and-items   \ sta-max-1s sta-max-0s
    region-new                  \ reg
;

