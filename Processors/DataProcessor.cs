using System.Collections.Concurrent;

namespace FileProcessingExample.Processors;

internal class DataProcessor : IDataProcessor
    {
        private readonly ConcurrentQueue<string> _fileQueue;
        private readonly ConcurrentQueue<string> _dataQueue;

        public DataProcessor(ConcurrentQueue<string> fileQueue, ConcurrentQueue<string> dataQueue)
        {
            _fileQueue = fileQueue;
            _dataQueue = dataQueue;
        }

        public void ProcessFiles(CancellationToken token)
        {
            while (!_fileQueue.IsEmpty || !_dataQueue.IsEmpty)
            {
                token.ThrowIfCancellationRequested();

                if (_fileQueue.TryDequeue(out var file))
                {
                    string processedData = File.ReadAllText(file).ToUpper(); // Simulación de procesamiento
                    _dataQueue.Enqueue(processedData);
                    Console.WriteLine($"Archivo procesado y encolado para subida: {file}");
                }
            }
        }
    }