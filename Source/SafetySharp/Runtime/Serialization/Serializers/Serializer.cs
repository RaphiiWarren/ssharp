// The MIT License (MIT)
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

namespace SafetySharp.Runtime.Serialization.Serializers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Modeling;

	/// <summary>
	///   Represents a dynamic code generator that generate the serialization code.
	/// </summary>
	internal abstract class Serializer
	{
		/// <summary>
		///   Checks whether the serialize is able to serialize the <paramref name="obj" />.
		/// </summary>
		/// <param name="obj">The obj that should be checked.</param>
		protected internal abstract bool CanSerialize(object obj);

		/// <summary>
		///   Generates the state slot metadata for the <paramref name="obj" />.
		/// </summary>
		/// <param name="obj">The object the state slot metadata should be generated for.</param>
		/// <param name="objectIdentifier">The identifier of the <paramref name="obj" />.</param>
		/// <param name="mode">The serialization mode that should be used to generate the metadata.</param>
		protected internal abstract IEnumerable<StateSlotMetadata> GetStateSlotMetadata(object obj, int objectIdentifier, SerializationMode mode);

		/// <summary>
		///   Serializes the information about <paramref name="obj" />'s type using the <paramref name="writer" />.
		/// </summary>
		/// <param name="obj">The object whose type information should be serialized.</param>
		/// <param name="writer">The writer the serialized information should be written to.</param>
		protected internal abstract void SerializeType(object obj, BinaryWriter writer);

		/// <summary>
		///   Creates an instance of the serialized type stored in the <paramref name="reader" /> without running
		///   any of the type's constructors.
		/// </summary>
		/// <param name="reader">The reader the serialized type information should be read from.</param>
		protected internal abstract object InstantiateType(BinaryReader reader);

		/// <summary>
		///   Gets all objects referenced by <paramref name="obj" />, excluding <paramref name="obj" /> itself.
		/// </summary>
		/// <param name="obj">The object the referenced objects should be returned for.</param>
		/// <param name="mode">The serialization mode that should be used to serialize the objects.</param>
		protected internal abstract IEnumerable<object> GetReferencedObjects(object obj, SerializationMode mode);

		/// <summary>
		///   Gets all objects referenced by <paramref name="obj" /> potentially marked with the <paramref name="hidden" /> attribute.
		/// </summary>
		/// <param name="obj">The object the referenced objects should be returned for.</param>
		/// <param name="mode">The serialization mode that should be used to serialize the objects.</param>
		/// <param name="hidden">The <see cref="HiddenAttribute" /> instance, if any, the field storing <paramref name="obj" /> was marked with.</param>
		protected internal virtual IEnumerable<object> GetReferencedObjects(object obj, SerializationMode mode, HiddenAttribute hidden)
		{
			yield return obj;
		}

		/// <summary>
		///   Gets the range information for the <paramref name="obj" />'s <paramref name="field" /> if it cannot be determined
		///   automatically by S#.
		///   Returns <c>false</c> to indicate that no range information is available.
		/// </summary>
		/// <param name="obj">The object the range should be determined for.</param>
		/// <param name="field">The field the range should be determined for.</param>
		/// <param name="mode">The serialization mode the range is obtained for.</param>
		/// <param name="range">Returns the range, if available.</param>
		protected internal virtual bool TryGetRange(object obj, FieldInfo field, SerializationMode mode, out RangeAttribute range)
		{
			range = null;
			return false;
		}
	}
}