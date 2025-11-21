# API Submission Gateway Information

**Author:** Nicholas Crump  
**Sprint:** 9 
**Associated PBI/Task:** Create file that Explains what we did for Submissions

# Overview
- Our MVP for Fall 2025 was to create the API Submission Gateway to allow for games to be submitted to through a game submission page and appear on the Admin's Game Page for review. Our current implementation does not create new microservices for each game submitted and purely has implemntation to remove unapproved games from the Admin games Page

# High-Level Flow of How the Overall Submission Process Works
- A user submits a game that fits the required criteria for a game submission.
- That submission is then wrote to a docker volume.
- When a admin navigates to the admin page the API Submission Gateway will access and read from that docker volume of the BucStop Webservice which then displays the games within the volume.

# How the API Submission Gateway
- The core of the API Submission Gateway was essentially copied from the API Gateway since the general process is the same, we need to take some data and translate it in the proper order to allow for the games page of BucStop Web to display and play the game. The main difference here is that our data is coming from 2 files in each submission in a docker volume as opposed to a microservice. This changes things a bit with actually converting the data but overall they're pretty identical.
- The main difference is with the leaderboards, we believe that the API Gateway didn't have these implemented fully either but there is a leaderboard attribute that has dummy data in it as opposed to NULL which is what the API Gateway has for this attribute, hopefully this will help jumpstart some development on that in the future potentially.

# Improvements to Make
- This implementation should be changed at some point in the future as this volume shouldn't be accessed by both BucStop Web and the Submission API, we were thinking that Bucstop should send this volume data to the Submission API directly instead of both separately accessing the volume to upheld the separation of these microservices.
- Beyond this the ability to accept games on the admin page and create them into a new microservice that can be displayed on the games page would be what we'd work on next since this would fully complete the flow of BucStop.