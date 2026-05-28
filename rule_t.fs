
: rule-test-union
    s" 01/10/11/00/" rule-from-string-a  \ rul1
    s" 01/10/11/00/" rule-from-string-a  \ rul1 rul2
    2dup rule-union                     \ rul1 rul2, rul-u t | f
    if
        swap rule-deallocate            \ rul1 rul-u
        swap rule-deallocate            \ rul-u
        cr ." rule-union: " dup .rule cr
        rule-deallocate 
    else
        true abort" Rule union failed?"
    then

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." rule-test-union - Ok"
;

: rule-test-intersection
    s" 01/10/11/00/" rule-from-string-a  \ rul1
    s" 01/10/11/00/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection               \ rul1 rul2, rul-i t | f
    if
        swap rule-deallocate            \ rul1 rul-i
        swap rule-deallocate            \ rul-i
        cr ." rule-intersection: " dup .rule cr
        rule-deallocate 
    else
        true abort" Rule intersection failed?"
    then

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." rule-test-intersection - Ok"
;

: rule-tests
    rule-test-union
    rule-test-intersection
;
