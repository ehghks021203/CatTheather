public static class RandomCalc {
    public static bool ProbabilityCalc(float probability) {
        float randomValue = UnityEngine.Random.value;
        return randomValue < probability;
    }
}