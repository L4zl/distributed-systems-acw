using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcw.Controllers
{
    public class TalkbackController : BaseController
    {
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext) { }

        [ActionName("Hello")]
        public IActionResult Get()
        {
            #region TASK1
            return Ok("Hello World");
            #endregion
        }

        [ActionName("Sort")]
            #region TASK1
        public IActionResult Get([FromQuery]int[] integers)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Response");
            }
            else
            {
                Array.Sort(integers);
                return Ok(integers);
            }
        }
            #endregion
    }
}
