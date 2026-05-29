
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
    s" 10/01/00/11_10/01/00/11/" rule-from-string-a \ rul1
    s" Xx/Xx/XX/XX_X0/X1/X0/X1/" rule-from-string-a \ rul1 rul2
    s" X0/X1/X0/X1_Xx/Xx/XX/XX/" rule-from-string-a \ rul1 rul2 rul3
    2dup rule-intersection                          \ rul1 rul2 rul3, rul-i t | f
    if
        swap rule-deallocate            \ rul1 rul2 rul-i
        swap rule-deallocate            \ rul1 rul-i
        cr ." rule-intersection: " dup .rule cr
        2dup rules-eq?                  \ rul1 rul-i bool
        if
        else
            ." rules ne?"
            abort
        then
        rule-deallocate                 \ rul1
        rule-deallocate                 \ 
    else
        true abort" rule-intersection 1 failed?"
    then

    s" Xx/" rule-from-string-a  \ rul1
    s" XX/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection      \ rul1 rul2, rul-u t | f
    abort" rule-intersection 2 succeded?"

    swap                        \ rul2 rul1
    2dup rule-intersection      \ rul2 rul1, rul-u t | f
    abort" rule-intersection 3 succeded?"
    rule-deallocate             \ rul2
    rule-deallocate             \

    s" X0/" rule-from-string-a  \ rul1
    s" X1/" rule-from-string-a  \ rul1 rul2
    2dup rule-intersection      \ rul1 rul2, rul-u t | f
    abort" rule-intersection 4 succeded?"

    swap                        \ rul2 rul1
    2dup rule-intersection      \ rul2 rul1, rul-u t | f
    abort" rule-intersection 4 succeded?"

    rule-deallocate             \ rul2
    rule-deallocate             \

    \ Check for memory leaks.
    structinfo-list-store structinfo-list-project-deallocated

    cr ." rule-test-intersection - Ok"
;

: rule-tests
    rule-test-union
    rule-test-intersection
;
