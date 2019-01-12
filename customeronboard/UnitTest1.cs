using System;
using System.Net;
using RestSharp;
using Xunit;

namespace customeronboard
{
    public class UnitTest1
    {
        private const string baseURL = "https://secret-escarpment-70468.herokuapp.com";

        [Fact]
        public void GetConfig_ForTenant()
        {
            Random r = new Random();
            int id = r.Next(10, 100);
            RestClient cleint = new RestClient(baseURL);
            RestRequest req = new RestRequest($"/tenants/{id}/config", Method.GET);
            RestResponse resp = cleint.Get(req) as RestResponse;

            Assert.Equal(ResponseStatus.Completed, resp.ResponseStatus);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var respcontent = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp.Content);
            Assert.Equal(respcontent.entity.name.ToString(), $"Tenant_{id}");
            Assert.Equal(respcontent.entity.relationship.ToString(), $"customer_{id}");
        }

        [Fact]
        public void UploadTemplate_ValidResponse()
        {
            RestClient cleint = new RestClient(baseURL);
            RestRequest req = new RestRequest("/customers/upload", Method.POST);
            req.AddHeader("mediatype", "multipart");
            req.AddHeader("content-type", "application/json");
            req.AddJsonBody("{\"test\":\"TestExcel upload\"}");
            RestResponse resp = cleint.Post(req) as RestResponse;

            Assert.Equal(ResponseStatus.Completed, resp.ResponseStatus);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var respcontent = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp.Content);
            Assert.Equal(respcontent.message.ToString(), "Uploaded successfully");
        }

        [Fact]
        public void UploadTemplate_CheckMediaType()
        {
            RestClient cleint = new RestClient(baseURL);
            RestRequest req = new RestRequest("/customers/upload", Method.POST);
            req.AddHeader("content-type", "application/json");
            req.AddJsonBody("{\"test\":\"TestExcel upload\"}");
            RestResponse resp = cleint.Post(req) as RestResponse;

            Assert.Equal(ResponseStatus.Completed, resp.ResponseStatus);
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, resp.StatusCode);

            var respcontent = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp.Content);
            Assert.Equal(respcontent.message.ToString(), "Unsupported media type");
        }

        [Fact]
        public void UploadTemplate_CheckInvalidExcel()
        {
            RestClient cleint = new RestClient(baseURL);
            RestRequest req = new RestRequest("/customers/upload", Method.POST);
            req.AddHeader("mediatype", "multipart");
            req.AddJsonBody("{\"test\":\"TestExcel\"}");
            RestResponse resp = cleint.Post(req) as RestResponse;

            Assert.Equal(ResponseStatus.Completed, resp.ResponseStatus);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

            var respcontent = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp.Content);
            Assert.Equal(respcontent.message.ToString(), "Invalid excel");
        }

        [Fact]
        public void UploadTemplate_CheckInvalidExcel_NoPostData()
        {
            RestClient cleint = new RestClient(baseURL);
            RestRequest req = new RestRequest("/customers/upload", Method.POST);
            req.AddHeader("mediatype", "multipart");
            req.AddHeader("content-type", "application/json");
            RestResponse resp = cleint.Post(req) as RestResponse;

            Assert.Equal(ResponseStatus.Completed, resp.ResponseStatus);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

            var respcontent = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp.Content);
            Assert.Equal(respcontent.message.ToString(), "Invalid excel");
        }
    }
}
