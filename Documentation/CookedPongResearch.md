# Docker Research

## How to run docker:

* Download docker desktop
* Navigate to project directory in terminal and run `docker-compose up --build`
* View docker images/containers in docker desktop

## What's currently wrong:

* Our docker compose file currently exposes the containers on different ports than the ones that are used locally
* I tried fixing this by changing the ports to the ones that are used locally ex: changed api gateway to port 4141. *These changes did not work so they weren't committed*
* This still doesn't work, because our docker containers are using http rather than https
* When using curl on `http://localhost:4141/Gateway` rather than `https://localhost:4141/Gateway` we can see that it returns 200, meaning the api gateway is running on this port, just not using https

## Potential Solutions:

* We need to configure docker to use https. One possible problem is that we don't have the proper SSL certificates when running our applications containerized.
* Changing URLs for our production environment in `appsettings.json` may also help to resolve our problem, as we should be sending requests to the URLs that our applications are located at in our production environment rather than localhost

---

## Required Images:
*No images were present in this document.*