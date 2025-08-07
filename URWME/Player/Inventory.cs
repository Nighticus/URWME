using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class Inventory
    {
        public ReadWriteMem RWMain;
        public ItemArray Items;

        public Inventory(ReadWriteMem RW)
        {
            RWMain = RW;
            Items = new ItemArray(RWMain);
        }
        public class ItemArray
        {
            ReadWriteMem RWMain;
            public ItemArray(ReadWriteMem RW)
            {
                RWMain = RW;
            }

            public Item this[int Index]
            {
                get { return new Item(RWMain, RWMain.Read<int>(Address.PC_Items_IDs + (Index * 4)) - 50000); }// { Quantity = RWMain.Read<int>(Address.PC_Items_Counts + (Index * 4)) }; }
            }

            public byte[] IDs
            {
                get { return RWMain.Read<byte[]>(Address.PC_Items_IDs, 400); } // confirm max length of 100
                set { RWMain.Write(Address.PC_Items_IDs, value); }
            }
            public byte[] Counts
            {
                get { return RWMain.Read<byte[]>(Address.PC_Items_Counts, 400); }
                set { RWMain.Write(Address.PC_Items_Counts, value); }
            }

            public bool IsMaxCapacity
            {
                get
                {
                    byte[] ItemIDs = IDs;
                    for (int i = 0; i < 100; i++)
                    {
                        int ID = BitConverter.ToInt32(ItemIDs, i * 4);
                        if (ID == 0) { return false; }
                    }
                    return true;
                }
            }

            public float GetWeight()
            {
                float TotalWeight = 0f;
                Item Item = new Item(RWMain, 0);
                Dictionary<int, int> Items = AsDictionary;

                foreach (int key in Items.Keys)
                {
                    Item.Index = key - 50000;
                    TotalWeight += Item.Weight * Items[key];
                }
                return TotalWeight;
            }

            public Dictionary<int, int> AsDictionary
            {
                get
                {
                    Dictionary<int, int> Return = new Dictionary<int, int>();
                    byte[] ItemIDs = IDs; byte[] ItemCounts = Counts;
                    for (int i = 0; i < 100; i++)
                    {
                        int ID = BitConverter.ToInt32(ItemIDs, i * 4);
                        int Count = BitConverter.ToInt32(ItemCounts, i * 4);
                        if (ID >= 50000)
                        {
                            if (!Return.ContainsKey(ID))
                            {
                                Return.Add(ID, Count);
                            }
                            else
                            {
                                // ???
                            }
                        }
                    }
                    return Return;
                }
                set
                {
                    int Index = 0;
                    byte[] ItemIDs = IDs; byte[] ItemCounts = Counts;
                    foreach (int i in value.Keys)
                    {
                        byte[] ID = BitConverter.GetBytes(i).ToArray();
                        byte[] Count = BitConverter.GetBytes(value[i]).ToArray();
                        Array.Copy(ID, 0, ItemIDs, Index * 4, ID.Length);
                        Array.Copy(Count, 0, ItemCounts, Index * 4, Count.Length);
                        Index++;
                    }
                    for (int i = Index; i < 100; i++)
                    {
                        Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, ItemIDs, i * 4, 4);
                        Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, ItemCounts, i * 4, 4);
                    }
                    IDs = ItemIDs; Counts = ItemCounts;
                }
            }
        }
    }

}
