# 🚀 AWS Shared Account Setup

## **Overview**
This document explains the setup of our **shared AWS account**, including user access, permissions, and some simplified best practices for working with AWS resources. For anyone provisioning new resources, please take an extra few minutes to familiarize yourself with what is in the free tier and what isn't - [AWS Free Tier](https://docs.aws.amazon.com/) (search the resource then select pricing - my wallet thanks you)

---

## **👤 User Roles & Permissions**
There are **two IAM user roles** in this AWS account:

### **1️⃣ Developer Access (subject to change as needed)**
**👥 User Name:** `cooked-dev`  
📌 **Permissions:**
- ✅ **Full control** over **EC2, S3, CloudWatch, CloudTrail, CloudFormation, and Launch Templates**.
- ✅ Can **start, stop, modify, and delete instances**.
- ✅ Can **create and manage infrastructure using CloudFormation**.
- ❌ **Cannot modify IAM settings (users, roles, policies)**.
- ❌ **Cannot modify billing or security settings**.


### **2️⃣ Admin Access**
**👥 User Name:** `cooked-admin`  
📌 **Permissions:**
- ✅ **TLDR full access to everything**
- ✅ Can **create, modify, and delete** AWS resources.
- ❌ **Should not be used for daily development tasks**—only for account administration.
- ❌ **Root account usage is restricted** (only used for emergency account management).

🔹 **Who gets this role?**
- I will use this (Chris P) for the time being for admin related tasks. Because there is no way to set spend limits, exposure is theoretically unlimited so I think we should use a "Principle of Least" priviledge approach to start off. If permission management becomes too burdensome in reference to making sure the dev user has access to everything they need, I will just yolo admin priviledges to it. 

🔹 **Important:**    
- **Monitor billing usage regularly** to prevent unexpected charges.

---

## **🔑 AWS Login & Security**
### **🔹 How to Access AWS**
1. **Go to the AWS Sign-In Page:**  
   - 🔗 [AWS Console](https://aws.amazon.com/console/)
2. **Enter your IAM user credentials** (This will be as a dev IAM user unless you are designated admin related tasks).
    - *I can shared these credentials in person or over discord DM*
3. **Navigate to the AWS services** required for *cooking*.

---

## **🛠️ Allowed AWS Services for Devs (subject to change)**
| Service            | Full Access | Read-Only Access |
|--------------------|------------|-----------------|
| EC2 (Instances)   | ✅ Full Control | ✅ View Only |
| S3 (Storage)      | ✅ Full Control | ✅ View Only |
| CloudWatch        | ✅ Full Control | ✅ View Only |
| CloudTrail        | ✅ Full Control | ✅ View Only |
| CloudFormation    | ✅ Full Control | ✅ View Only |
| Launch Templates  | ✅ Full Control | ✅ View Only |
| IAM (Users/Roles) | ❌ No Access | ✅ View Only |
| Billing           | ❌ No Access | ✅ View Only |
| RDS (Databases)   | ❌ No Access | ✅ View Only |
| DynamoDB          | ❌ No Access | ✅ View Only |
| Load Balancers    | ❌ No Access | ✅ View Only |
| Auto Scaling      | ❌ No Access | ✅ View Only |
| Security Settings | ❌ No Access | ❌ No Access |

---

## **⚡ Best Practices**
- **🚀 Use the AWS Free Tier** where possible to avoid unnecessary charges ().
- **❌ Do not stop or terminate instances unless necessary** (EC2 charges apply based on running time).
- **🔒 Keep your AWS credentials secure**—do not share your IAM credentials (for the love of god, I can't set spend limits).
- **📊 Monitor AWS costs** via the **Billing Dashboard**.
- **🔄 Always shut down unused EC2 instances** to save costs.
  - *Termination of an EC2 ≠ stopping an EC2*

---

## **🔔 Alerts & Cost Management**
- **AWS Budgets & Alerts are enabled** to notify the admin (Chris temproarily) if usage exceeds limits.
- **Free Tier alerts** are configured to avoid unexpected costs.

---

## **📌 AWS Resources You Can Use**
### **EC2 (Virtual Machines)**
- Use EC2 for running applications.
- Start/Stop instances when needed.
- Select **t2.micro** or **t3.micro** to stay within Free Tier.

### **S3 (Storage)**
- Store project files, backups, and logs.
- If we do end up using this for persistent storage on Bucstop, follow the bucket naming convention please:  
  📂 `projectname-environment-storagedescription`

### **CloudWatch & CloudTrail**
- Use **CloudWatch** for monitoring logs and metrics.
- Use **CloudTrail** to track AWS API activity.

### **CloudFormation**
- Automate AWS infrastructure deployments.
- Define resources using YAML/JSON templates.

---

## **🚀 Getting Started**
1. **Log into AWS** using your IAM user credentials.
2. **Explore AWS services** based on your permissions.
3. **Use CloudWatch logs** to debug applications.
4. **Experiment with EC2, S3, and CloudFormation**.

---

## **❓ Need Help?**
- **AWS Documentation:** [AWS Docs](https://docs.aws.amazon.com/) 📜 
- **Hit me up on discord**


