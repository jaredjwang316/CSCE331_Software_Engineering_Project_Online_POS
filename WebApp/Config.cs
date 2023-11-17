/*
    File: Config.cs
    Author: Griffin Beaudreau
    Date: October 30, 2023
    Description: This file contains the Config class, which is used to store
        general configuration information for the web app.
*/

namespace WebApp;

/// <summary>
/// Config class, used to store general configuration information.
/// </summary>
static class Config {
    public static readonly string GOOGLE_TRANSLATE_API_KEY;
    public static readonly string CONNECTION_STRING;
    public static readonly string AWS_DB_NAME = "csce315331_07r_db";
    public static string returnUrl; // This is the URI that the user will be redirected to after logging in.

    static Config() {
        GOOGLE_TRANSLATE_API_KEY = Environment.GetEnvironmentVariable("GOOGLE_TRANSLATE_API_KEY")
            ?? throw new ArgumentException("GOOGLE_TRANSLATE_API_KEY environment variable cannot be null or empty.");
            
        CONNECTION_STRING = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_AWS_DB")
            ?? throw new ArgumentException("POSTGRESQLCONNSTR_AWS_DB environment variable cannot be null or empty.");

        #if DEBUG
        returnUrl = "https://localhost:5001";
        #else
        returnUrl = "https://07r-webapp.azurewebsites.net/";
        #endif
    }
}
