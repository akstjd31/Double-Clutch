//포지션과 추천에 필요한 능력치를 묶기 위한 데이터 클래스

public class PositionOfferData
{
    public Position Position;
    public potential MainPotential1;
    public potential MainPotential2;
    public potential SubPotential;

    public PositionOfferData(Position position)
    {
        Position = position;
        SetPotential(position);
    }

    private void SetPotential(Position position)
    {
        switch (position)
        {
            case Position.C:
                MainPotential1 = potential.StatRebound;
                MainPotential2 = potential.StatBlock;
                SubPotential = potential.Stat2pt;
                break;
            case Position.PF:
                MainPotential1 = potential.StatRebound;
                MainPotential2 = potential.Stat2pt;
                SubPotential = potential.StatBlock;
                break;
            case Position.SF:
                MainPotential1 = potential.Stat2pt;
                MainPotential2 = potential.Stat3pt;
                SubPotential = potential.StatSteal;
                break;
            case Position.SG:
                MainPotential1 = potential.Stat3pt;
                MainPotential2 = potential.Stat2pt;
                SubPotential = potential.StatPass;
                break;
            case Position.PG:
                MainPotential1 = potential.StatPass;
                MainPotential2 = potential.StatSteal;
                SubPotential = potential.Stat3pt;
                break;
        }
    }
}
