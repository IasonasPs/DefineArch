
using Mono.Cecil;
using System.IO;
using System.Reflection.PortableExecutable;

namespace DefineArch
{
    internal class Program : OrganizeByArchitecture
    {
        static void Main(string[] args)
        {

            var userSpecifiedPath = GetUserSpecifiedPath(args) ?? "";
            Console.WriteLine("Initial path : " + userSpecifiedPath);

            if (userSpecifiedPath != null)
            {
                Console.WriteLine("The given path is valid");
                OrganizeByArchitecture.OrganizeDllFilesByArchitecture(userSpecifiedPath, false);
            }
            else
            {
                Console.WriteLine("Please provide a path");
            }
        }
    }
}
