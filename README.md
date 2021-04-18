# IPConsumer Web API
Built with .NET Core 5.0
## Consists of: 
 - Common - Library Project holding common models.
 - IPLibrary - Library Project to consume service from IPStack.
 - API - Web API project with IPManagerController. Functionality includes:
    - [GET] Details/{ip}: Get IP Details from Memory Cache, Our Repository, or IPStack.
    - [GET] GetDetailsFromDB: Get List of IP details from Database.
    - [GET] GetDetailsFromDB/{ip}:  Get IP details from Database by IP.
    - [POST] BatchUpdateWithProgressReport: Update IP details records in bulk. Separate thread to be created. GUID returned.
    - [GET] BatchUpdateProgress: Read task progress report by Guid.
 - API.UnitTests - Unit Test Project consisting of tests for IPManagerService.
 
