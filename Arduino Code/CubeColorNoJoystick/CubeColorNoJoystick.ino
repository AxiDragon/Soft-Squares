#include <Wire.h>

#define colorUpButton 2
#define colorDownButton 4
#define modeUpButton 7
#define modeDownButton 12
#define joystickX 1
#define joystickY 0
#define joystickButton 13

unsigned long currentMillis = 0;
unsigned long previousMillis = 0;

const long interval = 700;

int buttons[] = {colorUpButton, colorDownButton, modeUpButton, modeDownButton, joystickButton};
int analogPins[] = {joystickX, joystickY};

bool buttonPressed[] = {true, true, true, true, true};

int joyX, joyY;

int accel = 0x53;
int x, y, z;

void setup() {  
  for (int i = 0; i < (sizeof(buttons) / sizeof(buttons[0])); i++)
  {
      pinMode(buttons[i], INPUT);
      digitalWrite(buttons[i], HIGH);
  }
  
  Serial.begin(9600);

  Wire.begin();
  Wire.beginTransmission(accel);
  Wire.write(0x2D);
  Wire.write(8);
  Wire.endTransmission();
}

void loop() {  
  for (int i = 0; i < (sizeof(buttons) / sizeof(buttons[0])); i++)
  {
    int buttonState = CheckButtonPress(buttons[i], i);

    if (buttonState != 0)
      Serial.println(String(buttonState) + ButtonID(i));
  }
    
  currentMillis = millis();

  if (currentMillis - previousMillis < interval)
    return;
 
  if (CheckForce() > 600  )
  {
    Serial.println("3R");
    previousMillis = currentMillis;
  }
}

void CheckJoystick(int pinNumber){
  int tilt = CheckTilt(analogRead(pinNumber));
  char axis = 'O';
  bool newTilt = false;

  switch (pinNumber)
  {
    case 0:
      axis = 'Y';
      if (tilt != joyY)
      {
        newTilt = true;
        joyY = tilt;
      }
      break;
    case 1:
      axis = 'X';
      if (tilt != joyX)
      {
        newTilt = true;
        joyX = tilt;
      }
      break;
  }

  if (newTilt)
    Serial.println(String(tilt) + axis);
}

int CheckTilt(int result){
  if (result > 950)
    return 5;
  if (result > 600)
    return 4;
  if (result > 400)
    return 3;
  if (result > 150)
    return 2;

    return 1;
}
float CheckForce(){  
  Wire.beginTransmission(accel);
  Wire.write(0x32);
  Wire.endTransmission(false);
  Wire.requestFrom(accel, 6);
  
  x = (Wire.read() | Wire.read() << 8);
  x = abs(x);
  y = (Wire.read() | Wire.read() << 8);
  y = abs(y);
  z = (Wire.read() | Wire.read() << 8);
  z = abs(z);

  return x + y + z - 256;
}

int CheckButtonPress(int pinNumber, int arrayNumber) {
  if (!digitalRead(pinNumber))  {
    if (buttonPressed[arrayNumber])
    {
      buttonPressed[arrayNumber] = false;
      return 1;
    }    
    buttonPressed[arrayNumber] = false;
  }
  
  if (!digitalRead(pinNumber) || buttonPressed[arrayNumber])
    return 0;

  buttonPressed[arrayNumber] = true;
    return 2;
}

char ButtonID(int id){
  //C bottom, D top, J right, M left
  switch (id)
  {
    case 0:
      return 'C';
      break;
    case 1:
      return 'D';
      break;
    case 2:
      return 'M';
      break;
    case 3:
      return 'J';
      break;
    case 4:
      return 'P';
      break;
  }
}
