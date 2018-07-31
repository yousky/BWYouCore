using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.ViewModels
{
    /// <summary>
    /// 밸리데이션 에러 저장용 클래스
    /// </summary>
    public class ValidationObjectError
    {
        /// <summary>
        /// 오브젝트 명
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// 에러 메세지
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public ValidationObjectError()
        {

        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <param name="ErrorMessage"></param>
        public ValidationObjectError(string ObjectName, string ErrorMessage)
        {
            this.ObjectName = ObjectName;
            this.ErrorMessage = ErrorMessage;
        }
    }
}
