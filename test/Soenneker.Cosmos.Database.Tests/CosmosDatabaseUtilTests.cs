using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cosmos.Database.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class CosmosDatabaseUtilTests : HostedUnitTest
{
    private readonly ICosmosDatabaseUtil _util;

    public CosmosDatabaseUtilTests(Host host) : base(host)
    {
        _util = Resolve<ICosmosDatabaseUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
