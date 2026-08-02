\ Functions for state lists.

\ Check TOS for state-list.
: is-state-list? ( tos -- t )
    dup is-list?        \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?  \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links      \ link
    link-get-data       \ data
    is-state?           \ bool
;

\ Deallocate a state list.
: state-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-state-list? )

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
    assert( tos is-state-list? )

    [ ' .state ] literal swap .list
;

\ Push a state to a state-list.
: state-list-push ( reg1 list0 -- )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state? )

    list-push-struct
;

\ Push a state to the end of a state-list.
: state-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state? )

    list-push-end-struct
;

\ Return a state-list from a string.
: state-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute \ lst t | f
    if
        \ Check items.
        [ ' is-state? ] literal over            \ lst xt lst
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
    assert( tos is-state-list? )
    assert( dup list-get-length 0> )

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
    assert( tos is-state-list? )
    assert( dup list-get-length 0> )

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
    assert( tos is-state-list? )
    assert( dup list-get-length 0> )

    dup state-list-or-items     \ sta-lst0 sta-max-1s
    swap state-list-and-items   \ sta-max-1s sta-max-0s
    region-new                  \ reg
;

\ Return true if two state-lists are equal.
: state-lists-eq? ( sta-lst1 sta-lst0 -- bool )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state-list? )

    \ Check list lengths.
    over list-get-length
    over list-get-length                \ sta-lst1 sta-lst0 len1 len0
    <>
    if
        2drop
        false
        exit
    then

    \  Check list contents.
    foreach                             \ sta-lst1 lnk0
        \ Get current state.
        dup link-get-data               \ sta-lst1 lnk0 data

        \ Check if its in the other list.
        [ ' states-eq? ] literal swap   \ sta-lst1 lnk0 xt data
        #3 pick                         \ sta-lst1 lnk0 xt data lst1
        list-member?                    \ sta-lst1 lnk0 flag

        ifnot
            2drop
            false
            exit
        then
    next
                                        \ sta-lst1
    drop
    true
;

\ Return true if a state state is a subset of a region.
: state-in-region? ( reg1 sta0 -- bool )
    \ Check args.
    assert( tos is-state? )
    assert( nos is-region? )

    swap
    region-superset-of-state?
;

\ Return states in a given region.
: state-list-in-region ( reg1 sta-lst0 -- sta-lst )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-region? )

    [ ' state-in-region? ] literal -rot    \ xt reg1 sqr-lst0
    list-find-all-struct                            \ ret-list
;

\ Append nos state-list to the tos state-list.
: state-list-append ( lst1 lst0 -- )                                                                                                                     
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state-list? )

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link regx
        #2 pick             \ lst0 link regx lst0
        state-list-push     \ lst0 link

        link-get-next
    repeat
                            \ lst0
    drop 
;

\ Return true if the tos corner is a proper superset of the nos corner.
: state-list-is-proper-superset? ( sta-lst1 sta-lst0 -- bool )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state-list? )

    \ Check lengths.
    over list-get-length        \ sta-lst1 sta-lst0 len1
    over list-get-length        \ sta-lst1 sta-lst0 len1 len0
    <
    ifnot 2drop false exit then

    \ Check contents of list.
    [ ' states-eq? ] literal -rot   \ xt sta-lst1 sta-ls0
    list-difference-struct          \ dif-lst'
    dup list-is-empty?              \ dif-lst' bool
    swap state-list-deallocate      \ bool
;
