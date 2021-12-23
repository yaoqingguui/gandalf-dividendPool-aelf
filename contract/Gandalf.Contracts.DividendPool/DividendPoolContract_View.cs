using System;
using System.Linq;
using AElf.CSharp.Core;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace Gandalf.Contracts.DividendPoolContract
{
    public partial class DividendPoolContract
    {
        /**
         * PoolLength
         */
        public override Int32Value PoolLength(Empty input)
        {
            return new Int32Value
            {
                Value = State.PoolInfo.Value.PoolList.Count
            };
        }

        /**
         * Pending
         */
        public override PendingOutput Pending(PendingInput input)
        {
            var pool = State.PoolInfo.Value.PoolList[input.Pid];
            var user = State.UserInfo[input.Pid][input.User];
            var tokenList = State.TokenList.Value.Tokens;

            var pendingOutput = new PendingOutput();

            var number = Context.CurrentHeight > State.EndBlock.Value.Value
                ? State.EndBlock.Value.Value
                : Context.CurrentHeight;
            if (number > pool.LastRewardBlock && !pool.TotalAmount.Equals(0))
            {
                var multiplier = number.Sub(pool.LastRewardBlock);
                for (int i = 0; i < tokenList.Count; i++)
                {
                    var tokenSymbol = tokenList[i];
                    var amount = GetUserReward(input.Pid,pool, user, tokenSymbol, multiplier);
                    pendingOutput.Tokens.Add(tokenSymbol);
                    pendingOutput.Amounts.Add(amount);
                }
            }
            else
            {
                for (int i = 0; i < tokenList.Count; i++)
                {
                    pendingOutput.Tokens.Add(tokenList[i]);
                    pendingOutput.Amounts.Add(new BigIntValue(0));
                }
            }
            return pendingOutput;
        }
        
        /**
         * GetTokenListLength
         */
        public override Int32Value GetTokenListLength(Empty input)
        {
            return new Int32Value
            {
                Value = State.TokenList.Value.Tokens.Count
            };
        }
        
        /**
         * IsTokenList
         */
        public override BoolValue IsTokenList(Token input)
        {
            return new BoolValue
            {
                Value = State.TokenList.Value.Tokens.Contains(input.TokenSymbol)
            };
        }

        /**
         * owner
         */
        public override Address Owner(Empty input)
        {
            return State.Owner.Value;

        }
        
        /**
         *  Get token by index from token list.
         */
        public override StringValue TokenList(Int32Value input)
        {
            return new StringValue
            {
                Value = State.TokenList.Value.Tokens[input.Value]
            };
        }
        
        /**
         *  Get perBlock from state
         */
        public override BigIntValue PerBlock(StringValue input)
        {
            return State.PerBlock[input.Value];
        }
        
        /**
         *  Get poolInfo by pid  address form state.
         */
        public override PoolInfoStruct PoolInfo(Int32Value input)
        {
            return State.PoolInfo.Value.PoolList[input.Value];
        }
        
        /**
         * Get user Info
         */
        public override UserInfoStruct UserInfo(UserInfoInput input)
        {
            return State.UserInfo[input.Pid][input.User];
        }
        
        /**
         * Get totalAllocPoint
         */
        public override Int64Value TotalAllocPoint(Empty input)
        {
            return new Int64Value
            {
                Value = State.TotalAllocPoint.Value
            };
        }
        
        /**
         * Get startBlock.
         */
        public override Int64Value StartBlock(Empty input)
        {
            return new Int64Value
            {
                Value = State.StartBlock.Value.Value
            };
        }
        
        /**
         *  Get endBlock.
         */
        public override Int64Value EndBlock(Empty input)
        {
            return new Int64Value
            {
                Value = State.EndBlock.Value.Value
            };
        }
        
        /**
         * Get Cycle.
         */
        public override Int64Value Cycle(Empty input)
        {
            return new Int64Value
            {
                Value = State.Cycle.Value.Value
            };
        }
        
        /**
         *  Get RewardDebt.
         */
        public override BigIntValue RewardDebt(RewardDebtInput input)
        {
            return State.RewardDebt[input.Pid][input.User][input.Token];
        }

        /**
         * Get AccPerShare
         */
        public override BigIntValue AccPerShare(AccPerShareInput input)
        {
            return State.AccPerShare[input.Pid][input.Token];
        }

        private BigIntValue GetUserReward(int pid, PoolInfoStruct pool,
            UserInfoStruct user,
            string token,
            long multiplier)
        {
            var reward = State.PerBlock[token]
                .Mul(multiplier)
                .Mul(pool.AllocPoint)
                .Div(State.TotalAllocPoint.Value);
            var tokenMultiplier = GetMultiplier(token);
            var accPerShare = State.AccPerShare[pid][token].Add(
                Convert.ToInt64((tokenMultiplier.Mul(reward).Div(pool.TotalAmount)).ToString())
            );

            var amount = user.Amount.Mul(accPerShare).Div(tokenMultiplier).Sub(State.RewardDebt[pid][Context.Sender][token]);
            return amount;
        }
    }
}