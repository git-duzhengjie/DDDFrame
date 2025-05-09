﻿using Infra.Core.Abstract;
using Infra.Core.Attributes;
using Infra.EF.Service;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.EF.Entities
{
    [NotMapped]
    public abstract class EntityBase:FrameObjectBase,IEntity
    {
        [NotMapped]
        public bool IsNew { get; set; }
        /// <summary>
        /// 唯一id
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [NotSet]
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最后一次时间
        /// </summary>
        public virtual DateTime LastTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract IInputDTO Input { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract IOutputDTO Output { get; }

        public virtual void Build()
        {
            if (Id <= 0)
            {
                Id= IdGenerater.Yitter.IdGenerater.GetNextId();
            }
            if (IsNew)
            {
                CreateTime = DateTime.Now;
            }
            else
            {
                UpdateTime = DateTime.Now;
            }
            LastTime = DateTime.Now;
            return;
        }

        public virtual void Build(IDomainServiceContext domainServiceContext)
        {

        }
    }
}
