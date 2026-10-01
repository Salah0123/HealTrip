namespace HealTrip.Api.Services.Ai;

public class SafetyTriageService
{
    private static readonly string[] EmergencyIndicators =
    {
         // English
        "severe chest pain",
        "difficulty breathing",
        "shortness of breath",
        "fainting",
        "loss of consciousness",
        "heavy bleeding",

        // Arabic
        "ألم شديد في الصدر",
        "ألم شديد بالصدر",
        "صعوبة في التنفس",
        "ضيق في التنفس",
        "ضيق نفس",
        "إغماء",
        "فقدان الوعي",
        "نزيف شديد"
    };

    public bool MayRequireUrgentCare(string message)
    {
        var normalized = message.ToLowerInvariant();

        return EmergencyIndicators.Any(
            indicator => normalized.Contains(indicator));
    }
}