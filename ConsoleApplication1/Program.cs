using BizTalkComponents.CustomComponents.AuthHelper;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using TestAuthHelper;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateSevice();
            Console.ReadLine();
        }
        static ServiceHost svcHost;
        static void CreateSevice()
        {
            WebHttpBinding binding = new WebHttpBinding();
            svcHost = new ServiceHost(typeof(TestService));
            var svcbinding = svcHost.AddServiceEndpoint(typeof(ITestService), binding, "http://localhost:1234/service");
            svcbinding.EndpointBehaviors.Add(new WebHttpBehavior() { AutomaticFormatSelectionEnabled = true });
            Task.Run(() => svcHost.Open());
        }

        private static void SvcHost_Faulted(object sender, EventArgs e)
        {

        }

        private static void SvcHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {

        }
    }
}
