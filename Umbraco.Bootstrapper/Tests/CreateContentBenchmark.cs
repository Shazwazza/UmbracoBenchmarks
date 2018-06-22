using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class CreateContentBenchmark
    {
        private ApplicationContext _appCtx;
        private IContentType _contentType;
        private string _alias;

        [GlobalSetup]
        public void Setup()
        {
            _appCtx = ApplicationContext.Current;
            _contentType = _appCtx.Services.ContentTypeService.GetAllContentTypes().First();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _alias = "test_" + Guid.NewGuid();
        }

        [Benchmark]
        public void Run()
        {   
            var c = new Content(_alias, -1, _contentType);
            foreach (var p in _contentType.PropertyTypes) 
            {
                c.SetPropertyValue(p.Alias, Guid.NewGuid().ToString());
            }
            _appCtx.Services.ContentService.Save(c);
        }
    }


}
