using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Variable
{
    public string name;
    public object targetObject;
    public string fieldName;
    public Type type;

    public Variable(string name, object targetObject, string fieldName)
    {
        this.name = name;
        this.targetObject = targetObject;
        this.fieldName = fieldName;

        var field = targetObject.GetType().GetField(fieldName,
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance);

        if (field != null)
        {
            type = field.FieldType;
        }
    }

     public object GetValue()
    {
        var obj = targetObject;

        if (obj is Transform t)
        {
            switch (fieldName)
            {
                case "position":
                    return t.position;
                case "rotation":
                    return t.rotation;
                case "scale":
                    return t.localScale;
            }
        }

        var parts = fieldName.Split('.');

        foreach (var part in parts)
        {
            if (obj == null) return null;

            var type = obj.GetType();

            var field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                obj = field.GetValue(obj);
                continue;
            }

            var prop = type.GetProperty(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                obj = prop.GetValue(obj);
                continue;
            }

            return null;
        }

        return obj;
    }

    public void SetValue(object value)
    {
        if (targetObject is Transform t)
        {
            switch (fieldName)
            {
                case "position":
                    t.position = (Vector3)value;
                    return;

                case "rotation":
                    t.rotation = (Quaternion)value;
                    return;

                case "scale":
                    t.localScale = (Vector3)value;
                    return;
            }
        }

        var field = targetObject.GetType().GetField(fieldName);
        if (field != null)
        {
            field.SetValue(targetObject, value);
        }
    }
}

public static class VariableRegistry
{
    public static List<Variable> Variables = new List<Variable>();

    public static void Register(Variable variable)
    {
        Variables.Add(variable);
    }
}