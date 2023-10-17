using System;
using System.IO;
using Google.Apis.Auth.OAuth2;

namespace Radiant.Common.API.Google;

public class RadiantGoogleApi : IDisposable
{
    // ********************************************************************
    //                            Private
    // ********************************************************************
    protected const string APPLICATION_NAME = "Radiant";
    private const string DEFAULT_SERVICE_KEY_FILE_PATH = "radiant-401111-51788a83823c.json"; // default file, if client doesn't override it
    private readonly string fJsonServiceKeyFilePath;
    private readonly string[] fScopes;

    // ********************************************************************
    //                            Constructors
    // ********************************************************************
    public RadiantGoogleApi(string jsonServiceKeyFilePath, string[] scopes)
    {
        // Try to find the default service creds file if it wasn't provided (not custom)
        if (string.IsNullOrWhiteSpace(jsonServiceKeyFilePath))
            jsonServiceKeyFilePath = DEFAULT_SERVICE_KEY_FILE_PATH;

        if (!File.Exists(jsonServiceKeyFilePath))
            throw new Exception($"Required file [{jsonServiceKeyFilePath}] is missing.");

        fJsonServiceKeyFilePath = jsonServiceKeyFilePath;
        fScopes = scopes;
    }

    public virtual void Dispose()
    {
        // Nothing to dispose
    }

    public GoogleCredential GetCredentials()
    {
        GoogleCredential _Credential;

        using (var _Stream = new FileStream(fJsonServiceKeyFilePath, FileMode.Open, FileAccess.Read))
        {
            _Credential = GoogleCredential.FromStream(_Stream).CreateScoped(fScopes);
        }

        return _Credential;
    }
}