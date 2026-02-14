# 🐉 Pokemon: Scarlet (Unity 3D)

Unity 기반으로 제작한 3D 오픈월드 RPG 시스템 구현 프로젝트입니다.  
탐험 중심 구조에서 AI 상태 전환, 인벤토리, 상호작용 시스템을  
시스템 단위로 설계하고 구현하는 것에 중점을 두었습니다.

---

## 🎥 Gameplay Video
[![Watch the Gameplay](https://img.youtube.com/vi/s__GzKLjPf0/0.jpg)]
(https://www.youtube.com/watch?v=s__GzKLjPf0)

---

## 🧠 Project Overview
본 프로젝트는 오픈월드 환경에서 플레이어 탐험과  
AI 상호작용이 자연스럽게 동작하도록 설계한 RPG 프로토타입입니다.

단순 기능 구현이 아닌,
- AI 구조 설계
- 시스템 단위 기능 구현
- 데이터 기반 구조  
를 중심으로 개발했습니다.

---

## ⚙️ Core Implementation

### 🧠 AI System (State Pattern)
- 상태 패턴(State Pattern) 기반 AI 구조 설계
- 상황에 따른 상태 전환 로직 구현 (Idle / Patrol / Chase 등)
- 플레이어 탐지 및 행동 전환 시스템 구현
- 유지보수와 확장을 고려한 AI 구조 설계

### 🎒 Inventory & Shop System
- 인벤토리 시스템 직접 구현
- 아이템 데이터 관리 구조 설계
- 상점 구매/판매 기능 구현
- UI와 데이터 연동 구조 설계

### 🌍 Open World & Exploration
- Terrain 기반 3D 오픈월드 환경 구성
- 플레이어 탐험 중심 게임 흐름 구현
- 오브젝트 상호작용 시스템 구현

### 📊 Data Structure Design
- 시스템 확장을 고려한 데이터 구조 설계
- 게임 데이터와 로직 분리 구조 적용
- 유지보수를 고려한 모듈화 설계

---

## 🚀 Technical Focus
이 프로젝트에서 가장 집중한 부분은  
단순한 기능 구현이 아닌 “시스템 구조 설계”였습니다.

- AI, 인벤토리, 상호작용 기능을 독립적인 시스템 단위로 분리
- 기능 추가 시 기존 구조를 수정하지 않도록 설계
- 오픈월드 환경에서의 시스템 확장성을 고려한 구조 구현

---

## 🛠 Development Environment
- Engine: Unity3D
- Language: C#
- Genre: 3D Open World RPG
- Role: Solo Developer (System / AI / Gameplay)
