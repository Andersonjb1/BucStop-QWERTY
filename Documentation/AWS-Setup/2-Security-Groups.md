# Security Group Setup for BucStop Web + Microservices

This guide explains how to configure an AWS Security Group so that:
- Only two CIDR blocks can reach the public website (ports 80/443).
- SSH access is restricted to a trusted IP.
- Microservices can communicate with each other internally (ports 8080–8084).

---

## Prerequisites
- AWS account with permissions to manage EC2 Security Groups.
- Security Group already created (or create a new one).
- Your own trusted IP address for SSH.

---

## Steps

### 1. Open Security Group in AWS Console
1. Navigate to **EC2 → Security Groups**.
2. Name the Security Group **BucStop**
3. Select the Security Group attached to your instance or create a new one.

---

### 2. Configure Inbound Rules

Add the following rules:

| Port(s)     | Protocol | Source            | Purpose                                      |
| ----------- | -------- | ----------------- | -------------------------------------------- |
| 22          | TCP      | `<your-ip>/32`    | Admin SSH access (replace with your IP)      |
| All Traffic | TCP      | `151.141.0.0/16`  | Allow web traffic from ETSU Wifi CIDR block  |
| All Traffic | TCP      | `216.145.70.0/23` | Allow web traffic from ETSU E-NET CIDR block |

---

### 3. Configure Outbound Rules
- Leave outbound rules as default (`All traffic → 0.0.0.0/0`).
- AWS Security Groups are **stateful**: return traffic is automatically allowed.

## 4. Make sure to save!
- When creating an instance your new security group should be available.
