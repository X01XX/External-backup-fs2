
\ For use outside of the GPL 3.0 license,
\ except for stack.fs, mm_array.fs, link.fs, list.fs, tools.fs, stackprint.fs, struct.fs,
\            structlist.fs, structinfo.fs, and structinfolist.fs,
\ contact the Wisconsin Alumni Research Foundation (WARF).

\ The Linux command: alias words='grep "^: "'
\ allows a quick list of functions in a file, like "words session.fs" or more, "words *list.fs"
\ Words can be filtered, like "words list.fs | grep remove" or "words list.fs | grep -- list-set-"
\ A brief comment on the same line as the function name can be helpful.
\ For a bash shell user, the command can be added at the end of ~/.bashrc to make it permanent.

\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime.
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ Link   List   Region Rule   RuleStore Square
\ 17137, 17971, 19317, 23131, 23173,    23197
\
\ Sample Action Session Domain Need   Changes
\ 23719, 29717, 31319,  31379, 19717, 31973
\
\ PlanStep   Plan   Group  Rate   RegionCorrRate  RegionCorr
\ 37171,     37379, 43717, 41719, 41737,          47317
\
\ RuleCorr  Changescorr PathStep Plancorr Corner Stack-info
\ 53171,    53173,      53197,   53717,   53719, 53731
\
\ Token
\ 59797
\
\ Struct ids not yet used:
\ 61379, 61717, 61719, 61979.

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

include mm_array.fs     \ includes stack.fs
include struct.fs
include link.fs
include list.fs
include structlist.fs

\ Application.
include globals.fs

include token.fs
include tokenlist.fs

include value.fs
include valuelist.fs

include region.fs
include region2.fs

include regionlist.fs

include changes.fs
include changeslist.fs
include changes2.fs
include changescorr.fs

include sample.fs

include rule.fs
include rulestore.fs
include rulelist.fs

include square.fs

include squarelist.fs

include need.fs
include needlist.fs

include corner.fs
include cornerlist.fs

include planstep.fs
include plansteplist.fs

include regioncorr.fs
include regioncorrlist.fs
include rate.fs
include regioncorrrate.fs
include regioncorrratelist.fs
include rulecorr.fs

include plan.fs
include planlist.fs

include plancorr.fs
include plancorrlist.fs

include pathstep.fs
include pathsteplist.fs

include group.fs
include grouplist.fs

include action.fs
include actionlist.fs
include actionxts.fs

include domain.fs
include domainlist.fs

include structinfo.fs
include structinfolist.fs
include stackprint.fs
include list2.fs

include session.fs

cs

\ Test files.
include square_t.fs
include corner_t.fs
include squarelist_t.fs
include region_t.fs
include regionlist_t.fs
include rule_t.fs
include action_t.fs
include rulestore_t.fs
include tokenlist_t.fs
include regioncorr_t.fs
include regioncorrlist_t.fs
include session_t.fs
include plan_t.fs
include domain_t.fs
include structinfolist_t.fs
include list_t.fs
include pathstep_t.fs
include pathsteplist_t.fs

cr ." main.fs"

\ Init array-stacks.
#6500 link-mma-init
#1902 list-mma-init
list-new to structinfo-list-store

#0030 structinfo-mma-init
' noop ' noop ' link-deallocate ' .link s" Link" link-mma link-id structinfo-new structinfo-list-store structinfo-list-push

' lists-eq? ' noop ' structinfo-list-deallocate-struct-list ' structinfo-list-print-struct-list s" List" list-mma list-id structinfo-new structinfo-list-store structinfo-list-push-end
' noop ' noop ' structinfo-deallocate ' .structinfo s" StructInfo" structinfo-mma structinfo-id structinfo-new structinfo-list-store structinfo-list-push-end

#0001 session-mma-init
' noop ' noop ' session-deallocate ' .session s" Session" session-mma session-id structinfo-new structinfo-list-store structinfo-list-push-end

