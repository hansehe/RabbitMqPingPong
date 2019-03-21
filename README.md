# dotnetService

## Introduction
Dotnet core service with ping pong capabilities using Rabbitmq!
Implements both mqtt and amqp messaging! :D

Follow the steps below to get the service up and running! :)

## Get Started
1. Install [Docker](https://www.docker.com/)
2. Install [Python](https://www.python.org/) and [pip](https://pypi.org/project/pip/)
    - Windows: https://www.python.org/downloads/windows/
        - Be sure to add python and pip to system environment variables PATH.
    - Ubuntu: Python is installed by default
        - Install pip: sudo apt-get install python-pip
3. Install `DockerBuildManagement` build system tool:
    - pip install --update DockerBuildManagement
4. See available commands with [DockerBuildManagement](https://github.com/DIPSAS/DockerBuildManagement) using the `dbm` cli:
    - `dbm -help`

## Build & Run
1. Start domain development by deploying service dependencies:
    - `dbm -swarm -start`
    - The `-swarm -start` command uses [SwarmManagement](https://github.com/DIPSAS/SwarmManagement) deployment tool to deploy all services as described in [src/ServiceDependencies/swarm-management.yml](src/ServiceDependencies/swarm-management.yml) to your local Docker Swarm.
    - One of the deployed services is [Portainer](https://www.portainer.io/), and you can access it at [http://localhost:9000](http://localhost:9000) to manage all your running services.
2. Test solution in containers:
    - `dbm -test`
3. Build and run solution as container images:
    - `dbm -build -run dotnetService`
    - Send a request to the service to publish an event message
        - http://localhost:5000/api/event/
        - Take note of the event id from the response.
    - The service subscribes on the event message, so it should have been handled:
        - http://localhost:5000/api/search/<event_id>
    - It is also possible to build a standalone executable of the service:
        - !Note: You need to have dotnet core installed to build the executable: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
        - `dbm -build dotnetServiceStandalone`
        - Find the standalone executable in the generated [output](output/) folder.
4. Open solution and continue development:
    - [src/RabbitMqPingPong](src/RabbitMqPingPong)
    - !Note: Be aware that Visual Studios/Rider/VSCode sets the working directory to the project directory, but correct working directory should be in `<project_dir>/bin/debug/netcoreapp2.2/`.
5. Publish new docker image:
    - Bump version in [CHANGELOG.md](CHANGELOG.md)
    - Publish docker image: `dbm -publish`
6. Stop all running services:
    - `dbm -swarm -stop`

## Build System
- [DockerBuildSystem](https://github.com/DIPSAS/DockerBuildSystem)
- [SwarmManagement](https://github.com/DIPSAS/SwarmManagement)
- [DockerBuildManagement](https://github.com/DIPSAS/DockerBuildManagement)

## Maintainers:
- Team Frost