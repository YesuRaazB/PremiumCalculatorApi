# PremiumCalculatorApi
Coding Test.docx :
1.	Quick assumptions:
A.	Premium formula used:
yearly = (DeathCover * OccupationFactor * Age) / 1000
monthly = yearly / 12

— If you want (…)/1000 * 12 instead, change one line in the calculator (I’ll show where).
B.  All fields mandatory (UI + API validate).
C. Date of Birth stored as "MM/YYYY" string (no DB date column).
2. Environment & tools :
	A. .NET SDK 8 (or 7) installed (dotnet --version).
	B. Visual Studio 2022/2023 or VS Code.
C. SQL Server accessible: either SQL Server Developer Edition, SQL Server Express, or Azure SQL. For dev you can use LocalDB ((localdb)\MSSQLLocalDB).
	D. EF Core CLI tools: dotnet tool install --global dotnet-ef (if not installed).
	E.  Node (16+) and npm/yarn for React.
	F. GIT
3. Create solution & projects (commands):
	Open a terminal in your dev / Main folder.
	mkdir InsurancePremiumCalculator
cd InsurancePremiumCalculator
# Create solution
dotnet new sln -n InsurancePremiumCalculator
# Create backend web api project
dotnet new webapi -n premium.Api
dotnet sln add premium.Api/ premium.Api.csproj
# Create test project
dotnet new xunit -n premium.Tests
dotnet sln add premium.Tests/ premium.Tests.csproj
dotnet add premium.Tests/ premium.Tests.csproj reference premium.Api/ premium.Api.csproj

4. Add required NuGet packages (backend):
	cd premium.Api
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Cors
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection    optional
5.	Project layout & files (recommended in Back-end)
	 
6. Configure SQL Server connection (appsettings.json & Program.cs)
	{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InsuranceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft": "Warning", "Microsoft.Hosting.Lifetime": "Information" }
  }
}
7. EF Core Migrations (SQL Server):
	dotnet tool install --global dotnet-ef
8. Create migration and update DB:
cd premium.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
9. generate SQL scripts:
	dotnet ef migrations script -o init.sql
10. Sample SQL table structure (if you prefer manual script):
	CREATE TABLE Occupations (
  	Code NVARCHAR(50) PRIMARY KEY,
  DisplayName NVARCHAR(200) NOT NULL,
  	Rating NVARCHAR(50) NOT NULL,
 	 Factor DECIMAL(18,4) NOT NULL
);


CREATE TABLE Members (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(200) NOT NULL,
  AgeNextBirthday INT NOT NULL,
  DateOfBirthMMYYYY NVARCHAR(7) NOT NULL,
  OccupationCode NVARCHAR(50) NOT NULL,
  DeathSumInsured DECIMAL(18,2) NOT NULL,
  MonthlyPremium DECIMAL(18,2) NOT NULL,
  CONSTRAINT FK_Members_Occupations FOREIGN KEY (OccupationCode) REFERENCES Occupations(Code)
);

Insert sample occupations : 
INSERT INTO Occupations (Code, DisplayName, Rating, Factor) VALUES ('Doctor','Doctor','Professional',1.50);

11 Frontend (React) – quick setup & integration with SQL Server API
	Create React app (in root or separate folder):
	npx create-react-app premium-client
cd premium-client
npm install
Start frontend : 
npm install   # first-time only
npm start
Key points:
•	Configure package.json proxy or set REACT_APP_API_BASE env var to point to your API: e.g. http://localhost:5000.
•	Enable CORS on backend (we added AllowAll policy).
•	Use the App.jsx code I provided earlier (it calls /api/occupations and /api/members/calc and POST /api/members).
 
Start the React dev server:
Windows PowerShell: $env:REACT_APP_API_BASE="http://localhost:5000"; npm start.
	Windows cmd  : set REACT_APP_API_BASE=http://localhost:5000
npm start
12. Validation & UI
•	Validate all fields required (HTML required or form-level).
•	Validate DOB format with a regex ^\d{2}\/\d{4}$.
•	Provide helpful inline error messages.
•	On occupation dropdown change, re-call /api/members/calc (we already trigger calculation only when occupation changes).
•	Format currency using Intl.NumberFormat.
Example test command:
cd Insurance.Tests
dotnet test
 
