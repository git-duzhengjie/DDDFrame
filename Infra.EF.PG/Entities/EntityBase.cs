using Infra.Core.Abstract;

namespace Infra.EF.PG.Entities
{
    public abstract class EntityBase:FrameobjectBase,IEntity
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
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最后一次时间
        /// </summary>
        public DateTime LastTime { get; set; }

        public virtual void Build()
        {
            return;
        }
    }
}
