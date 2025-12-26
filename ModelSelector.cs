using Microsoft.ML.Data;

namespace S3mphony.ModelSelector {
    public static class ModelSelector {
        public static RegressionSnapshot? ChooseBest(
            IReadOnlyList<RegressionSnapshot> candidates,
            ModelSelectionPolicy policy,
            RegressionSnapshot? currentBest = null) {
            if (candidates.Count == 0)
                return null;

            // 1) quality gates
            var gated = candidates
                .Where(c => c.RSquared >= policy.MinRSquared)
                .Where(c => c.Rmse <= policy.MaxRmse)
                .Where(c => !double.IsNaN(c.RSquared) && !double.IsNaN(c.Rmse))
                .ToList();

            if (gated.Count == 0)
                return null;

            // 2) normalize RMSE within gated set
            var rmseMin = gated.Min(x => x.Rmse);
            var rmseMax = gated.Max(x => x.Rmse);
            double NormRmse(double rmse) {
                if (rmseMax <= rmseMin)
                    return 1.0; // all same
                                // invert: lower rmse => higher score
                return 1.0 - ((rmse - rmseMin) / (rmseMax - rmseMin));
            }

            // 3) score
            double Score(RegressionSnapshot c) {
                var r2 = c.RSquared;                 // higher is better
                var rmseScore = NormRmse(c.Rmse);    // higher is better

                // clamp r2 into [0,1] for blend (optional but practical)
                var r2Clamped = Math.Clamp(r2, 0.0, 1.0);

                return (policy.WeightRSquared * r2Clamped) +
                       (policy.WeightRmse * rmseScore);
            }

            var ranked = gated
                .Select(c => (c, score: Score(c)))
                .OrderByDescending(x => x.score)
                .ThenByDescending(x => x.c.TrainedAtUtc)
                .ToList();

            var best = ranked[0].c;
            var bestScore = ranked[0].score;

            // 4) stability: only switch if meaningfully better
            if (currentBest is not null) {
                var currentScore = Score(currentBest);

                var improvement = bestScore - currentScore;
                if (improvement < policy.MinScoreImprovementToSwitch) {
                    // keep current, unless it fails gates
                    var currentPasses = currentBest.RSquared >= policy.MinRSquared &&
                                        currentBest.Rmse <= policy.MaxRmse;

                    if (currentPasses)
                        return currentBest;
                }
            }

            // 5) prefer newer when close
            if (ranked.Count >= 2) {
                var second = ranked[1];
                if (Math.Abs(bestScore - second.score) < policy.MinScoreImprovementToSwitch) {
                    // if second is newer and within the "prefer newer" window, pick it
                    var newer = ranked
                        .Where(x => (ranked[0].c.TrainedAtUtc - x.c.TrainedAtUtc).Duration() <= policy.PreferNewerWithin)
                        .OrderByDescending(x => x.c.TrainedAtUtc)
                        .FirstOrDefault();

                    if (newer.c is not null)
                        return newer.c;
                }
            }

            return best;
        }
    }
}
