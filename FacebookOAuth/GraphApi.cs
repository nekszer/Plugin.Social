using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Plugin.Social.Facebook
{
    public class GraphApi
    {

        public string AccessToken
        {
            get; set;
        }

        internal GraphApi(string accesstoken)
        {
            AccessToken = accesstoken;
        }

        public async Task<T> Explorer<T>(string path, Dictionary<string, object> parameters = null)
        {
            try
            {
                var requestUrl = UrlRequest(path, parameters);
                return await GetRequest<T>(requestUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex, "FacebookApi");
            }
            return default(T);
        }

        public string UrlRequest(string path, Dictionary<string, object> parameters = null)
        {
            if(string.IsNullOrEmpty(path)) throw new NullReferenceException("Path is null");
            if(!path.StartsWith("/")) throw new Exception("Path requires / at start");
            if (parameters != null)
            {
                List<string> parameterslist = new List<string>();
                foreach (var item in parameters)
                {
                    var keyvalue = $"{item.Key}={item.Value}";
                    parameterslist.Add(keyvalue);
                }
                string parametersget = string.Join("&", parameterslist);
                path = $"{path}?{parametersget}";
            }
            string requestUrl = $"https://graph.facebook.com{path}&access_token={AccessToken}";
            System.Diagnostics.Debug.WriteLine(requestUrl, "FacebookApi");
            return requestUrl;
        }

        private async Task<T> GetRequest<T>(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            try
            {
                System.Diagnostics.Debug.WriteLine(url, "FacebookApi");
                response = await client.GetAsync(url);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex, "FacebookApi");
            }
            if(response != null)
            {
                if(response.Content != null)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(json, "FacebookApi");
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    return result;
                }
            }
            return default(T);
        }

    }
}
