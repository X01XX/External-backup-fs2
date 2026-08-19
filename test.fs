: get-first-word ( addr -- w  t | f )
    try
      @
      true
    restore
     cr .s cr
     2drop false
    endtry
;
