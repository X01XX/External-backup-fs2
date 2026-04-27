
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

' mask-deallocate ' f. s" Mask" mask-mma mask-id structinfo-new structinfo-list-store structinfo-list-push-end
' state-deallocate ' f. s" State" state-mma state-id structinfo-new structinfo-list-store structinfo-list-push-end
' region-deallocate ' f. s" Region" region-mma region-id structinfo-new structinfo-list-store structinfo-list-push-end
' rule-deallocate ' f. s" Rule" rule-mma rule-id structinfo-new structinfo-list-store structinfo-list-push-end
' sample-deallocate ' f. s" Sample" sample-mma sample-id structinfo-new structinfo-list-store structinfo-list-push-end

$d #4 state-new             \ msk0'
$5 #4 state-new             \ msk0' msk1'
2dup rule-new               \ msk0' msk1' rul0'

cr cr ." rule: " dup .rule cr

\ Finish.
cr structinfo-list-store structinfo-list-print-memory-use cr

\ Deallocate remaining struct instances.
cr ." Deallocating ..."
rule-deallocate
state-deallocate
state-deallocate

cr structinfo-list-store structinfo-list-print-memory-use cr

structinfo-list-store structinfo-list-project-deallocated

\ Free heap memory before exiting.
." Freeing heap memory"
structinfo-list-store structinfo-list-free-heap
cr
