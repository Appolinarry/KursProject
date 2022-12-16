using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Model
{
    internal class EKB
    {
        public string ClassName { get; set; }

        public List<(string, string)> Attribute { get; set; }


        public EKB(string className, List<(string, string)> attribute)
        {
            ClassName = className;
            Attribute = attribute;
        }
    }
}
