# ec2-runner

Build your own simple AWS Lambda, but only for your use. Sort-of-serverless. With a 2-3 min startup time per invocation (gah!).

Why would you even do that? To be able to run your 2 - 30 minute workloads on high-end computers without owning them. Especially GPUs, since AWS Lambda doesn't provide GPUs (and they're [really expensive right now](https://en.wikipedia.org/wiki/2020–2021_global_chip_shortage) to purchase).

## What does this do?
It takes an executable program that you've written, starts a temporary EC2 instance (in your AWS account), runs your program on it, uploads the logs to your S3 bucket, and shows you the log in your command line.

The EC2 instance is run only for the duration of the program execution, so you can save costs while running pretty massive instance types (especially on high-end GPUs). EC2 bills per second of usage, and stopped instances don't get billed (but EBS volumes are billed, which is why you should terminate after a session [see [Usage strategy](#usage-strategy) below]).

Suitable for when you're developing algorithms that you frequently want to test on high-end specifications (while you're programming), with run times between 2 - 30 minutes. Anything less or more, you should really think about whether the overhead time-per-invocation or costs are worth it for you.

Designed for programming -> deploying and running on EC2 -> analyzing the results -> and returning to programming some more, with as little overhead in the programming feedback loop.

**Note:** this is NOT even close to ready for others to use. It contains URLs and paths that are hardcoded for a specific project of mine, but can be generalized (feel free to edit the code). See the [What needs to be done](#what-needs-to-be-done-to-make-this-work-for-everyone) section below for more info.

## Commands:
* `build.sh` - builds Parallel.exe from the Unity Project.
* `upload.sh` - uploads the task zip to S3.

* `new-server.sh` - creates an EC2 Windows instance from a launch template in the AWS Console. A user-data script fetches the task zip, executes the task, uploads the run log to S3, and stops the instance.
* `restart-server.sh` - restarts an exist EC2 Windows instance from the `.instance_id` file in the local directory, created by the `new-server.sh` script. This runs the task and uploads the log as usual.
* `fetch-latest-logs.sh` - keeps polling until it can display a new log file (from the last run).

* `kill-server.sh` - terminates the EC2 instance (and associated resources, like the EBS Volume).

## Usage strategy:
1. `new-server.sh` - Start a new instance at the start of a new programming session.
2. `restart-server.sh` - Keep restarting the instance for tasks in that session. The instance runs only for the duration of the task run.
3. `kill-server.sh` - Terminate the instance and EBS volume at the end of that session.

## Useful combinations:
* `./build.sh && ./upload.sh`
* `./new-server.sh && ./fetch-latest-log.sh`
* `./restart-server.sh && ./fetch-latest-log.sh`

### Overhead for each task run:
- 3 min overhead for the first task in a session (for starting a new instance).
- 2 min overhead for every subsequent task in a session (for restarting the instance).

**Note:** the overhead varies slightly based on the instance size. The more powerful the instance type, the less the overhead. But you probably won't get lower than 1 minute for the overhead (I may be wrong).

### Costs:
The EC2 cost depends on the instance type and how much it's used, obviously. The EBS cost shouldn't be more than $1.2 per month, assuming a 30gb SSD volume is used for 12 hours a day.

## What needs to be done, to make this work for everyone?
1. The EC2 template I've used, and the IAM policy/role (for writing to S3) haven't been described here. CloudFormation might be a good idea.
2. My bucket and instance template Ids are hardcoded in the scripts.
3. My demo project is checked-in here, and gets built and deployed, instead of taking a user-defined binary/ZIP-file to deploy and run.

## A note about EBS cost:
The extra EBS cost for restarting vs new is probably about $1.2 per month. This assumes a 30gb EBS volume is kept around for 12 hours every day. That many hours of usage is unlikely, since I'm not likely to work on GPU stuff every single day in the month, throughout the day. But if I do, that's the price (which isn't much TBH).

In practice, the EBS volume just needs to be kept around for the duration of each programming session. If I go for lunch, and return, it's okay to kill the instance (and the EBS volume) before lunch, and take the extra 1 min penalty for the first task run of the next session (for recreating the instance and EBS volume).