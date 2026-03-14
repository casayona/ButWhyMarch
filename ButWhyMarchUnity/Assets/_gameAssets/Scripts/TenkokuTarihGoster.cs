using UnityEngine;
using TMPro;
using Tenkoku.Core;
public class TenkokuTarihGoster : MonoBehaviour
{
    public TMP_Text textUI;
    public TenkokuModule tenkoku;

    void Update()
    {
        int day = tenkoku.currentDay;
        int month = tenkoku.currentMonth;
        int year = tenkoku.currentYear;

        int hour = tenkoku.currentHour;
        int minute = tenkoku.currentMinute;

        textUI.text =
            day.ToString("00") + " / " +
            month.ToString("00") + " / " +
            year + "\n" +
            hour.ToString("00") + ":" +
            minute.ToString("00");
    }
}
