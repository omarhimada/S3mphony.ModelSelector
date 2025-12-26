using System;
using System.Collections.Generic;
using System.Text;

namespace S3mphony.ModelSelector {
    public class ModelMetric {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ModelName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public byte[]? ModelZip { get; set; } = [];

        // ─── Regression (MF, etc.) ───
        public string RegressionMetricsSerialized { get; set; } = string.Empty;
        public double RSquared { get; set; }
        public double RMSE { get; set; }
        public double MeanAbsoluteError { get; set; }
        public double MeanSquaredError { get; set; }
        public double LossFunction { get; set; }
        public bool R2Improvement { get; set; }
        public bool RMSEImprovement { get; set; }

        // ─── Classification (Churn) ───
        public string? ClassificationMetricsSerialized { get; set; } = string.Empty;

        /// <summary>Area Under ROC Curve (higher is better)</summary>
        public double AUC { get; set; }

        /// <summary>Overall accuracy (useful but can mislead on imbalanced data)</summary>
        public double Accuracy { get; set; }

        /// <summary>F1 score for the positive (churn) class (higher is better)</summary>
        public double F1Score { get; set; }

        /// <summary>Precision for churn predictions (TP / (TP + FP))</summary>
        public double Precision { get; set; }

        /// <summary>Recall for churn predictions (TP / (TP + FN))</summary>
        public double Recall { get; set; }

        /// <summary>Binary log-loss (lower is better)</summary>
        public double LogLoss { get; set; }

        /// <summary>Log-loss improvement over baseline (higher is better)</summary>
        public double LogLossReduction { get; set; }

        // ─── Generic model selection fields ───

        /// <summary>
        /// A single comparable score used by your model selector 
        /// (ex: -RMSE for regression, AUC for churn, etc.)
        /// </summary>
        public double PrimaryScore { get; set; }

        /// <summary>
        /// Name of the primary metric being used for model selection ("RMSE", "AUC", etc.)
        /// </summary>
        public string PrimaryScoreName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if higher values are better for the PrimaryScore metric
        /// </summary>
        public bool HigherIsBetter { get; set; } = true;

        /// <summary>
        /// When this model was trained (UTC)
        /// </summary>
        public DateTime TrainedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Algorithm name, ex: "MatrixFactorization", "LogisticRegression", "FastTree", etc.
        /// </summary>
        public string? Algorithm { get; set; }

        /// <summary>
        /// Number of epochs/iterations if applicable
        /// </summary>
        public int? Epochs { get; set; }

        /// <summary>
        /// Number of unique users in training (for recommenders)
        /// </summary>
        public int? UserCount { get; set; }

        /// <summary>
        /// Number of unique items (SKUs) in training (for recommenders)
        /// </summary>
        public int? ItemCount { get; set; }

        /// <summary>
        /// Freeform notes, hyperparameters, eval summary, etc.
        /// </summary>
        public string? Notes { get; set; }
    }
}
