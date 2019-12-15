using System;
using System.Collections.Generic;
using System.Text;

namespace Semicolon.OnlineJudge.Models
{
    public class SemicolonUrl
    {
        public string UrlAddress { get; set; }

        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();

        public string ToStringUrl()
        {
            string url = UrlAddress + "?";
            url.TrimEnd('/');
            foreach(var param in Params)
            {
                url += param.Key + '=' + param.Value + '&';
            }

            return url.TrimEnd('&', '?');
        }
    }
}
