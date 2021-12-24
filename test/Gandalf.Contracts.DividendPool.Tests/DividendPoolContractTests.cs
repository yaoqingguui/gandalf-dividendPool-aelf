using System.Threading.Tasks;

using Xunit;

namespace Gandalf.Contracts.DividendPool
{
    public partial class DividendPoolContractTests : DividendPoolContractTestBase
    {
       
        
        
        [Fact]
        public async Task Test()
        {
            await Initialize();
        }
        
        
    }
}