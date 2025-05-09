﻿namespace Infra.Core.Abstract
{
    public interface IEntity:IObject
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最后一次时间
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// 输出对象
        /// </summary>
        public IOutputDTO Output { get; }

        /// <summary>
        /// 输入对象
        /// </summary>
        public IInputDTO Input { get; }

        /// <summary>
        /// 构建
        /// </summary>
        public void Build();
    }
}
