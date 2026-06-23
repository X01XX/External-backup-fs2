\ Return a list from a token list, given an xt to convert tokens.
\ xt signature is: c-addr u -- result t | f
: list-from-token-list ( xt tkn-lst -- int-lst t | f )
    \ Check arg.
    assert-tos-is-token-list

    \ Init stack.
    dup token-list-depth                    \ xt tkn-lst depth
    1+                                      \ xt tkn-lst depth+
    stack-new                               \ xt tkn-lst stk

    \ Init return list.
    list-new                                \ xt tkn-lst stk ret-lst
    over stack-push                         \ xt tkn-lst stk

    \ Prep for loop.
    swap                                    \ xt stk tkn-lst
    list-get-links                          \ xt stk tkn-link

    begin
        ?dup
    while
        \ Check for left paren.
        s" ("                               \ xt stk tkn-link c-addr u
        #2 pick link-get-data               \ xt stk tkn-link c-addr u tkt
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

        link-get-next
    repeat

    \ Get highest level list.               \ xt stk
    dup stack-pop                           \ xt stk int-lst

    \ Free stack.
    swap free                               \ xt int-lst
    0<> abort" stack free failed"

    nip                                     \ int-lst

    dup list-get-length                     \ int-lst len
    0> if
        \ Avoid unneeded top-level list.
        dup list-get-first-item                 \ int-lst itm0
        is-allocated-list?                      \ int-lst bool
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
    then

    true
;

' list-from-token-list to list-from-token-list-xt

\ Return a struct instance, number from a token.
\ If no conversion can be made, return the token itself.
: list-interpret-string ( c-addr u -- result t | f )
    \ cr ." list-interpret-string: " 2dup type cr
    \ Check for struct instance.
    2dup structinfo-list-store          \ c-addr u c-addr u stkinf-lst
    structinfolist-interpret-string     \ c-addr u, instance t | f
    if
        nip nip
        true
        exit
    then
    \ Return token.
    token-new                               \ tkn
    true
;

\ Produce a list from a string.
: list-from-string ( c-addr u -- lst t | f )
    \ cr ." list-from-string: len: " dup . cr
    token-list-from-string                              \ tkn-lst' t | f
    if
        [ ' list-interpret-string ] literal over        \ tkn-lst' xt tkn-lst'
        list-from-token-list                            \ tkn-lst', lst t | f
        if
            swap token-list-deallocate                  \ lst
            true
        else
            token-list-deallocate                       \
            false
        then
    else                                                \
        false
    then
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

\ Return true if two lists are equal, that is
\ having the same members, order does not matter.
\ Sub-lists are Ok.
: lists-eq? ( lst1 lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ cr ." lists-eq?: "
    \ over structinfo-list-print-struct-list-xt execute
    \ space
    \ over structinfo-list-print-struct-list-xt execute
    \ cr

    \ Check lengths.
    over list-get-length            \ lst1 lst0 len1
    over list-get-length            \ lst1 lst0 len1 len0
    <> if
        2drop
        false
        exit
    then

    \ Prep for loop.
    list-get-links                  \ lst1 lnk0

    begin
        ?dup
    while
        \ Get next item to check.
        over                        \ lst0 lnk1 lst0
        over link-get-data          \ lst0 lnk1 lst0 stc0
        swap                        \ lst0 lnk1 stc0 lst0

        \ Check item.
        structinfo-list-member?     \ lnk0 lnk1 bool
        if
        else
            2drop false exit
        then

        link-get-next
    repeat

                                \ lst0
    drop
    true
;

