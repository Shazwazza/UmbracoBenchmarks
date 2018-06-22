using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbracoBenchmarks.Tools.Tests
{
    public class CreateContent
    {


        public static void Execute(ApplicationContext appCtx)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.Write("Creating content... ");

                var allDts = appCtx.Services.DataTypeService.GetAllDataTypeDefinitions().ToList();
                var dts = new[] { "Label", "Textstring", "Richtext editor" }.Select(x =>
                {
                    var dt = allDts.First(d => d.Name == x);
                    if (dt == null) throw new InvalidOperationException($"No data type found by name {x}");
                    return dt;
                }).ToList();

                var ct = new ContentType(-1)
                {
                    Alias = "home",
                    Name = "Home",
                    AllowedAsRoot = true
                };
                ct.AddPropertyGroup("test1");
                ct.AddPropertyGroup("test2");
                ct.AddPropertyGroup("test3");
                ct.AddPropertyType(new PropertyType(dts[0]) { Alias = "test1" }, "test1");
                ct.AddPropertyType(new PropertyType(dts[1]) { Alias = "test2" }, "test2");
                ct.AddPropertyType(new PropertyType(dts[2]) { Alias = "test3" }, "test3");
                appCtx.Services.ContentTypeService.Save(ct);

                Console.WriteLine("OK");
                Console.WriteLine($"Total content types: {appCtx.Services.ContentTypeService.GetAllContentTypes().Count()}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }


}
