using System;
using framework;
using model;
using System.Collections.Generic;
using System.Linq;
using data;

namespace api
{
    public class WebApiOperationResolver : IOperationResolver
    {
        private readonly Repository<RotinaDoSistema> RotinaDoSistemaRepository;
        private static Dictionary<long, KeyValuePair<string, string>> openOperations = null;
        private static Dictionary<long, KeyValuePair<string, string>> restrictOperations = null;
        private static readonly object _lock = new object();

        public WebApiOperationResolver(Repository<RotinaDoSistema> rotinaDoSistemaRepository)
        {
            RotinaDoSistemaRepository = rotinaDoSistemaRepository;
        }

        public virtual IOperation Resolve(string httpMethod, string httpUrl)
        {
            IOperation result = null;
            var operation = OpenOperations.Where(openOperation => IsMatch(openOperation.Value,
                httpUrl, httpMethod)).FirstOrDefault();
            if (operation.Key > 0)
            {
                result = new WebApiOperation()
                {
                    Id = operation.Key,
                    Description = "Open operation",
                    RequiresAuthentication = false,
                    OpenOperation = true,
                };
            }
            else
            {
                operation = RestrictOperations.Where(restrictOperation => IsMatch(restrictOperation.Value, 
                    httpUrl, httpMethod)).FirstOrDefault();
                if (operation.Key > 0)
                {
                    result = RotinaDoSistemaRepository.Get(operation.Key);
                    if (result == null)
                    {
                        result = new WebApiOperation()
                        {
                            Id = operation.Key,
                            Description = operation.Key.ToString(),
                            RequiresAuthentication = true,
                            RestrictedOperation = true,
                        };
                    }
                }
            }

            if (result == null)
            {
                throw new Exception("Operação inválida.");
            }

            return result;
        }

        public virtual Dictionary<long, KeyValuePair<string, string>> OpenOperations
        {
            get
            {
                if (openOperations == null)
                {
                    lock (_lock)
                    {
                        openOperations = new Dictionary<long, KeyValuePair<string, string>>();
                        openOperations.Add(0001, new KeyValuePair<string, string>("/;/favicon.ico", "GET"));
                        openOperations.Add(0002, new KeyValuePair<string, string>("/defaulthub", "*"));
                        openOperations.Add(0003, new KeyValuePair<string, string>("/login/token", "POST"));
                        openOperations.Add(0004, new KeyValuePair<string, string>("/login/recover", "POST"));
                        openOperations.Add(0005, new KeyValuePair<string, string>("/publicfile;/publicfile/download*", "GET"));
                        openOperations.Add(0006, new KeyValuePair<string, string>("/publickey", "GET"));
                        openOperations.Add(0007, new KeyValuePair<string, string>("/login/recoverpassword", "POST"));
                        openOperations.Add(0008, new KeyValuePair<string, string>("/login/resetpassword", "POST"));
                        openOperations.Add(0009, new KeyValuePair<string, string>("/cidade", "GET"));
                        openOperations.Add(0010, new KeyValuePair<string, string>("/parametrodosistema/urlpublica", "GET"));
                        openOperations.Add(0011, new KeyValuePair<string, string>("/receiver/moko/status", "GET"));
                        openOperations.Add(0012, new KeyValuePair<string, string>("/receiver/moko", "POST"));
                        openOperations.Add(0013, new KeyValuePair<string, string>("/receiver/ab;/receiver/ab2", "POST"));
                        openOperations.Add(0014, new KeyValuePair<string, string>("/maintenance*", "GET"));
                        openOperations.Add(0015, new KeyValuePair<string, string>("/H1*", "GET"));
                        openOperations.Add(0016, new KeyValuePair<string, string>("/receiver/mikrotik*", "POST"));
                        openOperations.Add(0018, new KeyValuePair<string, string>("/ack", "GET"));
                        openOperations.Add(0019, new KeyValuePair<string, string>("/ack", "POST"));
                        openOperations.Add(0020, new KeyValuePair<string, string>("/", "HEAD"));
                        openOperations.Add(0021, new KeyValuePair<string, string>("/receiver/mobile", "POST"));
                        openOperations.Add(0022, new KeyValuePair<string, string>("/receiver/status", "GET"));
                        openOperations.Add(0023, new KeyValuePair<string, string>("/MKGW01BWPRO*", "GET"));
                    }
                }
                return openOperations;
            }
        }

