namespace S3mphony.ModelSelector.Utility {
    /// <summary>
    /// Specifies the source or metric used to calculate a model's score in evaluation scenarios.
    /// </summary>
    /// <remarks>Use this enumeration to indicate which statistical measure or metric is being used to
    /// represent the model's performance. Common values include area under the curve (AUC), R-squared, negative root
    /// mean squared error (NegRmse), negative log loss (NegLogLoss), and micro accuracy. The Unknown value can be used
    /// when the score source is not specified or cannot be determined.</remarks>
    public enum ScoreSource { Auc, RSquared, NegRmse, NegLogLoss, MicroAccuracy, Unknown }

    /// <summary>
    /// Represents a ranked result produced by evaluating a workflow metric, including its score, source, and additional
    /// details.
    /// </summary>
    /// <param name="Metric">The workflow metric that was evaluated to produce this ranking. Cannot be null.</param>
    /// <param name="Score">The numeric score assigned to the metric. Higher values typically indicate better performance or relevance.</param>
    /// <param name="Source">The origin of the score, indicating how the score was determined.</param>
    /// <param name="Detail">Additional information or context about the ranking, such as calculation notes or metric-specific details.</param>
    public sealed record RankedModel(
        IWorkflowMetric Metric,
        double Score,
        ScoreSource Source,
        string Detail
    );

    public static class ModelPicker {
        /// <summary>
        /// Selects the highest-ranked model from a collection of workflow metrics, using score and completion time as
        /// criteria.
        /// </summary>
        /// <remarks>Models are ranked primarily by score, with higher scores preferred. If multiple
        /// models share the highest score, the one with the most recent training completion is selected as a
        /// tie-breaker. Metrics that cannot be ranked are excluded from consideration.</remarks>
        /// <param name="items">The collection of workflow metrics to evaluate. Cannot be null. Each metric is assessed for ranking; metrics
        /// that cannot be ranked are ignored.</param>
        /// <returns>A RankedModel representing the best candidate based on score and most recent completion time, or null if no
        /// valid candidates are found.</returns>
        public static RankedModel? PickBest(IEnumerable<IWorkflowMetric> items) {
            RankedModel? ranked = items
                .Select(TryRank)
                .Where(x => x is not null)
                .Select(x => x!)
                // Higher is better (we negate error metrics below)
                .OrderByDescending(x => x.Score)
                // Tie-break: most recent completion
                .ThenByDescending(x => x.Metric.TrainedAtUtc)
                .FirstOrDefault();

            return ranked;
        }

        /// <summary>
        /// Attempts to rank the specified workflow metric using the most appropriate available scoring metric.
        /// </summary>
        /// <remarks>The method prioritizes metrics in the following order: AUC, R-squared, MicroAccuracy,
        /// RMSE (negated), and LogLoss (negated). Lower-is-better metrics are negated to ensure consistent ranking. If
        /// none of the metrics are present or valid, the method returns null.</remarks>
        /// <param name="m">The workflow metric to evaluate and rank. Must contain at least one valid scoring metric (such as AUC,
        /// R-squared, MicroAccuracy, RMSE, or LogLoss).</param>
        /// <returns>A RankedModel instance representing the ranked metric if a valid score is found; otherwise, null.</returns>
        private static RankedModel? TryRank(IWorkflowMetric m) {
            // Binary AUC if present
            double? auc = m.AUC;
            if (auc is double a && IsFinite(a))
                return new RankedModel(m, a,
                ScoreSource.Auc, $"AUC = {a:0.####}");

            // Regression R²
            double? r2 = m.RSquared;
            if (r2 is double r && IsFinite(r))
                return new RankedModel(m, r,
                ScoreSource.RSquared, $"R2 = {r:0.####}");

            // See Microsoft.ML.MulticlassClassificationMetrics for details on MicroAccuracy
            double? micro = m.Multiclass?.MicroAccuracy;
            if (micro is double mi && IsFinite(mi))
                return new RankedModel(m, mi,
                ScoreSource.MicroAccuracy, $"Micro Accuracy = {mi:0.####}");

            // 4) Lower-is-better metrics → negate
            double? rmse = m.RMSE;
            if (rmse is double e && IsFinite(e))
                return new RankedModel(m, -e,
                ScoreSource.NegRmse, $"R.M.S.E. = {e:0.####} (score =- RMSE)");

            double? logloss = m?.LogLoss;
            return logloss is double ll && IsFinite(ll)
                ? new RankedModel(m, -ll,
                ScoreSource.NegLogLoss, $"Log Loss = {ll:0.####} (score =- LogLoss)")
                : null;
        }

        /// <summary>
        /// Determines whether the specified double-precision floating-point value is finite.
        /// </summary>
        /// <param name="x">The double-precision floating-point value to evaluate.</param>
        /// <returns>true if the value is neither NaN nor infinity; otherwise, false.</returns>
        private static bool IsFinite(double x) => !double.IsNaN(x) && !double.IsInfinity(x);
    }
}
