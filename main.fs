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
include state.fs
include statelist.fs

include region.fs
include regionlist.fs
include rule.fs
include sample.fs

include structinfo.fs
include structinfolist.fs
include stackprint.fs
cs

include mask_t.fs
include state_t.fs
include region_t.fs

\ Init array-stacks.
#101 link-mma-init
#102 list-mma-init
#010 structinfo-mma-init
#100 mask-mma-init
#100 state-mma-init
#100 region-mma-init
#100 rule-mma-init
#100 sample-mma-init

\ Init structinfo list.
list-new to structinfo-list-store
' link-deallocate ' .link s" Link" link-mma link-id structinfo-new structinfo-list-store structinfo-list-push
' structinfo-list-deallocate-struct-list ' structinfo-list-print-struct-list s" List" list-mma list-id structinfo-new structinfo-list-store structinfo-list-push-end
' structinfo-deallocate ' .structinfo s" StructInfo" structinfo-mma structinfo-id structinfo-new structinfo-list-store structinfo-list-push-end

\ The list, link, and StructInfo structs allow for the creation of the structinfo-list-store,

' mask-deallocate '     .mask   s" Mask"    mask-mma    mask-id     structinfo-new structinfo-list-store structinfo-list-push-end
' state-deallocate '    .state  s" State"   state-mma   state-id    structinfo-new structinfo-list-store structinfo-list-push-end
' region-deallocate '   .region s" Region"  region-mma  region-id   structinfo-new structinfo-list-store structinfo-list-push-end
' rule-deallocate '     .rule   s" Rule"    rule-mma    rule-id     structinfo-new structinfo-list-store structinfo-list-push-end
' sample-deallocate '   .sample s" Sample"  sample-mma  sample-id   structinfo-new structinfo-list-store structinfo-list-push-end

: main
    $d #4 state-new             \ msk0'
    $5 #4 state-new             \ msk0' msk1'
    sample-new                  \ msk0' msk1' smp0'

    cr cr ." sample: " dup .sample cr

    \ Finish.
    cr structinfo-list-store structinfo-list-print-memory-use cr

    \ Deallocate remaining struct instances.
    cr ." Deallocating ..."
    sample-deallocate
    \ state-deallocate
    \ state-deallocate

    cr structinfo-list-store structinfo-list-print-memory-use cr

    structinfo-list-store structinfo-list-project-deallocated

    \ Free heap memory before exiting.
    ." Freeing heap memory"
    structinfo-list-store structinfo-list-free-heap
    cr
;

: all-tests
    structinfo-list-store structinfo-list-project-deallocated
    mask-tests
    state-tests
    region-tests
;

