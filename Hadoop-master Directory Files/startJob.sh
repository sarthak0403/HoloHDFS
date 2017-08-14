date=`date "+%d-%b-%y"`
startTime=`date "+%H:%M:%S"`
hadoop jar ~/hadoop-mapreduce-examples.jar wordcount /user/hololens/input /user/hololens/output
endTime=`date "+%H:%M:%S"`
echo -e "$date,$startTime,$endTime" >> startJobLog.csv
