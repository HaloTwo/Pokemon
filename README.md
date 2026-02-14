# 🐉 Pokemon: Scarlet (Unity 3D)

GAME FREAK의 3D 오픈월드 RPG 게임, Pokemon: Scarlet 모작 프로젝트입니다.  
오픈월드 탐험 구조와 AI 시스템, 인벤토리 및 데이터 기반 게임 시스템 구현을 목표로 개발했습니다.

---

## 📌 Project Info
- 개발 인원 : 1인  
- 개발 기간 : 2023.06.19 ~ 2023.07.24 (5주)  
- 개발 환경 : C#, Unity3D, GitHub  

---

## 🎥 Gameplay Video
[![Watch the Gameplay](https://img.youtube.com/vi/s__GzKLjPf0/0.jpg)](https://www.youtube.com/watch?v=s__GzKLjPf0)

---

## 🧠 핵심 구현 요소
- 상태 패턴(State Pattern)을 이용한 야생 포켓몬 및 적 AI 구현  
- 전체 게임 시스템 구조 직접 설계 및 구현  
- 포켓몬 박스 및 인벤토리, 상점 시스템 구현  
- Unity New Input System 기반 플레이어 이동 구현  
- Cinemachine Camera를 활용한 카메라 시스템 구성  
- SoundManager를 통한 사운드 제어 구조 구현  
- UGUI 기반 UI 시스템 구현  
- Object Pool 시스템 구현  
- 싱글톤 패턴 기반 매니저 구조 설계  
- Json + ScriptableObject 기반 데이터 저장 구조  
- Terrain 기반 오픈월드 환경 구성 및 LOD 최적화  
- Occlusion Culling 적용  
- Light Map Bake 적용  
- Profiler 기반 성능 최적화

### 🔎 Code
- 🔊 Sound Manager (전역 오디오 시스템)
  https://github.com/HaloTwo/Pokemon/blob/main/Assets/3.Script/Manager/SoundManager.cs

- 🧬 Pokemon Stats Data Structure (캐릭터 데이터 설계)
  https://github.com/HaloTwo/Pokemon/blob/main/Assets/3.Script/Pokemon/PokemonStats.cs

- 🎒 Item Data System (아이템 데이터 구조)
  https://github.com/HaloTwo/Pokemon/blob/main/Assets/3.Script/ItemData.cs

---

## 📷 Screenshots
![Gameplay1](https://github.com/HaloTwo/Pokemon/assets/94442043/807d13ae-208c-49e1-876d-ec69b09b69a4)
![Gameplay2](https://github.com/HaloTwo/Pokemon/assets/94442043/d6cd855f-e7da-4b9a-a7e9-10bc868fc415)
![Gameplay3](https://github.com/HaloTwo/Pokemon/assets/94442043/2152681c-9d84-495a-89a7-301dae1de9b69a4)
