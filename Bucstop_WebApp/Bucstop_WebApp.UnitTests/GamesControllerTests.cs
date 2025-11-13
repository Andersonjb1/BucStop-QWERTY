using Xunit;
using BucStop.Controllers; // To access the GamesController class

namespace Bucstop_WebApp.UnitTests; 



public class GamesControllerTests
{
    // NOTE: Since the methods ValidateGameSubmission and ContainsDangerousCode 
    // are private in the original GamesController, we will test the publicly 
    // accessible helper class `ValidationResult` for now, 
    // and create an instance of the controller to eventually call the 
    // public action method `ValidateSuggestion` in future, more complex tests.
    //
    // However, the internal helper classes like GameSubmissionJson and 
    // ValidationResult are publicly nested, so we can test them directly.

    #region ValidationResult Helper Tests

    [Fact]
    public void ValidationResult_StartsValid()
    {
        // Arrange
        var result = new GamesController.ValidationResult();

        // Act & Assert
        // It should be valid (no errors) when first created.
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal("", result.ErrorMessage);
    }

    [Fact]
    public void ValidationResult_BecomesInvalid_WhenErrorIsAdded()
    {
        // Arrange
        var result = new GamesController.ValidationResult();
        string fieldName = "Title";
        string errorMessage = "Title is required.";

        // Act
        result.AddError(fieldName, errorMessage);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(fieldName, result.Errors.Keys);
        Assert.Equal(errorMessage, result.Errors[fieldName]);
        Assert.Contains(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void ValidationResult_MultipleErrors_GeneratesCorrectMessage()
    {
        // Arrange
        var result = new GamesController.ValidationResult();
        string error1 = "Error A.";
        string error2 = "Error B.";

        // Act
        result.AddError("Field1", error1);
        result.AddError("Field2", error2);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(error1, result.ErrorMessage);
        Assert.Contains(error2, result.ErrorMessage);
        Assert.Equal(2, result.Errors.Count);
    }

    #endregion

    // We can add more regions here for specific methods, 
    // for example:
    // #region IndexAsync Tests
    // [Fact]
    // public async Task IndexAsync_ReturnsViewWithGames() { ... }
    // #endregion

}
