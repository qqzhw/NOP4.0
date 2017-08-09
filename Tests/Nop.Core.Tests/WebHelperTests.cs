﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Nop.Core.Configuration;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class WebHelperTests
    {
        private DefaultHttpContext _httpContext;
        private IWebHelper _webHelper;

        [SetUp]
        public void SetUp()
        {
            _httpContext = new DefaultHttpContext();
            var queryString = new QueryString("");
            queryString = queryString.Add("Key1", "Value1");
            queryString = queryString.Add("Key2", "Value2");
            _httpContext.Request.QueryString = queryString;
            _httpContext.Request.Headers.Add(HeaderNames.Host, "www.Example.com");
            _webHelper = new WebHelper(new HostingConfig(), new FakeHttpContextAccessor(_httpContext));
        }

        [Test]
        public void Can_get_storeHost_without_ssl()
        {
            _webHelper.GetStoreHost(false).ShouldEqual("http://www.example.com/");
        }

        [Test]
        public void Can_get_storeHost_with_ssl()
        {
            _webHelper.GetStoreHost(true).ShouldEqual("https://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_without_ssl()
        {
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_with_ssl()
        {
            _webHelper.GetStoreLocation(true).ShouldEqual("https://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_in_virtual_directory()
        {
            _httpContext.Request.PathBase = "/nopCommercepath";
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/nopcommercepath/");
        }

        [Test]
        public void Get_storeLocation_should_return_lowerCased_result()
        {
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/");
        }

        [Test]
        public void Can_get_queryString()
        {
            _webHelper.QueryString<string>("Key1").ShouldEqual("Value1");
            _webHelper.QueryString<string>("Key2").ShouldEqual("Value2");
            _webHelper.QueryString<string>("Key3").ShouldEqual(null);
        }

        [Test]
        public void Can_remove_queryString()
        {
            //first param (?)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param1")
                .ShouldEqual("http://www.example.com/?param2=value2");
            //second param (&)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param2")
                .ShouldEqual("http://www.example.com/?param1=value1");
            //non-existing param
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param3")
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value2");
        }

        [Test]
        public void Can_remove_queryString_should_return_lowerCased_result()
        {
            _webHelper.RemoveQueryString("htTp://www.eXAmple.com/?param1=value1&parAm2=value2", "paRAm1")
                .ShouldEqual("http://www.example.com/?param2=value2");
        }

        [Test]
        public void Can_remove_queryString_should_ignore_input_parameter_case()
        {
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&parAm2=value2", "paRAm1")
                .ShouldEqual("http://www.example.com/?param2=value2");
        }

        [Test]
        public void Can_modify_queryString()
        {
            //first param (?)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param1=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2");
            //second param (&)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param2=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value3");
            //non-existing param
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param3=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value2&param3=value3");
        }

        [Test]
        public void Can_modify_queryString_with_anchor()
        {
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param1=value3", "Test")
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2#test");
        }

        [Test]
        public void Can_modify_queryString_new_anchor_should_remove_previous_one()
        {
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2#test1", "param1=value3", "Test2")
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2#test2");
        }
    }

    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public FakeHttpContextAccessor(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; set; }
    }
}