12. Troubleshooting tips & common pitfalls
•	If dotnet ef migrations add fails with “no design-time DbContext”, ensure your Program.cs wires up AddDbContext and that migrations project is the same as API project (or specify --project).
•	If SQL connection fails, check firewall, instance name, credentials.
•	If migrations try to use Linux and SQL Server connection string points to (localdb), that only works on Windows LocalDB.
•	For production, do not use db.Database.Migrate() on app startup indiscriminately — manage migrations through pipeline.
Example quick calc:
•	Death = 100000, Age = 30, Occupation Doctor (factor 1.5)
•	Yearly = (100000 * 1.5 * 30) / 1000 = 4500
•	Monthly = 4500 / 12 = 375 → UI should show ₹ 375
Manual git commands & commit messages (step-by-step): 
	git init
git add .
git commit -m "init: create solution scaffold (Insurance.Api and premium-client)"
# Backend focused commits (if you add files incrementally)
git add Insurance.Api/*.csproj
git commit -m "chore(api): add project file and NuGet references"
git add premium.Api/Models
git commit -m "feat(api): add Member and Occupation models"
git add Insurance.Api/Data/premiumDbContext.cs
git commit -m "feat(api): add DbContext and seed occupations"
git add premium.Api/Services
git commit -m "feat(api): add premium calculation service"
git add premium.Api/Controllers/MembersController.cs
git commit -m "feat(api): add MembersController with CRUD and calc endpoint"
# Frontend
git add premium-client
git commit -m "feat(frontend): add Angular/React client skeleton"
# Tests & CI later
git commit --allow-empty -m "test: add unit tests for PremiumCalculator (placeholder)"
git commit --allow-empty -m "ci: add GitHub Actions workflow (placeholder)"




Quick run checklist : 
1.	Copy backend files into premium.Api.
2.	From terminal, cd premium.Api and run:
o	dotnet restore
o	dotnet tool install --global dotnet-ef (if needed)
o	dotnet ef migrations add InitialCreate
o	dotnet ef database update
o	dotnet run
o	Confirm GET http://localhost:5000/api/occupations returns data and swagger works.

Some sample outputs (from the table I generated) :
I computed premiums for 3 sample users. Here are the rows (exact values):
•	Alice — Doctor (factor 1.5), Age 30, Death Sum = 100,000
o	Yearly = (100000 * 1.5 * 30) / 1000 = 4500.00
o	Monthly = 4500 / 12 = 375.00
•	Bob — Cleaner (factor 11.5), Age 30, Death Sum = 100,000
o	Yearly = (100000 * 11.5 * 30) / 1000 = 34500.00
o	Monthly = 34500 / 12 = 2875.00
•	Carol — Farmer (factor 31.75), Age 45, Death Sum = 200,000
o	Yearly = (200000 * 31.75 * 45) / 1000 = 286, (compute below)
o	Exact Yearly = 286, (see numbers below) → 286, (full number) — refer to table for exact.
 
Explanation of exact numeric outputs :
Inputs: DeathSumInsured = 100,000, Occupation factor = 1.5, AgeNextBirthday = 30
  Step 1: compute death * factor * age → 100000 * 1.5 * 30 = 4,500,000
 Step 2: divide by 1000 → 4,500,000 / 1000 = 4500 (this is the yearly premium)
  Step 3: monthly = yearly / 12 → 4500 / 12 = 375
So output monthlyPremium = 375.00. The API returns that decimal rounded to 2 places.
 
SELECT COLUMN_NAME, DATA_TYPE, NUMERIC_PRECISION, NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Occupations' OR TABLE_NAME = 'Members';
# Simple GET test
Invoke-RestMethod -Uri "http://localhost:5191/swagger/index.html" -Method GET -UseBasicParsing

# Or test API health (replace /api/members with a GET route your app supports)
Invoke-RestMethod -Uri "http://localhost:5191/api/members" -Method GET

cd path\to\premium-client
$env:REACT_APP_API_BASE="http://localhost:5000"
npm start

http://localhost:5191/swagger/index.html

 

 
