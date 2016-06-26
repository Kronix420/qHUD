namespace qHUD.Poe.FilesInMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Framework;
    public class TagsDat : FileInMemory
    {
        public Dictionary<string, TagRecord> records =
            new Dictionary<string, TagRecord>(StringComparer.OrdinalIgnoreCase);

        public TagsDat(Memory m, int address) : base(m, address)
        {
            loadItems();
        }
        private void loadItems()
        {
            foreach (var r in RecordAddresses().Select(addr => new TagRecord(M, addr)))
            {
                records.Add(r.Key, r);
            }
        }

        public class TagRecord
        {
            public readonly string Key;
            public int Hash;
            public TagRecord(Memory m, int addr)
            {
                Key = m.ReadStringU(m.ReadInt(addr + 0), 255);
                Hash = m.ReadInt(addr + 4);
            }
        }
    }
}