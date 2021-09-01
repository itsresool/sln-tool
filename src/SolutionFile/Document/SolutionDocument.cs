using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolutionFile.Document.Sections;

namespace SolutionFile.Document
{
    public class SolutionDocument
    {
        public static Guid FolderTypeId = new("2150E333-8FDC-42A3-9474-1A3956D46DE8");
        private readonly string _lineEnding;
        private readonly bool _shouldEmitByteOrderMark;

        public SolutionDocument(string lineEnding, bool shouldEmitByteOrderMark)
        {
            _lineEnding = lineEnding;
            _shouldEmitByteOrderMark = shouldEmitByteOrderMark;
        }

        public List<IDocumentSection> Sections { get; } = new();

        public void AddFileToFolder(string folder, string file)
        {
            if (!FolderExists(folder)) throw new ArgumentException("Folder could not be found");

            var itemsSection =
                (SolutionItems)Sections.First(s => s is SolutionItems sItems && sItems.FolderName == folder);
            itemsSection.Elements.Add(file, file);
        }

        public void RemoveFileToFolder(string folder, string file)
        {
        }

        public void SaveToFile(string filePath)
        {
            var rawLines = Sections.SelectMany(s => s.ToRawLines());

            var rawDocBuilder = new StringBuilder();
            rawDocBuilder.AppendJoin(_lineEnding, rawLines);
            rawDocBuilder.Append(_lineEnding);

            var encoding = new UTF8Encoding(_shouldEmitByteOrderMark);
            File.WriteAllText(filePath, rawDocBuilder.ToString(), encoding);
        }

        private bool FolderExists(string name)
        {
            return Sections.Any(s => s is ProjectBodyHeader project && project.Name == name);
        }
    }
}
