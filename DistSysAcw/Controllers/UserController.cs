using DistSysAcw.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysAcw.Controllers
{
    public class UserController : BaseController
    {
        public UserController(Models.UserContext dbcontext) : base(dbcontext) { }

        [HttpGet] [ActionName("new")]
        public IActionResult Get([FromQuery] string username)
        {
            string response = "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
            if (UserDatabaseAccess.CheckUser(username))
            {
                response = "True - User Does Exist! Did you mean to do a POST to create a new user?";
            }
            return Ok(response);

        }

        [HttpPost] [ActionName("new")]
        public IActionResult Post([FromBody] string username)
        {
            string response = "Oops. This username is already in use. Please try again with a new username.";
            if (username == null)
            {
                response = "Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json";
                return BadRequest(response);
            }
            else if (UserDatabaseAccess.CheckUser(username))
            {
                return Forbid(response);
            }

            User user = UserDatabaseAccess.CreateUser(username);
            response = user.ApiKey;
            return Ok(response);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        [ActionName("removeuser")]
        public IActionResult Delete([FromQuery]string username)
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /User/RemoveUser");
            if(UserDatabaseAccess.CheckKeyUser(key, username))
            {
                UserDatabaseAccess.DeleteUser(key);
                return Ok(true);
            }
            return Ok(false);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("changerole")]
        public IActionResult Post([FromBody]JsonElement jobject)
        {
            var key = Request.Headers["ApiKey"].ToString();
            UserDatabaseAccess.CreateLog(key, "User requested /User/ChangeRole");
            try
            {
                string username = jobject.GetProperty("username").GetString();
                string role = jobject.GetProperty("role").GetString();
                if (!UserDatabaseAccess.CheckUser(username))
                {
                    return BadRequest("NOT DONE: Username does not exist");
                }
                if (role != "Admin" && role != "User")
                {
                    return BadRequest("NOT DONE: Role does not exist");
                }
                bool changed = UserDatabaseAccess.ChangeRole(username, role);
                if (changed == false)
                {
                    return BadRequest("NOT DONE: An error occured");
                }
                return Ok("DONE");
            }
            catch
            {
                return BadRequest("NOT DONE: An error occured");
            }
        }

    }
}
