using DistSysAcw.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DistSysAcw.Controllers
{
    public class ProtectedController : BaseController
    {
        public ProtectedController(Models.UserContext dbcontext) : base(dbcontext) { }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ActionName("Hello")]
        public IActionResult GetHello()
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /Protected/Hello");
            User user = UserDatabaseAccess.GetUser(key);
            if(user == null)
            {
                return BadRequest("ApiKey not Valid");
            }
            return Ok("Hello " + user.UserName);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ActionName("sha1")]
        public IActionResult GetSHA1([FromQuery] string message)
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /Protected/SHA1");
            if (message == null)
            {
                return BadRequest("Bad Request");
            }

            byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] sha1ByteMessage;
            SHA1 sha1Provider = new SHA1CryptoServiceProvider();
            sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage);

            StringBuilder hex = new StringBuilder(sha1ByteMessage.Length * 2);
            foreach(byte b in sha1ByteMessage)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return Ok(hex.ToString());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ActionName("sha256")]
        public IActionResult GetSHA256([FromQuery] string message)
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /Protected/SHA256");
            if (message == null)
            {
                return BadRequest("Bad Request");
            }

            byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] sha256ByteMessage;
            SHA256 sha256Provider = new SHA256CryptoServiceProvider();
            sha256ByteMessage = sha256Provider.ComputeHash(asciiByteMessage);

            StringBuilder hex = new StringBuilder(sha256ByteMessage.Length * 2);
            foreach (byte b in sha256ByteMessage)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return Ok(hex.ToString());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ActionName("getpublickey")]
        public IActionResult GetPublicKey()
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /Protected/GetPublicKey");
            return Ok(StoreKey.GetPublicKey());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [ActionName("sign")]
        public IActionResult GetSign([FromQuery]string message)
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /Protected/Sign");
            return Ok(StoreKey.Sign(message));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ActionName("addfifty")]
        public IActionResult GetAddFifty([FromQuery] string encryptedInteger, string encryptedSymKey, string encryptedIV)
        {
            try
            {
                var key = Request.Headers["ApiKey"].ToString();
                UserDatabaseAccess.CreateLog(key, "User requested /Protected/AddFifty");

                byte[] decryptedInt = StoreKey.decryptBytes(encryptedInteger);
                byte[] decryptedSymKey = StoreKey.decryptBytes(encryptedSymKey);
                byte[] decryptedIV = StoreKey.decryptBytes(encryptedIV);
                int intAddFifty = BitConverter.ToInt32(decryptedInt) + 50;

                var myAes = new AesCryptoServiceProvider();
                myAes.Key = decryptedSymKey;
                myAes.IV = decryptedIV;

                ICryptoTransform encryptor = myAes.CreateEncryptor();
                byte[] encryptedIntOut;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(intAddFifty);
                        }
                        encryptedIntOut = ms.ToArray();
                    }
                }
                return Ok(BitConverter.ToString(encryptedIntOut));
            }
            catch
            {
                return BadRequest("Bad Request");
            }
        }
    }
}
