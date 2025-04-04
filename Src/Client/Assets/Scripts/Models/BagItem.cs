using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Models
{
    //确保结构体的字段在内存中按声明顺序紧密排列，并强制指定内存对齐方式为 1 字节
    //处理二进制数据流（如网络协议、文件格式）
    [StructLayout(LayoutKind.Sequential,Pack =1)]
    struct BagItem
    {

        public ushort ItemId;
        public ushort Count;


        //定义一个全局可访问的空物品实例（ItemId和Count均为0）。
        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };

        public BagItem(int itemId,int count)
        {
            this.ItemId = (ushort)itemId;
            this.Count = (ushort)count;
        }

        public static bool operator == (BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;//id相等 返回true  否则相反
        }

        public static bool operator != (BagItem lhs,BagItem rhs)//同上
        {
            return !(lhs == rhs);

        }

        /// <summary>
        /// <para>Returns true if this objects are equal.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other  is BagItem)
            {
                return Equals((BagItem)other);
            }
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
        }
    }
}
