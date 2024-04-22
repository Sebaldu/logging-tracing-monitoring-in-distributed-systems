# Logging, Monitoring, and Tracing in distributed systems
This is a simple demonstration of how you can 
implement logging, monitoring, and tracing in a
distributed system using a forwarder-aggregator architecture 
with the following tools:

- fluentbit (as forwarder)
  Each machine in your system will have one instance of fluentbit running on it.
  This instance acts as a sink for logs from the applications running on the machine,
  metrics from the machine itself, and traces from the applications running on the machine.
  It then forwards this data to a central fluentd instance. (except for traces, which are sent to jaeger)

- fluentbit (as aggregator)
  It acts as a sink for all the data that is forwarded by the fluentbit forwarder instances.
  Forwarded massages can be modified, filtered or enriched before being sent to the final destination.
  (multiple destinations can be configured)

- jaeger
  For Visualizing traces gathered from all the applications running in the system.

- opensearch
  For persisting all the collected data.

- opensearch dashboards
  For varius visualizations and dashboards and querying the data stored in opensearch.

- hotrod
  A simple application that generates traces and logs.
  (for demonstration purposes)

## Setup
There are two ways to setup this system:
This setup is for demonstration purposes only as it is not secured in any way!!!
Requires docker and docker-compose to be installed on your machine.

### Single node setup
For opensearch to work, you might need to increase the max_map_count on your machine.
```bash
sudo sysctl -w vm.max_map_count=262144
```
Move to the `single-node-setup` directory:
Afterwards, run the following command:
```bash
docker-compose up
```

After all services have started you need to run the following scripts to create the necessary index patterns and visualizations in opensearch.
```bash
NOT YET IMPLEMENTED
```
opensearch dashboards will be available at `http://localhost:5601`

the Visualizations can be found in Dashboards -> Stats (Cpu Usage and Memory Usage)
for querying the data stored in opensearch, there are better examples in the opensearch documentation.

jaeger ui will be available at             `http://localhost:16686`

hotrod will be available at                `http://localhost:8080`

### Multi-node setup

# Not yet implemented

Each machine needs to have docker and docker-compose installed.
For running multiple docker containers that need to communicate with each other, 
you need to create a network on each machine.
```bash
docker network create my_network
```
Also the max_map_count needs to be increased on the machine running opensearch.
```bash
sudo sysctl -w vm.max_map_count=262144
```

Copy the fluentbit directory to each machine.
Copy the .env.example file to .env and set the values for the environment variables.

Spread one instance of all the remainig services among the machines.
Copy the .env.example file to .env and set the values for the environment variables.

opensearch dashboards will be available at `http://ip_of_opensearch_host:5601`

the Visualizations can be found in Dashboards -> Stats (Cpu Usage and Memory Usage)
for querying the data stored in opensearch, there are better examples in the opensearch documentation.

jaeger ui will be available at             `http://ip_of_jaeger_host:16686`

hotrod will be available at                `http://ip_of_hotrod_host:8080`