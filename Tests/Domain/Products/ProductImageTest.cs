using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ShopTex.Domain.Products;
using Xunit;

namespace ShopTex.Tests.Domain.Products;

public class ProductImageTest : IDisposable
{
    private readonly string _tempDir;

    public ProductImageTest()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "ShopTexTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void Constructor_CreatesImagePath_AndGeneratesKeyAndIV()
    {
        // Arrange
        var productId = "test-product-123";

        // Act
        var image = new ProductImage(productId, _tempDir);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(image.ImagePath));
        Assert.True(File.Exists(Path.GetFullPath(image.ImagePath)) == false); // File should not exist yet
        Assert.False(string.IsNullOrWhiteSpace(image.EncryptionKey));
        Assert.False(string.IsNullOrWhiteSpace(image.InitializationVector));

        // Check key and IV are valid base64
        var keyBytes = Convert.FromBase64String(image.EncryptionKey);
        var ivBytes = Convert.FromBase64String(image.InitializationVector);
        Assert.Equal(32, keyBytes.Length); // 256-bit key
        Assert.Equal(16, ivBytes.Length);  // 128-bit IV
    }

    [Fact]
    public void EncryptImage_EncryptsData_AndSavesToDisk()
    {
        // Arrange
        var productId = "test-product-456";
        var plainImageBytes = Encoding.UTF8.GetBytes("this is a fake image file content");

        var image = new ProductImage(productId, _tempDir);

        // Act
        var success = image.EncryptImage(plainImageBytes);

        // Assert
        Assert.True(success);
        Assert.True(File.Exists(image.ImagePath));

        var fileBytes = File.ReadAllBytes(image.ImagePath);
        Assert.NotEmpty(fileBytes);
        Assert.NotEqual(plainImageBytes, fileBytes);
    }

    public void Dispose()
    {
        // Cleanup temp directory
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }
}
