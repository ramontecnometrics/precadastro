#! netcoreapp3.1
#r "nuget: SSH.NET, 2020.0.1"

using Renci.SshNet;

/* ====================  DEFINIR OS PARÂMETROS AQUI ========================= */
var servidores = new string[] {
    "54.233.198.13"
};
var usuario = "gestia";
var caminhoLocal = "./../setup/bin/release/net8.0/publish";
var caminhoNoServidor = "/opt/app/precadastro_drhair/setup";
/* ========================================================================== */

Console.Clear();
var linha = "==========================================================================";
Console.WriteLine(linha);
Console.WriteLine("Publicação do SETUP para o ambiente de HOMOLOGAÇÃO");
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

        Console.Clear();
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
                var remoteDirectory = string.Format("{0}{1}", caminhoNoServidor, RemoveDirectoryBasePath(subDirectory)).Replace("\\", "/");
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

        sftp.ChangeDirectory(caminhoNoServidor);
        currentFolder = caminhoNoServidor;
        ProcessDirectory(rootDirectory.FullName);

        sftp.Disconnect();
        sshClient.Disconnect();
    }
}

Console.WriteLine("Fim do script...");

Console.ReadKey();