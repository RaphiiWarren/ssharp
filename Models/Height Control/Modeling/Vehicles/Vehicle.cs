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

namespace SafetySharp.CaseStudies.HeightControl.Modeling.Vehicles
{
	using SafetySharp.Modeling;

	/// <summary>
	///   Represents an overheight vehicle that is not allowed to enter the tunnel on the left lane.
	/// </summary>
	public class Vehicle : Component
	{
		[Hidden]
		private VehicleKind _kind;

		[Range(0, Model.TunnelPosition, OverflowBehavior.Clamp)]
		private int _position;

		[Hidden]
		private int _speed;

		/// <summary>
		///   Gets the current lane of the vehicle.
		/// </summary>
		public Lane Lane { get; protected set; } = Lane.Right;

		/// <summary>
		///   Gets the kind the vehicle.
		/// </summary>
		public VehicleKind Kind
		{
			get { return _kind; }
			set { _kind = value; }
		}

		/// <summary>
		///   Gets the current vehicle's position.
		/// </summary>
		public int Position
		{
			get { return _position; }
			protected set { _position = value; }
		}

		/// <summary>
		///   Gets the current vehicle's speed.
		/// </summary>
		public int Speed
		{
			get { return _speed; }
			protected set { _speed = value; }
		}

		/// <summary>
		///   Gets a value indicating whether the vehicle has collided with the tunnel.
		/// </summary>
		public bool IsCollided =>
			Kind == VehicleKind.OverheightTruck && Position >= Model.TunnelPosition && Lane == Lane.Left;

		/// <summary>
		///   Informs the vehicle whether the tunnel is closed.
		/// </summary>
		public extern bool IsTunnelClosed { get; }

		/// <summary>
		///   Chooses the lane the vehicle drives on. By default, vehicles always drive on the right lane.
		/// </summary>
		protected virtual Lane ChooseLane() => Lane.Right;

		/// <summary>
		///   Moves the vehicle.
		/// </summary>
		public override void Update()
		{
			if (IsTunnelClosed)
				return;

			Speed = ChooseFromRange(1, Model.MaxSpeed);
			Position += Speed;

			// The road layout makes lane changes impossible when the end control has been reached
			if (Position < Model.EndControlPosition)
				Lane = ChooseLane();
		}

		/// <summary>
		///   A fault effect representing the case where a vehicle ignores the traffic rules and potentially chooses
		///   to drive on the left lane.
		/// </summary>
		[FaultEffect]
		public class DisregardTrafficRulesEffect : Vehicle
		{
			protected override Lane ChooseLane() => Choose(Lane.Left, Lane.Right);
		}
	}
}