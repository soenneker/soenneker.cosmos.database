using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Soenneker.Cosmos.Database.Tests;

public class CacheKeyRegressionTests
{
    [Test]
    public async Task AccountKeyDigestPreservesUppercaseSha256ForStackAndPooledBuffers()
    {
        MethodInfo method = typeof(CosmosDatabaseUtil).GetMethod("GetAccountKeyHash", BindingFlags.NonPublic | BindingFlags.Static)!;
        foreach (string key in new[] { "", "test-account-key", new string('界', 500) })
        {
            string expected = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
            string actual = (string)method.Invoke(null, new object[] { key })!;
            await Assert.That(actual).IsEqualTo(expected);
        }
    }
}
