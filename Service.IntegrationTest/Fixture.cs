using Testcontainers.MsSql;

namespace Service.IntegrationTest;

public class Fixture : IAsyncDisposable
{
    private readonly MsSqlContainer _msSqlContainer;
    
    public Fixture()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
            .Build();

        _msSqlContainer.StartAsync();
        
        // TODO: Configure connection string, create database and run migrations, then feed data
    }

    public async ValueTask DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}
