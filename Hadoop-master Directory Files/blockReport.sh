hdfs fsck / -files -blocks | grep -E 'Under-replicated blocks:|Over-replicated blocks:' | cut -d "(" -f2 | cut -d " " -f1 > /home/hduser/blockUtil.txt
