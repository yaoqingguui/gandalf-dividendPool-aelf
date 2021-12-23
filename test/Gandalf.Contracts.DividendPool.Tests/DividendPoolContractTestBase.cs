using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;
using Gandalf.Contracts.DividendPoolContract;

namespace Gandalf.Contracts.DividendPool
{
    public class DividendPoolContractTestBase : DAppContractTestBase<DividendPoolContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal DividendPoolContractContainer.DividendPoolContractStub GetDividendPoolContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<DividendPoolContractContainer.DividendPoolContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}