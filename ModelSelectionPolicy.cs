using System;
using System.Collections.Generic;
using System.Text;

namespace S3mphony.ModelSelector {
    public sealed class ModelSelectionPolicy {
        // Hard gates
        public double MinRSquared { get; init; } = 0.0;           // set higher in prod
        public double MaxRmse { get; init; } = double.PositiveInfinity;

        // Ranking knobs
        public double WeightRSquared { get; init; } = 0.7;        // higher better
        public double WeightRmse { get; init; } = 0.3;            // lower better

        // Stability knobs
        public double MinScoreImprovementToSwitch { get; init; } = 0.01; // 1% improvement
        public TimeSpan PreferNewerWithin { get; init; } = TimeSpan.FromDays(3);
    }

}