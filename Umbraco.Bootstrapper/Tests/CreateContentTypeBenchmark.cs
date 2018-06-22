﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
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

namespace UmbracoBenchmarks.Tools.Tests
{

    public class CreateContentTypeBenchmark
    {
        private ApplicationContext _appCtx;
        private List<IDataTypeDefinition> _dataTypes;
        private string _alias;

        [GlobalSetup]
        public void Setup()
        {
            _appCtx = ApplicationContext.Current;
            var allDts = _appCtx.Services.DataTypeService.GetAllDataTypeDefinitions().ToList();
            _dataTypes = new[] { "Label", "Textstring", "Richtext editor" }.Select(x =>
            {
                var dt = allDts.First(d => d.Name == x);
                if (dt == null) throw new InvalidOperationException($"No data type found by name {x}");
                return dt;
            }).ToList();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _alias = "test_" + Guid.NewGuid();
        }

        [Benchmark]
        public void Run()
        {   
            var ct = new ContentType(-1)
            {
                Alias = _alias,
                Name = _alias,
                AllowedAsRoot = true
            };
            ct.AddPropertyGroup("test1");
            ct.AddPropertyGroup("test2");
            ct.AddPropertyGroup("test3");
            ct.AddPropertyType(new PropertyType(_dataTypes[0]) { Alias = "test1" }, "test1");
            ct.AddPropertyType(new PropertyType(_dataTypes[1]) { Alias = "test2" }, "test2");
            ct.AddPropertyType(new PropertyType(_dataTypes[2]) { Alias = "test3" }, "test3");
            _appCtx.Services.ContentTypeService.Save(ct);
        }
    }


}
