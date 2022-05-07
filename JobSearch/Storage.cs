using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace JobSearch
{
    public static class Storage
    {
        public static string Get()
        {
            return File.ReadAllText(@".\Storage\token");
        }

        public static void Set(string token)
        {
            if (!Directory.Exists(@".\Storage"))
            {
                Directory.CreateDirectory(@".\Storage");
            }
            File.WriteAllText(@".\Storage\token", token);

        }
    }
}
