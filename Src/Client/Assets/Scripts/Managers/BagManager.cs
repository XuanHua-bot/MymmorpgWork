using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class BagManager: Singleton<BagManager>
    {

        public int Unlocked;//解锁的第几个格子

        public BagItem[] Items;

        NBagInfo Info;//网络传入的 背包信息  解锁格子 和 物品的字节数组

        public unsafe void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            // 如果 是空的  或者  物品数组的长度大于 解锁格子数
            if (info.Items !=null && info.Items.Length >= this.Unlocked)
            {
                
                Analyze(info.Items);
            }
            else
            {
                //从新建立数组并 reset背包整理
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }

        public void Reset()//背包整理
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                //如果 道具管理器中的 道具数量小于堆叠数量
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    //直接填入背包的格子
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                //如果超出了堆叠数量 则进行拆分
                else
                {
                    //记录道具数量
                    int count = kv.Value.Count;

                    //道具数量大于 最大堆叠数量时
                    while (count > kv.Value.Define.StackLimit)
                    {
                        //把道具 最大堆叠数量放入格子
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;

                        //下一个格子
                        i++;
                        //剩下的道具数量
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }
        unsafe void Analyze(byte[] data)//内存字节数组 解析为结构体数组
        {
            //fixed  执行过程中 地址 不发生改变
            //指针
            fixed(byte * pt = data)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    //偏移一定的 字节 获取 item对应的
                    //BagItem的指针 指向了    开始的指针  当前第几个格子  一个格子的结构占几个字节
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;//bagitem的数组
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()//结构体数组 解析为内存字节数组
        {
            fixed (byte*pt = Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem*item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
           

            }
            return this.Info;



        }
    }
}
