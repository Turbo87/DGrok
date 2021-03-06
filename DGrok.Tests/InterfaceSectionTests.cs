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
using DGrok.Framework;
using NUnit.Framework;

namespace DGrok.Tests
{
    [TestFixture]
    public class InterfaceSectionTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.InterfaceSection; }
        }

        [Test]
        public void EmptyInterfaceSection()
        {
            Assert.That("interface", ParsesAs(
                "UnitSectionNode",
                "  HeaderKeywordNode: InterfaceKeyword |interface|",
                "  UsesClauseNode: (none)",
                "  ContentListNode: ListNode"));
        }
        [Test]
        public void Uses()
        {
            Assert.That("interface uses Foo;", ParsesAs(
                "UnitSectionNode",
                "  HeaderKeywordNode: InterfaceKeyword |interface|",
                "  UsesClauseNode: UsesClauseNode",
                "    UsesKeywordNode: UsesKeyword |uses|",
                "    UnitListNode: ListNode",
                "      Items[0]: DelimitedItemNode",
                "        ItemNode: UsedUnitNode",
                "          NameNode: Identifier |Foo|",
                "          InKeywordNode: (none)",
                "          FileNameNode: (none)",
                "        DelimiterNode: (none)",
                "    SemicolonNode: Semicolon |;|",
                "  ContentListNode: ListNode"));
        }
        [Test]
        public void WithContents()
        {
            Assert.That("interface resourcestring Foo = 'Bar';", ParsesAs(
                "UnitSectionNode",
                "  HeaderKeywordNode: InterfaceKeyword |interface|",
                "  UsesClauseNode: (none)",
                "  ContentListNode: ListNode",
                "    Items[0]: ConstSectionNode",
                "      ConstKeywordNode: ResourceStringKeyword |resourcestring|",
                "      ConstListNode: ListNode",
                "        Items[0]: ConstantDeclNode",
                "          NameNode: Identifier |Foo|",
                "          ColonNode: (none)",
                "          TypeNode: (none)",
                "          EqualSignNode: EqualSign |=|",
                "          ValueNode: StringLiteral |'Bar'|",
                "          PortabilityDirectiveListNode: ListNode",
                "          SemicolonNode: Semicolon |;|"));
        }
    }
}
