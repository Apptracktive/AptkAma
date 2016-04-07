using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Foundation;

namespace Aptk.Plugins.AptkAma.Extensions
{
    public static class ObjectExtensions
    {
        public static object ToObject(this NSObject nsO)
        {
            return nsO.ToObject(null);
        }

        public static object ToObject(this NSObject nsO, Type targetType)
        {
            if (nsO is NSString)
            {
                return nsO.ToString();
            }

            var nsDate = nsO as NSDate;
            if (nsDate != null)
            {
                var nsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
                return nsRef.AddSeconds(nsDate.SecondsSinceReferenceDate);
            }

            var number = nsO as NSDecimalNumber;
            if (number != null)
            {
                return decimal.Parse(nsO.ToString(), CultureInfo.InvariantCulture);
            }

            var nsNumber = nsO as NSNumber;
            if (nsNumber != null)
            {
                switch (Type.GetTypeCode(targetType))
                {
                    case TypeCode.Boolean:
                        return nsNumber.BoolValue;
                    case TypeCode.Char:
                        return Convert.ToChar(nsNumber.ByteValue);
                    case TypeCode.SByte:
                        return nsNumber.SByteValue;
                    case TypeCode.Byte:
                        return nsNumber.ByteValue;
                    case TypeCode.Int16:
                        return nsNumber.Int16Value;
                    case TypeCode.UInt16:
                        return nsNumber.UInt16Value;
                    case TypeCode.Int32:
                        return nsNumber.Int32Value;
                    case TypeCode.UInt32:
                        return nsNumber.UInt32Value;
                    case TypeCode.Int64:
                        return nsNumber.Int64Value;
                    case TypeCode.UInt64:
                        return nsNumber.UInt64Value;
                    case TypeCode.Single:
                        return nsNumber.FloatValue;
                    case TypeCode.Double:
                        return nsNumber.DoubleValue;
                }
            }

            var value = nsO as NSValue;
            if (value != null)
            {
                if (targetType == typeof(IntPtr))
                {
                    return value.PointerValue;
                }

                if (targetType == typeof(SizeF))
                {
                    return value.SizeFValue;
                }

                if (targetType == typeof(RectangleF))
                {
                    return value.RectangleFValue;
                }

                if (targetType == typeof(PointF))
                {
                    return value.PointFValue;
                }
            }

            return nsO;
        }
    }
}
