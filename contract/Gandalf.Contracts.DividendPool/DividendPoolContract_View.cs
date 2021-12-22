using System;
using System.Linq;
using AElf.CSharp.Core;
using AElf.Types;
using Gandalf.Contracts.DividendPool;
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
                    var amount = GetUserReward(pool, user, tokenSymbol, multiplier);
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

        private BigIntValue GetUserReward(PoolInfoStruct pool,
            UserInfoStruct user,
            string token,
            long multiplier)
        {
            var reward = State.PerBlock[token]
                .Mul(multiplier)
                .Mul(pool.AllocPoint)
                .Div(State.TotalAllocPoint.Value);
            var tokenMultiplier = GetMultiplier(token);
            var accPerShare = pool.AccPerShare[token].Add(
                Convert.ToInt64((tokenMultiplier.Mul(reward).Div(pool.TotalAmount)).ToString())
            );

            var amount = user.Amount.Mul(accPerShare).Div(tokenMultiplier).Sub(user.RewardDebt[token]);
            return amount;
        }
    }
}