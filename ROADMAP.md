# Trainr — 30-Day Roadmap to Mid-Level .NET Engineer

This roadmap uses the Trainr project as the teaching tool. Every concept gets applied to real code, not throwaway exercises. Budget 2-4 hours per day.

---

## WEEK 1: C# Refresh + Foundation (Days 1-7)

### Day 1 — C# Fundamentals: Types, Classes, OOP
- Review value types vs reference types, `class` vs `struct`
- Review OOP: inheritance, polymorphism, encapsulation, abstraction
- **Task:** Look at your `ApplicationUser` → `Athlete` / `Trainer` inheritance chain in Trainr. Write a short explanation of why this design was chosen and what the trade-offs are. Submit for review.
- **Exercise:** Write a `SportCategory` enum and a `UserProfile` abstract class with a `GetDisplayInfo()` abstract method. Have `AthleteProfile` and `TrainerProfile` implement it differently.

### Day 2 — Collections, Generics, and LINQ
- Review `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`
- When to use which collection and why
- LINQ: `Where`, `Select`, `GroupBy`, `OrderBy`, `FirstOrDefault`, `Any`, `All`, `Aggregate`
- **Task:** Open `TrainingSessionRepo.cs` — read the `GetBookingData()` method. Rewrite it using clean LINQ instead of the manual join. Submit both versions for review.
- **Exercise:** Given a `List<TrainingSession>`, write LINQ queries to: (1) get all sessions for a specific trainer, (2) group sessions by sport type with counts, (3) find the trainer with the most bookings.

### Day 3 — Async/Await
- What `async`/`await` actually does (task-based asynchronous pattern)
- Why we don't use `.Result` or `.Wait()` (deadlocks)
- `Task` vs `Task<T>` vs `ValueTask`
- When to use `ConfigureAwait(false)`
- **Task:** Audit every async method in Trainr. Find any that are incorrectly implemented (fire-and-forget, missing await, blocking calls). Document what you find.
- **Exercise:** Write an async method that calls two independent async operations in parallel using `Task.WhenAll`, then one that calls them sequentially. Explain when you'd use each.

### Day 4 — Interfaces, Dependency Injection, SOLID
- Review interfaces vs abstract classes (when to use which)
- Single Responsibility, Open/Closed, Dependency Inversion — focus on these three
- Why we inject `ITrainerRepo` instead of `TrainerRepo` directly
- Service lifetimes: Transient vs Scoped vs Singleton
- **Task:** Look at Trainr's `Startup.cs`. All repositories are registered as `Transient`. Should they be? Write your reasoning for what lifetime each service should use. Submit for review.

### Day 5 — Data Structures & Algorithms, Part 1
- **Dictionary/HashSet** — how hash tables work, O(1) lookup, when to use each
- **Stack & Queue** — LIFO vs FIFO, real-world use cases
- **Big O basics** — O(1), O(n), O(log n), O(n log n), O(n²) — know what they mean and how to spot them
- **Exercise:** (1) Given a list of training sessions, find duplicate bookings using a HashSet. (2) Implement a simple "undo" feature for schedule changes using a Stack. (3) Implement a waitlist for fully-booked sessions using a Queue.

### Day 6 — Data Structures & Algorithms, Part 2
- **Binary search** — how it works, when to apply it
- **Two pointer technique** — common interview pattern
- **Hash map patterns** — frequency counting, grouping, lookup optimization
- **Exercise:** (1) Given a sorted list of available time slots, binary search for the nearest slot to a requested time. (2) Given a list of trainer availability windows, merge overlapping windows (interval merge problem). (3) Find all trainers who are available at the same time as a given athlete's preferred times.

### Day 7 — Upgrade Trainr to .NET 8
- Upgrade `TargetFramework` from `netcoreapp3.0` to `net8.0`
- Update all NuGet packages to latest compatible versions
- Replace old `Startup.cs` + `Program.cs` pattern with minimal hosting model
- Fix any breaking changes
- Switch database from SQL Server to **SQLite** (works on Mac, no install needed)
- Delete old migrations, create a fresh initial migration
- Verify the app builds and runs
- **This is a guided exercise — we'll do it together step by step.**

