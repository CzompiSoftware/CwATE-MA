using Markdig;
using System.Collections.Generic;
using System.IO;

namespace CWCTMA.Model
{
    public class Config
    {
        public string CdnUrl { get; set; }
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string SiteURL { get; set; }
        public Meta Meta { get; set; }
    }
}