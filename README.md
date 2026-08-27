RaceDay

RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, and track their personal performance history.

This repository contains the Portfolio of Evidence (POE) for PROG6212, submitted in three progressive parts:

Part 1: System planning — ERD, API endpoint plan, and SQL database script.
Part 2: RESTful API built in C#, connected to the database, with unit tests and CI/CD.
Part 3: MVC web application consuming the API, with Azure Blob Storage and Docker containerisation.
User Roles

The system supports two distinct user roles:

Organiser — can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
Participant — can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.
Part 1 — Planning Deliverables

All Part 1 planning documents are located in the /docs folder:

ERD.png — Entity Relationship Diagram for the RaceDay database.
API-Endpoint-Plan.docx — Full API endpoint plan covering Authentication, User Profile, Events, Categories, Enrolments, and Results.
RaceDay-Schema.sql — SQL Server script to create and seed the RaceDay database.
Setup Instructions
Clone this repository:
   git clone https://github.com/uhonemuedi/RaceDay.git
Open SQL Server Management Studio (SSMS).
Open docs/RaceDay-Schema.sql.
Execute the script against your local SQL Server instance — this creates the RaceDayDB database, all tables, and seeds sample data.
CI/CD

A GitHub Actions workflow (.github/workflows/validate-docs.yml) automatically validates that the /docs folder contains all required Part 1 files on every push to main.

Successful build screenshot:

   ![CI/CD Success](docs/CI-CD-Success.png)


Video Presentation

An unlisted YouTube video walking through the planning documents, ERD decisions, endpoint plan choices, and a live run of the SQL script in SSMS:

Video link: [Insert unlisted YouTube link here]

AI Disclosure

AI tools (Claude) were used to assist with planning structure, drafting documentation, and troubleshooting GitHub/SSMS setup issues during this project. All design decisions and final work were reviewed and understood by the author.

