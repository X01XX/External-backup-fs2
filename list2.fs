\ Return a list from a token list, given an xt to convert tokens.
\ xt signature is: c-addr u -- result t | f
: list-from-token-list ( xt tkn-lst -- int-lst t | f )
    \ Check arg.
    assert( tos is-token-list? )

    \ Init stack.
    dup token-list-depth                    \ xt tkn-lst depth
    1+                                      \ xt tkn-lst depth+
    stack-new                               \ xt tkn-lst stk

    \ Init return list.
    list-new                                \ xt tkn-lst stk ret-lst
    over stack-push                         \ xt tkn-lst stk

    \ Prep for loop.
    swap                                    \ xt stk tkn-lst

    foreach                                 \ xt stk tkn-link tkn
        \ Check for left paren.
        s" ("                               \ xt stk tkn-link tkn c-addr u
        rot                                 \ xt stk tkn-link c-addr u tkn
        token-eq-string                     \ xt stk tkn-link flag
        if
            \ Process left paren.

            \ Start new list.
            list-new dup                    \ xt stk tkn-link next-list next-list

            \ Add new list to last list.
            #3 pick                         \ xt stk tkn-link next-list next-list stk
            stack-tos                       \ xt stk tkn-link next-list next-list last-list
            list-push-end-struct            \ xt stk tkn-link next-list

            \ Make new list tos.
            #2 pick                         \ xt stk tkn-link next-list stk
            stack-push                      \ xt stk tkn-link next-list
        else
            \ Check for right paren.
            s" )"                           \ xt stk tkn-link c-addr u
            #2 pick link-get-data           \ xt stk tkn-link c-addr u tkt
            token-eq-string                 \ xt stk tkn-link flag
            if
                \ Process right paren.
                over                        \ xt stk tkn-link stk
                stack-pop                   \ xt stk tkn-link list
                drop                        \ xt stk tkn-link
            else
                \ Check for number
                dup link-get-data           \ xt stk tkn-link tkn
                \ cr ." token: " dup .token cr
                token-get-string            \ xt stk tkn-link c-addr u
                snumber?                    \ xt stk tkn-link, num t | f
                if
                    \ Add integer.          \ xt stk tkn-link num
                    #2 pick                 \ xt stk tkn-link num stk
                    stack-tos               \ xt stk tkn-link num top-list
                    list-push-end           \ xt stk tkn-link
                else
                    \ Process non-integer token.
                    dup link-get-data           \ xt stk tkn-link tkn
                    token-get-string            \ xt stk tkn-link c-addr u
                    #4 pick execute             \ xt stk tkn-link, inst t | f
                    if
                        \ Add instance to list on stack.
                        #2 pick                 \ xt stk tkn-link inst stk
                        stack-tos               \ xt stk tkn-link inst top-list
                        list-push-end-struct    \ xt stk tkn-link
                    else
                        \ Process bad result.
                        drop                    \ xt stk

                        \ Get first list put on stack.
                        dup stack-pop swap      \ xt last-list stk
                        begin
                            dup stack-empty? invert
                        while
                            dup stack-pop       \ xt last-list stk next-list
                            rot drop            \ xt stk next-list
                            swap                \ xt next-list stk
                        repeat

                        \ Free stack.
                        free                    \ xt last-list flag
                        0<> abort" stack free failed?"

                        structinfo-list-store structinfo-list-deallocate-recursive  \ xt
                        drop

                        false
                        exit
                    then
                then
            then
        then
    next

    \ Get highest level list.               \ xt stk
    dup stack-pop                           \ xt stk int-lst

    \ Free stack.
    swap free                               \ xt int-lst
    0<> abort" stack free failed"

    nip                                     \ int-lst

    dup list-is-empty?                      \ int-lst
    if
        list-deallocate
        false
        exit
    then

    \ Avoid unneeded top-level list.
    dup list-get-first-item                 \ int-lst itm0
    is-list?                                \ int-lst bool
    if
        dup list-get-length                 \ int-lst len
        1 =
        if
            \ Get rid of upper-level list.
            dup list-pop-struct             \ int-lst next-lst bool
            invert abort" pop failed?"
            swap list-deallocate            \ next-lst
        then
    then

    true
;

\ Return a struct instance, number from a token.
\ If no conversion can be made, return the token itself.
: list-interpret-string ( c-addr u -- result t | f )

    \ Check for struct instance.
    2dup structinfo-list-store          \ c-addr u c-addr u stkinf-lst
    structinfolist-interpret-string     \ c-addr u, instance t | f
    if
        nip nip
        true
        exit
    then

    \ Return token.
    token-new                           \ tkn t | f
;

