// DGrok Delphi parser
// Copyright (C) 2007 Joe White
// http://www.excastle.com/dgrok
//
// Licensed under the Open Software License version 3.0
// http://www.opensource.org/licenses/osl-3.0.php
using System;
using System.Collections.Generic;
using System.Text;
using DGrok.Framework;
using NUnitLite.Framework;

namespace DGrok.Tests
{
    [TestFixture]
    public class MethodOrPropertyTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.MethodOrProperty; }
        }

        public void TestMethod()
        {
            Assert.That("procedure Foo;", ParsesAs(
                "MethodHeadingNode",
                "  Class: (none)",
                "  MethodType: ProcedureKeyword |procedure|",
                "  Name: Identifier |Foo|",
                "  OpenParenthesis: (none)",
                "  ParameterList: ListNode",
                "  CloseParenthesis: (none)",
                "  Colon: (none)",
                "  ReturnType: (none)",
                "  Semicolon: Semicolon |;|",
                "  DirectiveList: ListNode"));
        }
        public void TestClassMethod()
        {
            Assert.That("class procedure Foo;", ParsesAs(
                "MethodHeadingNode",
                "  Class: ClassKeyword |class|",
                "  MethodType: ProcedureKeyword |procedure|",
                "  Name: Identifier |Foo|",
                "  OpenParenthesis: (none)",
                "  ParameterList: ListNode",
                "  CloseParenthesis: (none)",
                "  Colon: (none)",
                "  ReturnType: (none)",
                "  Semicolon: Semicolon |;|",
                "  DirectiveList: ListNode"));
        }
        public void TestProperty()
        {
            Assert.That("property Foo: Integer read FFoo;", ParsesAs(
                "PropertyNode",
                "  Class: (none)",
                "  Property: PropertyKeyword |property|",
                "  Name: Identifier |Foo|",
                "  OpenBracket: (none)",
                "  ParameterList: ListNode",
                "  CloseBracket: (none)",
                "  Colon: Colon |:|",
                "  Type: Identifier |Integer|",
                "  Index: (none)",
                "  IndexValue: (none)",
                "  Read: ReadSemikeyword |read|",
                "  ReadSpecifier: Identifier |FFoo|",
                "  Write: (none)",
                "  WriteSpecifier: (none)",
                "  Stored: (none)",
                "  StoredSpecifier: (none)",
                "  Default: (none)",
                "  DefaultValue: (none)",
                "  Implements: (none)",
                "  ImplementsSpecifier: (none)",
                "  Semicolon: Semicolon |;|"));
        }
        public void TestClassProperty()
        {
            Assert.That("class property Foo: Integer read FFoo;", ParsesAs(
                "PropertyNode",
                "  Class: ClassKeyword |class|",
                "  Property: PropertyKeyword |property|",
                "  Name: Identifier |Foo|",
                "  OpenBracket: (none)",
                "  ParameterList: ListNode",
                "  CloseBracket: (none)",
                "  Colon: Colon |:|",
                "  Type: Identifier |Integer|",
                "  Index: (none)",
                "  IndexValue: (none)",
                "  Read: ReadSemikeyword |read|",
                "  ReadSpecifier: Identifier |FFoo|",
                "  Write: (none)",
                "  WriteSpecifier: (none)",
                "  Stored: (none)",
                "  StoredSpecifier: (none)",
                "  Default: (none)",
                "  DefaultValue: (none)",
                "  Implements: (none)",
                "  ImplementsSpecifier: (none)",
                "  Semicolon: Semicolon |;|"));
        }
    }
}
