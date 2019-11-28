using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Finances.Mvc
{
    public static class ResourceHelpers
    {
        public static async Task<string> GetString(Assembly asm, string resource)
        {
            var names = asm.GetManifestResourceNames();

            var name = names.FirstOrDefault(x => x.EndsWith(resource, StringComparison.InvariantCultureIgnoreCase));

            if(name == null)
            {
                throw new FileNotFoundException($"Resource: {name}");
            }

            using (var sr = new StreamReader(asm.GetManifestResourceStream(name)))
            {
                return await sr.ReadToEndAsync();
            }
        }

    }
}
