using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Cosmos.Database.Tests;

[Collection("Collection")]
public class CosmosDatabaseUtilTests : FixturedUnitTest
{
    private readonly ICosmosDatabaseUtil _util;

    public CosmosDatabaseUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICosmosDatabaseUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
