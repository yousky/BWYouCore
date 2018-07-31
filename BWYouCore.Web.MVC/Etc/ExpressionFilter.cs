using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.Etc
{
    public class ExpressionFilter
    {
        public string PropertyName { get; set; }
        public Op Operation { get; set; }
        public object Value { get; set; }

        public ExpressionFilter()
        {

        }
        public ExpressionFilter(string PropertyName, Op Operation, object Value)
        {
            this.PropertyName = PropertyName;
            this.Operation = Operation;
            this.Value = Value;
        }
    }

    public enum Op
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }
}
