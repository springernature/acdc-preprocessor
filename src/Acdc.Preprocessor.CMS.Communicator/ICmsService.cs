using System.Net.Http;

namespace Acdc.Preprocessor.CMS.Communicator
{
  public interface ICmsService
  {
    CmsRouteResource GetResourceAsync(string route, bool throwException = true,HttpClient fakeHttpClientForTest=null);
    string GetCMSResponseFromRequest(string route, bool throwException = true, HttpClient fakeHttpClientForTest = null);
    StreamAndFileName GetXml(string fileUrl, bool throwException = true, HttpClient fakeHttpClientForTest = null);
  }
}
