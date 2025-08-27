#r "nuget: SSH.NET, 2024.1.0"

using Renci.SshNet;
using System;

/* ====================  DEFINIR OS PARÂMETROS AQUI ========================= */
var servidores = new string[] {
    "177.71.184.27",
};
var usuario = "tecnometrics";
var nomeDoServicoNoServidor = "tecnometrics_service.service";
/* ========================================================================== */

Console.Clear();
var linha = "==========================================================================";
Console.WriteLine(linha);
Console.WriteLine("Reniciar o SERVIÇO no ambiente de PRODUÇÃO");
Console.WriteLine(linha);

Console.WriteLine($"Usuário: {usuario}");
Console.Write("Senha: ");
var senha = Console.ReadLine();


foreach (var servidor in servidores)
{
    Console.WriteLine();
    Console.WriteLine($"Servidor: {servidor}");
    Console.WriteLine();

    ConnectionInfo ConnNfo = new ConnectionInfo(servidor, 22, usuario,
        new AuthenticationMethod[] {
        new PasswordAuthenticationMethod(usuario, senha)
        }
    );
    
    Action<SshClient, string, Action<string>> ExecuteCommand = (sshclient, command, onReadResponse) =>
    {
        using (var sshCommand = sshclient.CreateCommand(command))
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            try
            {
                Console.WriteLine(string.Format("{0}@{1}$ {2}", usuario, servidor, command));
                var result = sshCommand.Execute();
                if (!string.IsNullOrEmpty(sshCommand.Result))
                {
                    result = sshCommand.Result;
                }
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine(result);
                }
                if (onReadResponse != null)
                {
                    onReadResponse(result);
                }
            }
            finally
            {
                Console.ForegroundColor = oldColor;
            }
        }
    };

    using (var sshClient = new SshClient(ConnNfo))
    using (var sftp = new SftpClient(ConnNfo))
    {
        sshClient.Connect();

        Console.WriteLine(linha);
        Console.WriteLine("Parando o serviço...");
        ExecuteCommand(sshClient, string.Format("sudo systemctl stop {0}", nomeDoServicoNoServidor), (response) => { });

        ExecuteCommand(sshClient, string.Format("sudo systemctl status {0}", nomeDoServicoNoServidor),
            (response) =>
            {
                Console.WriteLine(response);
                if ((string.IsNullOrEmpty(response) || (!(response.Contains("Active: inactive (dead)") || response.Contains("Active: failed")))))
                {
                    throw new Exception("Erro ao parar o serviço.");
                }
            }
        );        

        Console.WriteLine(linha);
        Console.WriteLine("Iniciando o serviço...");
        ExecuteCommand(sshClient, string.Format("sudo systemctl start {0}", nomeDoServicoNoServidor), null);
        ExecuteCommand(sshClient, string.Format("sudo systemctl status {0}", nomeDoServicoNoServidor),
            (response) =>
            {
                Console.WriteLine(response);
                if ((string.IsNullOrEmpty(response) || (!response.Contains("Active: active (running)"))))
                {
                    throw new Exception("Erro ao iniciar o serviço.");
                }
            }
        );        

        sshClient.Disconnect();
    }
}

Console.WriteLine("Fim do script...");

Console.ReadKey();