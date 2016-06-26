namespace qHUD.Poe.Components
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Enums;
    using RemoteMemoryObjects;
    public class Mods : Component
    {
        public ItemRarity ItemRarity => Address != 0 ? (ItemRarity)M.ReadInt(Address + 0x58) : ItemRarity.Normal;
        public int ItemLevel => Address != 0 ? M.ReadInt(Address + 0xE4) : 1;

        public List<ItemMod> ItemMods
        {
            get
            {
                var implicitMods = GetMods(0x5C, 0x60);
                var explicitMods = GetMods(0x68, 0x6C);
                return implicitMods.Concat(explicitMods).ToList();
            }
        }

        private IEnumerable<ItemMod> GetMods(int startOffset, int endOffset)
        {
            var list = new List<ItemMod>();
            if (Address == 0) return list;
            int begin = M.ReadInt(Address + startOffset);
            int end = M.ReadInt(Address + endOffset);
            int count = (end - begin) / 0x14;
            if (count > 12) return list;
            for (int i = begin; i < end; i += 0x14)
                list.Add(GetObject<ItemMod>(i));
            return list;
        }
    }
}