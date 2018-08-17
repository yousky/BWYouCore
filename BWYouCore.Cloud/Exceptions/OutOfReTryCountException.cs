using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Cloud.Exceptions
{
    /// <summary>
    /// 재시도 횟수 초과에 의한 예외 발생
    /// </summary>
    [Serializable]
    public class OutOfReTryCountException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public OutOfReTryCountException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public OutOfReTryCountException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public OutOfReTryCountException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected OutOfReTryCountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
