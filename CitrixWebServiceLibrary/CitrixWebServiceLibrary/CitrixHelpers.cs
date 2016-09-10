using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitrixWebServiceLibrary
{
    public static class CitrixHelpers
    {
        static string baseSFURL = System.Configuration.ConfigurationManager.AppSettings["CitrixStorefrontUrl"].ToString();

       
        public static Dictionary<string, string> AuthenticateWithPost(string _username, string _password, string _domain, bool notUsed = false)
        {
            Dictionary<string, string> _returnValues = new Dictionary<string, string>();

            try
            {
                string _csrfToken = Guid.NewGuid().ToString();
                string _aspnetSessionID = Guid.NewGuid().ToString();
                string SFURL = "https://" + baseSFURL;

                string _authenticationBody = string.Format("username={0}\\{1}&password={2}", _domain, _username, _password);
                Console.WriteLine(string.Format("AuthenticateWithPost info: auth body: {0}", _authenticationBody));

                RestClient _rc = new RestClient(SFURL);
                RestRequest _authReq = new RestRequest("/PostCredentialsAuth/Login", Method.POST);
                _authReq.AddHeader("X-Citrix-IsUsingHTTPS", "Yes");
                _authReq.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                _authReq.AddHeader("Csrf-Token", _csrfToken);
                _authReq.AddCookie("csrtoken", _csrfToken);
                _authReq.AddCookie("asp.net_sessionid", _aspnetSessionID);
                _authReq.AddParameter("application/x-www-form-urlencoded", _authenticationBody, ParameterType.RequestBody);

                RestSharp.IRestResponse _authResp = _rc.Execute(_authReq);
                Console.WriteLine(string.Format("AuthenticateWithPost info: got _authResp"));

                foreach (var header in _authResp.Headers.Where(i => i.Name == "Set-Cookie"))
                {

                    Console.WriteLine(string.Format("AuthenticateWithPost info: got header.Name: {0}", header.Name));
                    string[] cookieValues = header.Value.ToString().Split(',');
                    foreach (string cookieValue in cookieValues)
                    {
                        Console.WriteLine(string.Format("AuthenticateWithPost info: cookieValue: {0}", cookieValue));
                        string[] cookieElements = cookieValue.Split(';');
                        string[] keyValueElements = cookieElements[0].Split('=');
                        _returnValues.Add(keyValueElements[0], keyValueElements[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error in AuthenticateWithPost: {0}", ex.Message));
            }
            return _returnValues;
        }

        public static List<CitrixApplicationInfo> GetResources(string SessionID, string AuthID, string CsrfToken, bool UseSSL)
        {
            string SFURL = null;

            if (UseSSL)
            {
                SFURL = "https://" + baseSFURL;
            }
            else
            {
                SFURL = "http://" + baseSFURL;
            }


            List<CitrixApplicationInfo> _applicationList = new List<CitrixApplicationInfo>();
            RestClient _rc = new RestClient(SFURL);
            RestRequest _getResourcesReq = new RestRequest(@"Resources/List", Method.POST);
            if (UseSSL)
            {
                _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "Yes");
            }
            else
            {
                _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "No");
            }
            _getResourcesReq.AddHeader("Accept", "application/json");
            _getResourcesReq.AddHeader("Csrf-Token", CsrfToken);
            _getResourcesReq.AddCookie("csrftoken", CsrfToken);
            _getResourcesReq.AddCookie("asp.net_sessionid", SessionID);
            _getResourcesReq.AddCookie("CtxsAuthId", AuthID);
            //_getResourcesReq.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse _resourceListResp = _rc.Execute(_getResourcesReq);

            string json = _resourceListResp.Content;

            JObject a = JObject.Parse(json);
            JArray resources = (JArray)a["resources"];

            foreach (var o in resources)
            {
                CitrixApplicationInfo r = new CitrixApplicationInfo();
                r.AppTitle = o["name"].ToString();
                try
                {
                    r.AppDesc = o["description"].ToString();
                }
                catch (Exception e)
                {
                    r.AppDesc = "";
                }
                r.AppIcon = o["iconurl"].ToString();
                r.AppLaunchURL = o["launchurl"].ToString();
                r.ID = o["id"].ToString();
                r.Subscribed = o["subscriptionstatus"] == null ? "" : o["subscriptionstatus"].ToString();
                r.Position = o["position"] == null ? -1 : Convert.ToInt32(o["position"].ToString());
                r.SubscriptionUrl = o["subscriptionurl"].ToString();
                _applicationList.Add(r);
            }

            return _applicationList;
        }
        public static string GetIcaString(string SessionID, string AuthID, string CsrfToken, string AppID, bool UseSSL)
        {
            string SFURL = null;

            if (UseSSL)
            {
                SFURL = "https://" + baseSFURL;
            }
            else
            {
                SFURL = "http://" + baseSFURL;
            }

            RestClient _rc = new RestClient(SFURL);
            RestRequest _getResourcesReq = new RestRequest(string.Format("Resources/LaunchIca/{0}", AppID), Method.GET);
            if (UseSSL)
            {
                _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "Yes");
            }
            else
            {
                _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "No");
            }
            _getResourcesReq.AddHeader("Content-Type", "application/octet-stream");
            _getResourcesReq.AddHeader("Csrf-Token", CsrfToken);
            _getResourcesReq.AddCookie("csrftoken", CsrfToken);
            _getResourcesReq.AddCookie("asp.net_sessionid", SessionID);
            _getResourcesReq.AddCookie("CtxsAuthId", AuthID);
            IRestResponse _resourceListResp = _rc.Execute(_getResourcesReq);

            string _icaFileString = _resourceListResp.Content;

            return _icaFileString;
        }
       
        public static string GetLaunchStatus(string SessionID, string AuthID, string CsrfToken, string icaName, bool UseSSL)
        {
            string SFURL = null;
            SFURL = "https://" + baseSFURL;           
           
            RestClient _rc = new RestClient(SFURL);
            RestRequest _getResourcesReq = new RestRequest(string.Format("Resources/GetLaunchStatus/{0}", icaName), Method.POST);
          
            _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "Yes");
            _getResourcesReq.AddHeader("Content-Type", "application/octet-stream");
            _getResourcesReq.AddHeader("Csrf-Token", CsrfToken);
            _getResourcesReq.AddCookie("csrftoken", CsrfToken);
            _getResourcesReq.AddCookie("asp.net_sessionid", SessionID);
            _getResourcesReq.AddCookie("CtxsAuthId", AuthID);
            IRestResponse _resourceListResp = _rc.Execute(_getResourcesReq);
            string _icaFileString = _resourceListResp.Content;

            //
            _rc = new RestClient(SFURL);
            _getResourcesReq = new RestRequest(string.Format("Resources/LaunchIca/{0}.ica", icaName), Method.GET);
            _getResourcesReq.AddHeader("X-Citrix-IsUsingHTTPS", "Yes");
            _getResourcesReq.AddHeader("Content-Type", "application/octet-stream");
            _getResourcesReq.AddHeader("Csrf-Token", CsrfToken);
            _getResourcesReq.AddCookie("csrftoken", CsrfToken);
            _getResourcesReq.AddCookie("asp.net_sessionid", SessionID);
            _getResourcesReq.AddCookie("CtxsAuthId", AuthID);
           _resourceListResp = _rc.Execute(_getResourcesReq);
            string _icaFileContent = _resourceListResp.Content;

            return _icaFileContent;

        }





        public static void LaunchApplication(string SessionID, string AuthID, string CsrfToken, string AppID, bool UseSSL, string LocalPath)
        {

            string _ica = GetIcaString(SessionID, AuthID, CsrfToken, AppID, UseSSL);
            //save the file
            string FileNamdAndPath = string.Format(@"{0}\{1}.ica", LocalPath, AppID);
            File.WriteAllText(FileNamdAndPath, _ica);
            System.Diagnostics.Process.Start(FileNamdAndPath);
        }       
    }
}
