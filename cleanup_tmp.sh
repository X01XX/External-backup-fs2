# Cleanup control characters from the file tmp.txt.
# Use after filling tmp.txt with output from the script command.
/usr/bin/tr -d '\000\033\007\010\027\r' < tmp.txt > tmp_wrk.txt
/usr/bin/dos2unix -n tmp_wrk.txt tmp.txt
/usr/bin/rm tmp_wrk.txt
/usr/bin/dos2unix tmp.txt
