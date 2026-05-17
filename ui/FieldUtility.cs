using System;
using System.Collections.Generic;
using System.Reflection;

public static class FieldUtility
{
    private static Dictionary<Type, MemberInfo[]> cache =
        new Dictionary<Type, MemberInfo[]>();

    public static EditableField[] GetFields(object obj)
    {
        if (obj == null)
            return new EditableField[0];

        Type type = obj.GetType();

        if (!cache.TryGetValue(type, out var members))
        {
            List<MemberInfo> list = new List<MemberInfo>();

            BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance;

            list.AddRange(type.GetFields(flags));
            list.AddRange(type.GetProperties(flags));

            members = list.ToArray();

            cache[type] = members;
        }

        List<EditableField> result = new List<EditableField>();

        foreach (var member in members)
        {
            result.Add(new EditableField(member.Name, obj, member));
        }

        return result.ToArray();
    }
}