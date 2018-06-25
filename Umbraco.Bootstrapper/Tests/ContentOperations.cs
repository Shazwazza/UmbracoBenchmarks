using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbracoBenchmarks.Tools.Tests
{

    public class ContentOperations : UmbracoOperation
    {
        private ApplicationContext _appCtx;
        private IContentType _contentType;
        private IContent _existing;
        private string _alias;

        public override void Setup()
        {
            base.Setup();
            _appCtx = ApplicationContext.Current;
            _contentType = _appCtx.Services.ContentTypeService.GetAllContentTypes().First();
            _existing = CreateContent("test_" + Guid.NewGuid());
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _alias = "test_" + Guid.NewGuid();
        }

        private IContent CreateContent(string alias)
        {
            var c = new Content(alias, -1, _contentType);
            foreach (var p in _contentType.PropertyTypes)
            {
                c.SetValue(p.Alias, Guid.NewGuid().ToString());
            }
            _appCtx.Services.ContentService.Save(c);
            return c;
        }

        [Benchmark]
        public void CreateContent()
        {
            var c = CreateContent(_alias);
        }

        [Benchmark]
        public void UpdateContent()
        {
            var c = _existing;
            foreach (var p in _contentType.PropertyTypes)
            {
                c.SetValue(p.Alias, Guid.NewGuid().ToString());
            }
            _appCtx.Services.ContentService.Save(c);
        }
    }


}
