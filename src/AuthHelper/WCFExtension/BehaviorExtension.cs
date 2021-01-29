using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.CustomComponents.AuthHelper
{
    public class OAuthSecurityTokenBehavior : IClientMessageInspector, IEndpointBehavior
    {

        public string OAuth2TokenEndPoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool CacheToken { get; set; }
        public string Claims { get; set; }

        public OAuthSecurityTokenBehavior(string oAuth2TokenEndPoint, string clientId, string clientSecret, string claims, bool cacheToken)
        {
            this.Claims = claims;
            this.OAuth2TokenEndPoint = oAuth2TokenEndPoint;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.CacheToken = cacheToken;
        }
        public OAuthSecurityTokenBehavior() { }

        #region IClientMessageInspector

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // do nothing
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest = null;
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            }

            if (httpRequest == null)
            {
                httpRequest = new HttpRequestMessageProperty()
                {
                    Method = "GET",
                    SuppressEntityBody = true
                };
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }
            WebHeaderCollection headers = httpRequest.Headers;

            string token = CacheToken ? AppDomainHelper.TokenDictionary.GetToken(OAuth2TokenEndPoint, ClientId, ClientSecret, Claims)
                : TokenDictionary.GetNewToken(OAuth2TokenEndPoint, ClientId, ClientSecret, Claims).Token;
            //Remove the authorization header if already exists.
            headers.Remove(HttpRequestHeader.Authorization);
            headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            return null;
        }

        #endregion IClientMessageInspector

        #region IEndpointBehavior

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        #endregion IEndpointBehavior
    }

    public class OAuthSecurityTokenBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(OAuthSecurityTokenBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new OAuthSecurityTokenBehavior(OAuth2TokenEndPoint, ClientId, ClientSecret, Claims, CacheToken);
        }

        [ConfigurationProperty("OAuth2TokenEndPoint", IsRequired = true)]
        public string OAuth2TokenEndPoint
        {
            get { return (string)this["OAuth2TokenEndPoint"]; }
            set { this["OAuth2TokenEndPoint"] = value; }
        }

        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get { return (string)this["ClientId"]; }
            set { this["ClientId"] = value; }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get { return (string)this["ClientSecret"]; }
            set { this["ClientSecret"] = value; }
        }

        [ConfigurationProperty("Claims", IsRequired = false, DefaultValue = null)]
        [RegexStringValidator(@"(^$)|((\w+?=[^&=]+?)(&\w+=[^&]+)*$)")]
        public string Claims
        {
            get { return (string)this["Claims"]; }
            set { this["Claims"] = value; }
        }

        [ConfigurationProperty("CacheToken", IsRequired = false, DefaultValue = false)]
        public bool CacheToken
        {
            get { return (bool)this["CacheToken"]; }
            set { this["CacheToken"] = value; }
        }
    }

}
