using System;
using System.Linq;
using data;
using framework;

namespace model.Repositories
{ 
   public class DateTimeRepository
    {
        private readonly ISqlCommand SqlCommand;

        public DateTimeRepository(ISqlCommand sqlCommand)
        {
            SqlCommand = sqlCommand;
        }

        public virtual void Sincronizar()
        {
            try
            {
                var command = new CommandData();
                command.Sql = @"select now() as dataatual";
                var records = SqlCommand.Execute(command);
                var record = records.FirstOrDefault();
                var dataAtual = record.GetFieldValue<DateTime>("dataatual");
                DateTimeSync.SetNow(dataAtual);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
