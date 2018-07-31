using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.ViewModels
{
    /// <summary>
    /// 웹 결과 상태 메세지 본문 클래스
    /// </summary>
    public class WebStatusMessageBody
    {
        /// <summary>
        /// 상태
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 코드
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 메세지
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 관련 링크
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 개발자용 메세지
        /// </summary>
        public string DeveloperMessage { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        public WebStatusMessageBody()
        {

        }
    }
}
