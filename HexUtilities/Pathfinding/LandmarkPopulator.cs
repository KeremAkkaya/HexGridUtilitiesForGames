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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  using HexSize = System.Drawing.Size;

  /// <summary>TODO</summary>
  public interface ILandmarkPopulator {
    /// <summary>TODO</summary>
    BoardStorage<Maybe<short>> Fill();
  }

  /// <summary>The default implementation of <see cref="ILandmarkPopulator"/>.</summary>
  internal sealed partial class LandmarkPopulator : LandmarkPopulatorFunctor {
    public LandmarkPopulator(
      HexCoords                            hex, 
      HexSize                              mapSizeHexes, 
      Func<IPriorityQueue<int, HexCoords>> queueFactory, 
      TryDirectedCost                      tryDirectedStepCost
    ) : base(hex, mapSizeHexes, queueFactory, tryDirectedStepCost) {
      queueFactory.RequiredNotNull("queue");
      tryDirectedStepCost.RequiredNotNull("tryDirectedStepCost");
      Contract.Requires(queueFactory() != null);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
      Contract.Invariant(_queue != null);
      Contract.Invariant(_store != null);
      Contract.Invariant(_tryDirectedStepCost != null);
    }

    public override BoardStorage<Maybe<short>> Fill() {
      Contract.Ensures(Contract.Result<BoardStorage<Maybe<short>>>() != null);

      // Reduce field references by keeping all these on stack.
      var queue = _queue;
      var store = _store;
      var tryDirectedStepCost = _tryDirectedStepCost;

      HexKeyValuePair<int,HexCoords> item;
      while (queue.TryDequeue(out item)) {
        item.AssumeInvariant();

        var here = item.Value;
        var key  = item.Key;
        Tracing.FindPathDetail.Trace("Dequeue Path at {0} w/ cost={1,4}.", here, key);

        Hexside.ForEach( hexside => Action(queue,store,tryDirectedStepCost,here,key,hexside) );
      }
      return store;
    }

    private void Action(
      IPriorityQueue<int,HexCoords> queue, 
      BoardStorage<Maybe<short>>    store,
      TryDirectedCost               tryDirectedStepCost,
      HexCoords here, int key, Hexside hexside
    ) {
      hexside.RequiredNotNull("hexside");
      Contract.Requires(hexside.Value >= 0);
      hexside.AssumeInvariant();

      var neighbour = here.GetNeighbour(hexside);
      tryDirectedStepCost(here, hexside).IfHasValueDo( stepCost => {
        store[neighbour].ElseDo(() => Enqueue((short)(key+stepCost),neighbour,store));
      } );
    }
  }

  /// <summary>The default implementation of <see cref="ILandmarkPopulator"/>.</summary>
  internal partial class LandmarkPopulatorFunctor : FastIteratorFunctor<Hexside>, ILandmarkPopulator {
    public LandmarkPopulatorFunctor(
      HexCoords                            hexCoords, 
      HexSize                              mapSizeHexes, 
      Func<IPriorityQueue<int, HexCoords>> queueFactory, 
      TryDirectedCost                      tryDirectedStepCost
    ) {
      queueFactory.RequiredNotNull("queue");
      tryDirectedStepCost.RequiredNotNull("tryDirectedStepCost");
      Contract.Requires(queueFactory() != null);

      Tracing.FindPathDetail.Trace("Find distances from {0}", hexCoords);

      _queue               = queueFactory();
      _store               = BlockedBoardStorage.New32x32<Maybe<short>>(mapSizeHexes, c=>Maybe<short>.NoValue());
      _tryDirectedStepCost = tryDirectedStepCost;

      Enqueue(0,hexCoords,_store);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
      Contract.Invariant(_queue != null);
      Contract.Invariant(_store != null);
      Contract.Invariant(_tryDirectedStepCost != null);
    }

    protected readonly IPriorityQueue<int,HexCoords> _queue;
    protected readonly BoardStorage<Maybe<short>>    _store;
    protected readonly TryDirectedCost               _tryDirectedStepCost;
    private            HexCoords                     _here;
    private            short                         _key;

    public virtual BoardStorage<Maybe<short>> Fill() {
      Contract.Ensures(Contract.Result<BoardStorage<Maybe<short>>>() != null);
      HexKeyValuePair<int,HexCoords> item;

      var queue = _queue; //!< Reduce field references by keeping on stack.

      while (queue.TryDequeue(out item)) {
        _here = item.Value;
        _key  = (short)item.Key;
        Tracing.FindPathDetail.Trace("Dequeue Path at {0} w/ cost={1,4}.", _here, _key);

        Hexside.ForEach(this);
      }
      return _store;
    }

    public sealed override void Invoke(Hexside hexside) {
      var here = _here;
      _tryDirectedStepCost(here, hexside).IfHasValueDo(stepCost => InvokeInner(hexside,here,(short)(_key+stepCost)) );
    }

    private void InvokeInner(Hexside hexside, HexCoords here, short cost) {
      hexside.RequiredNotNull("hexside");
      hexside.AssumeInvariant();

      //!< Reduce field references by keeping these on stack.
      var neighbour = here.GetNeighbour(hexside);
      var store     = _store;
      if (store.MapSizeHexes.IsOnboard(neighbour))
        store[neighbour].ElseDo(() => Enqueue(cost,neighbour,store));
    }

    protected void Enqueue(short cost, HexCoords neighbour, BoardStorage<Maybe<short>> store) {
      store.RequiredNotNull("store");
      Tracing.FindPathDetail.Trace("   Enqueue {0}: {1,4}", neighbour, cost);
      store.SetItem(neighbour, cost);
      _queue.Enqueue(cost, neighbour);
    }
  }
}
