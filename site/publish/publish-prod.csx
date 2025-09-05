#r "nuget: SSH.NET, 2020.0.1"

using Renci.SshNet;

Console.Clear();
var linha = "==========================================================================";
Console.WriteLine(linha);
Console.WriteLine("Publicação WEB para o ambiente de PRODUÇÃO");
Console.WriteLine(linha);

Console.Write("Senha do servidor: ");
var senha = Console.ReadLine();

var servidores = new string[] {
    "54.233.198.13"
};

var usuario = "gestia";
var caminhoLocal = "./dist";
var caminhoNoServidor = "/opt/app/precadastro_drhair/site";

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
        Console.WriteLine(linha);
        Console.WriteLine("Upload dos arquivos");

        sshClient.Connect();
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
            using (var uploadFileStream = System.IO.File.OpenRead(fullFileName))
            {
                var fileName = Path.GetFileName(fullFileName);
                Console.Write(string.Format("Uploading {0}/{1}...", currentFolder, fileName));
                sftp.UploadFile(uploadFileStream, fileName, true);
                Console.WriteLine(" OK!");
            }
        };

        try
        {
            sftp.DeleteDirectory(caminhoNoServidor);
        }
        catch { }
        ExecuteCommand(sshClient, string.Format("mkdir {0}", caminhoNoServidor), null);
        sftp.ChangeDirectory(caminhoNoServidor);
        currentFolder = caminhoNoServidor;
        ProcessDirectory(rootDirectory.FullName);
    }
}

Console.WriteLine("Fim do script...");

Console.ReadKey();