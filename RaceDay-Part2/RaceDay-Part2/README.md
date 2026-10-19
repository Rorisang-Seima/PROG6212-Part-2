# RaceDay API — PROG6212 Part 2 starter implementation

RaceDay is an ASP.NET Core Web API for race/event organisers and participants. This solution is a working starter implementation intended to be reviewed, tested, aligned with the lecturer's rubric, and completed before submission. It has **not** been run in your local Visual Studio environment, and you must build/test it there.

## Requirements
- Visual Studio 2022 with the ASP.NET and web development workload
- .NET 8 SDK
- SQL Server Database Engine
- SQL Server Management Studio (optional GUI for database inspection)

## Open and run
1. Extract the ZIP.
2. Open `RaceDay.sln` in Visual Studio 2022.
3. Wait for NuGet restore to finish.
4. Check `RaceDay.Api/appsettings.json`. The default connection uses LocalDB:
   `Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
5. If your installed SQL Server is a named instance rather than LocalDB, change the `RaceDay` connection string to match your instance.
6. Open **Tools > NuGet Package Manager > Package Manager Console**, set Default project to `RaceDay.Api`, then run:
   `Add-Migration InitialCreate`
   `Update-Database`
   If EF migration tools are not installed, install `Microsoft.EntityFrameworkCore.Tools` version `8.0.11` in the API project first.
7. Set `RaceDay.Api` as startup project and press **F5**.
8. Open `/swagger` on the launched HTTPS URL.

## Roles
- **Organiser**: create/update/delete their own events, add categories to their events, view enrolments for their own events, record results for enrolments belonging to their events.
- **Participant**: view events/categories, update their own profile, enrol in an event category, view their enrolments and results.
- Both roles can register/login and view/update their own profile.

## Authentication notes
Login creates a server-side ASP.NET Core session and sets an HTTP-only session cookie. Keep cookies between requests. Swagger's browser UI may retain the cookie automatically, but if it does not, test with a cookie-aware REST client. Sessions expire after 30 minutes idle. Passwords are hashed using ASP.NET Core's `PasswordHasher<TUser>`.
For a real deployment, use HTTPS, persistent distributed session storage, rate limiting, robust validation, and security review.

## API endpoints
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- `GET /api/users/me`, `PUT /api/users/me`
- `GET /api/events`, `GET /api/events/{id}`
- Organiser: `POST /api/events`, `PUT /api/events/{id}`, `DELETE /api/events/{id}`
- Organiser: `POST /api/events/{id}/categories`
- `GET /api/categories`; Organiser: `POST /api/categories`
- Participant: `POST /api/enrolments`, `GET /api/enrolments/mine`
- Organiser: `GET /api/enrolments/event/{eventId}`
- Organiser: `POST /api/results/enrolment/{enrolmentId}`
- Participant: `GET /api/results/mine`

## Data model
Users, Events, Categories, EventCategories, Enrolments and Results are implemented with EF Core Code First. Unique constraints cover user email, category name, event/category pairs, participant/category enrolments, and one result per enrolment. Event has DistanceKm and EventType to satisfy the brief's event requirements; confirm these additions against your approved Part 1 ERD and report.

## Tests and CI
Run `dotnet test RaceDay.sln`. GitHub Actions builds and runs tests on each push and pull request. The tests included are a starting set; add integration tests for successful/failed registration/login, event ownership, role-based HTTP rejection, and enrolment persistence before submission. CI will not run until this repository is pushed to GitHub.

## Important before submission
- Compare every entity/relationship with your approved Part 1 ERD and SQL script.
- Add comprehensive integration tests, meaningful commit history (do not fake or batch commits), and a screenshot of a successful GitHub Actions run.
- Record the required unlisted YouTube demonstration and add its real link here.
- Confirm the submission rubric and lecturer's required evidence. This ZIP does not include a generated migration yet; generate it using the steps above.
