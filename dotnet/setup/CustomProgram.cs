using System;
using System.Collections.Generic;
using System.Linq;

namespace setup
{
    public abstract class CustomProgram
    {
        private IDictionary<string, Func<bool>> _opcoes = new Dictionary<string, Func<bool>>();
        private bool _finalizado = false;
        private readonly string _linhas = "-----------------------------------------------------------";

        protected static void Execute<T>()
        {
            Activator.CreateInstance<T>();
        }

        public CustomProgram()
        {
            RegistrarOpcoes();
            while (!_finalizado)
            {
                MostrarOpcoes();
                var opcao = Console.ReadLine();
                ExecutarOpcao(opcao);
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Realize aqui as chamadas ao método AdicionarOpcao()
        /// </summary>        
        protected abstract void RegistrarOpcoes();

        protected virtual void Clear()
        {
            Console.Clear();
        }

        protected virtual void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        protected virtual void Write(string line)
        {
            Console.Write(line);
        }

        protected virtual void WriteLine()
        {
            Console.WriteLine(string.Empty);
        }

        protected virtual void Esperar()
        {
            Console.Write("pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        protected virtual string ReadLine()
        {
            return Console.ReadLine();
        }

        protected virtual void ExecutarOpcao(string opcao)
        {
            if (_opcoes.ContainsKey(opcao))
            {
                try
                {
                    _opcoes[opcao]();
                }
                catch (Exception e)
                {

                    WriteLine("============================================================================================================================");

                    if (e.InnerException != null)
                    {
                        if (e.InnerException.InnerException != null)
                        {
                            WriteLine(string.Format("Exception: {0}", e.InnerException.InnerException.Message));
                        }
                        WriteLine(string.Format("Exception: {0}", e.InnerException.Message));
                    }

                    WriteLine(string.Format("Exception: {0}", e.Message));

                    WriteLine("============================================================================================================================");

                    WriteLine(e.ToString());

                    WriteLine("============================================================================================================================");

                    Esperar();
                }
            }
            else
            {
                WriteLine("Opção inválida.");
                Esperar();
            }
        }

        protected virtual void MostrarOpcoes()
        {
            Clear();
            WriteLine("Digite uma das opções a seguir");
            WriteLine(_linhas);
            foreach (var opcao in _opcoes)
            {
                WriteLine(opcao.Key);
            }
            WriteLine(_linhas);
        }

        protected virtual void AdicionarOpcao(string opcao, Func<bool> metodo)
        {
            _opcoes.Add(opcao, metodo);
        }

        protected virtual void LimparOpcoes()
        {
            _opcoes.Clear();
        }

        protected virtual bool Sair()
        {
            _finalizado = true;
            return true;
        }

        protected virtual T ObterValor<T>(string titulo, Action<T> validar = null)
        {
            var result = default(T);
            var valorValido = false;
            do
            {
                var ehBoolean = typeof(T) == typeof(bool);
                if (ehBoolean)
                {
                    Write(string.Format("{0} [S/N] ", titulo));
                }
                else
                {
                    Write(string.Format("{0} ", titulo));
                }

                var value = ReadLine();
                try
                {
                    if (ehBoolean)
                    {
                        if (!(new string[] { "S", "N", "s", "n" }).Contains(value))
                        {
                            throw new InvalidCastException();
                        }
                        value = (new string[] { "S", "s" }).Contains(value) ? "true" : "false";
                    }
                    result = ConverterValor<T>(value);
                    valorValido = true;
                    if ((valorValido) && (validar != null))
                    {
                        valorValido = false;
                        validar(result);
                        valorValido = true;
                    }
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }

            } while (!valorValido);
            return result;
        }

        protected virtual T ConverterValor<T>(string value)
        {
            try
            {
                var result = GetValue<T>(value);
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0} não é um {1} válido.", value, typeof(T).Name));
            }
        }

        public static T GetValue<T>(string value)
        {
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return (value == null || string.IsNullOrWhiteSpace(value) || DBNull.Value.Equals(value)) ?
               default(T) : (T)Convert.ChangeType(value, t);
        }
    }
}
