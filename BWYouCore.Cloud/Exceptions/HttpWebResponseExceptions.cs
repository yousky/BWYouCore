using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Cloud.Exceptions
{
    /// <summary>
    /// HttpWebResponse 중에 처리 되지 않은 예외 발생
    /// </summary>
    [Serializable]
    public class HttpWebResponseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpWebResponseException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HttpWebResponseException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public HttpWebResponseException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected HttpWebResponseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    /// <summary>
    /// HttpWebResponse 중에 Unauthorized 예외 발생
    /// </summary>
    [Serializable]
    public class HttpWebResponseUnauthorizedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpWebResponseUnauthorizedException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HttpWebResponseUnauthorizedException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public HttpWebResponseUnauthorizedException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected HttpWebResponseUnauthorizedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
