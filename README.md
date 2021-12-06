# ec2-runner

Remote runner code for a lambda-like experience, for running code on EC2 instances. Good for running GPU stuff without owning an expensive GPU.

Designed for programming -> deploying and running on EC2 -> analyzing the results -> and returning to programming some more, with as little overhead in the programming feedback loop.

**Note:** this is really hardcoded for a specific project, but can be generalized.

### Overhead for each task run:
- 3 min overhead for the first task in a session (for starting a new instance)
- 2 min overhead for every subsequent task in a session (for restarting the instance)

Note: the overhead varies slightly. These measurements are for t2.large. For t2.micro, the overheads were about 30s-1min more.

### Costs:
The EC2 cost depends on the instance type and how much it's used, obviously. The EBS cost shouldn't be more than $1.2 per month, assuming a 30gb SSD volume is used.

## Commands:
* `build.sh` - builds Parallel.exe from the Unity Project
* `upload.sh` - uploads the zip to S3, and starts an EC2 Windows instance, 

* `new-server.sh` - creates an EC2 Windows instance from a launch template in the AWS Console. A user-data script executes the task, uploads the run log to S3, and stops the instance.
* `restart-server.sh` - restarts an exist EC2 Windows instance from the `.instance_id` file in the local directory, created by the `new-server.sh` script. This runs the task and uploads the log as usual.
* `fetch-latest-logs.sh` - keeps polling until it can display a new log file (from the last run).

* `kill-server.sh` - terminates the EC2 instance.

## Usage strategy:
1. `new-server.sh` - Start a new instance at the start of a new programming session
2. `restart-server.sh` - Keep restarting the instance for tasks in that session
3. `kill-server.sh` - Stop the instance at the end of that session

## Useful combinations:
* `./build.sh && ./upload.sh`
* `./new-server.sh && ./fetch-latest-log.sh`
* `./restart-server.sh && ./fetch-latest-log.sh`

## A note about EBS cost:
The extra EBS cost for restarting vs new is probably about $1.2 per month. This assumes a 30gb EBS volume is kept around for 12 hours every day. That's unlikely, since I'm probably unlikely to work on GPU stuff every single day in the month, throughout the day. The EBS volume just needs to be kept around for continuous sessions. If I go for lunch, and return, it's okay to kill the instance (and the EBS volume) before lunch, and take the extra 1 min penalty for the first task run of the next session (for recreating the instance and EBS volume).