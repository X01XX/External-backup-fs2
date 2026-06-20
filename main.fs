\ Start a clean vocabulary.
cr ." Starting vocabulary UES," cr
vocabulary UES

\ Put new words into the UES vocabulary.
UES definitions

decimal
\ #2 base !  \ Test all numbers GT 1, LT -1, have a base prefix.

include xtindirect.fs
include bool.fs

include tools.fs

include mm_array.fs
include struct.fs
include link.fs
include list.fs
include structlist.fs

include globals.fs

include mask.fs
include masklist.fs

include state.fs


include region.fs
include statelist.fs
include regionlist.fs
include region2.fs
include state2.fs
include regionlist2.fs

include sample.fs
include samplelist.fs
include rule.fs
include rulelist.fs
include floatnum.fs

include token.fs
include tokenlist.fs

include structinfo.fs
include structinfolist.fs
include stackprint.fs
include list2.fs

include square.fs
include squarelist.fs

include action.fs
include corner.fs

cr

include mask_t.fs
include state_t.fs
include statelist_t.fs
include region_t.fs
include rule_t.fs
include sample_t.fs
include regionlist_t.fs
include square_t.fs
include corner_t.fs
include squarelist_t.fs
include action_t.fs

\ Init array-stacks.
#101 link-mma-init
#102 list-mma-init
#030 structinfo-mma-init
#100 mask-mma-init
#100 state-mma-init
#100 region-mma-init
#100 rule-mma-init
#100 sample-mma-init
#100 token-mma-init
#100 floatnum-mma-init
#100 square-mma-init
#010 action-mma-init
#010 corner-mma-init
cr cr

\ Init structinfo list.
list-new to structinfo-list-store
' noop          ' noop                  ' link-deallocate       ' .link     s" Link"        link-mma        link-id structinfo-new structinfo-list-store structinfo-list-push
' lists-eq?     ' noop                  ' structinfo-list-deallocate-struct-list ' structinfo-list-print-struct-list s" List" list-mma list-id structinfo-new structinfo-list-store structinfo-list-push-end
' noop          ' noop                  ' structinfo-deallocate ' .structinfo s" StructInfo" structinfo-mma structinfo-id structinfo-new structinfo-list-store structinfo-list-push-end

\ The list, link, and StructInfo structs allow for the creation of the structinfo-list-store,

' masks-eq?     ' mask-from-string      ' mask-deallocate       ' .mask     s" Mask"        mask-mma        mask-id     structinfo-new structinfo-list-store structinfo-list-push-end
' states-eq?    ' state-from-string     ' state-deallocate      ' .state    s" State"       state-mma       state-id    structinfo-new structinfo-list-store structinfo-list-push-end
' regions-eq?   ' region-from-string    ' region-deallocate     ' .region   s" Region"      region-mma      region-id   structinfo-new structinfo-list-store structinfo-list-push-end
' floatnums-eq? ' floatnum-from-string  ' floatnum-deallocate   ' .floatnum s" FloatNum"    floatnum-mma    floatnum-id structinfo-new structinfo-list-store structinfo-list-push-end
' tokens-eq?    ' noop                  ' token-deallocate      ' .token    s" Token"       token-mma       token-id    structinfo-new structinfo-list-store structinfo-list-push-end
' rules-eq?     ' rule-from-string      ' rule-deallocate       ' .rule     s" Rule"        rule-mma        rule-id     structinfo-new structinfo-list-store structinfo-list-push-end
' samples-eq?   ' sample-from-string    ' sample-deallocate     ' .sample   s" Sample"      sample-mma      sample-id   structinfo-new structinfo-list-store structinfo-list-push-end
' noop          ' noop                  ' action-deallocate     ' .action   s" Action"      action-mma      action-id   structinfo-new structinfo-list-store structinfo-list-push-end
' noop          ' noop                  ' corner-deallocate     ' .corner   s" Corner"      corner-mma      corner-id   structinfo-new structinfo-list-store structinfo-list-push-end
' =             ' noop                  ' square-deallocate     ' .square   s" Square"      square-mma      square-id   structinfo-new structinfo-list-store structinfo-list-push-end

: main

    cr cr
    s" (s1010->s0111 (1 rX001) (3 m1010) (5 s1000) 01/10/XX/Xx/ 4.2e)"
    cr 2dup ." list string: " [char] " emit type [char] " emit cr
    list-from-string        \ lst t | f
    if
        cr ." List: " dup structinfo-list-print-struct-list
    else
        cr ." list-from-string failed" cr
        abort
    then

    cr cr
    s" (r1001 r00000 r101)"
    cr 2dup ." Region list string: " [char] " emit type [char] " emit cr
    region-list-from-string        \ lst t | f
    if
        cr ." List: " dup structinfo-list-print-struct-list
    else
        cr ." list-from-string failed" cr
        abort
    then

    s" r1001" region-from-string
    invert abort" region-from-string: failed"
    s" rX0X1" region-from-string
    invert abort" region-from-string: failed"
    2dup region-subtract            \ reg2 reg1 reg-lst
    cr cr ." rX0X1 - r1001 = " dup .region-list cr

    \ Finish.
    cr structinfo-list-store structinfo-list-print-memory-use cr

    \ Deallocate remaining struct instances.
    cr ." Deallocating ..."

    region-list-deallocate
    region-deallocate
    region-deallocate
    region-list-deallocate
    structinfo-list-deallocate-struct-list

    cr structinfo-list-store structinfo-list-print-memory-use cr

    structinfo-list-store structinfo-list-project-deallocated
;

: free-heap
    \ Free heap memory before exiting.
    ." Freeing heap memory"
    structinfo-list-store structinfo-list-free-heap
    bye
;

: all-tests
    structinfo-list-store structinfo-list-project-deallocated
    mask-tests
    state-tests
    region-tests
    rule-tests
    sample-tests
    state-list-tests
    region-list-tests
    corner-tests
    square-list-tests
    action-tests
    cr
;

