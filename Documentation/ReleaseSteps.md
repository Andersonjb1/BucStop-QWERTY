# Steps for Releasing Solution after Sprint Review

1. Get into AWS environment (Start timer)

### Terminal Commands

2. Ensure there are no running containers 
3. Switch to development branch (n = sprint #) 
4. Start up the project 
 
```
# Step 2
sudo docker ps
# Step 3
git fetch
git checkout Dev-<n>
git pull
# Step 4
sudo docker compose build --no-cache
sudo env=containers docker compose up -d
```

### BucStop Confirmation

5. Connect to the BucStop website
6. Confirm that the games work

### GitHub

7. Create pull request for Dev-n into Sprint-n
8. Code reviews to merge Dev-n into Sprint-n
9. Merge Dev-n into Sprint-n

### Terminal Commands

10. Switch to Sprint-n in docker
11. Rebuild website
```
# Step 10
git fetch
git checkout Sprint-<n>
git pull
# Step 11
sudo docker compose build --no-cache
sudo env=containers docker compose up -d
```

### Final Confirmation
12. Connect to website to confirm it works
13. Create a rollback snapshot for the release
14. Create a release on GitHub (Stop timer)
