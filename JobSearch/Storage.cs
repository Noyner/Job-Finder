using System.IO;

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
