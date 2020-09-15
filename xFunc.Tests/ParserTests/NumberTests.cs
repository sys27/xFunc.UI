// Copyright 2012-2020 Dmytro Kyshchenko
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using xFunc.Maths;
using xFunc.Maths.Expressions;
using Xunit;

namespace xFunc.Tests.ParserTests
{
    public class NumberTests : BaseParserTests
    {
        [Fact]
        public void NumberFormatTest()
            => Assert.Throws<TokenizeException>(() => parser.Parse("0."));

        [Fact]
        public void ExpNegativeNumber()
            => ParseTest("1.2345E-10", new Number(0.00000000012345));

        [Fact]
        public void ExpNumber()
            => ParseTest("1.2345E10", new Number(12345000000));

        [Fact]
        public void ExpPositiveNumber()
            => ParseTest("1.2345E+10", new Number(12345000000));

        [Fact]
        public void ExpNumberLowerCase()
            => ParseTest("1.2e2", new Number(120));

        [Theory]
        [InlineData("0b01100110")]
        [InlineData("0B01100110")]
        public void BinTest(string function)
        {
            var expected = new Number(0b01100110);

            ParseTest(function, expected);
        }

        [Theory]
        [InlineData("0XFF00")]
        [InlineData("0xff00")]
        public void HexTest(string function)
        {
            var expected = new Number(0xFF00);

            ParseTest(function, expected);
        }

        [Fact]
        public void OctTest()
        {
            var expected = new Number(286);

            ParseTest("0436", expected);
        }
    }
}