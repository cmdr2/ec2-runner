prev_log="0"
if [ -f ".prev_log_id" ]; then
 prev_log=$(cat .prev_log_id)
 echo "Most recent log file at last_log.txt (log id: $prev_log)"
 echo ""
fi

newest_log=$prev_log

tried=0
sleep_time=10

while [ $newest_log == $prev_log ]
do
  time_elapsed=$(($tried*$sleep_time))
  echo "Checking for new logs ($time_elapsed seconds elapsed).."

  newest_log=$(aws s3 ls s3://me.cmdr2.org/damla/logs/ | sort -r | head -1 | awk '{print $4}')
  if [ $newest_log != $prev_log ]; then
    echo "Found new log: $newest_log"
    break
  fi

  sleep 10

  if [ $tried -gt 1000 ]
  then
    echo "Waited for 10,000 seconds, no new logs found. Maybe the task died?"
    exit
  fi
  tried=$(($tried+1))
done

aws s3 cp s3://me.cmdr2.org/damla/logs/$newest_log last_log.txt

echo ""

cat last_log.txt

echo $newest_log > .prev_log_id
