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

namespace Tests.Serialization.Ranges
{
	using System;
	using SafetySharp.Modeling;
	using SafetySharp.Runtime;
	using SafetySharp.Runtime.Serialization;
	using Shouldly;

	internal class Error : SerializationObject
	{
		protected override void Check()
		{
			var c = new C();

			GenerateCode(SerializationMode.Full, c);

			c.F = 49;
			try
			{
				Serialize();
				throw new InvalidOperationException("Expected an exception.");
			}
			catch (RangeViolationException e)
			{
				e.Object.ShouldBe(c);
				e.Field.ShouldBe(c.GetType().GetField("F"));
				e.FieldValue.ShouldBe(49);
				e.Range.LowerBound.ShouldBe(0);
				e.Range.UpperBound.ShouldBe(5);
				e.Range.OverflowBehavior.ShouldBe(OverflowBehavior.Error);
			}

			c.F = 0;
			c.G = -49;
			try
			{
				Serialize();
				throw new InvalidOperationException("Expected an exception.");
			}
			catch (RangeViolationException e)
			{
				e.Object.ShouldBe(c);
				e.Field.ShouldBe(c.GetType().GetField("G"));
				e.FieldValue.ShouldBe(-49);
				e.Range.LowerBound.ShouldBe(-1);
				e.Range.UpperBound.ShouldBe(54);
				e.Range.OverflowBehavior.ShouldBe(OverflowBehavior.Error);
			}
		}

		internal class C
		{
			[Range(0, 5, OverflowBehavior.Error)]
			public short F;

			[Range(-1, 54, OverflowBehavior.Error)]
			public short G;
		}
	}
}