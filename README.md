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

+ `server.py`
  + 커맨드 창에서 실행하면, 웹캠 장치의 유무를 판단하여 지정한 IP에서 **Listen** 상태가 된다.
  + 웹캠 화면을 UDP 통신이 전송할 수 있는 **최대 픽셀 단위**로 전송한다.
  + 여러 IP로부터 요청을 받을 수 있다.
+ `Drawer`
  + 초기에 **웹캠 화면**을 전송받기 위해 Listen 상태의 포트에 접근하는 과정을 거친다.
  + 또한, 입력이 발생한 좌표를 전송하는 패킷을 전송하기 위해 지정 IP에서 **Listen** 상태가 된다.
  + **페인트 툴**이 제공되고, 전송 받은 웹캠 화면에 **오버레이**한다.
+ `Drawed_HoloLens`
  + **Universal Windows Platform**에서 빌드된 솔루션 배포를 통해 홀로렌즈에 추가되는 프로젝트이다.
  + server.py와 Drawer이 Listen 상태일 때 요청하여, **웹캠 화면**과 **입력 좌표**를 전송받는다.
  + 입력 좌표를 받을 때마다, **Read/Write Enabled** 텍스처의 픽셀을 변경한다.

## 💊 개선 가능성
+ 본래 공간 인식 및 동작 인식이 가능한 홀로렌즈 내에서 웹캠 화면 통신과 Drawer 좌표 통신이 동시에 이뤄지다 보니, **프레임 드랍**이 심하게 나타났다.
+ 개선을 위해서는 **Drawer** 상의 화면만을 통신하여 부하를 줄여야 할 것으로 보인다.
