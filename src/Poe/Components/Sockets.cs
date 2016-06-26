namespace qHUD.Poe.Components
{
    using System.Collections.Generic;
    using System.Text;
    public class Sockets : Component
    {
        public int LargestLinkSize
        {
            get
            {
                if (Address == 0)
                {
                    return 0;
                }
                int num = M.ReadInt(Address + 0x3C);
                int num2 = M.ReadInt(Address + 0x40);
                int num3 = num2 - num;
                if (num3 <= 0 || num3 > 6)
                {
                    return 0;
                }
                int num4 = 0;
                for (int i = 0; i < num3; i++)
                {
                    int num5 = M.ReadByte(num + i);
                    if (num5 > num4)
                    {
                        num4 = num5;
                    }
                }
                return num4;
            }
        }

        public List<int[]> Links
        {
            get
            {
                var list = new List<int[]>();
                if (Address == 0)
                {
                    return list;
                }
                int num = M.ReadInt(Address + 0x3C);
                int num2 = M.ReadInt(Address + 0x40);
                int num3 = num2 - num;
                if (num3 <= 0 || num3 > 6)
                {
                    return list;
                }
                int num4 = 0;
                List<int> socketList = SocketList;
                for (int i = 0; i < num3; i++)
                {
                    int num5 = M.ReadByte(num + i);
                    var array = new int[num5];
                    for (int j = 0; j < num5; j++)
                    {
                        array[j] = socketList[j + num4];
                    }
                    list.Add(array);
                    num4 += num5;
                }
                return list;
            }
        }

        public List<int> SocketList
        {
            get
            {
                var list = new List<int>();
                if (Address == 0)
                {
                    return list;
                }
                int num = Address + 12;
                for (int i = 0; i < 6; i++)
                {
                    int num2 = M.ReadInt(num);
                    if (num2 >= 1 && num2 <= 4)
                    {
                        list.Add(M.ReadInt(num));
                    }
                    num += 4;
                }
                return list;
            }
        }

        public int NumberOfSockets => SocketList.Count;

        public List<string> SocketGroup
        {
            get
            {
                var list = new List<string>();
                foreach (var current in Links)
                {
                    var sb = new StringBuilder();
                    foreach (var color in current)
                    {
                        if (color == 1)
                        {
                            sb.Append("R");
                        }
                        else if (color == 2)
                        {
                            sb.Append("G");
                        }
                        else if (color == 3)
                        {
                            sb.Append("B");
                        }
                        else if (color == 4)
                        {
                            sb.Append("W");
                        }
                    }
                    list.Add(sb.ToString());
                }
                return list;
            }
        }
    }
}