namespace S3mphony.ModelSelector {
    public sealed record RegressionSnapshot(
        Guid ModelId,
        string ModelName,
        DateTimeOffset TrainedAtUtc,
        double RSquared,
        double Rmse
    );
}
