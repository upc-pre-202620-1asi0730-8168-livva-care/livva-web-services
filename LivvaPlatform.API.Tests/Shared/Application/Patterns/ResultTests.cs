using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.Tests.Shared.Application.Patterns;

/// <summary>
///     Unit tests for <see cref="Result{TValue, TError}" />.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Success_ShouldReportIsSuccess()
    {
        Result<int, string> result = new Result<int, string>.Success(5);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        Result<int, string> result = new Result<int, string>.Success(5);

        var mapped = result.Map(value => value * 2);

        Assert.Equal(10, mapped.Fold(value => value, _ => -1));
    }

    [Fact]
    public void Map_OnFailure_ShouldKeepError()
    {
        Result<int, string> result = new Result<int, string>.Failure("error");

        var mapped = result.Map(value => value * 2);

        Assert.True(mapped.IsFailure);
        Assert.Equal("error", mapped.Fold(_ => string.Empty, error => error));
    }
}