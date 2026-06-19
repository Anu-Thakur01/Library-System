# Library Management System - Flow, Basics, and Interview Guide

## 1. Project Overview

This project is a Library Management System built using ASP.NET Core MVC, C#, Entity Framework Core, and PostgreSQL.

The system helps manage:

- Books
- Authors
- Categories
- Publications
- Members
- Borrow records
- Return records
- Reports
- Users and roles

The project follows a layered architecture, which makes the code easier to understand, maintain, and explain in interviews.

## 2. Technology Stack

Frontend:

- HTML
- CSS
- Bootstrap
- Razor Views
- JavaScript
- DataTables

Backend:

- C#
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core

Database:

- PostgreSQL

Tools:

- Visual Studio
- Visual Studio Code
- pgAdmin
- Git/GitHub

## 3. Architecture Flow

The system follows this flow:

```text
User Browser
    ↓
Razor View
    ↓
MVC Controller
    ↓
Business Layer
    ↓
Repository Layer
    ↓
Entity Framework Core
    ↓
PostgreSQL Database
```

Example:

When a user adds a member:

1. User fills the Add Member form.
2. The form sends data to `MemberController`.
3. `MemberController` validates the model.
4. Controller calls `MemberBusiness`.
5. `MemberBusiness` prepares the member entity.
6. `MemberRepository` saves the data.
7. EF Core inserts the data into PostgreSQL.
8. User is redirected to the member list.

## 4. Project Layers

### LibrarySystem

This is the main MVC project. It contains:

- Controllers
- Razor views
- Layout
- Authentication pages
- Static files
- App configuration

### LibrarySystem.Shared

This contains shared DTOs and validation models.

Examples:

- `MemberDetails`
- `BookDetails`
- `ReportDetails`

### LibrarySystem.Business

This contains business logic.

Example:

- Calculating member expiration date
- Mapping form data to database models
- Applying rules before saving data

### LibrarySystem.Repository

This contains database access logic.

Examples:

- `MemberRepository`
- `BookRepository`
- `BorrowRepository`
- `ApplicationDbContext`

### LibrarySystem.API

This is the API project. It can expose system features through API endpoints and Swagger.

## 5. Database Flow

The project uses PostgreSQL with this connection:

```text
Host=localhost;Port=5433;Database=library_system_db;Username=postgres;Password=admin
```

Main database tables:

- `Books`
- `Author`
- `Category`
- `Publication`
- `Member`
- `Borrows`
- `AspNetUsers`
- `AspNetRoles`

## 6. Important Features

### Authentication and Authorization

The project uses ASP.NET Core Identity.

Default users:

```text
Username: admin
Password: adminpassword
Role: SuperAdmin

Username: staff
Password: staffpassword
Role: Staff
```

SuperAdmin can manage setup data like books, authors, members, and users.

Staff can access borrowing and reports based on assigned role.

### Book Management

Book Management allows adding, editing, changing status, and deleting books.

Book fields include:

- Book name
- Author
- Publication
- Category
- ISBN
- Total copies
- Available copies
- Edition
- Cover image

ISBN is unique, so the same ISBN cannot be used again.

### Author Management

Author Management allows adding, editing, changing status, and deleting authors.

Author fields include:

- First name
- Middle name
- Last name
- Bio
- Date of birth

### Member Management

Member Management allows adding, editing, changing status, and deleting members.

Member fields include:

- Member name
- Address
- Phone number
- Email
- Joined date
- Membership type

Privacy rules:

- Member ID is stored in the database but not shown in the UI.
- Address, email, and phone number are stored in the database but not shown in list views.
- Phone number must be exactly 10 digits.
- Email must be a Gmail address.

### Borrow Management

Borrow Management allows a member to borrow a book.

The system stores:

- Book ID
- Member ID
- Borrowed date
- Due date
- Return status
- Fine amount

IDs are stored internally for database relationships but are not shown to normal users.

### Reports

Reports include:

- Borrowed Books by Date
- Member Borrow History

Member borrow history search is case-insensitive.

Example:

If the database has `Aayan Thakur`, searching `aayan thakur` will still find the member.

## 7. Why IDs Are Hidden in the UI

IDs are database keys. They are important for internal operations but not useful for normal users.

We hide IDs because:

- It keeps the UI clean.
- It avoids exposing internal database structure.
- It improves privacy and safety.

The IDs are still stored and used in the database.

## 8. Light Mode and Dark Mode

The system includes a theme toggle button.

Users can switch between:

- Light mode
- Dark mode

The selected mode is stored in browser local storage, so the selected theme remains after refresh.

## 9. How To Run the Project

Stop any already running project first:

```powershell
Get-Process LibrarySystem | Stop-Process
```

Run MVC project:

```powershell
dotnet run --project LibrarySystem\LibrarySystem\LibrarySystem.csproj
```

Open:

```text
http://localhost:5136
```

Run API project:

