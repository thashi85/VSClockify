using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VSClockify.Services.Models;

namespace VSClockify.Services
{
    public static class ServiceUtility
    {
        static Configuration _appConfig;
        public static string AppFolderPath = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
        static string _appConfigPath= System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath) + "\\app.config";
        public static Configuration AppConfig
        {
            get
            {
                if (_appConfig==null)
                {
                    ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                    configMap.ExeConfigFilename =@""+ _appConfigPath; // the path of the custom app.config
                    _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                }
                return _appConfig;
            }
            set
            {
                _appConfig = value;
            }
    }
        public static string ClockifyApiUrl
        {
            get
            {
                return AppConfig.AppSettings.Settings["ClockifyApiUrl"]?.Value;
            }
        }
        public static string LogDetails
        {
            get
            {
                return AppConfig.AppSettings.Settings["LogDetails"]?.Value;
            }
        }
        public static string SecurityProtocolType
        {
            get
            {
                return AppConfig.AppSettings.Settings["SecurityProtocolType"]?.Value;
            }
        }
        
        public static string ClockifyApiKey
        {
            get
            {
                return AppConfig.AppSettings.Settings["ClockifyApiKey"]?.Value;
            }
            set
            {                         

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = @"" + _appConfigPath; // the path of the custom app.config
                _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                _appConfig.AppSettings.Settings.Remove("ClockifyApiKey");
                _appConfig.AppSettings.Settings.Add("ClockifyApiKey", value);
                _appConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
        public static string ClockifyDefaultProject
        {
            get
            {
                return AppConfig.AppSettings.Settings["ClockifyDefaultProject"].Value;
            }
            set
            {

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = @"" + _appConfigPath; // the path of the custom app.config
                _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                _appConfig.AppSettings.Settings.Remove("ClockifyDefaultProject");
                _appConfig.AppSettings.Settings.Add("ClockifyDefaultProject", value);
                _appConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
        public static string AzurePAT
        {
            get
            {
                return AppConfig.AppSettings.Settings["AzurePAT"]?.Value;
            }
            set
            {

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = @"" + _appConfigPath; // the path of the custom app.config
                _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                _appConfig.AppSettings.Settings.Remove("AzurePAT");
                _appConfig.AppSettings.Settings.Add("AzurePAT", value);
                _appConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
        public static string AzureSearchAPIEndPoint
        {
            get
            {
                return AppConfig.AppSettings.Settings["AzureSearchAPIEndPoint"]?.Value;
            }
            set
            {

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = @"" + _appConfigPath; // the path of the custom app.config
                _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                _appConfig.AppSettings.Settings.Remove("AzureSearchAPIEndPoint");
                _appConfig.AppSettings.Settings.Add("AzureSearchAPIEndPoint", value);
                _appConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
        public static string AzureAPIEndPoint
        {
            get
            {
                return AppConfig.AppSettings.Settings["AzureAPIEndPoint"]?.Value;
            }
            set
            {

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = @"" + _appConfigPath; // the path of the custom app.config
                _appConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                _appConfig.AppSettings.Settings.Remove("AzureAPIEndPoint");
                _appConfig.AppSettings.Settings.Add("AzureAPIEndPoint", value);
                _appConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
        public static string SendWebRequest(string webMethod, string webMethodUrl, string postData)
        {


            var request = WebRequest.Create(webMethodUrl);
            request.Method = webMethod;
            request.ContentType = "application/json; charset=utf-8";
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }
            string result = "";
            using (var response = request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

       
        public static string WebAPIRequest(out int status, string baseUrl, Enums.WebMethod webMethod, string webMethodUrl, List<Parameter> urlParam, object bodyParam, string id = "", string include = "", string fields = "", ICollection<KeyValuePair<string,string>> headers=null)
        {

            //  OptimoAPIResult<T> retObj = new OptimoAPIResult<T>();
            try
            {

                var postData = "";
                if (bodyParam != null)
                {
                    //To serialize a POCO in json:api format
                    //postData = JsonConvert.SerializeObject(bodyParam, new JsonApiSerializerSettings());
                    postData = JsonConvert.SerializeObject(bodyParam, new JsonSerializerSettings()
                    {
                        //DateFormatHandling =DateFormatHandling.IsoDateFormat,
                        NullValueHandling=NullValueHandling.Ignore,
                        DateFormatString= "yyyy-MM-ddTHH:mm:ssZ"
                    });
                }

                var client = new RestClient(baseUrl);
                //client.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
                {
                    RestRequest request = new RestRequest();
                    request.Resource = webMethodUrl;
                    switch (webMethod)
                    {
                        case Enums.WebMethod.GET:
                            request.Method = Method.GET;
                            if (urlParam != null && urlParam.Count > 0)
                            {
                                foreach (Parameter p in urlParam)
                                {
                                    if (p.Value != null)
                                    {
                                        request.AddQueryParameter(p.Name, p.Value.ToString());
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(include))
                                request.AddQueryParameter("include", include);
                            if (!string.IsNullOrEmpty(fields))
                                request.AddQueryParameter("fields", fields);
                            break;
                        case Enums.WebMethod.POST:
                            request.Method = Method.POST;
                            break;
                        case Enums.WebMethod.PUT:
                            request.Method = Method.PUT;
                            break;
                        //case Enums.WebMethod.DELETE:
                        //    request.Method = Method.DELETE;
                        //    break;
                        case Enums.WebMethod.PATCH:
                            request.Method = Method.PATCH;
                            break;
                        default:
                            request.Method = Method.GET;
                            break;
                    }
                    if (headers!=null && headers.Count>0)
                    {
                        request.AddHeaders(headers);
                    }
                   
                    request.AddHeader("Accept", "application/json");

                    //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
                    if (!string.IsNullOrEmpty(postData))
                    {
                        //request.AddBody(postData);
                        request.AddParameter("application/json", postData, ParameterType.RequestBody);
                    }

                    if (!string.IsNullOrEmpty(id))
                        request.AddUrlSegment("id", id); // replaces matching token in request.Resource

                    var st = DateTime.Now;
                    var sec_protocol = SecurityProtocolType;
                    if (sec_protocol != null && !string.IsNullOrEmpty(sec_protocol))
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)Int32.Parse(sec_protocol);
                    }


                    IRestResponse res = client.Execute(request);
                    //var content = res.Content;
                    var content = "";
                    if (res.ContentEncoding == "")
                    {
                        content = res.Content;
                    }
                    else
                    {
                        byte[] decompress = Decompress(res.RawBytes, res.ContentEncoding);
                        content = System.Text.ASCIIEncoding.ASCII.GetString(decompress);
                    }


                    if (LogDetails=="1")
                    {
                        LogRequest(request, res, (DateTime.Now - st).Milliseconds, client);
                    }
                    status = ((Int32)res.StatusCode);
                    return content;
                  

                }
            }
            catch (Exception ex)
            {
                Log4netLogger<string>.LogError("ServiceUtility: WebAPIRequest :" + webMethodUrl, ex);
                throw ex;
            }
            //return retObj;
        }
  
        static byte[] Decompress(byte[] content, string encoding)
        {
            encoding = encoding.ToLower();
            if (encoding == "gzip")
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(content), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
            else if (encoding == "deflate")
            {
                using (DeflateStream stream = new DeflateStream(new MemoryStream(content), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
            return null;
        }
        public static string ConverDateTimeToISOString(DateTime? dt)
        {
            if (dt != null && dt.HasValue)
            {
                return dt.Value.ToString("O");
            }
            return "";
        }

        private static void LogRequest(IRestRequest request, IRestResponse response, long durationMs, RestClient _restClient)
        {
            var requestToLog = new
            {
                resource = request.Resource,
                // Parameters are custom anonymous objects in order to have the parameter type as a nice string
                // otherwise it will just show the enum value
                parameters = request.Parameters.ConvertAll(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
                // ToString() here to have the method as a nice string otherwise it will just show the enum value
                method = request.Method.ToString(),
                // This will generate the actual Uri used in the request
                uri = _restClient.BuildUri(request),
            };

            var responseToLog = new
            {
                statusCode = response.StatusCode,
                content = response.Content,
                headers = response.Headers,
                // The Uri that actually responded (could be different from the requestUri if a redirection occurred)
                responseUri = response.ResponseUri,
                errorMessage = response.ErrorMessage,
            };
           
            MethodLogger.SaveLogToFile(string.Format("Request completed in {0} ms", durationMs));
            MethodLogger.SaveLogToFile(string.Format("Request: {0}", JsonConvert.SerializeObject(requestToLog).ToString().Replace("\\\"", "\"")));
            MethodLogger.SaveLogToFile(string.Format("Response: {0}", JsonConvert.SerializeObject(responseToLog).ToString().Replace("\\\"", "\"")));


            //Log4netLogger<string>.LogInfo(string.Format("Request completed in {0} ms", durationMs));
            //Log4netLogger<string>.LogInfo(string.Format("Request: {0}", JsonConvert.SerializeObject(requestToLog).ToString().Replace("\\\"", "\"")));
            //Log4netLogger<string>.LogInfo(string.Format("Response: {0}", JsonConvert.SerializeObject(responseToLog).ToString().Replace("\\\"", "\"")));

        }

    }
}
