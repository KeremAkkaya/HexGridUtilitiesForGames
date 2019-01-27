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

namespace PGNapoleonics.HexUtilities.Common {
  using HexPointF    = System.Drawing.PointF;
  using HexPoint     = System.Drawing.Point;
  using HexSizeF     = System.Drawing.SizeF;
  using HexSize      = System.Drawing.Size;
  using HexRectangle = System.Drawing.Rectangle;

  /// <summary>TODO</summary>
  public static class SizeExtensions {
    /// <summary>TODO</summary>
    public static HexSize Scale(this HexSize @this, int value) { 
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static HexSize Scale(this HexSize @this, int valueX, int valueY) {
      return new HexSize(@this.Width * valueX, @this.Height * valueY);
    }


    /// <summary>TODO</summary>
    public static HexSizeF Scale(this HexSize @this, float value) {
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static HexSizeF Scale(this HexSize @this, float valueX, float valueY) {
      return new HexSizeF(@this).Scale(valueX,valueY);
    }
    /// <summary>TODO</summary>
    public static HexSizeF Scale(this HexSizeF @this, float value) { 
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static HexSizeF Scale(this HexSizeF @this, float valueX, float valueY) {
      return new HexSizeF(@this.Width * valueX, @this.Height * valueY);
    }
  }
}

