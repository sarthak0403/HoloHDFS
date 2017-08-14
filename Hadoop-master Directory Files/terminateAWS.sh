date=`date "+%d-%b-%y"`
startTime=`date "+%H:%M:%S"`
instanceId=`aws ec2 describe-instances --filters "Name=private-ip-address,Values=$1" --query 'Reservations[0].Instances[0].InstanceId' --output text`
#echo $instanceId
#if [[ ! -z $instanceId ]]
#then
#exit 1;
#fi

HostName=`aws ec2 describe-instances --instance-ids $instanceId --query 'Reservations[0].Instances[0].PrivateDnsName' --output text`
HostName=`echo "$HostName" | awk -F '[.]' '{print $1}'`

sed -i "/\b$HostName\b/d" /usr/local/hadoop/etc/hadoop/slaves
sudo sed -i "/\b$HostName\b/d" /etc/hosts
sudo sed -i "/\b$HostName\b/d" /home/hduser/workersList.csv

# Copy the master hosts file to each worker
while read worker; do
  cat /etc/hosts | ssh $worker "sudo sh -c 'cat >/etc/hosts'"
done < /usr/local/hadoop/etc/hadoop/slaves

echo $HostName >> /usr/local/hadoop/dfs.exclude

hdfs dfsadmin -refreshNodes

aws ec2 terminate-instances --instance-ids $instanceId
endTime=`date "+%H:%M:%S"`
echo -e "$date,$startTime,$endTime" >> terminatelog.csv

