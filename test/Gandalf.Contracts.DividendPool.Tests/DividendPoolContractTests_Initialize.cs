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
        public const string RewardToken2 = "AAAE";

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
                Cycle = 100
            });
            await CreateToken();
            return stub;
        }


        private async Task CreateToken()
        {
            var tokenStub = GetTokenContractStub(OwnerKeyPair);
            await tokenStub.Create.SendAsync(new CreateInput
            {
                Decimals = 5,
                Symbol = LockedToken,
                Issuer = Owner,
                IsBurnable = true,
                TokenName = LockedToken,
                TotalSupply = 100000000000
            });
            await tokenStub.Issue.SendAsync(new IssueInput
            {
                Amount = 100000000000,
                Symbol = LockedToken,
                To = Owner
            });

            await tokenStub.Transfer.SendAsync(new TransferInput
            {
                Amount = 60000000000,
                Symbol = LockedToken,
                To = Tom
            });

            await tokenStub.Transfer.SendAsync(new TransferInput
            {
                Amount = 20000000000,
                Symbol = LockedToken,
                To = Kitty
            });

            var tomBalance = await tokenStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Tom,
                Symbol = LockedToken
            });
            tomBalance.Balance.ShouldBe(60000000000);

            var kittyBalance = await tokenStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Kitty,
                Symbol = LockedToken
            });
            kittyBalance.Balance.ShouldBe(20000000000);

            await tokenStub.Create.SendAsync(new CreateInput
            {
                Decimals = 5,
                Symbol = RewardToken1,
                Issuer = Owner,
                IsBurnable = true,
                TotalSupply = 10000000000,
                TokenName = RewardToken1
            });
            await tokenStub.Issue.SendAsync(new IssueInput
            {
                Amount = 10000000000,
                Symbol = RewardToken1,
                To = Owner
            });
                
            var usdt = await tokenStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Owner,
                Symbol = RewardToken1
            });
            usdt.Balance.ShouldBe(10000000000);

            await tokenStub.Create.SendAsync(new CreateInput
            {
                Decimals = 2,
                Issuer = Owner,
                Symbol = RewardToken2,
                IsBurnable = true,
                TokenName = RewardToken2,
                TotalSupply = 10000000000,
            });
            await tokenStub.Issue.SendAsync(new IssueInput
            {
                Amount = 10000000000,
                Symbol = RewardToken2,
                To = Owner
            });
            
        }
    }
}