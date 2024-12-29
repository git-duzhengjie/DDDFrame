namespace Infra.WebApi.DTOs
{
    /// <summary>
    /// 枚举类型
    /// </summary>
    public class EnumDTO
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 枚举变量名
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; set; }
    }
}
