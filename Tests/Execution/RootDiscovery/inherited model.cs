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

namespace Tests.Execution.RootDiscovery
{
	using SafetySharp.Analysis;
	using SafetySharp.Modeling;
	using Shouldly;
	using Utilities;

	internal class InheritedModel : TestObject
	{
		protected override void Check()
		{
			var m = new M2();
			m.Roots.ShouldBe(new[] { m.A, m.B, m.C, m.D() }, ignoreOrder: true);
			m.Components.ShouldBe(new[] { m.A, m.B, m.C, m.D() }, ignoreOrder: true);
			m.Faults.ShouldBeEmpty();
		}

		private class M : ModelBase
		{
			private readonly C _c2 = new C();

			public readonly C E = new C();

			[Root(Role.Environment)]
			public C C { get; } = new C();

			public C F => new C();

			[Root(Role.Environment)]
			public C D() => _c2;
		}

		private class M2 : M
		{
			[Root(Role.Environment)]
			public readonly C A = new C();

			[Root(Role.Environment)]
			public C B { get; } = new C();

			public C G { get; } = new C();

			public C H() => new C();
		}

		private class C : Component
		{
		}
	}
}