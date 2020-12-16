using System;
using System.Runtime.Serialization;

namespace Acdc.Preprocessor.CMS.Communicator
{
  [Serializable]
  public class CmsResourceNotFoundException : Exception
  {
    public CmsResourceNotFoundException()
    {
    }

    public CmsResourceNotFoundException(string message) : base(message)
    {
    }

    public CmsResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CmsResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
