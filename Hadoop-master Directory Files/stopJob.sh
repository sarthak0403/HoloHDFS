date=`date "+%d-%b-%y"`
startTime=`date "+%H:%M:%S"`
for i in  `mapred job -list | grep -w hduser | awk '{print $1}' | grep job_` ;  do mapred job -kill $i ;  done ;
hdfs dfs -rmdir /user/hololens/output
endTime=`date "+%H:%M:%S"`
echo -e "$date,$startTime,$endTime" >> stopJobLog.csv
