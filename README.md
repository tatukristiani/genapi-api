# Genapi API
Genapi is an API in development which could be utilized to generate a data-driven API with user inputs.


## Genapi Workflow
Good To Know:
- An empty Github repository is required before generating the API there
- Github Credentials (Username + PAT) are required to generate the API
- Azure SQL Database can't include tables when first time running the generated API in Azure Web Apps

### Initial code generation
1. The user creates an Azure SQL Database without tables
2. The user creates an empty Azure Web Apps project to Azure
3. The user provides Github credentials (Username + PAT)
4. The user provides configurations for the API from Genapi UI
5. Genapi API generates the code to Github
6. The user connects the Azure Web Apps to deploy from the Github repository automatically (This will generate a Github workflow)
7. The generated API creates the database tables according to configurations 
8. DONE - The API should be up an running!

### Modification of generated code (in progress..)
1. The user modifies the requirements for a specific endpoint
2. Genapi API then overwrites that specific endpoint to match the new requirements
3. Genapi API pushes the changes to Github & the already existing Github workflows should publish them to Azure

Note! Modification of Models or Database columns might bring some difficulties.

## Next items to work on
### Genapi API
- Add validations to input configurations when UI is ready
- Support for other databases ready (only need to implement them)

### Genapi UI
1. Create Genapi UI simple version
2. Add more customization options


### Documentation
Later on...


