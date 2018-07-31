using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.M.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DbFieldMapAttribute : Attribute
    {
        public string Field { get; set; }
        public DbFieldMapAttribute(string field)
        {
            Field = field;
        }
    }
}
