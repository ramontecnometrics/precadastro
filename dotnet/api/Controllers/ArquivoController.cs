using model;
using data;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using api.Dtos;
using System;
using framework;

namespace api.Controllers
{
    [ApiController]
    [Route("file/[action]")]
    [Route("file")]
    public class ArquivoController : ControllerBase
    {
        private FileManager FileManager;

        public ArquivoController(Repository<Arquivo> arquivoRepository, FileManager fileManager)
        {
            FileManager = fileManager;
        }

        [HttpPost]
        public virtual ArquivoDto Post([FromBody]UploadDeArquivo uploadFileRequest)
        {
            var arquivo = new Arquivo()
            {
                Nome = "",
                Tipo = uploadFileRequest.Tipo,
                Descricao = uploadFileRequest.Descricao,
                Temporario = uploadFileRequest.Temporario,
                DataDeCriacao = DateTimeSync.Now
            };
            FileManager.SaveFile(arquivo, uploadFileRequest.Base64);
            return ArquivoDto.Build(arquivo);
        }

        [HttpGet]
        public virtual Stream Get(string fileName)
        {
            var stream = FileManager.Get(fileName);
            return stream;
        }

        [HttpGet]
        [Route("base64/{fileName}")]
        public virtual string GetBase64(string fileName)
        {
            var result = Convert.ToBase64String(FileManager.GetBytes(fileName));
            return result;
        }

        [HttpGet]
        [Route("info/{fileName}")]
        public virtual ArquivoDto GetInfo(string fileName)
        {
            var result = FileManager.GetInfo(fileName);
            return result;
        }
    }


    [ApiController]
    [Route("publicfile")]
    public class ArquivoPublicoController : ControllerBase
    {
        private Repository<Arquivo> ArquivoRepository;
        private FileManager FileManager;

        public ArquivoPublicoController(Repository<Arquivo> arquivoRepository, FileManager fileManager)
        {
            ArquivoRepository = arquivoRepository;
            FileManager = fileManager;
        }

        [HttpGet]
        public virtual Stream Get(string fileName)
        {
            var stream = FileManager.Get(fileName);
            return stream;
        }

        [HttpGet]
        [Route("download/{fileName}")]
        public virtual Stream Download(string fileName)
        {
            var stream = FileManager.DownloadPublicFile(fileName);
            return stream;        
        }
    }

    public class UploadDeArquivo
    {
        public string Tipo { get; set; }
        public string Base64 { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Temporario { get; set; }
    }
}
