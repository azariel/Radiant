using System.Collections.Generic;
using System.Windows.Media;

namespace Radiant.Custom.Games.EveOnline.EveFight.Configuration;

public class ThreatColorByDps
{
    public ThreatColorByDps()
    {
        //if (this.ThreatColorDpsList == null)
        //    this.ThreatColorDpsList = new List<ThreatColorDps>();

        //if (this.ThreatColorDpsList.Count <= 0)
        //{
        //    this.ThreatColorDpsList.AddRange(new List<ThreatColorDps>{

        //        new ThreatColorDps
        //        {
        //            Treshold = -1,
        //            Color = new(Color.FromArgb(150, 0, 175, 0))
        //        },
        //        new ThreatColorDps
        //        {
        //            Treshold = 100,
        //            Color = new(Color.FromArgb(150, 175, 175, 0))
        //        },
        //        new ThreatColorDps
        //        {
        //            Treshold = 350,
        //            Color = new(Color.FromArgb(150, 200, 125, 0))
        //        },
        //        new ThreatColorDps
        //        {
        //            Treshold = 700,
        //            Color = new(Color.FromArgb(150, 175, 0, 0))
        //        },
        //        new ThreatColorDps
        //        {
        //            Treshold = 1000,
        //            Color = new(Color.FromArgb(150, 255, 0, 0))
        //        }});
        //}
    }

    public List<ThreatColorDps> ThreatColorDpsList { get; set; }
}

public class ThreatColorDps
{
    public int Treshold { get; set; }
    public SolidColorBrush Color { get; set; }
}
