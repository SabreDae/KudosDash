# KudosDash

KudosDash is an [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) MVC web application built with .NET 8. The site utilises a SQLite database, [Entity Framework](https://learn.microsoft.com/en-us/ef/) and [Identity](https://learn.microsoft.com/en-us/aspnet/identity/overview/getting-started/introduction-to-aspnet-identity#aspnet-identity) to manage database records created and edited in the web application. 

The production KudosDash POC is hosted at https://kudosdash.onrender.com. Deployment is automated via Github pipelines.

![Github pipeline screenshot](https://private-user-images.githubusercontent.com/102368837/311304773-215f5f55-b76a-4ab5-a738-ddd2b6761196.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MDk5MTcxOTIsIm5iZiI6MTcwOTkxNjg5MiwicGF0aCI6Ii8xMDIzNjg4MzcvMzExMzA0NzczLTIxNWY1ZjU1LWI3NmEtNGFiNS1hNzM4LWRkZDJiNjc2MTE5Ni5wbmc_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjQwMzA4JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI0MDMwOFQxNjU0NTJaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT0xODNjY2E1ZjNkZDNjMjBlNDY1ZTg0ZTBkMGZlODE0N2FmYzY0ZTc4NTU3ZGIwNjdkNjQ4NDZmNmU0NjBhMDE3JlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCZhY3Rvcl9pZD0wJmtleV9pZD0wJnJlcG9faWQ9MCJ9._uPl1TRRo0wbEQSgYkxz8K6U12TZ-_afJVmG-tQja0A)

## Development

### Setup

1. Ensure you have an IDE with C# language support installed. You may be required to install also .NET and NUnit plugins if this is the first ASP.NET project you're working on.
2. Clone the repository from Github.
3. Install Node and NPM. See the following [guides](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm) for each OS. 
4. Install required dev dependencies from the `package.json` by running the below command. (Ensure you `cd` into the project directory at the same level as the `package.json`). 

```
npm install
```
5. Verify the project builds successfully.
```
dotnet build
``` 
### Running the Application locally

The application can be launched locally using the IDE run configuration or from the terminal with the following command.

```
dotnet run --project KudosDash/KudosDash.csproj
```

Both of these options will provide a localhost URL to access the website at. Note that the `dotnet run` command will launch a HTTP version of the site, whilst the IDE may launch a HTTPS redirected version depending on the chosen run configuration. Some IDEs may also provide the opportunity to run the project from docker. The dockerfile in the project and the project publication settings ensure that the website will function properly within container.

### Making Changes

Changes should be tested locally via unit tests and Cypress. Non-breaking changes should be pushed to the  to the [development](https://github.com/SabreDae/KudosDash/tree/development) branch.

![development pipeline screenshot](https://private-user-images.githubusercontent.com/102368837/311313532-7a24deec-63fb-4694-b51f-891b7d0cfffe.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MDk5MTkxMTEsIm5iZiI6MTcwOTkxODgxMSwicGF0aCI6Ii8xMDIzNjg4MzcvMzExMzEzNTMyLTdhMjRkZWVjLTYzZmItNDY5NC1iNTFmLTg5MWI3ZDBjZmZmZS5wbmc_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjQwMzA4JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI0MDMwOFQxNzI2NTFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT1mOWJmZjA5Yzk1ZTg4MWE2OTkzZDFjZWVkMDNjOTFkOGYxN2M5ZWY3MmM3NGM3ZjlkNWFlZDI3ZGJhMWYxMjllJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCZhY3Rvcl9pZD0wJmtleV9pZD0wJnJlcG9faWQ9MCJ9.LXZa6qPu_CXzUUjjwH7z0EgOl8SnBtBb0qJrBLwsjFQ)

Note that the master branch which deploys to Render is protected against direct pushes. Changes which pass successfully through the development pipeline, will trigger a pull request to be manually reviewed before being merged into the master branch and deployed to production.

### Updating dependencies

It may be relevant sometimes to update dependencies the application is using.  The remote repository utilises [Snyk Code](https://docs.snyk.io/integrate-with-snyk/git-repositories-scms-integrations-with-snyk/snyk-github-integration) to scan dependencies for vulnerabilities on each push to the development branch and on a weekly basis. 

Any found vulnerablities will be flagged with recommended versions to change to wherever possible. 

### Running Unit Tests

Unit tests can be run in the IDE or via the commandline by running the below command.

```
dotnet test
```

This command will run all tests found in the `KudosDash.Tests/Unit/` directory. Running tests within the IDE offers a greater degree of control over which tests to run and provides debugging options also. 

### Running Cypress Tests

[Cypress](https://www.cypress.io/) tests should be executed before pushing any changes to the remote repository. 

Cypress will be installed during the NPM installation in step 4 of the setup described above.

Cypress's UI can be opened via running `npx cypress open`. 

![Cypress browser choice](https://learn.cypress.io/images/testing-your-first-application/installing-cypress-and-writing-your-first-test/choose_a_browser.jpg)

Using this UI will allow you to select which browser to use to run tests, which tests to run and give you the ability to see the test in action. 

![Cypress spec execution](https://learn.cypress.io/images/testing-your-first-application/installing-cypress-and-writing-your-first-test/Screen_Shot_2022-06-28_at_9.03.51_AM.png)

The existing Cypress tests verify the accessibility of pages, client-side validation, functionality of navigation through the site and security against certain types of cyber attacks. These tests are also executed by the pipeline associated to the master branch of the repository. 

The Cypress tests are configured to use the localhost via HTTP as this is how the site is most often interacted with during development. For the tests to be executable, the localhost server must also be running. 

#### test prod protection
