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
    public class RelOpTests : ParserTestCase
    {
        protected override RuleType RuleType
        {
            get { return RuleType.RelOp; }
        }

        [Test]
        public void EqualSign()
        {
            Assert.That("=", ParsesAs("EqualSign |=|"));
        }
        [Test]
        public void GreaterThan()
        {
            Assert.That(">", ParsesAs("GreaterThan |>|"));
        }
        [Test]
        public void LessThan()
        {
            Assert.That("<", ParsesAs("LessThan |<|"));
        }
        [Test]
        public void LessOrEqual()
        {
            Assert.That("<=", ParsesAs("LessOrEqual |<=|"));
        }
        [Test]
        public void GreaterOrEqual()
        {
            Assert.That(">=", ParsesAs("GreaterOrEqual |>=|"));
        }
        [Test]
        public void NotEqual()
        {
            Assert.That("<>", ParsesAs("NotEqual |<>|"));
        }
        [Test]
        public void InKeyword()
        {
            Assert.That("in", ParsesAs("InKeyword |in|"));
        }
        [Test]
        public void IsKeyword()
        {
            Assert.That("is", ParsesAs("IsKeyword |is|"));
        }
        [Test]
        public void AsKeyword()
        {
            Assert.That("as", ParsesAs("AsKeyword |as|"));
        }
    }
}
