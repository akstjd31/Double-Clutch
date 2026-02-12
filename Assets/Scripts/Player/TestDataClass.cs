
public enum SkillCategory_Test
{
    Match, Training
}

public enum SpeciesType_Test
{
    Animal, Humanoid
}

public enum CoreType_test
{
    Discipline, Bond, Cold, Instinct
}

public enum PersonalityType_Test
{
    Principled, Guardian, Tactician, Exemplary,
    Kind, Loyal, Sincere, Moodmaker,
    Planner, Opportunist, Cunning, Believer,
    Bluffer, Wild, Egoist, Fighter
}

public enum Nation_Test
{
    Korea
}

public enum NameParts_Test
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
    public Nation_Test nation;
    public NameParts_Test nameParts;
    public string nameKey;
    public string desc;

    public TestNameData(int i)
    {
        id = i;
        nation = Nation_Test.Korea;
        nameKey = i.ToString() + "이름키";
        desc = i.ToString() + " ";

        if (i < 5)
        {
            nameParts = NameParts_Test.LastName;
        }
        else if (i < 10)
        {
            nameParts = NameParts_Test.MiddleName;
        }
        else
        {
            nameParts = NameParts_Test.FirstName;
        }


    }
}

public class TestSpecieData
{
    public int specieId;
    public SpeciesType_Test speciesType;
    public string speciesName;
    public string desc;

    public TestSpecieData(int i)
    {
        specieId = i;
        if (i < 8)
        {
            speciesType = SpeciesType_Test.Humanoid;
        }
        else
        {
            speciesType = SpeciesType_Test.Animal;
        }
        speciesName = i.ToString() + "종족명";
        desc = i.ToString() + "종족설명";
    }
}

public class TestPersonalityData
{
    public int personalityId;
    public CoreType_test coreType;
    public PersonalityType_Test personalityType;
    public string personalityName;
    public string desc;

    public TestPersonalityData(int i)
    {
        personalityId = i;

        personalityType = (PersonalityType_Test)i;
        switch (personalityType)
        {
            case PersonalityType_Test.Principled:
            case PersonalityType_Test.Guardian:
            case PersonalityType_Test.Tactician:
            case PersonalityType_Test.Exemplary:
                coreType = CoreType_test.Discipline;
                break;
            case PersonalityType_Test.Kind:
            case PersonalityType_Test.Loyal:
            case PersonalityType_Test.Sincere:
            case PersonalityType_Test.Moodmaker:
                coreType = CoreType_test.Bond;
                break;
            case PersonalityType_Test.Planner:
            case PersonalityType_Test.Opportunist:
            case PersonalityType_Test.Cunning:
            case PersonalityType_Test.Believer:
                coreType = CoreType_test.Cold;
                break;
            case PersonalityType_Test.Bluffer:
            case PersonalityType_Test.Wild:
            case PersonalityType_Test.Egoist:
            case PersonalityType_Test.Fighter:
                coreType = CoreType_test.Instinct;
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
    public SkillCategory_Test skillCategory;
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
            skillCategory = SkillCategory_Test.Match;
        }
        else
        {
            skillCategory = SkillCategory_Test.Training;
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