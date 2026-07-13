\ Functions for region lists.

\ Check TOS for region-list.
: is-region-list? ( tos -- bool )
    dup is-allocated-list?  \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-allocated-region?    \ bool
;

\ Deallocate a region list.
: region-list-deallocate ( reg-lst0 -- )
    \ Check arg.
    assert( tos is-region-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ reg-lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' region-deallocate ] literal over        \ reg-lst0 xt reg-lst0
        list-apply                                  \ reg-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a region-list
: .region-list ( reg-lst0 -- )
    \ Check arg.
    assert( tos is-region-list? )

    [ ' .region ] literal swap .list
;

\ Push a region to a region-list.
: region-list-push ( reg1 reg-lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    list-push-struct
;

\ Push a region to the end of a region-list.
: region-list-push-end ( reg1 reg-lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    list-push-end-struct
;

\ Return a region-list from a string.
: region-list-from-string ( c-addr u -- reg-lst t | f )
    list-from-string-xt execute                 \ lst t | f
    if
        \ Check items.
        [ ' is-allocated-region? ] literal over \ lst xt lst
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

\ Return a region-list from a string, or abort.
: region-list-from-string-a ( c-addr u -- reg-lst )
    region-list-from-string \ reg-list t | f
    invert abort" region-list-from-string failed."
;

\ Return true if two region lists are equal.
: region-lists-eq? ( reg-lst1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    [ ' regions-eq? ] literal -rot  \ xt reg-lst1 reg-lst0
    struct-lists-eq?                \ bool
;

\ Remove the first subset region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove-subset ( reg1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' region-subset? ] literal        \ reg1 reg-lst0  xt
    -rot                                \ xt reg1 reg-lst0

    list-remove                         \ reg2 t | f
    if
        region-deallocate
        true
    else
        false
    then
;

\ Push a region onto a list, if there are no supersets in the list.
\ If there are no supersets in the list, delete any subsets and push the region.
\ Return true if the region is added to the list.
: region-list-push-nosubs ( reg1 reg-lst0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 reg-lst0 reg1 reg-lst0
    [ ' region-superset? ] literal          \ reg1 reg-lst0 reg1 reg-lst0 xt
    -rot                                    \ reg1 reg-lst0 xt reg1 reg-lst0
    list-member?                            \ reg1 reg-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 reg-lst0

    \ Remove all subsets.
    begin
        2dup                                \ reg1 reg-lst0 reg1 reg-lst0
        region-list-remove-subset           \ reg1 reg-lst0 | flag
    while
    repeat

    \ Add region to list.                   \ reg1 reg-lst0
    region-list-push
    true
;



\ Return a list of region intersections with a region-list, no subsets.
: region-list-intersections-nosubs ( reg-lst1 list0 -- reg-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ reg-lst1 reg-lst0
    list-new -rot                       \ ret-list reg-lst1 reg-lst0
    foreach                             \ ret-list reg-lst1 reg-lnk0
        dup link-get-data               \ ret-list reg-lst1 reg-lnk0 reg0
        #2 pick                         \ ret-list reg-lst1 reg-lnk0 reg0 reg-lst1

        foreach                         \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1
            dup link-get-data           \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg1
            #2 pick                     \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg1 reg0
            region-intersection         \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1, reg-int t | f
            if
                                        \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int
                dup                     \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int reg-int
                #6 pick                 \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int reg-int ret-list
                region-list-push-nosubs \ ret-list reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int flag
                if
                    drop
                else
                    region-deallocate
                then
            then
        next                            \ ret-list reg-lst1 link0 reg0 reg-lnk1
                                        \ ret-list reg-lst1 link0 reg0
        drop                            \ ret-list reg-lst1 link0
    next
                                        \ ret-list reg-lst1
    drop
;

\ Combine two reigion-lists, deleting subsets.
: region-list-union-nosubs ( reg-lst1 reg-lst0 -- reg-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ Init return list.
    list-new swap               \ reg-lst1 ret-lst reg-lst0

    foreach                     \ reg-lst1 ret-lst reg-lnk0
        dup link-get-data       \ reg-lst1 ret-lst reg-lnk0 reg0
        #2 pick                 \ reg-lst1 ret-lst reg-lnk0 reg0 ret-lst
        region-list-push-nosubs \ reg-lst1 ret-lst reg-lnk0 bool
        drop
    next
                                \ reg-lst1 ret-lst
    \ Prep for loop 2.
    swap                        \ ret-lst reg-lst1

    foreach                     \ ret-lst reg-lnk1
        dup link-get-data       \ ret-lst reg-lnk1 reg1
        #2 pick                 \ ret-lst reg-lnk1 reg1 ret-lst
        region-list-push-nosubs \ ret-lst reg-lnk1 bool
        drop
    next
                                \ ret-lst
;

\ Return a copy of a list, except for any regions equal to a given region.
: region-list-copy-except ( reg1 reg-lst0 -- lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    \ Init return list.
    list-new                        \ reg1 reg-lst0 ret-lst

    \ For each region in reg-lst0.
    over                            \ reg1 reg-lst0 ret-lst reg-lst0

    foreach                         \ reg1 reg-lst0 ret-lst reg-lnk0
        dup link-get-data           \ reg1 reg-lst0 ret-lst reg-lnk0 regx
        #4 pick                     \ reg1 reg-lst0 ret-lst reg-lnk0 regx reg1
        regions-eq?                 \ reg1 reg-lst0 ret-lst reg-lnk0 bool
        if
        else
            dup link-get-data       \ reg1 reg-lst0 ret-lst reg-lnk0 regx
            #2 pick                 \ reg1 reg-lst0 ret-lst reg-lnk0 regx ret-lst
            region-list-push-end    \ reg1 reg-lst0 ret-lst reg-lnk0
        then
    next
                                    \ reg1 reg-lst0 ret-lst
    over list-get-length            \ reg1 reg-lst0 ret-lst len1
    over list-get-length            \ reg1 reg-lst0 ret-lst len1 len2
    = abort" region not found in list?"

    nip nip                         \ ret-lst
;

\ Return a TOS region-list minus the NOS region.
: region-list-subtract-region ( reg1 reg-lst0 -- lst )
    \ Check args.egion-list-state-in-region
    assert( tos is-region-list? )
    assert( nos is-region? )

    \ Init return list.
    list-new -rot                           \ ret-lst reg1 reg-lst0

    \ Scan through the given list.
    foreach                                 \ ret-lst reg1 reg-lnk0
        over                                \ ret-lst reg1 reg-lnk0 reg1
        over link-get-data                  \ ret-lst reg1 reg-lnk0 reg1 reg2

        \ Test if equal
        2dup region-subset?                 \ ret-lst reg1 reg-lnk0 reg1 reg2 flag
        if
            \ Skip, region does not appear in the result.
            2drop
        else
            \ Check if they intersect
            2dup region-intersects?         \ ret-lst reg1 reg-lnk0 reg1 reg2 flag
            if
                \ They intersect, there will be some remainder.
                region-subtract-xt execute  \ ret-lst reg1 reg-lnk0 remainder-lst

                \ Add remainders to the return list
                dup                         \ ret-lst reg1 reg-lnk0 rem-lst rem-lst

                foreach                     \ ret-lst reg1 reg-lnk0 rem-lst rem-lnk
                    dup link-get-data       \ ret-lst reg1 reg-lnk0 rem-lst rem-lnk rem-reg
                    #5 pick                 \ ret-lst reg1 reg-lnk0 rem-lst rem-lnk rem-reg ret-lst
                    region-list-push-nosubs \ ret-lst reg1 reg-lnk0 rem-lst rem-lnk flag
                    drop                    \ ret-lst reg1 reg-lnk0 rem-lst rem-lnk
                next
                                            \ ret-lst reg1 reg-lnk0 rem-lst
                region-list-deallocate      \ ret-lst reg1 reg-lnk0
            else
                \ Add whole region to the result.
                nip                         \ ret-lst reg1 reg-lnk0 reg2
                #3 pick                     \ ret-lst reg1 reg-lnk0 reg2 ret-lst
                region-list-push-nosubs     \ ret-lst reg1 reg-lnk0 flag
                drop                        \ ret-lst reg1 reg-lnk0
            then
        then
    next
                                            \ ret-lst reg1
    drop                                    \ ret-lst
;

\ Return a copy of a region-list.
: region-list-copy ( reg-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-region-list? )

    \ Init return list.
    list-new swap               \ ret-lst reg-lst0

    foreach                     \ ret-lst reg-lnk0
        dup link-get-data       \ ret-lst reg-lnk0 region
        #2 pick                 \ ret-lst reg-lnk0 region lst-n
        region-list-push-end    \ ret-lst reg-lnk0
    next
                                \ ret-lst
;
\ From the TOS region-list, subtract the NOS region-list.
: region-list-subtract ( reg-lst1 reg-lst0 -- ret-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ Make a list that way be returned empty, or deallocated.
    region-list-copy                \ reg-lst1 ret-lst

    swap                            \ ret-lst reg-lst1

    \ Process each region in reg-lst1.
    foreach                         \ ret-lst reg-lnk1
        dup link-get-data           \ ret-lst reg-lnk1 reg0
        rot                         \ reg-lnk1 reg0 ret-lst
        swap                        \ reg-lnk1 ret-lst reg0
        over                        \ reg-lnk1 ret-lst reg0 ret-lst
        region-list-subtract-region \ reg-lnk1 ret-lst ret-lst-new
        -rot                        \ ret-lst-new reg-lnk1 ret-lst
        region-list-deallocate      \ ret-lst-new reg-lnk1
    next
                                    \ ret-lst
;

\ Return defining region info from a given region list.
\ Returns a list of (defining-region (defining-parts))
: region-list-defining-regions-parts ( reg-lst0 -- defining-parts )
    \ Check arg.
    assert( tos is-region-list? )

    \ Init return list.
    list-new swap                   \ ret-lst reg-lst0

    dup                             \ ret-lst reg-lst0 reg-lst0

    \ For each region.
    foreach                         \ ret-lst reg-lst0 reg-lnk0
        \ Get a region.
        dup link-get-data           \ ret-lst reg-lst0 reg-lnk0 reg0

        \ Get region list, except regx.
        dup                         \ ret-lst reg-lst0 reg-lnk0 reg0 reg0
        #3 pick                     \ ret-lst reg-lst0 reg-lnk0 reg0 reg0 reg-lst0
        region-list-copy-except     \ ret-lst reg-lst0 reg-lnk0 reg0 reg-lst-tmp'

        \ Get reg0 minus region list.
        swap                        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' reg0
        list-new                    \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' reg0 regx-lst'
        tuck region-list-push       \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst'
        2dup                        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst' reg-lst-tmp' regx-lst'
        region-list-subtract        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst' regx-parts'
        swap region-list-deallocate \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-parts'
        swap region-list-deallocate \ ret-lst reg-lst0 reg-lnk0 regx-parts'

        \ Check subtraction results.
        dup list-get-length         \ ret-lst reg-lst0 reg-lnk0 regx-parts' len
        0=
        if
            list-deallocate         \ ret-lst reg-lst0 reg-lnk0
        else
            \ Build ( reg reg-parts ) list.
            list-new                \ ret-lst reg-lst0 reg-lnk0 regx-parts' lstx'
            tuck list-push-struct   \ ret-lst reg-lst0 reg-lnk0 lstx'
            over link-get-data      \ ret-lst reg-lst0 reg-lnk0 lstx' regx
            over list-push-struct   \ ret-lst reg-lst0 reg-lnk0 lstx'

            \ Add list to return list.
            #3 pick                 \ ret-lst reg-lst0 reg-lnk0 lstx' ret-lst
            list-push-struct        \ ret-lst reg-lst0 reg-lnk0
        then
    next
                                    \ ret-lst reg-lst0
    drop                            \ ret-lst
;

\ Return defining region info from a given region list.
\ Returns a list of defining-regions.
: region-list-defining-regions ( reg-lst0 -- dreg-lst )
    \ Check arg.
    assert( tos is-region-list? )

    \ Init return list.
    list-new swap                   \ ret-lst reg-lst0

    dup                             \ ren-lst reg-lst0 reg-lst0

    \ For each region.
    foreach                         \ ret-lst reg-lst0 reg-lnk0
        \ Get a region.
        dup link-get-data           \ ret-lst reg-lst0 reg-lnk0 regx

        \ Get region list, except regx.
        dup                         \ ret-lst reg-lst0 reg-lnk0 regx regx
        #3 pick                     \ ret-lst reg-lst0 reg-lnk0 regx regx reg-lst0
        region-list-copy-except     \ ret-lst reg-lst0 reg-lnk0 regx reg-lst-tmp'

        \ Get regx minus region list.
        swap                        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx
        list-new                    \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx regx-lst'
        tuck region-list-push       \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst'
        2dup                        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst' reg-lst-tmp' regx-lst'
        region-list-subtract        \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-lst' regx-parts'
        swap region-list-deallocate \ ret-lst reg-lst0 reg-lnk0 reg-lst-tmp' regx-parts'
        swap region-list-deallocate \ ret-lst reg-lst0 reg-lnk0 regx-parts'

        \ Check subtraction results.
        dup list-is-empty?          \ ret-lst reg-lst0 reg-lnk0 regx-parts' bool
        swap list-deallocate        \ ret-lst reg-lst0 reg-lnk0 bool
        if
        else
            \ region to list.
            over link-get-data      \ ret-lst reg-lst0 reg-lnk0 regx
            #3 pick                 \ ret-lst reg-lst0 reg-lnk0 regx ret-lst
            list-push-struct        \ ret-lst reg-lst0 reg-lnk0
        then
    next
                                    \ ret-lst reg-lst0
    drop                            \ ret-lst
;

\ Return a list of regions a state is in.
: region-list-regions-state-in ( sta1 reg-lst0 -- ret-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state? )

    \ Init return list.
    list-new -rot                       \ ret-lst sta1 reg-lst0

    \ Check each region.
    foreach                             \ ret-lst sta1 reg-lnk0
        \ Check the current region.
        over                            \ ret-lst sta1 reg-lnk0 sta1
        over link-get-data              \ ret-lst sta1 reg-lnk0 sta1 reg0
        region-superset-of-state?       \ ret-lst sta1 reg-lnk0 flag
        if
            \ Add the region to the return list.
            dup link-get-data           \ ret-lst sta1 reg-lnk0 reg0
            #3 pick                     \ ret-lst sta1 reg-lnk0 reg0 ret-lst
            list-push-struct            \ ret-lst sta1 reg-lnk0
        then
    next

    drop                                \ ret-lst
;

\ Calc a list of (state (regions-state-in)).
: state-list-regions-states-in ( reg-lst1 sta-lst0 -- ret-lst )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-region-list? )

    \ Init return list.
    list-new -rot                       \ ret-lst reg-lst1 sta-lst0

    foreach                             \ ret-lst reg-lst1 sta-lnk0
        dup link-get-data               \ ret-lst reg-lst1 sta-lnk0 sta0
        #2 pick                         \ ret-lst reg-lst1 sta-lnk0 sta0 reg-lst1
        region-list-regions-state-in    \ ret-lst reg-lst1 sta-lnk0 regs-sta-in

        \ Init sub-list
        list-new                        \ ret-lst reg-lst1 sta-lnk0 regs-sta-in sub-lst
        tuck list-push-struct           \ ret-lst reg-lst1 sta-lnk0 sub-lst
        over link-get-data              \ ret-lst reg-lst1 sta-lnk0 sub-lst stax
        over list-push-struct           \ ret-lst reg-lst1 sta-lnk0 sub-lst

        \ Add sub-list to return list.
        #3 pick                         \ ret-lst reg-lst1 sta-lnk0 sub-lst ret-lst
        list-push-struct                \ ret-lst reg-lst1 sta-lnk0
    next
                                        \ ret-lst reg-lst1
    drop
;

\ Function to help sort a list of ( state region-list ),
\ by ascending number of regions.
: state-regs-sort-xt ( sta-regs1 sta-regs0 -- bool )
    \ Check args.
    assert( tos is-state-list? )
    assert( nos is-state-list? )

    list-get-second-item list-get-length        \ sta-num1 len0
    swap list-get-second-item list-get-length   \ len0 len1
    <
;

\ Return the number of regions a state is in.
: region-list-num-state-in ( sta1 reg-lst0 -- u )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state? )

    \ Init count.
    0 swap                          \ sta1 cnt reg-lst0

    foreach                         \ sta1 cnt reg-lnk0
        #2 pick                     \ sta1 cnt reg-lnk0 sta1
        over link-get-data          \ sta1 cnt reg-lnk0 sta1 regx
        region-superset-of-state?   \ sta1 cnt reg-lnk0 bool
        if
            \ Inc count.
            swap 1+ swap
        then
    next
                                    \ sta1 cnt
    nip
;

\ Return a list of states that are in only one region.
: region-list-states-in-only-one ( sta-lst1 reg-lst0 -- sta-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state-list? )

    \ Init return list.
    list-new -rot                       \ ret-lst sta-lst1 reg-lst0

    swap                                \ ret-lst reg-lst0 sta-lst1

    foreach                             \ ret-lst reg-lst0 sta-lnk1
        dup link-get-data               \ ret-lst reg-lst0 sta-lnk1 stax
        #2 pick                         \ ret-lst reg-lst0 sta-lnk1 stax reg-lst0
        region-list-num-state-in        \ ret-lst reg-lst0 sta-lnk1 u
        1 =                             \ ret-lst reg-lst0 sta-lnk1 bool
        if
            \ Add state to list.
            dup link-get-data           \ ret-lst reg-lst0 sta-lnk1 stax
            #3 pick                     \ ret-lst reg-lst0 sta-lnk1 stax ret-lst
            list-push-struct            \ ret-lst reg-lst0 sta-lnk1
        then
    next
                                        \ ret-lst reg-lst0
    drop
;

: region-list-states-in ( sta-lst1 reg-lst0 -- reg-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state-list? )
    \ cr ." region-list-states-in: start: " .stack-gbl cr

    \ Init return list.
    list-new -rot                       \ ret-lst sta-lst1 reg-lst0

    swap                                \ ret-lst reg-lst0 sta-lst1

    foreach                             \ ret-lst reg-lst0 sta-lnk1
        dup link-get-data               \ ret-lst reg-lst0 sta-lnk1 sta1
        #2 pick                         \ ret-lst reg-lst0 sta-lnk1 sta1 reg-lst0
        region-list-regions-state-in    \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst'
        dup                             \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst' sta-reg-lst'

        foreach
            dup link-get-data           \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst' sta-reg-lnk regx
            #5 pick                     \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst' sta-reg-lnk regx ret-lst
            region-list-push-nosubs     \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst' sta-reg-lnk bool
            drop
        next
                                        \ ret-lst reg-lst0 sta-lnk1 sta-reg-lst'
        region-list-deallocate          \ ret-lst reg-lst0 sta-lnk1
    next
                                        \ ret-lst reg-lst0
    drop
    \ cr ." region-list-states-in: end: " .stack-gbl cr
;

: region-list-state-in ( sta1 reg-lst0 -- reg-lst )
    \ Check args.
    \ cr ." region-list-state-in: start: " .stack-gbl cr
    assert( tos is-region-list? )
    assert( nos is-state? )

    \ Init return list.
    list-new -rot                   \ ret-lst sta1 reg-lst0

    foreach                         \ ret-lst sta1 reg-lnk0
        over                        \ ret-lst sta1 reg-lnk0 sta1
        over link-get-data          \ ret-lst sta1 reg-lnk0 sta1 regx
        region-superset-of-state?   \ ret-lst sta1 reg-lnk0 bool
        if
            dup link-get-data       \ ret-lst sta1 reg-lnk0 regx
            #3 pick                 \ ret-lst sta1 reg-lnk0 regx ret-lst
            region-list-push        \ ret-lst sta1 reg-lnk0
        then
    next
                                    \ ret-lst sta1
    drop
;

: region-list-states-not-in ( sta-lst1 reg-lst0 -- sta-lst )
    \ Check args.
    \ cr ." region-list-states-not-in: start: " .stack-gbl cr
    assert( tos is-region-list? )
    assert( nos is-state-list? )

    \ Init return list.
    list-new -rot                       \ ret-lst sta-lst1 reg-lst0

    swap                                \ ret-lst reg-lst0 sta-lst1

    foreach                             \ ret-lst reg-lst0 sta-lnk1
        dup link-get-data               \ ret-lst reg-lst0 sta-lnk1 sta1
        #2 pick                         \ ret-lst reg-lst0 sta-lnk1 sta1 reg-lst0
        region-list-num-state-in        \ ret-lst reg-lst0 sta-lnk1 u
        0=                              \ ret-lst reg-lst0 sta-lnk1 bool
        if
            \ Add state to list.
            dup link-get-data           \ ret-lst reg-lst0 sta-lnk1 sta1
            #3 pick                     \ ret-lst reg-lst0 sta-lnk1 sta1 ret-lst
            list-push-struct            \ ret-lst reg-lst0 sta-lnk1
        then
    next
                                        \ ret-lst reg-lst0
    drop
;

\ Given a list of states and a list of possible region, evaluate for corners and needs.
: region-list-evaluate-for-corners ( sta-lst1 reg-lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state-list? )

    cr ." region-list-evaluate-for-corners: " over .state-list space dup .region-list cr

    \ Get states in only one region.
    2dup region-list-states-in-only-one         \ sta-lst1 reg-lst0 stas-in-one
    cr ." States in only one region: " dup .state-list cr

    \ Get defining regions, based on states given.
    2dup swap region-list-states-in             \ sta-lst1 reg-lst0 stas-in-one def-regs
    cr ." Defining regions: " dup .region-list cr

    \ Get states not in defining regions.
    #3 pick                                     \ sta-lst1 reg-lst0 stas-in-one def-regs sta-lst1
    over region-list-states-not-in              \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in
    cr ." States not in defining regions: " dup .state-list cr

    \ Get list of (states-not-in (regions in)), sorted in ascending order by number regions in.
    #3 pick                                     \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in pos-reg-lst0
    over state-list-regions-states-in           \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg
    cr ." State-regs list: " dup structinfo-list-print-struct-list-xt execute cr
    [ ' state-regs-sort-xt ] literal over       \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg xt stas-reg
    list-sort                                   \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg
    cr ." State-regs list sorted: " dup structinfo-list-print-struct-list-xt execute cr

    \ Check for corners.
    #2 pick                                     \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg def-regs
    foreach
        dup link-get-data                       \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg def-lnk def-regx
        cr ." def reg: " .region cr
    next

    \ Check for new anchors.
    dup                                         \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg stas-reg
    foreach                                     \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg stas-reg-lnk
        dup link-get-data                       \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg stas-reg-lnk sta-regx
        cr ." sta-regs: " structinfo-list-print-struct-list-xt execute cr
    next
                                                \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg
    structinfo-list-deallocate-struct-list-xt   \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in xt
    execute                                     \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in
    state-list-deallocate                       \ sta-lst1 reg-lst0 stas-in-one def-regs
    region-list-deallocate                      \ sta-lst1 reg-lst0 stas-in-one
    state-list-deallocate                       \ sta-lst1 reg-lst0
    2drop
;

\ Return a list containing a region with max X bit positions,
\ given a number of bits.
: region-list-max-x ( nb -- reg-lst )
    region-max-x        \ reg-max
    list-new tuck       \ ret-lst reg-max ret-lst
    list-push-struct    \ ret-lst
;

\ Remove the first subset region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove-superset ( reg1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' region-superset? ] literal      \ reg1 reg-lst0  xt
    -rot                                \ xt reg1 reg-lst0

    list-remove                         \ reg2 t | f
    if
        region-deallocate
        true
    else
        false
    then
;

\ Push a region onto a list.
\ If there are no subsets in the list, delete any supersets and push the region,
\ return true.
: region-list-push-nosups ( reg1 reg-lnk0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 reg-lnk0 reg1 reg-lnk0
    [ ' region-subset? ] literal            \ reg1 reg-lnk0 reg1 reg-lnk0 xt
    -rot                                    \ reg1 reg-lnk0 xt reg1 reg-lnk0
    list-member?                            \ reg1 reg-lnk0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 reg-lnk0
    \ Remove all supersets.
    begin
        2dup                                \ reg1 reg-lnk0 reg1 reg-lnk0
        region-list-remove-superset         \ reg1 reg-lnk0 | flag
    while
    repeat

    \ Store region in list.                 \ reg1 reg-lnk0
    region-list-push
    true
;

\ Return true if a region is in a region-list.
: region-list-member? ( reg1 reg-lst0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' regions-eq? ] literal -rot list-member?
;
