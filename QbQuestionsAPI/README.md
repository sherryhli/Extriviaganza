# QbQuestionsAPI

A REST API built with .NET Core.

Root URL: [http://qbquestionsapi.azurewebsites.net](http://qbquestionsapi.azurewebsites.net)

See [endpoints](#endpoints) for available routes. Note that a bearer token is required.

## Requirements

* .NET Core 2.2
* Entity Framework Core
* AutoMapper
* Azure Key Vault

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

## Secret Management

Secrets are stored in Azure Key Vault and are retrieved with their key IDs using the `GetSecretAsync` method from the `Microsoft.Azure.KeyVault` package. Access to secrets is restricted to select users and applications (configured in the Azure portal). When running locally, you must be logged in as an authorized user through the Azure CLI.
```
az login
```

For the deployed application, a system assigned managed identity is obtained for the Azure App Service. This identity is included in the access policies of the Key Vault and is given `Get` access.

## Endpoints

You must obtain a bearer token using `/api/authenticate` and include it in your request header. Note that user creation is restricted to myself at the moment so please contact me if you would like to obtain tokens to test out this API.

### `GET`: `/api/qbquestions/{id}`
Sample response: `200 OK`
```
{
    "id": 3,
    "level": "Middle School",
    "tournament": "Test Tournament",
    "year": 2019,
    "power": "This great-grandson of Queen Victoria infamously led the disastrous Dieppe raid, which resulted in decades of strained relations with Canadians. This figure notably served as Supreme Allied Commander",
    "body": "of the South East Asia Command, and as the last viceroy of India, was tasked with overseeing its independence. For 10 points, name this figure who was assassinated in his fishing boat by the provisional IRA in 1979.",
    "answer": "Louis Mountbatten",
    "notes": "or 1st Earl Mountbatten of Burma, accept Lord Mountbatten"
}
```

### `GET`: `/api/qbquestions/random`
Sample response: `200 OK`
```
 {
    "id": 25,
    "level": "High School",
    "tournament": "PACE NSC",
    "year": 2019,
    "power": "A 1958 book on the \"method of theory\" of the \"American\" form of this discipline stated the authors' belief that this discipline is \"anthropology \r\nor it is nothing.\" A 20th-century practitioner of this discipline adapted Robert Merton's idea of \"middle range theories\" to describe an approach to this discipl\r\nine that involved ethnographic fieldwork among hunter-gatherer societies. Lewis Binford pioneered the \"new,\" or",
    "body": "\"processual,\" form of this discipline. The \"Harris matrix\" is a method of creating seriation diagrams in this discipline that rely on the laws of \"original horizontality,\" \"stratigraphic succession,\" and \"superposition.\" Sites such as GÃ¶bekli Tepe (go-BEK-lee TEH-pay) and Cahokia (kuh-HO-kee-uh) are examined by practitioners of, for 10 points, what academic discipline concerning the excavation of human-built sites?",
    "answer": "archaeology",
    "notes": "[accept ethnoarchaeology or processual archaeology or new archaeology or middle-range archaeology; accept forms of the word archaeology such as archae\r\nological study; do not accept or prompt on \"paleontology\" or similar answers]"
}
```

Optional query string parameter of `level` with 1 = Middle School, 2 = High School, 3 = Collegiate, 4 = Trash. If not specified, the random question can be of any of these levels.

If no question is retrieved, `404 Not Found` is returned.

### `POST`: `/api/qbquestions`
This endpoint accepts bulk requests, the request body must be a JSON array of questions.

Sample request body:
```
[
  {
    "level": 2,
    "tournament": "PACE NSC",
    "year": 2019,
    "power": "A 1958 book on the \"method of theory\" of the \"American\" form of this discipline stated the authors' belief that this discipline is \"anthropology 
or it is nothing.\" A 20th-century practitioner of this discipline adapted Robert Merton's idea of \"middle range theories\" to describe an approach to this discipl
ine that involved ethnographic fieldwork among hunter-gatherer societies. Lewis Binford pioneered the \"new,\" or",
    "body": "\"processual,\" form of this discipline. The \"Harris matrix\" is a method of creating seriation diagrams in this discipline that rely on the laws of \"original horizontality,\" \"stratigraphic succession,\" and \"superposition.\" Sites such as GÃ¶bekli Tepe (go-BEK-lee TEH-pay) and Cahokia (kuh-HO-kee-uh) are examined by practitioners of, for 10 points, what academic discipline concerning the excavation of human-built sites?",
    "answer": "archaeology",
    "notes": "[accept ethnoarchaeology or processual archaeology or new archaeology or middle-range archaeology; accept forms of the word archaeology such as archae
ological study; do not accept or prompt on \"paleontology\" or similar answers]"
  }
]
```

Sample response: `200 OK`
```
[
    {
        "id": 25,
        "level": "High School",
        "tournament": "PACE NSC",
        "year": 2019,
        "power": "A 1958 book on the \"method of theory\" of the \"American\" form of this discipline stated the authors' belief that this discipline is \"anthropology \r\nor it is nothing.\" A 20th-century practitioner of this discipline adapted Robert Merton's idea of \"middle range theories\" to describe an approach to this discipl\r\nine that involved ethnographic fieldwork among hunter-gatherer societies. Lewis Binford pioneered the \"new,\" or",
        "body": "\"processual,\" form of this discipline. The \"Harris matrix\" is a method of creating seriation diagrams in this discipline that rely on the laws of \"original horizontality,\" \"stratigraphic succession,\" and \"superposition.\" Sites such as GÃ¶bekli Tepe (go-BEK-lee TEH-pay) and Cahokia (kuh-HO-kee-uh) are examined by practitioners of, for 10 points, what academic discipline concerning the excavation of human-built sites?",
        "answer": "archaeology",
        "notes": "[accept ethnoarchaeology or processual archaeology or new archaeology or middle-range archaeology; accept forms of the word archaeology such as archae\r\nological study; do not accept or prompt on \"paleontology\" or similar answers]"
    }
]
```

### `PUT`: `/api/qbquestions/{id}`
Sample request body:
```
{
    "level": 1,
    "tournament": "Updated Test Tournament",
    "year": 2019,
    "power": "This great-grandson of Queen Victoria infamously led the disastrous Dieppe raid, which resulted in decades of strained relations with Canadians. This figure notably served as Supreme Allied Commander",
    "body": "of the South East Asia Command, and as the last viceroy of India, was tasked with overseeing its independence. For 10 points, name this figure who was assassinated in his fishing boat by the provisional IRA in 1979.",
    "answer": "Louis Mountbatten",
    "notes": "or 1st Earl Mountbatten of Burma, accept Lord Mountbatten"
}
```

Sample response: `200 OK`
```
{
    "id": 3,
    "level": "Middle School",
    "tournament": "Updated Test Tournament",
    "year": 2019,
    "power": "This great-grandson of Queen Victoria infamously led the disastrous Dieppe raid, which resulted in decades of strained relations with Canadians. This figure notably served as Supreme Allied Commander",
    "body": "of the South East Asia Command, and as the last viceroy of India, was tasked with overseeing its independence. For 10 points, name this figure who was assassinated in his fishing boat by the provisional IRA in 1979.",
    "answer": "Louis Mountbatten",
    "notes": "or 1st Earl Mountbatten of Burma, accept Lord Mountbatten"
}
```

### `DELETE`: `/api/qbquestions/{id}`
Sample response: `200 OK`
```
{
    "id": 3,
    "level": "Middle School",
    "tournament": "Test Tournament",
    "year": 2019,
    "power": "This great-grandson of Queen Victoria infamously led the disastrous Dieppe raid, which resulted in decades of strained relations with Canadians. This figure notably served as Supreme Allied Commander",
    "body": "of the South East Asia Command, and as the last viceroy of India, was tasked with overseeing its independence. For 10 points, name this figure who was assassinated in his fishing boat by the provisional IRA in 1979.",
    "answer": "Louis Mountbatten",
    "notes": "or 1st Earl Mountbatten of Burma, accept Lord Mountbatten"
}
```

## Deployment

This API is containerized with Docker and deployed to Azure App Services. Images are pushed to a private Azure Container Registry (ACR) where they are used by App Services to create a web app. Continuous deployment is enabled so the App Service is updated whenever a new version of the image is pushed to the registry. You must be authenticated with ACR in order to push (credentials can be found through the CLI or through Azure Portal).

```
docker build -t <my-registry-name>.azurecr.io/qb-questions-api:<version> .
docker push <my-registry-name>.azurecr.io/qb-questions-api:<version>
```

Images are tagged with a version and versioning is done manually for now using MAJOR.MINOR.PATCH format. MAJOR is incremented for breaking changes, MINOR for backwards-compatible feature additions, and PATCH for bug fixes.

Ensure that outbound IP addresses of the web app are added to database security settings. Obtain IPs with the Azure CLI:
```
az webapp show --resource-group <group_name> --name <app_name> --query outboundIpAddresses --output tsv
```

The app runs on port 80, and it has already been configured to do so but if this ever needs to be updated, run:
```
az webapp config appsettings set --resource-group <group_name> --name <app_name> --settings WEBSITES_PORT=80
```