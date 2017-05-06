#include <Servo.h>
#define ECHOPIN 10
#define TRIGPIN 11
#define SOFTWAREHIGH 12

Servo mainServo;
Servo secondServo;

float getDistance();
void rotating();

bool start = false;
int delayC=10;

void setup()
{
Serial.begin(9600);
pinMode(SOFTWAREHIGH,OUTPUT);
digitalWrite(SOFTWAREHIGH,HIGH);
pinMode(ECHOPIN, INPUT);
pinMode(TRIGPIN, OUTPUT);

mainServo.attach(9);
secondServo.attach(8);
secondServo.write(3);
}

void loop()
{
  if(Serial.available()>0){
    int val = Serial.parseInt();

    if(val==-1){
      start=true;
      mainServo.write(0);
      delay(300);
    }
    else if(val==-2){
      start =false;  
    }
    else if(val>0){
      delayC=val;
    }
    
  }
  if(start == true){
    rotating();
  }
  
}

void rotating(){  
  for(short i = 0;i<=180;i++){
    mainServo.write(i);  
    delay(delayC);
    float dis = getDistance(); 
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = -1;
    }    
    String answer = String(dis);
    answer+="*";
    answer+=String(i);
    answer+="\n";
    Serial.print(answer); 
  }

  for(short i = 180;i>=0;i--){
    mainServo.write(i);  
    delay(delayC); 
        float dis = getDistance(); 
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = getDistance();
    }
    if(dis > 500){
       dis = -1;
    }    
    String answer = String(dis);
    answer+="*";
    answer+=String(i);
    answer+="\n";
    Serial.print(answer); 
  }
  }

float getDistance(){
  digitalWrite(TRIGPIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIGPIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIGPIN, LOW);
  
  float distance = pulseIn(ECHOPIN, HIGH);
  distance= distance*0.017315f;
  return distance;
 }
