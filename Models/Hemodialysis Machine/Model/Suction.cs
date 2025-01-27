﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.CaseStudies.HemodialysisMachine.Model
{
	using Modeling;
	using Utilities.BidirectionalFlow;

	public enum SuctionType
	{
		SourceDependentSuction,
		CustomSuction
	}

	public class Suction : IElement<Suction>
	{
		[Hidden]
		public SuctionType SuctionType = SuctionType.SourceDependentSuction;

		[Hidden,Range(0, 8, OverflowBehavior.Error)]
		public int CustomSuctionValue = 0;

		public void CopyValuesFrom(Suction from)
		{
			SuctionType = from.SuctionType;
			CustomSuctionValue = from.CustomSuctionValue;
		}

		public void PrintSuctionValues(string description)
		{
			System.Console.Out.WriteLine("\t" + description);
			System.Console.Out.WriteLine("\t\tSuction Type: " + SuctionType.ToString());
			System.Console.Out.WriteLine("\t\tCustomSuctionValue: " + CustomSuctionValue);
		}
	}
}
