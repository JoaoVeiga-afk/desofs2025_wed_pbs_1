# Fase 1

## Application Description

This document is used for planning and documenting the ShopTex application.
ShopTex is an E-Commerce platform, consisting of multiple stores. Each store controls it own products, but never accesses any user data or info.

## Assets
This section presents all assets of the application, both physical and virtual (data).
The Confidentiality, Integrity and Availability (CIA) parameters are defined either Low, Medium or High, applied as defined in [CIA Definition](#cia-definitions)

### Assets Table
| ID      | Name  | Description                         | Confidentiality | Integrity | Availability | Total/Priority |
|---------|-------|-------------------------------------|-----------------|-----------|--------------|----------------|
| SRV-001 | vs531 | Server hosting the main application |                 |           |              |                |
| SRV-002 | vs204 | Server hosting the main database    |                 |           |              |                |
| SRV-003 | vs285 | Server hosting the logging database |                 |           |              |                |
|         |       |                                     |                 |           |              |                |
|         |       |                                     |                 |           |              |                |
|         |       |                                     |                 |           |              |                |
|         |       |                                     |                 |           |              |                |
|         |       |                                     |                 |           |              |                |
|         |       |                                     |                 |           |              |                |

### CIA Definitions

| Level  | Confidentiality                                                          | Integrity                                                                                 | Availability                                                                                  |
|--------|--------------------------------------------------------------------------|-------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| Low    | There is no personal or identifying data                                 | The integrity of the asset does not impact the application                                | The asset can be unavailable without affecting the usability or continuity of the application |
| Medium | Contains personal data that cannot be used to identify a particular user | The asset can have it's integrity compromised, if it only impacts some function/feature   | The unavailability of the asset only compromises some features/functions.                     |
| High   | Personal data that can identify and/or track a particular user           | The asset cannot have it's integrity compromised, without affecting the whole application | The unavailability of the asset makes the application unusable and impacts continuity         |

## Requirements
The functional and non-functional requirements are defined in the document linked bellow.

[Requirements Document](Requirements.md)

## Logic Diagram
![Logic Diagram](Logic.jpg)

## Domain Model

![Domain Model](domainModel.png)

## Requirements

This section lists the main functional and non-functional requirements for the system/application.

![Link to Requirements](Requirements.md)

## Use Case
This section lists the Use Cases and the Use Case Diagram for the system/application.

![Link to Use Cases](useCases.md)

![Link to Use Case Diagram](usecaseDiagram.puml)

## Security
![Link to Security](Security.md)

## Deployment Diagram

The server for the ShopTex will use Fedora 38 and for the requests it will use HTTPS.
The databases will HTTPS for the connections with the backend and it won't have open ports open to outside internet. This databases will use MySQL version 8.0.

![Deployment Diagram](Deployment%20Diagram.jpg)

## Other

In this folder you can find also the Thread Model in the TM.json file for the Threat Dragon tool and the report in the threat_model_report.pdf file.
You can also find the ASVS (Application Security Verification Standard) checklist in the excel file.
