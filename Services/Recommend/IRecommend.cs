namespace Services.Recommend
{
    public interface IRecommend
    {
        void LoadData();

        void Train();

        void Evaluate();

        void Prediction();

        void Result();
    }
}