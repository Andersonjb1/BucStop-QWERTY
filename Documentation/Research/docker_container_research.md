# Research Document: Building a Docker Container and Connecting with Services

## Introduction
Docker is a platform that allows developers to package applications and their dependencies into standardized units called containers. Containers ensure consistent behavior across different environments, from local development to cloud deployment. This research document explores how to build Docker containers, connect them with other services, and provides examples based on the BucStop server project.

---

## 1. Building a Docker Container

### Steps to Build
1. **Create a Dockerfile** – This file defines the environment and how to build the container image.
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
   WORKDIR /app
   EXPOSE 80

   FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
   WORKDIR /src
   COPY . .
   RUN dotnet restore
   RUN dotnet publish -c Release -o /app/publish

   FROM base AS final
   WORKDIR /app
   COPY --from=build /app/publish .
   ENTRYPOINT ["dotnet", "Snake.dll"]
   ```

2. **Build the image**
   ```bash
   docker build -t snake-game .
   ```

3. **Run the container**
   ```bash
   docker run -d -p 8082:80 snake-game
   ```
   This maps port 8082 on the host to port 80 inside the container.

### Example from BucStop
The BucStop project uses multiple services:
- `Snake` → exposed on port 8082
- `Pong` → exposed on port 8083
- `Tetris` → exposed on port 8084
- `APIGateway` → exposed on port 8081
- `WebApp` → exposed on port 8080

Each service has its own Dockerfile, enabling independent containerization.

---

## 2. Connecting Containers with Other Services

### Docker Compose
The BucStop project uses **docker-compose** to define and manage multiple services.

Example `docker-compose.yml` snippet:
```yaml
version: '3.8'
services:
  snake:
    build: ./Team-3-BucStop_Snake/Snake
    ports:
      - "8082:80"

  pong:
    build: ./Team-3-BucStop_Pong/Pong
    ports:
      - "8083:80"

  tetris:
    build: ./Team-3-BucStop_Tetris/Tetris
    ports:
      - "8084:80"

  apigateway:
    build: ./Team-3-BucStop_APIGateway/APIGateway
    ports:
      - "8081:80"

  webapp:
    build: ./Team-3-BucStop_WebApp/WebApp
    ports:
      - "8080:80"
```

This configuration:
- Starts all services together.
- Ensures networking between containers using the default Docker bridge network.
- Maps each service to a host port for external access.

### Networking
By default, Docker Compose creates a network where services can reach each other by **service name**. For example:
- The `APIGateway` can connect to `snake` by using `http://snake:80`.
- This avoids hardcoding IP addresses.

---

## 3. Common Issues and Solutions

1. **Service not reachable on host machine**
   - Check that the port mapping (`8082:80`) is correct.
   - Ensure the service is bound to `0.0.0.0`, not `127.0.0.1`.

2. **Cross-Origin Requests (CORS) in browsers**
   - If the WebApp loads scripts from `http://server:8082/js/snake.js`, ensure that CORS headers are configured.

3. **AWS Deployment Security**
   - Security Groups must allow inbound traffic on ports 8080–8084.
   - NACLs must allow return traffic on ephemeral ports.

4. **Better Practice: Reverse Proxy**
   - Instead of exposing multiple ports, use **Nginx or an AWS Application Load Balancer** to route requests:
     ```nginx
     location /snake/ {
       proxy_pass http://snake:80/;
     }
     ```
   - This way, only port 80/443 is public.

---

## 4. Example Workflow with BucStop

1. **Build and start services locally**
   ```bash
   docker-compose up --build -d
   ```

2. **Access WebApp** at `http://localhost:8080`
   - The WebApp retrieves JS game files from API Gateway URLs like:
     - `http://localhost:8082/js/snake.js`
     - `http://localhost:8083/js/pong.js`
     - `http://game-tetris/js/tetris.js`

3. **Deploy to AWS**
   - Push GitHub to AWS server.
   - Run services on Amazon ECS or EC2 with Docker installed.
   - Configure Security Groups and NACLs to allow access from your specified CIDR ranges only.

---

## Conclusion
Docker containers provide a consistent and scalable way to deploy applications. With `docker-compose`, multiple services like the BucStop games can run together and communicate seamlessly. For production, a reverse proxy or load balancer improves security and simplifies access, avoiding the need to expose multiple custom ports. The BucStop example illustrates a real-world, multi-container deployment strategy using Docker and AWS.

