﻿#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.Storage {
  using HexSize     = System.Drawing.Size;

  /// <summary>A row-major <c>BoardStorage</c> implementation optimized for small maps, 
  /// providing both serial and parallel initialization.</summary>
  public sealed class FlatBoardStorage<T> : BoardStorage<T> {
    /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
    /// initialized using <paramref name="factory"/>.</summary>
    /// <param name="sizeHexes"><c>Size</c> structure speccifying the Width and Height of 
    /// the desired board storage.</param>
    /// <param name="factory"></param>
    /// <param name="inParallel">Boolean indicating how the board should be initialized: 
    /// in parallel or serially.</param>
    public FlatBoardStorage(HexSize sizeHexes, Func<HexCoords,T> factory, bool inParallel) 
    : base (sizeHexes) {
      Contract.Requires(sizeHexes.Width > 0);
      Contract.Requires(sizeHexes.Height > 0);
      Contract.Requires( ( from y in Enumerable.Range(0, sizeHexes.Height)
                           from x in Enumerable.Range(0, sizeHexes.Width)
                           select factory(HexCoords.NewUserCoords(x,y)) != null
                         ).All() );

      var rowRange = inParallel ? ParallelEnumerable.Range(0,sizeHexes.Height).AsOrdered()
                                : Enumerable.Range(0,sizeHexes.Height);
      Contract.Assume(rowRange != null);
      _backingStore = InitializeStoreX(sizeHexes, factory, rowRange);
    }
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
      Contract.Invariant(BackingStore != null);
      Contract.Invariant( BackingStore.All(row => row != null) );
      Contract.Invariant( BackingStore.All(row => row.All(block => block != null) ) );
    }
    private static IFastList<IFastListX<T>> InitializeStoreX(HexSize sizeHexes, Func<HexCoords, T> tFactory, IEnumerable<int> rowRange) {
      rowRange.RequiredNotNull("rowRange");
      Contract.Requires( ( from y in Enumerable.Range(0, sizeHexes.Height)
                           from x in Enumerable.Range(0, sizeHexes.Width)
                           select tFactory(HexCoords.NewUserCoords(x,y)) != null
                         ).All() );
      Contract.Ensures(Contract.Result<IFastList<IFastListX<T>>>() != null);

      return ( from y in rowRange
               select ( from x in Enumerable.Range(0, sizeHexes.Width)
                        select tFactory(HexCoords.NewUserCoords(x,y))
                      ).ToArray().ToFastListX()
             ).ToArray().ToFastList();
    }

    /// <inheritdoc/>>
    [Pure]protected override T ItemInner(int x, int y) {
      Contract.Ensures(Contract.Result<T>() != null);
      return BackingStore[y][x] ;
    }

    /// <inheritdoc/>>
    public override void ForEach(Action<T> action) {
      Contract.Assert(action != null);
      BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                  .ForAll(row => row.ForEach(action));
    }
    /// <inheritdoc/>>
    public override void ForEach(FastIteratorFunctor<T> functor) {
      Contract.Assert(functor != null);
      BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                  .ForAll(row => row.ForEach(functor));
    }

    /// <inheritdoc/>>
    public override void ForEachSerial(Action<T> action) {
      Contract.Assert(action != null);
      BackingStore.ForEach(row => row.ForEach(action));
    }
    /// <inheritdoc/>>
    public override void ForEachSerial(FastIteratorFunctor<T> functor) {
      Contract.Assert(functor != null);
      BackingStore.ForEach(row => row.ForEach(functor));
    }

    /// <summary>TODO</summary>
    /// <remarks>Use carefully - can interfere with iterators.</remarks>
    internal override void SetItem(HexCoords coords, T value) {
      Contract.Assume(value != null); // Enforced by base class
      coords.AssumeInvariant();

      var v = coords.User;
      BackingStore [v.Y].SetItem(v.X, value);
    }
    private IFastList<IFastListX<T>> BackingStore { get { return _backingStore; } }
    private readonly IFastList<IFastListX<T>> _backingStore;
  }
}
