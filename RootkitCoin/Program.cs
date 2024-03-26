using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace RootkitCoin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BlockChain rootkitCoin = new BlockChain();
            rootkitCoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddHmmssffff"), "TxAmount: 50"));
            rootkitCoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddHmmssffff"), "TxAmount: 50"));


            Block newBlock = new Block(1, DateTime.Now.ToString("yyyyMMddHmmssffff"), "TxAmount: 55", "");
            string blockJSON = JsonConvert.SerializeObject(rootkitCoin, Formatting.Indented);
            Console.WriteLine(blockJSON);
                



            Console.Write("Hello, World!");

        }

        class BlockChain
        {
            public List<Block> Chain {  get; set; }
            public BlockChain()
            {
                this.Chain = new List<Block>();
                this.Chain.Add(CreateGenesisBlock());
            }

            public Block CreateGenesisBlock()
            {
                return new Block(0, DateTime.Now.ToString("yyyyMMddHmmssffff"), "GENESIS BLOCK");
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
            

        }

        class Block
        {
            public int Index { get; set; }
            public string PreviousHash { get; set; }

            public string TimeStamp { get; set; }
            public string Data {  get; set; }
            public string Hash { get; set; }

            public Block(int index, string timestampe, string data, string previousHash="")
            {
                this.Index = index;
                this.PreviousHash = previousHash;
                this.TimeStamp = timestampe;
                this.Data = data;
                this.Hash = CalculateHash();
            }

            public string CalculateHash()
            {
                string blockData = this.Index + this.PreviousHash + this.TimeStamp + this.Data;
                byte[] blockBytes = Encoding.ASCII.GetBytes(blockData);
                byte[] hashBytes = SHA256.Create().ComputeHash(blockBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");

                
            }

        }
    }
}
