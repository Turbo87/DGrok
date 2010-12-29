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
    public class ClassHelperTypeTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.ClassHelperType; }
        }

        public void TestSimple()
        {
            Assert.That("class helper for TObject end", ParsesAs(
                "TypeHelperNode",
                "  TypeKeywordNode: ClassKeyword |class|",
                "  HelperSemikeywordNode: HelperSemikeyword |helper|",
                "  OpenParenthesisNode: (none)",
                "  BaseHelperTypeNode: (none)",
                "  CloseParenthesisNode: (none)",
                "  ForKeywordNode: ForKeyword |for|",
                "  TypeNode: Identifier |TObject|",
                "  ContentListNode: ListNode",
                "  EndKeywordNode: EndKeyword |end|"));
        }
        public void TestDescended()
        {
            Assert.That("class helper (TFooHelper) for TObject end", ParsesAs(
                "TypeHelperNode",
                "  TypeKeywordNode: ClassKeyword |class|",
                "  HelperSemikeywordNode: HelperSemikeyword |helper|",
                "  OpenParenthesisNode: OpenParenthesis |(|",
                "  BaseHelperTypeNode: Identifier |TFooHelper|",
                "  CloseParenthesisNode: CloseParenthesis |)|",
                "  ForKeywordNode: ForKeyword |for|",
                "  TypeNode: Identifier |TObject|",
                "  ContentListNode: ListNode",
                "  EndKeywordNode: EndKeyword |end|"));
        }
        public void TestMethod()
        {
            Assert.That("class helper for TObject procedure Foo; end", ParsesAs(
                "TypeHelperNode",
                "  TypeKeywordNode: ClassKeyword |class|",
                "  HelperSemikeywordNode: HelperSemikeyword |helper|",
                "  OpenParenthesisNode: (none)",
                "  BaseHelperTypeNode: (none)",
                "  CloseParenthesisNode: (none)",
                "  ForKeywordNode: ForKeyword |for|",
                "  TypeNode: Identifier |TObject|",
                "  ContentListNode: ListNode",
                "    Items[0]: VisibilitySectionNode",
                "      VisibilityNode: (none)",
                "      ContentListNode: ListNode",
                "        Items[0]: MethodHeadingNode",
                "          ClassKeywordNode: (none)",
                "          MethodTypeNode: ProcedureKeyword |procedure|",
                "          NameNode: Identifier |Foo|",
                "          OpenParenthesisNode: (none)",
                "          ParameterListNode: ListNode",
                "          CloseParenthesisNode: (none)",
                "          ColonNode: (none)",
                "          ReturnTypeNode: (none)",
                "          DirectiveListNode: ListNode",
                "          SemicolonNode: Semicolon |;|",
                "  EndKeywordNode: EndKeyword |end|"));
        }
    }
}
