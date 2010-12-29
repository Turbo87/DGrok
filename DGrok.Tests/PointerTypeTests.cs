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
    public class PointerTypeTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.PointerType; }
        }

        [Test]
        public void Identifier()
        {
            Assert.That("^TFoo", ParsesAs(
                "PointerTypeNode",
                "  CaretNode: Caret |^|",
                "  TypeNode: Identifier |TFoo|"));
        }
        [Test]
        public void Qualified()
        {
            Assert.That("^Foo.TBar", ParsesAs(
                "PointerTypeNode",
                "  CaretNode: Caret |^|",
                "  TypeNode: BinaryOperationNode",
                "    LeftNode: Identifier |Foo|",
                "    OperatorNode: Dot |.|",
                "    RightNode: Identifier |TBar|"));
        }
        [Test]
        public void String()
        {
            Assert.That("^string", ParsesAs(
                "PointerTypeNode",
                "  CaretNode: Caret |^|",
                "  TypeNode: StringKeyword |string|"));
        }
    }
}
