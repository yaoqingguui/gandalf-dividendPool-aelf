using AElf.Contracts.MultiToken;

namespace Gandalf.Contracts.DividendPool
{
    public partial class DividendPoolContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
    }
}