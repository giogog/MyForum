using System.Data;
using Npgsql;

namespace Database;

public class PostgresConnectivityTests
{
    private const string RawConnectionString = "host=localhost port=5432 dbname=myforum-db user=postgres password=!Ab123123 connect_timeout=10 sslmode=prefer";

    [Fact]
    public async Task Can_connect_to_postgres()
    {
        var connectionString = NormalizeConnectionString(RawConnectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        Assert.Equal(ConnectionState.Open, connection.State);
    }

    private static string NormalizeConnectionString(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new ArgumentException("Connection string is empty.", nameof(raw));
        }

        // Standard ADO.NET format is semicolon-separated; provided is libpq-style (space-separated).
        if (raw.Contains(';'))
        {
            return raw;
        }

        var tokens = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var mapped = tokens.Select(MapToken);
        return string.Join(';', mapped);
    }

    private static string MapToken(string token)
    {
        var idx = token.IndexOf('=');
        if (idx <= 0 || idx >= token.Length - 1)
        {
            return token;
        }

        var key = token[..idx].Trim();
        var value = token[(idx + 1)..].Trim();

        return key.ToLowerInvariant() switch
        {
            "host" => $"Host={value}",
            "port" => $"Port={value}",
            "dbname" => $"Database={value}",
            "user" => $"Username={value}",
            "password" => $"Password={value}",
            "connect_timeout" => $"Timeout={value}",
            "sslmode" => $"Ssl Mode={value}",
            _ => token
        };
    }
}