        public virtual Dictionary<long, KeyValuePair<string, string>> RestrictOperations
        {
            get
            {
                if (restrictOperations == null)
                {
                    lock (_lock)
                    {
                        restrictOperations = new Dictionary<long, KeyValuePair<string, string>>();

                        // Operações acima de 90 mil não será verificado se o usuário logado tem ou não permissão, mas precisa ter um usuário logado.  
                        restrictOperations.Add(90001, new KeyValuePair<string, string>("/file*;/download/*;/info*;/base64*", "GET"));
                        restrictOperations.Add(90002, new KeyValuePair<string, string>("/file", "POST"));
                        restrictOperations.Add(90003, new KeyValuePair<string, string>("/login/aceitartermosdeuso", "POST"));
                        restrictOperations.Add(90004, new KeyValuePair<string, string>("/login/alteraridioma", "POST"));
                        restrictOperations.Add(90005, new KeyValuePair<string, string>("/login/termosdeuso", "GET"));
                        restrictOperations.Add(90006, new KeyValuePair<string, string>("/cidade", "GET"));
                        restrictOperations.Add(90007, new KeyValuePair<string, string>("/pais", "GET"));

                        // 

                        restrictOperations.Add(9001, new KeyValuePair<string, string>("/log", "GET"));

                        restrictOperations.Add(1011, new KeyValuePair<string, string>("/perfildeusuario;/perfildeusuario/fast", "GET"));
                        restrictOperations.Add(1012, new KeyValuePair<string, string>("/perfildeusuario", "POST"));
                        restrictOperations.Add(1013, new KeyValuePair<string, string>("/perfildeusuario", "PUT"));
                        restrictOperations.Add(1014, new KeyValuePair<string, string>("/perfildeusuario", "DELETE"));

                        restrictOperations.Add(1021, new KeyValuePair<string, string>("/usuarioadministrador;/usuarioadministrador/fast", "GET"));
                        restrictOperations.Add(1022, new KeyValuePair<string, string>("/usuarioadministrador", "POST"));
                        restrictOperations.Add(1023, new KeyValuePair<string, string>("/usuarioadministrador", "PUT"));
                        restrictOperations.Add(1024, new KeyValuePair<string, string>("/usuarioadministrador", "DELETE"));

                        restrictOperations.Add(1031, new KeyValuePair<string, string>("/rotinadosistema", "GET"));

                        restrictOperations.Add(1041, new KeyValuePair<string, string>("/parametrodosistema", "GET"));
                        restrictOperations.Add(1042, new KeyValuePair<string, string>("/parametrodosistema", "POST"));
                        restrictOperations.Add(1043, new KeyValuePair<string, string>("/parametrodosistema", "PUT"));
                        restrictOperations.Add(1044, new KeyValuePair<string, string>("/parametrodosistema", "DELETE"));

                        restrictOperations.Add(1051, new KeyValuePair<string, string>("/lead", "GET"));
                        restrictOperations.Add(1052, new KeyValuePair<string, string>("/lead", "POST"));
                        restrictOperations.Add(1053, new KeyValuePair<string, string>("/lead", "PUT"));
                        restrictOperations.Add(1054, new KeyValuePair<string, string>("/lead", "DELETE"));

                        restrictOperations.Add(1381, new KeyValuePair<string, string>("/termodeuso", "GET"));
                        restrictOperations.Add(1382, new KeyValuePair<string, string>("/termodeuso", "POST"));
                        restrictOperations.Add(1383, new KeyValuePair<string, string>("/termodeuso", "PUT"));

                        restrictOperations.Add(1501, new KeyValuePair<string, string>("/auditoria;/auditoria/acoes", "GET"));
                    }
                }
                return restrictOperations;
            }
        }

        private bool IsMatch(KeyValuePair<string, string> operation, string module, string action)
        {
            var result = false;
            var keys = operation.Key.Trim().Split(';');
            foreach (var iKey in keys)
            {
                var key = iKey.Trim();
                if ((operation.Value == action) || (operation.Value == "*"))
                {
                    if ((key == module))
                    {
                        result = true;
                        break;
                    }
                    else
                    if (key.EndsWith('*') && module.StartsWith(key.Substring(0, key.Length - 1)))
                    {
                        var x = module.Replace(key.Replace("*", ""), "");
                        result = (x.StartsWith("/") || x.StartsWith("?") || (string.IsNullOrWhiteSpace(x)));
                        if (result)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }

    internal class WebApiOperation : IOperation
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool RequiresAuthentication { get; set; }
        public bool OpenOperation { get; set; }
        public bool RestrictedOperation { get; set; }
    }
}
