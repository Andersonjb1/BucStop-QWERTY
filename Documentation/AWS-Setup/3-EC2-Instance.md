# BucStop EC2 Setup Guide

This document explains how to launch and configure the BucStop EC2 instance in AWS.  
It assumes you already have an AWS account, IAM permissions, a key pair, and a security group from the previous steps.

---

## 1. Launch the EC2 Instance

1. Go to **EC2 → Instances → Launch Instance**.
2. Name the instance: **BucStop**.
3. Choose an Ubuntu.
4. Select an instance type, `t3.micro` is the most up-to-date as of September 2025. **Currently, you have to use a `t3.large` instance it to work.**
5. Attach a key pair:
   - Select existing BucStop keypair or see `1-Key-Pairs.md`.
6. Configure:
   - VPC: Leave as default.
   - Subnet: No Preference.
   - Availability Zone: No Preference.
   - Auto-assign Public IP: **Enabled**.
   - **Firewall**: Select Existing Security Group → BucStop or see `2-Security-Groups.md`
   - Storage: Leave as default.
7. Click **Launch**.

---

## 2. Connect to the Instance

1. Select the instance that was created.
2. Click **Connect** → **SSH Client**
3. Copy the example given, ie. ```ssh -i "BucStop.pem" ubuntu@ec2-54-224-120-40.compute-1.amazonaws.com```
4. From your terminal, enter what you just copied.
5. The server should use the key to log you in.

_Note: Make sure to cd into the location you saved your key or use the full path to the key in the command_
