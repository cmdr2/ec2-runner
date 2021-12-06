echo "Starting new server.."

instance_id=$(aws ec2 run-instances --launch-template "LaunchTemplateId=lt-027fb2d0ed6eec46f" | grep InstanceId | awk '{print $2}' | sed -e 's/\"//g' | sed -e 's/,//g')

echo "InstanceId: $instance_id"
echo "$instance_id" > .instance_id
