# Design

## Project
According to the requirement document, it says: Not have a UI, CLI, or database, so I chose ASP.Net Web API 
There are four controllers: ReleaseContoller, ProjectController, DeploymentController, ProjectEnvironmentController
All REST APIs are tested using Postman.

With regard to the requirement, only one REST is necessary in ReleaseController its route is called "GetReleases".
so for testing, I only covered this function. In practices, all functions will need testing. 

## Test
The project integrates with Swagger. So for the testing, I chose to use NUnit.
SwaggerIntegration Class is to create webhost and setup configuration for the test, helps to start the test and do the disposal etc.
GenerateAPIs is to generate RestAPIs for the main project and exports them into the file called WebAPI.cs
ReleaseTest.cs is the test file for release REST APIs. It calls exported function saved in WebAPI.cs

All functions should be tested, but as for the problem, it only covers 5 test cases. 

## Swagger
The project integrates with Swagger, so for each REST, users can see request data model and their responses. 


## Logging
It supports Console Logging and File Logging. Console Logging is probably not necessary in practice, but for debugging purpose here, I added it in the projects.

After running the application, log is saved in log folder, so users can check the details of logging why releases are kept. logging looks like this.

2021-08-07T15:51:12.7987921+10:00 0HMAPCTD72V89:00000002 [INF] GetReleaseByProjectId: ProjectId Project-1 Random Quotes EnviornmentId Environment-1 Staging (1bc3da1a)
2021-08-07T15:51:12.8146980+10:00 0HMAPCTD72V89:00000002 [INF] There are 2 release for project Project-1 on environment Environment-1: (f424b664)
2021-08-07T15:51:12.8152559+10:00 0HMAPCTD72V89:00000002 [INF] | Release-2 (Version: 1.0.1, Created: 2/01/2000 9:00:00 AM) | Deployment-2 (DeployedAt: 2/01/2000 10:00:00 AM) (8fa7a44f)
2021-08-07T15:51:12.8153083+10:00 0HMAPCTD72V89:00000002 [INF] | Release-1 (Version: 1.0.0, Created: 1/01/2000 9:00:00 AM) | Deployment-1 (DeployedAt: 1/01/2000 10:00:00 AM) (8db50c61)
2021-08-07T15:51:12.8153336+10:00 0HMAPCTD72V89:00000002 [INF] The most recently 1 releases for Project Project-1 on Environment Environment-1:  (b7cf03a2)
2021-08-07T15:51:12.8154172+10:00 0HMAPCTD72V89:00000002 [INF] Release-2 kept because it was the most recently deployed  (4fc79b56)
