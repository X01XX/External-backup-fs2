#!/usr/bin/bash

# Given a directory of source files,

# Get a list of function names.

# Search the files for each function name.

# For function names that have only one reference, that is the function definition,
# and will be displayed.

echo "A function may be used only in a test file, to test the function (so may be deleted), or to test other functions (should be kept)."

srcdir=.

# Set work directory, should not exist.
wrkdir=work

# Clear/create work directory.
if [ -d $wrkdir ]
then
	rm -R $wrkdir
fi
mkdir $wrkdir

# Put each file into work directory.
/usr/bin/ls *.fs | /usr/bin/grep -v _t.fs | /usr/bin/xargs -n 1 /usr/bin/cat >> $wrkdir/source.fs
if [ -e $wrkdir/unused.txt ]
then
   /usr/bin/rm $wrkdir/unused.txt
fi

# Parse out function names.
functions=`/usr/bin/egrep -h '^\s*\:\s+' $wrkdir/source.fs | /usr/bin/sed -e 's/\s*\:\s*//g' | /usr/bin/sed -e 's/\s.*//' > $wrkdir/functions.txt`

# Preamble.
echo "Unused functions:"
echo
functions=`/usr/bin/cat $wrkdir/functions.txt`

# For each function name.
# Check the number of references for each function name.
for line in $functions
do
 	count=`/usr/bin/grep -- $line $wrkdir/source.fs | /usr/bin/grep -v ":' " | /usr/bin/grep -v -- $line-[^xt] | /usr/bin/grep -cv -- "$line:"`  # remove references to the function name in prints and aborts.
 	if [ $count -lt 2 ]
   	then
        echo "    " $line
        echo $line >> $wrkdir/unused.txt
 	fi
done
# End
echo
echo -n "Number unused functions: "
/usr/bin/cat $wrkdir/unused.txt | wc -l 
echo -n "Total functions: "
/usr/bin/cat $wrkdir/functions.txt | wc -l 

# Clean up.
rm -R $wrkdir

