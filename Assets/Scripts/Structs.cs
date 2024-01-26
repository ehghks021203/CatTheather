using System;

namespace Structs
{
    [Serializable]
    public struct ScoreData
    {
        public int totalScore;
        public int ticketScore;
        public int foodScore;
        public int etcScore;

        public int correctFood;
        public int totalFood;

        public ScoreData(int totalScore, int ticketScore, int foodScore, int etcScore, int correctFood, int totalFood) {
            this.totalScore = totalScore;
            this.ticketScore = ticketScore;
            this.foodScore = foodScore;
            this.etcScore = etcScore;
            this.correctFood = correctFood;
            this.totalFood = totalFood;
        }
    }
}