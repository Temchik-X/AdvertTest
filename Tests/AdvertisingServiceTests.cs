using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using AdvertTest.Services;

public class AdvertisingServiceTests
{
    private readonly AdvertisingService _service;

    public AdvertisingServiceTests()
    {
        _service = new AdvertisingService();
    }

    [Fact]
    public void LoadFromFile_ValidData_LoadsCorrectly()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, @"
            Яндекс.Директ:/ru
            Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
            Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
            Крутая реклама:/ru/svrd");

        try
        {
            // Act
            _service.LoadFromFile(tempFilePath);

            // Assert
            var platformsForRu = _service.FindPlatformsForLocation("/ru");
            Assert.Contains("Яндекс.Директ", platformsForRu);
            Assert.DoesNotContain("Ревдинский рабочий", platformsForRu);

            var platformsForSvrd = _service.FindPlatformsForLocation("/ru/svrd");
            Assert.Contains("Крутая реклама", platformsForSvrd);
            Assert.Contains("Яндекс.Директ", platformsForSvrd);
            Assert.DoesNotContain("Ревдинский рабочий", platformsForSvrd);

            var platformsForRevda = _service.FindPlatformsForLocation("/ru/svrd/revda");
            Assert.Contains("Крутая реклама", platformsForRevda);
            Assert.Contains("Яндекс.Директ", platformsForRevda);
            Assert.Contains("Ревдинский рабочий", platformsForRevda);
            Assert.DoesNotContain("Газета уральских москвичей", platformsForRevda);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public void FindPlatformsForLocation_InvalidLocation_ReturnsEmptyList()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, "Яндекс.Директ:/ru");

        try
        {
            _service.LoadFromFile(tempFilePath);

            // Act
            var result = _service.FindPlatformsForLocation("");

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public void LoadFromFile_InvalidFile_ThrowsException()
    {
        // Arrange
        var invalidFilePath = "nonexistent_file.txt";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _service.LoadFromFile(invalidFilePath));
    }

    [Fact]
    public void LoadFromFile_MalformedData_SkipsInvalidLines()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, @"
            Яндекс.Директ:/ru
            InvalidLineWithoutColon
            Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik");

        try
        {
            // Act
            _service.LoadFromFile(tempFilePath);

            // Assert
            var platformsForRu = _service.FindPlatformsForLocation("/ru");
            Assert.Contains("Яндекс.Директ", platformsForRu);
            Assert.DoesNotContain("InvalidLineWithoutColon", platformsForRu);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }
}