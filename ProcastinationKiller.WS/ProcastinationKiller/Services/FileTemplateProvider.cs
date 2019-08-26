using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class FileTemplateProvider : ITemplateProvider
    {
        public async Task<string> GetTemplate(string filePath)
        {
            var path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                .Substring(6);

            using (var fs = new StreamReader($"{path}\\Assets\\{filePath}"))
            {
                return await fs.ReadToEndAsync();
            }
        }
    }
}
