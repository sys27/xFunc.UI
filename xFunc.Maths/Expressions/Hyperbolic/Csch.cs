﻿// Copyright 2012-2013 Dmitry Kischenko
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

namespace xFunc.Maths.Expressions.Hyperbolic
{

    public class Csch : UnaryMathExpression
    {

        public Csch()
            : base(null)
        {

        }

        public Csch(IMathExpression firstMathExpression)
            : base(firstMathExpression)
        {

        }

        public override string ToString()
        {
            return ToString("csch({0})");
        }

        public override double Calculate()
        {
            return MathExtentions.Csch(firstMathExpression.Calculate());
        }

        public override double Calculate(MathParameterCollection parameters)
        {
            return MathExtentions.Csch(firstMathExpression.Calculate(parameters));
        }

        public override IMathExpression Clone()
        {
            return new Csch(firstMathExpression.Clone());
        }

        protected override IMathExpression _Differentiation(Variable variable)
        {
            var coth = new Coth(firstMathExpression.Clone());
            var csch = Clone();
            var mul1 = new Mul(coth, csch);
            var mul2 = new Mul(firstMathExpression.Clone().Differentiate(variable), mul1);
            var unMinus = new UnaryMinus(mul2);

            return unMinus;
        }

    }

}