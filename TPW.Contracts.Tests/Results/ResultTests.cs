using Contracts.Results;

namespace Contracts.Tests;

public sealed class ResultTests
{
    [Theory]
    [MemberData(nameof(ValueTypeData))]
    public void Result_ValueType_Instantiate_ShouldSucceed(int value)
    {
        var result = new Result<int>(value);

        result.IsSuccess.Should().BeTrue();

        var resultValue = (int)result;

        resultValue.Should().Be(value);
    }

    [Fact]
    public async Task Result_ValueType_ArgNull_ShouldFail()
    {
        Exception e = null;
        try
        {
            _ = new Result<string>((string)null);
        }
        catch (Exception ex)
        {
            e = ex;
        }

        e.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(SelectIfData))]
    public void Result_Bind_ValidData_ShouldSucceed(int before, string after)
    {
        var beforeResult = new Result<int>(before);

        var afterResult = beforeResult.Bind(x => x.ToString());

        afterResult.IsSuccess.Should().BeTrue();
        ((string)afterResult).Should().BeEquivalentTo(after);
    }

    [Theory]
    [MemberData(nameof(EffectData))]
    public void Result_Effect_ValidData_ShouldSucceed(MutableClass before, Action<MutableClass> function, int valueAfter)
    {
        var beforeResult = new Result<MutableClass>(before);

        var afterResult = beforeResult.Effect(x => function(x));

        afterResult.IsSuccess.Should().BeTrue();
        ((MutableClass)afterResult).Value.Should().Be(valueAfter);
    }

    [Theory]
    [MemberData(nameof(SelectIfData))]
    public async Task Result_BindAsync_ValidData_ShouldSucceed(int before, string after)
    {
        var beforeResult = Task.FromResult(new Result<int>(before));

        var afterResult = beforeResult.BindAsync(x => Task.FromResult(x.ToString()));

        var taskResult = await afterResult;

        taskResult.IsSuccess.Should().BeTrue();
        ((string)taskResult).Should().BeEquivalentTo(after);
    }

    [Theory]
    [MemberData(nameof(ErrorData))]
    public void Result_SelectIf_ErrorData_ShouldContainError(Errors.Error error)
    {
        var errorResult = new Result<int>(error);

        var afterResult = errorResult.Bind(x => x.ToString());

        afterResult.IsSuccess.Should().BeFalse();

        Exception e = null;
        try
        {
            _ = (string)afterResult;
        }
        catch (Exception ex)
        {
            e = ex;
        }

        e.Should().NotBeNull();
        e.Message.Should().BeEquivalentTo("Value of result is null");
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

    public static TheoryData<int, string> SelectIfData()
    {
        return new TheoryData<int, string>
        {
            { 1 , "1" },
            { 2 , "2" },
            { 3 , "3" }
        };
    }

    public static TheoryData<Errors.Error> ErrorData()
    {
        return new TheoryData<Errors.Error>
        {
            { new Errors.Error("Some error", 0) }
        };
    }

    public static TheoryData<MutableClass, Action<MutableClass>, int> EffectData()
    {
        static void addAction(MutableClass x) => x.Value++;

        return new TheoryData<MutableClass, Action<MutableClass>, int>
        {
            { new(1) , addAction, 2 },
            { new(2) , addAction, 3 },
            { new(3) , addAction, 4 },
        };
    }

    public sealed class MutableClass
    {
        public int Value { get; set; } = 0;

        public MutableClass(int value)
        {
            Value = value;
        }
    }
}
