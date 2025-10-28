#!/bin/bash

# Color codes
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color
RED='\033[0;31m'

# Array to store PIDs
declare -a SERVICE_PIDS

# Function to start a service
start_microservice() {
    local service_name=$1
    local service_path=$2
    local port=$3
    
    echo -e "${CYAN}Starting $service_name on port $port...${NC}"
    cd "$(dirname "$0")/../$service_path" || exit
    dotnet run --urls="http://0.0.0.0:$port" --environment Development > "/tmp/$service_name.log" 2>&1 &
    SERVICE_PIDS+=($!)
    cd - > /dev/null
}

# Function to cleanup services
cleanup() {
    echo -e "\n${RED}Stopping all services...${NC}"
    for pid in "${SERVICE_PIDS[@]}"; do
        kill $pid 2>/dev/null
    done
    exit 0
}

# Set up cleanup trap
trap cleanup SIGINT SIGTERM

echo -e "${GREEN}Starting BucStop microservices...${NC}"

# Start game services
start_microservice "Snake" "Team-3-BucStop_Snake/Snake" 8082
start_microservice "Pong" "Team-3-BucStop_Pong/Pong" 8083
start_microservice "Tetris" "Team-3-BucStop_Tetris/Tetris" 8084

# Wait for game services to start
echo "Waiting for game services to initialize..."
sleep 5

# Start API Gateway
start_microservice "API-Gateway" "Team-3-BucStop_APIGateway/APIGateway" 8081

# Wait for API Gateway to start
echo "Waiting for API Gateway to initialize..."
sleep 5

# Start main BucStop WebApp
start_microservice "BucStop-WebApp" "Bucstop WebApp/BucStop" 8080

echo -e "\n${GREEN}All services are starting up...${NC}"
echo -e "${YELLOW}Service endpoints:${NC}"
echo -e "${GRAY}- BucStop WebApp: http://localhost:8080${NC}"
echo -e "${GRAY}- API Gateway:    http://localhost:8081${NC}"
echo -e "${GRAY}- Snake Game:     http://localhost:8082${NC}"
echo -e "${GRAY}- Pong Game:      http://localhost:8083${NC}"
echo -e "${GRAY}- Tetris Game:    http://localhost:8084${NC}"

echo -e "\n${YELLOW}Service logs are available in /tmp/:{NC}"
echo -e "${GRAY}- /tmp/Snake.log${NC}"
echo -e "${GRAY}- /tmp/Pong.log${NC}"
echo -e "${GRAY}- /tmp/Tetris.log${NC}"
echo -e "${GRAY}- /tmp/API-Gateway.log${NC}"
echo -e "${GRAY}- /tmp/BucStop-WebApp.log${NC}"

echo -e "\n${YELLOW}Press Ctrl+C to stop all services${NC}"

# Wait for Ctrl+C
wait