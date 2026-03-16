using System;
using System.Collections.Generic;
using UnityEngine;

// 경기 하나의 전체 기록을 담을 통
[Serializable]
public class MatchResultRecord
{
    public int MatchId; // 딕셔너리의 Key값을 복구하기 위한 변수
    public string HomeTeamName;
    public string AwayTeamName;
    public int HomeScore;
    public int AwayScore;
    public List<MatchLogData> FullLogs; // 1~4쿼터 전체 로그
    public List<int> HomePlayerIds = new List<int>(); // 리그 결산때 상태이상체크용
}

// SaveLoadManager 호환을 위한 데이터 컨테이너 클래스 (SaveBase 상속)
[Serializable]
public class LeagueRecordSaveData : SaveBase
{
    public List<MatchResultRecord> recordList = new List<MatchResultRecord>();
}

public class LeagueRecordManager : MonoBehaviour
{
    public static LeagueRecordManager Instance;

    // 저장용 파일 이름 정의
    private const string SAVE_FILE = "LeagueRecordSave.json";

    // 경기 ID(또는 라운드 번호)를 키값으로 하여 경기 기록을 저장하는 딕셔너리
    private Dictionary<int, MatchResultRecord> _leagueRecords = new Dictionary<int, MatchResultRecord>();

    private void Awake()
    {
        // 씬이 넘어가도 파괴되지 않는 싱글톤 세팅
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 씬이 로드될 때 저장된 경기 기록 불러오기
        LoadGame();
    }

    // 경기 종료 시 엔진에서 이 함수를 호출해 기록을 저장합니다.
    public void SaveMatchRecord(int matchId, MatchState state, List<MatchLogData> fullLogs)
    {
        MatchResultRecord record = new MatchResultRecord()
        {
            MatchId = matchId,
            HomeTeamName = state.HomeTeam.TeamName,
            AwayTeamName = state.AwayTeam.TeamName,
            HomeScore = state.HomeTeam.Score,
            AwayScore = state.AwayTeam.Score,
            FullLogs = new List<MatchLogData>(fullLogs) // 데이터 복사해서 저장
        };

        foreach (var player in state.HomeTeam.Roster)
        {
            record.HomePlayerIds.Add(player.PlayerId);
        }

        _leagueRecords[matchId] = record;
        Debug.Log($"[LeagueRecordManager] {matchId}번 경기 기록 저장 완료! (총 로그 수: {fullLogs.Count}개)");
        SaveGame();
    }

    // 나중에 지난 경기 로그를 불러올 때 쓸 함수
    public MatchResultRecord GetMatchRecord(int matchId)
    {
        Debug.Log($"[로그 확인] 결산창에서 요청한 ID: {matchId}");
        if (_leagueRecords.ContainsKey(matchId))
            return _leagueRecords[matchId];
        Debug.LogWarning($"[로그 에러] {matchId}번 기록이 없습니다! 현재 저장된 ID 목록: {string.Join(", ", _leagueRecords.Keys)}");
        return null;
    }

    // 기획 요청: 리그가 끝나면 로그를 싹 정리하는 함수
    public void ClearLeagueRecords()
    {
        _leagueRecords.Clear();
        Debug.Log("[LeagueRecordManager] 리그가 종료되어 모든 경기 로그가 초기화되었습니다.");
        SaveGame();
    }
    
    // 특정 경기의 특정 쿼터 로그만 쏙 뽑아서 반환해 주는 함수
    public List<MatchLogData> GetLogsByQuarter(int matchId, int targetQuarter)
    {
        MatchResultRecord record = GetMatchRecord(matchId);

        if (record != null && record.FullLogs != null)
        {
            // 전체 로그 중에서 Quarter 값이 targetQuarter와 일치하는 것만 걸러서 리스트로 만듦
            return record.FullLogs.FindAll(log => log.Quarter == targetQuarter);
        }

        // 기록이 없으면 빈 리스트 반환
        return new List<MatchLogData>();
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance == null) return;

        // 딕셔너리는 JsonUtility로 직렬화가 불가능하므로, 리스트로 변환하여 컨테이너에 담음
        LeagueRecordSaveData saveData = new LeagueRecordSaveData();
        saveData.recordList = new List<MatchResultRecord>(_leagueRecords.Values);

        SaveLoadManager.Instance.Save(SAVE_FILE, saveData);
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance == null) return;

        if (SaveLoadManager.Instance.TryLoad<LeagueRecordSaveData>(SAVE_FILE, out var data))
        {
            _leagueRecords.Clear();
            foreach (var record in data.recordList)
            {
                // 리스트의 데이터를 다시 딕셔너리로 복구
                _leagueRecords[record.MatchId] = record;
            }
            Debug.Log($"[LeagueRecordManager] {data.recordList.Count}개의 경기 기록 파일 로드 성공!");
        }
    }
}
