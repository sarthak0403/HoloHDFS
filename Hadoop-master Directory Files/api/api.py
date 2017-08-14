#!/usr/bin/env python
import csv
import json
import subprocess
import psutil
over='0.0'
under='0.0'

#!./flask/bin/python
#import psutil
from flask import Flask, jsonify
from flask import request
app = Flask(__name__)

@app.route('/workers', methods=['GET'])
def get_tasks():
        result = {}
        reader = csv.DictReader(open('/home/hduser/workersList.csv'))
        for row in reader:
                key = row.pop('hostname')
                if key in result:
                        # implement your duplicate row handling here
                        pass
                result[key] = row
        for key in result:
            cpucmd = 'ssh '+str(key)+' top -bn1 | grep "Cpu(s)" | sed "s/.*, *\([0-9.]*\)%* id.*/\\1/" | awk \'{print 100 - $1}\''
            cpu = subprocess.check_output(cpucmd, shell=True)
            result[key]['CPU']=cpu.rstrip()

        subprocess.Popen('/home/hduser/blockReport.sh',shell=True)
        util = open("/home/hduser/blockUtil.txt","r")
        i=1
        global over
        global under
        for line in util:
            if(i==1):
                over=line.rstrip()
                i=i+1
            else:
                under=line.rstrip()
        masterCPU=psutil.cpu_percent()
        masterCPU=str(masterCPU)
        return jsonify({'workers': result,'over':over,'under':under,'master':masterCPU})

@app.route('/launch', methods=['GET'])
def add_instnace():
    #from subprocess import call
    subprocess.Popen(['bash','/home/hduser/launchAWS.sh'])
    subprocess.Popen('/home/hduser/blockReport.sh',shell=True)
    return jsonify({'script':'called'})

@app.route('/destroy',methods=['GET'])
## Usage: /destroy?ip=0.0.0.0
def remove_instance():
    args = request.args
    #print (args)
    ip = args['ip']
    print (ip)
    subprocess.Popen('/home/hduser/terminateAWS.sh '+str(ip), shell=True)
    subprocess.Popen('/home/hduser/blockReport.sh',shell=True)
    return jsonify({'script':'called'})

@app.route('/startjob',methods=['GET'])
def start_job():
        subprocess.Popen('/home/hduser/startJob.sh',shell=True)
        return jsonify({'job':'started'})

@app.route('/stopjob',methods=['GET'])
def stop_job():
	subprocess.Popen('/home/hduser/stopJob.sh',shell=True)
	return jsonify({'job':'stopped'})


if __name__ == '__main__':
    #app.run(debug=True)
    app.run(host="0.0.0.0",debug=True)
