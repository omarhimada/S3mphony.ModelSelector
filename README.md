# S3mphony.ModelSelector
##
`S3mphony.ModelSelection` adds an ML-aware decision layer on top of your persisted S3 models, letting you automatically determine which trained model is best suited for predictions — using sane, mathematically sound comparisons over R², RMSE, and training recency.

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
  // Pull your candidate regression snapshots from persisted model runs in S3
var candidates = await S3Channel.ListRegressionSnapshotsAsync("models/customer-recs");

// Choose the best model using a policy that fits your problem
var policy = new ModelSelectionPolicy
{
    MinRSquared = 0.25,                  // minimum model quality gate
    MaxRmse = 5.0,                       // optional RMSE ceiling for switching
    WeightRSquared = 0.8,                // R² is the dominant ranking signal
    WeightRmse = 0.2,                    // lower RMSE still matters
    MinScoreImprovementToSwitch = 0.01,  // avoid churn unless it's at least 1% better
    PreferNewerWithin = TimeSpan.FromDays(2)
};

// Select the best model
var best = ModelSelector.ChooseBest(candidates, policy);

// Load it and use it for predictions
if (best is not null)
{
    var modelBytes = await S3Channel.DownloadModelAsync(best.ModelName, best.ModelId);
    var model = ModelLoader.Load(modelBytes);
    var prediction = model.Predict(input);
}

```
###
How it works (from a developer perspective)
- Candidate models are scanned in S3
- Metrics are parsed into normalized regression snapshots
- Invalid models are removed using quality gates
- Remaining models are ranked using a composite score
- The best model is returned with model + metric metadata
- Caller loads the model blob from S3 and predicts
- All of this is done with minimal boilerplate, no fragile configuration, and no repeated bucket calls, making it ideal for scheduled CI training loops, ML prototypes, and cache-aware inference layers.
- Designed for clean MLOps flows
- Whether you're training 100 models or 1,000, S3mphony.ModelSelection treats them as candidates, not static artifacts. Your system always feels intelligent, responsive, and efficient — more like a curated prediction advisor, less like a storage client.

Best practices
- Use ModelSelection for models trained on the same dataset or target scale
- Tune policy gates for your problem domain
- Inject S3Channel into workers or APIs and let it manage caching and concurrency
- Combine with AddOutputCache() and AddMemoryCache() for fast, non-empty UI hydration
- Persist only successful training runs for the cleanest candidate pool

```
TODO:
1) Selection logic (which model to serve)
    - Choose min RMSE
    - tie-break by min MAE
    - tie-break by max R²
    - then newest timestamp
2) Recommendation logic (what options to run next)
- Given history and current best (rank=20, iters=500):
- -Generate candidate ranks: {20*2, 20*4, 20*6} → {40, 80, 120}
- Generate lambda sweep around current best lambda (0.05): {0.01, 0.05, 0.10, 0.20}
- Anchor LR, alpha, iters constant
- Remove already-tried configs
```