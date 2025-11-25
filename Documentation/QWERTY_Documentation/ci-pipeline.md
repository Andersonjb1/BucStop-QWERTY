# CI/CD Pipeline for BucStop on AWS EC2

This document outlines the implementation of a Continuous Integration and Continuous Deployment (CI/CD) pipeline for the BucStop application when deployed on Amazon EC2 instances.

## Pipeline Overview

```
[Code] → [Build] → [Test] → [Deploy] → [Monitor]
```

## Implementation Steps

### 1. Source Control Setup

- Use GitHub/GitLab for source code management
- Configure branch protection rules
  - Require pull request reviews before merging
  - Require status checks to pass before merging

### 2. CI Pipeline with GitHub Actions

#### GitHub Actions Implementation

Create a workflow file at `.github/workflows/ci-pipeline.yml` (example):

```yaml
name: BucStop CI Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '16'
        cache: 'npm'
    
    - name: Install dependencies
      run: npm ci
    
    - name: Run linting
      run: npm run lint
    
    - name: Run tests
      run: npm test
    
    - name: Build application
      run: npm run build
```
> Above, we can see the workflow is being excuted (via GitHub Actions) on an Ubuntu runner server. It checks the code out, sets up a Node.js environment, and install dependencies. After completing those tasks, its rolls straight into linting (to check code quality), test execution, and application building.


### 3. Deployment to EC2

Extend the CI workflow to include deployment steps:
>See deployment workflow -> `.github/worflows/DeployToGHCR.yml`

