﻿using System;
using System.Collections.Generic;

namespace Ambermoon
{
    public class Size : IEquatable<Size>, IEqualityComparer<Size>
    {
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public bool Empty => Width <= 0 || Height <= 0;

        public Size()
        {

        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Size(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public static bool operator ==(Size size1, Size size2)
        {
            if (ReferenceEquals(size1, size2))
                return true;

            if (ReferenceEquals(size1, null) || ReferenceEquals(size2, null))
                return false;

            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(Size size1, Size size2)
        {
            if (ReferenceEquals(size1, size2))
                return false;

            if (ReferenceEquals(size1, null) || ReferenceEquals(size2, null))
                return true;

            return size1.Width != size2.Width || size1.Height != size2.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Size)
                return Equals(obj as Size);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, just wrap
            {
                int hash = 23;

                hash = hash * 17 + Width.GetHashCode();
                hash = hash * 17 + Height.GetHashCode();

                return hash;
            }
        }

        public bool Equals(Size other)
        {
            return this == other;
        }

        public bool Equals(Size x, Size y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);

            return x == y;
        }

        public int GetHashCode(Size obj)
        {
            return (obj == null) ? 0 : obj.GetHashCode();
        }
    }

    public class FloatSize : IEquatable<FloatSize>, IEqualityComparer<FloatSize>
    {
        public float Width { get; set; } = 0.0f;
        public float Height { get; set; } = 0.0f;

        public bool Empty => Width <= 0.0f || Height <= 0.0f;

        public FloatSize()
        {

        }

        public FloatSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public FloatSize(FloatSize size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public FloatSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public static bool operator ==(FloatSize size1, FloatSize size2)
        {
            if (ReferenceEquals(size1, size2))
                return true;

            if (ReferenceEquals(size1, null) || ReferenceEquals(size2, null))
                return false;

            return Util.FloatEqual(size1.Width, size2.Width) && Util.FloatEqual(size1.Height, size2.Height);
        }

        public static bool operator !=(FloatSize size1, FloatSize size2)
        {
            if (ReferenceEquals(size1, size2))
                return false;

            if (ReferenceEquals(size1, null) || ReferenceEquals(size2, null))
                return true;

            return !Util.FloatEqual(size1.Width, size2.Width) || !Util.FloatEqual(size1.Height, size2.Height);
        }

        public static implicit operator FloatSize(Size size)
        {
            return new FloatSize(size);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is FloatSize)
                return Equals(obj as FloatSize);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, just wrap
            {
                int hash = 23;

                hash = hash * 17 + Width.GetHashCode();
                hash = hash * 17 + Height.GetHashCode();

                return hash;
            }
        }

        public bool Equals(FloatSize other)
        {
            return this == other;
        }

        public bool Equals(FloatSize x, FloatSize y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);

            return x == y;
        }

        public int GetHashCode(FloatSize obj)
        {
            return (obj == null) ? 0 : obj.GetHashCode();
        }
    }
}
