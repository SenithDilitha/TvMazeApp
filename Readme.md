# TvMazeApp  

A .NET application that integrates with the TVMaze API to fetch and manage TV shows.  

## 1. Introduction  
TvMazeApp is a .NET-based application designed to fetch and manage TV show data. It provides RESTful API endpoints for retrieving, updating, and deleting show records.  

## 2. Prerequisites  
Ensure you have the following installed before proceeding:  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)  

## 3. Clone the Repository  
```sh  
git clone https://github.com/SenithDilitha/TvMazeApp.git  
cd TvMazeApp  
```  

## 4. Configure Database Connection  
Update `appsettings.json` in the `TvMazeApp.API` project to set up the database connection string:  
```json  
"ConnectionStrings": {  
  "DefaultConnection": "server=<DatabaseServer>;database=<DatabaseName>;User=<username>;Password=<password>;TrustServerCertificate=True"  
}  
```  

## 5. Restore Dependencies  
Run the following command in the project root directory to restore required dependencies:  
```sh  
dotnet restore  
```  

## 6. Apply Migrations and Update the Database  
Apply the EF Core migrations to create and update the database schema:  
```sh  
dotnet ef database update --project .\TvMazeApp.Infrastructure\ --startup-project .\TvMazeApp.API\  
```  

## 7. Run the Application  
To start the API server, run:  
```sh  
dotnet run --project .\TvMazeApp.API\  
```  

## 8. API Endpoints  
Once the application is running, use the following API endpoints in postman:  

### a. Fetch Movies from TVMaze  
```sh  
curl --location --request GET 'http://localhost:5224/api/Shows/fetch'  
```  

### b. Get All Shows  
```sh  
curl --location --request GET 'http://localhost:5224/api/Shows'  
```  

### c. Update All Properties of a Show  
```sh  
curl --location --request PUT 'http://localhost:5224/api/Shows/5' \  
--header 'Content-Type: application/json' \  
--data-raw '{  
  "id": 5,  
  "name": "string",  
  "language": "English",  
  "premiered": "2025-02-02T17:57:41.757Z",  
  "summary": "string",  
  "genres": [  
    "Crime"  
  ]  
}'  
```  

### d. Delete a Show  
```sh  
curl --location --request DELETE 'http://localhost:5224/api/Shows/5'  
```  
