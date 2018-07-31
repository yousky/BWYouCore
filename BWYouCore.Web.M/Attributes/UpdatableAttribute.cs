using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.M.Attributes
{
    /// <summary>
    /// DB에서 업데이트 될 수 있는 칼럼임을 나타냄.
    /// Web에서 바인딩 되어 서비스의 UpdateAsync 에서만 사용 됨.
    /// 바인딩모델을 사용 하는 곳이 아니면 의미 없음.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdatableAttribute : Attribute
    {
        public bool IsUpdatable { get; set; }

        public UpdatableAttribute(bool IsUpdatable = true)
        {
            this.IsUpdatable = IsUpdatable;
        }
    }
}
