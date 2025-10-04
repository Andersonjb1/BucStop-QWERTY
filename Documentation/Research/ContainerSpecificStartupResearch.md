## Key points

- We need to be able to start up only parts of docker at a time so that we only have to redeploy microservices that changed.

## General Information & Notes

In our current solution, profiles do not work for some reason. I created a branch for the sake of getting the images for this document and testing the profiles but whenever I use the profile command, it still just starts all the services. Additionally, I also had an issue with it where for some reason after I closed it I just kept getting a message in the terminal saying that the API GW container needed one of the game containers at random, preventing both docker compose down and up from working (despite stopping and running docker compose down before the issue). I’d recommend holding off on profiles for the moment and just naming any of the services that need to be restarted individually.

Another note regarding taking down and restarting specific containers is that we need to be wary of dependencies between services. If we have to take down a game service, then we will also need to take down the API GW service or it might cause unexpected consequences. By extension, if we take down the API GW, we would also have to take down the bucstop service since it also depends on the API GW service. Basically, in our current implementation we can take down the bucstop service on its own but any other container could cause a ripple effect if we were to just take it down.

## Commands

In order to restart containers when changes are made to them, we must stop the specific containers rather than all containers. This can be done using the following commands:
```docker stop <container name>```
```docker container rm <container name>```

There are a few different ways to start multiple docker containers:

We can specify to just start a service or services that are contained in our docker-compose.yml file:
```docker compose up -d <service name> <service name>```

### Profiles

Profiles can also be specified in our docker-compose.yml file that we can use to start up multiple services at a time without specifying all of the individual containers. This is done by adding the “profiles” flag to services in our and profile names. I added a profile called “games” to all 3 of the games which can be seen in Figure 1. You can also assign multiple profiles to a service in 2 different ways, as shown in Figure 2. According to Docker’s documentation, the command to start up profiles are:

Start a single specified profile -
```docker compose --profile <profile name> up```
```COMPOSE_PROFILES=<profile name> docker compose up```

Start multiple specified profiles -
```docker compose --profile <profile name> --profile <profile name> up```
```COMPOSE_PROFILES=<profile name>,<profile name> docker compose up```
Start up every profile -
```docker compose --profile “*” up```

Figure 1 - "games" Profile Tied to Snake
 
Figure 2 - Multiple Profiles Tied to a Service

## Helpful Sources

Using profiles with Compose -
https://docs.docker.com/compose/how-tos/profiles/

Learn how to use profiles in Docker Compose -
https://docs.docker.com/reference/compose-file/profiles/
