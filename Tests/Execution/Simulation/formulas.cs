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

namespace Tests.Execution.Simulation
{
	using SafetySharp.Analysis;
	using SafetySharp.Modeling;
	using Shouldly;
	using Utilities;

	internal class Formulas : TestObject
	{
		protected override void Check()
		{
			var m = new M();
			var simulator = new Simulator(m, m.F);
			m = (M)simulator.Model;
			var f = m.F.Compile();
			var g = m.G.Compile();
			
			m.C.X.ShouldBe(0);
			m.F.Evaluate().ShouldBe(true);
			m.G.Evaluate().ShouldBe(true);

			f().ShouldBe(true);
			g().ShouldBe(true);

			simulator.SimulateStep();

			m.C.X.ShouldBe(1);
			m.F.Evaluate().ShouldBe(false);
			m.G.Evaluate().ShouldBe(false);

			f().ShouldBe(false);
			g().ShouldBe(false);
		}

		private class M : ModelBase
		{
			[Root(Role.Environment)]
			public readonly C C = new C();

			public Formula F => C.X != 1;

			public Formula G { get; }

			public M()
			{
				G = C.X == 0;
			}
		}

		private class C : Component
		{
			public int X;

			public override void Update()
			{
				++X;
			}
		}
	}
}