```yml
# Build changed microservice images on PRs to Sprint-* and ensure all 5 images exist in GHCR
#
# - Triggers on push to branches matching Sprint-*
# - Detects which of the five microservice directories changed
# - Builds & pushes images for changed services to GitHub Container Registry (GHCR)
# - Ensures at the end that all 5 service images exist in GHCR (builds any missing ones from the default branch)
#
# Services:
# - webapp  -> "BucStop_WebApp/BucStop/"
# - gateway -> "Team-3-BucStop_APIGateway/APIGateway/"
# - snake   -> "Team-3-BucStop_Snake/Snake/"
# - pong    -> "Team-3-BucStop_Pong/Pong/"
# - tetris  -> "Team-3-BucStop_Tetris/Tetris/"

name: DeployToGHCR

on:
  push:
    branches:
      - "Sprint-*"

permissions:
  contents: read
  packages: write

# There are 4 total jobs:
# detect-changes - filters the folders of each service,
#                  if anything changed in one of those folders
#                  it sets its output to a list of the changed folders.
#
# set-namespace - pretty simple, it just sets the path to our repo to all lowercase
#                 there was issues if the path had uppercase letters
#
# build-changed - takes the list of changed services from detect-changes, builds the new version, and pushes it to GHCR
#                 if there weren't any changes, this is skipped
#
# ensure-all-images - check GHCR for images of each service,
#                     if one is missing, it builds and pushes it to GHCR
#

jobs:
  detect-changes:
    name: Detect changed microservices
    runs-on: ubuntu-latest
    outputs:
      services: ${{ steps.services-json.outputs.services }}
    steps:
      - name: Checkout PR branch (full history)
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Filter changed paths
        id: filter
        uses: dorny/paths-filter@v3
        with:
          filters: |
            webapp:
              - 'BucStop_WebApp/BucStop/**'
              - 'BucStop_WebApp/**'
              - 'BucStop/**'
            gateway:
              - 'Team-3-BucStop_APIGateway/APIGateway/**'
              - 'Team-3-BucStop_APIGateway/**'
            snake:
              - 'Team-3-BucStop_Snake/Snake/**'
              - 'Team-3-BucStop_Snake/**'
            pong:
              - 'Team-3-BucStop_Pong/Pong/**'
              - 'Team-3-BucStop_Pong/**'
            tetris:
              - 'Team-3-BucStop_Tetris/Tetris/**'
              - 'Team-3-BucStop_Tetris/**'

      - name: Produce JSON array of changed services
        id: services-json
        run: |
          changed=()
          [[ "${{ steps.filter.outputs.webapp }}" == 'true' ]] && changed+=('webapp')
          [[ "${{ steps.filter.outputs.gateway }}" == 'true' ]] && changed+=('gateway')
          [[ "${{ steps.filter.outputs.snake }}" == 'true' ]] && changed+=('snake')
          [[ "${{ steps.filter.outputs.pong }}" == 'true' ]] && changed+=('pong')
          [[ "${{ steps.filter.outputs.tetris }}" == 'true' ]] && changed+=('tetris') 

          echo "services=$(jq -nc --argjson arr "$(printf '%s\n' "${changed[@]}" | jq -R . | jq -s .)" '$arr')" >> "$GITHUB_OUTPUT"

          echo $services
          echo ${changed[@]}

  set-namespace:
    name: Set Namespace
    runs-on: ubuntu-latest
    outputs:
      namespace: ${{ steps.set.outputs.namespace }}
    steps:
      - name: To Lowercase
        id: set
        env:
          INPUT: ${{ github.repository }}
        run: |
          ns="${INPUT,,}"
          echo "namespace=${ns}" >> "$GITHUB_OUTPUT"

  build-changed:
    name: Build & push changed microservice images
    needs: [set-namespace, detect-changes]
    runs-on: ubuntu-latest
    if: ${{ needs.detect-changes.outputs.services }}
    strategy:
      fail-fast: false
      matrix:
        service: ${{ fromJson(needs.detect-changes.outputs.services) }}
    env:
      GHCR_REGISTRY: ghcr.io
      IMAGE_NAMESPACE: ${{ needs.set-namespace.outputs.namespace }}
    steps:
      - name: Checkout PR branch
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Resolve service paths
        id: set-path
        run: |
          svc=${{ matrix.service }}
          case "$svc" in
            webapp)
              path="BucStop_WebApp/BucStop"
              dockerfile="BucStop_WebApp/BucStop/Dockerfile"
              ;;
            gateway)
              path="Team-3-BucStop_APIGateway/APIGateway"
              dockerfile="Team-3-BucStop_APIGateway/APIGateway/Dockerfile"
              ;;
            snake)
              path="Team-3-BucStop_Snake/Snake"
              dockerfile="Team-3-BucStop_Snake/Snake/Dockerfile"
              ;;
            pong)
              path="Team-3-BucStop_Pong/Pong"
              dockerfile="Team-3-BucStop_Pong/Pong/Dockerfile"
              ;;
            tetris)
              path="Team-3-BucStop_Tetris/Tetris"
              dockerfile="Team-3-BucStop_Tetris/Tetris/Dockerfile"
              ;;
            *)
              echo "Unknown service: $svc"
              exit 1
              ;;
          esac
          echo "path=$path" >> "$GITHUB_OUTPUT"
          echo "dockerfile=$dockerfile" >> "$GITHUB_OUTPUT"

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v2
        with:
          registry: ${{ env.GHCR_REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build & push image for service
        uses: docker/build-push-action@v4
        with:
          context: ${{ steps.set-path.outputs.path }}
          file: ${{ steps.set-path.outputs.dockerfile }}
          push: true
          tags: |
            ${{ env.GHCR_REGISTRY }}/${{ env.IMAGE_NAMESPACE }}/${{ matrix.service }}:${{ github.head_ref || github.ref_name }}
            ${{ env.GHCR_REGISTRY }}/${{ env.IMAGE_NAMESPACE }}/${{ matrix.service }}:latest
          cache-from: type=registry,ref=${{ env.GHCR_REGISTRY }}/${{ env.IMAGE_NAMESPACE }}/${{ matrix.service }}:cache
          cache-to: type=inline

  ensure-all-images:
    name: Ensure all 5 images exist in GHCR (build missing ones from latest release or default branch)
    needs: [set-namespace, detect-changes]
    runs-on: ubuntu-latest
    env:
      GHCR_REGISTRY: ghcr.io
      IMAGE_NAMESPACE: ${{ needs.set-namespace.outputs.namespace }}
    steps:
      - name: Get latest release tag (if any)
        id: get_release
        uses: actions/github-script@v6
        with:
          script: |
            const owner = context.repo.owner;
            const repo = context.repo.repo;
            try {
              const resp = await github.rest.repos.getLatestRelease({ owner, repo });
              // return tag_name as the step output
              return { tag: resp.data.tag_name || '' };
            } catch (err) {
              // no releases or API returned 404 -> return empty string
              return { tag: '' };
            }

      - name: Decide ref to checkout (latest release tag or default branch)
        id: choose_ref
        run: |
          if [ -n "${{ steps.get_release.outputs.tag }}" ]; then
            echo "Using release tag: ${{ steps.get_release.outputs.tag }}"
            echo "ref=${{ steps.get_release.outputs.tag }}" >> "$GITHUB_OUTPUT"
          else
            echo "No release found; using default branch: ${{ github.event.repository.default_branch }}"
            echo "ref=${{ github.event.repository.default_branch }}" >> "$GITHUB_OUTPUT"
          fi

      - name: Checkout chosen ref (release tag or default branch)
        uses: actions/checkout@v4
        with:
          ref: ${{ steps.choose_ref.outputs.ref }}
          fetch-depth: 0

      - name: Login to GHCR
        uses: docker/login-action@v2
        with:
          registry: ${{ env.GHCR_REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Check & build missing images
        shell: bash
        run: |
          set -euo pipefail

          # canonical list of services
          services=(webapp gateway snake pong tetris)

          # mapping function: return directory for a service name
          get_dir() {
            case "$1" in
              webapp)  echo "BucStop_WebApp/BucStop" ;;
              gateway) echo "Team-3-BucStop_APIGateway/APIGateway" ;;
              snake)   echo "Team-3-BucStop_Snake/Snake" ;;
              pong)    echo "Team-3-BucStop_Pong/Pong" ;;
              tetris)  echo "Team-3-BucStop_Tetris/Tetris" ;;
              *) echo "" ;;
            esac
          }

          missing=()
          for svc in "${services[@]}"; do
            image="${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/${svc}:latest"
            echo "Checking ${image}..."
            if docker pull "${image}" > /dev/null 2>&1; then
              echo "Exists: ${image}"
            else
              echo "Missing: ${image}"
              missing+=("${svc}")
            fi
          done

          if [ ${#missing[@]} -eq 0 ]; then
            echo "All images already present in GHCR."
            exit 0
          fi

          echo "Will build missing images from checked-out ref (${GITHUB_SHA}): ${missing[*]}"
          for svc in "${missing[@]}"; do
            dir="$(get_dir "$svc")"
            dockerfile="${dir}/Dockerfile"
            #######################
            echo "Forcing Action to Build BucStop WebApp"
            echo "Delete When Done"
            docker build -t "${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/webapp:latest" -t "${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/webapp:${{ steps.choose_ref.outputs.ref }}" -f "Bucstop WebApp/BucStop/Dockerfile" "Bucstop WebApp/BucStop"
            docker push "${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/webapp:latest"
            docker push "${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/webapp:${{ steps.choose_ref.outputs.ref }}"
            #######################
            if [ ! -f "$dockerfile" ]; then
              echo "Warning: Dockerfile not found for $svc at $dockerfile — skipping"
              continue
            fi
            tag_latest="${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/${svc}:latest"
            tag_ref="${GHCR_REGISTRY}/${IMAGE_NAMESPACE}/${svc}:${{ steps.choose_ref.outputs.ref }}"
            echo "Building $svc from $dir -> $tag_latest, $tag_ref"
            docker build -t "${tag_latest}" -t "${tag_ref}" -f "${dockerfile}" "${dir}"
            docker push "${tag_latest}"
            docker push "${tag_ref}"
          done

      - name: Done
        run: echo "Ensure step complete."

```
>This workflow builds and pushes Docker images for the BucStop microservices to GitHub Container Registry (GHCR) on pushes to branches named in the format `Sprint-*`. It first detects which of the five service folders changed, formats the path name to lowercase, then runs a matrix job __(build in parallel)__ to build and push only the changed services (tagging by `branch/ref` and `:latest`, using registry cache). After that it verifies all five images exist in GHCR and, for any missing image, checks out the latest release or default branch and builds/pushes the missing images so the registry always contains a complete set. The workflow uses full checkout history, `dorny/paths-filter` to detect changes, `docker/login-action` and `docker/build-push-action` to authenticate and push, and requires package write permission (`GITHUB_TOKEN`).

### 4. EC2 Instance Setup

>For AWS Setup see -> `Documentation/AWS-Setup` for a comprehensive explanation.


### 5. Monitoring and Alerting

>Once EC2 is setup and after running the compose file for the first time, watchtower will begin monitoring for new images pushed to the repo's GHCR

### 6. Rollback Strategy

- Maintain versioned deployments in GHCR
- Create a rollback script/command:
  ```bash
  sudo docker stop <container-name>
  sudo docker rm <container-name>
  sudo docker run -p <container-port>:80 -d --name <container-name> <container-image-name>:<prev-image-tag>
  sudo docker restart bucstop 
  ```
- Alternatively, leverage the rollback the snapshot service integrated into BucStop.

## Security Considerations

- Store secrets in GitHub Secrets or AWS Parameter Store
- Implement VPC for network isolation
  - *Tradeoff = much more networking complexity*
- Use Security Groups to restrict access
- Apply the principle of least privilege for IAM roles
  - *Tradeoff = role complexity and additional time to troubleshoot*


## Future Improvements

- Implement blue/green deployments
- Consider containerization with Docker on ECS/EKS instead of EC2 