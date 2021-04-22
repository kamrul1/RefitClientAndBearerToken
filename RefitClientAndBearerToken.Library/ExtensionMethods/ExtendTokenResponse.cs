using IdentityModel.Client;
using RefitClientAndBearerToken.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RefitClientAndBearerToken.Library.ExtensionMethods
{
    public static class ExtendTokenResponse
    {
        public static OutputToken GetJsonExtensionMethod(this TokenResponse tokenResponse)
        {
            return new OutputToken
            {
                AccessToken = tokenResponse.AccessToken ?? "",
                Scope = tokenResponse.Scope ?? "",
                ExpiresIn = tokenResponse.ExpiresIn,
                TokenType = tokenResponse.TokenType ?? ""
            };
        }
    }
}
