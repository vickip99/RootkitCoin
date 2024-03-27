using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EllipticCurve;
using System.Transactions;
using System.Security.Cryptography;
using PublicKey = EllipticCurve.PublicKey;

namespace RootkitCoin
{
    internal class BlockChain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> pendingTransactions { get; set; }
        public Decimal MiningReward { get; set; }


        public BlockChain(int difficulty, decimal miningReward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = miningReward;
            this.pendingTransactions = new List<Transaction>();
        }

        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHmmssffff"), new List<Transaction>());   
        }

        public Block GetLatestBlock()
        {
            return this.Chain.Last();
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLatestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }
        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;
            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");


            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromAddressDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");

                        if (fromAddressDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                    string toAddressDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                    if (toAddressDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }

        public void addPendingTransaction(Transaction tx)
        {
            if (tx.ToAddress is null)
            {
                throw new Exception("Transactions must have a To Address.");
            }

            if (tx.Amount > this.GetBalanceOfWallet(tx.FromAddress))
            {
                throw new Exception("There must be sufficient funds in the wallet.");
            }

            if (tx.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block.");
            }

            this.pendingTransactions.Add(tx);
        }

        public void MinePendingTransaction(PublicKey publicKey)
        {
            Transaction rewardTx = new Transaction(null, publicKey, this.MiningReward);
            this.pendingTransactions.Add(rewardTx);

            Block newBlock = new Block(GetLatestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddmmssffff"), this.pendingTransactions, GetLatestBlock().Hash);

            newBlock.Mine(this.Difficulty);
            this.Chain.Add(newBlock);
            this.pendingTransactions = new List<Transaction>();

        }
    }
}
