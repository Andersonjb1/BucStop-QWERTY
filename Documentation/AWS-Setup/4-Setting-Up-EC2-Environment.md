
## 1. First Update the Server

```sudo apt update && sudo apt upgrade -y```

# 2. Install common tools

```sudo apt install -y git curl unzip```

# 3. Setup **Docker** (Copy & Paste Both Sets of Commands)
  *Link to official docs: https://docs.docker.com/engine/install/ubuntu/#install-using-the-repository*

### 3.1 Add Docker's official GPG key:

sudo apt-get update
sudo apt-get install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

### 3.2 Add the repository to Apt sources:

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update

### 3.3 Install Docker Packages
sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

### 3.4 Test if Docker is running
- Run the command: `sudo systemctl status docker`
- **Option 1:** If `docker.service` is enabled and active, you're good to continue (exit with q).
- **Option 2:** If `docker.service` is not enabled or active, run the following command:
  - ```sudo systemctl start docker```
- Now run a test container:
  - ```sudo docker run hello-world```
  - If all is well, you should get a _Hello from Docker!_ message.

# 4. Deploy Repo
1. ```git clone https://github.com/Andersonjb1/BucStop-QWERTY.git```
2. ```cd BucStop-QWERTY```

# 5. Build Application (**As of right now will not build correctly**)
```sudo env=containers docker compose up -d``` (-d runs the containers in the background)
