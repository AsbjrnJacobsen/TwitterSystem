# Twitter System

# Design, Architecture, and Scoping
<p align="center">
  <img src="imgs/diagram.png" alt="Architecture Diagram"/>
</p>  

# Deployment using Docker
Deploy as local development environment
`docker compose up -d`

Deploy as production environment
`docker compose up -d -f docker-compose.yml -f docker-production.yml`

## Deployment Plan
As we are using Docker for deployment, the deployment strategy is simple.  
Below is described how to deploy to a production environment.

### How to deploy on production
1. Clone the repository to the production machine
2. Run the 'Deploy as production environment' command in previous section.
3. Additionally the firewall should be set to allow incoming connections on port `8080`.

### Security
Using docker as the production environment, there are few security concerns.

For one, the isolation of the containers makes it so, that if one container is breached, 
it is very difficult for the intruders to pivot into other containers, or the host system.  
Spreading from one container to another is further secured by the segmentation of the networks between the containers.
Containers only have access to other required containers.

### Scalability
To scale the system, we plan to add more gateway, post, account, and timeline services as needed based on bottlenecks.
The posts and accounts databases will be scaled in the form of beefier database servers, also called vertical scaling.
Depending on needs, regional servers could be setup. 
Additionally, segmentation of data accross servers can be implemented.

### Disaster recovery
Assuming docker is fault-free, the pressing issue is that of faults in the TwitterSystem. 
By always having several instances of the different services running, downtime is minimized.
Systems that are running into faults will be restarted with a clean state to avoid cascading faults.
The databases will be copied to cold storage every 4 hours, so that if a fire breaks out in the datacenter, we still have data security.

# Inter Service Communication
We make use of the HTTP protocol to define an API using an RPC schema. This makes communication simple and straightforward.

It was considered if message queues should be employed, but for the system envisioned it is not fitting as there are no need 
for multiple receivers of the requests within the system. 

Our initial thought was to use an API with an RPC schema. This would create a simple and straightforward communication model. 
After implementation we started discussing, whether or not, we should have used message queues's. 
Our discussion lead us to the following conclusion:  

Using a message queue system could be faster, but also more complicated to setup. However, it would most likely be better for scaling the system, i.e. many microservices. Although, after a certain scaling size, the message queue's would also become a bottle neck.  
The option we went with was to use the internal DNS mapping of services with replicas. This means we have a single gateway that forwards 
traffic to the services which each has multiple replicas.  

To further extend this model, loadbalancers could be added for incoming client requests, and multiple gateway replicas could be used.  
Additionally, loadbalancers for each service would also be required.  

This sure would have been a great discussion to have, before we started implementing and designing. However, circumstances happened.
Going forward, we might change the communication from HTTP RPC API to message queue's.  

If we were to design a twitter clone without having to implement it ourselves, we would have chosen message queue's as our communication channel because it's properties are better suited for very large systems.
