# Introduction 
The Anomaly-Service offers a REST API to perform the CRUD operation on the Anomaly-Storage.
It also manages the upload of files to the File-Storage.
The basics operations are CRUD for Anomalies and CRUD for Reports.

Functionality	Interface		
Create Anomaly	       POST /anomalies	
Update Anomaly	       PUT /anomalies/{anomalyId}	
Delete Anomaly	       DELETE /anomalies/{anomalyId}	
Get Anomaly	           GET /anomalies/{anomalyId}	
Get Anomaly List	   GET /anomalies/	
Create Report	       POST /reports	
Update Report	       PUT /reports/{reportId}	
Delete Report	       DELETE /reports/{reportId}	
Get Report	           GET /reports/{id}	
Get Report List	       GET /anomalies/{anomalyId} /reports/	



#Env variable
- MYSQL_CONNECTION_STRING
- AZURE_STORAGE_CONNECTION_STRING
- AZURE_CONTAINER_NAME
- RABBITMQ_HOSTNAME
- RABBITMQ_USERNAME
- RABBITMQ_PASSWORD
- RABBITMQ_EXCHANGE_NAME

# Getting Started


## Setting up the database
Update the database, create table with entity framework

For now this process is done manually by :
- changing the appsettings.json connection string to the right database's url to update
- execute the command
```bash
dotnet ef database update
```

## Service intallation

Installation process

checkout the repository
open the project in Visual Studio
update the NuGet packages
update the azure storage connection string anf the container name in the appsettings