interface IFileProducer
{
    void EnqueueFiles(CancellationToken token);
}