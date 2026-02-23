using System;

namespace ProjetoLAWBD.Extensions {
    public static class DateExtensions {
        public static string TempoDecorrido(this DateTime data) {
            var timeSpan = DateTime.Now - data;

            if (timeSpan.TotalMinutes < 1)
                return "Agora mesmo";

            if (timeSpan.TotalMinutes < 60)
                return $"Há {timeSpan.Minutes} min";

            if (timeSpan.TotalHours < 24)
                return $"Há {timeSpan.Hours} horas";

            if (timeSpan.TotalDays < 2)
                return "Ontem";

            if (timeSpan.TotalDays < 30)
                return $"Há {timeSpan.Days} dias";

            if (timeSpan.TotalDays < 365) {
                int meses = (int)(timeSpan.TotalDays / 30);
                return meses == 1 ? "Há 1 mês" : $"Há {meses} meses";
            }

            return "Há mais de 1 ano";
        }
    }
}