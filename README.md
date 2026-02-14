# Pokemon: Scarlet (Unity)

Unity로 제작한 3D 오픈월드 RPG 프로젝트입니다.  
오픈월드 환경에서 **AI 상태 전환**, **데이터 기반 시스템(인벤토리/상점)**, **탐험 흐름**을 구현하는 것을 목표로 진행했습니다.

- Engine: Unity3D (C#)
- Genre: 3D Open World RPG (Prototype)
- Focus: AI(State Pattern), System Design, Data Management, Optimization

🎥 [YouTube 동영상](https://www.youtube.com/watch?v=s__GzKLjPf0)

---

## 📌 Project Overview
이 프로젝트는 오픈월드에서 플레이어가 맵을 탐험하며 상호작용하고,  
야생 몬스터(또는 적)가 상황에 맞춰 행동하도록 만드는 데 집중했습니다.

특히 아래 3가지를 “시스템 단위”로 정리하는 것을 목표로 했습니다.
- **AI 구조**: 상태 패턴 기반으로 행동 전환(탐색/추적/전투/이탈 등)
- **게임 시스템**: 인벤토리/상점 등 데이터 기반 기능 구현
- **성능**: Terrain 기반 월드에서 LOD / Occlusion Culling 등 최적화 적용

---

## 🧠 Key Features

### 1) AI (State Pattern)
야생 몬스터/적이 상황에 따라 자연스럽게 행동하도록 **State Pattern 기반 AI**를 구현했습니다.

- 기본 상태 예시: Idle / Patrol / Chase / Attack / Return
- 상태 전환 조건을 명확히 분리하여, 기능 추가/수정 시 유지보수 부담을 낮춤
- 전투 진입/이탈, 거리 기반 전환 등 “게임플레이 상황” 중심으로 설계

### 2) Inventory / Shop System
오픈월드 RPG에서 필수적인 **인벤토리 및 상점 시스템**을 구현했습니다.

- 아이템 데이터 구조를 분리하여 시스템 확장 가능하도록 구성
- 상점 구매/판매 플로우 구현
- UI 갱신은 이벤트 기반으로 처리하여 불필요한 갱신 비용 최소화

### 3) Data Management
데이터 수정/확장에 유리하도록 **데이터 중심 구조**로 설계했습니다.

- 아이템/상점 데이터 분리 관리
- (선택) ScriptableObject 기반 구성 / 또는 JSON 기반 로드 구조로 확장 가능한 형태로 정리

### 4) World & Optimization
오픈월드 환경에서의 성능을 고려해 최적화를 적용했습니다.

- Terrain 기반 월드 구성
- LOD 적용
- Occlusion Culling 적용
- (필요 시) 오브젝트 풀링 적용 포인트 고려

---

## 🛠 What I Focused On
이 프로젝트에서 가장 신경 쓴 부분은 “기능 구현” 자체보다,  
**추가 구현이 계속 들어와도 구조가 무너지지 않게 만드는 것**이었습니다.

- AI와 시스템을 **역할/책임 단위로 분리**
- 데이터는 **데이터/로직/UI**를 분리해서 관리
- 오픈월드 환경에서 **성능 리스크**를 의식하고 최적화 적용

---

## 📷 Screenshots
(아래 이미지)

![123](https://github.com/HaloTwo/Pokemon/assets/94442043/807d13ae-208c-49e1-876d-ec69b09b69a4)
![1245](https://github.com/HaloTwo/Pokemon/assets/94442043/d6cd855f-e7da-4b9a-a7e9-10bc868fc415)
![1234](https://github.com/HaloTwo/Pokemon/assets/94442043/2152681c-9d84-495a-89a7-301dae1de9b6)

---

## ✅ Next Steps (Optional)
- 전투 로직 확장(스킬/상태이상/어그로 등)
- AI 패턴 다양화(군집 행동, 시야/청각 기반 탐지 등)
- 데이터 테이블/툴링 개선(에디터 툴 추가)
