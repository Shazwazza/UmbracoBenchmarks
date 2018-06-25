using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using BenchmarkDotNet.Attributes;

namespace UmbracoBenchmarks.Tools.Tests
{
    [OrderProvider(methodOrderPolicy: MethodOrderPolicy.Declared)]
    public class ContentTypeOperations : UmbracoOperation
    {
        private ApplicationContext _appCtx;
        private IContentType _existing;
        private List<IDataTypeDefinition> _dataTypes;
        private string _alias;

        public override void Setup()
        {
            base.Setup();

            _appCtx = ApplicationContext.Current;
            var allDts = _appCtx.Services.DataTypeService.GetAllDataTypeDefinitions().ToList();
            _dataTypes = new[] { "Label", "Textstring", "Richtext editor" }.Select(x =>
            {
                var dt = allDts.First(d => d.Name == x);
                if (dt == null) throw new InvalidOperationException($"No data type found by name {x}");
                return dt;
            }).ToList();

            _existing = CreateNew("test_" + Guid.NewGuid());
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _alias = "test_" + Guid.NewGuid();
        }

        private IContentType CreateNew(string alias)
        {
            var ct = new ContentType(-1)
            {
                Alias = alias,
                Name = alias,
                AllowedAsRoot = true
            };
            ct.AddPropertyGroup("test1");
            ct.AddPropertyGroup("test2");
            ct.AddPropertyGroup("test3");
            ct.AddPropertyType(new PropertyType(_dataTypes[0]) { Alias = "test1" }, "test1");
            ct.AddPropertyType(new PropertyType(_dataTypes[1]) { Alias = "test2" }, "test2");
            ct.AddPropertyType(new PropertyType(_dataTypes[2]) { Alias = "test3" }, "test3");
            _appCtx.Services.ContentTypeService.Save(ct);
            return ct;
        }

        [Benchmark]
        public void CreateContentType()
        {
            CreateNew(_alias);
        }

        [Benchmark]
        public void UpdateContentType()
        {
            var ct = _existing;
            string name1 = "test" + Guid.NewGuid();
            ct.AddPropertyGroup(name1);
            string name2 = "test2" + Guid.NewGuid();
            ct.AddPropertyGroup(name2);
            string name3 = "test3" + Guid.NewGuid();
            ct.AddPropertyGroup(name3);
            ct.AddPropertyType(new PropertyType(_dataTypes[0]) { Alias = name1 }, name1);
            ct.AddPropertyType(new PropertyType(_dataTypes[1]) { Alias = name2 }, name2);
            ct.AddPropertyType(new PropertyType(_dataTypes[2]) { Alias = name3 }, name3);
            _appCtx.Services.ContentTypeService.Save(ct);
            _existing = ct;
        }
    }


}
