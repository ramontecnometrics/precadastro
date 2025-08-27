#r "nuget: SSH.NET, 2024.1.0"

using Renci.SshNet;

var args = Environment.GetCommandLineArgs();

/* ====================  DEFINIR OS PARÂMETROS AQUI ========================= */
var servidores = new string[] {
    "177.71.184.27"
};

var usuario = "tecnometrics";
var caminhoLocal = "./bin/release/net8.0/publish";
var caminhoNoServidor = "/opt/app/tecnometrics/api";
var caminhoTemporarioNoServidor = "/opt/app/tecnometrics/api-temp";
var nomeDoServicoNoServidor = "tecnometrics.service";
/* ========================================================================== */

Console.Clear();
var linha = "==========================================================================";
Console.WriteLine(linha);
Console.WriteLine("Publicação da API para o ambiente de PRODUÇÃO");
Console.WriteLine(linha);

Console.WriteLine($"Usuário: {usuario}");
Console.Write("Senha: ");
var senha = Console.ReadLine();

foreach (var servidor in servidores)
{
    Console.WriteLine();
    Console.WriteLine($"Servidor: {servidor}");
    Console.WriteLine();

    var filesToDelete = new string[] {
        "appsettings.json", "cert.pfx"
    };

    foreach (var file in filesToDelete)
    {
        var filePath = Path.Combine(caminhoLocal, file);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"Apagando arquivo local: {filePath}");
        }
    }

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

    try
    {
        using (var sshClient = new SshClient(ConnNfo))
        using (var sftp = new SftpClient(ConnNfo))
        {
            ConnNfo.Timeout = TimeSpan.FromSeconds(10);

            sshClient.Connect();

            Console.WriteLine(linha);
           
            Console.WriteLine("Upload dos arquivos");
            sftp.Connect();

            Console.WriteLine();
            var rootDirectory = new DirectoryInfo(caminhoLocal);
            Action<string> ProcessDirectory = null;
            Action<string> ProcessFile = null;

            Func<string, string> RemoveDirectoryBasePath = (directory) =>
            {
                var directoryInfo = new DirectoryInfo(directory);
                return directoryInfo.FullName.Replace(rootDirectory.FullName, "");
            };

            string currentFolder = null;

            ProcessDirectory = (targetDirectory) =>
            {
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                {
                    ProcessFile(fileName);
                }

                string[] subDirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subDirectory in subDirectoryEntries)
                {
                    var remoteDirectory = string.Format("{0}{1}", caminhoTemporarioNoServidor, RemoveDirectoryBasePath(subDirectory)).Replace("\\", "/");
                    ExecuteCommand(sshClient, string.Format("mkdir {0}", remoteDirectory), null);
                    sftp.ChangeDirectory(remoteDirectory);
                    Console.WriteLine(string.Format("Change directory {0}", remoteDirectory));
                    currentFolder = remoteDirectory;
                    ProcessDirectory(subDirectory);
                }
            };

            ProcessFile = (fullFileName) =>
            {
                var fileName = Path.GetFileName(fullFileName);
                // monta o caminho completo no servidor
                var remoteFilePath = $"{currentFolder}/{fileName}";

                // pega a data de última modificação local em UTC
                var localLastWriteUtc = File.GetLastWriteTimeUtc(fullFileName);

                bool shouldUpload = true;

                // verifica se já existe um arquivo remoto
                if (sftp.Exists(remoteFilePath))
                {
                    // obtém atributos do arquivo remoto
                    var remoteAttrs = sftp.GetAttributes(remoteFilePath);
                    // data de modificação remota em UTC
                    var remoteLastWriteUtc = remoteAttrs.LastWriteTime.ToUniversalTime();

                    if (localLastWriteUtc <= remoteLastWriteUtc)
                    {
                        Console.WriteLine($"Pulando {remoteFilePath}: remoto ({remoteLastWriteUtc:yyyy-MM-dd HH:mm:ss}) é mais novo ou igual ao local ({localLastWriteUtc:yyyy-MM-dd HH:mm:ss}).");
                        shouldUpload = false;
                    }
                }

                if (shouldUpload)
                {
                    using var uploadStream = File.OpenRead(fullFileName);
                    Console.Write($"Enviando {remoteFilePath}… ");
                    // envia para o caminho completo, sobrescrevendo se existir
                    sftp.UploadFile(uploadStream, remoteFilePath, true);
                    Console.WriteLine("OK!");
                }
            };

            sftp.ChangeDirectory(caminhoTemporarioNoServidor);
            currentFolder = caminhoTemporarioNoServidor;

            // FAZ O UPLOAD AQUI
            ProcessDirectory(rootDirectory.FullName);


            // PARA O SERVIÇO

            Console.WriteLine(linha);

            var servicoEstaRodando = false;

            Console.WriteLine("Verificanto o status o serviço...");
            ExecuteCommand(sshClient, string.Format("sudo systemctl status {0}", nomeDoServicoNoServidor),
            (response) =>
            {
                Console.WriteLine(response);
                if ((string.IsNullOrEmpty(response) || (response.Contains("Active: active (running)"))))
                {
                    servicoEstaRodando = true;
                }
            });

            if (servicoEstaRodando)
            {
                Console.WriteLine("Parando o serviço...");
                ExecuteCommand(sshClient, string.Format("sudo systemctl stop {0}", nomeDoServicoNoServidor),
                    (response) => { });
                ExecuteCommand(sshClient, string.Format("sudo systemctl status {0}", nomeDoServicoNoServidor),
                    (response) =>
                    {
                        Console.WriteLine(response);
                        if ((string.IsNullOrEmpty(response) || (!(response.Contains("Active: inactive (dead)") || response.Contains("Active: failed")))))
                        {
                            throw new Exception("Erro ao parar o serviço.");
                        }
                    });
            }

            // COPIA PARA A PASTA OFICIAL

            ExecuteCommand(sshClient, string.Format("cp -rf {0}/* {1}", caminhoTemporarioNoServidor, caminhoNoServidor), null);

            sftp.Disconnect();

            // INICIA O SERVIÇO

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
             });

            sshClient.Disconnect();
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}

Console.WriteLine("Fim do script...");

Console.ReadKey();