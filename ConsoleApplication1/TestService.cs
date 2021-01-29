using System;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace TestAuthHelper
{
    [ServiceContract]
    internal interface ITestService
    {
        [OperationContract]

        TokenInfo GetToken();

        [OperationContract]
        string CallAPIWithToken();
    }
    internal class TestService : ITestService
    {
        [WebInvoke(Method = "POST")]
        public string CallAPIWithToken()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            return request.Headers[HttpRequestHeader.Authorization];
        }
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        public TokenInfo GetToken()
        {
            //WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";                        
            return new TokenInfo
            {
                Token = "somevalue"
            };
        }
    }

    [DataContract(Namespace = "http://test/data/token")]
    public class TokenInfo
    {
        internal bool IsWaiting;
        [DataMember(Name = "token_type", Order = 0)]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in", Order = 1)]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "ext_expires_in", Order = 2)]
        public int ExtExpiresIn { get; set; }

        [DataMember(Name = "expires_on", Order = 3)]
        private long expires_on;
        public DateTime ExpiresOn { get { return (new DateTime(1970, 1, 1)).AddSeconds(expires_on).ToLocalTime(); } }

        [DataMember(Name = "not_before", Order = 4)]
        private long not_before;

        public DateTime NotBefore { get { return (new DateTime(1970, 1, 1)).AddSeconds(not_before).ToLocalTime(); } }

        [DataMember(Name = "resource", Order = 5)]
        public string Resource { get; set; }

        [DataMember(Name = "access_token", Order = 6)]
        public string Token { get; set; }
        public DateTime Created { get; private set; }    
    }
}
