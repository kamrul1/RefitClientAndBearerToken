using Microsoft.Extensions.Configuration;
using Refit;
using RefitClientAndBearerToken.ConsoleClient.Interfaces;
using RefitClientAndBearerToken.Library.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();


Console.WriteLine("Press Enter to Start:");
Console.ReadLine(); //pause console

var idp = RestService.For<ILoginIDP>(ReadConfig().idpUrl);

UserCredentail userCredentails = new UserCredentail
{
    username = ReadConfig().username,
    password = ReadConfig().password,
};

OutputToken outputToken = await idp.GetBearerToken(userCredentails);
Console.WriteLine($"Bearer {outputToken.AccessToken}");


var webApi = RestService.For<IWeather>(ReadConfig().webApiUrl);
var weatherList = await webApi.GetWeather(outputToken.AccessToken);

//seralize the result to console
var weatherJson = JsonSerializer.Serialize(weatherList, new JsonSerializerOptions { WriteIndented=true});
Console.WriteLine(weatherJson);
Console.ReadLine(); //pause console


//function returning a tuple - used for organising file only
(string idpUrl, string username, string password, string webApiUrl) ReadConfig() {
    string _idpUrl = config.GetSection("RefitClientAndBearerTokenIDP:Url").Value;
    string _username = config.GetSection("RefitClientAndBearerTokenIDP:username").Value;
    string _password = config.GetSection("RefitClientAndBearerTokenIDP:password").Value;
    string _webApiUrl = config.GetSection("WebApi:Url").Value;
    return (_idpUrl, _username, _password, _webApiUrl); 
}
 

