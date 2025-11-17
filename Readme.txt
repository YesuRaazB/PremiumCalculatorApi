Insurance Premium Calculator
============================
Stack:
- Backend: .NET 7 Web API, EF Core (SQL Server)
- Frontend: React (Create React App)

How to run (Backend):
1. cd premium.Api
2. dotnet restore
3. If you prefer InMemory DB for quick run:
   - In Program.cs, useServices.AddDbContext<premiumDbContext>(opts => opts.UseInMemoryDatabase("PremiumCalculatorDb"));
   - Then 'dotnet run'
4. For SQLite:
   - Ensure dotnet-ef installed.
   - dotnet ef migrations add InitialCreate
   - dotnet ef database update
   - dotnet run
5. API URL: http://localhost:5000 (or printed output)
6. Swagger available in Development at http://localhost:5000/swagger

How to run (Frontend):
1. cd premium-client (or where you placed the React code)
2. set REACT_APP_API_BASE=http://localhost:5000
3. npm install
4. npm start

API Endpoints:
- GET /api/occupations
- GET /api/members
- GET /api/members/{id}
- POST /api/members
- PUT /api/members/{id}
- DELETE /api/members/{id}
- GET /api/members/calc?occupationCode={code}&death={death}&age={age}

Assumptions & clarifications:
- The doc's formula was ambiguous: I interpreted the formula as:
    yearly = (death * factor * age) / 1000
    monthly = yearly / 12
  If instead you expect monthly = (death * factor * age)/1000 * 12 please tell me and I'll change logic.
- Occupation rating factors are stored in DB (Occupation table) and seeded from the doc.
- All fields are mandatory   both UI and API validate mandatory fields.
- DateOfBirth stored as "MM/YYYY" as requested (no parsing/validation beyond basic presence).
- For production: add authentication, input sanitization, stronger DOB validation, and logging.

Unit tests:
- xUnit tests included for premium calculation.
- Integration tests can be added using WebApplicationFactory.

Submission & Git:
- Create a git repo and commit early/often as requested. Recommended branch names:
  - main (or master)
  - feat/backend, feat/frontend, test/unit

  Notes on EF migrations :
    dotnet ef migrations add InitialCreate
    dotnet ef database update

host the React app on same origin). To enable CORS add:
EX : builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
app.UseCors();


Example sample requests :
GET /api/members/calc?occupationCode=Doctor&death=100000&age=30
Response:
{ "monthlyPremium": 375.00 }
Create member:
POST /api/members
{
  "name": "John Doe",
  "ageNextBirthday": 30,
  "dateOfBirthMMYYYY": "10/1995",
  "occupationCode": "Doctor",
  "deathSumInsured": 100000
}
Response 201 Created:
{
  "id": 1,
  "name": "John Doe",
  "ageNextBirthday": 30,
  "dateOfBirthMMYYYY": "10/1995",
  "occupationCode": "Doctor",
  "deathSumInsured": 100000,
  "monthlyPremium": 375.00
}

