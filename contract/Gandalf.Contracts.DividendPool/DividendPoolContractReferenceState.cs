using AElf.Contracts.MultiToken;

namespace Gandalf.Contracts.DividendPoolContract
{
    public partial class DividendPoolContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
    }
}