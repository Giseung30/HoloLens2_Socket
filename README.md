# HoloLens2 Socket

## 📣 프로젝트 목적
+ **의료용 목적**으로 활용할 수 있는 홀로렌즈 프로그램을 만들기 위해 기획했다.
+ 홀로렌즈 사용자는 **웹캠을 활용**하여 제 3자의 의사를 확인할 수 있다.

## ⚙ 개발 환경
+ 개발툴 : `2019.4.26f1`
+ 하드웨어 : `Microsoft HoloLens2`
+ MR SDK : `Mixed Reality Toolkit`
+ 모델링 수정 : `Blender`

## 🔍 구동 원리
<div align="center">
  <img width="75%" height="75%" src="https://user-images.githubusercontent.com/60832219/211207490-f78020dc-14c2-44bd-9f38-5a9d512194cb.png"/>
</div>


## 💊 개선 가능성
+ 본래 공간 인식 및 동작 인식이 가능한 홀로렌즈 내에서 웹캠 화면 통신과 Drawer 좌표 통신이 동시에 이뤄지다 보니, **프레임 드랍**이 심하게 나타났다.
+ 개선을 위해서는 **Drawer** 상의 화면만을 통신하여 부하를 줄여야 할 것으로 보인다.
