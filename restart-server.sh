echo "Restarting existing server.."

if [ ! -f ".instance_id" ]; then
  echo "No .instance_id file found! Does the server exist? Try running new-server.sh"
  exit
fi

instance_id=$(cat .instance_id)
aws ec2 start-instances --instance-ids "$instance_id"
