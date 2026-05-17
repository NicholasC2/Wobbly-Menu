using System;
using System.Reflection;

public class EditableField
{
    public string name;
    public object target;
    public MemberInfo member;
    public Type type;

    public EditableField(string name, object target, MemberInfo member)
    {
        this.name = name;
        this.target = target;
        this.member = member;

        if (member is FieldInfo f)
            type = f.FieldType;
        else if (member is PropertyInfo p)
            type = p.PropertyType;
    }

    public object Get()
    {
        if (member is FieldInfo f)
            return f.GetValue(target);

        if (member is PropertyInfo p)
            return p.GetValue(target);

        return null;
    }

    public void Set(object value)
    {
        if (member is FieldInfo f)
            f.SetValue(target, value);

        else if (member is PropertyInfo p && p.CanWrite)
            p.SetValue(target, value);
    }
}