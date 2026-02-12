using UnityEngine;

public abstract class DataReaderBase : ScriptableObject
{
    [Header("시트의 주소")][SerializeField] public string associatedSheet = "1z-A7tRX8Z3JhorvxdcPKu2imL9XlMNaMSuvbMu_C1qQ";
    [Header("스프레드 시트의 시트 이름")][SerializeField] public string associatedWorksheet = "";
    [Header("읽기 시작할 행 번호")][SerializeField] public int START_ROW_LENGTH = 4;
    [Header("읽을 마지막 행 번호")][SerializeField] public int END_ROW_LENGTH = -1;
}
