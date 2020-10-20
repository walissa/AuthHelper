using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace BizTalkComponents.CustomComponents.AuthHelper
{
    [Serializable]
    public static class AuthHelper
    {
        public static string GetBearerToken(string oAuth2Url, string client_Id, string client_Secret, string claims, bool forceNewToken = false)
        {
            return AppDomainHelper.TokenDictionary.GetToken(oAuth2Url, client_Id, client_Secret, claims, forceNewToken);
        }
        
        public static string GetBasicAuthEncoded(string username,string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");
            string credentials = string.Format("{0}:{1}", username, password);
            byte[] data = Encoding.UTF8.GetBytes(credentials);
            string encoded=Convert.ToBase64String(data);
            return encoded;
        }
    }
}
