﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.CaseStudies.HemodialysisMachine.Tests
{
	using FluentAssertions;
	using Model;
	using NUnit.Framework;
	using Analysis;
	using Modeling;
	using Runtime;
	using Utilities;


	class DialyzerTestEnvironmentPatient : Component
	{
		public readonly BloodFlowSource ArteryFlow = new BloodFlowSource();
		public readonly BloodFlowSink VeinFlow = new BloodFlowSink();

		public int Water = 50;
		public int SmallWasteProducts = 10;
		public int BigWasteProducts = 3; //Only removable by ultrafiltration

		public int TimeStepsLeft = 6;

		[Provided]
		public void CreateBlood(Blood outgoingBlood)
		{
			var incomingSuction = ArteryFlow.Outgoing.BackwardFromSuccessor;
			var hasSuction = incomingSuction.SuctionType == SuctionType.CustomSuction && incomingSuction.CustomSuctionValue > 0;
			if (hasSuction)
			{
				var totalUnitsToDeliver = ArteryFlow.Outgoing.BackwardFromSuccessor.CustomSuctionValue;
				var bigWasteUnitsToDeliver = totalUnitsToDeliver / 2;
				if (BigWasteProducts >= bigWasteUnitsToDeliver)
				{
					outgoingBlood.BigWasteProducts = bigWasteUnitsToDeliver;
					var waterUnitsToDeliver = totalUnitsToDeliver - bigWasteUnitsToDeliver;
					outgoingBlood.Water = waterUnitsToDeliver;
				}
				else
				{
					outgoingBlood.BigWasteProducts = BigWasteProducts; // Deliver rest of unfiltrated blood or none
					var waterUnitsToDeliver = totalUnitsToDeliver - outgoingBlood.BigWasteProducts;
					outgoingBlood.Water = waterUnitsToDeliver;
				}
				if (SmallWasteProducts >= outgoingBlood.Water)
				{
					outgoingBlood.SmallWasteProducts = outgoingBlood.Water;
				}
				else
				{
					outgoingBlood.SmallWasteProducts = SmallWasteProducts; // Deliver rest of unfiltrated blood or none
				}
				Water -= outgoingBlood.Water;
				SmallWasteProducts -= outgoingBlood.SmallWasteProducts;
				BigWasteProducts -= outgoingBlood.BigWasteProducts;
				outgoingBlood.HasHeparin = true;
				outgoingBlood.ChemicalCompositionOk = true;
				outgoingBlood.GasFree = true;
				outgoingBlood.Pressure = QualitativePressure.GoodPressure;
				outgoingBlood.Temperature = QualitativeTemperature.BodyHeat;
			}
			else
			{
				outgoingBlood.Water = 0;
				outgoingBlood.BigWasteProducts = 0;
				outgoingBlood.SmallWasteProducts = 0;
				outgoingBlood.HasHeparin = true;
				outgoingBlood.ChemicalCompositionOk = true;
				outgoingBlood.GasFree = true;
				outgoingBlood.Pressure = QualitativePressure.NoPressure;
				outgoingBlood.Temperature = QualitativeTemperature.BodyHeat;
			}
		}

		[Provided]
		public void CreateBloodSuction(Suction outgoingSuction)
		{
			if (TimeStepsLeft > 0)
			{
				outgoingSuction.SuctionType = SuctionType.CustomSuction;
				outgoingSuction.CustomSuctionValue = 4;
			}
			else
			{
				outgoingSuction.SuctionType = SuctionType.CustomSuction;
				outgoingSuction.CustomSuctionValue = 0;
			}
		}
			[Provided]
			public void BloodReceived(Blood incomingBlood)
			{
				Water += incomingBlood.Water;
				SmallWasteProducts += incomingBlood.SmallWasteProducts;
				BigWasteProducts += incomingBlood.BigWasteProducts;
			}

			[Provided]
			public void DoNothing(Suction incomingSuction)
			{
			}

			public override void Update()
			{
				TimeStepsLeft = (TimeStepsLeft > 0) ? (TimeStepsLeft - 1) : 0;
			}

			protected override void CreateBindings()
			{
				Bind(nameof(ArteryFlow.SetOutgoingForward), nameof(CreateBlood));
				Bind(nameof(ArteryFlow.BackwardFromSuccessorWasUpdated), nameof(DoNothing));
				Bind(nameof(VeinFlow.SetOutgoingBackward), nameof(CreateBloodSuction));
				Bind(nameof(VeinFlow.ForwardFromPredecessorWasUpdated), nameof(BloodReceived));
			}

			public void PrintBloodValues(string pointOfTime)
			{
				System.Console.Out.WriteLine("\t" + "Patient ("+pointOfTime+")");
				System.Console.Out.WriteLine("\t\tWater: " + Water);
				System.Console.Out.WriteLine("\t\tSmallWasteProducts: " + SmallWasteProducts);
				System.Console.Out.WriteLine("\t\tBigWasteProducts: " + BigWasteProducts);
			}
	}

	class DialyzerTestEnvironment : ModelBase
	{
		[Root(Role.System)]
		public readonly Dialyzer Dialyzer = new Dialyzer();

		[Root(Role.Environment)]
		public readonly DialyzingFluidFlowCombinator DialysingFluidFlowCombinator = new DialyzingFluidFlowCombinator();
		[Root(Role.Environment)]
		public readonly BloodFlowCombinator BloodFlowCombinator = new BloodFlowCombinator();
		[Root(Role.Environment)]
		public readonly DialyzingFluidFlowSource DialyzingFluidFlowSource = new DialyzingFluidFlowSource();
		[Root(Role.Environment)]
		public readonly DialyzingFluidFlowSink DialyzingFluidFlowSink = new DialyzingFluidFlowSink();
		[Root(Role.Environment)]
		public readonly DialyzerTestEnvironmentPatient Patient = new DialyzerTestEnvironmentPatient();
		
		[Provided]
		public void CreateDialyzingFluid(DialyzingFluid outgoingDialyzingFluid)
		{
			//Hard code delivered quantity 2 and suction 3. We simulate if Ultra Filtration works with Dialyzer.
			outgoingDialyzingFluid.Quantity = 2;
			outgoingDialyzingFluid.KindOfDialysate = KindOfDialysate.Bicarbonate;
			outgoingDialyzingFluid.ContaminatedByBlood = false;
			outgoingDialyzingFluid.Temperature=QualitativeTemperature.BodyHeat;
	}

		[Provided]
		public void CreateDialyzingFluidSuction(Suction outgoingSuction)
		{
			//Hard code delivered quantity 2 and suction 3. We simulate if Ultra Filtration works with Dialyzer.
			outgoingSuction.SuctionType = SuctionType.CustomSuction;
			outgoingSuction.CustomSuctionValue = 3;
		}

		[Provided]
		public void DoNothing(Suction incomingSuction)
		{
		}

		[Provided]
		public void DoNothing(DialyzingFluid incomingDialyzingFluid)
		{
		}

		public DialyzerTestEnvironment()
		{
			Component.Bind(nameof(DialyzingFluidFlowSource.SetOutgoingForward), nameof(CreateDialyzingFluid));
			Component.Bind(nameof(DialyzingFluidFlowSource.BackwardFromSuccessorWasUpdated), nameof(DoNothing));
			Component.Bind(nameof(DialyzingFluidFlowSink.SetOutgoingBackward), nameof(CreateDialyzingFluidSuction));
			Component.Bind(nameof(DialyzingFluidFlowSink.ForwardFromPredecessorWasUpdated), nameof(DoNothing));

			DialysingFluidFlowCombinator.Connect(DialyzingFluidFlowSource.Outgoing, Dialyzer.DialyzingFluidFlow.Incoming);
			DialysingFluidFlowCombinator.Connect(Dialyzer.DialyzingFluidFlow.Outgoing, DialyzingFluidFlowSink.Incoming);
			BloodFlowCombinator.Connect(Patient.ArteryFlow.Outgoing, Dialyzer.BloodFlow.Incoming);
			BloodFlowCombinator.Connect(Dialyzer.BloodFlow.Outgoing, Patient.VeinFlow.Incoming);
		}
	}


	class DialyzerTests
	{
		[Test]
		public void DialyzerWorks_Simulation()
		{
			var specification = new DialyzerTestEnvironment();

			var simulator = new Simulator(specification); //Important: Call after all objects have been created
			var dialyzerAfterStep0 = (Dialyzer)simulator.Model.Roots.OfType<Dialyzer>().First();
			var patientAfterStep0 =
				(DialyzerTestEnvironmentPatient)
					simulator.Model.Roots.OfType<DialyzerTestEnvironmentPatient>().First();
			Console.Out.WriteLine("Initial");
			patientAfterStep0.ArteryFlow.Outgoing.ForwardToSuccessor.PrintBloodValues("outgoing Blood");
			patientAfterStep0.VeinFlow.Incoming.ForwardFromPredecessor.PrintBloodValues("incoming Blood");
			patientAfterStep0.PrintBloodValues("");
			Console.Out.WriteLine("Step 1");
			simulator.SimulateStep();
			var dialyzerAfterStep1 = (Dialyzer)simulator.Model.Roots.OfType<Dialyzer>().First();
			var patientAfterStep1 =
				(DialyzerTestEnvironmentPatient)
					simulator.Model.Roots.OfType<DialyzerTestEnvironmentPatient>().First();
			patientAfterStep1.ArteryFlow.Outgoing.ForwardToSuccessor.PrintBloodValues("outgoing Blood");
			patientAfterStep1.VeinFlow.Incoming.ForwardFromPredecessor.PrintBloodValues("incoming Blood");
			patientAfterStep1.PrintBloodValues("");
			Console.Out.WriteLine("Step 2");
			simulator.SimulateStep();
			var dialyzerAfterStep2 = (Dialyzer)simulator.Model.Roots.OfType<Dialyzer>().First();
			var patientAfterStep2 =
				(DialyzerTestEnvironmentPatient)
					simulator.Model.Roots.OfType<DialyzerTestEnvironmentPatient>().First();
			patientAfterStep2.ArteryFlow.Outgoing.ForwardToSuccessor.PrintBloodValues("outgoing Blood");
			patientAfterStep2.VeinFlow.Incoming.ForwardFromPredecessor.PrintBloodValues("incoming Blood");
			patientAfterStep2.PrintBloodValues("");
			Console.Out.WriteLine("Step 3");
			simulator.SimulateStep();
			var dialyzerAfterStep3 = (Dialyzer)simulator.Model.Roots.OfType<Dialyzer>().First();
			var patientAfterStep3 =
				(DialyzerTestEnvironmentPatient)
					simulator.Model.Roots.OfType<DialyzerTestEnvironmentPatient>().First();
			patientAfterStep3.ArteryFlow.Outgoing.ForwardToSuccessor.PrintBloodValues("outgoing Blood");
			patientAfterStep3.VeinFlow.Incoming.ForwardFromPredecessor.PrintBloodValues("incoming Blood");
			patientAfterStep3.PrintBloodValues("");
			Console.Out.WriteLine("Step 4");
			simulator.SimulateStep();
			var dialyzerAfterStep4 = (Dialyzer)simulator.Model.Roots.OfType<Dialyzer>().First();
			var patientAfterStep4 =
				(DialyzerTestEnvironmentPatient)
					simulator.Model.Roots.OfType<DialyzerTestEnvironmentPatient>().First();
			patientAfterStep4.ArteryFlow.Outgoing.ForwardToSuccessor.PrintBloodValues("outgoing Blood");
			patientAfterStep4.VeinFlow.Incoming.ForwardFromPredecessor.PrintBloodValues("incoming Blood");
			patientAfterStep4.PrintBloodValues("");

			//dialyzerAfterStep1.Should().Be(1);
			patientAfterStep4.BigWasteProducts.Should().Be(0);
			patientAfterStep4.SmallWasteProducts.Should().Be(2);
		}

		[Test]
		public void DialyzerWorks_ModelChecking()
		{
			var specification = new DialyzerTestEnvironment();
			var analysis = new SafetyAnalysis();

			var result = analysis.ComputeMinimalCriticalSets(specification, specification.Dialyzer.MembraneIntact==false);
			result.SaveCounterExamples("counter examples/hdmachine");

			Console.WriteLine(result);
		}
	}
}
