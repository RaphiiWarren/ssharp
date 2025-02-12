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

namespace SafetySharp.CaseStudies.HeightControl.Modeling.ModelVariants
{
	using Controllers;
	using Sensors;
	using Vehicles;

	/// <summary>
	///   Represents a design variant of the height control with an additional overhead detector at the right lane of the end
	///   control.
	/// </summary>
	public class DetectorAtRightEndModel : Model
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public DetectorAtRightEndModel()
		{
			var detectorEndRight = new OverheadDetector { Position = EndControlPosition, Lane = Lane.Right };

			HeightControl = new HeightControl
			{
				PreControl = new PreControlOriginal
				{
					Detector = LightBarrierPre
				},
				MainControl = new MainControlOriginal
				{
					LeftDetector = DetectorMainLeft,
					RightDetector = DetectorMainRight,
					PositionDetector = LightBarrierMain,
					Timer = new Timer()
				},
				EndControl = new EndControlAdditionalLightBarrier
				{
					LeftLaneDetector = DetectorEndLeft,
					RightLaneDetector = detectorEndRight,
					Timer = new Timer()
				},
				TrafficLights = new TrafficLights()
			};

			Setup(detectorEndRight, "OD-End-Right");
		}
	}
}