---

## WEEK 2: Modern ASP.NET Core (Days 8-14)

### Day 8 — New Program.cs, Middleware Pipeline
- Understand the minimal hosting model (no more `Startup.cs`)
- How the middleware pipeline works (order matters)
- Write custom middleware (request logging, timing)
- **Task:** Add request timing middleware to Trainr that logs how long each request takes.

### Day 9 — REST API Design
- RESTful conventions: verbs, status codes, resource naming
- API controllers vs MVC controllers
- `[ApiController]`, model binding, `ActionResult<T>`
- **Task:** Create a new `Api/TrainersApiController.cs` that exposes trainer data as a JSON API alongside the existing MVC views. Endpoints: `GET /api/trainers`, `GET /api/trainers/{id}`, `POST /api/trainers`, `PUT /api/trainers/{id}`, `DELETE /api/trainers/{id}`.

### Day 10 — Entity Framework Core Modern Patterns
- Code-first with proper configurations (Fluent API vs annotations)
- Navigation properties, lazy vs eager loading
- `AsNoTracking()` for read queries
- Proper migration workflow
- **Task:** Refactor Trainr's models to use Fluent API configuration instead of `[ForeignKey]` attributes. Create a proper `TrainerConfiguration : IEntityTypeConfiguration<Trainer>` class.

### Day 11 — Repository Pattern Revisited + Service Layer
- Should you even use the repository pattern with EF Core? (controversial — both sides explained)
- Introduce a Service layer between controllers and repositories
- **Task:** Create a `TrainerService` class that contains the business logic currently sitting in your controllers. Controllers should only call the service.

### Day 12 — Validation, Error Handling, Logging
- FluentValidation vs Data Annotations
- Global exception handling middleware
- Structured logging with `ILogger` and Serilog
- Problem Details (RFC 7807) for API errors
- **Task:** Add global error handling to Trainr. Add Serilog with console + file sinks. Add input validation on the trainer creation endpoint.

### Day 13 — Authentication: JWT for APIs
- How JWT works (header, payload, signature)
- Difference between authentication and authorization
- Implement JWT auth for the API endpoints
- Role-based authorization (`[Authorize(Roles = "Trainer")]`)
- **Task:** Add JWT authentication to Trainr's API. Keep the cookie-based Identity auth for the MVC views, add JWT for the API layer.

### Day 14 — Review Day
- Clean up all code from the week
- Full code review of everything built
- Fix any issues flagged
- Commit everything with clean commit messages

---

## WEEK 3: Build the "Uber for Training" Features (Days 15-21)

### Day 15 — Clean Architecture Refactor
- Separate the solution into projects: `Trainr.Core` (domain), `Trainr.Infrastructure` (data), `Trainr.Web` (API/UI)
- Why layered architecture matters for testability and maintainability
- **Task:** Restructure the Trainr solution into 3 projects with proper dependency direction.

### Day 16 — Trainer Search & Matching
- Build a real search system: filter by sport, location, availability, price range
- Pagination with `Skip`/`Take`
- Sorting options
- **Task:** Build `GET /api/trainers/search?sport=basketball&available=true&page=1&pageSize=10` with proper pagination response.

### Day 17 — Real-Time Availability System
- Redesign the scheduling model for the "Uber" concept
- Recurring availability (weekly schedules, not single slots)
- Conflict detection when booking
- **Task:** Replace the current `TrainerSchedule` with a proper availability system. A trainer sets weekly recurring availability. Athletes book from those windows. System prevents double-booking.

