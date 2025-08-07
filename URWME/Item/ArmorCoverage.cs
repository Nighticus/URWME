using System;

namespace URWME // Unreal World MemoryManager
{
    [Flags]
    public enum ArmorCoverage
    { // Skull	Face	Neck	Shoulders	Upper arms	Elbows	Arms	Hands	Chest	Waist	Hips	Groin	Thighs	Knees	Shins	Feet
        None = 0,
        Skull = 1,
        Face = 2,
        Neck = 4,
        Shoulders = 8,
        UpperArms = 16,
        Elbows = 32,
        Arms = 64,
        Hands = 128,
        Chest = 256,
        Waist = 512,
        Hips = 1024,
        Groin = 2048,
        Thighs = 4096,
        Knees = 8192,
        Shins = 16384,
        Feet = 32768
    }

}
