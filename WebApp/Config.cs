/*

*/

namespace WebApp;

static class Config {
    // public static readonly string? USERNAME = Environment.GetEnvironmentVariable("PSQL_USERNAME");
    // public static readonly string? PASSWORD = Environment.GetEnvironmentVariable("PSQL_PASSWORD");
    public static readonly string? CONNECTION_STRING;

    static Config() {
        // if (string.IsNullOrEmpty(USERNAME)) {
        //     throw new ArgumentException("PSQL_USERNAME environment variable cannot be null or empty.");
        // }

        // if (string.IsNullOrEmpty(PASSWORD)) {
        //     throw new ArgumentException("PSQL_PASSWORD environment variable cannot be null or empty.");
        // }

        CONNECTION_STRING = Environment.GetEnvironmentVariable("PSQL_CONNECTION_STRING");
    }
}
