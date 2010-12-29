// Copyright 2007, 2008 Joe White
//
// This file is part of DGrok <http://www.excastle.com/dgrok/>.
//
// DGrok is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// DGrok is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with DGrok.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DGrok.DelphiNodes;

namespace DGrok.Framework
{
    public class CodeBase
    {
        private CompilerDefines _compilerDefines;
        private Dictionary<string, NamedContent<Exception>> _errors;
        private IFileLoader _fileLoader;
        private object _mutex = new object();
        private Dictionary<string, NamedContent<AstNode>> _parsedFiles;
        private TimeSpan _parseDuration;
        private Dictionary<string, NamedContent<AstNode>> _projects;
        private Dictionary<string, NamedContent<UnitNode>> _units;

        public CodeBase(CompilerDefines compilerDefines, IFileLoader fileLoader)
        {
            _compilerDefines = compilerDefines;
            _fileLoader = fileLoader;
            _errors = new Dictionary<string, NamedContent<Exception>>(StringComparer.InvariantCultureIgnoreCase);
            _parsedFiles = new Dictionary<string, NamedContent<AstNode>>(StringComparer.InvariantCultureIgnoreCase);
            _projects = new Dictionary<string, NamedContent<AstNode>>(StringComparer.InvariantCultureIgnoreCase);
            _units = new Dictionary<string, NamedContent<UnitNode>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public int ErrorCount
        {
            get { return _errors.Count; }
        }
        public IEnumerable<NamedContent<Exception>> Errors
        {
            get { return SortedPairs(_errors); }
        }
        public int ParsedFileCount
        {
            get { return _parsedFiles.Count; }
        }
        public IList<string> ParsedFileNames
        {
            get { return SortedKeys(_parsedFiles); }
        }
        public IEnumerable<NamedContent<AstNode>> ParsedFiles
        {
            get { return SortedPairs(_parsedFiles); }
        }
        public TimeSpan ParseDuration
        {
            get { return _parseDuration; }
            set { _parseDuration = value; }
        }
        public int ProjectCount
        {
            get { return _projects.Count; }
        }
        public IEnumerable<NamedContent<AstNode>> Projects
        {
            get { return SortedPairs(_projects); }
        }
        public int UnitCount
        {
            get { return _units.Count; }
        }
        public IEnumerable<NamedContent<UnitNode>> Units
        {
            get { return SortedPairs(_units); }
        }

        public void AddError(string fileName, Exception exception)
        {
            NamedContent<Exception> namedError = new NamedContent<Exception>(fileName, exception);
            lock (_mutex)
                _errors.Add(namedError.FileName, namedError);
        }
        public void AddFile(string fileName, string text)
        {
            try
            {
                AddFileExpectingSuccess(fileName, text);
            }
            catch (IOException ex)
            {
                AddError(fileName, ex);
            }
            catch (DGrokException ex)
            {
                AddError(fileName, ex);
            }
        }
        public void AddFileExpectingSuccess(string fileName, string text)
        {
            Parser parser = Parser.FromText(text, fileName, _compilerDefines, _fileLoader);
            AstNode parseTree = parser.ParseRule(RuleType.Goal);
            AddParsedFile(fileName, text, parseTree);
        }
        public void AddParsedFile(string fileName, string fileSource, AstNode parseTree)
        {
            NamedContent<AstNode> namedParseTree = new NamedContent<AstNode>(fileName, parseTree);
            UnitNode unit = parseTree as UnitNode;
            if (unit != null)
                AddParsedFileToCollection(_units, fileName, fileSource, unit);
            else
                AddParsedFileToCollection(_projects, fileName, fileSource, parseTree);
        }
        private void AddParsedFileToCollection<T>(Dictionary<string, NamedContent<T>> collection,
            string fileName, string fileSource, T content)
            where T : AstNode
        {
            NamedContent<T> namedContent = new NamedContent<T>(fileName, content);
            NamedContent<AstNode> baseContent = new NamedContent<AstNode>(fileName, content);
            string existingName;
            lock (_mutex)
            {
                if (!collection.ContainsKey(namedContent.Name))
                {
                    collection.Add(namedContent.Name, namedContent);
                    _parsedFiles.Add(fileName, baseContent);
                    return;
                }
                else
                    existingName = collection[namedContent.Name].FileName;
            }
            string message = "File '" + fileName + "' has the same name as '" + existingName + "'";
            Location location = new Location(fileName, fileSource, 0);
            throw new DuplicateFileNameException(message, location);
        }
        public Exception ErrorByFileName(string fileName)
        {
            return _errors[fileName].Content;
        }
        public AstNode ParsedFileByFileName(string fileName)
        {
            return _parsedFiles[fileName].Content;
        }
        private IList<string> SortedKeys<T>(IDictionary<string, T> dictionary)
        {
            List<string> keys = new List<string>(dictionary.Keys);
            keys.Sort(StringComparer.InvariantCultureIgnoreCase);
            return keys;
        }
        private IEnumerable<T> SortedPairs<T>(IDictionary<string, T> dictionary)
        {
            foreach (string key in SortedKeys(dictionary))
                yield return dictionary[key];
        }
        public UnitNode UnitByName(string name)
        {
            return _units[name].Content;
        }
    }
}
