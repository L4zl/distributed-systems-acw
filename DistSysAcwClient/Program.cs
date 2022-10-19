using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DistSysAcwClient
{
    #region Task 10 and beyond
    class Client
    {
        private static string key { get; set; }

        private static string username { get; set; }

        private static string publicKey { get; set; }

        private static RSACryptoServiceProvider rSA;

        static async Task Main(string[] args)
        {
            RSACryptoServiceProvider.UseMachineKeyStore = true;
            rSA = new RSACryptoServiceProvider();

            Console.WriteLine("Hello. What would you like to do?");
            while (true)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44394/");

                string input = Console.ReadLine();
                Console.Clear();
                switch (input.Split(' ')[0])
                {
                    case "TalkBack":
                        await RunTalkAsync(input, client);
                        break;
                    case "User":
                        await RunUserAsync(input, client);
                        break;
                    case "Protected":
                        await RunProtectedAsync(input, client);
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
                Console.WriteLine("What would you like to do next?");
            }

        }

        static async Task RunTalkAsync(string input, HttpClient client)
        {
            try
            {
                Task<string> task;

                switch (input.Split(' ')[1])
                {
                    case "Hello":
                        task = GetContentAsync("api/TalkBack/hello", client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "Sort":

                        string output = "";
                        string sort = input.Split(' ')[2].Trim(new char[2] { '[',']'});
                        foreach(string s in sort.Split(','))
                        {
                            output += "integers=" + s + "&";
                        }

                        task = GetContentAsync("api/TalkBack/sort?" + output, client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        static async Task RunUserAsync(string input, HttpClient client)
        {
            try
            {
                Task<string> task;

                switch (input.Split(' ')[1])
                {
                    case "Get":
                        task = GetContentAsync("api/user/new?username=" + input.Split(' ')[2], client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "Post":
                        task = PostUserAsync("api/user/new", input.Split(' ')[2], client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            if (task.Result.StartsWith("Oops"))
                            {
                                Console.WriteLine(task.Result);
                                break;
                            }
                            username = input.Split(' ')[2];
                            key = task.Result;
                            Console.WriteLine("Got API Key");
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;


                    case "Set":
                        username = input.Split(' ')[2];
                        key = input.Split(' ')[3];
                        Console.WriteLine("Stored");
                        break;

                    case "Delete":

                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }

                        task = DeleteUserAsync("api/user/removeuser?username=", client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "Role":

                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }

                        Message message = new Message { username = input.Split(' ')[2], role = input.Split(' ')[3] };
                        task = PostJsonAsync("api/user/changerole", message , client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        static async Task RunProtectedAsync(string input, HttpClient client)
        {
            try
            {
                Task<string> task;

                switch (input.Split(' ')[1])
                {
                    case "Hello":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        client.DefaultRequestHeaders.Add("ApiKey", key);
                        task = GetContentAsync("api/protected/hello", client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "SHA1":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        client.DefaultRequestHeaders.Add("ApiKey", key);
                        task = GetContentAsync("api/protected/sha1?message=" + input.Split(' ')[2], client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "SHA256":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        client.DefaultRequestHeaders.Add("ApiKey", key);
                        task = GetContentAsync("api/protected/sha256?message=" + input.Split(' ')[2], client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            Console.WriteLine(task.Result);
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "Get":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        if(input.Split(' ')[2] != "PublicKey")
                        {
                            Console.WriteLine("Invalid Get Command!");
                            break;
                        }
                        client.DefaultRequestHeaders.Add("ApiKey", key);
                        task = GetContentAsync("api/protected/getpublickey", client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            if (!task.Result.StartsWith("<RSAKeyValue>"))
                            {
                                Console.WriteLine("Couldn’t Get the Public Key");
                                break;
                            }
                            publicKey = task.Result;
                            Console.WriteLine("Got Public Key");
                            break;
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "Sign":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        if (string.IsNullOrEmpty(publicKey))
                        {
                            Console.WriteLine("Client doesn’t yet have the public key");
                            break;
                        }

                        client.DefaultRequestHeaders.Add("ApiKey", key);
                        task = GetContentAsync("api/protected/Sign?message=" + input.Split(' ')[2], client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            bool verify = VerifySign(input.Split(' ')[2], task.Result);
                            if (verify)
                            {
                                Console.WriteLine("Message was successfully signed");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Message was not successfully signed");
                                break;
                            }
                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    case "AddFifty":
                        if (string.IsNullOrEmpty(key))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                            break;
                        }
                        if (string.IsNullOrEmpty(publicKey))
                        {
                            Console.WriteLine("Client doesn’t yet have the public key");
                            break;
                        }
                        if (!int.TryParse(input.Split(' ')[2], out int outInt))
                        {
                            Console.WriteLine("A valid integer must be given!");
                            break;
                        }
                        client.DefaultRequestHeaders.Add("ApiKey", key);

                        AesCryptoServiceProvider myAes = new AesCryptoServiceProvider();
                        myAes.GenerateKey();
                        myAes.GenerateIV();

                        task = GetContentAsync("api/protected/addfifty?" + EncryptAddFifty(outInt, myAes.Key, myAes.IV), client);

                        if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                        {
                            if(task.Result == "Bad Request")
                            {
                                Console.WriteLine("“An error occurred!");
                                break;
                            }
                            string[] hexData = task.Result.Split('-');
                            byte[] byteData = new byte[hexData.Length];
                            for (int i = 0; i < hexData.Length; i++)
                            {
                                byteData[i] = Convert.ToByte(hexData[i], 16);
                            }

                            ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV);
                            string output;
                            using (MemoryStream msDecrypt = new MemoryStream(byteData))
                            {
                                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                    {
                                        output = srDecrypt.ReadToEnd();
                                    }
                                }
                            }
                            Console.WriteLine(output);
                            break;

                        }
                        Console.WriteLine("Request Timed Out");
                        break;

                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
        private static bool VerifySign(string message, string signedData)
        {
            rSA.FromXmlString(publicKey);
            byte[] byteMessage = Encoding.ASCII.GetBytes(message);
            string[] hexData = signedData.Split('-');
            byte[] byteData = new byte[hexData.Length];
            for(int i = 0; i < hexData.Length; i++)
            {
                byteData[i] = Convert.ToByte(hexData[i], 16);
            }
            return rSA.VerifyData(byteMessage, SHA1.Create(), byteData);
        }

        private static string EncryptAddFifty(int integer, byte[] Key, byte[] IV)
        {
            rSA.FromXmlString(publicKey);
            return "encryptedInteger=" + BitConverter.ToString(rSA.Encrypt(BitConverter.GetBytes(integer), false)) + "&encryptedSymKey=" +
                BitConverter.ToString(rSA.Encrypt(Key, false)) + "&encryptedIV=" + BitConverter.ToString(rSA.Encrypt(IV, false));
        }

        private static async Task<string> GetContentAsync(string uri, HttpClient client)
        {
            Console.WriteLine("...please wait...");
            var response = await client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> PostUserAsync(string uri, string user, HttpClient client)
        {
            Console.WriteLine("...please wait...");
            var response = await client.PostAsJsonAsync(uri, user);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> PostJsonAsync(string uri, Message message, HttpClient client)
        {
            Console.WriteLine("...please wait...");
            client.DefaultRequestHeaders.Add("ApiKey", key);
            var response = await client.PostAsJsonAsync(uri, message);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> DeleteUserAsync(string uri, HttpClient client)
        {
            Console.WriteLine("...please wait...");
            client.DefaultRequestHeaders.Add("ApiKey", key);
            var response = await client.DeleteAsync(uri + username); 
            return await response.Content.ReadAsStringAsync();
        }

    }
    #endregion
}
