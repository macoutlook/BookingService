# Introduction 
Booking Service which allows to book an appointment and get available slots.

# Build and Test
To run project:
1. Run `docker-compose up` command
2. Run project- it will generate the database with table and data feed.
3. Or run migration and then run project.

# To generate migration:
Run command `dotnet ef migrations add FirstMigration -c AppointmentContext --startup-project ../Service`

# To generate certificate:
Run command `dotnet dev-certs https --trust`

# Solution written with assist of Cascade(former Codeium)
Example prompts:
1. "Hey, generate me unit test which covers that line from SlotService:
   'throw new EntityNotFoundException("Day schedule for appointment day cannot be found.");'
Use Xunit with NSubstitute and FluentAssertions. Keep MS unit tests recommendations."