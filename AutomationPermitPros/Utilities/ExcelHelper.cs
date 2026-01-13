using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Utilities
{
    public class  ExcelHelper
    {

        private bool IsTrue(Dictionary<string, string> data, string key)
        {
            return data.ContainsKey(key) &&
                   bool.TryParse(data[key], out var result) &&
                   result;
        }

    }
}
