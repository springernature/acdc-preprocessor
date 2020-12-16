using System.Threading.Tasks;
using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using Acdc.Preprocessor.Logging;

namespace Acdc.Preprocessor.CMS.Communicator
{
  public class CmsService : ICmsService
  {
    private RetryPolicy GetRetryPolicy(string temppath)
    {
      var logger = LoggerCF.GetInstance();
      return Policy.Handle<HttpRequestException>()
        .WaitAndRetryForever(
          (retryAttempt) =>
          {
            if (retryAttempt <= 3)
            {
              return TimeSpan.FromSeconds(10);
            }
            else if (retryAttempt == 4)
            {
              return TimeSpan.FromMinutes(2);
            }
            else if (retryAttempt == 5)
            {
              return TimeSpan.FromMinutes(3);
            }
            else
            {
              return TimeSpan.FromMinutes(5);
            }
          }, (ex, retry, t) =>
          {

            logger.LogError($"retrycount:- {retry}, timespan{t}");
            logger.LogInfo("Retrying path :: " + temppath);
          }
        );
    }

    public CmsRouteResource GetResourceAsync(string route, bool throwException = true, HttpClient fakeHttpClientForTest = null)
    {
      if (string.IsNullOrEmpty(route)) return null;

      string response = GetCMSResponseFromRequest(route, throwException, fakeHttpClientForTest);
      return !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<CmsRouteResource>(response) : null;
    }

    public string GetCMSResponseFromRequest(string route, bool throwException = true, HttpClient fakeHttpClientForTest = null)
    {
      var logger = LoggerCF.GetInstance();
      string res = string.Empty;
      var httpResponse = new HttpResponseMessage();

      GetRetryPolicy(route).Execute(() =>
      {
        HttpClient httpClient = null;
        httpClient = (fakeHttpClientForTest == null) ? new HttpClient() : fakeHttpClientForTest;
        try
        {
          httpClient.DefaultRequestHeaders.Clear();
          httpResponse = httpClient.GetAsync(route).Result;
          res = GetHttpResponseForGetCMSResponseFromRequest(route, httpResponse);
        }
        catch (TaskCanceledException e)
        {
          logger.LogError($"Request to {route} failed, returned with TaskCanceledException");
          throw new HttpRequestException(e.Message);
        }
        catch (AggregateException e)
        {
          logger.LogError($"Request to {route} failed, returned with AggregateException");
          throw new HttpRequestException(e.Message);
        }
        finally
        {
          httpClient.Dispose();
        }

      });


      return res;
    }

