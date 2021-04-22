using Refit;
using RefitClientAndBearerToken.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefitClientAndBearerToken.ConsoleClient.Interfaces
{
    public interface ILoginIDP
    {
        [Post("/api/token")]
        Task<OutputToken> GetBearerToken([Body(BodySerializationMethod.UrlEncoded)] UserCredentail userCredentails);

    }

    public interface IWeather
    {
        [Get("/WeatherForecast")]
        Task<IEnumerable<WeatherForecast>> GetWeather([Authorize("Bearer")] string token);
    }
}
