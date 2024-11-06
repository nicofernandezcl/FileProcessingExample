using System.Collections.Concurrent;

namespace FileProcessingExample.Utilities;

internal class QueueManager : IQueueManager
{
    public ConcurrentQueue<string> FileQueue { get; }
    public ConcurrentQueue<string> DataQueue { get; }

    public QueueManager()
    {
        FileQueue = new ConcurrentQueue<string>();
        DataQueue = new ConcurrentQueue<string>();
    }
}