﻿// The MIT License (MIT)
// 
// Copyright (c) 2014-2016, Institute for Software & Systems Engineering
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Tests.Formulas.TemporalOperators
{
	using SafetySharp.Analysis;
	using static SafetySharp.Analysis.Operators;

	internal class T4 : FormulaTestObject
	{
		protected override void Check()
		{
			var intValue = 7;

			{
				var actual = F(intValue < 7);
				var expected = new UnaryFormula(
					new StateFormula(() => intValue < 7),
					UnaryOperator.Finally);

				Check(actual, expected);
			}

			{
				var actual = F(F(intValue >= 7));
				var expected = new UnaryFormula(
					new UnaryFormula(
						new StateFormula(() => intValue >= 7),
						UnaryOperator.Finally),
					UnaryOperator.Finally);

				Check(actual, expected);
			}

			{
				var actual = AF(EF(intValue >= 7));
				var expected = new UnaryFormula(
					new UnaryFormula(
						new UnaryFormula(
							new UnaryFormula(
								new StateFormula(() => intValue >= 7),
								UnaryOperator.Finally),
							UnaryOperator.Exists),
						UnaryOperator.Finally),
					UnaryOperator.All);

				Check(actual, expected);
			}
		}
	}
}