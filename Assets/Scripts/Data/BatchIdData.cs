using UnityEngine;

// 배치 ID 데이터
public class BatchIdData : SaveBase
{
    public int[] batchIds;

    public BatchIdData(int[] batchIds)
    {
        this.batchIds = batchIds;   
    }

    public BatchIdData() { }
}
