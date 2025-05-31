using DotNetEnv;

namespace ShopTex;

public class Configs
{
    static Configs()
    {
        Env.Load();
    }
    
    public const string ValidEmail = "isep.ipp.pt";

    public static string DbConnection => Environment.GetEnvironmentVariable("DB_CONNECTION_SERVER") 
                                         ?? throw new Exception("DB is not set");
    
    public static string SYS_ADMIN_ROLE_NAME ="System Administrator";
    public static string USER_ROLE_NAME = "Client";
    public static string STORE_ADMIN_ROLE_NAME = "Store Administrator";
    public static string STORE_COLAB_ROLE_NAME = "Store Collaborator";
    
}
