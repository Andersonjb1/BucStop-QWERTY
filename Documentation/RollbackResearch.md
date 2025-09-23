# Rollback Research

## Potential solutions for keeping persistent data during rollback:

### 1. Docker volumes

**Documentation Link:** https://docs.docker.com/engine/storage/volumes/

First need to mount a volume, which gives the volume access to a specific directory inside a container (ex. `Playcount.json` or log files)

* **Define/Create the volume**
* **Volumes can be mounted and defined inside of docker compose.** Running `docker compose up` for the first time creates a volume. Docker reuses the same volume when you run the command subsequently.

![Docker volume configuration example](images/image_1.png)

## Some Implications for Docker Volumes:

* `Docker system prune` command in deployment script may remove volumes
* If a new EC2 instance is created, volumes will most likely be lost since they are stored on the EC2 host machine (Could use AWS Elastic Block Storage to store volumes)

**Link to this document for editing:**  
https://docs.google.com/document/d/11LTixLWicBxM4XUPWyNRi4D5uFsL58xRFvQ-kOm0b9s/edit?usp=sharing

---

## Required Images:
* `images/image_1.png` - "Docker volume configuration example"