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
    public class SetLiteralTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.SetLiteral; }
        }

        [Test]
        public void EmptySet()
        {
            Assert.That("[]", ParsesAs(
                "SetLiteralNode",
                "  OpenBracketNode: OpenBracket |[|",
                "  ItemListNode: ListNode",
                "  CloseBracketNode: CloseBracket |]|"));
        }
        [Test]
        public void OneValue()
        {
            Assert.That("[42]", ParsesAs(
                "SetLiteralNode",
                "  OpenBracketNode: OpenBracket |[|",
                "  ItemListNode: ListNode",
                "    Items[0]: DelimitedItemNode",
                "      ItemNode: Number |42|",
                "      DelimiterNode: (none)",
                "  CloseBracketNode: CloseBracket |]|"));
        }
        [Test]
        public void TwoRanges()
        {
            Assert.That("[1..2, 4..5]", ParsesAs(
                "SetLiteralNode",
                "  OpenBracketNode: OpenBracket |[|",
                "  ItemListNode: ListNode",
                "    Items[0]: DelimitedItemNode",
                "      ItemNode: BinaryOperationNode",
                "        LeftNode: Number |1|",
                "        OperatorNode: DotDot |..|",
                "        RightNode: Number |2|",
                "      DelimiterNode: Comma |,|",
                "    Items[1]: DelimitedItemNode",
                "      ItemNode: BinaryOperationNode",
                "        LeftNode: Number |4|",
                "        OperatorNode: DotDot |..|",
                "        RightNode: Number |5|",
                "      DelimiterNode: (none)",
                "  CloseBracketNode: CloseBracket |]|"));
        }
    }
}
