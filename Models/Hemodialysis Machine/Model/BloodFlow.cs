﻿using System;

namespace SafetySharp.CaseStudies.HemodialysisMachine.Model
{
	using System.ComponentModel;
	using Modeling;
	using Utilities.BidirectionalFlow;

	public class Blood : IElement<Blood>
	{
		[Hidden,Range(-1,7, OverflowBehavior.Error)]
		public int Water = 0;
		[Hidden, Range(0, 8, OverflowBehavior.Error)]
		public int SmallWasteProducts = 0;
		[Hidden, Range(0, 8, OverflowBehavior.Error)]
		public int BigWasteProducts = 0;

		[Hidden]
		public bool HasHeparin = false;
		[Hidden]
		public bool ChemicalCompositionOk = true;
		[Hidden]
		public bool GasFree = false;
		[Hidden]
		public QualitativePressure Pressure = QualitativePressure.NoPressure;
		[Hidden]
		public QualitativeTemperature Temperature = QualitativeTemperature.TooCold;

		public void CopyValuesFrom(Blood from)
		{
			Water = from.Water;
			SmallWasteProducts = from.SmallWasteProducts;
			BigWasteProducts = from.BigWasteProducts;
			HasHeparin = from.HasHeparin;
			ChemicalCompositionOk = from.ChemicalCompositionOk;
			GasFree = from.GasFree;
			Pressure = from.Pressure;
			Temperature = from.Temperature;
		}

		public void CopyValuesFrom(BufferedBlood from)
		{
			Water = from.Water;
			SmallWasteProducts = from.SmallWasteProducts;
			BigWasteProducts = from.BigWasteProducts;
			HasHeparin = from.HasHeparin;
			ChemicalCompositionOk = from.ChemicalCompositionOk;
			GasFree = from.GasFree;
			Pressure = from.Pressure;
			Temperature = from.Temperature;
		}

		public bool HasWaterOrBigWaste()
		{
			return Water > 0 || BigWasteProducts > 0;
		}

		public string ValuesAsText()
		{
			return 
				"Water: " + Water +
				"\nSmallWasteProducts: " + SmallWasteProducts +
				"\nBigWasteProducts: " + BigWasteProducts;
		}

		public void PrintBloodValues(string description)
		{
			System.Console.Out.WriteLine("\t"+description);
			System.Console.Out.WriteLine("\t\tWater: " + Water);
			System.Console.Out.WriteLine("\t\tSmallWasteProducts: " + SmallWasteProducts);
			System.Console.Out.WriteLine("\t\tBigWasteProducts: " + BigWasteProducts);
		}
	}


	public class BufferedBlood
	{
		[Range(-1, 7, OverflowBehavior.Error)]
		public int Water = 0;
		[Range(0, 8, OverflowBehavior.Error)]
		public int SmallWasteProducts = 0;
		[Range(0, 8, OverflowBehavior.Error)]
		public int BigWasteProducts = 0;
		
		public bool HasHeparin = false;
		public bool ChemicalCompositionOk = true;
		public bool GasFree = false;
		public QualitativePressure Pressure = QualitativePressure.NoPressure;
		public QualitativeTemperature Temperature = QualitativeTemperature.TooCold;

		public void CopyValuesFrom(Blood from)
		{
			Water = from.Water;
			SmallWasteProducts = from.SmallWasteProducts;
			BigWasteProducts = from.BigWasteProducts;
			HasHeparin = from.HasHeparin;
			ChemicalCompositionOk = from.ChemicalCompositionOk;
			GasFree = from.GasFree;
			Pressure = from.Pressure;
			Temperature = from.Temperature;
		}
	}


	public class BloodFlowInToOutSegment : FlowInToOutSegment<Blood, Suction>
	{
	}

	public class BloodFlowSource : FlowSource<Blood, Suction>
	{
	}

	public class BloodFlowSink : FlowSink<Blood, Suction>
	{
	}

	public class BloodFlowComposite : FlowComposite<Blood, Suction>
	{
	}


	public class BloodFlowVirtualSplitter : FlowVirtualSplitter<Blood, Suction>, IIntFlowComponent
	{
		public BloodFlowVirtualSplitter(int number)
			: base(number)
		{
		}

		public override void SplitForwards(Blood source, Blood[] targets, Suction[] dependingOn)
		{
			var number = targets.Length;
			// Copy all needed values
			for (int i = 0; i < number; i++)
			{
				targets[i].CopyValuesFrom(source);
			}
			// TODO: No advanced splitting implemented, yet.
		}

		public override void MergeBackwards(Suction[] sources, Suction target)
		{
			target.CopyValuesFrom(sources[0]);
			var number = sources.Length;
			for (int i = 1; i < number; i++) //start with second element
			{
				if (target.SuctionType == SuctionType.SourceDependentSuction || sources[i].SuctionType == SuctionType.SourceDependentSuction)
				{
					target.SuctionType = SuctionType.SourceDependentSuction;
					target.CustomSuctionValue = 0;
				}
				else
				{
					target.SuctionType = SuctionType.CustomSuction;
					target.CustomSuctionValue += sources[i].CustomSuctionValue;
				}
			}
		}
	}

	public class BloodFlowVirtualMerger : FlowVirtualMerger<Blood, Suction>, IIntFlowComponent
	{
		public BloodFlowVirtualMerger(int number)
			: base(number)
		{
		}

		public override void SplitBackwards(Suction source, Suction[] targets)
		{
			var number = targets.Length;

			if (source.SuctionType == SuctionType.SourceDependentSuction)
			{
				for (int i = 0; i < number; i++)
				{
					targets[i].CopyValuesFrom(source);
				}
			}
			else
			{
				var suctionForEach = source.CustomSuctionValue / number;
				for (int i = 0; i < number; i++)
				{
					targets[i].SuctionType = SuctionType.CustomSuction;
					targets[i].CustomSuctionValue = suctionForEach;
				}
			}
		}

		public override void MergeForwards(Blood[] sources, Blood target, Suction dependingOn)
		{
			target.CopyValuesFrom(sources[0]);
			var number = sources.Length;
			for (int i = 1; i < number; i++) //start with second element
			{
				target.ChemicalCompositionOk &= sources[i].ChemicalCompositionOk;
				target.GasFree &= sources[i].GasFree;
				target.BigWasteProducts += sources[i].BigWasteProducts;
				target.SmallWasteProducts += sources[i].SmallWasteProducts;
				target.HasHeparin |= sources[i].HasHeparin;
				target.Water += sources[i].Water;
				if (sources[i].Temperature != QualitativeTemperature.BodyHeat)
					target.Temperature = sources[i].Temperature;
				if (sources[i].Pressure != QualitativePressure.GoodPressure)
					target.Pressure = sources[i].Pressure;
			}
		}
	}

	public class BloodFlowCombinator : FlowCombinator<Blood, Suction>, IIntFlowComponent
	{
		public override FlowVirtualMerger<Blood, Suction> CreateFlowVirtualMerger(int elementNos)
		{
			return new BloodFlowVirtualMerger(elementNos);
		}

		public override FlowVirtualSplitter<Blood, Suction> CreateFlowVirtualSplitter(int elementNos)
		{
			return new BloodFlowVirtualSplitter(elementNos);
		}
	}

	public class BloodFlowUniqueOutgoingStub : FlowUniqueOutgoingStub<Blood, Suction>
	{
	}

	public class BloodFlowUniqueIncomingStub : FlowUniqueIncomingStub<Blood, Suction>
	{
	}
}
