/*

*/

namespace WebApp;

static class Config {
    public static readonly string CONNECTION_STRING;
    public static readonly string AWS_DB_NAME = "csce315331_07r_db";

    static Config() {
        CONNECTION_STRING = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_AWS_DB")
            ?? throw new ArgumentException("POSTGRESQLCONNSTR_AWS_DB environment variable cannot be null or empty.");
    }
}