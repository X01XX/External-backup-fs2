\ Start a clean vocabulary.
cr ." Starting vocabulary UES," cr
vocabulary UES

true constant debug

\ Put new words into the UES vocabulary.
UES definitions

decimal
\ #2 base !  \ Test all numbers GT 1, LT -1, have a base prefix.

\ include /usr/share/gforth/0.7.3/objects.fs
include xtindirect.fs
include globals.fs
include bool.fs
include tools.fs
include mm_array.fs
include struct.fs
include link.fs
include list.fs
include structlist.fs
include mask.fs
include masklist.fs

include state.fs

include region.fs
include statelist.fs
include regionlist.fs
include incpairs.fs
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

include regioncorr.fs
include regioncorrlist.fs

include structinfo.fs
include structinfolist.fs
include stackprint.fs
include list2.fs

include square.fs
include squarelist.fs

include group.fs
include grouplist.fs

include corner.fs
include cornerlist.fs

\ include need.fs
\ include needlist.fs

include actionxts.fs

include action.fs

include actionlist.fs

include frame.fs
include domain.fs
include domainlist.fs

include session.fs

cr

include mask_t.fs
include state_t.fs
include statelist_t.fs
include region_t.fs
include rule_t.fs
include sample_t.fs
include regionlist_t.fs
include regioncorr_t.fs
include regioncorrlist_t.fs
include square_t.fs
include corner_t.fs
\ include need_t.fs
include cornerlist_t.fs
include squarelist_t.fs
include action_t.fs
include group_t.fs
include incpairs_t.fs
include domain_t.fs
include session_t.fs

\ Init array-stacks.
#301 link-mma-init
#302 list-mma-init
#030 structinfo-mma-init
#200 mask-mma-init
#200 state-mma-init
#200 region-mma-init
#200 rule-mma-init
#200 sample-mma-init
#100 token-mma-init
#100 floatnum-mma-init
#200 square-mma-init
#010 action-mma-init
#110 corner-mma-init
\ #110 need-mma-init
#130 group-mma-init
#100 regioncorr-mma-init
#010 frame-mma-init
#010 domain-mma-init
#005 session-mma-init
cr cr

\ Init structinfo list.
list-new to structinfo-list-store
' noop  ' noop  ' noop          ' noop                  ' link-deallocate       ' .link         s" Link"        link-mma        link-struct-id          structinfo-new structinfo-list-store-push
' noop  ' noop  ' lists-eq?     ' noop                  ' struct-list-deallocate ' .struct-list s" List"        list-mma        list-struct-id          structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' noop                  ' structinfo-deallocate ' .structinfo   s" StructInfo"  structinfo-mma  structinfo-struct-id    structinfo-new structinfo-list-store-push-end

\ The list, link, and StructInfo structs allow for the creation of the structinfo-list-store,

' noop  ' noop  ' masks-eq?     ' mask-from-string      ' mask-deallocate       ' .mask         s" Mask"        mask-mma        mask-struct-id          structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' states-eq?    ' state-from-string     ' state-deallocate      ' .state        s" State"       state-mma       state-struct-id         structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' regions-eq?   ' region-from-string    ' region-deallocate     ' .region       s" Region"      region-mma      region-struct-id        structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' floatnums-eq? ' floatnum-from-string  ' floatnum-deallocate   ' .floatnum     s" FloatNum"    floatnum-mma    floatnum-struct-id      structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' tokens-eq?    ' noop                  ' token-deallocate      ' .token        s" Token"       token-mma       token-struct-id         structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' rules-eq?     ' rule-from-string      ' rule-deallocate       ' .rule         s" Rule"        rule-mma        rule-struct-id          structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' samples-eq?   ' sample-from-string    ' sample-deallocate     ' .sample       s" Sample"      sample-mma      sample-struct-id        structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' noop                  ' action-deallocate     ' .action       s" Action"      action-mma      action-struct-id        structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' corner-from-string    ' corner-deallocate     ' .corner       s" Corner"      corner-mma      corner-struct-id        structinfo-new structinfo-list-store-push-end
' regioncorr-from-list  ' regioncorr-valid-list?    ' noop  ' noop  ' regioncorr-deallocate ' .regioncorr   s" Regioncorr"  regioncorr-mma  regioncorr-struct-id    structinfo-new structinfo-list-store-push-end
\ ' noop  ' noop  ' noop          ' noop                  ' need-deallocate       ' .need       s" Need"        need-mma        need-struct-id          structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' =             ' noop                  ' square-deallocate     ' .square       s" Square"      square-mma      square-struct-id        structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' =             ' noop                  ' group-deallocate      ' .group        s" Group"       group-mma       group-struct-id         structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' noop                  ' frame-deallocate      ' .frame        s" Frame"       frame-mma       frame-struct-id         structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' noop                  ' domain-deallocate     ' .domain       s" Domain"      domain-mma      domain-struct-id        structinfo-new structinfo-list-store-push-end
' noop  ' noop  ' noop          ' noop                  ' session-deallocate    ' .session      s" Session"     session-mma     session-struct-id       structinfo-new structinfo-list-store-push-end

: main
    session-new                     \ sess

    #4 over session-add-domain      \ sess dom
    drop

    #6 over session-add-domain      \ sess dom
    drop

    dup session-init-after-domains  \ sess

    cr dup .session cr

    true
    begin
    while
                                                        \ sess

        \ Print header.
        cr ." ***************************"
        cr ." Step: " dup session-get-step-num dec.
        space ." Current state: "
        dup .session-current-states                     \ sess

        dup session-get-user-input                      \ sess bool ( t  = continue )
    repeat

    \ Finish.
    cr .memory-use cr

    \ Clean up.
    cr ." Deallocating ..."
    session-deallocate

    \ Finish.
    cr .memory-use cr

    \ Check for memory leaks.
    check-project-deallocated
;

: free-heap
    \ Free heap memory before exiting.
    ." Freeing heap memory"
    structinfo-list-store structinfo-list-free-heap
    bye
;

: all-tests
     check-project-deallocated
    mask-tests
    state-tests
    region-tests
    rule-tests
    sample-tests
    state-list-tests
    region-list-tests
    square-list-tests
    action-tests
    group-tests
    domain-tests
    corner-tests
    corner-list-tests
\    need-tests
    inc-pair-tests
    regioncorr-tests
    regioncorr-list-tests
    session-tests
    cr
;

