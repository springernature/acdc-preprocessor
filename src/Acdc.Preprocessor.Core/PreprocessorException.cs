using System;
using System.Runtime.Serialization;

namespace Acdc.Preprocessor.Core
{
    [Serializable]
    public class PreprocessorException  : Exception
    {
        public PreprocessorException()
        {
        }

        public PreprocessorException(string message)
            : base(message)
        {
        }

        public PreprocessorException(string message, Exception inner)
            : base(message, inner)
        {
        }
        protected PreprocessorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

