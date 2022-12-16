using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Model
{
    internal class Association
    {
        public string AssotionName { get; set; }
        public string SourceName { get; set; }
        public string TargetName { get; set; }

        public Association(string assotionName, string sourceName, string targetName)
        {
            AssotionName = assotionName;
            SourceName = sourceName;
            TargetName = targetName;
        }
    }
}