\ Go through a list, adding elements to the return list,
\ converting selected lists to complex structs.
\ Note: A single list, describing a complex struct, coul`   d return a struct instead of a list.
: list-from-string2 ( lst0 -- lst t | f )
    \ Check arg.
    assert( tos is-list? )

    \ Check if the list is a struct definition.
    dup list-get-first-item                     \ lst0 first
    is-token?
    if
\ cr ." at 1: " .stack-gbl cr
        dup structinfo-list-store-list-to-struct-xt execute   \ lst0, strct t | f
        if
            nip true exit
        then
    then

    \ Process the list.

    \ Init return list.
    list-new swap                               \ ret-lst lst0

    foreach                                     \ ret-lst lnk item
        dup is-list?                            \ ret-lst lnk item bool
        if
\ cr ." at 2: " .stack-gbl cr
            dup list-get-first-item             \ ret-lst lnk item first
            is-token?                           \ ret-lst lnk item bool
            if
\ cr ." at 3: " .stack-gbl cr
                structinfo-list-store-list-to-struct-xt execute  \ ret-lst lnk, strct t | f
                if
\ cr ." at 4: " .stack-gbl cr
                    #2 pick                     \ ret-lst lnk strct ret-lst
                    list-push-end-struct        \ ret-lst lnk
                else
\ cr ." at 5: " .stack-gbl cr
                    dup link-get-data           \ ret-lst lnk item
                    #2 pick                     \ ret-lst lnk item ret-lst
                    list-push-end-struct        \ ret-lst lnk
                then
            else
\ cr ." at 6: " .stack-gbl cr
                recurse                         \ ret-lst lnk, ret t | f
\ cr ." at 6.1: " .stack-gbl cr
                if
\ cr ." at 6.2: " .stack-gbl cr
                    #2 pick                     \ ret-lst lnk ret ret-lst
\ cr ." at 6.3: " .stack-gbl cr
                    list-push-end-struct        \ ret-lst lnk
                else
\ cr ." at 6.4: " .stack-gbl cr
                    drop
                    struct-list-deallocate
                    false
                    exit
                then
            then
        else
\ cr ." at 7: " .stack-gbl cr
            dup is-struct?                      \ ret-lst lnk item bool
            if
\ cr ." at 7.1: " .stack-gbl cr
                #2 pick                         \ ret-lst lnk item ret-lst
                list-push-end-struct            \ ret-lst lnk
            else
\ cr ." at 7.2: " .stack-gbl cr
                #2 pick                         \ ret-lst lnk item ret-lst
                list-push-end-struct            \ ret-lst lnk
            then
        then
\ cr ." at 9: " .stack-gbl cr
    next
    true
;

\ Produce a, possibly complex, list from a string.
: list-from-string ( c-addr u -- lst t | f )
    token-list-from-string                          \ tkn-lst t | f
    ifnot
        false
        exit
    then

    [ ' list-interpret-string ] literal over        \ tkn-lst xt tkn-lst
    list-from-token-list                            \ tkn-lst, lst t | f
    ifnot
        token-list-deallocate                       \
        false
        exit
    then

    swap token-list-deallocate                      \ lst

    \ cr ." list 1: " dup .struct-list cr
    dup list-from-string2                           \ lst, lst2 t | f
    if
        dup is-list?
        ifnot
            list-new tuck list-push-struct
        then

        swap struct-list-deallocate

        true
     else
        struct-list-deallocate
        false
    then
\    true
;

' list-from-string to list-from-string-xt

\ Return a, possibly complex, list from a string, or abort.
: list-from-string-a ( c-addr u -- lst )
    list-from-string    \ lst t | f
    if
    else
        cr ." list-from-string failed?" cr
        abort
    then
;

\ Put zero, one, or more items on the stack, from a string.
: string-to-stack ( c-addr u -- x* t | f)
    \ Parse string to list.
    list-from-string                    \ lst' t | f
    ifnot
        false
        exit
    then

    \ Push each list item onto stack.
    begin
        dup list-is-empty?              \ lst' bool
        if
            list-deallocate             \ x*
            true
            exit
        then

        dup list-get-first-item         \ lst' x
        is-struct?                      \ lst' bool
        if
            dup list-pop-struct         \ lst', x t | f
        else
            dup list-pop                \ lst', x t | f
        then
        invert abort" pop failed?"

        swap
    again
;

\ Put zero, one, or more items on the stack, from a string.
: string-to-stack-a ( c-addr u -- x* )
    string-to-stack     \ x* t | f
    invert abort" string-to-stack failed"
;

\ Return true if two lists are equal, that is
\ having the same members, order does not matter.
\ Sub-lists are Ok.
: lists-eq? ( lst1 lst0 -- bool )
    \ Check args.
    assert( tos is-list? )
    assert( nos is-list? )

    \ cr ." lists-eq?: "
    \ over print-struct-list-xt execute
    \ space
    \ over print-struct-list-xt execute
    \ cr

    \ Check lengths.
    over list-get-length            \ lst1 lst0 len1
    over list-get-length            \ lst1 lst0 len1 len0
    <> if
        2drop
        false
        exit
    then

    foreach                         \ lst1 lnk0 stc0
        \ Get next item to check.
        #2 pick swap                \ lst0 lnk1 lst0 stc0
        swap                        \ lst0 lnk1 stc0 lst0

        \ Check item.
        structinfo-list-member?     \ lnk0 lnk1 bool
        if
        else
            2drop false exit
        then
    next
                                \ lst0
    drop
    true
;

