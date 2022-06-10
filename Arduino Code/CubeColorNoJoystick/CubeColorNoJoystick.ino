#include <Wire.h>

#define colorUpButton 2
#define colorDownButton 4
#define modeUpButton 7
#define modeDownButton 12
#define joystickX 1
#define joystickY 0
#define joystickButton 13

//Interval between chisel strikes - delay lags Unity
unsigned long currentMillis = 0;
unsigned long previousMillis = 0;

const long interval = 700;

//Array for buttons, to be used in for-loops
int buttons[] = {colorUpButton, colorDownButton, modeUpButton, modeDownButton, joystickButton};

//Array to check if buttons are already pressed (to avoid sending multiple commands)
bool buttonPressed[] = {true, true, true, true, true};

//Integers for the tilt of the joystick
int joyX, joyY;

//Variables for the accelerometer
int accel = 0x53;
int x, y, z;

void setup() {  
  //Set up the pin buttons. Set to HIGH to avoid pull-up resistors fighting
  for (int i = 0; i < (sizeof(buttons) / sizeof(buttons[0])); i++)
  {
      pinMode(buttons[i], INPUT);
      digitalWrite(buttons[i], HIGH);
  }

  //Start serial communication to send commands to Unity
  Serial.begin(9600);

  //Start communication with accelerometer
  Wire.begin();
  Wire.beginTransmission(accel);
  Wire.write(0x2D);
  Wire.write(8);
  Wire.endTransmission();
}

void loop() {  
  //Check whether button is pressed or released this frame
  for (int i = 0; i < (sizeof(buttons) / sizeof(buttons[0])); i++)
  {
    int buttonState = CheckButtonPress(buttons[i], i);

    if (buttonState != 0)
      Serial.println(String(buttonState) + ButtonID(i));
  }

  //Check if chisel cooldown has worn off
  currentMillis = millis();

  if (currentMillis - previousMillis < interval)
    return;

  //if player swings with more than approx 3g
  if (CheckForce() > 600)
  {
    Serial.println("3R");
    previousMillis = currentMillis;
  }
}

//Check accelerometer force
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

//Check if button was pressed (1) or released (2) this frame
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

//Assign a character to each button for Unity to read
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
