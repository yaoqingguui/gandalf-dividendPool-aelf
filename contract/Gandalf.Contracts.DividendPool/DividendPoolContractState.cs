using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace Gandalf.Contracts.DividendPool
{
    /// <summary>
    /// The state class of the contract, it inherits from the AElf.Sdk.CSharp.State.ContractState type. 
    /// </summary>
    public class DividendPoolContractState : ContractState
    {   
        public SingletonState<Address> Owner { get; set; }
        public SingletonState<AddressList> AddressList { get; set; }
        // token=> number
        public MappedState<string,BigIntValue> PerBlock { get; set; }
        public SingletonState<PoolInfo> PoolInfo { get; set; }
        // pid=>user=>userInfo
        public MappedState<Int32State, Address, UserInfoStruct> UserInfo { get; set; }
        public SingletonState<BigIntValue> TotalAllocPoint { get; set; }
        public SingletonState<Int64State> StartBlock { get; set; }
        public SingletonState<Int64State> EndBlock { get; set; }
        public SingletonState<Int64State> Cycle { get; set; }
    }
}