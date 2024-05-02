using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jrs.DBContexts;
using jrs.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IRepository _rep;
        public TestController(IRepository rep) {
            this._rep = rep;
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        public IEnumerable<Test> Get()
        {
            return _rep.GetTests().ToList();
        }

        [HttpPost]
        [Route("[action]")]
        public IEnumerable<Test> GetFromSP(int id)
        {
            //return _rep.GetTestsFromSp(id).ToList();
            return null;
        }
    }
}