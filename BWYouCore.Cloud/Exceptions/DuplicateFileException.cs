using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Cloud.Exceptions
{
    /// <summary>
    /// 중복 파일 존재로 인한 예외 발생
    /// </summary>
    [Serializable]
    public class DuplicateFileException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public DuplicateFileException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public DuplicateFileException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public DuplicateFileException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DuplicateFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
