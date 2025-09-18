using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Utilities
{
    public class GenericBinder : Binder
    {
        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            return match.First(m => m.IsGenericMethod);
        }

        #region not implemented
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state) => throw new NotImplementedException();
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture) => throw new NotImplementedException();
        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers) => throw new NotImplementedException();
        public override object ChangeType(object value, Type type, CultureInfo culture) => throw new NotImplementedException();
        public override void ReorderArgumentArray(ref object[] args, object state) => throw new NotImplementedException();
        #endregion
    }
    public class NonGenericBinder : Binder
    {
        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            return match.First(m => !m.IsGenericMethod);
        }

        #region not implemented
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state) => throw new NotImplementedException();
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture) => throw new NotImplementedException();
        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers) => throw new NotImplementedException();
        public override object ChangeType(object value, Type type, CultureInfo culture) => throw new NotImplementedException();
        public override void ReorderArgumentArray(ref object[] args, object state) => throw new NotImplementedException();
        #endregion
    }
}
