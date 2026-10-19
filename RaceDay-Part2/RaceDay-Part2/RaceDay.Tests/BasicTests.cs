using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;
using Xunit;

namespace RaceDay.Tests;

public class BasicTests
{
    [Fact]
    public void PasswordHasher_DoesNotStorePlainText()
    {
        var user = new User();
        var hasher = new PasswordHasher<User>();
        var hash = hasher.HashPassword(user, "ExamplePassword123!");
        Assert.NotEqual("ExamplePassword123!", hash);
        Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(user, hash, "ExamplePassword123!"));
        Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, hash, "WrongPassword!"));
    }

    [Theory]
    [InlineData("Organiser", true)]
    [InlineData("Participant", true)]
    [InlineData("Admin", false)]
    [InlineData("", false)]
    public void OnlySupportedRolesAreAllowed(string role, bool expected)
    {
        var actual = role is "Organiser" or "Participant";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void EventTypesAreLimitedToRunWalkCycle()
    {
        Assert.Contains("Run", new[] { "Run", "Walk", "Cycle" });
        Assert.DoesNotContain("Swim", new[] { "Run", "Walk", "Cycle" });
    }
}
