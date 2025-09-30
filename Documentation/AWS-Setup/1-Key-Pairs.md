# AWS EC2 Key Pair Setup

Key pairs are used to securely connect to your EC2 instances over SSH.  

---

## 1. What is a Key Pair?
- **Public key** → stored in AWS, attached to your instance.
- **Private key (.pem)** → downloaded **once** by you, used to prove your identity when connecting.
- Together, they form the secure handshake for SSH.

**Important:** If you lose the `.pem` file, you **cannot** SSH into your instance unless you replace the key pair. (Which is not easy)

---

## 2. Create a Key Pair

1. Go to **EC2 → Key Pairs** in the AWS Console.
2. Click **Create key pair**.
3. Enter a name (e.g., `BucStop`).
4. Select **RSA** or **ed25519** and **.pem** format.
5. Click **Create**.
6. A `.pem` file will be downloaded automatically — **store it securely**.

## 3. Already have a Key Pair? Import it.
1. Go to **EC2 → Key Pairs** in the AWS Console.
2. Click **Actions** → **Import Key Pair**
3. Enter a name → Click Browse and find your key.
4. Click **Import Key Pair**
5. The pair should be ready to use when creating an instance.

## 4. Using the Key Pair
1. Open a terminal 
2. Use the command below, adding your own key path and your instance's IP. 

```ssh -i <key path> ec2-user@<instance-public-ip>```

_This command can be found when clicking connect on an instance then clicking SSH_

