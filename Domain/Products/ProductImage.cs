using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ShopTex.Domain.Products;

public class ProductImage
{
    [Required]
    [StringLength(260)]
    public string ImagePath { get; private set; }

    [Required]
    public string EncryptionKey { get; private set; }

    [Required]
    public string InitializationVector { get; private set; }

    private ProductImage() {}
    
    public ProductImage(string productId, string imageStoragePath)
    {
        // Generate deterministic filename from product ID
        var fileName = GenerateImageFilename(productId);
        var fullPath = Path.Combine(imageStoragePath, fileName);
        ImagePath = fullPath;

        // Generate AES key and IV, then encrypt the image
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        EncryptionKey = Convert.ToBase64String(aes.Key);
        InitializationVector = Convert.ToBase64String(aes.IV);
    }

    private static string GenerateImageFilename(string productId)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(productId + DateTime.UtcNow.Ticks));
        return $"{Convert.ToBase64String(hash).Replace("/", "_").Replace("+", "-").Substring(0, 16)}.enc";
    }

    public bool EncryptImage(byte[] plainData)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(EncryptionKey);
        aes.IV = Convert.FromBase64String(InitializationVector);

        using var encryptor = aes.CreateEncryptor();
        var encryptedImage = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);

        // Save only the encrypted data
        try
        {
            File.WriteAllBytes(ImagePath, encryptedImage);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
