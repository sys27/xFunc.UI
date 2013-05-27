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

    public class Cos : TrigonometryMathExpression
    {

        public Cos() : base(null) { }

        public Cos(IMathExpression firstMathExpression) : base(firstMathExpression) { }

        public Cos(IMathExpression firstMathExpression, AngleMeasurement angleMeasurement) : base(firstMathExpression, angleMeasurement) { }

        public override string ToString()
        {
            return ToString("cos({0})");
        }

        public override double CalculateDergee(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            var radian = firstMathExpression.Calculate(parameters, functions) * Math.PI / 180;

            return Math.Cos(radian);
        }

        public override double CalculateRadian(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            return Math.Cos(firstMathExpression.Calculate(parameters, functions));
        }

        public override double CalculateGradian(MathParameterCollection parameters, MathFunctionCollection functions)
        {
            var radian = firstMathExpression.Calculate(parameters, functions) * Math.PI / 200;

            return Math.Cos(radian);
        }

        protected override IMathExpression _Differentiation(Variable variable)
        {
            Sin sine = new Sin(firstMathExpression.Clone(), this.angleMeasurement);
            Mul multiplication = new Mul(sine, firstMathExpression.Clone().Differentiate(variable));
            UnaryMinus unMinus = new UnaryMinus(multiplication);

            return unMinus;
        }

        public override IMathExpression Clone()
        {
            return new Cos(firstMathExpression.Clone());
        }

    }

}
