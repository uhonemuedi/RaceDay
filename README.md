RaceDay (Part 3: MVC Web Application)

RaceDay is a web based event management system for South African road running, walking and cycling events. This branch (part3-mvc) holds the finished system:

RaceDay.Api is the RESTful API from Part 2. It handles all data, sessions and role checks.
RaceDay.Mvc is the ASP.NET Core MVC web application built in Part 3. It has no database access. Every action goes through the API.
docs holds the Part 1 planning documents (ERD, endpoint plan and SQL script).
User roles

Organiser

Sees a dashboard with their events, enrolments per event and upcoming dates
Creates, edits and deletes events
Adds distance or age categories to each event
Views everyone enrolled in their events
Captures finish times and positions after an event

Participant

Browses upcoming events and filters them by name and type
Views full event details and available categories
Enrols in an event by choosing a category
Views their own enrolments with colour coded status
Views their personal race history on the My Results page
Views and updates their profile

Each role gets its own navigation menu. Pages are protected, and a user cannot open pages that belong to the other role.

Run it locally
Create the database by running docs/RaceDay-Schema.sql in SQL Server Management Studio.
Open RaceDay.Api/appsettings.json and change the RaceDayDB connection string so it points at your own SQL Server instance.
Open RaceDay.Mvc/RaceDay.Mvc.sln in Visual Studio.
Right click the solution, choose Configure Startup Projects, pick Multiple startup projects and set both projects to Start.
Press F5. The API runs on https://localhost:7108 (Swagger is at /swagger) and the MVC app runs on https://localhost:7194.
Register a new Organiser and a new Participant on the Register page. The sample users in the SQL script cannot log in because their passwords are placeholders, not real hashes.
Run it with Docker

The MVC app has a Dockerfile in RaceDay.Mvc. The API must be running first (step 5 above).

docker build -t raceday-mvc ./RaceDay.Mvc
docker run --rm -p 8080:8080 -e ApiBaseUrl=https://host.docker.internal:7108/ -e TrustApiCertificate=true raceday-mvc

Then open http://localhost:8080. TrustApiCertificate=true is for local development only. It lets the container accept the API's development certificate.

Technologies
ASP.NET Core 8 (MVC and Web API)
Entity Framework Core with SQL Server
Session based authentication with BCrypt password hashing
Bootstrap 5
Docker
GitHub Actions
CI/CD

A GitHub Actions workflow builds the API, the MVC project and the Docker image on every push to this branch.

Successful build screenshot:

[Insert the green build screenshot here]

Video

Unlisted YouTube video showing the running application, the MVC structure and the Docker setup:

Video link: [Insert the unlisted YouTube link here]

AI disclosure

AI tools (Claude) were used to help plan the structure, draft code and troubleshoot setup problems. I reviewed and tested the code, and I can explain how it works.
