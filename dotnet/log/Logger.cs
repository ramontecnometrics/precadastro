using log.Data;
using log.NHibernate;
using System; 
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace log
{
    public static class Logger
    {
        private static LogSetup _logSetup = null;
        static readonly object Obj = new Object();

        public static void SetupLog(LogSetup logSetup)
        {
            lock (Obj)
            {
                _logSetup = logSetup;
                try 
                {
                    SaveParamsToDatabase(_logSetup);
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static void SaveParamsToDatabase(LogSetup logSetup)
        {
            DispatchUnitOfWorkScope((scope) =>
            {
                var uowCategory = new UnitOfWorkForHN<Category>(scope);
                foreach (var i in logSetup.AvailableCategories)
                {
                    var category = uowCategory.Get(i.Id);
                    if (category == null)
                    {
                        uowCategory.Save(i);
                    }
                    else
                    {
                        category.Description = i.Description;
                        uowCategory.Save(category);
                    }
                }

                var uowSubCategory = new UnitOfWorkForHN<SubCategory>(scope);
                foreach (var i in logSetup.AvailableSubCategories)
                {
                    var subcategory = uowSubCategory.Get(i.Id);
                    if (subcategory == null)
                    {
                        uowSubCategory.Save(i);
                    }
                    else
                    {
                        subcategory.Description = i.Description;
                        subcategory.Category = i.Category;
                        uowSubCategory.Save(subcategory);
                    }
                }
                return true;
            });
        }

        public static void Add(string module, string action, string queryParams, Exception message, int categoryId,
            int? subcategoryId, string groupId, string httpMethod, IPAddress iPAddress, DateTime startTime, int? httpResponseCode,
            int? userId)
        {
            Add(module, action, queryParams, GetExceptionInfo(message), categoryId, subcategoryId,
                groupId, httpMethod, iPAddress.ToString(), startTime, httpResponseCode, userId);
        }

        public static void Add(string module, string action, string queryParams, string message, int categoryId,
            int? subcategoryId, string groupId, string httpMethod, IPAddress iPAddress, DateTime startTime,int? httpResponseCode,
            int? userId)
        {
            Add(module, action, queryParams, message, categoryId, subcategoryId,
                groupId, httpMethod, iPAddress.ToString(), startTime, httpResponseCode, userId);
        }

        public static void Add(string module, string action, string queryParams, string message, int categoryId,
            int? subcategoryId, string groupId, string httpMethod, string clientAddress, DateTime startTime, int? httpResponseCode,
            int? userId)
        {
            if ((_logSetup != null) && (_logSetup.Enabled))
            {
                Task.Run(() =>
                {
                    DispatchUnitOfWorkScope((scope) =>
                    {
                        var uow = new UnitOfWorkForHN<LogData>(scope);
                        var data = new LogData()
                        {
                            Module = module,
                            Action = action,
                            QueryParams = queryParams,
                            Message = string.IsNullOrWhiteSpace(message) ? null : message.Replace("\0", string.Empty),
                            Category = _logSetup.AvailableCategories.Where(i => i.Id == categoryId).FirstOrDefault(),
                            SubCategory = _logSetup.AvailableSubCategories.Where(i => i.Id == subcategoryId).FirstOrDefault(),
                            GroupId = groupId,
                            Date = DateTime.Now,
                            HostName = _logSetup.HostName,
                            HttpMethod = httpMethod,
                            HttpResponseCode = httpResponseCode,
                            ClientAddress = clientAddress,
                            UserId = userId,
                            StartTime = startTime
                        };
                        try
                        {
                            uow.Save(data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                       
                        return true;
                    });
                });
            }
        }

        public static void GetAll(Action<IQueryable<LogData>> action)
        {
          var unitOfWorkScopeParams = new UnitOfWorkScopeParams()
            {
                ConnectionString = _logSetup.ConnectionString,
            };
            using (var unitOfWorkScope = new UnitOfWorkScopeForNH(unitOfWorkScopeParams))
            {
                var uow = new UnitOfWorkForHN<LogData>(unitOfWorkScope);
                action(uow.GetAll());
            };
        }

        public static void DispatchUnitOfWorkScope(Func<UnitOfWorkScope, bool> func)
        {
            var unitOfWorkScopeParams = new UnitOfWorkScopeParams()
            {
                ConnectionString = _logSetup.ConnectionString,
            };

            using (var unitOfWorkScope = new UnitOfWorkScopeForNH(unitOfWorkScopeParams))
            {
                unitOfWorkScope.StartTransaction();
                func(unitOfWorkScope);
                unitOfWorkScope.Commit();
            };
        }

        private static string GetExceptionInfo(Exception e)
        {
            var result =
                string.Format("Message: {0}\nStack trace: {1}\n", e.Message, e.StackTrace);
            if (e.InnerException != null)
            {
                result = string.Format("{0}\nInner exception:{1}", result, e.InnerException.Message);
            }
            return result;
        }

        public static void Delete(string whereClause)
        {
            var unitOfWorkScopeParams = new UnitOfWorkScopeParams()
            {
                ConnectionString = _logSetup.ConnectionString,
            };
            using (var unitOfWorkScope = new UnitOfWorkScopeForNH(unitOfWorkScopeParams))
            {
                var sqlcommand = new NHCommand(unitOfWorkScope);
                sqlcommand.Execute(new CommandData(){
                    Sql = $"delete from logdata where {whereClause}"
                });
            };
        }
    }
}