```powershell
dotnet run --project LibrarySystem\LibrarySystem.API\LibrarySystem.API.csproj
```

Open:

```text
http://localhost:5088/swagger
```

## 10. Common Problems and Answers

### Port already in use

Meaning:

Another copy of the project is already running.

Solution:

```powershell
Get-Process LibrarySystem | Stop-Process
```

Then run again.

### PostgreSQL password error

Meaning:

The username, password, or port is wrong.

Solution:

Check PostgreSQL connection:

```text
Host=localhost;Port=5433;Username=postgres;Password=admin
```

### Failed to add book

Possible reasons:

- Duplicate ISBN
- Missing author/category/publication
- Database connection problem
- Invalid date/time value

### Failed to add member

Possible reasons:

- Phone number is not exactly 10 digits
- Email is not Gmail
- Required fields are missing
- Database connection problem

## 11. How To Explain the Project in an Interview

Sample answer:

> I built a Library Management System using ASP.NET Core MVC, C#, Entity Framework Core, and PostgreSQL. The system manages books, authors, categories, publications, members, borrowing, returns, and reports. I used layered architecture with MVC controllers, business services, repositories, shared DTOs, and EF Core database models. I also implemented role-based authentication using ASP.NET Core Identity, PostgreSQL database integration, data validation, search, and a light/dark mode UI.

## 12. Basic Interview Questions and Answers

### What is ASP.NET Core MVC?

ASP.NET Core MVC is a web framework that separates an application into Model, View, and Controller. Model represents data, View displays UI, and Controller handles user requests.

### What is Entity Framework Core?

Entity Framework Core is an ORM. It allows us to work with the database using C# classes instead of writing raw SQL for every operation.

### What is PostgreSQL?

PostgreSQL is a relational database management system. It stores data in tables and supports relationships, constraints, indexes, and SQL queries.

### What is a controller?

A controller handles browser requests, validates input, calls business logic, and returns views or redirects.

### What is a repository?

A repository handles database operations. It separates data access logic from controllers and business logic.

### What is a DTO?

A DTO, or Data Transfer Object, is used to transfer data between layers without directly exposing database entities.

### What is dependency injection?

Dependency injection is a design pattern where required objects are provided automatically. In this project, controllers receive business services through constructors.

### What is authentication?

Authentication checks who the user is, usually through login.

### What is authorization?

Authorization checks what the logged-in user is allowed to do.

### Why did you use PostgreSQL?

PostgreSQL is reliable, open-source, and suitable for relational data. It supports strong data consistency and works well with Entity Framework Core.

### Why did you use layered architecture?

Layered architecture makes the project clean and maintainable. Each layer has a clear responsibility.

### Why are IDs hidden from the UI?

IDs are internal database keys. Users do not need to see them, so hiding them makes the UI cleaner and safer.

### How did you validate member phone number?

I used frontend input restrictions and server-side validation to make sure the phone number contains exactly 10 digits.

### How did you validate email?

I used required validation, email validation, and a regular expression to allow only Gmail addresses.

### How does dark mode work?

The UI has a theme toggle button. JavaScript changes the theme and saves the selected mode in browser local storage.

## 13. How To Sit and Answer in an Interview

Before entering:

- Dress neatly.
- Carry your resume.
- Carry project screenshots or GitHub link if available.
- Revise your project flow.
- Practice your introduction.

While sitting:

- Sit straight.
- Keep your hands relaxed.
- Do not lean too much.
- Keep eye contact naturally.
- Listen carefully before answering.

While answering:

- Start with a direct answer.
- Then explain with your project example.
- If you do not know something, say honestly that you are learning.
- Do not guess too much.
- Speak slowly and clearly.

Good sentence when you do not know:

> I have not used that deeply yet, but I understand the basic idea and I am willing to learn it.

## 14. Self Introduction for Internship

Sample answer:

> Good morning. My name is Aayan Thakur. I am currently pursuing a Bachelor in Information Technology at Texas College of Management and IT. I have learned C#, Python, C, SQL, PostgreSQL, HTML, CSS, Bootstrap, Tailwind CSS, and .NET. I have built a Library Management System using .NET and PostgreSQL, and I have also created a Twitter clone using Tailwind CSS and worked with WordPress. I recently completed a 50-day IT training organized by Kathmandu Metropolitan City. I am looking for an internship where I can improve my practical skills, learn from experienced developers, and contribute with dedication.

## 15. Questions You Can Ask the Interviewer

- What technologies does your development team mainly use?
- What kind of tasks are given to interns?
- Will interns get mentorship from senior developers?
- What should I prepare before joining?
- Is there a possibility of full-time opportunity after internship?

## 16. Final Interview Tips

- Be honest about being a beginner.
- Focus on your projects.
- Explain what you built and what you learned.
- Show interest in learning.
- Do not say "I know everything."
- Say "I am confident in basics and ready to learn industry practices."
