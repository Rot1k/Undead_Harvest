using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class ProfilerExportTemp
{
    public static void Export()
    {
        string capturePath = "C:/Unity projects/Undead_Harvest/ProfilerCaptures/Undead Harvest_2026-06-23_22-45-54.data";
        string outputPath = "C:/Unity projects/Undead_Harvest/Temp/profiler-export.txt";

        Type driverType = Type.GetType("UnityEditorInternal.ProfilerDriver, UnityEditor");
        if (driverType == null)
        {
            File.WriteAllText(outputPath, "ProfilerDriver type not found.");
            EditorApplication.Exit(2);
            return;
        }

        MethodInfo loadProfile = driverType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "LoadProfile" && m.GetParameters().Length >= 1);
        InvokeBest(loadProfile, capturePath);

        int firstFrame = GetIntMember(driverType, "firstFrameIndex", 0);
        int lastFrame = GetIntMember(driverType, "lastFrameIndex", 1999);
        MethodInfo getRawFrameDataView = driverType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "GetRawFrameDataView" && m.GetParameters().Length == 2);

        if (getRawFrameDataView == null)
        {
            File.WriteAllText(outputPath, $"GetRawFrameDataView not found. Frames {firstFrame}-{lastFrame}.");
            EditorApplication.Exit(3);
            return;
        }

        int[] interestingFrames = { 0, 199, 380, 577, 769, 951, 959, 1125, 1138, 1149, 1164, 1498, 1680, 1848 };
        using StreamWriter writer = new StreamWriter(outputPath);
        writer.WriteLine($"frames={firstFrame}-{lastFrame}");

        foreach (int frame in interestingFrames)
        {
            writer.WriteLine($"FRAME {frame}");
            for (int thread = 0; thread < 8; thread++)
            {
                object view = getRawFrameDataView.Invoke(null, new object[] { frame, thread });
                if (view == null)
                {
                    continue;
                }

                Type viewType = view.GetType();
                bool valid = GetBoolMember(view, viewType, "valid", true);
                if (!valid)
                {
                    Dispose(view);
                    continue;
                }

                int sampleCount = GetIntMember(view, viewType, "sampleCount", 0);
                if (sampleCount <= 0)
                {
                    Dispose(view);
                    continue;
                }

                writer.WriteLine($" thread={thread} samples={sampleCount}");
                MethodInfo getSampleName = viewType.GetMethod("GetSampleName", new[] { typeof(int) });
                MethodInfo getSampleTimeMs = viewType.GetMethod("GetSampleTimeMs", new[] { typeof(int) });
                MethodInfo getSampleSelfTimeMs = viewType.GetMethod("GetSampleSelfTimeMs", new[] { typeof(int) });

                for (int i = 0; i < sampleCount; i++)
                {
                    float time = getSampleTimeMs != null ? Convert.ToSingle(getSampleTimeMs.Invoke(view, new object[] { i })) : 0f;
                    if (time < 0.1f)
                    {
                        continue;
                    }

                    string name = getSampleName != null ? Convert.ToString(getSampleName.Invoke(view, new object[] { i })) : "";
                    float self = getSampleSelfTimeMs != null ? Convert.ToSingle(getSampleSelfTimeMs.Invoke(view, new object[] { i })) : 0f;
                    writer.WriteLine($"  {time:0.###} ms self {self:0.###} ms | {name}");
                }

                Dispose(view);
            }
        }

        EditorApplication.Exit(0);
    }

    private static void InvokeBest(MethodInfo method, string capturePath)
    {
        if (method == null)
        {
            return;
        }

        object[] args = method.GetParameters()
            .Select(p => p.ParameterType == typeof(string) ? capturePath : p.HasDefaultValue ? p.DefaultValue : GetDefault(p.ParameterType))
            .ToArray();
        method.Invoke(null, args);
    }

    private static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static int GetIntMember(Type type, string name, int fallback)
    {
        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (property != null)
        {
            return Convert.ToInt32(property.GetValue(null));
        }

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        return field != null ? Convert.ToInt32(field.GetValue(null)) : fallback;
    }

    private static int GetIntMember(object instance, Type type, string name, int fallback)
    {
        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return Convert.ToInt32(property.GetValue(instance));
        }

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null ? Convert.ToInt32(field.GetValue(instance)) : fallback;
    }

    private static bool GetBoolMember(object instance, Type type, string name, bool fallback)
    {
        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return Convert.ToBoolean(property.GetValue(instance));
        }

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null ? Convert.ToBoolean(field.GetValue(instance)) : fallback;
    }

    private static void Dispose(object instance)
    {
        if (instance is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
