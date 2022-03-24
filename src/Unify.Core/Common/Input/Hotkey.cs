using ProtoBuf;

namespace Unify.Core.Common.Input;

[ProtoContract]
public sealed class Hotkey
{
    [ProtoMember(1)]
    public Key Key { get; }
    [ProtoMember(2)]
    public KeyModifiers Modifiers { get; }

    public static bool TryParse(string input, out Hotkey value)
    {
        string[] args = input.Split(':');
        value = null;

        if (args.Length == 0)
            return false;

        if (!Enum.TryParse<Key>(args[0], true, out var key))
            return false;

        KeyModifiers mods = 0;

        for (int i = 1; i < args.Length; i++)
        {
            string modStr = args[i];

            if (modStr == KeyModifiers.Alt.ToString())
                mods |= KeyModifiers.Alt;
            else if (modStr == KeyModifiers.Ctrl.ToString())
                mods |= KeyModifiers.Ctrl;
            else if (modStr == KeyModifiers.Shift.ToString())
                mods |= KeyModifiers.Shift;

            if (mods == 0)
                return false;
        }

        value = new Hotkey(key, mods);
        return true;
    }

    public Hotkey(Key key, KeyModifiers mods)
    {
        Key = key;
        Modifiers = mods;
    }

    private Hotkey()
    {
        Key = Key.None;
        Modifiers = KeyModifiers.None;
    }

    public static KeyModifiers CreateKeyModifiers(bool alt, bool ctrl, bool shift, bool win)
    {
        KeyModifiers mods = 0;

        if (alt)
            mods |= KeyModifiers.Alt;
        if (ctrl)
            mods |= KeyModifiers.Ctrl;
        if (shift)
            mods |= KeyModifiers.Shift;
        if (win)
            mods |= KeyModifiers.Win;

        return mods;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is not Hotkey hk2)
            return false;

        return Key == hk2.Key && Modifiers == hk2.Modifiers;
    }

    public static bool operator ==(Hotkey a, Hotkey b)
    {
        if (a is null && b is null)
            return true;

        if(a is null && b is not null)
        {
            return false;
        }

        if (a is not null && b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Hotkey a, Hotkey b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"{Modifiers}:{Key}";
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}