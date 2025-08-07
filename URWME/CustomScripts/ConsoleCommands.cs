using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace URWME
{
    public class ConsoleCommands
    {
        private Scripts Scripts;

        public ConsoleCommands(ReadWriteMem RW)
        {
            Scripts = new Scripts(RW);
        }

        public string Help = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("URWME.Misc.Help.txt")).ReadToEnd();
    }

    public class Scripts
    {
        ReadWriteMem RWMain;

        Player player;
        WorldTime worldTime;
        Map map;
        ItemBuffered item;
        MapObjectBuffered mapObject;

        public Scripts(ReadWriteMem RW)
        {
            RWMain = RW;

            player = new Player(RWMain);
            worldTime = new WorldTime(RWMain);
            map = new Map(RWMain);
            item = new ItemBuffered(RWMain, 0);
            mapObject = new MapObjectBuffered(RWMain, 0);
        }

        public void Teleport(int x, int y)
        {

        }

        public void Dig(int x, int y, int distance)
        {

        }

        public void SetAttribute(string Name, int Value)
        {

        }
        public void SetSkill(string Name, int Value)
        {

        }

        public void SpawnItem(string Name, int x, int y, int Value)
        {
            //List<Item> Items = item.GetItems();
            // FoundItem = Items.FirstOrDefault(c => c.Name == Name);

        }

        public void RemoveItem(string Name, int x, int y, int Value)
        {

        }
    }
}
