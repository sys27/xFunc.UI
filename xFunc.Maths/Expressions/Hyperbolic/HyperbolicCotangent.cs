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

    public class HyperbolicCotangent : UnaryMathExpression
    {

        public HyperbolicCotangent()
            : base(null)
        {

        }

        public HyperbolicCotangent(IMathExpression firstMathExpression)
            : base(firstMathExpression)
        {

        }

        public override string ToString()
        {
            return ToString("coth({0})");
        }

        public override double Calculate(MathParameterCollection parameters)
        {
            return MathExtentions.Coth(firstMathExpression.Calculate(parameters));
        }

        public override IMathExpression Clone()
        {
            return new HyperbolicCotangent(firstMathExpression.Clone());
        }

        protected override IMathExpression _Differentiation(Variable variable)
        {
            var sinh = new HyperbolicSine(firstMathExpression.Clone());
            var inv = new Exponentiation(sinh, new Number(2));
            var div = new Division(firstMathExpression.Clone().Differentiation(variable), inv);
            var unMinus = new UnaryMinus(div);

            return unMinus;
        }

    }

}