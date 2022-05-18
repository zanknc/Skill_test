using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RISTExamOnlineProject.Models.db;
using RISTExamOnlineProject.Models.TSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RISTExamOnlineProject.api
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class GetOperatorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SPTODbContext _sptoDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;


        public GetOperatorController(SPTODbContext context, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {

            _sptoDbContext = context;
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;

        }
        public DataTable Get()
        {
            DataTable dt = new DataTable();
            string Strsql = "SELECT * from OperatorOnline";
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = ObjRun.GetDatatables(Strsql);
            return dt;
        }

    }
}
