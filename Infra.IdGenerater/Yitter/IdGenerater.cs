﻿using Yitter.IdGenerator;

namespace Infra.IdGenerater.Yitter
{
    /// <summary>
    /// Id生成器
    /// </summary>
    public static class IdGenerater
    {
        private static bool isSet = false;
        private static readonly Lock locker = new();

        public static byte WorkerIdBitLength => 6;
        public static byte SeqBitLength => 6;
        public static int MaxWorkerId => (int)(Math.Pow(2.0, WorkerIdBitLength) - 1);
        public static int CurrentWorkerId { get; private set; } = -1;

        /// <summary>
        /// 初始化Id生成器
        /// </summary>
        /// <param name="workerId"></param>
        public static void SetWorkerId(ushort workerId)
        {
            if (isSet)
                throw new InvalidOperationException("allow only once");

            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");

            lock (locker)
            {
                if (isSet)
                    throw new InvalidOperationException("allow only once");

                YitIdHelper.SetIdGenerator(new IdGeneratorOptions(workerId)
                {
                    WorkerIdBitLength = WorkerIdBitLength,
                    SeqBitLength = SeqBitLength
                });

                CurrentWorkerId = workerId;
                isSet = true;
            }
        }

        /// <summary>
        /// 获取唯一Id，默认支持64个节点，每毫秒下的序列数6位。
        /// 在默认配置下，ID可用 71000 年不重复
        /// 在默认配置下，70年才到 js Number Max 值
        /// 默认情况下，500毫秒可以生成10W个号码。
        /// 如果需要提高速度，可以修改SeqBitLength长度。当SeqBitLength =10 ,100W个id约800毫秒。
        /// </summary>
        /// <returns>Id</returns>
        public static long GetNextId()
        {
            if (!isSet)
                throw new InvalidOperationException("please call SetIdGenerator first");

            return YitIdHelper.NextId();
        }
    }
}
