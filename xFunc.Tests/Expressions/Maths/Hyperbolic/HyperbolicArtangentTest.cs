﻿using System;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Hyperbolic;
using Xunit;

namespace xFunc.Test.Expressions.Maths.Hyperbolic
{
    
    public class HyperbolicArtangentTest
    {

        [Fact]
        public void CalculateTest()
        {
            var exp = new Artanh(new Number(1));

            Assert.Equal(MathExtentions.Atanh(1), exp.Calculate());
        }

    }

}