using System.Collections.Concurrent;

interface IQueueManager
{
    ConcurrentQueue<string> FileQueue { get; }
    ConcurrentQueue<string> DataQueue { get; }
}