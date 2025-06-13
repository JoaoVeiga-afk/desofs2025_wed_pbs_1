using System.Text;
using DotNetEnv;
using Konscious.Security.Cryptography;

namespace ShopTex;

public class Configurations
{
    static Configurations()
    {
        Env.Load();
    }

    public const string ValidEmail = "isep.ipp.pt";

    public static string DbConnection => Environment.GetEnvironmentVariable("DB_CONNECTION_SERVER")
                                         ?? throw new Exception("DB is not set");

    public const string SYS_ADMIN_ROLE_NAME = "System Administrator";
    public const string USER_ROLE_NAME = "Client";
    public const string STORE_ADMIN_ROLE_NAME = "Store Administrator";
    public const string STORE_COLAB_ROLE_NAME = "Store Collaborator";
    public const string IMAGE_STORAGE_PATH = "ProductImages";
    // 10 MB
    public const int MAX_FILE_SIZE = 10 * 1024 * 1024;

    public static string HashString(string source, byte[] salt)
    {
        var hasher = new Argon2id(Encoding.UTF8.GetBytes(source))
        {
            Salt = salt,
            Iterations = 2,
            MemorySize = 1024,
            DegreeOfParallelism = 1
        };
        byte[] hashBytes = hasher.GetBytes(32);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

}
