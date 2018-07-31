using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.M.Attributes
{
    /// <summary>
    /// 순차적으로 향해 지는 프로퍼티임을 나타냄
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CascadeRelationAttribute : Attribute
    {
        /// <summary>
        /// 관계 연결의 방향
        /// </summary>
        public CascadeDirection Direction { get; set; }
        /// <summary>
        /// 복사 가능 한 값인지 여부. false일 경우는 원본이 그대로 들어감.
        /// </summary>
        public bool Clonable { get; set; }
        /// <summary>
        /// 생성자. 관계 연결의 방향이 필요
        /// </summary>
        /// <param name="Direction">관계 연결의 방향</param>
        public CascadeRelationAttribute(CascadeDirection Direction)
        {
            this.Direction = Direction;
            this.Clonable = true;
        }
        /// <summary>
        /// 관계 연결의 방향
        /// </summary>
        public enum CascadeDirection
        {
            /// <summary>
            /// 부모가 자식을 바라 보는 방향
            /// </summary>
            Down,
            /// <summary>
            /// 자식이 부모를 바라 보는 방향
            /// </summary>
            Up
        }
    }
}
