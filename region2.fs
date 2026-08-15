\ Return a region-list by subtraction,
\ given reg-x over 0, and reg-x over 1, masks.
: _region-subtract ( x0-msk x1-msk reg0 -- reg-lst )

    \ Change x over 1 positions to 0, one position at a time.

    \ Init return list.
    list-new                        \ x0-msk x1-msk reg0 ret-lst

    \ Process x-over-1 mask.
    rot mask-copy                   \ x0-msk reg0 ret-lst x1-msk'

    begin
        dup mask-split-lsb          \ x0-msk reg0 ret-lst x1-msk', x1-msk-rem' x1-lsb t | f
    while
        rot mask-deallocate         \ x0-msk reg0 ret-lst | x1-msk-rem' x1-lsb'
        dup                         \ x0-msk reg0 ret-lst | x1-msk-rem' x1-lsb' x1-lsb'
        #4 pick                     \ x0-msk reg0 ret-lst | x1-msk-rem' x1-lsb' x1-lsb' reg0
        region-x-to-0               \ x0-msk reg0 ret-lst | x1-msk-rem' x1-lsb' reg0'
        swap mask-deallocate        \ x0-msk reg0 ret-lst | x1-msk-rem' reg0'
        #2 pick region-list-push    \ x0-msk reg0 ret-lst | x1-msk-rem'
    repeat
    mask-deallocate                 \ x0-msk reg0 ret-lst

    \ Process x-over-0 mask.
    rot mask-copy                   \ reg0 ret-lst x0-msk'

    begin
        dup mask-split-lsb          \ reg0 ret-lst x0-msk', x0-rem' x0-lsb t | f
    while
        rot mask-deallocate         \ reg0 ret-lst | x0-rem' x0-lsb'
        dup                         \ reg0 ret-lst | x0-rem' x0-lsb' x0-lsb'
        #4 pick                     \ reg0 ret-lst | x0-rem' x0-lsb' x0-lsb' reg0
        region-x-to-1               \ reg0 ret-lst | x0-rem' x0-lsb' reg0'
        swap mask-deallocate        \ reg0 ret-lst | x0-rem' reg0'
        #2 pick region-list-push    \ reg0 ret-lst | x0-rem'
    repeat
    mask-deallocate                 \ reg0 ret-lst

    nip                             \ ret-lst
;

\ Return a region-list from a TOS region minus the NOS region.
: region-subtract ( reg1 reg0 -- reg-lst )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    assert( 2dup regions-same-num-bits? )

    \ Check if any subtraction is needed.
    2dup regions-intersect?         \ reg1 reg0 flag
    ifnot
        list-new tuck               \ reg1 ret-lst reg0 ret-lst
        region-list-push-xt execute \ reg1 ret-lst
        nip                         \ ret-lst
        exit
    then

    \ Check if the result is nothing.
    2dup swap                       \ reg1 reg0 reg0 reg1
    region-superset?                \ reg1 reg0 flag
    if
        2drop
        list-new
        exit
    then

    \ Get X-over-0 mask.
    dup region-calc-x-mask      \ reg1 reg0 x-msk'
    rot                         \ reg0 x-msk' reg1
    dup region-calc-0-mask dup  \ reg0 x-msk' reg1 0-msk' 0-msk'
    #3 pick                     \ reg0 x-msk' reg1 0-msk'
    mask-and                    \ reg0 x-msk' reg1 0-msk' x0-msk'
    swap mask-deallocate        \ reg0 x-msk' reg1 x0-msk'

    \ Get X-over-1 mask.
    swap region-calc-1-mask dup \ reg0 x-msk' x0-msk' 1-msk' 1-msk'
    #3 pick                     \ reg0 x-msk' x0-msk' 1-msk' 1-msk' x-msk'
    mask-and                    \ reg0 x-msk' x0-msk' 1-msk' x1-msk'
    swap mask-deallocate        \ reg0 x-msk' x0-msk' x1-msk'
    rot mask-deallocate         \ reg0 x0-msk' x1-msk'

    \ Do subtraction.
    2dup                        \ reg0 x0-msk' x1-msk' x0-msk' x1-msk'
    #4 pick                     \ reg0 x0-msk' x1-msk' x0-msk' x1-msk' reg0
    _region-subtract            \ reg0 x0-msk' x1-msk' reg-lst

    \ Clean up.
    swap mask-deallocate        \ reg0 x0-msk' reg-lst
    swap mask-deallocate        \ reg0 reg-lst
    nip                         \ reg-lst
;

' region-subtract to region-subtract-xt

\ Return a region-list from a TOS region minus the NOS state.
: region-subtract-state ( sta1 reg0 -- region-list )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-state? )
    assert( over state-get-num-bits over region-get-num-bits = )

    \ Check if any subtraction is needed.
    2dup region-superset-of-state?  \ sta1 reg0 | flag
    ifnot
        nip                         \ reg0
        list-new tuck               \ ret-lst reg0 ret-lst
        region-list-push            \ ret-lst
        exit
    then

    \ Check if the result is nothing.
    dup region-calc-x-mask          \ sta1 reg0 | x-msk'
    dup mask-is-zero?               \ sta1 reg0 | x-msk' bool
    if
        mask-deallocate             \ sta1 reg0
        2drop                       \
        list-new                    \ ret-lst
        exit
    then

    \ Get X-over-0 mask.
    #2 pick state-invert-to-mask    \ sta1 reg0 | x-msk' 0-msk'
    2dup                            \ sta1 reg0 | x-msk' 0-msk' x-msk' 0-msk'
    mask-and                        \ sta1 reg0 | x-msk' 0-msk' x0-msk'
    swap mask-deallocate            \ sta1 reg0 | x-msk' x0-msk'

    \ Get X-over-1 mask.
    swap                            \ sta1 reg0 | x0-msk' x-msk'
    dup                             \ sta1 reg0 | x0-msk' x-msk' x-msk'
    #4 pick                         \ sta1 reg0 | x0-msk' x-msk' x-msk' sta1
    state-and-mask-to-mask          \ sta1 reg0 | x0-msk' x-msk' x1-msk'
    swap mask-deallocate            \ sta1 reg0 | x0-msk' x1-msk'

    \ Subtract.
    2dup                            \ sta1 reg0 | x0-msk' x1-msk' x0-msk' x1-msk'
    #4 pick                         \ sta1 reg0 | x0-msk' x1-msk' x0-msk' x1-msk' reg0
    _region-subtract                \ sta1 reg0 | x0-msk' x1-msk' ret-lst

    \ Clean up.
    swap mask-deallocate            \ sta1 reg0 | x0-msk' ret-lst
    swap mask-deallocate            \ sta1 reg0 | ret-lst
    nip nip                         \ ret-lst
;
