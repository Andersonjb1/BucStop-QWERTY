# BucStop

![BucStop Logo](/Bucstop_WebApp/BucStop/wwwroot/Logo.png)

## Overview

BucStop is a modern microservices-based gaming platform developed as part of the Software Engineering II course. <br>This platform features classic arcade games (Snake, Tetris, and Pong) with a clean, responsive UI and a scalable architecture designed for cloud deployment.

[▶️ Watch the BucStop Intro & Demo Video](https://vimeo.com/1079595088/f69404c8a6?ts=0&share=copy)



## Architecture

The application is built using a microservices architecture with the following components:

- **WebApp**: Main frontend service that handles user authentication, game selection, and user interface
- **API Gateway**: Orchestrates communication between the WebApp and game microservices
- **Submission Gateway**: In a future release, it will allow users to submit their own games.
- **Game Microservices**: Independent services for each game (Snake, Tetris, Pong)

![Architecture Diagram](/Documentation/CookedDocumentation/CookedGraph.png)

## Technologies

- **Backend**: ASP.NET Core 
- **Frontend**: HTML5, CSS3, JavaScript
- **Containerization**: Docker, Docker Compose
- **Deployment**: AWS EC2
- **Logging**: Serilog
- **CI/CD**: GitHub Actions


## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) and Docker Compose
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (for development only)
- [Git](https://git-scm.com/downloads)

### Local Development (with containerization)

1. Clone the repository:
   ```bash
   git clone https://github.com/Andersonjb1/BucStop-QWERTY.git
   cd BucStop-QWERTY
   ```

2. Start all services locally using Docker Compose specifying the .dev version:
   ```
   sudo env=containersLocal docker compose -f docker-compose.dev.yml build --no-cache
   ```

3. Access the application:
   - WebApp: http://localhost:8080
   - API Gateway: http://localhost:8081
   - Snake: http://localhost:8082
   - Pong: http://localhost:8083
   - Tetris: http://localhost:8084

### Local Development Without Docker

While Docker Compose manages service discovery and networking between containers, Visual Studio provides a powerful <br> alternative for development that will probably be more familiar for students through its multiple project startup feature.

#### Using Visual Studio

1. Open the solution file `BucStop.sln` in Visual Studio.

2. Configure multiple startup projects:
   - Right-click on the Solution in Solution Explorer and select "add existing project"
   - Select the `csproj` file for each other service.
	 - Right click solution and select "configure multi-project startup".
   - Set the following projects to "Start":
     - `BucStop` (WebApp)
     - `APIGateway`
     - `Snake`
     - `Pong`
     - `Tetris`
   - Configure the startup order with the API Gateway first, followed by the game services, and finally the WebApp
   - Click "OK" to save the configuration

3. Press F5 or click the "Start" button to run all projects simultaneously.

Visual Studio automatically handles:
- Starting each project on a different port
- Configuring the correct environment variables
- Launching debug sessions for each project



## Deployment to AWS

### Setting Up AWS Resources

__See (Documentation/AWS-Setup)__

### Environment Configuration

The application supports multiple environments through configuration files:

- `appsettings.Development.json`: Local development settings (please don't use this - for your own sanity)
- `appsettings.containersLocal.json`: Local Docker container settings **(Default Config)**
- `appsettings.containers.json`: Production container settings
- `appsettings.Production.json`: Production settings (currently deprecated - consider removing)

When deploying to production, use the appropriate environment variable (consider persisting environment variable<br> by adding it to `.bashrc` or `/etc/profile`):

```bash
env=containers docker-compose up -d
```

## Project Structure

```
BucStop-QWERTY/
├── BucStop_WebApp/            # Main web application
│   └── BucStop/
│       ├── Controllers/       # MVC controllers
│       ├── Views/             # UI templates
│       ├── Models/            # Data models
│       ├── Services/          # Business logic
│       └── MicroServices/     # Service communication
├── BucStop_SubmissionGateway  # Submission Gateway service
├── Team-3-BucStop_APIGateway/ # API Gateway service
├── Team-3-BucStop_Snake/      # Snake game microservice
├── Team-3-BucStop_Tetris/     # Tetris game microservice
├── Team-3-BucStop_Pong/       # Pong game microservice
├── Documentation/             # Project documentation
└── docker-compose.yml         # Container orchestration
```

## Contributing

1. Clone the repository 
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Troubleshooting

### Common Issues

1. **Services not connecting properly**:
   - Ensure all services are running (`docker ps`)
   - Check if the API Gateway is configured with correct service URLs
   - Verify network connectivity between containers

2. **Game not loading**:
   - Check browser console for JavaScript errors
   - Verify that the game's microservice is running
   - Check API Gateway logs for routing issues

### Logs

All services use Serilog for structured logging:

```bash
# View logs for all containers
docker compose logs

# View logs for a specific service
docker compose logs bucstop
docker compose logs api-gateway
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

* Original BucStop project that served as foundation from previous semesters
* All contributors to the project from the most recent semester:

	- @Teal_04, @rhanc, @LoftKl, @Andersonjb1, @T0xen, @JoeNeglia, @Kataruse, @Derek214, @K-Baker101020

* Software Engineering II course instructor, Professor Kinser
