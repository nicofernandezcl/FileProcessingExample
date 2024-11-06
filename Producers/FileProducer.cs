using System.Collections.Concurrent;

namespace FileProcessingExample.Producers;

internal class FileProducer : IFileProducer
{
    private readonly string _folderPath;
    private readonly ConcurrentQueue<string> _fileQueue;

    public FileProducer(string folderPath, ConcurrentQueue<string> fileQueue)
    {
        _folderPath = folderPath;
        _fileQueue = fileQueue;
    }

    public void EnqueueFiles(CancellationToken token)
    {
        foreach (var file in Directory.GetFiles(_folderPath))
        {
            token.ThrowIfCancellationRequested();

            _fileQueue.Enqueue(file);
            Console.WriteLine($"Archivo encolado: {file}");
        }
    }

}