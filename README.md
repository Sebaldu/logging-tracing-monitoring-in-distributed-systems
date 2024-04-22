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

After all services have started you need to run the following commands to create the index-patterns in opensearch.
```bash
curl --request POST \
  --url http://localhost:5601/api/saved_objects/index-pattern \
  --header 'Content-Type: application/json' \
  --header 'osd-xsrf: true' \
  --data '{
  "attributes": {
    "title": "cpu-index",
    "timeFieldName": "@timestamp",
    "fields": "[{\"count\":0,\"name\":\"@timestamp\",\"type\":\"date\",\"esTypes\":[\"date\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"count\":0,\"name\":\"_id\",\"type\":\"string\",\"esTypes\":[\"_id\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_index\",\"type\":\"string\",\"esTypes\":[\"_index\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_score\",\"type\":\"number\",\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_source\",\"type\":\"_source\",\"esTypes\":[\"_source\"],\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_type\",\"type\":\"string\",\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"cpu_p\",\"type\":\"number\",\"esTypes\":[\"float\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"count\":0,\"name\":\"hostname\",\"type\":\"string\",\"esTypes\":[\"text\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"hostname.keyword\",\"type\":\"string\",\"esTypes\":[\"keyword\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true,\"subType\":{\"multi\":{\"parent\":\"hostname\"}}}]"
  }
}'
```

```bash
curl --request POST \
  --url http://localhost:5601/api/saved_objects/index-pattern \
  --header 'Content-Type: application/json' \
  --header 'osd-xsrf: true' \
  --data '{
  "attributes": {
    "title": "mem-index",
    "timeFieldName": "@timestamp",
    "fields": "[{\"count\":0,\"name\":\"@timestamp\",\"type\":\"date\",\"esTypes\":[\"date\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"count\":0,\"name\":\"_id\",\"type\":\"string\",\"esTypes\":[\"_id\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_index\",\"type\":\"string\",\"esTypes\":[\"_index\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_score\",\"type\":\"number\",\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_source\",\"type\":\"_source\",\"esTypes\":[\"_source\"],\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"_type\",\"type\":\"string\",\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"hostname\",\"type\":\"string\",\"esTypes\":[\"text\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"count\":0,\"name\":\"hostname.keyword\",\"type\":\"string\",\"esTypes\":[\"keyword\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true,\"subType\":{\"multi\":{\"parent\":\"hostname\"}}},{\"count\":0,\"name\":\"memFree\",\"type\":\"number\",\"esTypes\":[\"long\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"count\":0,\"name\":\"memTotal\",\"type\":\"number\",\"esTypes\":[\"long\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"count\":0,\"name\":\"memUsed\",\"type\":\"number\",\"esTypes\":[\"long\"],\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true}]"
  }
}'
```
opensearch dashboards will be available at `http://localhost:5601`

Click the Hamburger menu in the top left corner.
Go to Dashboards Management -> Saved Objects -> Import
And select the `dashboards.ndjson` file from the `single-node-setup` directory.

The Dashboard with its Visualizations can be found in Dashboards in the Hamburguer menu and should be selected as the default dashboard.
For querying the data stored in opensearch, there are better examples in the opensearch documentation.

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