using System;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using ShopTex.Domain.Shared.Validation;
using Xunit;

namespace ShopTex.Tests.Domain.Shared.Validation;

public class NotEmptyGuidAttributeTest
{
    private readonly NotEmptyGuidAttribute _attribute = new();

    [Fact]
    public void IsValid_WithNonEmptyGuid_ShouldReturnTrue()
    {
        var guid = Guid.NewGuid();

        var result = _attribute.IsValid(guid);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptyGuid_ShouldReturnFalse()
    {
        var emptyGuid = Guid.Empty;

        var result = _attribute.IsValid(emptyGuid);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithNull_ShouldReturnFalse()
    {
        var result = _attribute.IsValid(null);

        result.Should().BeFalse();
    }

    [Fact]
    public void FormatErrorMessage_ShouldReturnCustomMessage()
    {
        var message = _attribute.FormatErrorMessage("UserId");

        message.Should().Be("UserId cannot be empty.");
    }
}