### Day 18 — Booking Flow
- Full booking lifecycle: Request → Confirm → Complete → Review
- State machine pattern for booking status
- Notification system (email or in-app)
- **Task:** Build the complete booking API: `POST /api/bookings`, `PUT /api/bookings/{id}/confirm`, `PUT /api/bookings/{id}/complete`, `PUT /api/bookings/{id}/cancel`.

### Day 19 — Data Structures & Algorithms, Part 3
- **Recursion** — base case, recursive case, when to use it
- **Basic tree concepts** — why you should understand them even if you don't build trees daily
- **Sorting** — know the differences between common sorts, when to use which
- **Exercise:** (1) Recursively flatten a nested category structure (sport → sub-categories → specializations). (2) Given a booking system with dependencies (session B can't start until session A ends), determine a valid booking order (topological sort concept). (3) Practice 3 LeetCode Easy problems tagged "Hash Table" or "Array".

### Day 20 — Unit Testing
- xUnit fundamentals: `[Fact]`, `[Theory]`, `[InlineData]`
- Mocking with Moq — mock the repository, test the service
- What to test vs what not to test
- Arrange-Act-Assert pattern
- **Task:** Write tests for `TrainerService`: test search filtering, test booking conflict detection, test booking state transitions. Minimum 10 tests.

### Day 21 — Integration Testing
- `WebApplicationFactory<T>` for testing real HTTP endpoints
- In-memory database for test isolation
- Test the full request pipeline (auth → controller → service → database)
- **Task:** Write integration tests for the booking API. Test the happy path and at least 3 error scenarios.

---

## WEEK 4: DevOps + Interview Prep (Days 22-30)

### Day 22 — Docker
- What containers are and why they matter
- Write a `Dockerfile` for Trainr
- `docker build`, `docker run`, `docker compose`
- **Task:** Containerize Trainr. Write a `docker-compose.yml` that runs the app + a PostgreSQL database.

### Day 23 — CI/CD with GitHub Actions
- What a pipeline is and why automated builds matter
- Build → Test → Deploy pipeline
- **Task:** Create `.github/workflows/ci.yml` that builds the project, runs all tests, and reports results on every push to `main`.

### Day 24 — Deployment & Monitoring Basics
- Environment configuration (`appsettings.Development.json` vs `appsettings.Production.json`)
- Health checks endpoint
- Application monitoring concepts
- **Task:** Add a `/health` endpoint to Trainr. Add environment-specific configuration. Document how you'd deploy this to Azure App Service or Railway.

### Day 25 — Code Review Day
- Full review of the entire Trainr codebase as it stands
- Architecture review, code quality, performance, security
- Fix everything flagged

### Day 26 — Interview Prep: Coding
- 3 coding problems at mid-level difficulty
- Timed: 30 minutes each
- Review solutions together — graded on correctness, efficiency, and code quality

### Day 27 — Interview Prep: System Design
- "Design an Uber-for-training platform" — you already built it, now explain it
- How to talk about architecture decisions in an interview
- Database design questions
- Scaling questions (what happens at 10K users? 100K?)

### Day 28 — Interview Prep: .NET Technical Questions
- Simulated real mid-level .NET phone screen
- Topics: DI, middleware, EF Core, async/await, REST, SOLID, testing
- Answer and get evaluated with coaching

### Day 29 — Interview Prep: Behavioral
- STAR method (Situation, Task, Action, Result)
- Common questions: conflict resolution, tight deadlines, technical disagreements, mistakes
- Practice with real experiences

### Day 30 — Final Review + Next Steps
- Full project review
- Gap assessment: what's solid, what needs more work
- Plan for continued learning beyond the 30 days

---

## The Rules

1. **Do the work before asking for explanations.** Struggle first. That's where learning happens.
2. **Commit every day.** Your GitHub history should show 30 days of consistent work.
3. **Ask questions when you're stuck**, but explain what you tried first.
4. **Code will be reviewed honestly.** If something is wrong, it gets flagged. That's how you improve.
5. **If a day takes longer than 4 hours**, stop and pick it up the next day. Consistency beats intensity.
