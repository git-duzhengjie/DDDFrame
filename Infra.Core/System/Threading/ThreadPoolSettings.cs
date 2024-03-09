
namespace System.Threading
{
    /// <summary>
    /// 线程池配置
    /// </summary>
    public class ThreadPoolSettings
    {
        /// <summary>
        /// 最小线程数
        /// </summary>
        public Int32 MinThreads { get; set; } = 300;

        public Int32 MinCompletionPortThreads { get; set; } = 300;

        /// <summary>
        /// 最大线程数
        /// </summary>
        public Int32 MaxThreads { get; set; } = 32767;

        public Int32 MaxCompletionPortThreads { get; set; } = 1000;
    }
}
