using System;
using System.IO;
using System.Linq;
using System.Globalization;
using NUnit.Framework;
using HitMe.Core;
public sealed class MultiplayerParityTests
{
    [Test] public void SharedServerFixturesMatchUnityResolver(){
        var file=Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),"../shared-fixtures/combat.csv"));
        foreach(var group in File.ReadAllLines(file).Skip(1).Where(s=>s.Length>0).Select(s=>s.Split(',')).GroupBy(r=>r[0])){
            var config=new FoundationConfig{arenaShape="roundedRectangle",arenaWidth=2000,arenaHeight=3500,cornerRadius=300};
            Func<string,double> number=s=>double.Parse(s,CultureInfo.InvariantCulture);
            var result=CombatRules.Resolve(group.Select(r=>new FighterSnapshot(r[1],int.Parse(r[2]))),group.Where(r=>int.Parse(r[2])>0).Select(r=>new LockedAction(r[1],new Point(number(r[3]),number(r[4])),new Point(number(r[5]),number(r[6])),r[7]=="true")),config);
            foreach(var row in group){Assert.AreEqual(int.Parse(row[8]),result.Health.First(h=>h.Id==row[1]).HPAfter,group.Key);Assert.AreEqual(row[9],result.Throws.FirstOrDefault(t=>t.Thrower==row[1])?.Target??"",group.Key);}
            Assert.AreEqual(group.First()[10],result.Outcome.ToString(),group.Key);
        }
    }
}
