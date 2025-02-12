﻿// The MIT License (MIT)
// 
// Copyright (c) 2014-2015, Institute for Software & Systems Engineering
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

namespace SafetySharp.CaseStudies.HemodialysisMachine
{
	using Model;
	using Analysis;

	public class Specification : Modeling.ModelBase
	{
		[Root(Role.Environment)]
		private readonly DialyzingFluidFlowCombinator _dialysingFluidFlowCombinator;

		[Root(Role.Environment)]
		private readonly BloodFlowCombinator _bloodFlowCombinator;

		public Specification()
		{
			_dialysingFluidFlowCombinator = new DialyzingFluidFlowCombinator();
			_bloodFlowCombinator = new BloodFlowCombinator();

			HdMachine = new HdMachine
			{
			};

			Patient = new Patient
			{
			};

			HdMachine.AddFlows(_dialysingFluidFlowCombinator, _bloodFlowCombinator);

			_bloodFlowCombinator.Replace(HdMachine.ToPatientVein.Incoming, Patient.VeinFlow.Incoming);
			_bloodFlowCombinator.Replace(HdMachine.FromPatientArtery.Outgoing, Patient.ArteryFlow.Outgoing);
		}
		

		[Root(Role.System)]
		internal HdMachine HdMachine { get; }

		[Root(Role.Environment)]
		internal Patient Patient { get; }
		
		public Formula IncomingBloodNotOk
		{
			get
			{
				var incomingBlood = Patient.VeinFlow.Incoming.ForwardFromPredecessor;
				Formula receivedSomething = incomingBlood.BigWasteProducts > 0 || incomingBlood.Water > 0;
				Formula compositionOk = incomingBlood.ChemicalCompositionOk && incomingBlood.GasFree &&
										(incomingBlood.Temperature == QualitativeTemperature.BodyHeat);
				return receivedSomething && !compositionOk;
			}
		}

		public Formula BloodNotCleanedAndDialyzingFinished
		{
			get
			{
				Formula bloodIsNotCleaned = Patient.BigWasteProducts > 0 || Patient.SmallWasteProducts > 0;
				Formula dialyzingFinished = HdMachine.ControlSystem.TimeStepsLeft == 0;
				return bloodIsNotCleaned && dialyzingFinished;
			}
		}
	}
}