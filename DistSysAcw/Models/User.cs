using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcw.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public User() 
        {
            Logs = new HashSet<Log>();
        }

        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging

    public class Log
    {
        [Key]
        public string LogID { get; set; }
        public string LogString { get; set; }
        public string LogDateTime { get; set; }
        public Log() { }
    }

    public class ArchivedLog
    {
        [Key]
        public string LogID { get; set; }
        public string LogString { get; set; }
        public string LogDateTime { get; set; }
        public string ApiKey { get; set; }
        public ArchivedLog() { }
    }

    #endregion

    public static class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 

        public static User CreateUser(string username)
        {
            Guid guid = Guid.NewGuid();
            string role = "User";

            using (var ctx = new UserContext())
            {
                if(ctx.Users.Count() == 0)
                {
                    role = "Admin";
                }
                User user = new User()
                {
                    ApiKey = guid.ToString(),
                    UserName = username,
                    Role = role
                };
                ctx.Users.Add(user);
                ctx.SaveChanges();
                return user;
            }
        }

        public static void CreateLog(string ApiKey, string message)
        {
            Guid guid = Guid.NewGuid();
            using (var ctx = new UserContext())
            {
                Log log = new Log()
                {
                    LogID = guid.ToString(),
                    LogString = message,
                    LogDateTime = DateTime.Now.ToString()
                };
                User user = ctx.Users.Find(ApiKey);
                user.Logs.Add(log);
                ctx.Logs.Add(log);
                ctx.SaveChanges();
            }
        }

        public static bool CheckKey(string key)
        {
            bool found = false;
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.Find(key);
                if(user != null)
                {
                    found = true;
                }
                ctx.SaveChanges();
            }
            return found;
        }

        public static bool CheckUser(string username)
        {
            bool found = false;
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.FirstOrDefault(user => user.UserName == username);
                if (user != null)
                {
                    found = true;
                }
                ctx.SaveChanges();
            }
            return found;
        }


        public static bool CheckKeyUser(string key, string userName)
        {
            bool found = false;
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.Find(key);
                if ((user != null) && (user.UserName == userName))
                {
                    found = true;
                }
                ctx.SaveChanges();
            }
            return found;
        }


        public static User GetUser(string key)
        {
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.Find(key);
                ctx.SaveChanges();
                return user;
            }
        }

        public static void DeleteUser(string key)
        {
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.Find(key);

                if (user != null)
                {
                    foreach (Log l in user.Logs)
                    {
                        ArchivedLog log = new ArchivedLog()
                        {
                            LogID = l.LogID,
                            LogString = l.LogString,
                            LogDateTime = l.LogDateTime,
                            ApiKey = user.ApiKey
                        };
                        ctx.ArchivedLogs.Add(log);
                        ctx.Logs.Remove(l);
                    }
                    ctx.Users.Remove(user);
                }
                ctx.SaveChanges();
            }
        }


        public static bool ChangeRole(string username, string role)
        {
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.FirstOrDefault(user => user.UserName == username);
                if(user == null)
                {
                    return false;
                }
                user.Role = role;
                ctx.SaveChanges();
                return true;
            }
        }
        #endregion
    }


}