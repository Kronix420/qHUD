using System.Collections.Generic;

namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class EntityList : RemoteMemoryObject
    {
        public IEnumerable<Entity> Entities => EntitiesAsDictionary.Values;

        public Dictionary<int, Entity> EntitiesAsDictionary
        {
            get
            {
                System.Console.WriteLine("EntityList: " + Address.ToString("x8"));
                var dictionary = new Dictionary<int, Entity>();
                CollectEntities(M.ReadInt(Address), dictionary);
                return dictionary;
            }
        }

        private void CollectEntities(int addr, Dictionary<int, Entity> list)
        {
            int num = addr;
            addr = M.ReadInt(addr + 4);
            var hashSet = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(addr);
            while (queue.Count > 0)
            {
                int nextAddr = queue.Dequeue();
                if (hashSet.Contains(nextAddr))
                    continue;

                hashSet.Add(nextAddr);
                if (nextAddr != num && nextAddr != 0)
                {
                    int key = M.ReadInt(nextAddr + 0x14, 0x14);
                    if (!list.ContainsKey(key))
                    {
                        int address = M.ReadInt(nextAddr + 0x14);
                        var entity = GetObject<Entity>(address);
                        list.Add(key, entity);
                    }
                    queue.Enqueue(M.ReadInt(nextAddr));
                    queue.Enqueue(M.ReadInt(nextAddr + 8));
                }
            }
        }
    }
}