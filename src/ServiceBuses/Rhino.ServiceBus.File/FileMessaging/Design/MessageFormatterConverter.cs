using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.Serialization.Formatters;

namespace Rhino.ServiceBus.FileMessaging.Design
{
    internal class MessageFormatterConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) { return (sourceType == typeof(string)); }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) { return (destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType)); }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null && value is string)
            {
                if (((string)value) == typeof(BinaryMessageFormatter).Name)
                    return new BinaryMessageFormatter();
                if (((string)value) == typeof(XmlMessageFormatter).Name)
                    return new XmlMessageFormatter();
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && destinationType == typeof(string))
            {
                if (value == null)
                    return "toStringNone";
                return value.GetType().Name;
            }
            if (destinationType == typeof(InstanceDescriptor))
            {
                if (value is XmlMessageFormatter)
                {
                    var formatter = (XmlMessageFormatter)value;
                    var constructor = typeof(XmlMessageFormatter).GetConstructor(new Type[] { typeof(string[]) });
                    if (constructor != null)
                        return new InstanceDescriptor(constructor, new object[] { formatter.TargetTypeNames });
                }
                else if (value is BinaryMessageFormatter)
                {
                    var formatter = (BinaryMessageFormatter)value;
                    var constructor = typeof(BinaryMessageFormatter).GetConstructor(new Type[] { typeof(FormatterAssemblyStyle), typeof(FormatterTypeStyle) });
                    if (constructor != null)
                        return new InstanceDescriptor(constructor, new object[] { formatter.TopObjectFormat, formatter.TypeFormat });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = new object[3];
            values[0] = new BinaryMessageFormatter();
            values[1] = new XmlMessageFormatter();
            return new TypeConverter.StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
    }
}
