using System.Linq;
using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AElf.ContractTestBase.ContractTestKit;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Gandalf.Contracts.DividendPoolContract;
using Shouldly;

namespace Gandalf.Contracts.DividendPool
{
    public partial class DividendPoolContractTests
    {
        public Address Owner;
        public ECKeyPair OwnerKeyPair;
        public Address Tom;
        public ECKeyPair TomKeyPair;
        public Address Kitty;
        public ECKeyPair KittyKeyPair;

        public const string LockedToken = "ISTAR";
        public const string RewardToken1 = "USDT";
        public const string RewardToken2 = "ELF";

        private async Task<DividendPoolContractContainer.DividendPoolContractStub> Initialize()
        {
            OwnerKeyPair = SampleAccount.Accounts.First().KeyPair;
            Owner = Address.FromPublicKey(OwnerKeyPair.PublicKey);
            TomKeyPair = SampleAccount.Accounts[1].KeyPair;
            Tom = Address.FromPublicKey(TomKeyPair.PublicKey);
            KittyKeyPair = SampleAccount.Accounts[2].KeyPair;
            Kitty = Address.FromPublicKey(KittyKeyPair.PublicKey);
            var stub = GetDividendPoolContractStub(OwnerKeyPair);
            // initialize contract.
            await stub.Initialize.SendAsync(new InitializeInput
            {
                Owner = Owner,
                Cycle = 50
            });
            await CreateToken();
            return stub;
        }


        private async Task CreateToken()
        {
            var istarStub = GetTokenContractStub(OwnerKeyPair);
            await istarStub.Create.SendAsync(new CreateInput
            {
                Decimals = 10,
                Symbol = LockedToken,
                Issuer = Owner,
                IsBurnable = true,
                TokenName = LockedToken,
                TotalSupply = 10000000000
            });
            await istarStub.Issue.SendAsync(new IssueInput
            {
                Amount = 10000000000,
                Symbol = LockedToken,
                To = Owner
            });

            await istarStub.Transfer.SendAsync(new TransferInput
            {
                Amount = 60000000000,
                Symbol = LockedToken,
                To = Tom
            });

            await istarStub.Transfer.SendAsync(new TransferInput
            {
                Amount = 20000000000,
                Symbol = LockedToken,
                To = Kitty
            });

            var tomBalance = await istarStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Tom,
                Symbol = LockedToken
            });
            tomBalance.Balance.ShouldBe(60000000000);

            var kittyBalance = await istarStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Kitty,
                Symbol = LockedToken
            });
            kittyBalance.Balance.ShouldBe(20000000000);
        }
    }
}