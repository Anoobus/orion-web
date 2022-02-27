using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Employees;
using orion.web.TimeEntries;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    

    

    
   
   
  
  
    
    [Authorize]
    [Route("orion-api/v1/spikes/")]
    [ApiController]
    public class SpikeController : ControllerBase
    {
        public const string PPE_MISSING_TIME_TASK = "on-ppe-missing-time-email";
        public const string NONPPE_MISSING_TIME_TASK = "off-ppe-missing-time-email";
        private readonly IConfiguration _configuration;

        public SpikeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("get-db-schema/tables/{table}")]
        public async Task<ActionResult> GetTableSchema(string table, [FromQuery] bool addMore)
        {
            if (addMore)
            {
                var bldr = await new TestRDBDataBuilder(_configuration)
               .AddTable("Employees", cfg => cfg.AddSpecialColumn("UserRoleId").ThatUsesCustomGenerator(x => 2))
               .AddTable("TaskCategories")
               .AddTable("JobTasks", cfg => cfg.AddSpecialColumn("UsageStatusId").ThatMustBeValueOnOtherTable("UsageStatuses", "UsageStatusId")
                                               .AddSpecialColumn("TaskCategoryId").ThatMustBeValueOnOtherTable("TaskCategories", "TaskCategoryId"))
                .AddTable("UsageStatuses")
               .AddTable("TimeEntries", cfg => cfg.AddSpecialColumn("EmployeeId").ThatMustBeValueOnOtherTable("Employees", "EmployeeId")
                                                   .AddSpecialColumn("JobId").ThatMustBeValueOnOtherTable("Jobs", "JobId")
                                                   .AddSpecialColumn("JobTaskId").ThatMustBeValueOnOtherTable("JobTasks", "JobTaskId"))
               .PrepareForDataLoad();

                await bldr.UploadTestData(20);
            }




            using (var conn = new SqlConnection(_configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                cmd.CommandText = "select * from [dbo].[" + table + "]";
                cmd.CommandType = System.Data.CommandType.Text;
                var hmm = new DataTable("wtf");
                hmm.Load(await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SingleResult));



                return Ok(new
                {
                    columns = hmm.Columns.OfType<DataColumn>()
                            .Select(x => new
                            {
                                colName = x.ColumnName,
                                typeName = x.DataType.Name,
                                nullable = x.AllowDBNull,
                                AutoIncrement = x.AutoIncrement,
                                Unique = x.Unique
                            }).ToList(),
                    testData = hmm.Rows.OfType<DataRow>().Select(x => x.ItemArray).ToList()
                });

            }

        }
    }
}
