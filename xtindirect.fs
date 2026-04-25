\ Support the indirect use of functions, due to a function being
\ referenced before being defined.
\
\ Source code reordering can sometimes solve this, but it is not always easy, or aesthetic, and can change with new code.
\
\ So a place holder is defined here, like: ' xtabort value <function-name>-xt
\
\ In a file before a function is defined, use a line like: "<function-name>-xt execute" instead of "<function-name>"
\
\ After the function is defined, add a line like: ' <function-name> to <function-name>-xt
\

: xtabort
    ." indirect xt not initialized"
    abort
;

' xtabort value session-set-current-domain-xt

' xtabort value session-get-number-domains-xt

' xtabort value session-calc-max-regions-xt

' xtabort value session-get-current-domain-xt

' xtabort value session-get-domains-xt

' xtabort value assert-tos-is-session-xt

' xtabort value session-update-points-xt


' xtabort value domain-get-all-bits-mask-xt

' xtabort value domain-get-ms-bit-mask-xt

' xtabort value domain-get-inst-id-xt

' xtabort value domain-get-max-region-xt

' xtabort value domain-get-current-state-xt

' xtabort value domain-get-current-action-xt

' xtabort value domain-get-sample-step-xt

' xtabort value domain-get-plan-fc-xt

' xtabort value domain-get-plan-bc-xt

' xtabort value domain-get-number-actions-xt

' xtabort value assert-tos-is-domain-xt

' xtabort value assert-nos-is-domain-xt

' xtabort value domain-get-num-bits-xt

' xtabort value domain-state-pair-complement-xt

' xtabort value domain-get-sample-xt

' xtabort value domain-get-parent-session-xt


' xtabort value action-get-inst-id-xt

' xtabort value assert-tos-is-action-xt

' xtabort value assert-nos-is-action-xt

' xtabort value action-make-need-xt

' xtabort value action-get-parent-domain-xt

' xtabort value action-get-logical-structure-xt

' xtabort value action-find-square-xt

' xtabort value action-get-corners-xt

' xtabort value action-state-confirmed-xt

' xtabort value action-update-existing-square-xt

' xtabort value action-add-sample-xt

' xtabort value .action-xt


' xtabort value planstep-list-push-xt

' xtabort value planstep-new-xt

' xtabort value planstep-set-forward-xt

' xtabort value planstep-set-number-unwanted-changes-xt

' xtabort value .planstep-xt

' xtabort value region-list-push-xt

' xtabort value assert-nos-is-group-xt


' xtabort value .regioncorrrate-xt

' xtabort value .abort-xt

' xtabort value .stack-structs-xt

' xtabort value regioncorr-list-copy-except-xt

' xtabort value regioncorr-list-deallocate-xt

' xtabort value assert-tos-is-regioncorr-xt

' xtabort value assert-nos-is-regioncorr-xt

' xtabort value regioncorr-get-list-xt

' xtabort value .regioncorr-list-xt

' xtabort value .regioncorr-xt


' xtabort value structinfo-list-project-deallocated-xt

' xtabort value structinfo-list-print-memory-use-xt

' xtabort value structinfo-get-mma-xt

' xtabort value structinfo-get-name-xt

' xtabort value structinfo-list-find-xt

' xtabort value structinfo-list-print-struct-list-xt

' xtabort value structinfo-list-deallocate-struct-list-xt

' xtabort value assert-nos-is-plan-xt

