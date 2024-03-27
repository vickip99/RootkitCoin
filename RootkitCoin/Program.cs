using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using EllipticCurve;
using Newtonsoft.Json;
using RootkitCoin;

namespace RootkitCoin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PrivateKey Key1 = new PrivateKey();
            PublicKey wallet1 = Key1.publicKey();

            PrivateKey Key2 = new PrivateKey();
            PublicKey wallet2 = Key2.publicKey();

            PrivateKey Key3 = new PrivateKey();
            PublicKey wallet3 = Key3.publicKey();

            BlockChain rootkitCoin = new BlockChain(2, 100);

            rootkitCoin.MinePendingTransaction(wallet1);
            rootkitCoin.MinePendingTransaction(wallet2);
            rootkitCoin.MinePendingTransaction(wallet3);

            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet1).ToString());
            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet2).ToString());
            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet3).ToString());

            Transaction tx1 = new Transaction(wallet1, wallet2, 55.00m);
            tx1.SignTransaction(Key1);
            rootkitCoin.addPendingTransaction(tx1);

            Transaction tx2 = new Transaction(wallet3, wallet2, 20.00m);
            tx2.SignTransaction(Key3);
            rootkitCoin.addPendingTransaction(tx2);

            rootkitCoin.MinePendingTransaction(wallet3);

            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet1).ToString());
            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet2).ToString());
            Console.Write("\nBalance of wallet1: $" + rootkitCoin.GetBalanceOfWallet(wallet3).ToString());


            string blockJSON = JsonConvert.SerializeObject(rootkitCoin, Formatting.Indented);
            Console.WriteLine(blockJSON);

            if (rootkitCoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is valid!!!");
            }
            else
            {
                Console.WriteLine("Blockchain is NOT valid.");
            }
        }
    }
}
