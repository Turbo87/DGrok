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
using System.Text;
using DGrok.DelphiNodes;
using DGrok.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace DGrok.Tests
{
    [TestFixture]
    public class CodeBaseTests
    {
        private CodeBase _codeBase;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase(CompilerDefines.CreateEmpty(), new MemoryFileLoader());
        }

        [Test]
        public void AddError()
        {
            _codeBase.AddError(@"C:\Foo.pas", new Exception("Oops"));
            Assert.That(_codeBase.ErrorCount, Is.EqualTo(1));
        }
        [Test]
        public void AddFileThatIsValidUnit()
        {
            _codeBase.AddFile("Foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1));
            Assert.That(_codeBase.UnitCount, Is.EqualTo(1));
        }
        [Test]
        public void AddFileThatIsValidProject()
        {
            _codeBase.AddFile("Foo.dpr", "program Foo; end.");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1));
            Assert.That(_codeBase.UnitCount, Is.EqualTo(0));
        }
        [Test]
        public void AddFileWithError()
        {
            _codeBase.AddFile("Foo.pas", "");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(0));
            Assert.That(_codeBase.ErrorCount, Is.EqualTo(1));
        }
        [Test]
        public void AddFileExpectingSuccessWithSuccess()
        {
            _codeBase.AddFileExpectingSuccess("Foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1));
        }
        [Test, ExpectedException(typeof(ParseException))]
        public void AddFileExpectingSuccessWithError()
        {
            _codeBase.AddFileExpectingSuccess("Foo.pas", "");
        }
        [Test]
        public void AddParsedFile()
        {
            AstNode parseTree = new Token(TokenType.EndKeyword, null, "end", "");
            _codeBase.AddParsedFile("Foo.pas", "end", parseTree);
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1));
        }
        [Test]
        public void ErrorByFileName()
        {
            Exception error = new Exception("Oops");
            _codeBase.AddError("Foo.pas", error);
            Assert.That(_codeBase.ErrorByFileName("Foo.pas"), Is.SameAs(error));
        }
        [Test]
        public void ParsedFileByFileNameWithUnit()
        {
            _codeBase.AddFile("Foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.ParsedFileByFileName("Foo.pas"), Is.Not.Null);
        }
        [Test]
        public void ParsedFileByFileNameWithProject()
        {
            _codeBase.AddFile("Foo.dpr", "program Foo; end.");
            Assert.That(_codeBase.ParsedFileByFileName("Foo.dpr"), Is.Not.Null);
        }
        [Test]
        public void UnitByName()
        {
            _codeBase.AddFile("Foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.UnitByName("Foo"), Is.Not.Null);
        }
        [Test]
        public void UnitsGoIntoUnitList()
        {
            _codeBase.AddFile("Foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.UnitCount, Is.EqualTo(1), "UnitCount");
            Assert.That(_codeBase.ProjectCount, Is.EqualTo(0), "ProjectCount");
        }
        [Test]
        public void ProgramsGoIntoProjectList()
        {
            _codeBase.AddFile("Foo.pas", "program Foo; end.");
            Assert.That(_codeBase.UnitCount, Is.EqualTo(0), "UnitCount");
            Assert.That(_codeBase.ProjectCount, Is.EqualTo(1), "ProjectCount");
        }
        [Test]
        public void LibrariesGoIntoProjectList()
        {
            _codeBase.AddFile("Foo.pas", "library Foo; end.");
            Assert.That(_codeBase.UnitCount, Is.EqualTo(0), "UnitCount");
            Assert.That(_codeBase.ProjectCount, Is.EqualTo(1), "ProjectCount");
        }
        [Test]
        public void PackagesGoIntoProjectList()
        {
            _codeBase.AddFile("Foo.pas", "package Foo; end.");
            Assert.That(_codeBase.UnitCount, Is.EqualTo(0), "UnitCount");
            Assert.That(_codeBase.ProjectCount, Is.EqualTo(1), "ProjectCount");
        }
        [Test]
        public void UnitsRememberOriginalPath()
        {
            _codeBase.AddFile(@"C:\Foo.pas", "unit Foo; interface implementation end.");
            List<NamedContent<UnitNode>> units = new List<NamedContent<UnitNode>>(_codeBase.Units);
            Assert.That(units.Count, Is.EqualTo(1));
            Assert.That(units[0].Name, Is.EqualTo("Foo"));
            Assert.That(units[0].FileName, Is.EqualTo(@"C:\Foo.pas"));
        }
        [Test]
        public void ProjectsRememberOriginalPath()
        {
            _codeBase.AddFile(@"C:\Foo.dpr", "program Foo; end.");
            List<NamedContent<AstNode>> projects = new List<NamedContent<AstNode>>(_codeBase.Projects);
            Assert.That(projects.Count, Is.EqualTo(1));
            Assert.That(projects[0].Name, Is.EqualTo("Foo"));
            Assert.That(projects[0].FileName, Is.EqualTo(@"C:\Foo.dpr"));
        }
        [Test]
        public void ErrorOnDuplicateUnitName()
        {
            _codeBase.AddFile(@"C:\Dir1\Foo.pas", "unit Foo; interface implementation end.");
            _codeBase.AddFile(@"C:\Dir2\foo.pas", "unit Foo; interface implementation end.");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1), "ParsedFileCount");
            List<NamedContent<Exception>> errors = new List<NamedContent<Exception>>(_codeBase.Errors);
            Assert.That(errors.Count, Is.EqualTo(1), "Error count");
            Assert.That(errors[0].Content, Is.InstanceOfType(typeof(DuplicateFileNameException)));
            Assert.That(errors[0].Content.Message, Is.EqualTo(
                @"File 'C:\Dir2\foo.pas' has the same name as 'C:\Dir1\Foo.pas'"));
        }
        [Test]
        public void ErrorOnDuplicateProjectName()
        {
            _codeBase.AddFile(@"C:\Dir1\Foo.dpr", "program Foo; end.");
            _codeBase.AddFile(@"C:\Dir2\foo.dpr", "library Foo; end.");
            Assert.That(_codeBase.ParsedFileCount, Is.EqualTo(1), "ParsedFileCount");
            List<NamedContent<Exception>> errors = new List<NamedContent<Exception>>(_codeBase.Errors);
            Assert.That(errors.Count, Is.EqualTo(1), "Error count");
            Assert.That(errors[0].Content, Is.InstanceOfType(typeof(DuplicateFileNameException)));
            Assert.That(errors[0].Content.Message, Is.EqualTo(
                @"File 'C:\Dir2\foo.dpr' has the same name as 'C:\Dir1\Foo.dpr'"));
        }
    }
}
