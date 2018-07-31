using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.M.Attributes
{
    /// <summary>
    /// DB에서 검색 시 Where 조건으로 필터링 될 수 있는지 조건 처리.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterableAttribute : Attribute
    {
        public bool IsFilterable { get; set; }

        public FilterableAttribute(bool IsFilterable = true)
        {
            this.IsFilterable = IsFilterable;
        }
    }
}
