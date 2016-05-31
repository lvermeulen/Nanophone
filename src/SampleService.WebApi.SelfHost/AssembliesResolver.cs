using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace SampleService.WebApi.SelfHost
{
    public class AssembliesResolver : DefaultAssembliesResolver
    {
        //public override ICollection<Assembly> GetAssemblies()
        //{
        //    var assemblies = base.GetAssemblies();
        //    var nano = Assembly.LoadFrom("Nanophone.RegistryConsumer.WebApi.dll");
        //    assemblies.Add(nano);
        //    return assemblies;
        //}

        public override ICollection<Assembly> GetAssemblies()
        {
            var assemblies = base.GetAssemblies();

            var thisAssembly = Assembly.GetExecutingAssembly();
            foreach (string fileName in Directory.GetFiles(Path.Combine(thisAssembly.Location, "*.dll")))
            {
                if (Path.GetFileNameWithoutExtension(fileName) != thisAssembly.GetName().Name)
                {
                    assemblies.Add(Assembly.LoadFrom(fileName));
                }
            }

            return assemblies;
        }
    }
}
