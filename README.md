# .NET Chat Application

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop)

## Design / Architecture

- Rich Domain Models

- DI with Porperty Injection

- `ChatApi` > (http) > `BotApi` > (message broker) > `ChatApi.Worker`

## Running the project

To run the tests type the following command in the [src directory](./src):

```bash
docker-compose up --build integration-tests
```

To run the dependencies (SqlServer, RabbitMq or Migrations) you could type in the [src directory](./src):

```bash
docker-compose up --build <dependency>
```

> Note: it's important to run the migrations before running the tests and also running the APIs and worker.

To open the specification, open the following in your browser:

```bash
http://localhost/swagger/index.html
```
