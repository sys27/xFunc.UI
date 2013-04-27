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

namespace xFunc.Maths.Expressions.Trigonometric
{

    public class Cot : TrigonometryMathExpression
    {

        public Cot() : base(null) { }

        public Cot(IMathExpression firstMathExpression) : base(firstMathExpression) { }

        public override string ToString()
        {
            return ToString("cot({0})");
        }

        public override double CalculateDergee(MathParameterCollection parameters)
        {
            var radian = firstMathExpression.Calculate(parameters) * Math.PI / 180;

            return MathExtentions.Cot(radian);
        }

        public override double CalculateRadian(MathParameterCollection parameters)
        {
            var x = firstMathExpression.Calculate(parameters);

            return MathExtentions.Cot(x);
        }

        public override double CalculateGradian(MathParameterCollection parameters)
        {
            var radian = firstMathExpression.Calculate(parameters) * Math.PI / 200;

            return MathExtentions.Cot(radian);
        }

        protected override IMathExpression _Differentiation(Variable variable)
        {
            Sin sine = new Sin(firstMathExpression.Clone());
            Pow involution = new Pow(sine, new Number(2));
            Div division = new Div(firstMathExpression.Clone().Differentiate(variable), involution);
            UnaryMinus unMinus = new UnaryMinus(division);

            return unMinus;
        }

        public override IMathExpression Clone()
        {
            return new Cot(firstMathExpression.Clone());
        }

    }

}