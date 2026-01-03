# S3mphony.ModelSelector
##
`S3mphony.ModelSelection` adds an ML-aware decision layer as a post-process for model persistence.
It ranks specified workflow metric using the most appropriate available scoring metric.

- It is generic, algorithm and metric-agnostic, and built for repeated scheduled training runs (like your metrics workers), where every models can belong to different targets, and is safely persisted in S3.
- What it gives you:
  - Automatic best-model selection from any number of persisted training runs
  - Quality gates to filter out invalid or underperforming candidates
  - Composite scoring that prefers high R² and low RMSE
  - Stability policy to avoid model flapping or churn
  - Recency biasing when metric scores are close
  - Works seamlessly in APIs, workers, dashboards, or prototype MLOps flows

![NuGet Version](https://img.shields.io/nuget/v/S3mphony?style=flat)

### Example usage
 ```csharp

// e.g.: S3mphony chanel setup to retrieve model metrics from your bucket
var allCandidates = await _s3Channel.ListModelsAsync(
    bucket: "my-ml-model-metrics-bucket",
    prefix: "regression-models/",
    ct: ct);

// Pick the best model without scheduling a meeting with a scientist 
var myBestModel = ModelSelector.PickBest(allCandidates);

```