﻿#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Builder for <see cref="HexKeyValuePair{TKey,TValue}"/>.</summary>
  public static class HexKeyValuePair {
    /// <summary>Constructs a new <see cref="HexKeyValuePair{TKey,TValue}"/> instance, with type inference.</summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static HexKeyValuePair<TKey,TValue> New<TKey,TValue>(TKey key, TValue value)
      where TKey : struct, IEquatable<TKey>, IComparable<TKey> 
//      where TValue : class
    {
      return new HexKeyValuePair<TKey,TValue>(key, value);
    }
  }

  /// <summary>An immutable struct representing an associated Key and Value pair with equality
  /// and  comparabilitye of instances defined by the supplied TKey type.</summary>
  public struct HexKeyValuePair<TKey,TValue> 
    : IEquatable<HexKeyValuePair<TKey,TValue>>, 
      IComparable<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey> 
  {
    /// <summary>Constructs a new HexKeyValuePair instance.</summary>
    internal HexKeyValuePair(TKey key, TValue value) : this() {
      Key   = key;
      Value = value;
    }

    #region Properties
    /// <summary>TODO</summary>
    public TKey   Key     { get; private set; }
    /// <summary>TODO</summary>
    public TValue Value   { get; private set; }
    #endregion

    #region Value equality
    /// <inheritdoc/>
    public override bool Equals(object obj) {
      var other = obj as HexKeyValuePair<TKey,TValue>?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Key.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(HexKeyValuePair<TKey,TValue> other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) != 0;
    }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) == 0;
    }

    #region IComparable implementation
    /// <summary>Tests whether lhs &lt; rhs.</summary>
    public static bool operator <  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) < 0;;
    }
    /// <summary>Tests whether lhs &lt;= rhs.</summary>
    public static bool operator <= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) <= 0;;
    }
    /// <summary>Tests whether lhs &gt;= rhs.</summary>
    public static bool operator >= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) >= 0;
    }
    /// <summary>Tests whether lhs &gt; rhs.</summary>
    public static bool operator >  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) > 0;
    }
    /// <inheritdoc/>
    public int CompareTo(HexKeyValuePair<TKey,TValue> other) { return this.Key.CompareTo(other.Key); }
    #endregion
    #endregion
  }
}
