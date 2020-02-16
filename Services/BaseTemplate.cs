using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public abstract class BaseTemplate
    {
        public static string GetTemplateFile { get; }

        public abstract BaseTemplate FillParams(params Param[] parameters);

        public abstract Task<Mail> BuildAsync();
    }

    public struct Param
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
