# AWS Elastic IP Setup Guide

An Elastic IP (EIP) provides a static public IPv4 address that persists across EC2 instance stops/starts and can be reassigned between instances.

## Why Use an Elastic IP?

- **Static IP**: Prevents your public IP from changing when you stop/start the instance
- **DNS Stability**: Allows you to point domain names to a fixed IP
- **High Availability**: Can quickly reassign to a backup instance during failures
- **Cost**: Free when attached to a running instance; $0.005/hour when unattached

## 1. Allocate an Elastic IP

### Via AWS Console
1. Navigate to **EC2 Dashboard** → **Network & Security** → **Elastic IPs**
2. Click **Allocate Elastic IP address**
3. Choose **Amazon's pool of IPv4 addresses** (default)
4. Add tags (optional):
   - Key: `Name`, Value: `BucStop-Production-EIP`
   - Key: `Environment`, Value: `Production`
5. Click **Allocate**
6. Note the allocated IP address (e.g., `3.82.21.195`)

## 2. Assign EIP with BucStop Instance

### In the AWS Console
1. Click into the EIP that was just created
2. Under **Resource Type** click instance
3. Move down the page, under **Instance** select the instance that was created for BucStop
4. Lastly, select the Private IP assigned to your instance

## All done
- See `5-Setting-Up-EC2-Environment` for the next steps