namespace Contracts.Tests;

public sealed class ResultTests
{
    [Theory]
    [MemberData(nameof(ValueTypeData))]
    public void Result_ValueType_Instantiate_ShouldSucceed(int value)
    {
        var result = new Contracts.Results.Result<int>(value);

        result.IsSuccess.Should().BeTrue();

        var resultValue = (int)result;

        resultValue.Should().Be(value);
    }

    public static TheoryData<int> ValueTypeData()
    {
        return new TheoryData<int>
        {
            { 1 },
            { 2 },
            { 3 }
        };
    }
}
