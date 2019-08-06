# QbQuestionsAPI

A REST API built with .NET Core.

## Requirements

* .NET Core 2.2
* Entity Framework Core
* AutoMapper

## Data

This API interacts with a SQL Server database hosted in Azure. Entity Framework Core is used "code first" as the ORM.

Whenever changes are made to models, run the following to generate necessary migration files:
```
dotnet ef migrations add [name]
```

To apply database updates, run:
```
dotnet ef database update
```