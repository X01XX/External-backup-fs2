
: rule-test-union
    s" 00/00/00/01/01/01/" rule-from-string-a  \ rul1
    s" 00/11/10/01/11/10/" rule-from-string-a  \ rul1 rul2
    2dup rule-union                     \ rul1 rul2, rul-u t | f
    if
        swap rule-deallocate            \ rul1 rul-u
        swap rule-deallocate            \ rul-u
        \ cr ." rule-union: " dup .rule
        rule-deallocate 
    else
        true abort" Rule union 1 failed?"
    then

    s" 11/11/11/10/10/10/" rule-from-string-a  \ rul1
    s" 11/00/01/10/00/01/" rule-from-string-a  \ rul1 rul2
    2dup rule-union                     \ rul1 rul2, rul-u t | f
    if
        swap rule-deallocate            \ rul1 rul-u
        swap rule-deallocate            \ rul-u
        \ cr ." rule-union: " dup .rule
        rule-deallocate 
    else
        true abort" Rule union 2 failed?"
    then

    s" 00/" rule-from-string-a  \ rul1
    s" 01/" rule-from-string-a  \ rul1 rul2
    2dup rule-union                     \ rul1 rul2, rul-u t | f
    abort" rule-union 3 succeded?"
    rule-deallocate            \ rul1 rul-u
    rule-deallocate            \ rul-u

    s" 11/" rule-from-string-a  \ rul1
    s" 10/" rule-from-string-a  \ rul1 rul2
    2dup rule-union                     \ rul1 rul2, rul-u t | f
    abort" rule-union 4 succeded?"
    rule-deallocate            \ rul1 rul-u
    rule-deallocate            \ rul-u

    cr
    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." rule-test-union - Ok"
;

: rule-test-intersection
    s" Xx/Xx/XX/XX/" rule-from-string-a  \ rul1
    s" X0/X1/X0/X1/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection               \ rul1 rul2, rul-i t | f
    if
        swap rule-deallocate            \ rul1 rul-i
        swap rule-deallocate            \ rul-i
        \ cr ." rule-intersection: " dup .rule cr
        rule-deallocate 
    else
        true abort" Rule intersection 1 failed?"
    then

    s" Xx/" rule-from-string-a  \ rul1
    s" XX/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection      \ rul1 rul2, rul-u t | f
    abort" rule-intersection 2 succeded?"
    rule-deallocate            \ rul1 rul-u
    rule-deallocate            \ rul-u

    s" X0/" rule-from-string-a  \ rul1
    s" X1/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection      \ rul1 rul2, rul-u t | f
    abort" rule-intersection 3 succeded?"
    rule-deallocate            \ rul1 rul-u
    rule-deallocate            \ rul-u

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." rule-test-intersection - Ok"
;

: rule-tests
    rule-test-union
    rule-test-intersection
;
