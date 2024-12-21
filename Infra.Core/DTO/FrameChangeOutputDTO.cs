using Infra.Core.Abstract;

namespace Infra.Core.DTO
{
    public class FrameChangeOutputDTO
    {
        /// <summary>
        /// 新增或者更新对象
        /// </summary>
        public List<IOutputDTO> Changes { get; set; }=[];

        /// <summary>
        /// 删除对象
        /// </summary>
        public List<IOutputDTO> Deletes { get; set; } = [];

        public List<IOutputDTO> Outputs => (Changes ?? [])
            .Union(Deletes ?? []).ToList();

        /// <summary>
        /// 合并结果集
        /// </summary>
        /// <param name="frameChangeOutput"></param>
        public virtual void MergeResult(FrameChangeOutputDTO frameChangeOutput)
        {
            Changes?.RemoveWhere(x => frameChangeOutput.Outputs.Any(o => x.Id == o.Id));
            Deletes?.RemoveWhere(x => frameChangeOutput.Outputs.Any(o => x.Id == o.Id));
            Changes ??= [];
            Deletes ??= [];
            Changes.AddRange(frameChangeOutput.Changes ?? []);
            Deletes.AddRange(frameChangeOutput.Deletes ?? []);
        }
    }
}
