using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace GraphQL.Query.Builder
{
    public class GraphQLObject : DynamicObject
    {
        public Func<string, string> KeyFormatter = CamelCaseStringFormatter.Formatter;

        internal Dictionary<string, object> _properties = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get => GetProperty(propertyName);
            set => SetProperty(propertyName, value);
        }

        public Dictionary<string, object> ToDictionary() => _properties;

        public string[] Keys => _properties.Keys.ToArray();

        public T GetProperty<T>(string key) => (T)GetProperty(key);

        public object GetProperty(string key)
        {
            if (_properties.ContainsKey(key))
                return _properties[key];
            return null;
        }

        public void SetProperty(string key, object value)
            => _properties[key] = value;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.ContainsKey(binder.Name))
            {
                result = GetProperty(binder.Name);
                return true;
            }
            else
            {
                result = "Invalid Property!";
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            dynamic method = _properties[binder.Name];
            result = method(args[0].ToString(), args[1].ToString());
            return true;
        }
    }
}

