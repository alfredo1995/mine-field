using System.Collections.Specialized;

namespace Ws.DataTypes
{
    using System;
    using System.Web;

    namespace NoTask.CrashGamePlatform.DataTypes
    {
        [Serializable]
        public struct QueryParam
        {
            public string key;
            public string value;

            public QueryParam(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }

        [Serializable]
        public struct Endpoint
        {
            private Uri _uri;
            private NameValueCollection _queryParams;
            public string this[string key] => _queryParams[key];
            
            public Endpoint(string uri)
            {
                _uri = new Uri(uri);
                _queryParams = new NameValueCollection();

                QueriesToParam();
            }

            public string GetBaseEndpoint(string scheme, string key) => $"{scheme}://{this[key].Replace("wss://", string.Empty)}";

            private void QueriesToParam()
            {
                if (_uri.Query.Length <= 0) return;
                _queryParams = HttpUtility.ParseQueryString(_uri.Query);
            }
        }
    }
}