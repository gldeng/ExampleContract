using AElf.Kernel;
using AElf.Sdk.CSharp;
using AElf.Sdk.CSharp.Types;
using Api = AElf.Sdk.CSharp.Api;

namespace SimpleToken
{
    #region Events

    public class Transfered : Event
    {
        [Indexed] public Hash From { get; set; }
        [Indexed] public Hash To { get; set; }
        [Indexed] public ulong Amount { get; set; }
    }

    #endregion Events

    public class Contract : CSharpSmartContract
    {
        private readonly BoolField _initialized = new BoolField("_Initialized_");
        private readonly StringField _symbol = new StringField("_Symbol_");
        private readonly StringField _tokenName = new StringField("_TokenName_");
        private readonly UInt64Field _totalSupply = new UInt64Field("_TotalSupply_");
        private readonly UInt32Field _decimals = new UInt32Field("_Decimals_");
        private readonly MapToUInt64<Hash> _balances = new MapToUInt64<Hash>("_Balances_");

        #region View Only Methods

        public string Symbol()
        {
            return _symbol.GetValue();
        }

        public string TokenName()
        {
            return _tokenName.GetValue();
        }

        public ulong TotalSupply()
        {
            return _totalSupply.GetValue();
        }

        public uint Decimals()
        {
            return _decimals.GetValue();
        }

        public ulong BalanceOf(Hash owner)
        {
            return _balances[owner];
        }

        #endregion View Only Methods


        #region Actions

        public void Initialize(string symbol, string tokenName, ulong totalSupply, uint decimals)
        {
            Api.Assert(!_initialized.GetValue(), "Already initialized.");
            Api.Assert(Api.GetContractOwner().Equals(Api.GetTransaction().From),
                "Only owner can initialize the contract state.");
            _symbol.SetValue(symbol);
            _tokenName.SetValue(tokenName);
            _totalSupply.SetValue(totalSupply);
            _decimals.SetValue(decimals);
            _balances[Api.GetTransaction().From] = totalSupply;
            _initialized.SetValue(true);
        }

        public void Transfer(Hash to, ulong amount)
        {
            var from = Api.GetTransaction().From;
            var balSender = _balances[from];
            Api.Assert(balSender > amount, "Insufficient balance.");
            var balReceiver = _balances[to];
            balSender = balSender.Sub(amount);
            balReceiver = balReceiver.Add(amount);
            _balances[from] = balSender;
            _balances[to] = balReceiver;
            new Transfered()
            {
                From = from,
                To = to,
                Amount = amount
            }.Fire();
        }

        #endregion Actions
    }
}