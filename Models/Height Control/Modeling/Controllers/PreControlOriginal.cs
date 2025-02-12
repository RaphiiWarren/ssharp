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

namespace SafetySharp.CaseStudies.HeightControl.Modeling.Controllers
{
	using SafetySharp.Modeling;
	using Sensors;

	/// <summary>
	///   Represents the simple pre-control of the Elbtunnel height control that only checks incoming overheight vehicles using a
	///   single detector.
	/// </summary>
	public sealed class PreControlOriginal : PreControl
	{
		/// <summary>
		///   The vehicle detector that is used to detect vehicles entering the pre-control area.
		/// </summary>
		[Hidden]
		public VehicleDetector Detector;

		/// <summary>
		///   Gets the number of vehicles that passed the pre-control during the current system step.
		/// </summary>
		public override int GetNumberOfPassingVehicles()
		{
			return Detector.IsVehicleDetected ? 1 : 0;
		}

		/// <summary>
		///   Updates the state of the component.
		/// </summary>
		public override void Update()
		{
			Update(Detector);
		}
	}
}