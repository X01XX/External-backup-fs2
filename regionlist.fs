\ Functions for region lists.

\ Check TOS for region-list.
: is-region-list? ( tos -- bool )
    dup is-list?            \ tos bool
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
    is-region?              \ bool
;

\ Deallocate a region list.
: region-list-deallocate ( reg-lst0 -- )
    \ Check arg.
    assert( tos is-region-list? if true else cr ." tos not region-list? " .stack-gbl cr false then )

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
        [ ' is-region? ] literal over           \ lst xt lst
        list-apply-all-true?                    \ lst bool
        if
            true
        else
            deallocate-struct-list
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

\ Return true if two region-lists are equal.
: region-lists-eq? ( reg-lst1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ Check list lengths.
    over list-get-length
    over list-get-length                \ reg-lst1 reg-lst0 len1 len0
    <>
    if
        2drop
        false
        exit
    then

    \  Check list contents.
    foreach                             \ reg-lst1 lnk0
        \ Get current region.
        dup link-get-data               \ reg-lst1 lnk0 data

        \ Check if its in the other list.
        [ ' regions-eq? ] literal swap  \ reg-lst1 lnk0 xt data
        #3 pick                         \ reg-lst1 lnk0 xt data lst1
        list-member?                    \ reg-lst1 lnk0 flag

        ifnot
            2drop
            false
            exit
        then
    next
                                        \ reg-lst1
    drop
    true
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

\ Push a region onto a list, if there are no duplicates in the list.
\ Return true if the region is added to the list.
: region-list-push-nodups ( reg1 reg-lst0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 reg-lst0 reg1 reg-lst0
    [ ' regions-eq? ] literal               \ reg1 reg-lst0 reg1 reg-lst0 xt
    -rot                                    \ reg1 reg-lst0 xt reg1 reg-lst0
    list-member?                            \ reg1 reg-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 reg-lst0

    \ Add region to list.                   \ reg1 reg-lst0
    region-list-push
    true
;

\ Return a list of region intersections with a region-list, no subsets.
: region-list-intersections-nosubs ( reg-lst1 list0 -- reg-lst)
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    \ reg-lst1 reg-lst0
    list-new -rot                       \ ret-lst reg-lst1 reg-lst0
    foreach                             \ ret-lst reg-lst1 reg-lnk0
        dup link-get-data               \ ret-lst reg-lst1 reg-lnk0 reg0
        #2 pick                         \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lst1

        foreach                         \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1
            dup link-get-data           \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg1
            #2 pick                     \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg1 reg0
            region-intersection         \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1, reg-int t | f
            if
                                        \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int
                dup                     \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int reg-int
                #6 pick                 \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int reg-int ret-list
                region-list-push-nosubs \ ret-lst reg-lst1 reg-lnk0 reg0 reg-lnk1 reg-int flag
                if
                    drop
                else
                    region-deallocate
                then
            then
        next                            \ ret-lst reg-lst1 link0 reg0 reg-lnk1
                                        \ ret-lst reg-lst1 link0 reg0
        drop                            \ ret-lst reg-lst1 link0
    next
                                        \ ret-lst reg-lst1
    drop                                \ ret-lst
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
            2dup regions-intersect?         \ ret-lst reg1 reg-lnk0 reg1 reg2 flag
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

\ Return a list of regions a list of states are in.
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
    cr ." State-regs list: " dup print-struct-list cr
    [ ' state-regs-sort-xt ] literal over       \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg xt stas-reg
    list-sort                                   \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg
    cr ." State-regs list sorted: " dup print-struct-list cr

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
        cr ." sta-regs: " print-struct-list cr
    next
                                                \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in stas-reg
    deallocate-struct-list                      \ sta-lst1 reg-lst0 stas-in-one def-regs stas-not-in
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

\ Return true if a region uses a given state.
: region-list-uses-state? ( sta1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state? )

   [ ' region-uses-state? ] literal -rot list-member? \ lst
;

\ Return true if a state is in any region.
: region-list-any-superset-state? ( sta1 lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-state? )

    foreach                             \ sta1 lnk
        \ Check the current region.
        2dup                            \ sta1 lnk sta1 lnk
        link-get-data                   \ sta1 lnk sta1 regx
        region-superset-of-state?       \ sta1 lnk flag
        if                              \ sta1 lnk
            2drop
            true
            exit
        then

    next
                                        \ sta1
    drop
    false
;

\ Return a list of regions in a region-list that are superset of a
\ given region.
: region-list-supersets-of  ( reg1 reg-lst0 -- reg-lst )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' region-superset? ] literal  \ reg1 reg-lst0 xt
    -rot                            \ xt reg1 reg-lst0
    list-find-all-struct            \ reg-lst
;

\ Return a list of states used to define regions in a region-list.
: region-list-states ( reg-lst0 -- sta-lst )
    \ Check arg.
    assert( tos is-region-list? )

    \ Init return list.
    list-new swap                   \ sta-lst reg-lst0

    foreach                         \ sta-lst reg-lnk
        dup link-get-data           \ sta-lst reg-lnk regx

        \ Check region state-0.
        [ ' states-eq? ] literal    \ sta-lst reg-lnk regx xt
        over region-get-state-0     \ sta-lst reg-lnk regx xt sta0
        #4 pick                     \ sta-lst reg-lnk regx xt sta0 sta-lst
        list-member?                \ sta-lst reg-lnk regx bool
        ifnot
            dup region-get-state-0  \ sta-lst reg-lnk regx sta0
            #3 pick                 \ sta-lst reg-lnk regx sta0 sta-lst
            list-push-struct        \ sta-lst reg-lnk regx
        then

        \ Check region state-1.
        [ ' states-eq? ] literal    \ sta-lst reg-lnk regx xt
        over region-get-state-1     \ sta-lst reg-lnk regx xt sta1
        #4 pick                     \ sta-lst reg-lnk regx xt sta1 sta-lst
        list-member?                \ sta-lst reg-lnk regx bool
        ifnot
            region-get-state-1      \ sta-lst reg-lnk sta1
            #2 pick                 \ sta-lst reg-lnk sta1 sta-lst
            list-push-struct        \ sta-lst reg-lnk
        else
            drop                    \ sta-lst reg-lnk
        then

    next
;

\ Return true if a region-list contains a subset, or equal, region.
: region-list-any-subset-of? ( reg1 list0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' region-subset? ] literal -rot list-member?
;

\ Remove a region from a list.
\ If use count becomes zero, deallocate it.
: region-list-remove ( reg1 reg-lst0 -- )
\ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' = ] literal -rot            \ xt reg1 reg-lst0
    list-remove-struct              \ reg t | f
    if
        dup struct-get-use-count    \ reg uc
        0=
        if
            region-deallocate
        else
            drop
        then
    then
;

\ Return true if a region-list contains a superset, or equal, region.
: region-list-any-superset-of? ( reg1 list0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' region-superset? ] literal -rot list-member?
;

\ Return true if a region intersects any region in a region-list.
: region-list-any-intersection? ( reg1 list0 -- flag )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region? )

    [ ' regions-intersect? ] literal -rot list-member?
;

\ Append nos region-list to the tos region-list.
: region-list-append ( lst1 lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link regx
        #2 pick             \ lst0 link regx lst0
        region-list-push    \ lst0 link

        link-get-next
    repeat
                            \ lst0
    drop
;

\ Append nos region-list to the tos region-list, except duplicates.
: region-list-append-nodups ( lst1 lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    swap                        \ lst0 lst1
    list-get-links              \ lst0 link
    begin
        ?dup
    while
        dup link-get-data       \ lst0 link regx
        #2 pick                 \ lst0 link regx lst0
        region-list-push-nodups \ lst0 link bool
        drop

        link-get-next
    repeat
                                \ lst0
    drop
;

\ Append nos region-list to the tos region-list, no subsets.
: region-list-append-nosubs ( lst1 lst0 -- )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    swap                        \ lst0 lst1
    list-get-links              \ lst0 link
    begin
        ?dup
    while
        dup link-get-data       \ lst0 link regx
        #2 pick                 \ lst0 link regx lst0
        region-list-push-nosubs \ lst0 link bool
        drop

        link-get-next
    repeat
                                \ lst0
    drop
;

\ Return a list of propre intersections of any two regions in a list.
: region-list-proper-intersections ( reg-lst0 -- reg-lst t | f )
    \ Check arg.
    assert( tos is-region-list? )
    \ cr ." region-list-proper-intersections: start" cr
    \ Init return list.
    list-new swap                       \ ret-lst reg-lst

    foreach                             \ ret-lst reg-lnk1
        dup link-get-next               \ ret-lst reg-lnk1 reg-lnk2
        begin
            ?dup
        while
            over link-get-data          \ ret-lst reg-lnk1 reg-lnk2 reg1
            over link-get-data          \ ret-lst reg-lnk1 reg-lnk2 reg1 reg2
            region-proper-intersection  \ ret-lst reg-lnk1 reg-lnk2, reg-int' t | f
            if
                dup                     \ ret-lst reg-lnk1 reg-lnk2 reg-int' reg-int'
                #4 pick                 \ ret-lst reg-lnk1 reg-lnk2 reg-int' reg-int' ret-lst
                region-list-push-nodups \ ret-lst reg-lnk1 reg-lnk2 reg-int' bool
                if
                    drop                \ ret-lst reg-lnk1 reg-lnk2
                else
                    region-deallocate   \ ret-lst reg-lnk1 reg-lnk2
                then
            then
        next
    next
                                        \ ret-lst
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
    \ cr ." region-list-proper-intersections: end" cr
;

\ Return a list of intersections of any two regions in a list.
: region-list-self-intersections-nodups ( reg-lst0 -- reg-lst t | f )
    \ Check arg.
    assert( tos is-region-list? )
    \ cr ." region-list-self-intersections-nodups: start" cr
    \ Init return list.
    list-new swap                       \ ret-lst reg-lst

    foreach                             \ ret-lst reg-lnk1
        dup link-get-next               \ ret-lst reg-lnk1 reg-lnk2
        begin
            ?dup
        while
            over link-get-data          \ ret-lst reg-lnk1 reg-lnk2 reg1
            over link-get-data          \ ret-lst reg-lnk1 reg-lnk2 reg1 reg2
            region-intersection         \ ret-lst reg-lnk1 reg-lnk2, reg-int' t | f
            if
                dup                     \ ret-lst reg-lnk1 reg-lnk2 reg-int' reg-int'
                #4 pick                 \ ret-lst reg-lnk1 reg-lnk2 reg-int' reg-int' ret-lst
                region-list-push-nodups \ ret-lst reg-lnk1 reg-lnk2 reg-int' bool
                if
                    drop                \ ret-lst reg-lnk1 reg-lnk2
                else
                    region-deallocate   \ ret-lst reg-lnk1 reg-lnk2
                then
            then
        next
    next
                                        \ ret-lst
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
    \ cr ." region-list-self-intersections-nodups: end" cr
;

\ Split a region list by intersections.
\ So each result region is a subset of one, or more, of the original regions,
\ but never a proper intersection.
: region-list-split-by-intersections ( reg-lst0 -- reg-lst t | f )
    \ Check arg.
    assert( tos is-region-list? )

    \ Try first pass.
    dup region-list-self-intersections-nodups \ reg-lst0, int-regs' t | f
    ifnot
        drop
        false
        exit
    then

    \ Init return list.
    list-new -rot                           \ ret-lst reg-lst0 int-regs'

    \ Get arg regions minus intersections.
    2dup swap                               \ ret-lst reg-lst0 int-regs' int-regs' reg-lst0
    region-list-subtract                    \ ret-lst reg-lst0 int-regs' rem-lst'
    \ cr ." remainders: " dup .region-list cr

    \ Add remainders to the return list.
    dup #4 pick                             \ ret-lst reg-lst0 int-regs' rem-lst' rem-lst' ret-lst
    region-list-append-nodups               \ ret-lst reg-lst0 int-regs' rem-lst'

    \ Replace the current regions with the intersections.
    region-list-deallocate                  \ ret-lst reg-lst0 int-regs'
    swap drop                               \ ret-lst int-regs'

    begin
        dup                                 \ ret-lst cur-regs' cur-regs'
        region-list-self-intersections-nodups   \ ret-lst cur-regs', int-regs' t | f
        if
            \ Get current regions minus intersections.
            2dup swap                       \ ret-lst cur-regs' int-regs' int-regs' cur-regs'
            region-list-subtract            \ ret-lst cur-regs' int-regs' rem-lst'
            \ cr ." remainders: " dup .region-list cr

            \ Add remainders to the return list.
            dup #4 pick                     \ ret-lst cur-regs' int-regs' rem-lst' rem-lst' ret-lst
            region-list-append-nodups       \ ret-lst cur-regs' int-regs' rem-lst'

            \ Replace the current regions with the intersections.
            region-list-deallocate          \ ret-lst cur-regs' int-regs'
            swap region-list-deallocate     \ ret-lst int-regs'
        else
            \ No new intersections, add whats left.
            2dup swap                       \ ret-lst cur-regs' cur-regs' ret-lst
            region-list-append-nodups       \ ret-lst cur-regs'
            \ cr ." remainders: " dup .region-list cr
            region-list-deallocate
            true
            exit
        then
    again
;

\ Return true if any item in the nos list are proper intersections of any
\ region in the tos list.
: region-list-any-proper-intersections? ( reg-lst1 reg-lst0 -- bool )
    \ Check args.
    assert( tos is-region-list? )
    assert( nos is-region-list? )

    swap                                           \ reg-lst0 reg-lst1

    foreach                                         \ reg-lst0 reg-lnk
        [ ' region-proper-intersection? ] literal   \ reg-lst0 reg-lnk xt
        over link-get-data                          \ reg-lst0 reg-lnk xt regx
        #3 pick                                     \ reg-lst0 reg-lnk xt regx reg-lst0
        list-member?                                \ reg-lst0 reg-lnk bool
        if
            cr ." reg: " dup link-get-data .region
            space ." is a proper subset of: " over .region-list cr
            2drop
            true
            exit
        then
    next
    drop
    false
;

\ Return non-overlapped parts of regions.
: region-list-defining-region-parts ( reg-lst0 -- reg-lst )
    \ Check arg.
    assert( tos is-region-list? )

    \ Init return list.
    list-new swap                               \ ret-lst reg-lst0
    dup                                         \ ret-lst reg-lst0 reg-lst0

    foreach                                     \ ret-lst reg-lst0 reg-lnk
        \ Init temp list.
        list-new                                \ ret-lst reg-lst0 reg-lnk tmp-lst'

        \ Get current target region.
        over link-get-data                      \ ret-lst reg-lst0 reg-lnk tmp-lst' regx

        \ Create target list.
        2dup swap                               \ ret-lst reg-lst0 reg-lnk tmp-lst' regx regx tmp-lst'
        list-push-struct                        \ ret-lst reg-lst0 reg-lnk tmp-lst' regx
        swap                                    \ ret-lst reg-lst0 reg-lnk regx tmp-lst'

        \ Subtract all regions, except itself.
        #3 pick                                 \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lst0
        foreach                                 \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk
            dup link-get-data                   \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy
            #3 pick                             \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy regx
            <>                                  \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk bool
            if
                \ Region is not the target region.
                dup link-get-data               \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy
                #2 pick                         \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy tmp-lst'
                region-list-any-intersection?   \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk bool
                if
                    \ There is a reason to subtract.
                    dup link-get-data           \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy
                    #2 pick                     \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk regy tmp-lst'
                    region-list-subtract-region \ ret-lst reg-lst0 reg-lnk regx tmp-lst' reg-lnk rslt-lst'

                    \ Replace the temp list with the new result.
                    rot                         \ ret-lst reg-lst0 reg-lnk regx reg-lnk rslt-lst' tmp-lst'
                    region-list-deallocate      \ ret-lst reg-lst0 reg-lnk regx reg-lnk rslt-lst'
                    swap                        \ ret-lst reg-lst0 reg-lnk regx rslt-lst' reg-lnk
                then
            then
        next
                                                \ ret-lst reg-lst0 reg-lnk regx tmp-lst'
        dup                                     \ ret-lst reg-lst0 reg-lnk regx tmp-lst' tmp-lst'
        #5 pick                                 \ ret-lst reg-lst0 reg-lnk regx tmp-lst' tmp-lst' ret-lst
        region-list-append-nosubs               \ ret-lst reg-lst0 reg-lnk regx tmp-lst'
        region-list-deallocate                  \ ret-lst reg-lst0 reg-lnk regx
        drop                                    \ ret-lst reg-lst0 reg-ln
    next
    drop
;
