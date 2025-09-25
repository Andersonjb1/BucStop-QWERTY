# AWS Security Groups – BucStop

## What is an AWS Security Group?

An AWS Security Group is a virtual firewall that can be applied to EC2 instances to control the traffic that is able to flow into and out of the EC2 instance. Traffic is controlled using two sets of rules: Inbound Rules which control the traffic that is able to flow into the EC2 instance and Outbound Rules which control the traffic that is able to flow out of the EC2 instance. Different EC2 instances should have Security Groups designed specifically for the needs of the instance (like a Production Instance Security Group that only allows access from the IPv4 addresses associated with the ETSU Wi-Fi network and a Development Instance Security Group that allows access from IPv4 addresses outside of the ETSU network).

## How to access AWS Security Groups

AWS Security Groups can be accessed through the EC2 service on the AWS Console. From the EC2 service, you can find Security Groups under the Network and Security section as shown below:

![AWS Console showing Security Groups location under Network and Security section](images/AWS_SecurityGroups/image_1.png)

From the Security Groups page, you can create, manage, and delete any security groups that you have.

![](images/AWS_SecurityGroups/image_2.png)

## Creating a Security Group

When creating a security group, you need to provide a name, description, associated VPC (Virtual Private Cloud), the inbound rules, the outbound rules, and any tags (optional). For each of these, you can find more information by clicking the blue "Info" button beside the name to be provided additional information and links to manual pages that go into further detail.

![](images/AWS_SecurityGroups/image_3.png)

### Inbound Rules

Inbound Rules govern the types of traffic that is allowed to contact the EC2 instance. There are multiple different options that can be specified for an inbound rule from the type of traffic, protocol, port range, source, and description.

The **Type** determines what kind of traffic is allowed to connect to the EC2 instance whether it is a custom TCP, custom UDP, custom ICMP, SSH, HTTP, HTTPS, etc.

The **Protocol** is the associated type of protocol for the type specified (TCP for a custom TCP, UDP for a custom UDP, TCP for SSH, etc.).

The **Port Range** is the specific ports that will be allowed access (22 for SSH, 80 for HTTP, 443 for HTTPS, etc.) This section is very important for the project as the containers run on their own ports (8080 for the WebApp, 8081 for the API Gateway, 8082 for Snake, 8083 for Pong, 8084 for Tetris) and will need to be explicitly allowed with an inbound rule.

The **Source** is the IPv4 or IPv6 address that is being allowed to connect to the instance. This section is also very important for the project as the end goal of the project is that only those connected to the ETSU Wi-Fi networks should be allowed access (216.145.70.0/23 and 151.141.0.0/16 are the associated IPv4 addresses).

Lastly, the **Description** is an optional text field that can be used to write additional information about the rule.

![](images/AWS_SecurityGroups/image_4.png)

### Outbound Rules

Outbound Rules govern the types of traffic that is allowed out of the EC2 instance. There are multiple different options that can be specified for an outbound rule from the type of traffic, protocol, port range, destination, and description. The type, protocol, port range, and description are the exact same as those mentioned in the Inbound Rules section.

The **Destination** is the IPv4 or IPv6 addresses that are allowed to be contacted by the EC2 instance. This section is very important for the project as the end goal of the project is that only those connected to the ETSU Wi-Fi networks should be able to be accessed by the EC2 instance (216.145.70.0/23 and 151.141.0.0/16 are the associated IPv4 addresses).

![](images/AWS_SecurityGroups/image_5.png)

## Attaching a Security Group to an EC2 Instance

To attach a security group to an EC2 instance, navigate to the instances page of the EC2 service. From here, select the EC2 instance that you want to apply the Security Group to, select the "Actions" drop down menu on the right, select the "Security" section, and select "Change Security Groups".

![](images/AWS_SecurityGroups/image_6.png)

From here, you can view the currently attached security groups and add and remove security groups that are attached to the EC2 Instance.

![](images/AWS_SecurityGroups/image_7.png)

Another way of viewing the currently attached security groups along with their inbound and outbound rules is through the Security Tab on the specific instance's EC2 page.

![](images/AWS_SecurityGroups/image_8.png)

## Team Cooked's Security Groups

### Development Security Group:

![](images/AWS_SecurityGroups/image_9.png)

This Security Group was used for development purposes. It could have been made more secure by limiting access even further to individual IPv4 addresses of the Development Team, but we found this unnecessary. The Rules could also be condensed into a Port Range instead of explicitly allowing the Ports individually.

### Production Security Group (Currently Broken):

![](images/AWS_SecurityGroups/image_10.png)

This Security Group was designed to be used for our production EC2 instance and set up to only allow traffic to and from IPv4 addresses associated with the ETSU Wi-Fi network. It does not currently work though, and we do not know why (although a current idea is that the outbound rules are breaking it). Any future teams that pick this solution up and work with AWS should devote time to fixing this in-class as the ability to test changes that are made is dependent on being on-campus and connected to the ETSU Wi-Fi network. Like the previous Security Group, the Rules could also be condensed into a Port Range instead of explicitly allowing the Ports individually.

## Other Resources

* **Security Groups Overview (with Links to Further Topics)**  
  https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-security-groups.html?icmpid=docs_ec2_console#creating-security-group

* **Creating a Security Group**  
  https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/creating-security-group.html

* **Inbound and Outbound Rules (with Sample Scenarios)**  
  https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/security-group-rules-reference.html?icmpid=docs_ec2_console

---

## Required Images:
* `images/AWS_SecurityGroups/image_1.png`
* `images/AWS_SecurityGroups/image_2.png` 
* `images/AWS_SecurityGroups/image_3.png` 
* `images/AWS_SecurityGroups/image_4.png` 
* `images/AWS_SecurityGroups/image_5.png`
* `images/AWS_SecurityGroups/image_6.png` 
* `images/AWS_SecurityGroups/image_7.png` 
* `images/AWS_SecurityGroups/image_8.png` 
* `images/AWS_SecurityGroups/image_9.png` 
* `images/AWS_SecurityGroups/image_10.png` 
