using System;
using Microsoft.AspNetCore.Mvc;
using framework.Security;

namespace api.Controllers
{
    [Route("publickey")]
    [ApiController]
    public class PublicKey : ControllerBase
    {
        private readonly IKeyProvider KeyProvider;

        public PublicKey(IKeyProvider keyProvider)
        {
            KeyProvider = keyProvider;
        }

        [HttpGet]
        public string GetPublicKey()
        {
            var result = KeyProvider.GetRSAPublicKey();
            return result;
        }
    }
}
