
public enum SkillCategory
{
    Match, Training
}

public enum SpeciesType
{
    Animal, Humanoid
}

public enum CoreType
{
    Discipline, Bond, Cold, Instinct
}

public enum PersonalityType
{
    Principled, Guardian, Tactician, Exemplary,
    Kind, Loyal, Sincere, Moodmaker,
    Planner, Opportunist, Cunning, Believer,
    Bluffer, Wild, Egoist, Fighter
}

public enum Nation
{
    Korea
}

public enum NameParts
{
    FirstName, MiddleName, LastName
}

public class TestStartingState
{
    public int Grade;
    public int startMin;
    public int startMax;

    public TestStartingState(int i)
    {
        if (i == 1)
        {
            Grade = 1;
            startMin = 5;
            startMax = 15;
        }
        else if (i == 2)
        {
            Grade = 2;
            startMin = 10;
            startMax = 35;
        }
        else
        {
            Grade = 3;
            startMin = 15;
            startMax = 50;
        }
    }
}

public class TestMaxPotentialData
{
    public int key = 1;
    public int minPotientialValue = 30;
    public int maxPotentialValue = 70;
}

public class TestNameData
{
    public int id;
    public Nation nation;
    public NameParts nameParts;
    public string nameKey;
    public string desc;

    public TestNameData(int i)
    {
        id = i;
        nation = Nation.Korea;
        nameKey = i.ToString() + "이름키";
        desc = i.ToString() + " ";

        if (i < 5)
        {
            nameParts = NameParts.LastName;
        }
        else if (i < 10)
        {
            nameParts = NameParts.MiddleName;
        }
        else
        {
            nameParts = NameParts.FirstName;
        }


    }
}

public class TestSpecieData
{
    public int specieId;
    public SpeciesType speciesType;
    public string speciesName;
    public string desc;

    public TestSpecieData(int i)
    {
        specieId = i;
        if (i < 8)
        {
            speciesType = SpeciesType.Humanoid;
        }
        else
        {
            speciesType = SpeciesType.Animal;
        }
        speciesName = i.ToString() + "종족명";
        desc = i.ToString() + "종족설명";
    }
}

public class TestPersonalityData
{
    public int personalityId;
    public CoreType coreType;
    public PersonalityType personalityType;
    public string personalityName;
    public string desc;

    public TestPersonalityData(int i)
    {
        personalityId = i;

        personalityType = (PersonalityType)i;
        switch (personalityType)
        {
            case PersonalityType.Principled:
            case PersonalityType.Guardian:
            case PersonalityType.Tactician:
            case PersonalityType.Exemplary:
                coreType = CoreType.Discipline;
                break;
            case PersonalityType.Kind:
            case PersonalityType.Loyal:
            case PersonalityType.Sincere:
            case PersonalityType.Moodmaker:
                coreType = CoreType.Bond;
                break;
            case PersonalityType.Planner:
            case PersonalityType.Opportunist:
            case PersonalityType.Cunning:
            case PersonalityType.Believer:
                coreType = CoreType.Cold;
                break;
            case PersonalityType.Bluffer:
            case PersonalityType.Wild:
            case PersonalityType.Egoist:
            case PersonalityType.Fighter:
                coreType = CoreType.Instinct;
                break;
        }

        personalityName = i.ToString() + "성격명";
        desc = i.ToString() + "성격설명";
    }
}

public class TestPassiveData
{
    public int skillId;
    public string skillName;
    public SkillCategory skillCategory;
    public string triggerType;
    public int triggerValue;
    public string effectType;
    public float effectValue;
    public int effectDuration;
    public int CoolTime;
    public string passiveDesc;

    public TestPassiveData(int i)
    {
        skillId = i;
        skillName = i.ToString() + "패시브명";
        if (i < 8)
        {
            skillCategory = SkillCategory.Match;
        }
        else
        {
            skillCategory = SkillCategory.Training;
        }

        triggerType = i.ToString() + "트리거타입";
        triggerValue = i;
        effectDuration = i;
        CoolTime = i;
        passiveDesc = i.ToString() + "패시브설명";
    }
}

public class TestTraitData
{
    public int traitId;
    public string traitName;
    public string desc;

    public TestTraitData(int i)
    {
        traitId = i;
        traitName = i.ToString() + "특성명";
        desc = i.ToString() + "특성설명";
    }
}