using System.Collections.Concurrent;
using System.Net;

namespace FileProcessingExample.Consumers;

internal class FtpConsumer : IFtpConsumer
{
    private readonly string _ftpUrl;
    private readonly ConcurrentQueue<string> _queue;
    private readonly int _maxConcurrentUploads;

    public FtpConsumer(string ftpUrl, ConcurrentQueue<string> queue, int maxConcurrentUploads = 5)
    {
        _ftpUrl = ftpUrl;
        _queue = queue;
        _maxConcurrentUploads = maxConcurrentUploads;
    }

    public void StartUploading(CancellationToken token)
    {
        List<Task> uploadTasks = new List<Task>();

        for (int i = 0; i < _maxConcurrentUploads; i++)
        {
            var task = Task.Run(() => ProcessAndUpload());
            uploadTasks.Add(task);
        }

        try
        {
            Task.WhenAll(uploadTasks).Wait(token); // Espera con soporte de cancelación
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Subida cancelada.");
        }
    }

    private void ProcessAndUpload()
    {
        while (!_queue.IsEmpty)
        {
            if (_queue.TryDequeue(out var fileContent))
            {
                // Procesamiento de archivo (simulado)
                string processedData = fileContent.ToUpper(); // Ejemplo de procesamiento

                // Subir al FTP
                UploadToFtp(processedData);
            }
        }
    }

    private void UploadToFtp(string data)
    {
        try
        {
            var request = (FtpWebRequest)WebRequest.Create(_ftpUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("usuario", "contraseña");

            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(data);
            }

            Console.WriteLine("Archivo subido al FTP.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error subiendo al FTP: {ex.Message}");
        }
    }
}