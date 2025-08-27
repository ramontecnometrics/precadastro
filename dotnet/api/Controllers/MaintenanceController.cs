using data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("maintenance/[action]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly ISqlCommand SqlCommand;

        public MaintenanceController(ISqlCommand sqlCommand)
        {
            SqlCommand = sqlCommand;
        }

        [HttpGet]
        [ActionName("health")]
        public string Health()
        {
            var result = "OK";
            return result;
        }

        [HttpGet]
        [ActionName("workingstatus")]
        public WorkingStatusResponse[] WorkingStatus()
        {
            var result = new List<WorkingStatusResponse>();

            var cmd = new CommandData();
            cmd.Sql = @"
select * from (
   select t.data,
          'Data da última alteração de local' as descricao
     from alteracaodelocal t 
    order by id desc 
   limit 1
 ) as t1
where t1.data < now() - interval '180' minute

union all

select * from (
  select t.datadeinclusao as data, 
         'Data da última alteração de temperatura' as descricao
    from afericaodetemperatura t 
   order by id desc 
   limit 1
  ) as t2
where t2.data < now() - interval '60' minute";            

            var records = SqlCommand.Execute(cmd);

            foreach(var record in records)
            {
                result.Add(new WorkingStatusResponse()
                {
                    Data = record.GetFieldValue<DateTime>("data").ToString(),
                    Descricao = record.GetFieldValue<string>("descricao"),
                });
            }
            return result.ToArray();
        }
    }

    public class WorkingStatusResponse
    {
        public string Data { get; set; }
        public string Descricao { get; set; }
    }
}