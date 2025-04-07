# Genapi API
Genapi is an API in development which could be utilized to generate a data-driven API with user inputs.

## Current working Workflow
### Code Generation
Prerequisites:
- Github repository needs to be already created and empty
- Github Credentials need to be provided (Username + PAT)

.NET Web API is generated through Genapi API and the generated code is then pushed to Github

### Publishing of the generated API (Azure Web Apps)
1. Create a new Azure Web Apps project
2. Connect via GitHub to the generated repository (This will add a Github workflow file to the repository)

## Ideal workflow
### Initial code generation
1. The user creates an Azure SQL Database without tables
2. The user provides Github credentials (Username + PAT) & Azure SQL Database connection string in the Genapi UI
3. The user generates the API from Genapi UI
4. Genapi API creates Azure SQL Database tables 
5. Genapi API generates the code to Github
6. The user creates Azure Web Apps project, configures Azure SQL Database connection string & finally connects to the Github repository (This will create a Github workflow)
7. DONE - The API should be up an running!

### Modification of generated code
1. The user modifies the requirements for a specific endpoint
2. Genapi API then overwrites that specific endpoint to match the new requirements
3. Genapi API pushes the changes to Github & the already existing Github workflows should publish them to Azure

## Next items to work on
### Genapi API
- Add generation of Model classes
- Add generation of ModelDTO classes (?)
- Modify the code to generate Controllers instead of just app.MapGet() with use of Models
- Add required code to work with Azure SQL Database (DbContext and code for CRUD operations)
- Add code to create Azure SQL Database tables
- Add meaningful logs
- Add a better default .gitignore
- Refactor code before proceeding to add more functionality

### Genapi UI
1. First create a simple UI were the user can generate a simple API to Github
2. Add more customization & options when the Genapi API is further developed
### Documentation
Later on...