#0025 domain-mma-init
' noop ' noop ' domain-deallocate ' .domain s" Domain" domain-mma domain-id structinfo-new structinfo-list-store structinfo-list-push-end

#0050 action-mma-init
' noop ' noop ' action-deallocate ' .action s" Action" action-mma action-id structinfo-new structinfo-list-store structinfo-list-push-end

#0100 group-mma-init
' noop ' noop ' group-deallocate ' .group s" Group" group-mma group-id structinfo-new structinfo-list-store structinfo-list-push-end

#0406 square-mma-init
' noop ' noop ' square-deallocate ' .square s" Square" square-mma square-id structinfo-new structinfo-list-store structinfo-list-push-end

#0870 pathstep-mma-init
' noop ' noop ' pathstep-deallocate ' .pathstep s" PathStep" pathstep-mma pathstep-id structinfo-new structinfo-list-store structinfo-list-push-end

#0040 plancorr-mma-init
' noop ' noop ' plancorr-deallocate ' .plancorr s" PlanCorr" plancorr-mma plancorr-id structinfo-new structinfo-list-store structinfo-list-push-end

#0150 plan-mma-init
' noop ' noop ' plan-deallocate ' .plan s" Plan" plan-mma plan-id structinfo-new structinfo-list-store structinfo-list-push-end

#0150 planstep-mma-init
' noop ' noop ' planstep-deallocate ' .planstep s" PlanStep" planstep-mma planstep-id structinfo-new structinfo-list-store structinfo-list-push-end

#0100 regioncorrrate-mma-init
' noop ' noop ' regioncorrrate-deallocate ' .regioncorrrate s" RegionCorrRate" regioncorrrate-mma regioncorrrate-id structinfo-new structinfo-list-store structinfo-list-push-end

#0600 regioncorr-mma-init
' regioncorrs-eq? ' noop ' regioncorr-deallocate ' .regioncorr s" RegionCorr" regioncorr-mma regioncorr-id structinfo-new structinfo-list-store structinfo-list-push-end

#0450 changescorr-mma-init
' noop ' noop ' changescorr-deallocate ' .changescorr s" ChangesCorr" changescorr-mma changescorr-id structinfo-new structinfo-list-store structinfo-list-push-end

#0404 rulecorr-mma-init
' noop ' noop ' rulecorr-deallocate ' .rulecorr s" RuleCorr" rulecorr-mma rulecorr-id structinfo-new structinfo-list-store structinfo-list-push-end

#0505 rulestore-mma-init
' noop ' noop ' rulestore-deallocate ' .rulestore s" RuleStore" rulestore-mma rulestore-id structinfo-new structinfo-list-store structinfo-list-push-end

#0200 corner-mma-init
' noop ' noop ' corner-deallocate ' .corner s" Corner" corner-mma corner-id structinfo-new structinfo-list-store structinfo-list-push-end

#0200 need-mma-init
' noop ' noop ' need-deallocate ' .need s" Need" need-mma need-id structinfo-new structinfo-list-store structinfo-list-push-end

#0250 sample-mma-init
' noop ' noop ' sample-deallocate ' .sample s" Sample" sample-mma sample-id structinfo-new structinfo-list-store structinfo-list-push-end

#0100 rate-mma-init
' noop ' noop ' rate-deallocate ' .rate s" Rate" rate-mma rate-id structinfo-new structinfo-list-store structinfo-list-push-end

#0650 changes-mma-init
' noop ' noop ' changes-deallocate ' .changes s" Changes" changes-mma changes-id structinfo-new structinfo-list-store structinfo-list-push-end

#0904 rule-mma-init
' rules-eq? ' rule-from-string ' rule-deallocate ' .rule s" Rule" rule-mma rule-id structinfo-new structinfo-list-store structinfo-list-push-end

#3003 region-mma-init
' regions-eq? ' region-from-string ' region-deallocate ' .region s" Region" region-mma region-id structinfo-new structinfo-list-store structinfo-list-push-end

