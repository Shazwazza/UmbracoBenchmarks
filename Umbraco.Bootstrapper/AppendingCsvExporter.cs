using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBenchmarks.Tools
{
    public class AppendingCsvExporter : CsvExporter, IExporter
    {
        private bool _append;
        private readonly Guid runId;

        public AppendingCsvExporter(Guid runId) : base(CsvSeparator.CurrentCulture, SummaryStyle.Default)
        {
            this.runId = runId;
        }

        IEnumerable<string> IExporter.ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            string fileName = "Combined-" + runId.ToString("N");
            string filePath = GetArtifactFullName(summary, fileName); 

            _append = File.Exists(filePath);

            using (var stream = new StreamWriter(filePath, append: _append))
            {
                ExportToLog(summary, new StreamLogger(stream));
            }

            return new[] { filePath };
        }

        internal string GetArtifactFullName(Summary summary, string fileName)
        {
            return $"{Path.Combine(summary.ResultsDirectoryPath, fileName)}-{FileCaption}{FileNameSuffix}.{FileExtension}";
        }

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            string realSeparator = CsvSeparator.CurrentCulture.ToRealSeparator();

            foreach (var line in _append ? summary.Table.FullContent: summary.Table.FullContentWithHeader)
            {
                for (int i = 0; i < line.Length;)
                {
                    logger.Write(CsvHelper.Escape(line[i], realSeparator));

                    if (++i < line.Length)
                    {
                        logger.Write(realSeparator);
                    }
                }

                logger.WriteLine();
            }
        }
    }
}
