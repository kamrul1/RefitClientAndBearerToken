
# Refit Client And Bearer Token

This is a demo showing how to use Refit client to call an API protected by identity provider.  

##### There are 4 projects in this solution.  Their purpose is:
---
##### RefitClientAndBearerToken.ConsoleClient

This console project shows how to use Refit client to firstly get a bearer authorisation token from and Identity Provider and then use it to call a web api which requires it.

Key things to note are attribute in Refit interface file IApi.cs

**[Body(BodySerializationMethod.UrlEncoded)]**
This caused the content type to ***application/x-www-form-urlencoded***

**[Authorize("Bearer")]**
This is the parameter when added causes the Authorization bearer token to be added to the header.  It must be suppled on calling the method.



---

##### RefitClientAndBearerToken.IDP

This is the main identity provider, self contained in this solution.  I have used Identity Server 4 so that I can include its configuration in this project.  This can be swapped out for any external IDP, which support OpenID Connect.

OpenID Connect Discovery Document:
https://localhost:5001/.well-known/openid-configuration

Configure and Setup IdentityServer4 using the guide at:
https://www.scottbrady91.com/Identity-Server/Getting-Started-with-IdentityServer-4


See the Config.cs file for Resource setup.


Test to see it using curl, get a bearer token:

```

curl --location --request POST 'https://localhost:5001/connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=client_credentials' \
--data-urlencode 'scope=api1.read' \
--data-urlencode 'client_id=oauthClient' \
--data-urlencode 'client_secret=SuperSecretPassword'

```

This will return the following token:

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkEyQTIyMEM1MTU1QUY2OEVCQjUzRTJCNjdCQ0ZGMkM1IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MTkwOTQ5MDgsImV4cCI6MTYxOTA5ODUwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6WyJhcGkxIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMS9yZXNvdXJjZXMiXSwiY2xpZW50X2lkIjoib2F1dGhDbGllbnQiLCJqdGkiOiI1ODlFQkM2RDhFOTJCREU2QUI2QkYyMDFBNEVEOUZBQiIsImlhdCI6MTYxOTA5NDkwOCwic2NvcGUiOlsiYXBpMS5yZWFkIl19.fr6L8vstnhTw5PizeryG2_bjA9p7sbfTahoIDh4cZoGtVPSqGSIb6W748dveZfYbcAZiG6Z7ZAnCjpvmVV9mrS1ISChfg6qprCdwEK-4mwkW-FSsSGAwUNk6HNHzYWn5Xw_1g9VkxfVzKpg7dQt0igXho4QsMcnoSLxwes9tL2I95LdzDjUdJ7b7GtZ1b1hqQN_gKhghSWONJH-8R9ViUUXCoRjizcpyjr82X4ZUCx_B-S_A9Hs0u0DFNzUyxHV39RLZuXTkfMl-DarFd9g9-75r-d1jsm5fFZJT5wU92vbiStrOC-kAvCDMsZdZ1r8mcL5vk9iJI5lIxWWrwheJyw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "scope": "api1.read"
}
```

In order to make default some of the values, a controller can be setup to call the /connect/token endpoint.  This way the user will only need to supply their username and password.  The user does not need to supply the scope and grant_type fields.  This feature is implemented in the TokenController.  This allows the IDP to be called like:

```
curl --location --request POST 'https://localhost:5001/api/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'username=oauthClient' \
--data-urlencode 'password=SuperSecretPassword'
```

---
##### RefitClientAndBearerToken.Library
This project has mainly PCO models and a extension method added to the TokenResponse class to map it to OutputToken.  The OutToken is decorated with JsonPropertyNameAttribute so that when it is searlized the JSON names are same as that normally supplied by identity provider.

---
##### RefitClientAndBearerToken.WebApi

This is simple web api, that is rigged to require the bearer token from the IDP.  Once you have the bearer token you can call it using:

The key lines which configures all calls to it to have a bearer token from the IDP is in Startup.cs:
```
services.AddAuthentication("Bearer")
.AddIdentityServerAuthentication("Bearer", options =>
{
    options.ApiName = Configuration["RefitClientAndBearerTokenIDP:apiName"];
    options.Authority = Configuration["RefitClientAndBearerTokenIDP:authority"];
});
```

Test to see it using curl, get the data from the Web API controller, RefitClientAndBearerToken.WebApi project, with the bear token
```
curl --location --request GET 'https://localhost:44323/WeatherForecast' \
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkEyQTIyMEM1MTU1QUY2OEVCQjUzRTJCNjdCQ0ZGMkM1IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MTkwOTQ5MDgsImV4cCI6MTYxOTA5ODUwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6WyJhcGkxIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMS9yZXNvdXJjZXMiXSwiY2xpZW50X2lkIjoib2F1dGhDbGllbnQiLCJqdGkiOiI1ODlFQkM2RDhFOTJCREU2QUI2QkYyMDFBNEVEOUZBQiIsImlhdCI6MTYxOTA5NDkwOCwic2NvcGUiOlsiYXBpMS5yZWFkIl19.fr6L8vstnhTw5PizeryG2_bjA9p7sbfTahoIDh4cZoGtVPSqGSIb6W748dveZfYbcAZiG6Z7ZAnCjpvmVV9mrS1ISChfg6qprCdwEK-4mwkW-FSsSGAwUNk6HNHzYWn5Xw_1g9VkxfVzKpg7dQt0igXho4QsMcnoSLxwes9tL2I95LdzDjUdJ7b7GtZ1b1hqQN_gKhghSWONJH-8R9ViUUXCoRjizcpyjr82X4ZUCx_B-S_A9Hs0u0DFNzUyxHV39RLZuXTkfMl-DarFd9g9-75r-d1jsm5fFZJT5wU92vbiStrOC-kAvCDMsZdZ1r8mcL5vk9iJI5lIxWWrwheJyw'
```
This will return the weather Json:
```
[
    {
        "date": "2021-04-23T13:38:42.1453891+01:00",
        "temperatureC": 28,
        "temperatureF": 82,
        "summary": "Bracing"
    },
    {
        "date": "2021-04-24T13:38:42.1457542+01:00",
        "temperatureC": -19,
        "temperatureF": -2,
        "summary": "Warm"
    },
    {
        "date": "2021-04-25T13:38:42.1457567+01:00",
        "temperatureC": -19,
        "temperatureF": -2,
        "summary": "Hot"
    },
    {
        "date": "2021-04-26T13:38:42.1457577+01:00",
        "temperatureC": 3,
        "temperatureF": 37,
        "summary": "Scorching"
    },
    {
        "date": "2021-04-27T13:38:42.1457581+01:00",
        "temperatureC": -4,
        "temperatureF": 25,
        "summary": "Hot"
    }
]
```

---
appsettings.json is included to show the variable passed.  In production you would store these in environment variables or swap them out in the build. 


---
[Postman](https://www.postman.com/) collection is included in folder PostmanSamples, which you can use to test using your REST API testing tool.