    private string GetHttpResponseForGetCMSResponseFromRequest(string route, HttpResponseMessage httpResponse)
    {
      var res = httpResponse.Content.ReadAsStringAsync().Result;

      if (httpResponse.IsSuccessStatusCode)
        return res;

      if (httpResponse.StatusCode == HttpStatusCode.BadGateway)
      {
        LoggerCF.GetInstance().LogError($"Request to {route} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }

      if (httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
      {
        LoggerCF.GetInstance().LogError($"Request to {route} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }
      if (!httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == HttpStatusCode.NotFound)
      {
        IsFileExist(route, res, httpResponse);
      }
      return string.Empty;
    }

    private void IsFileExist(string route,string res, HttpResponseMessage httpResponse)
    {
      if (!IsFileNotFound(res))
      {
        LoggerCF.GetInstance().LogError($"Reques to {route} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }
      else
      {
        LoggerCF.GetInstance().LogError($"Request to {route} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new CmsResourceNotFoundException($"{route} not found, returned with response {httpResponse}");
      }
    }

    private bool IsFileNotFound(string response)
    {
      try
      {
        if (!string.IsNullOrEmpty(response))
        {
          JObject json = JObject.Parse(response);
          if (json["error code"] != null && json["error code"].Value<string>().Equals("404"))
            return true;
        }
        return false;
      }
      catch (JsonReaderException)
      {
        return false;
      }
    }

    public StreamAndFileName GetXml(string fileUrl, bool throwException = true, HttpClient fakeHttpClientForTest = null)
    {
      var logger = LoggerCF.GetInstance();
      var httpResponse = new HttpResponseMessage();
      StreamAndFileName streamAndFileName = new StreamAndFileName();

      GetRetryPolicy(fileUrl).Execute(() =>
      {
        HttpClient httpClient = null;
        httpClient = (fakeHttpClientForTest == null) ? new HttpClient() : fakeHttpClientForTest;

        try
        {
          httpResponse = httpClient.GetAsync(fileUrl).Result;
          streamAndFileName = GetHttpResponseForGetXml(fileUrl, httpResponse);
        }
        catch (TaskCanceledException e)
        {
          logger.LogError($"Request to {fileUrl} failed, returned with TaskCanceledException");
          throw new HttpRequestException(e.Message);
        }
        catch (AggregateException e)
        {
          logger.LogError($"Request to {fileUrl} failed, returned with AggregateException");
          throw new HttpRequestException(e.Message);
        }
        finally
        {
          httpClient.Dispose();
        }
        

      });

      return streamAndFileName;
    }

    private StreamAndFileName GetHttpResponseForGetXml(string fileUrl, HttpResponseMessage httpResponse)
    {
      StreamAndFileName streamAndFileName = new StreamAndFileName();
      streamAndFileName.FileStream = httpResponse.Content.ReadAsStreamAsync().Result;
      streamAndFileName.FileName = httpResponse.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponse.IsSuccessStatusCode)
        return streamAndFileName;

      if (httpResponse.StatusCode == HttpStatusCode.BadGateway)
      {
        LoggerCF.GetInstance().LogError($"Request to {fileUrl} failed, returned with status code {httpResponse.StatusCode}::{ streamAndFileName.FileName}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }

      if (httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
      {
        LoggerCF.GetInstance().LogError($"Request to {fileUrl} failed, returned with status code {httpResponse.StatusCode}::{ streamAndFileName.FileName}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }
      if (!httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == HttpStatusCode.NotFound)
      {

        if (!IsFileNotFound(streamAndFileName.FileName))
        {
          LoggerCF.GetInstance().LogError($"Request to {fileUrl} failed, returned with status code {httpResponse.StatusCode}::{streamAndFileName.FileName}");
          throw new HttpRequestException(httpResponse.StatusCode.ToString());
        }
        else
        {
          LoggerCF.GetInstance().LogError($"Request to {fileUrl} failed, returned with status code {httpResponse.StatusCode}::{streamAndFileName.FileName}");
          throw new CmsResourceNotFoundException($"{fileUrl} not found, returned with response {httpResponse}");
        }

      }
      return streamAndFileName;
    }

    public Stream GetFileStream(XDocument xDocument)
    {
      MemoryStream fileStream = new MemoryStream();

      XmlWriter writer = XmlWriter.Create(fileStream);

      xDocument.Save(writer);

      writer.Close();
      writer.Flush();

      fileStream.Position = 0;

      string xmlData = Encoding.UTF8.GetString(fileStream.ToArray());
      xmlData = xmlData.Replace("&amp;#x", "&#x");

      fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlData));

      return fileStream;
    }

    public void PutToCms(string uri, XDocument xDocument, string fileName, bool throwException = true, HttpClient fakeHttpClientForTest = null)
    {
      var logger = LoggerCF.GetInstance();
      string res = string.Empty;
      var httpResponse = new HttpResponseMessage();
      GetRetryPolicy(uri).Execute(() =>
      {
        HttpClient httpClient = null;
        httpClient = (fakeHttpClientForTest == null) ? new HttpClient() : fakeHttpClientForTest;
        try
        {
          var content = new MultipartFormDataContent();
          content.Add(new StreamContent(GetFileStream(xDocument)), "file", fileName);

          httpResponse = httpClient.PutAsync(uri, content).Result;
          if (httpResponse.IsSuccessStatusCode)
          {
            res = httpResponse.Content.ReadAsStringAsync().Result;
            httpResponse.EnsureSuccessStatusCode();
          }
          else
          {
            ErrorHandlingForPutToCms(httpResponse, res, uri);
          }

        }
        catch (TaskCanceledException e)
        {
          logger.LogError($"Error while putting/saving to cms:: Request to {uri} failed, returned with TaskCanceledException");
          throw new HttpRequestException(e.Message);
        }
        catch (AggregateException e)
        {
          logger.LogError($"Error while putting/saving to cms:: Request to {uri} failed, returned with AggregateException");
          throw new HttpRequestException(e.Message);
        }
        finally
        {
          httpClient.Dispose();
        }
        
      });

   
    }

    private void ErrorHandlingForPutToCms(HttpResponseMessage httpResponse,string res,string uri)
    {
      if (httpResponse.StatusCode == HttpStatusCode.BadGateway)
      {
        LoggerCF.GetInstance().LogError($"Error while putting/saving to cms:: Request to {uri} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }

      if (httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
      {
        LoggerCF.GetInstance().LogError($"Error while putting/saving to cms::Request to {uri} failed, returned with status code {httpResponse.StatusCode}::{res}");
        throw new HttpRequestException(httpResponse.StatusCode.ToString());
      }
      if (!httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == HttpStatusCode.NotFound)
      {
        if (!IsFileNotFound(res))
        {
          LoggerCF.GetInstance().LogError($"Error while putting/saving to cms::Request to {uri} failed, returned with status code {httpResponse.StatusCode}::{res}");
          throw new HttpRequestException(httpResponse.StatusCode.ToString());
        }
        else
        {
          LoggerCF.GetInstance().LogError($"Error while putting/saving to cms:: Request to {uri} failed, returned with status code {httpResponse.StatusCode}::{res}");
          throw new CmsResourceNotFoundException($"{uri} not found, returned with response {httpResponse}");
        }
      }
    }

    public CmsRouteResource GetCmsRouteResource(JObject brokerMessage, string response, bool throwException = true, HttpClient fakeHttpClientForTest = null)
    {
      var resource = GetResourceAsync(response, throwException, fakeHttpClientForTest);

      if (resource == null || !resource._links.Any() && throwException)
      {
        LoggerCF.GetInstance().LogError("Inner Links are missing in " + response);
        return null;
      }

      return resource;
    }

    public string GetLink(CmsRouteResource resources, string rel, string method)
    {
      try
      {
        return resources._links.FirstOrDefault(l => l.rel.Equals(rel) && l.method.Equals(method)).href;
      }
      catch
      {
        return null;
      }
    }
  }
}
