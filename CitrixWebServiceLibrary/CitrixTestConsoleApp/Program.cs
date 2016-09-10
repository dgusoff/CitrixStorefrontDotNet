using CitrixWebServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitrixTestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dictionary = CitrixHelpers.AuthenticateWithPost("username", "password", "domain");
            Console.WriteLine(dictionary.Count);
            var ca = CitrixHelpers.GetResources(dictionary["ASP.NET_SessionId"], dictionary["CtxsAuthId"], dictionary["CsrfToken"], true);
            Console.WriteLine(ca.Count);

            string icaString = ca[3].SubscriptionUrl.Replace("Resources/Subscription/", "");

            string ica = CitrixHelpers.GetIcaString(dictionary["ASP.NET_SessionId"], dictionary["CtxsAuthId"], dictionary["CsrfToken"], icaString, true);

            Console.WriteLine(ica);

        }


     
    }
}
