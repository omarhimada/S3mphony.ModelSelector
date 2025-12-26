using System;
using System.Collections.Generic;
using System.Text;

namespace S3mphony.ModelSelector {
    public class ModelMetric {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Human-friendly name, ex: "CustomerRecs-v1".
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// The filename of the stored/zip model (ex: "customer-recs-2025-12-07.zip").
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The zipped model bytes (181 kB or so).
        /// </summary>
        public byte[]? ModelZip { get; set; } = [];

        /// <summary>
        /// Regression metrics for the model.
        /// </summary>
        public string RegressionMetricsSerialized { get; set; } = string.Empty;

        /// <summary>
        /// R-squared metric for the model.
        /// </summary>
        public double RSquared { get; set; }

        /// <summary>
        /// Root mean squared error.
        /// </summary>
        public double RMSE { get; set; }

        /// <summary>
        /// When this model was trained (UTC).
        /// </summary>
        public DateTime TrainedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Short description of algorithm, eg "MatrixFactorization" or "ALS".
        /// </summary>
        public string? Algorithm { get; set; }

        /// <summary>
        /// Number of epochs / iterations (if applicable).
        /// </summary>
        public int? Epochs { get; set; }

        /// <summary>
        /// Number of unique users used in training.
        /// </summary>
        public int? UserCount { get; set; }

        /// <summary>
        /// Number of unique items (SKUs) used in training.
        /// </summary>
        public int? ItemCount { get; set; }

        /// <summary>
        /// Optional JSON or text containing hyperparameters, notes, etc.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Improvement from previous model. (R2 squared increased || RMSE decreased).
        /// </summary>
        public bool R2Improvement { get; set; }

        /// <summary>
        /// Improvement from previous model. (R2 squared increased || RMSE decreased).
        /// </summary>
        public bool RMSEImprovement { get; set; }

        /// <summary>
        /// Gets the mean absolute error of the model's predictions.
        /// </summary>
        public double MeanAbsoluteError { get; set; }

        /// <summary>
        /// Mean squared error of the model's predictions.
        /// </summary>
        public double MeanSquaredError { get; set; }

        /// <summary>
        /// Loss function value for the model.
        /// </summary>
        public double LossFunction { get; set; }
    }
}
