date=`date "+%d-%b-%y"`
startTime=`date "+%H:%M:%S"`
## Lauch an instance using AMI and get the instance ID directly using the following AWS CLI command:
instanceId=`aws ec2 run-instances --image-id ami-41fee538 --count 1 --instance-type t2.micro --key-name hadoop-master --security-groups hadoop-cluster-sg --query 'Instances[0].InstanceId' --output text`

# AWS CLI command to get the url, private Ip and Public Ip of the new instance
instanceUrl=`aws ec2 describe-instances --instance-ids $instanceId --query 'Reservations[0].Instances[0].PublicDnsName' --output text`
pubIp=`aws ec2 describe-instances --instance-ids $instanceId --query 'Reservations[0].Instances[0].PublicIpAddress' --output text`
prvIp=`aws ec2 describe-instances --instance-ids $instanceId --query 'Reservations[0].Instances[0].PrivateIpAddress' --output text`

#aws ec2 describe-instances --instance-ids i-0c96aca89f7caa804

HostName=`aws ec2 describe-instances --instance-ids $instanceId --query 'Reservations[0].Instances[0].PrivateDnsName' --output text`
HostName=`echo "$HostName" | awk -F '[.]' '{print $1}'`

#wCount=`grep -oP '(?<=hadoop-worker)[0-9]+' workersList | tail -1`
#wCount=$(($wCount+1))
#echo "hadoop-worker$wCount $pubIp $prvIp" >> ~/workersList

echo "$HostName,$pubIp,$prvIp" >> ~/workersList.csv

#sudo echo "$prvIp $HostName" >> /etc/hosts
echo "$prvIp $HostName" | sudo tee --append /etc/hosts > /dev/null

sudo echo "$HostName" >> /usr/local/hadoop/etc/hadoop/slaves

#ssh-keygen -R $prvIp

ssh $HostName 'rm /usr/local/hadoop_tmp/hdfs/datanode/current/VERSION'

# Copy the master hosts file to each worker
while read worker; do
  #echo $worker  
  cat /etc/hosts | ssh $worker "sudo sh -c 'cat >/etc/hosts'"
  #/etc/init.d/network restart
done </usr/local/hadoop/etc/hadoop/slaves

start-dfs.sh && start-yarn.sh
endTime=`date "+%H:%M:%S"`
echo -e "$date,$startTime,$endTime" >> launchlog.csv
