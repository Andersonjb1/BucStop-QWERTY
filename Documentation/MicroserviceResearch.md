# Microservice Research Documentation

## Current Implementation

- Game scripts are currently loaded into the "Play" view from the `wwwroot` folder in the web app
- `GameInfo` (JSON data passed from each game microservice to the API gateway and then to the web app) contains the filepath for each game's JavaScript file
- Potential solution: Host JavaScript files from the game microservice and pass URL instead of filepath

## Proposed Solution Example

Instead of the current approach where Snake returns `~/js/snake.js`, we could:

- Run Snake on `https://localhost:1948`
- Return `https://localhost:1948/js/snake.js`

**Required Changes:**
- Updates across each game microservice
- Updates to the API gateway
- Updates to the Web App

## Implementation Implications

### Technical Considerations

- **Static File Hosting**: Microservices may need to be enabled to host static files
- **Production Environment**: Using localhost may not work in production
- **API Gateway**: Currently non-functional and needs to be working first
- **Dependencies**: Games might have dependencies beyond just a single JS file

### Current Issues

- Files are unorganized and need proper organization
- Misplaced files need to be identified and relocated
- CORS may be needed to connect with the Gateway
- May need new fetching URL methods to replace current implementation

## CORS Configuration

### Current State
CORS is not actively being used within the WebApp, which explains why certain API calls are failing.

### Required CORS Setup
- **WebApp → API Gateway**: Enable CORS in API Gateway (different origins)
- **API Gateway → Microservices**: No CORS needed (server-to-server communication)

## Additional Resources

Last semester's microservices documentation: [Google Doc](https://docs.google.com/document/d/1614BGhXJ8EkGg9p286xH0KazdWtSf83aGFW192Is-DI/edit?tab=t.0)