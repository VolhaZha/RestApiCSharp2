using NLog;
using RestApiCSharp.Clients;
using RestApiCSharp.ConstantsTestingGeneral;

namespace RestApiCSharp.Tests
{
    public abstract class BaseApiTest
    {
        protected ApiClientHttp ApiClientInstance { get; private set; }
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        protected BaseApiTest()

        {
        Logger.Info("Initializing BaseApiTest");

        try
        {
            ApiClientHttp.Initialize(
                baseUrl: ConstantsTesting.BaseUrl,
                clientId: ConstantsTesting.ClientId,
                clientSecret: ConstantsTesting.ClientSecret
            );

            ApiClientInstance = ApiClientHttp.GetInstance();  
            
            Logger.Info("ApiClient initialized successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error initializing ApiClientHttp.");
            throw;  
        }

        }
    }
}
