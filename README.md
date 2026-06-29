# 🏀 Double Clutch

<a href="https://youtu.be/mO2eYQ_3N3c">
    <img width="326" height="508"
         alt="image"
         src="https://github.com/user-attachments/assets/dbabbe84-98c5-496a-893a-13d17f7f3477" />
</a>

> 📺 이미지를 클릭하면 플레이 영상을 확인할 수 있습니다.

> 🚀 Google Play에서 실제 출시된 프로젝트입니다.

> 📱 https://play.google.com/store/apps/details?id=com.doubleclutch.high5manager&hl=ko

> 기획 파트와 협업하여 제작한 캐주얼 농구 육성 시뮬레이션 게임

---

# 📑 목차

- [📌 프로젝트 개요](#-프로젝트-개요)
- [🎮 게임 소개](#-게임-소개)
- [⚙ 주요 시스템](#-주요-시스템)
- [🛠 기술적 구현](#-기술적-구현)
- [🤔 기술적 고민 및 해결](#-기술적-고민-및-해결)
- [🤝 협업 경험](#-협업-경험)
- [📈 성과](#-성과)
- [💡 회고](#-회고)

---

# 📌 프로젝트 개요

| 항목    | 내용                      |
| ----- | ----------------------- |
| 프로젝트명 | Double Clutch           |
| 개발 기간 | 2026.02.09 ~ 2026.04.10 |
| 개발 인원 | 11명 (개발 5, 기획 6)                     |
| 담당 역할 | 클라이언트 개발                |
| 개발 환경 | Unity 6000.2.10f1       |
| 플랫폼   | 모바일 (안드로이드)                    |
| 장르    | 캐주얼 농구 육성 시뮬레이션         |

---

# 🎮 게임 소개

플레이어는 농구 팀의 감독이 되어

선수 육성, 경기 운영, 이벤트 선택을 통해

팀을 성장시키고 리그 우승을 목표로 합니다.

매 주차마다 다양한 선택이 발생하며

플레이어의 결정에 따라 팀의 성장 방향이 달라집니다.

---

# ⚙ 주요 시스템

## 캘린더 시스템

* 주차 단위 시즌 진행
* 경기 일정 관리
* 이벤트 발생 시점 제어
* 시즌 흐름 관리

<img width="800" height="450" alt="download-ezgif com-video-to-gif-converter" src="https://github.com/user-attachments/assets/24570e3a-0266-4a10-8d75-4fe30f1005e2" />


---

## 선수 배치 시스템

* 드래그 앤 드롭 방식 배치
* 포지션 선택 배치

<img width="800" height="450" alt="ezgif com-video-to-gif-converter" src="https://github.com/user-attachments/assets/399577a4-4d99-4fee-8999-6ef92fd32cef" />

<img width="800" height="450" alt="ezgif com-video-to-gif-converter" src="https://github.com/user-attachments/assets/395da808-19e7-495e-b488-343b34497bf2" />

---

## 인프라 시스템

* 훈련 시설 업그레이드
* 시설별 성장 효과 적용
* 팀 운영 자원 소비

<img width="800" height="450" alt="ezgif com-video-to-gif-converter (1)" src="https://github.com/user-attachments/assets/30fe38b3-a659-4d17-a43b-d96dcc3ee269" />

---

# 🛠 기술적 구현

## 1. 주차 기반 캘린더 시스템

### 문제

게임 진행이 길어질수록

일정 및 이벤트 관리 로직이 복잡해질 수 있었습니다.

### 해결

캘린더 시스템을 중심으로

주차 단위 일정과 이벤트를 관리하는 구조를 설계했습니다.

### 결과

* 일정 관리 일원화
* 시즌 흐름 제어 용이
* 신규 일정 확장 용이

```csharp
public void CalcWeek(int weekId, GameManager gm)
{
    var data = _calReader.DataList[weekId - 1];

    if (data.isSpecialWeek)
    {
        weekId = data.hasSeasonOut
            ? data.targetidSpecial
            : data.targetidDefault;
    }
    else
    {
        weekId = data.targetidDefault;
    }

    data = _calReader.DataList[weekId - 1];

    calendar.month = data.month;
    calendar.week = data.weekNo;

    gm.SetWeekId(weekId);

    OnWeekChanged?.Invoke(calendar);
}
```

---

## 2. 선수 배치 시스템

### 요구사항

기획 의도에 따라

* 선수를 원하는 포지션에 자유롭게 배치
* 드래그 앤 드롭 기반 라인업 구성
* 배치 결과를 경기력에 반영

할 수 있는 시스템이 필요했습니다.

### 구현

선수와 포지션을 연결하는 배치 시스템을 설계하고

드래그 앤 드롭 방식으로 선수를 배치할 수 있도록 구현했습니다.

또한 배치된 선수 정보를 기반으로

팀 전력과 경기 결과 계산에 활용할 수 있도록 구성했습니다.

### 결과

* 직관적인 선수 배치 경험 제공
* 포지션별 라인업 구성 지원
* 배치 결과를 경기 시스템에 반영
* 선수 육성 결과를 즉각적으로 확인 가능

```csharp
public bool AddOnPosition(PlayerCard card, DropPosition dPos)
{
    if (card == null || dPos == null) return false;

    int idx = GetSlotIndex(dPos);
    if (idx < 0) return false;

    if (card.Player != null)
    {
        card.Player.SetMatchPosition(dPos.GetPosition());
    }

    PlayerCard prevCard = _positionCards[idx];
    if (prevCard != null && prevCard != card)
    {
        _positionCards[idx] = null;
        MoveToCardList(prevCard);
    }

    _cardList.Remove(card);
    _positionCards[idx] = card;

    UpdateMatchStartUI();
    return true;
}
```

## 3. 인프라 시스템

### 요구사항

기획 파트에서는

* 시설 업그레이드 기능
* 시설별 고유 효과 적용
* 단계별 성장 시스템

을 요구했습니다.

### 구현

인프라 데이터를 시설별로 분리하고

레벨에 따라 효과가 적용되는 구조를 설계했습니다.

또한 업그레이드 가능 여부와 최대 레벨을 관리하여

시설 성장 과정을 단계적으로 진행할 수 있도록 구현했습니다.

### 결과

* 시설별 성장 시스템 구현
* 데이터 기반 인프라 관리
* 신규 시설 추가 용이
* 팀 성장과 자연스럽게 연계

```csharp
public void Upgrade()
{
    if (infra.currentLevel >= infra.maxLevel)
        return;

    int cost = GetCostByNextLevel();
    if (GameManager.Instance.SaveData.money < cost)
        return;

    GameManager.Instance.SetMoney(
        GameManager.Instance.SaveData.money - cost);

    infra.currentLevel++;
    Upgraded?.Invoke(infra.currentLevel);

    InfraManager.Instance.UpdateInfraLevel(infra);
}
```

---

# 🤔 기술적 고민 및 해결

## 왜 이벤트 기반 구조를 사용했는가?

육성 시뮬레이션 게임은

콘텐츠와 이벤트가 지속적으로 추가되는 특성이 있습니다.

시스템 간 직접 참조를 사용할 경우

기능이 늘어날수록 결합도가 증가할 수 있기 때문에

이벤트 기반 구조를 통해 시스템 간 의존성을 줄였습니다.

---


## 왜 DI를 적용했는가?

객체 생성 책임과 사용 책임을 분리하여

유지보수성과 테스트 용이성을 향상시키기 위해

Zenject 기반 DI를 적용했습니다.

---

# 🤝 협업 경험

## 기획 파트와의 협업

기획자가 이벤트와 콘텐츠를 반복적으로 수정하는 과정에서

구조 변경 없이 기능을 추가할 수 있는 시스템을 설계했습니다.

이를 통해

* 요구사항 변경 대응 속도 향상
* 기능 추가 비용 감소
* 협업 효율 향상

효과를 얻을 수 있었습니다.

---

# 📈 성과

* Zenject 기반 DI 구조 설계
* 이벤트 기반 게임 흐름 관리
* UI 모듈화 구조 구축
* 데이터 기반 일정 관리 시스템 구현

---

# 💡 회고

이번 프로젝트에서는

단순 기능 구현보다

* 확장 가능한 아키텍처 설계
* 시스템 간 결합도 감소
* 유지보수성 향상

에 집중했습니다.

특히 Zenject와 이벤트 기반 구조를 실제 프로젝트에 적용하며

'기능 구현'보다 '구조 설계'가 장기적인 프로젝트 품질에 큰 영향을 준다는 점을 체감할 수 있었습니다.
