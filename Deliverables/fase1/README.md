# Fase 1

## Application Description

This document is used for planning and documenting the ShopTex application.
ShopTex is an E-Commerce platform, consisting of multiple stores. Each store controls it own products, but never accesses any user data or info.

## Assets
This section presents all assets of the application, both physical and virtual (data).
The Confidentiality, Integrity and Availability (CIA) parameters are defined either Low, Medium or High, applied as defined in [CIA Definition](#cia-definitions)

### Assets Table
| ID      | Name          | Description                                                                    | Confidentiality               | Integrity  | Availability | Total/Priority |
|---------|---------------|--------------------------------------------------------------------------------|-------------------------------|------------|--------------|----------------|
| SRV-001 | vs531         | Server hosting the main application                                            | 1 - Low (No data stored here) | 1 - Low    | 3 - High     | 5/9            |
| SRV-002 | vs204         | Server hosting the main database                                               | 3 - High                      | 3 - High   | 3 - High     | 9/9            |
| SRV-003 | vs285         | Server hosting the logging database                                            | 2 - Medium                    | 3 - High   | 1 - Low      | 6/9            |
| USRD-00 | User data     | The agglomeration of all user data                                             |                               |            |              |                |
| USRD-01 | User Personal | Personal information of the user (Name, Date of Birth, Address, E-mail, ID)    | 3 - High                      | 2 - Medium | 1 - Low      |                |
| USRD-02 | User Payment  | Payment information of the user (Payment Method, Billing Address, NIF)         | 3 - High                      | 2 - Medium | 1 - Low      |                |
| USRD-03 | User Other    | Other information about the user (Purchase History, Review info, Display name) | 2 - Medium                    | 1 - Low    | 1 - Low      |                |
|         |               |                                                                                |                               |            |              |                |
|         |               |                                                                                |                               |            |              |                |

### CIA Definitions

| Level      | Confidentiality                                                                      | Integrity                                                                                 | Availability                                                                                  |
|------------|--------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| 1 - Low    | There is no personal, identifying or secret data                                     | The integrity of the asset does not impact the application                                | The asset can be unavailable without affecting the usability or continuity of the application |
| 2 - Medium | Contains some sensitive information, but not secret                                  | The asset can have it's integrity compromised, if it only impacts some function/feature   | The unavailability of the asset only compromises some features/functions.                     |
| 3 - High   | Personal data that can identify and/or track a particular user or secret information | The asset cannot have it's integrity compromised, without affecting the whole application | The unavailability of the asset makes the application unusable and impacts continuity         |

When used together to evaluate an asset, the level numbers can be summed to determine priority of the asset in case of recovery or determining protection effort. The qualitative evaluation of the sum, goes as follows:

 - 1-3 : Low
 - 4-6 : Medium
 - 7-9 : High

## Requirements
The functional and non-functional requirements are defined in the document linked bellow.

[Requirements Document](Requirements.md)

## Logic Diagram
![Logic Diagram](Logic.jpg)

## Domain Model

![Domain Model](domainModel.png)

## Requirements

This section lists the main functional and non-functional requirements for the system/application.

[Link to Requirements](Requirements.md)

## Use Case
This section lists the Use Cases and the Use Case Diagram for the system/application.

[Link to Use Cases](useCases.md)

[Link to Use Case Diagram](usecaseDiagram.puml)

## Security
[Link to Security](Security.md)

## Deployment Diagram

The server for the ShopTex will use Fedora 38 and for the requests it will use HTTPS.
The databases will HTTPS for the connections with the backend and it won't have open ports open to outside internet. This databases will use MySQL version 8.0.

![Deployment Diagram](Deployment%20Diagram.jpg)

## Other

In this folder you can find also the Thread Model in the TM.json file for the Threat Dragon tool and the report in the threat_model_report.pdf file.
You can also find the ASVS (Application Security Verification Standard) checklist in the excel file.
