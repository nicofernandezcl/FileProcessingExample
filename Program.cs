using FileProcessingExample.Consumers;
using FileProcessingExample.Processors;
using FileProcessingExample.Producers;
using FileProcessingExample.Utilities;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    private static void Main(string[] args)
    {

        string folderPath = "Ruta/De/Tu/Carpeta";
        string ftpUrl = "ftp://tu-servidor-ftp.com/";

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IFileProducer>(provider => new FileProducer(folderPath, provider.GetRequiredService<IQueueManager>().FileQueue))
            .AddSingleton<IDataProcessor, DataProcessor>()
            .AddSingleton<IFtpConsumer>(provider => new FtpConsumer(ftpUrl, provider.GetRequiredService<IQueueManager>().DataQueue, maxConcurrentUploads: 5))
            .AddSingleton<IQueueManager, QueueManager>()
            .BuildServiceProvider();

        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var fileProducer = serviceProvider.GetRequiredService<IFileProducer>();
        var dataProcessor = serviceProvider.GetRequiredService<IDataProcessor>();
        var ftpConsumer = serviceProvider.GetRequiredService<IFtpConsumer>();

        var queueManager = serviceProvider.GetRequiredService<QueueManager>();

        Task producerTask = Task.Run(() => ExecuteTask(fileProducer.EnqueueFiles, "Productor", token));
        Task processorTask = Task.Run(() => ExecuteTask(dataProcessor.ProcessFiles, "Procesador", token));
        Task uploaderTask = Task.Run(() => ExecuteTask(token => ftpConsumer.StartUploading(token), "FTP Consumidor", token));


        try
        {
            Task.WhenAll(producerTask, processorTask, uploaderTask).Wait();
            Console.WriteLine("Proceso completo.");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine("Errores en las tareas:");
            foreach (var innerEx in ex.InnerExceptions)
            {
                Console.WriteLine($"- {innerEx.Message}");
            }
        }
    }

    private static void ExecuteTask(Action<CancellationToken> taskAction, string taskName, CancellationToken token)
    {
        try
        {
            taskAction.Invoke(token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"La tarea {taskName} fue cancelada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la tarea {taskName}: {ex.Message}");
        }
    }
}