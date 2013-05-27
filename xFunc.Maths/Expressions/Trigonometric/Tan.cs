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
    
    public class Tan : TrigonometryMathExpression
    {

        public Tan() : base(null) { }

        public Tan(IMathExpression firstMathExpression) : base(firstMathExpression) { }

        public Tan(IMathExpression firstMathExpression, AngleMeasurement angleMeasurement) : base(firstMathExpression, angleMeasurement) { }

        public override string ToString()
        {
            return ToString("tan({0})");
        }

        public override double CalculateDergee(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            var radian = firstMathExpression.Calculate(parameters, functions) * Math.PI / 180;

            return Math.Tan(radian);
        }

        public override double CalculateRadian(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            return Math.Tan(firstMathExpression.Calculate(parameters, functions));
        }

        public override double CalculateGradian(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            var radian = firstMathExpression.Calculate(parameters, functions) * Math.PI / 200;

            return Math.Tan(radian);
        }

        protected override IMathExpression _Differentiation(Variable variable)
        {
            Cos cos = new Cos(firstMathExpression.Clone(), this.angleMeasurement);
            Pow inv = new Pow(cos, new Number(2));
            Div div = new Div(firstMathExpression.Clone().Differentiate(variable), inv);

            return div;
        }

        public override IMathExpression Clone()
        {
            return new Tan(firstMathExpression.Clone());
        }

    }

}
