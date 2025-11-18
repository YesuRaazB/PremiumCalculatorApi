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

The exact sample outputs (from the table I generated): 

I computed premiums for 2 sample users. Here are the rows (exact values):

Alice — Doctor (factor 1.5), Age 30, Death Sum = 100,000

Yearly = (100000 * 1.5 * 30) / 1000 = 4500.00

Monthly = 4500 / 12 = 375.00

Bob — Cleaner (factor 11.5), Age 30, Death Sum = 100,000

Yearly = (100000 * 11.5 * 30) / 1000 = 34500.00

Monthly = 34500 / 12 = 2875.00

+----+-------+-------------------+--------------------+----------------+------------------+----------------+
| Id | Name  | AgeNextBirthday  | DateOfBirthMMYYYY | OccupationCode | DeathSumInsured  | MonthlyPremium |
+----+-------+-------------------+--------------------+----------------+------------------+----------------+
| 1  | Alice |        30        |      10/1995       |     Doctor     |     100000       |     375.00     |
+----+-------+-------------------+--------------------+----------------+------------------+----------------+
Explanation of exact numeric outputs — real example walkthrough (Alice): 
Inputs: DeathSumInsured = 100,000, Occupation factor = 1.5, AgeNextBirthday = 30

Step 1: compute death * factor * age → 100000 * 1.5 * 30 = 4,500,000

Step 2: divide by 1000 → 4,500,000 / 1000 = 4500 (this is the yearly premium)

Step 3: monthly = yearly / 12 → 4500 / 12 = 375
So output monthlyPremium = 375.00. The API returns that decimal rounded to 2 places.

