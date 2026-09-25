using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.MsSql;

namespace GigBooking.Infrastructure.IntegrationTests;

public class DatabaseContainerTests : IAsyncLifetime
{

    private  MsSqlContainer? _msSqlContainer = null;
    public async Task DisposeAsync()
    {
        await _msSqlContainer!.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await DockerBootstrapper.EnsureDockerIsRunning();
        _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

        await _msSqlContainer.StartAsync();
    }

    [Fact]
    public void CanConnect_WhenContainerStarts_ReturnsValidConnectionString()
    {
        string connectionString = _msSqlContainer!.GetConnectionString();
        Assert.NotNull(connectionString);
        Assert.NotEmpty(connectionString);
    }



}
