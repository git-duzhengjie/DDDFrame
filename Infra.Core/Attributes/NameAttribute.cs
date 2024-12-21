using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false,Inherited =true)]
    public class NameAttribute:Attribute
    {
        public NameAttribute() { }
        public NameAttribute(string name) { 
            this.Name = name;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
}
