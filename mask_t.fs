\ Test mask functions.

: mask-test-new
    \ test mask-new.
    #5 #4 mask-new              \ msk

    \ Test mask-str.
    dup mask-str                \ msk uc-addr 
    string@                     \ msk c-addr cnt
    s" 0101"                    \ msk c-addr cnt c-addr cnt
    str=
    false? abort" string not as expected"

    \ Test .mask.
    cr ." mask: " dup .mask     \ msk

    \ Test mask-deallocate.
    mask-deallocate
    structinfo-list-store structinfo-list-project-deallocated

    cr ." mask-test-new - Ok"
;

: mask-tests
    mask-test-new
;
