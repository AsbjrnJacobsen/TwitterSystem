# Variables
$tag = "latest"
$repository = "docker.io/teslatorben"

# Build the Docker image
docker build -t $repository/post-service:$tag -f PostsService/Dockerfile .

# Push the Docker image
docker push $repository/post-service:$tag 

# Build the Docker image
docker build -t $repository/account-service:$tag -f AccountService/Dockerfile .

# Push the Docker image
docker push $repository/account-service:$tag 

# Build the Docker image
docker build -t $repository/timeline-service:$tag -f TimelineService/Dockerfile .

# Push the Docker image
docker push $repository/timeline-service:$tag 

# Build the Docker image
docker build -t $repository/gatewayapi:$tag -f GatewayAPI/Dockerfile .

# Push the Docker image
docker push $repository/gatewayapi:$tag