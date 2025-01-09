using System;
using System.Runtime.InteropServices;

[ComImport]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IMMDeviceEnumerator
{
    int EnumAudioEndpoints(EDataFlow dataFlow, DeviceState stateMask, out IMMDeviceCollection devices);
    int GetDefaultAudioEndpoint(EDataFlow dataFlow, Role role, out IMMDevice defaultDevice);
}
#pragma warning disable CS0535

[ComImport]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
[ClassInterface(ClassInterfaceType.None)]
class MMDeviceEnumeratorCom : IMMDeviceEnumerator
{
}

[ComImport]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IMMDeviceCollection
{
    int GetCount(out uint count);
    int Item(uint index, out IMMDevice device);
}

[ComImport]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IMMDevice
{
    int GetId(out string id);
    int GetState(out uint state);
    int OpenPropertyStore(uint stgmAccess, out IPropertyStore properties);
}

[ComImport]
[Guid("568B9108-44BF-40B4-9006-86AFE5B5A620")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IPolicyConfig
{
    void SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string deviceId, Role role);
}

public enum EDataFlow
{
    Render,
    Capture,
    All
}

public enum Role
{
    Console,
    Multimedia,
    Communications
}

[Flags]
public enum DeviceState
{
    Active = 1,
    Disabled = 2,
    NotPresent = 4,
    Unplugged = 8,
    All = 15
}

class MMDevice
{
    public string FriendlyName { get; set; }
    public string Id { get; set; }
}

static class MMDeviceEnumeratorExtensions
{
    public static MMDevice[] EnumerateAudioEndPoints(this IMMDeviceEnumerator enumerator)
    {
        enumerator.EnumAudioEndpoints(EDataFlow.Render, DeviceState.Active, out IMMDeviceCollection deviceCollection);
        deviceCollection.GetCount(out uint count);

        var devices = new MMDevice[count];
        for (uint i = 0; i < count; i++)
        {
            deviceCollection.Item(i, out IMMDevice device);
            device.GetId(out string id);

            device.OpenPropertyStore(0, out IPropertyStore propertyStore);
            var nameProp = new PROPERTYKEY
            {
                fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"),
                pid = 14
            };

            propertyStore.GetValue(ref nameProp, out PROPVARIANT propValue);
            devices[i] = new MMDevice
            {
                Id = id,
                FriendlyName = Marshal.PtrToStringUni(propValue.pwszVal)
            };

            Marshal.ReleaseComObject(propertyStore);
        }

        return devices;
    }
}

[ComImport]
[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IPropertyStore
{
    int GetCount(out uint count);
    int GetAt(uint iProp, out PROPERTYKEY pkey);
    int GetValue(ref PROPERTYKEY key, out PROPVARIANT pv);
    int SetValue(ref PROPERTYKEY key, ref PROPVARIANT pv);
    int Commit();
}

[StructLayout(LayoutKind.Sequential)]
struct PROPERTYKEY
{
    public Guid fmtid;
    public uint pid;
}

[StructLayout(LayoutKind.Explicit)]
struct PROPVARIANT
{
    [FieldOffset(0)]
    public ushort vt;
    [FieldOffset(8)]
    public IntPtr pwszVal;
}
