
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RefitClientAndBearerToken.Library.ExtensionMethods;
using RefitClientAndBearerToken.Library.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace RefitClientAndBearerToken.IDP.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenController> _logger;
        private readonly string _host;

        public TokenController(IHttpClientFactory httpClientFactory, ILogger<TokenController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _host = httpContextAccessor.HttpContext.Request.Host.Value;
        } 

        [HttpPost()]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<OutputToken> PostCredentials([FromForm] UserCredentail input)
        {
            _logger.LogInformation($"current hose:{_host}");

            var tokenRequest = new TokenRequest
            {
                Address = $"https://{_host}/connect/token",
                GrantType = GrantType.ClientCredentials,
                ClientSecret = input.password,
                ClientId = input.username,
                Parameters =
                {
                    {"scope", "api1.read"},
                }

            };

            var response = await _httpClient.RequestTokenAsync(tokenRequest);
            if (response.IsError) throw new Exception(response.Error);


            return response.GetJsonExtensionMethod();

        }




    }




}
