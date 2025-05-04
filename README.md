# Introduction 
Booking Service which allows to book an appointment and get available slots.

# Build and Test
To run project:
1. Run `docker-compose up` command
2. Run project- it will generate the database with table and data feed.
3. Or run migration and then run project.
4. Service is running on https://localhost:7254 and can be tested with Swagger.

## Unit tests
Unit tests are written with xUnit and NSubstitute and FluentAssertions. Unit tests will be added for ScheduleAdapter and BasicAuthenticationHandler. 

## Integration tests
Integration tests are about to be implemented with using TestContainers library

# To generate migration:
Run command `dotnet ef migrations add FirstMigration -c AppointmentContext --startup-project ../Service`

# To generate certificate:
Run command `dotnet dev-certs https --trust`

# Data storage, data access and data approach
Data is stored in MS SQL Server. Tables are normalized to minimize data duplication.
Currently, Entity Framework Core is used as ORM since querying is simple and easy to maintain.
With Solution development and growing complexity it will be replaced with Dapper and custom queries.
Tables were designed with CodeFirst approach.

# CI/CD
GitHub Actions will be added in the future.

# Next steps
1. Add integration tests
2. Add CI/CD
3. Add Central Package Management
4. Implement CQRS in future with solution endpoints growth.

# Solution written with assist of Cascade(former Codeium)
Example prompts:
1. "Hey, generate me unit test which covers that line from SlotService:
   'throw new EntityNotFoundException("Day schedule for appointment day cannot be found.");'
Use xunit with NSubstitute and FluentAssertions. Keep MS unit tests recommendations."