# Double-Clutch

> 농구 팀을 육성하고 리그를 제패하는 **스포츠 경영 시뮬레이션 게임**

기업 협약 프로젝트 | Unity 6 | 한국어 · English · 日本語

---

## 게임 소개

Double-Clutch는 감독이 되어 팀을 직접 운영하는 농구 경영 시뮬레이션입니다.
선수를 스카우트하고 훈련시키며, 시설을 확장하고, 리그에서 경쟁해 최고의 팀을 만들어가는 것이 목표입니다.

---

## 주요 기능

| 기능 | 설명 |
|---|---|
| **선수 육성** | 신입생 스카우트, 개인/팀 훈련, 포지션별 성장 관리 |
| **리그 / 토너먼트** | 스위스 방식 & 토너먼트 방식 대진 생성, 순위 산정 |
| **경기 시뮬레이션** | 실시간 경기 흐름 시뮬레이션 및 결과 처리 |
| **인프라 관리** | 팀 시설 건설·업그레이드로 팀 능력치 강화 |
| **랜덤 이벤트** | 매 주기 발생하는 돌발 이벤트로 팀 운영에 변수 추가 |
| **졸업 시스템** | 선수 진급·졸업 처리 및 앨범 기록 |
| **다국어 지원** | 한국어 / 영어 / 일본어 실시간 전환 |

---

## 기술 스택

- **Engine**: Unity 6000.2.10f1
- **Render Pipeline**: Universal Render Pipeline (URP 17.2.0)
- **UI**: TextMeshPro
- **Data**: Google Sheets → ScriptableObject 자동 파싱 (GoogleSheetsToUnity)
- **Animation**: Unity 2D Animation + DotWeen
- **저장**: JSON 기반 SaveLoad 시스템

---

## 아키텍처

```
┌─────────────────────────────────────────────┐
│                  Game State                  │
│  Main → Lobby → Event → Match → Result ...  │
└────────────────────┬────────────────────────┘
                     │
        ┌────────────┼
        ▼            ▼            
   Manager Layer   Data Layer
   (Singleton) (ScriptableObject)
        │
   ┌────┴──────────────────────────┐
   │ GameManager  CalendarManager  │
   │ StringManager  AudioManager   │
   │ SaveLoadManager  InfraManager │
   └───────────────────────────────┘
```

| 레이어 | 설명 |
|---|---|
| **State Machine** | 게임 흐름 전체를 상태(State)로 관리 (Lobby, MatchPrep, MatchSim, Graduation ...) |
| **Manager** | 전역 싱글톤 매니저들이 각 도메인 담당 |
| **Command Pattern** | 농구 경기 시뮬레이션에서 슛, 패스 등 플레이 액션을 Command로 관리해 실행 흐름과 UI 의존성 분리 |
| **Parser** | Google Sheets 데이터를 Inspector 버튼 1클릭으로 ScriptableObject에 동기화 |
| **StringManager** | Observer 패턴 기반 다국어 처리, 언어 변경 시 모든 UI 자동 갱신 |

---

## 팀원

| 이름 | 역할 |
|---|---|
| 전형찬 | 기획 팀장 |
| 이승희 | 기획 부팀장 |
| 강신혁 | 기획 |
| 김성현 | 기획 |
| 조용환 | 기획 |
| 한재욱 | 기획 |
| 한만성 | 프로그래밍 팀장 |
| 송민서 | 프로그래밍 부팀장 |
| 박혜주 | 프로그래밍 |
| 정예찬 | 프로그래밍 |
| 주민규 | 프로그래밍 |

---

### 요구 사항

- **Unity 6000.2.10f1**
- Windows, Android