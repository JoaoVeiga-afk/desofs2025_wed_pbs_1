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
}
