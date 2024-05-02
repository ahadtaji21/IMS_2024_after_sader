﻿using jrs.DBContexts;
using jrs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class DonorContactController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public DonorContactController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
   
    
    }
}
