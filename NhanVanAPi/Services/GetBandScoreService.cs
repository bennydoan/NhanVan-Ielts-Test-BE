namespace NhanVanAPi.Services
{
    public class GetBandScoreService
    {
        public static decimal GetBand(int correct)
        {
            if (correct >= 39) return 9.0m; // decimal value 
            if (correct >= 37) return 8.5m;
            if (correct >= 35) return 8.0m;
            if (correct >= 33) return 7.5m;
            if (correct >= 30) return 7.0m;
            if (correct >= 27) return 6.5m;
            if (correct >= 23) return 6.0m;
            if (correct >= 20) return 5.5m;
            if (correct >= 16) return 5.0m;
            if (correct >= 13) return 4.5m;
            if (correct >= 10) return 4.0m;
            if (correct >= 7) return 3.5m;
            if (correct >= 5) return 3.0m;
            if (correct >= 3) return 2.5m;
            return 0m;
        }
    }
}
