using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizTalkComponents.CustomComponents.AuthHelper;
using System.ServiceModel;
using System.Threading.Tasks;
using System.ServiceModel.Description;

namespace TestAuthHelper
{
    [TestClass]
    public class TestBehaviorExtElement
    {
        [TestMethod]
        public void TestSetNullOrEmptyClaims()
        {
            var elm = new OAuthTokenSecurityBehaviorElement();
            elm.Claims = null;
            elm.Claims = "";
        }

        [TestMethod(),ExpectedException(typeof(System.Configuration.ConfigurationErrorsException))]
        public void TestSetWrongClaims()
        {
            var elm = new OAuthTokenSecurityBehaviorElement();
            elm.Claims = "Abc=Test&aa";
        }        

        [TestMethod]
        public void TestOAuthTokenSecurityBehavior()
        {
            var binding = new WebHttpBinding();
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress("http://localhost:1234"));
            OAuthTokenSecurityBehavior behavior = new OAuthTokenSecurityBehavior
            {
                OAuth2TokenEndPoint = "http://localhost:1234/GetToken",
                ClientId = "myclientId",
                ClientSecret = "mysecret",
                Claims = "Some=Thing&other=thing",                
            };
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            var channel=factory.CreateChannel();
            CreateSevice();
            Task.Delay(5000).Wait();
            string ret=channel.CallAPIWithToken();
            svcHost.Close();
            Assert.AreEqual(ret, "Bearer somevalue");
        }


        private ServiceHost svcHost;
        private void CreateSevice()
        {
            WebHttpBinding binding = new WebHttpBinding();
            svcHost = new ServiceHost(typeof(TestService));
            var svcEP=svcHost.AddServiceEndpoint(typeof(ITestService), binding, "http://localhost:1234");
            svcEP.EndpointBehaviors.Add(new WebHttpBehavior() { AutomaticFormatSelectionEnabled = true });
            Task.Run(() => svcHost.Open());
        }

    }

}