#100  token-mma-init
' tokens-eq? ' noop ' token-deallocate ' .token s" Token" token-mma token-id structinfo-new structinfo-list-store structinfo-list-push-end
cr

\ Init a session.
: init-main ( -- sess )
    \ Set up session.
    session-new                                 \ sess

    \ Add domain 0
    #4 over  domain-new                         \ sess dom

    \ Add actions to domain 0
    [ ' domain-0-act-1-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-2-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-3-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-4-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-5-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-6-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    \ Add a domain
    over session-add-domain                     \ sess

    \ Add domain 1
    #5 over domain-new                          \ sess dom1

    \ Add actions to domain 1
    [ ' domain-1-act-1-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-2-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-3-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-4-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-5-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-6-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    \ Add last domain
    over session-add-domain                     \ sess dom1

    s" (rX1X1 r01X1X)" regioncorr-from-string-a   \ sess regc
    0 #2 rate-new                              \ sess regc rt
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (r1XXX r01XXX)" regioncorr-from-string-a   \ sess regc
    #-2 0 rate-new                              \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (r00XX r000XX)" regioncorr-from-string-a   \ sess regc
    #-1 0 rate-new                              \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (r01XX r11XXX)" regioncorr-from-string-a   \ sess regc
    #-3 0 rate-new                              \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (r00X1 r11XXX)" regioncorr-from-string-a   \ sess regc
    0 #2 rate-new                               \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    \ todo? Set current domain states.

    dup .session
;

0 value step-num
: main ( -- )
    init-main                                   \ sess

    \ Set first points value.
    dup session-update-points                   \ sess
    dup session-set-previous-points             \ sess

    0 to step-num
    true
    begin
    while
        \ Inc step num.

        step-num 1+ to step-num                         \ sess

        \ Print header.
        cr ." ***************************"
        cr ." Step: " step-num dec.
        space ." Current state: "
        dup .session-current-states                     \ sess

        dup session-get-current-rate                    \ sess rate

        space ." rate: "
        dup .rate                                       \ sess rate
        space ." Status: "
        dup rate-get-positive
        0= if
            \ No positive value.
            dup rate-get-negative
            0= if
                \ No negative value.
                ." Neutral"
            else
                 \ Some negative value.
                ." Negative"
            then
        else
            \ Some positive value.
            dup rate-get-negative
            0= if
                \ No negative value.
                ." Positive"
            else
                \ Some negative value.
                ." Conflicted"
            then
        then

        rate-deallocate                                 \ sess

        \ Display points.
        dup session-get-points                          \ sess pnts
        space ." points: " dup dec.
        over session-get-previous-points                \ sess pnts ppnts
        -                                               \ sess dif
        ." change: " dec.                               \ sess
        dup session-set-previous-points                 \ sess
        cr

        dup session-get-user-input                      \ sess bool
        \ cr .s cr
        depth #2 <>
        if
            ." depth not equal two? " .s
            abort
        then
    repeat
    cr ." at end of session: " .stack-gbl cr

    \ Clean up                                          \ sess

    \ Print memory use before deallocating.
    cr structinfo-list-store structinfo-list-print-memory-use

    cr ." Deallocating ..." cr
    session-deallocate                                  \

    \ Print memory use after deallocating.
    cr structinfo-list-store structinfo-list-print-memory-use

    \ Check for memory leak, or something on the Forth stack.
    structinfo-list-store structinfo-list-project-deallocated-xt execute

    \ Free heap.
    \ structinfo-list-store structinfo-list-free-heap
;

: all-tests
    structinfo-list-store structinfo-list-project-deallocated-xt execute

    region-tests
    region-list-tests
    rule-tests
    rulestore-tests
    token-list-tests
    square-tests
    square-list-tests
    regioncorr-tests
    regioncorr-list-tests
    plan-tests
    structinfolist-tests
    list-tests
    corner-tests
    pathstep-tests
    pathsteplist-tests
    action-tests
    domain-tests
    session-tests

    structinfo-list-store structinfo-list-project-deallocated-xt execute
;
