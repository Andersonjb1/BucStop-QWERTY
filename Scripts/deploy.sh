#!/bin/bash
export env=containers
export GIT_COMMIT=$(git rev-parse HEAD)
# ─────────────────────────────────────────────────────────────
#  Config / Constants
# ─────────────────────────────────────────────────────────────

# Prod IP, still figuring out how we want to manage dev IP not being static
PUBLIC_IP="3.228.108.146"

# Required CLI tools
REQUIRED_CMDS=("git" "docker" "docker-compose")

# ─────────────────────────────────────────────────────────────
#  Check for required tools
# ─────────────────────────────────────────────────────────────
check_requirements() {
    for cmd in "${REQUIRED_CMDS[@]}"; do
        if ! command -v $cmd >/dev/null 2>&1; then
            echo "❌  Missing required command: $cmd"
            exit 1
        fi
    done
}
check_requirements

# ─────────────────────────────────────────────────────────────
#  Cleanup function
# Stops Docker containers and prunes Docker resources to save EBS space.
# EBS space costs money in AWS.
# ─────────────────────────────────────────────────────────────
cleanup() {
    echo -e "\n🚨  Cleaning up processes..."

    if [ -n "$BUILD_PID" ]; then
        kill "$BUILD_PID" 2>/dev/null
    fi

    # Create Snapshot 
    create_snapshot() {
    echo "📷  Creating snapshot..."
    curl -X POST http://3.228.108.146:8080/snapshots/create -d "description=Automated snapshot before shutdown" 2>/dev/null
    }

    create_snapshot

    echo -e "\n🧹  Stopping Docker containers..."
    docker-compose down

    echo -e "\n✂️  Pruning unused Docker resources..."
    docker system prune -af --volumes | awk '
        /Deleted Images:/ { skip=1; next }
        /Deleted build cache objects:/ { skip=1; next }
        /^Total reclaimed space:/ {
            skip=0
            print "   🧽  " $0
            next
        }
        skip==0 { print }
    '
}

# Bind cleanup to Ctrl+C and termination signals
trap cleanup SIGINT SIGTERM

# ─────────────────────────────────────────────────────────────
#  Timer display during builds for feedback
# ─────────────────────────────────────────────────────────────
timer() {
    local start_time=$(date +%s)
    local pid=$1

    echo -n "⏳  Building services... Elapsed time: 00:00"

    while kill -0 "$pid" 2>/dev/null; do
        sleep 1
        local current_time=$(date +%s)
        local elapsed=$((current_time - start_time))
        local minutes=$((elapsed / 60))
        local seconds=$((elapsed % 60))
        printf "\r⏳  Building services... Elapsed time: %02d:%02d" "$minutes" "$seconds"
    done

    echo -e "\r✅  Services built in $minutes minutes and $seconds seconds.    "
}

# ─────────────────────────────────────────────────────────────
#  Deployment Magic Starts Here
# ─────────────────────────────────────────────────────────────
echo "🚀  Starting deployment process..."

# Pull latest repo updates
echo "🔄  Checking repository status..."
pull_output=$(git pull)
if [[ "$pull_output" == "Already up to date." ]]; then
    echo "✅  Repo is already up to date."
else
    echo "$pull_output"
fi

# Clean up Docker resources
cleanup

echo -e "\n🐳  Launching microservices..."
(docker-compose up -d > /dev/null 2>&1) &
BUILD_PID=$!
timer $BUILD_PID
echo -e "✅  All containerized services started successfully!\n"

# Notes: 
# I am not 100% certain I am in love with the cleanup mechanism.
# Consider revisiting cleanup for containers both before and after deployment actions.
# Consider using 'killall dotnet' as blunt force to kill dotnet services instead.
# Consider altering `docker system prune` based on our approach to data persistence.
