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


# Getting Started

Installation process

checkout the repository
open the project in Visual Studio
update the NuGet packages
update the azure storage connection string anf the container name in the appsettings
