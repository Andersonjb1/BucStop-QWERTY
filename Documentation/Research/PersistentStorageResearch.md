## Key points

-	We need to store data for play counts of each game persistently in production, without letting our plays while testing affect it.
- Docker has container volumes that allow for data to persist beyond a single instance of a container running.

## General Information

- The same volume can be attached to multiple containers to share files between containers
- You can mount non-empty volumes to a container that has pre-existing files, but the pre-existing files are obscured by the mount. The best way to reveal the obscured files again is to recreate the container without the mount. However, if you mount an empty volume to a container that has pre-existing files, the files are copied into the volume (unless “volume-nocopy” is used when mounting).

## Using Volumes

### Volume Creation & Mounting

In order to get started you have to create a volume, which can be done with two commands:
```
docker volume create <volume name>
```

```
docker run -d -p 80:80 -v <volume name>:/logs docker/welcome-to-docker
```
The 2nd command will mount/attach the volume to a container and can be used with preexisting volumes, but if the volume does not exist it will be created then. Additionally with that command all files will be placed in the /logs folder for any container with that same name so the files will always be there when an instance of it is running.

A volume can also be mounted with the docker run command, using either the --mount (preferred) or ---volume flags:	
```
docker run --mount type=volume,src=<volume-name>,dst=<mount-path>
```

```
docker run --volume <volume-name>:<mount-path>
```

### Volume Management

Since volumes live for longer than containers, they can grow quite large, so there are some commands that are useful for managing volumes:

List all volumes -
```
docker volume ls
```

Remove a volume that is not attached to a container - 
```
docker volume rm <volume-name-or-id>
```

Remove all unattached volumes -
```
docker volume prune	
```

Forcefully remove a container by stopping the container then removing it so that a volume can be deleted - 
```
docker rm -f <container name>
```

Inspect a volume and display information about it -
```
docker volume inspect <volume name>
```

Verify that Docker created the volume and that it mounted correctly -
```
docker inspect devtest
```

### Volume Content Viewing

From the Docker Desktop Dashboard you can find a tab called Volumes that displays the volumes for the current solution and allows you to click on them. When viewing a volume you can read, update, and delete the files within the volume.
You can use the following command to view output from logs (which should show the data from files in volumes):
```
docker compose logs
```

### Volume Backup & Restoration

It is in our best interest to set up/at least know how to backup our volumes. I found two somewhat different commands to do it, with one being the command from Docker’s documentation on Volumes and the other being a more generalized one from stack overflow.

Docker command:
```
docker run --rm --volumes-from <volume name> -v $(pwd):/backup ubuntu tar cvf /backup/<backup filename>.tar /<container name>`
```
Stack Overflow command:
```
docker run --rm --mount source=<volume-name>,target=<target> -v $(pwd):/backup busybox tar -czvf /backup/<backup-filename>.tar.gz <target>
```
- target is the mount point in the container

Both commands mount a local host directory as /backup before passing another command that tars the contents of the volume into a .tar backup file.

Both of the previously mentioned sources also had commands to restore volumes based on backups where the backup file is un-tarred in a container’s data volume.

Docker command:
```
docker run --rm --volumes-from <container name> -v $(pwd):/backup ubuntu bash -c "cd /<volume name> && tar xvf /backup/<backup-filename>.tar --strip 1"
```
Stack Overflow command:
```
docker run --rm --mount source=<volume-name>,target=<target> -v $(pwd):/backup busybox tar -xzvf /backup/<backup-filename>.tar.gz -C /
```

## Alternative Methods

Bind mounts are good if you need to access files/directories from both containers and the host, but I don’t see a reason for us to use these.

## Helpful Sources

Docker's Documentation on Persisting Container Data -
https://docs.docker.com/get-started/docker-concepts/running-containers/persisting-container-data/

Docker's Documentation on Volumes -
https://docs.docker.com/engine/storage/volumes/

Stack Overflow Volume Backup and Restoration Post -
https://stackoverflow.com/questions/79246538/simple-way-to-backup-restore-docker-named-volumes

Stack Overflow Post About docker compose down Deleting Volumes -
https://stackoverflow.com/questions/65799945/why-docker-compose-down-deletes-my-volume-how-to-avoid-this-action-done-by-dow
