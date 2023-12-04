/*
    File: Config.cs
    Author: Griffin Beaudreau
    Date: October 30, 2023
    Purpose: This file contains the Config class, which is used to store
        general configuration information for the web app.
*/

namespace WebApp;

/// <summary>
/// Config class, used to store general configuration information.
/// </summary>
static class Config {
    public static readonly string GOOGLE_TRANSLATE_API_KEY;
    public static readonly string AZURE_MAPS_API_KEY;
    public static readonly string OPENAI_API_KEY;
    public static readonly string AZURE_SPEECH_KEY;
    public static readonly string AZURE_SPEECH_REGION = "eastus";
    public static readonly string CONNECTION_STRING;
    public static readonly string AWS_DB_NAME = "csce315331_07r_db";
    public static string returnUrl;

    static Config() {
        GOOGLE_TRANSLATE_API_KEY = Environment.GetEnvironmentVariable("GOOGLE_TRANSLATE_API_KEY")
            ?? throw new ArgumentException("GOOGLE_TRANSLATE_API_KEY environment variable cannot be null or empty.");
        
        AZURE_MAPS_API_KEY = Environment.GetEnvironmentVariable("AZURE_MAPS_API_KEY")
            ?? throw new ArgumentException("AZURE_MAPS_API_KEY environment variable cannot be null or empty.");

        CONNECTION_STRING = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_AWS_DB")
            ?? throw new ArgumentException("POSTGRESQLCONNSTR_AWS_DB environment variable cannot be null or empty.");

        OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new ArgumentException("OPENAI_API_KEY environment variable cannot be null or empty.");
        
        AZURE_SPEECH_KEY = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY")
            ?? throw new ArgumentException("AZURE_SPEECH_KEY environment variable cannot be null or empty.");

        #if DEBUG
        returnUrl = "https://localhost:5001";
        #else
        returnUrl = "https://07r-webapp.azurewebsites.net/";
        #endif
    }
}
