﻿// Copyright 2012-2017 Dmitry Kischenko
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
using System;
using System.Numerics;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.ComplexNumbers;
using xFunc.Maths.Expressions.LogicalAndBitwise;
using Xunit;

namespace xFunc.Tests.Expressionss
{

    public class ExpTest
    {

        [Fact]
        public void ExecuteTest1()
        {
            var exp = new Exp(new Number(2));

            Assert.Equal(Math.Exp(2), exp.Execute());
        }

        [Fact]
        public void ExecuteTest2()
        {
            var expected = new Complex(4, 2);
            var exp = new Exp(new ComplexNumber(expected));

            Assert.Equal(Complex.Exp(expected), exp.Execute());
        }

        [Fact]
        public void ExecuteExeptionTest()
        {
            var exp = new Exp(new Bool(false));

            Assert.Throws<ResultIsNotSupportedException>(() => exp.Execute());
        }

        [Fact]
        public void CloneTest()
        {
            var exp = new Exp(new Number(0));
            var clone = exp.Clone();

            Assert.Equal(exp, clone);
        }

    }

}
